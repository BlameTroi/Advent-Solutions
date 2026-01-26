\ test-stack.fs -- test the extra stack -- T.Brumley.

BASE @
DECIMAL

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

require test/ttester.fs

verbose on
T{ stack0 stack# -> 0 }T
T{ stack0 3 stack! 2 stack! stack# stack@ * stack@ * -> 12 }T
T{ stack0 3 stack! stack# stack^ stack# -> 1 3 1 }T
T{ stack0 3 stack! stack# stack@ stack# -> 1 3 0 }T
T{ stack0 3 stack! stack# stack@ drop stack# -> 1 0 }T
verbose off

BASE !

\ End of stack.fs.