\ 2025/01/part1.fth -- Secret Entrance -- T.Brumley.

   MARKER AOC250101

\ Input format is a single letter direction (L or R) and an
\ integer of arbitrry length for clicks to move.
\

\ Moves for part one are R = + digits, L = - digits, abs,
\ mod 100

\ The problem statement:
\ 
\ Get a password from a safe by counting the number of times
\ the dial stops on 0 when following the directions in the
\ data file.
\ 
\ Data file format is [LR]d+
\
\ If L then the value of the d+ field is negative.
\
\ The safe dial is set at 50 to start, turn the dial the
\ specified number of "clicks" in the direction given. The
\ dial goes from 0 to 99 inclusive.
\ 
\ So      Dial           Move          New Dial
\           50            L68                82
\           82            L30                52
\           52            R48                 0 <-- count

\ Input and output support. This problem only requires input
\ but I'll be building up some reusable definitions as I go
\ through the exercise.

0 VALUE fd-in
0 VALUE fd-out

\ A buffer to hold the data line, variable length lines
\ delimited by newlines. The 80 "max" is arbitrary. The
\ additional two bytes are for a possible crlf if someone
\ runs this on a DOS format file.

80 CONSTANT line-max
CREATE line-buffer line-max 2 + ALLOT

: open-input ( addr u -- , s" filename" on stack )
   r/o open-file
   throw
   to fd-in ;

: close-input ( -- )
   fd-in close-file throw ;
   
\ Read a line of input. Read-line returns the length read,
\ an eof flag, and the io result. The throw consumes the
\ ior, and we return the length and eof flag.

: read-input ( -- u f )
      line-buffer line-max blank
      line-buffer line-max fd-in read-line throw ;

: open-output ( addr u -- )
   w/o open-file
   throw
   to fd-out ;

\ Problem state. There is no need yet to store either the
\ raw or parsed input. For now each spin instruction will
\ execute as it is read.

VARIABLE dial
VARIABLE spins
VARIABLE zeros
VARIABLE len
VARIABLE reporting
VARIABLE sgn

\ Pretty straight forward IPO loop. Any error other than
\ EOF will throw an error (non-0 IOR).

: do-part-1 ( -- )
   50 dial !
   0 spins !
   0 zeros !
   0 len !
   0 sgn !
   
   begin

      read-input

   while                ( consumes flag, leaving length )

      len !             ( length of line read )

      cr dial @ . space

      line-buffer c@    ( L or R? )      
      'L' = if          ( L means negative or left )
         -1 sgn !                  
      else
         1 sgn !
      then              ( sign )

      line-buffer 1+    ( starting position for >number )
      len @ 1-          ( max length to evaluate )
      0 0 2swap         ( sign daccum addr len)
      >number           ( sign ud addr len , parse digits )
      2drop             ( discard next parse information )
      drop              ( discard high cell of parsed value )
      sgn @ *           ( apply sign )

      \ Spin magnitude on TOS.      
      \ reporting if
      \    cr 'D' emit dial @ .    ( dial starting postion )
      \    space
      \    line-buffer len @ type  ( instruction )
      \    space
      \ then

      dial @ + dial !     ( spin the dial )

      spins @ 1+ spins !  ( increment )

   repeat
   drop                   ( discard 0 length from eof read )

   cr ." answer: "
   zeros @ .
   ."k spins: "
   spins @ .
   cr
   ;

: run
   s" test.txt" open-input
   
   true reporting !

   do-part-1
   cr
   fd-in close-file throw
   cr ." done" ;

\ End of 2025/01/part1.fth
