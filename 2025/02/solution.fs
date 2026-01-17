\ solution.fs -- AoC 2025 02 GIft Shop -- T.Brumley.


\ 1-22,95-115,998-1012,1188511880-1188511890,222220-222224,
\ 1698522-1698528,446443-446449,38593856-38593862,
\ 565653-565659,824824821-824824827,2121212118-2121212124
\
\ Input is a single line of ranges.
\
\ Invalid patterns:
\
\ * Repeated substrings (55, 6464, 123123 are all invalid).
\ * May not start with 0.
\
\ Your job is to find all of the invalid IDs that appear in the
\ given ranges. In the above example:
\
\     11-22 has two invalid IDs, 11 and 22.
\     95-115 has one invalid ID, 99.
\     998-1012 has one invalid ID, 1010.
\     1188511880-1188511890 has one invalid ID, 1188511885.
\     222220-222224 has one invalid ID, 222222.
\     1698522-1698528 contains no invalid IDs.
\     446443-446449 has one invalid ID, 446446.
\     38593856-38593862 has one invalid ID, 38593859.
\     The rest of the ranges contain no invalid IDs.
\
\ Adding up all the invalid IDs in this example produces
\ 1227775554.

\ globals

\ input for day 2 shoud not exceed 42 +2 bytes per range
create in-fn 256 chars allot
80 constant in-max   variable in-len
create in-buf in-max 2 chars + allot
0 value in-fd
false value in-eof

\ Common utility definitions: ---------------------------------


\ Helpfulish debug trace.

false value do.diag.s
: diag.s ( c-addr u -- , print .s with tag )
  if do.diag.s cr type space .s else 2drop then ;

\ Reading the file as bytes r/o bin. A return of 0 0 from here
\ means logical end of file.
\
\ Assuming that HERE will not move across a call from this to
\ READ-FILE.

: read-file-next-byte ( -- c f/u )
  0 here c! here 1  in-fd read-file throw  here c@ swap ;

\ Copy a string to a counted string. I could have just copied
\ the s-string to a variable and the length into another, but
\ this was more fun.

: s$>c$ ( s-addr s-u c-addr c-u -- )
   2dup erase                      \ clear work, s su c cu
   1- swap 1+ swap                 \ s u c1 u1 adjust for count
   rot min                         \ s c m
   2dup swap 1- c!                 \ s c m count in dest
   move ;                          \ and move

\ Given an s string run it through >NUMBER. The return is
\ identical to >NUMBER.

: parse->number$ ( c-addr1 u1 -- c-addr2 u2 ud )
  0 0 2swap >number ;

\ A range is a string of two numbers separated by a dash. While
\ >NUMBER deals in doubles I drop them to single cells. That's
\ 64 bits which I hope is wide enough.

: parse-range$ ( s-addr u -- s-addr u lo hi )
  parse->number$ 2swap drop >r     ( stash lo )
  1- swap 1+ swap                  ( len -1 addr +1 for '-' )
  parse->number$ 2swap drop
  r> swap ;                        ( set return add u lo hi )

\ A bad part id is one made up of two pairs of repeating numbers.
\ So any even number of digits where the first len/2 digits are
\ the same as the second len/2 digits.
\
\ any odd length is automatically valid
\ 99 -> 9|9 -> 9=9 so invalid
\ 123124 -> 123|124 -> 123<>124 so valid
\
\ yes there is a lot of redundant swizzling here.

: ?bad-id-one$ ( c-addr u -- f )
  dup 1 and if 2drop false exit then \ odd length?
  1 rshift                        \ half length c1 u1
  2dup +                          \ half length second half
  over                            \ c1 u1 c2 u2
  compare 0= ;

: ?bad-id-two$
  \ we'll have to loopeth 1 2 3 ... half, watch for odd
  2drop false ;

\ Given an unsigned single, is it a valid part id? The actual
\ rule is in ?valid-id$. Convert ud to string and then check
\ validity.

create $id 48 allot
: ?bad-id-one ( u -- f )              \ sign ignored
  $id 48 blank                    \ as string in safe location
  0 <# #s #> dup >r               \ addr u, r: u
  $id swap move $id r>            \ addr u
  ?bad-id-one$ ;

: ?bad-id-two ( u -- f )
  $id 48 blank
  0 <# #s #> dup >r
  $id swap move $id r>
  ?bad-id-two$ ;

\ A range of part ids is a string s" ll-hh,". Convert this to a
\ pair integers and then iterate over them checking each in the
\ range to see if it is invalid. Accumulates a sum of the bad
\ ids.

: check-range-one ( lo hi -- sum )
  1+ swap 0 -rot do           ( sum hi lo )
    i ?bad-id-one if i + then
  loop ;

: check-range-two ( lo hi -- sum )
  1+ swap 0 -rot do           ( sum hi lo )
    i ?bad-id-two if i + then
  loop ;

\ : check-range$ ( s" ll-hh," -- flag )
\   2dup cr ." # " type cr
\   \ more to come
\   2drop
\   \
\   false ;


\ Problem state: ----------------------------------------------

create scratch 256 allot
variable range-begin  variable range-end
variable part-one     variable part-two

: read-to-comma ( c-addr u -- c-addr2 u2 )
  swap ( better ordering )  ( u ca )
  begin
    dup 0 swap c!       ( u ca -- clear next byte )
    read-file-next-byte ( -- c f/u , 0 0 on eof )
    2dup = >r           ( c=u=0 is eof )
    over ',' =          ( comma )
    r>  or  0=          ( eof or comma = out )
  while
    drop                ( not needed u )
    over c!             ( store )
    swap 1-             ( I'm not checking this )
    swap 1+             ( or this )
  repeat
  0 = = to in-eof       ( if 0 = c = u, remember eof )
  swap ;                ( back in caller order )

\ Read the next range from input. low-hi,low-hi<eof> or \n<eof>.
\ Ranges stored in variables.

: get-next-range ( -- f )
  scratch 256 2dup 0 fill
  read-to-comma
  256 swap -       ( actual length used )
  >r
  drop
  scratch r>
  scratch c@ 0<> if
    parse-range$ ( s u lo hi )
    range-end !
    range-begin !
    2drop true
  else
    1 range-begin ! 0 range-end ! \ leave invalid
    2drop
    false
  then ;

\ Driver for the solution. Use run-live/test interactively to
\ avoid typing long paths.

: solve ( s" input.txt" -- )

  \ Get input file from s" path", persist, and open the file.
  in-fn 256 s$>c$
  in-fn count  r/o bin open-file  throw  to in-fd

  0 part-one ! 0 part-two !

  begin
    in-eof if false else get-next-range then
  while
    range-begin @ range-end @ check-range-one
    part-one @ + part-one !
    range-begin @ range-end @ check-range-two
    part-two @ + part-two !
  repeat

  \ And out.
  in-fd close-file throw
  cr cr ." 2025 day 2 part 1 answer: "
  part-one @ 32 .r
  cr ."            part 2 answer: "
  part-two @ 32 .r cr
  ;

: run-test
  s" ../../../Advent-Data/2025/02/test.txt" solve ;

: run-live
  s" ../../../Advent-Data/2025/02/live.txt" solve ;

\ End of solution.fs
