\ strings.fs -- Some common words -- T.Brumley

BASE @
DECIMAL

\ A character string with a maximum length and placed on the
\ stack as c-addr u.

: string$ create dup , here over blank allot
          does> dup cell+ swap @ ;


\ safe-move copies up to u2 characters from addr1 to addr2.
\ If u1 < u2, only u1 characters are copied. If u2 > u1, only
\ u1 characters are copied and the remaining portion at addr2
\ is left unchanged.
\
\ TODO: This could be cleaner.

: safe-move$ ( c-addr1 u1 c-addr2 u2 -- )
   2dup 2>r       ( s: a1 u1 a2 u2       r: a2 u2 )
   swap drop 2dup ( s: a1 u1 u2 u1 u2    r: a2 u2 )
   > if           ( s: a1 u1 u2 u1>u2    r: a2 u2 )
      swap drop   ( s: a1 u2             r: a2 u2 )
   else
      drop        ( s: a1 u1             r: a2 u2 )
   then
   2r>
   cr .s cr
   drop swap      ( s: a1 u? a2 )
   move ;

\ Copy a string to a counted string. Will not overflow the
\ allowed length of the counted string (u2).

: s$>c$ ( c-addr1 u1 c-addr2 u2 -- )
   2dup erase  1- swap 1+ swap  rot min
   2dup swap 1- c!  move ;

\ Fetch the next character from a string, returns 0 if length
\ remaining < 1.

: c@-next$ ( c-addr u -- c-addr2 u2 c )
  dup 1 < if 0 else over c@ -rot 1- swap 1+ swap rot then ;

\ Is this string a single repeating character?

0 value asc-char
0 value asc-result
: ?all-same-char$ ( c-addr u -- f )
  c@-next to asc-char true to asc-result
  dup 0 do
    c@-next asc-char <> if false to asc-result leave then
  loop 2drop asc-result ;

\ Convert an unsigned single to a string and save it in the
\ supplied buffer. Result is left justified in buffer and is
\ quietly truncated if it would overflow the buffer.

: u>$ ( u c-addr u1 -- c-addr u2 )
  2dup blank rot 0 <# #s #>      \ u c-addr1 u1 c-addr2 u2
  swap >r min 2dup r> -rot       \ addr addr um
  move ;                         \

\ Can this string be divided into n equally sized substrings?
\ If so, how long are the substrings? The c-addr is not used
\ but as strings are passed as c-addr u pairs we accept the
\ standard and just discard the un-used part.

: ?n-sub$ ( c-addr u n -- un/ f )
  rot drop /mod swap 0= ;

\ Compare two adjacent strings of length u.

: ?adjacent$-equal ( c-addr u -- f )
  2dup dup >r + r> compare 0= ;

BASE !

\ End of strings.fs