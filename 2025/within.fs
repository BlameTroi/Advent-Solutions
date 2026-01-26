\ within.fs -- A closed interval range check -- T.Brumley.

BASE @
DECIMAL

\ Words relating to ranging.


\ Add a closed interval range check. I can't come up with a
\ consistent naming using ( and ] that won't be confused with
\ compile time vs interpretation time use of [] or the ()
\ for private definition convention.
\
\ ANS WITHIN is n x y -- flag , x <= n < y, or [x, y )
\
\ I have seen n x y 1+ WITHIN in some code, but I prefer this
\ definition from _Programming Forth_:

: within? ( n low high -- flag , closed interval )
   1+ within ;

BASE !

\ End of within.fs.
