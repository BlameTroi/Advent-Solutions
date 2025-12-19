\ AOC2501A.fth -- Secret Entrance -- T.Brumley

   MARKER AOC250101

\ Conventions/notes/ideas:
\ 
\ Standard Forth should be in all upper case letters for
\ system defined words. While I find case a helpful hint,
\ I'm not sure how best to use it. After experimenting, I'm
\ going to use lower case for most everything except defining
\ words such as VARIABLE or CREATE. I hope these will act as
\ guides when reading the code.
\ 
\ Neither of the Forths I use are case sensitive.
   
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

\ Input and output support. This problem only requires input
\ but I'll be building up some reusable definitions as I go
\ through the exercise.

0 VALUE fd-in
0 VALUE fd-out

: open-input ( addr u -- , s" filename" on stack )
   r/o open-file
   throw
   to fd-in ;

: open-output ( addr u -- )
   w/o open-file
   throw
   to fd-out ;

\ A buffer to hold the data line, variable length lines
\ delimited by newlines. The 80 "max" is arbitrary. The
\ additional two bytes are for a possible crlf if someone
\ runs this on a DOS format file.

80 CONSTANT line-max
CREATE line-buffer line-max 2 + ALLOT

\ Problem state. There is no need yet to store either the
\ raw or parsed input. For now each spin instruction will
\ execute as it is read.

VARIABLE dial
0 dial !
VARIABLE spins
0 spins !
VARIABLE zeros
0 zeros !

\ Pretty straight forward IPO loop. Any error other than
\ EOF will throw an error (non-0 IOR). While we don't want
\ the input data leaking outside to non-AOC participants,
\ I print everything as I go along for now.

: do-part-1 ( -- )
   50 dial !
   0 spins !
   0 zeros !

   s" part1.txt" open-input
   
   begin

      line-buffer line-max blank
      ( read-line returns length-read eof-flag and ior )
      ( throw consumes the ior, and while the eof-flag )
      line-buffer line-max fd-in read-line throw

      swap drop         ( discard length read )

   while

      \ After some experiments, I'm keep the sign indicator
      \ on the stack instead of storing it in a variable. I
      \ need to practice my stack juggling.

      line-buffer c@    ( L or R? )      
      'L' = if          ( L means negative or left )
         -1                  
      else
         1
      then              ( sign )
      line-buffer 1+    ( sign addr , position to digit )
      line-max 2 -      ( sign addr len , remaining buf )
      2>r               ( stash addr len )
      0 0 2r>           ( sign 0d addr len , insert accum )
      >number           ( sign ud addr len , parse digits )
      2drop             ( discard next parse position )
      drop              ( discard high half of parsed value )
      100 mod           ( deal with the larger spins )
      *                 ( apply sign )

      \ Display what we've got and then execute it.

      cr
      line-buffer c@ emit ( print as Ldd or Rdd )
      dup abs 0 <# # # #> type ( L # # | # # | # # | ? )
      ."  | "
      dial @ 0 <# # # #>  type ( dial start )
      dial @ +            ( spin the dial )
      100 + abs 100 mod
      dup dial !          ( and remember )
      ."  | "
      0 <# # # #> type    ( dial end )      
      dial @ 0= if        ( did we hit zero? )
         ."  <--- zero "
         zeros dup @ 1+ swap ! ( increment )
      then
      spins dup @ 1+ swap ! ( increment )

   repeat

   cr ." answer: "
   zeros @ .
   ."  spins: "
   spins @ .
   cr
   ;

: run
   do-part-1
   cr
   fd-in close-file throw
   cr ." done" ;

\ End of AOC202501A.fth
