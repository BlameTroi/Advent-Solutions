\ stack.fs -- An extra stack -- T.Brumley.

BASE @
DECIMAL

\ OVERVIEW:
\
\ An extra stack. The Forth data stack is primarily for parameter
\ passing and arithmetic operations. Stacks are also useful for
\ LIFO storage.
\
\ The original idea is from _Thinking Forth_ but I have added
\ more functionality and come up with my own naming standard.
\
\ Here's hoping the naming consistency helps.
\
\ NOTE:
\
\ Several of these names shadow
\ TODO:
\
\ 1. Should STACK be <STACK (start stack) to parallel STACK>
\    (stack end)?
\
\ 2. How might I manage multiple parallel stacks?
\
\ GLOSSARY:
\
\ STACK0   ( -- )       Initialize the stack.
\ STACK?   ( f -- )     (Private) report error and reset.
\ STACK!   ( n -- )     Push entry onto stack.
\ STACK@   ( -- n )     Pop entry off stack.
\ STACK^   ( -- n )     Peek top of stack.
\ STACK.   ( -- )       Print top of stack.
\ STACK#   ( -- n )     Stack depth (cells).
\ STACK.S  ( -- )       Print all entries like .S does.

[UNDEFINED] stack-#-max [IF]
  100 constant stack-#-max
[THEN]
create stack stack-#-max cells allot
here constant stack>

( Maintenance. )

: stack0 ( -- ) stack stack ! ; stack0
: stack? ( f -- ) if stack0 abort" stack bounds!" then ;

( Useful API. )

: stack! ( n -- ) cell stack +! stack @ dup stack> = stack? ! ;
: stack@ ( -- n ) stack @ @ -1 cells stack +! stack @ stack < stack? ;

( Non destructive queries. )

: stack^ ( -- n ) stack@ dup stack! ;
: stack. ( -- ) stack@ . ;
: stack# ( -- depth ) stack @ stack - cell / ;
: stack.s ( print all entries )
  '<' emit stack# dup . 8 emit '>' emit space
  if
    stack# 0 do
      stack i 1+ cells + ?
    loop
  then ;

require test/ttester.fs

verbose on
T{ stack0 stack# -> 0 }T
T{ stack0 3 stack! 2 stack! stack# stack@ * stack@ * -> 12 }T
T{ stack0 3 stack! stack# stack^ stack# -> 1 3 1 }T
T{ stack0 3 stack! stack# stack@ stack# -> 1 3 0 }T
verbose off

BASE !

\ End of stack.fs.