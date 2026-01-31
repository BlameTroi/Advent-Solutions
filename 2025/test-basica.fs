\ test-basica.fs -- Tests for same -- T.Brumley.

BASE @
DECIMAL

require test/ttester.fs

\ Copy a string to the pad, mostly for testing.

: >pad ( c-addr u -- , will abort if no room )
  dup 84 > if ." No room in Pad for " .s drop 10 dump throw then
  pad swap move ;

\ Assorted test data.

create blanks 128 allot
blanks 128 bl fill


\ LSET ( c-addr1 u1 c-addr2 u2 -- )
\ Left justify 1 in 2, truncating or blank padding as needed.

cr ." LSET"

\ INSTR ( c-addr1 u1 c-addr2 u2 -- u )
\ Report position of 1 in 2, -1 if not found.

\ Basic INSTR with some boundary checks.



\ LEFT$


\ LEN

cr ." LEN"

\ no testing for LEN, it's not needed.


\ MID$
\ RIGHT$ ( c-addr1 u1 n c-addr2 u2 -- c-addr2 u )


BASE !

\ End of test-basica.fs
