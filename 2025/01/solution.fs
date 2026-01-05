\ solution.fs -- AoC 2025 01 Secret Entrance -- T.Brumley.

require TxbWords.fs
require TxbStrings.fs

create in-fn 256 chars allot
0 value in-fd   80 constant in-max   variable in-len
create in-buf in-max 2 chars + allot

create out-fn 256 chars allot
0 value out-fd   80 constant out-max   variable out-len
create out-buf out-max 2 chars + allot   \ 2+ isn't needed

variable dial        variable new-dial    variable zeros
variable direction   variable magnitude

\ Print debugging will never die.

: debug-tracer
   ." Dial " dial @ 3 .r
   ."  dir" direction @ 3 .r
   ."  magnitude" magnitude @ 5 .r
   ."  new" new-dial @ 3 .r
   space in-buf in-len @ type cr ;

\ Parse ^[LR]d+$ to an magnitude of a turn of the dial (d+$)
\ and a direction (L = left = -1).

: parse-input ( -- +/-1 u )
   in-buf c@ 'L' = if -1 else 1 then
   in-buf char+   in-len @ 1 chars -   \ string to parse
   0 0 2swap   >number   2drop drop ;  \ discard next@ and msc

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
\ Invoke from Forth prompt s" datafile" part-one.

: part-one ( -- )
   in-fn 256 s$>c$  in-fn count r/o open-file  throw  to in-fd
   50 dial ! 0 zeros !
   begin
      in-buf in-max in-fd read-line
      throw   swap   in-len !    \ leaves eof flag
   while
      parse-input   magnitude !  direction !
      magnitude @ 100 mod direction @ * dial @ + \ new
      dup 99 > if 100 - else     \ clip back to 0-99
      dup 0< if 100 + then   then
      dup new-dial !
      0= if 1 zeros @ + zeros ! then  \ tally stops on zero
      debug-tracer
      new-dial @ dial !         \ update dial
   repeat
   cr ." 2025 day 1 part 1 answer: "
   zeros @ . cr
   in-fd close-file throw ;

\ End of solution.fs
