\ solution.fs -- AoC 2025 02 GIft Shop -- T.Brumley.

require ../common.fs

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

\ Globals: ----------------------------------------------------

variable range-begin  variable range-end
create range$ 32 chars allot
variable part-one     variable part-two

\ Common utility definitions: ---------------------------------

\ A bad part id is one made up of two pairs of repeating numbers.
\ So any even number of digits where the first len/2 digits are
\ the same as the second len/2 digits.
\
\ any odd length is automatically valid
\ 99 -> 9|9 -> 9=9 so invalid
\ 123124 -> 123|124 -> 123<>124 so valid
\
\ yes there is a lot of redundant swizzling here.

\ length     sequences     lengths
\   10           2 5        5 2
\    9           3          3
\    8           2 4        4 2
\    7           7          1
\    6           2 3        3 2
\    5           5          1
\    4           2          2
\    3           3          1
\    2           2          1
\    -- 0 not possible --
\
\ : cs1 CASE 1 OF 111 ENDOF
\   2 OF 222 ENDOF
\   3 OF 333 ENDOF
\   >R 999 R>
\   ENDCASE
\
\    c-addr  u   sequences
\
\

\ ?compare-adjacent$ ( c-addr u -- f )
\ ?compare-adjacent$ ( c-addr u -- f )

: ?2-sub$-equal ( c-addr u -- f )
  2dup 2 ?n-sub$ if              ( c u u2/ )
    swap drop                    ( c u2/ )
    ?adjacent$-equal             ( f )
  else
    2drop drop false
  then ;

: ?3-sub$-equal ( c-addr u -- f )
  2dup 3 ?n-sub$ if
    swap drop                   ( c u3/ )
    2dup
    ?adjacent$-equal            ( c u3/ f )
    cr .s
    if
      swap drop 2dup ?adjacent$-equal
    else
      false
    then
    2drop
  else
    2drop false
  then ;

: ?4-sub$-equal ( c-addr u -- f )
  2dup 4 ?n-sub$ if
    2drop false
  else
    2drop false
  then ;

: ?5-sub$-equal ( c-addr u -- f )
  2dup 5 ?n-sub$ if
    2drop false
  else
    2drop false
  then ;


\ A range of part ids is a string s" ll-hh,". Convert this to a
\ pair integers and then iterate over them checking each in the
\ range to see if it is invalid. Accumulates a sum of the bad
\ ids.

\ An invalid id is one that is made up a repeating sequence of
\ digits. Note that 11111111 is invalid because 1 repeats 8
\ times and 11 repeats 4 times and 1111 repeats twice.
\
\ Start at 2 halfs and work down. The check can be optimized a
\ bit by checking the last digit of the first sequence with the
\ last of the ID. If not equal, try next possibility.

\ Input is a single line of ranges separated by commas. There
\ may or may not be an LF at the end of the file.

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

\ Read the next range from input and store the ends [] in
\ variables. Format of input is: low-hi,low-hi<eof> or \n<eof>.

create $scratch 256 allot
: get-next-range ( -- f )
  $scratch 256 2dup 0 fill
  read-to-comma
  256 swap -       ( actual length used )
  >r
  drop
  $scratch r>
  $scratch c@ 0<> if
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
  open-input
  0 part-one ! 0 part-two !
  begin
    in-eof if false else get-next-range then
  while
    range-end @ 1+ range-begin @ do     ( -- )
      i range$ 32 u>$
      2dup ?2-sub$-equal if            ( c-a u )
        i part-one @ + part-one !
        i part-two @ + part-two !
        2drop
      else                              ( c-a u )
        2dup ?all-same-character if
          i part-two @ + part-two !
          2drop
        else                            ( c-a u )
          2drop
        then
      then
    loop
  repeat
  \ And out.
  close-input
  cr cr ." 2025 day 2 part 1 answer: "
  part-one ?
  cr ."            part 2 answer: "
  part-two ? cr
  ;

: run-test
  s" ../../../Advent-Data/2025/02/test.txt" solve ;

: run-live
  s" ../../../Advent-Data/2025/02/live.txt" solve ;

\ End of solution.fs
