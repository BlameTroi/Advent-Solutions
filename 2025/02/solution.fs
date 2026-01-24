\ solution.fs -- AoC 2025 02 GIft Shop -- T.Brumley.

BASE @
DECIMAL

require ../io.fs
require ../parsing.fs
require ../strings.fs
require ../stack.fs

\ Problem: ----------------------------------------------------

\ Input is a single line of ranges separated by commas. A
\ range is "low-high". The problem refers to the numbers as
\ "part ids." For both parts of the problem we find and sum any
\ "part ids" that are invalid as identified by some digit
\ patterns.
\
\ For part one, the only invalid pattern is the first and last
\ half of the id are equal: 55, 6464, 123123 are invalid. No
\ ID with an odd number of digits can be invalid in part one.
\
\ For part two, there are more invalid patterns but they all
\ a really just variations on the pattern for part one. For
\ every evenly divisible group of digits in the id, are the
\ groups equal?
\
\ 123123 is invalid
\ 111 is invalid
\ 123123123 is invalid
\ 123123124 is valid
\ 111111 is invalid, and will be caught at 111 111, but it
\        could also be caught at 11 11 11 and 1 1 1 1 1 1 1.
\
\ The tests for validity are done from largest number of digits
\ in the group to the smallest. So while 22222222 is made up of
\ four groups of two digits, it will fail the validity test at
\ the test for two groups of four digits.
\
\ In the data, the longest run of digits is 10. Since the
\ possible pattern arrangements are fixed for a length a
\ case structure makes sense.
\
\ length     # sequences    sequence lengths
\   10           2 5           5 2
\    9           3             3
\    8           2 4           4 2
\    7           7             1
\    6           2 3           3 2
\    5           5             1
\    4           2             2
\    3           3             1
\    2           2             1
\    -- 1 unclear from spec but they don't count --
\    -- 0 not possible --


\ Globals: ----------------------------------------------------

variable range-begin  variable range-end
create range$ 32 chars allot
variable part-one     variable part-two


\ Common utility definitions: ---------------------------------


\ Input is a single line of ranges separated by commas. There
\ may or may not be a LF at the end of the file.

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


\ Mainline: ---------------------------------------------------

\ Driver for the solution. Use run-live/test interactively to
\ avoid typing long paths.

: add-one part-one +! ;
: add-two part-two +! ;

: solve ( s" input.txt" -- )
  open-input
  0 part-one ! 0 part-two !
  begin
    in-eof if false else get-next-range then
  while
    range-end @ 1+ range-begin @ do
      i range$ 32 u>$
      dup ( len ) case
        10 of 2dup ?2-sub$-equal if i dup add-one add-two 2drop
              else drop 2 5 ?n-sub$-equal if i add-two then
              then endof
         9 of 2dup drop 3 3 ?n-sub$-equal if i add-two 2drop
              else ?all-same-char$ if i add-two then
              then endof
         8 of 2dup ?2-sub$-equal if i dup add-one add-two 2drop
              else drop 2 4 ?n-sub$-equal if i add-two then
              then endof
         7 of ?all-same-char$ if i add-two then endof
         6 of 2dup ?2-sub$-equal if i dup add-one add-two 2drop
              else drop 2 3 ?n-sub$-equal if i add-two then
              then endof
         5 of ?all-same-char$ if i add-two then endof
         4 of ?2-sub$-equal if i dup add-one add-two then endof
         3 of ?all-same-char$ if i add-two then endof
         2 of ?all-same-char$ if i dup add-one add-two then endof
         1 of ( a run from 3-9 in live ignored ) 2drop endof
         ( default of ) ." error bad id length " . i . abort"  fail" ( endof )
      endcase
    loop
  repeat

  close-input
  cr cr ." 2025 day 2 part 1 answer: "
  part-one ?
  cr ."            part 2 answer: "
  part-two ? cr
  ;

\ Save some typing:

: run-test
  s" ../../../Advent-Data/2025/02/test.txt" solve ;

: run-live
  s" ../../../Advent-Data/2025/02/live.txt" solve ;

BASE !

\ End of solution.fs
