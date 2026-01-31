\ solution.fs -- AoC 2025 03 Lobby -- T.Brumley.

BASE @
DECIMAL

132 constant in-max \ override default in io.fs
require ../io.fs
require ../parsing.fs
require ../strings.fs
require ../stack.fs

\ Problem: ----------------------------------------------------

\ for part one:
\
\ You are presented with several banks of batteries represented
\ as one digit per battery and one line per bank. You may not
\ rearrange the batteries.
\
\ For each bank find the pair of batteries you can turn on that
\ results in the largest "joltage." Sum all the "joltages."
\
\ 987654321111111
\ **
\ 811111111111119
\ *             *
\ 234234234234278
\              **
\ 818181911112111
\       *    *
\ 98 89 78 92 + + + . 357
\
\ Examination of data shows no 0 digits.
\
\ For part two:
\
\ Instead of two batteries turn on the 12 batteries that will
\ produce the highest "joltage." It's a combinatorial problem.
\
\ For the above data:
\
\ 987654321111111    (dropping some oes at the end)
\ 987654321111
\
\ 811111111111119    (again dropping some ones)
\ 811111111119
\
\ 234234234234278    (note a 2 3 and 2 near start are off)
\ 434234234278
\
\ 818181911112111    (note dropping ones near start)
\ 888911112111
\
\ So find the largest value of 12 digits from however many
\ there are


\ Problem state: ----------------------------------------------

variable part-one  variable part-two


\ Helpers: ----------------------------------------------------


\ It seems impossible to not find an answer so I do no error
\ checking.

: highest-joltage-one ( c-addr u -- n )
  9 99 do
    2dup
    i pad 3 u>$ 2drop       ( '99' to pad, c-addr u on stack )
    pad c@ -rot cposin$ dup if     ( c-a u )
      nextpos$
      pad 1+ c@ -rot cposin$ dup if
        2drop
        i leave
      then
    then
    2drop
  -1 +loop
  -rot 2drop ;

\ Find the largest possible n digits from an m digit string,
\ where the digit order must be preserved. So:
\
\ 9
\ 234234234234278    (note a 2 3 and 2 near start are off)
\ 434234234278
\ n = 12, m = 15
\ so the last 11 digits of m can not be scanned on first pass
\
\ 2342...........
\   4234234234278
\ n = 11, m = 12
\ so the last 10 diigts of m can not be scanned on second pass
\   43...........
\   434234234278
\ n = 10, m = 10 so we are done
\


\ pick the largest 12 digit number from however many digits
\ there are in the order presented. This is a greedy selection
\ from a set of decreasing size problem.

: highest-joltage-two ( c-addr u -- n )
  2drop 3141597312 ( just some random number )
  ;


\ Driver: -----------------------------------------------------

\ Driver for the solution. Use run-live/test interactively to
\ avoid typing long paths.

: solve ( s" input.txt" -- )

  open-input

  0 part-one ! 0 part-two !

  begin
    in-buf in-max 2dup blank read-line-skip-empty
  while
    drop \ not using length read
    in-buf in-max -trailing 2dup
    cr 2dup type space space highest-joltage-one dup .
    part-one @ + part-one !
    highest-joltage-two dup .
    part-two @ + part-two !
  repeat
  drop

  in-fd close-file throw
  cr cr ." 2025 day 03 part 1 answer: "
  part-one @ 6 .r
     cr ."             part 2 answer: "
  part-two @ 6 .r cr ;


\ Save some typing.

: run-test
  s" ../../../Advent-Data/2025/03/test.txt" solve ;

: run-live
  s" ../../../Advent-Data/2025/03/live.txt" solve ;

BASE !

\ End of solution.fs
