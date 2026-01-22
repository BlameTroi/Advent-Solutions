\ parsing.fs -- Some parse helpers -- T.Brumley

BASE @
DECIMAL

\ Given a string run it through >NUMBER. The return is
\ identical to >NUMBER, this factors out the standard setup.

: parse->number$ ( c-addr1 u1 -- c-addr2 u2 ud )
  0 0 2swap >number ;

\ A range is a string of two numbers separated by a dash. While
\ that may not seem like a general format, it is common in the
\ Advent of Code problems.
\
\ While >NUMBER deals in doubles I drop them to single cells.
\ That's 64 bits which I hope is wide enough.

: parse-range$ ( s-addr u -- s-addr u lo hi )
  parse->number$ 2swap drop >r     ( stash lo )
  1- swap 1+ swap                  ( len -1 addr +1 for '-' )
  parse->number$ 2swap drop
  r> swap ;                        ( set return add u lo hi )

BASE !

\ End of parsing.fs