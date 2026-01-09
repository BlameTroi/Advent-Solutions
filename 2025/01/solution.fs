\ solution.fs -- AoC 2025 01 Secret Entrance -- T.Brumley.

require TxbWords.fs
require TxbStrings.fs

create in-fn 256 chars allot
0 value in-fd   80 constant in-max   variable in-len
create in-buf in-max 2 chars + allot

variable direction   variable magnitude
variable dial        variable dial-net   variable dial-next
variable zero-stop   variable zero-pass

\ Print debugging will never die.

: debug-tracer
  cr ." cur" dial @ 4 .r
  ."  dir" direction @ 4 .r
  ."  mag" magnitude @ 6 .r
  ."  net" dial-net @ 4 .r
  ."  nxt" dial-next @ 4 .r
  ."  stop" zero-stop @ 5 .r
  ."  pass" zero-pass @ 5 .r ;
\  space in-buf in-len @ type ;


\ Part One:
\
\ Follow the directions to determine a password. The safe dial
\ ranges [0-99] and its starting position if 50. Count the
\ number of times the dial sits at 0 after a spin.
\ 
\ So      Dial           Move          New Dial
\           50            L68                82
\           50 -68 + -18 100 + 82
\           82            L30                52
\           82 30 - 52
\           52            R48                 0 <-- counts as 1
\
\ Part two:
\ You remember from the training seminar that "method 0x434C49434B"
\ means you're actually supposed to count the number of times any click
\ causes the dial to point at 0, regardless of whether it happens
\ during a rotation or at the end of one.
\
\ Following the same rotations as in the above example, the dial points
\ at zero a few extra times during its rotations:

\ So      Dial           Move          New Dial
\           50            L68                82 <-- passes 0 once
\           50 -68 + -18 100 + 82
\           82            L30                52
\           82 30 - 52
\           52            R48                 0 <-- counts as 1
\
\    The dial starts by pointing at 50.
\    The dial is rotated L68 to point at 82; it points at 0 once.
\    The dial is rotated L30 to point at 52.
\    The dial is rotated R48 to point at 0.
\    The dial is rotated L5 to point at 95.
\    The dial is rotated R60 to point at 55; it points at 0 once.
\    The dial is rotated L55 to point at 0.
\    The dial is rotated L1 to point at 99.
\    The dial is rotated L99 to point at 0.
\    The dial is rotated R14 to point at 14.
\    The dial is rotated L82 to point at 32; it points at 0 once.
\
\ In this example, the dial points at 0 three times at the end of a
\ rotation, plus three more times during a rotation. So, in this
\ example, the new password would be 6.
\
\ Invoke from Forth prompt s" datafile" solver.


: solver ( -- )
  50 dial ! 0 zero-stop ! 0 zero-pass !

  in-fn 256 s$>c$  in-fn count  r/o open-file throw  to in-fd
  cr

  begin
    in-buf in-max in-fd read-line
    throw  swap  in-len !         \ leaves eof flag
  while
    \ part one -- count # times stop at zero
    \ part two -- count # times rolls over, including stop
    \ Parse ^[LR]d+$ to an magnitude of a turn of the dial (d+$)
    \ and a direction (L = left = -1).

    in-buf c@ 'L' = if -1 else 1 then
    direction !

    in-buf char+  in-len @ 1 chars -  \ string to parse
    0 0 2swap  >number  2drop drop    \ discard next@ and msc
    magnitude !

    magnitude @ 100 /mod       \ net effect, full spins
    zero-pass @ + zero-pass !
    dial-net !

    dial-net @ direction @ * dial @ + dial-next !   \ net * direction

    dial-next @
    dup 99 > if 100 - zero-pass dup @ 1+ swap ! else
    dup 0< if 100 + zero-pass dup @ 1+ swap ! then then

    \ dial-next @
    dup 99 > if 100 - else
    dup 0< if 100 + then then
    dial-next !

    dial-next @ 0= if zero-stop dup @ 1+ swap ! then  \ @zero
    debug-tracer
    dial-next @ dial !         \ update dial
  repeat

  in-fd close-file throw
  cr cr ." 2025 day 1 part 1 answer: "
  zero-stop @ 6 .r
  cr ."            part 2 answer: "
  zero-pass @ 6 .r cr ;

\ End of solution.fs
