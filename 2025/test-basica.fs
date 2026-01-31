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

\ smaller to larger

T{ pad 84 erase                      ( nuls to test blank pad )
   s" 012345678012345"
   2dup pad 32 LSET
   pad over compare       ( did the l'source bytes get copied )
   blanks 16 pad 16 + 16 compare ->    ( and the rest blanks? )
   0 0 }T

\ larger to smaller

T{ pad 84 erase
   s" 012345678901234567890123456789012345678901234567"
   2dup pad 32 LSET
   drop 32 pad 32 compare       ( compare using right length )
   pad 32 + c@ -> 0 0 }T        ( no overflow )


\ MSET (statement form of MID$) ( c-addr1 u1 c-addr2 u2 u3 )
\ Copy 1 into 2+u3, watching for overflows.

cr ." MSET"

\ 5 bytes from a 6 byte string into 10 byte string + 5

T{ pad 84 erase
   s" 0123456789" >pad
   s" 0123456" pad 10 5 MSET    ( result should be 0123401234 )
   pad 10 s" 0123401234" compare
   pad 11 + c@ -> 0 0 }T

\ 3 bytes from a 7 byte string into 10 byte string + 5

T{ s" 0123456789" >pad
   s" 0123456" drop 3 pad 10 5 MSET  ( result should be 0123401289 )
   pad 10 s" 0123401289" compare
   pad 11 + c@ -> 0 0 }T

\ 7 byte string to 10 byte + 8, no overflow.

T{ s" 0123456789" >pad
   s" 0123456" pad 10 8 MSET    ( result should be 0123456701 )
   pad 10 s" 0123456701" compare
   pad 11 + c@ -> 0 0 }T        ( and byte after is untouched )


\ RSET ( c-addr1 u1 c-addr2 u2 -- )
\ Right justify 1 in 2, setting any unfilled leading characters
\ to blank. If 1 is larger than 2, the rightmost u2 characters
\ of 1 are moved.

cr ." RSET"

\ Overlay tail with 7 bytes. Proves that RSET blank pads.

T{ pad 84 erase
   pad 48 'a' fill
   s" 0123456" pad 48 RSET
   \ note following string literal has a leading blank.
   pad 48 + 8 - 8 s"  0123456" compare -> 0 }T

\ Truncate left of set. Proves that there's no overflow.

T{ pad 84 erase
   pad 48 'a' fill
   s" abcdefg" >pad
   s" 0123456789" pad 7 RSET
   pad 8 s" 3456789a" compare -> 0 }T

\ An exact fit.

T{ pad 84 erase pad 11 '-' fill
   s" 0123456789" pad 10 RSET
   s" 0123456789" pad 10 compare
   pad 10 + c@ '-' = -> 0 true }T

\ Smaller than destination. Checking for blank padding.

T{ pad 84 erase pad 15 '-' fill
   s" 0123456789" pad 15 RSET
   s" 0123456789" pad 5 + over compare
   blanks 5 pad 5 compare -> 0 0 }T

\ Larger than destination. This should get the right most n
\ characters from the source string.

T{ pad 84 erase pad 5 '-' fill
   s" 0123456789" pad 5 RSET
   s" 56789" pad 5 compare -> 0 }T


\ INSTR ( c-addr1 u1 c-addr2 u2 -- u )
\ Report position of 1 in 2, -1 if not found.

\ Basic INSTR with some boundary checks.

cr ." INSTR"

T{ s" 124" s" 012345" INSTR -> -1 }T   ( not found )
T{ s" 123" s" 012345" INSTR -> 1 }T    ( found @ 1 )
T{ s" 012" s" 012345" INSTR -> 0 }T    ( found @ 0 )
T{ s" 013" s" 012345" INSTR -> -1 }T   ( not found )
T{ s" 123" s" 12"     INSTR -> -1 }T   ( fails target short )
T{ s" 45"  s" 012345" INSTR -> 4 }T    ( check tail boundary )
T{ s" 5"   s" 012345" INSTR -> 5 }T

\ Some special cases on INSTR warrant their own definitions.

T{ s" 123" s" 123456789" BEGINS$ -> true }T
T{ s" 456" s" 123456789" BEGINS$ -> false }T
T{ s" 789" s" 123456789" BEGINS$ -> false }T

T{ s" 123" s" 123456789" ENDS$ -> false }T
T{ s" 456" s" 123456789" ENDS$ -> false }T
T{ s" 789" s" 123456789" ENDS$ -> true }T


\ LEFT$

cr ." LEFT$"

\ Take the first n bytes of the source and move to the destination
\ string, truncating if the target string is longer than n.

\ entire source to larger destination. Destination is truncated.

T{ pad 84 erase
   s" 0123456789" ( source string )
   dup            ( use its entire length )
   pad 84         ( and place it in a larger string )
   LEFT$
   -> pad s" 0123456789" swap drop }T \ pad l'source

\ whole source to smaller destination. Copies l'dest characters
\ from source to destination. any remainging positions in the
\ destination are left untouched.

T{ pad 84 erase
   s" 0123456789" dup      ( source length, length to use )
   pad over 2/            ( destination is half the length of source )
   LEFT$
   s" 0123456789" 2/ compare  ( LEFT$ leaves string ptr )
   pad 5 + c@ -> 0 0 }T    ( followed by nul )


\ LEN

cr ." LEN"

\ no testing for LEN, it's not needed.


\ MID$
\ : MID$ {: str slen spos sfor dstr dlen -- substr sfor :}
\ Some of these aren't realistic, but I'm looking at edge
\ cases.

cr ." MID$"

\ equal
T{ pad 84 erase
   s" 0123456789" 0 10 pad 10 MID$
   s" 0123456789" compare -> 0 }T

\ zero length
T{ pad 84 erase
   s" 0123456789" 0 0 pad 10 MID$
   s" 0123456789" drop 0 compare -> 0 }T

\ front
T{ pad 84 erase
   s" 0123456789" 0 3 pad 10 MID$
   3 = swap pad = and
   s" 0123456789" drop 3 pad 3 compare -> -1 0 }T

: RIGHT$ {: str slen n dst dlen -- dst u :}
  n dlen > if dlen to n then
  slen 0= n 0= or if dst 0 exit then
  slen n < if str dst slen move dst slen exit then
  str slen + n - dst n move dst n ;
\ RIGHT$ ( c-addr1 u1 n c-addr2 u2 -- c-addr2 u )

\ N is greater than destination length.

T{ pad 84 erase
   s" 0123456789" 10 pad 3 RIGHT$
   s" 789" compare -> 0 }T

\ Source length 0.

T{ pad 84 erase
   s" 0123456789" drop 0 10 pad 10 RIGHT$
   -> pad 0 }T

\ N is 0.

T{ pad 84 erase
   s" 0123456789" 0 pad 10 RIGHT$
   -> pad 0 }T

\ No more edges.

T{ pad 84 erase
   s" 0123456789" 5 pad 10 RIGHT$
   2dup s" 56789" compare
   -> pad 5 0 }T

BASE !

\ End of test-basica.fs
