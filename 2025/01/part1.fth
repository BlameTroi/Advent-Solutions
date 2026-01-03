\ 2025/01/part1.fth -- Secret Entrance -- T.Brumley.

\ Input and output support.
\
\ Input and output are text files, but in this problem we only
\ open input. Each line is read into a fixed buffer. Clients
\ can expect this buffer to be unchanged until the next read.
\
\ The buffer size is arbitrary, but we must allot two additional
\ bytes for end of line handling. After a line is read the
\ fields line-length and line-buffer are set. They should be
\ considered read only by client code.

create input-file-name 256 chars allot
0 VALUE fd-in

create output-file-name 256 chars allot
0 VALUE fd-out

0 value line-length
80 CONSTANT line-max
CREATE line-buffer line-max 2 + ALLOT

: open-input ( c-addr n -- ior )
   0 to line-length
   r/o open-file swap to fd-in ;

: close-input ( c-addr n -- ior )
   fd-in close-file
   0 to fd-in ;

: open-output ( c-addr n -- ior )
   w/o open-file swap to fd-out ;

: close-output ( s" filename" - ior )
   fd-out close-file
   0 to fd-out ;

\ Read a line of input. Read-line returns the length read,an eof
\ flag, and the io result. The throw consumes the ior. Save the
\ length with the buffer and return the eof flag.

: read-input ( -- f )
   line-buffer line-max 2dup
   0 fill                         \ prevent seeing prior line
   fd-in read-line                \ u f ior
   throw swap to line-length ;    \ eof?

: write-output ( c-addr u -- ior )
   type cr 0 ; \ stubb

\ Problem state.

VARIABLE dial                 \ current position
VARIABLE zeros                \ times at zero
variable next-dial
VARIABLE direction
VARIABLE clicks
variable net-turn
variable full-turn

: debug-tracer
   ." Dial " dial @ 4 .r
   ."  Direction" direction @ 4 .r
   ."  Clicks" clicks @ 4 .r
   ."  net.turn" net-turn @ 4 .r
   ."  Next" next-dial @ 4 .r
   ."  Full turn" full-turn @ 5 .r
   space line-buffer line-length type cr                   
;

\ Parse and evaluate each command. A command may request
\ that the dial be turned more than once around. We want
\ both the full and net effects.
\
\ The data file is one or more lines of the format ^[LR]d+$

: get-direction ( -- +/-1 )
   line-buffer c@ 'L' = if -1 else 1 then ;

: get-clicks ( -- n )
      line-buffer char+  \ c-addr
      line-length 1 chars -  \ u
      0 0 2swap          \ ud c-addr u
      >number            \ d c-addr u
      2drop drop ;       \ n

: parse-input ( -- )
   get-direction get-clicks clicks ! direction !
   direction @ clicks @ * full-turn !
   clicks @ 100 /mod drop
   direction @ * net-turn ! ;

\ Part One:
\
\ Follow the directions (turn the knob) to determine a password.
\ The safe dial ranges [0-99] and its starting position if 50.
\ Count the number of times the dial sits at 0 after a spin.
\ 
\ So      Dial           Move          New Dial
\           50            L68                82
\           50 -68 + -18 100 + 82
\           82            L30                52
\           82 30 - 52
\           52            R48                 0 <-- counts as 1

: part-1 ( -- )
   cr
   50 dial ! 0 zeros !                 \ clear results
   begin read-input while
      parse-input
      net-turn @ dial @ +              \ turn the knob
      dup 99 > if 100 - else           \ normalize to 0-99
      dup 0< if 100 + then then
      dup next-dial !                  \ save for reporting
      0= if 1 zeros @ + zeros ! then   \ tally
      debug-tracer
      next-dial @ dial !               \ new dial position
   repeat 
   cr ." 2025 day 1 part 1 answer: "
   zeros @ . cr ;

: run ( s" input filename" -- )

   \ Copy input file name to counted string buffer.
   input-file-name 256 erase
   input-file-name c!
   input-file-name char+ input-file-name c@ move

   input-file-name count open-input throw
   part-1
   cr
   close-input
   \ input-file-name count open-input throw
   \ part-2
   \ cr
   \ close-input throw
   cr ." done" ;

\ End of 2025/01/part1.fth
