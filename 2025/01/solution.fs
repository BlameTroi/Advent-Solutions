\ solution.fs -- AoC 2025 01 Secret Entrance -- T.Brumley.

\ Part one was trivial, but part two stumped a lot of people.
\ You may consider me to be part of the "lot". A bunch of
\ debug code has been pulled out and a set of test cases
\ are in tests.fs.

\ Common utility definitions: ---------------------------------

\ This is a version of read-line that eats blank lines. Its
\ interface is the same as read-line.

: read-line-skip-empty ( c-addr u1 fd -- u2 flag ior )
  { ubuf ulen ufd }
  begin
    ubuf ulen ufd read-line { rlen rgot rior }
          rior if true else
       rgot 0= if true else
               rlen then then
  until
  rlen rgot rior ;

\ Copy a string to a counted string. I could have just copied
\ the s-string to a variable and the length into another, but
\ this was more fun.

: s$>c$ ( s-addr s-u c-addr c-u -- )
   2dup erase                      \ clear work, s su c cu
   1- swap 1+ swap                 \ s u c1 u1 adjust for count
   rot min                         \ s c m
   2dup swap 1- c!                 \ s c m count in dest
   move ;                          \ and move


\ Problem related definitions: --------------------------------

\ Parse ^[LR]d+$ to find the magnitude of a turn of the dial
\ (d+$) and its direction ([Ll] = left = -1).

: parse-input ( s-addr u -- n )
  { str len } 1 { dir }
  str c@ 'l' = if -1 to dir then
  str c@ 'L' = if -1 to dir then
  str char+ len 1- 0 0 2swap >number 2drop drop
  dir swap * ;

\ Normalize the dial. IE, bring it back within the range
\ of [0, 100).

: normalize-dial ( n -- u )
  dup 0 < if 100 + else
  dup 99 > if 100 - then then
  100 mod ;

\ Do we end up on zero?

: calculate-one  ( u1 u2 --- n , 1 if ended on zero )
  + normalize-dial abs 100 mod if 0 else 1 then ;

\ How many clicks "touch" zero? This includes a stop at but not
\ a start from zero.

\ works but is pooooooorly written
: calculate-two  ( u1 u2 -- n , how many times at zero )
  { dial clicks } clicks abs 100 /mod { part full }
  dial clicks + normalize-dial { normed }
  normed 0= if
    part 0= if normed full exit
    else full 1+ exit then
  then
  part 0= if full exit then
  normed 0= if full exit then
  full 0= if normed 0= if 1 then then
  clicks 0< if \ 55 55
    normed part + 100 > if 1 else 0 then
  else
    normed part - 0 < if 1 else 0 then
  then
  full + ;

\ Problem state: ----------------------------------------------

\ Definitions prior to this should be pure functions, or as
\ pure as an i/o operation can be.

create in-fn 256 chars allot
80 constant in-max   variable in-len
create in-buf in-max 2 chars + allot
0 value in-fd

variable dial      variable clicks    variable next-dial
variable part-one  variable part-two

\ Driver for the solution. Use run-live/test interactively to
\ avoid typing long paths.

: solve ( s" input.txt" -- )

  \ Get input file from s" path", persist, and open the file.
  in-fn 256 s$>c$  in-fn count  r/o open-file throw  to in-fd

  50 dial ! 0 part-one ! 0 part-two !

  begin
    in-buf in-max in-fd read-line-skip-empty
    throw  swap  in-len !         \ leaves eof flag
  while
    in-buf in-len @  parse-input  clicks !

    \ The dial after the clicks. Dial and clicks are passed to
    \ the calculators. While not strictly needed for part one
    \ they are for part two.
    dial @ clicks @ + normalize-dial next-dial !

    \ Part one: count the number of times we stop at zero.
    dial @ clicks @ calculate-one
    part-one @ + part-one !

    \ Part two: count the number of times a click touches zero.
    \ This includes stopping on zero but not starting from
    \ zero.
    dial @ clicks @ calculate-two
    part-two @ + part-two !

    next-dial @ dial !
  repeat

  \ And out.
  in-fd close-file throw
  cr cr ." 2025 day 1 part 1 answer: "
  part-one @ 6 .r
  cr ."            part 2 answer: "
  part-two @ 6 .r cr
  ;

: run-test
  s" ../../../Advent-Data/2025/01/test.txt" solve ;

: run-live
  s" ../../../Advent-Data/2025/01/live.txt" solve ;

\ End of solution.fs
