\ solution.fs -- AoC 2025 01 Secret Entrance -- T.Brumley.

132 constant in-max \ override default in common.
require ../common.fs

\ Part one was trivial, but part two stumped a lot of people.
\ You may consider me to be part of the "lot". A bunch of
\ debug code has been pulled out and a set of test cases
\ are in tests.fs.

\ Problem state: ----------------------------------------------

variable dial      variable clicks    variable next-dial
variable part-one  variable part-two

\ Helpers: ----------------------------------------------------

\ Parse ^[LR]d+$ to find the magnitude of a turn of the dial
\ (d+$) and its direction ([Ll] = left = -1).

: parse-direction$ ( c-addr -- n , -1 for left, 1 for right )
  c@ dup 'l' = if drop -1 else
         'L' = if      -1 else
                        1 then then ;

: parse-input$ ( c-addr u -- n )
  over parse-direction$ >r
  1- swap char+ swap parse->number$ 2drop drop
  r> * ;

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

\ Driver: -----------------------------------------------------

\ Driver for the solution. Use run-live/test interactively to
\ avoid typing long paths.

: solve ( s" input.txt" -- )

  open-input

  50 dial ! 0 part-one ! 0 part-two !

  begin
    in-buf in-max read-line-skip-empty  ( -- ulen flag t=got )
  while
    in-buf swap  parse-input$  clicks !

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
  drop

  in-fd close-file throw
  cr cr ." 2025 day 1 part 1 answer: "
  part-one @ 6 .r
  cr ."            part 2 answer: "
  part-two @ 6 .r cr ;

: run-test
  s" ../../../Advent-Data/2025/01/test.txt" solve ;

: run-live
  s" ../../../Advent-Data/2025/01/live.txt" solve ;

\ End of solution.fs
