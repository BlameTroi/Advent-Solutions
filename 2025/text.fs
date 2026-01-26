\ text.fs -- An old FIG word that still shows up -- T.Brumley

BASE @
DECIMAL

\ TEXT is an old word not in the new standard. I'm not sure why
\ it was dropped, but it could be because definitions use the
\ PAD and the standard is that no standard definitions may
\ change the PAD.
\
\ The PAD must be at least 84 bytes in length and so this
\ version of TEXT will not read anything longer than 84 bytes.
\
\ WORD's buffer must be at least 33 bytes in length. One
\ length byte followed by room for at least 32 characters.
\
\ The original definition I used is from _Starting Forth_.
\
\ This does not return any length information. I assume that
\ people would surround calls with PAD 84 BLANK and
\ PAD 84 -TRAILING to get a proper length.

84 constant pad-usable-len              \ 84 is a minimum
: text ( c -- , delimiter for word )
   word count pad-usable-len min        \ c-addr u
   pad dup pad-usable-len erase swap move ; \ move to pad

BASE !

\ End of text.fs.