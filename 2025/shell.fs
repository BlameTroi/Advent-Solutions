\ shell.fs -- AoC 2025 01 solution shell -- T.Brumley.

BASE @
DECIMAL

132 constant in-max \ override default in io.fs
require ../io.fs
require ../parsing.fs
require ../strings.fs
require ../stack.fs

\ Problem: ----------------------------------------------------

\ For part one, a safe dial starts at 50. The dial is numbered
\ 0-99. Input are directions such as L123 or R15.
\
\ For part one, count the number of times the dial stops at 0
\ after a turn.
\
\ For part two, count the number of times the dial touches 0.
\ This includes stops.


\ Problem state: ----------------------------------------------

variable part-one  variable part-two


\ Helpers: ----------------------------------------------------



\ Driver: -----------------------------------------------------

\ Driver for the solution. Use run-live/test interactively to
\ avoid typing long paths.

: solve ( s" input.txt" -- )

  open-input

  50 dial ! 0 part-one ! 0 part-two !

  begin
    in-buf in-max read-line-skip-empty  ( -- ulen flag t=got )
  while
    in-buf swap  parse-input$  \ what next
    part-one @ + part-one !
    part-two @ + part-two !
  repeat
  drop

  in-fd close-file throw
  cr cr ." 2025 day ?? part 1 answer: "
  part-one @ 6 .r
     cr ."             part 2 answer: "
  part-two @ 6 .r cr ;


\ Save some typing.

: run-test
  s" ../../../Advent-Data/2025/??/test.txt" solve ;

: run-live
  s" ../../../Advent-Data/2025/??/live.txt" solve ;

BASE !

\ End of shell.fs
