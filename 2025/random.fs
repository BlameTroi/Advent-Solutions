\ random.fs -- A simple generator from old code -- T.Brumley

BASE @
DECIMAL

\ A random number generator (16-bit) from Starting Forth.
\
\ The RANDOM and CHOOSE in pforth use this algorithm but for
\ 64-bit instead of 32-bit.
\
\ For some reason I went for a 24-bit range instead of the full
\ 64. I don't remember why but it works.
\
\ TODO: adjust random-mask as a value.

variable random-seed   here random-seed !
16777215 constant random-mask   ( 24 bit )

: random ( -- u )
   random-seed @ 31421 * 6927 +
   random-mask and dup random-seed !   ;

: choose ( u1 -- u2 , 0 <= u1 < u1 )
   random-mask swap /           \ scale
   random swap / ;

BASE !
\ End of random.fs.