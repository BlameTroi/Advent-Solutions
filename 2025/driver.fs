\ driver.fs -- String experiments -- T.Brumley.

BASE @
DECIMAL

132 constant in-max \ override default in io.fs
require io.fs
require parsing.fs
require strings.fs
require stack.fs
require text.fs
require within.fs

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
\ produce the highest "joltage." It's a "best of the remaing"
\ problem. To find the first digit, select the largest digit
\ from the remaining string less the last 11 digits. To find
\ the second digit, select the largest digit to the right of
\ the first digit less the last 10 digits. Repeat ad nauseum.
\
\ No need to get too cute here, but we need several helper
\ functions.
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


\ pick the largest 12 digit number from however many digits
\ there are in the order presented. This is a greedy selection
\ from a set of decreasing size problem.

: s1full s" 987654321111111" ;   ( drops some ones from end )
\                      ***
: s1part s" 987654321111" ;

: s2full s" 811111111111119" ;
\                      **
: s2part s" 811111111119" ;       ( drops some ones before end )

: s3full s" 234234234234278" ;
\          ** *
: s3part s" 434234234278" ;       ( 2 3 and 2 near start )
\ 234234 largest digit 4 ...

: s4full s" 818181911112111" ;    ( dropping ones near start )
\           * * *
: s4part s" 888911112111" ;

\ edge: if remaining string length = needed length, done
\
\ error: if remaining string length < needed length, abort

\ to find first digit
\ full len = 15
\ remaining after digit = 11
\ therefore largest digit in first 4
\ 987654321
\ 0123
\ ____|

\ so
\ find largest in substring, adjust string pointer to after largest

: largest-char-in {: str len | chr idx :}
  str c@ to chr 0 to idx ( assume first is largest )
  len 1 do
    str i + c@
    dup chr > if
      to chr i to idx
    else
      drop
    then
  loop
  idx +
  len idx - ;

: highest-joltage-two ( c-addr u -- n )
  2drop 3141597312 ( just some random number )
  ;

: local-test { a s d f -- }
  cr a . s . d . f .
  5 { f } \ this gets a redefined warning but f does pick up
          \ 5 as expected
  cr a . s . d . f .
  10 0 do
    i { f } cr f . \ f is not scoped, the f below gets the value 9
  loop \ also redefined but has right val
  cr a . s . d . f . ;





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
  s" test.txt" solve ;

: run-live
  s" live.txt" solve ;


\ End of driver.fs

