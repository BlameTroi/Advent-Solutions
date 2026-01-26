\ test-text.fs -- Test an input collector -- T.Brumley

BASE @
DECIMAL

\ TEXT uses word to place a delimited word into the PAD. It
\ is not part of modern standards.
\
\ This does not return any length information. I assume
\ that people would surround calls with PAD 84 BLANK and
\ PAD 84 -TRAILING to get a proper length.

require test/ttester.fs

T{ pad 32 erase
  '"' text "this is a test"
  pad 8 s" this is " compare -> 0 }T

BASE !

\ End of test-text.fs.