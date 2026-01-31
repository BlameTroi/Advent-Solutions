\ legion.fs -- consolidating my helpers -- T.Brumley

\ Rather than struggle with finding the perfect grouping of
\ definitions by use, I'm pulling everything into this one
\ file and including my unit tests. There will be some attempt
\ at keeping related definitions in close proximity, but I
\ know it won't be perfect.

\ The Hayes based ttester.fs exists in several places on the
\ internet and a version is supplied with gforth. To include
\ and run the tests, set the constant INCLUDE-TESTS to true.
\
\ Words controlling code inclusion will be in UPPER case.
\
\ good: INCLUDE-TESTS [IF] test code block [THEN]
\  bad: include-tests [if] test code block [then]
\
\ The token >G at the start of a line comment is used to
\ identify comments used to build a glossary. The format is
\ rather strict: a line comment at the start of the line
\ followed by \b>G\b identifies the glossary text:
\
\  "\ >G WORD ( in -- out ) \ and maybe other text too".
\
\ >G must is case sensitive.
\
\ I know that gforth has \G for documentation generation but
\ I may want to do things that \G doesn't support.
\
\ Colon definition names should be in upper case in the
\ definition but otherwise my code is usually lower case.
\
\ TODO: blocks? mini-glossary and a fuller "dictionary"?


\ (c) 2026 Troy Brumley
\ All my stuff is public domain via the unlicense, or if you
\ don't like that you can choose the MIT license.

FALSE CONSTANT INCLUDE-TESTS

INCLUDE-TESTS [IF]
  require test/ttester.fs
[THEN]


\ Some globals for testing.
INCLUDE-TESTS [IF]
  create blanks 128 allot
  blanks 128 bl fill
[THEN]


\ Very minor definitions and types. These are mostly untested
\ as they are usually "intuitively obvious." I sometimes write
\ wrappers for standard words that I always invoke a certain
\ way.

\ A character string with a maximum length and placed on the
\ stack as c-addr u. The string is initialized to blanks.
\
\ TODO: alternate initializers.
\ TODO: byte strings.
\ TODO: dynamic.

: STRING create dup , here over blank allot
         does> dup cell+ swap @ ;


\ >G >PAD ( c-addr u -- ) Copy string to pad

: >PAD ( c-addr u -- , will abort if no room )
  dup 84 > if
    cr ." No room in Pad for " .s
    drop 32 dump throw   ( discard u, dump 32 bytes of string )
  then
  pad swap move ;


\ >G NEXTPOS$ ( c-addr u -- c-addr+1 u-1 ) Iterate string bytes forward
\ Stops when u (length) is zero.

: NEXTPOS$ ( c-addr u -- c-addr+1 u-1 , 0 if can't advance )
  dup if 1- swap 1+ swap then ;


\ >G PRIORPOS$ ( c-addr u -- c-addr-1 u+1 ) Iterate string bytes backward
\ There are no range checks.

: PRIORPOS$ ( c-addr u -- c-addr-1 u+1 , no range checks )
  1+ swap 1- swap ;


\ >G C@-NEXT$ ( c-addr u -- c-addr+1 u-1 c )
\ Fetch the next character from a string, returns 0 if length
\ remaining < 1. This is meant to be part of an iterator over
\ a string.

: C@-NEXT$ ( c-addr u -- c-addr2 u2 c )
  dup 1 < if 0 else over c@ -rot 1- swap 1+ swap rot then ;


\ >G SPACE$ ( c-addr u n -- c-addr n ) fill with spaces.
\ The length (u2) of result is the minimum of u and n.

: SPACE$ ( c-addr u n1 -- c-addr n2 )
  min 2dup bl fill ;


\ >G STRING4 ( c-addr u n c -- c-addr u2 ) fill with c
\ The length of result (u2) is the minimum of u and n.

: STRING$ ( c-addr u n c -- c-addr u2 )
  >r min 2dup r> fill ;

\ \G BEGINS$ ( c-addr1 u1 c-addr2 u2 -- flag )
\ Does 1 occupy the leftmost part of 2?

: BEGINS$ ( c-addr1 u1 c-addr2 u2 -- flag )
  {: beg blen str slen | -- flag :}
  blen slen <= if beg blen str blen compare 0=
               else false then ;

\ \G BEGINS$ ( c-addr1 u1 c-addr2 u2 -- flag )
\ Does 1 occupy the rightmost part of 2?

: ends$ ( c-adr1 u1 c-addr2 u2 -- flag )
  {: end elen str slen | -- flag :}
  elen slen <= if end elen str slen + elen - elen compare 0=
             else false then ;


\ I have a lot of Basic and Pascal habits built up over the
\ years. Here are string manipulation definitions that follow
\ Microsoft's PC era BASICA. In that parlance, a statement is
\ a modifier or l-value, while a function is r-values.

\ Statements  Description
\
\ LSET        Copy a left-justified value into a string buffer
\ MID$        Copy a value into part of a string buffer
\ RSET        Copy a right-justified value into a string buffer
\
\ Function    Description
\
\ INSTR       Find
\ LEFT$       Left substring
\ LEN         String length
\ MID$        Substring
\ RIGHT$      Right substring
\ SPACE$      Repeat spaces
\ STRING$     Repeat characters


\ >G LSET ( c-addr1 u1 c-addr2 u2 -- )
\
\ Copies a string1 to string2 left justified and blank
\ padded if string1 is shorter than string2. If string1
\ is longer than string2, it is quietly truncated.

: LSET ( c-addr1 u1 caddr2 u2 -- )
  2dup blank
  rot min move ;

INCLUDE-TESTS [IF]
  cr ." Running LSET tests..."
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
  cr
[THEN]






\ >G MSET ( c-addr1 u1 c-addr2 u2 u3 -- ) copy and fit from 1 to 2+u2 for u3 bytes
\
\ In Basic this is MID$ as an l-value. I've kept the MID$ name
\ for the r-value form and renamed this.
\
\ Copy sub to dst+dpos clipping sub to fit from dpos to dlen.

: MSET  ( c-addr1 u1 c-addr2 u2 u3 )
  {: sub slen dst dlen dpos | droom actlen -- :}
  dpos dlen >= dpos 0< or if ." MSET invalid arguments." throw 1 then
  dlen dpos - ( determine destination room )
  to droom
  droom slen min to actlen
  sub dst dpos + actlen move ;

\ MSET (statement form of MID$) ( c-addr1 u1 c-addr2 u2 u3 )
\ Copy 1 into 2+u3, watching for overflows.

INCLUDE-TESTS [IF]
cr ." Running MSET tests..."
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
   cr
[THEN]

\ >G RSET ( c-addr1 u1 c-addr2 u2 -- ) right justify 1 inoto 2 padding with blanks
\
\ Copies a string1 to string2 right justified with leading
\ blanks if string1 is shorter than string2. If string1 is
\ longer than string2, it is quietly truncated.

\ Exact fit helper.
: (RSET-equal) ( src slen dst dlen -- )
  drop swap move ;

\ Source shorter than destination helper.
: (RSET-less) ( c-addr1 u1 cc-addr2 u2 -- )
  {: src slen dst dlen -- :}
  dst dlen + slen - to dst
  src dst slen move ;

\ Source longer than destination helper.
: (RSET-more) {: src slen dst dlen -- :}
  src slen + dlen - dst dlen move ;

: RSET ( c-addr1 u1 c-addr2 u2 -- )
  {: src slen dst dlen -- :}
  dst dlen blank
  src slen dst dlen
  slen dlen = if (RSET-equal) else
  slen dlen < if (RSET-less) else
                 (RSET-MORE) then then ;

INCLUDE-TESTS [IF]
  cr ." TODO replace fills with STRING$ below."
  cr ." Running RSET tests..."
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
  cr
[THEN]


\ Functions:


\ >G INSTR ( c-addr1 u1 c-addr2 u2 -- u3 )
\
\ Return the offset of 1 in 2. If the string is not found
\ return -1.
\
\ Unlike the Basic version no offset to start the search from
\ is used. Adjust string 2 as needed before calling INSTR.

: INSTR ( c-addr1 u1 c-addr2 u2 -- u3 )
  {: cstr clen str slen | result -- u :}
  false to result
  str slen priorpos$
  begin
    nextpos$ dup clen < if true else
    cstr clen 2over begins$ dup to result then
  until
  result and if
    str -
  else
    drop -1
  then ;


\ >G LEFT$  ( c-addr1 u1 u2 c-addr3 u3 -- c-addr4 u4 )
\
\ Return the leftmost u2 characters of 1 in 2 as 4.
\
\ If n is zero or from_str is empty, LEFT$ returns an empty string.
\ If n is greater than the length of parent, returns parent.

: LEFT$ ( c-addr1 u1 u2 c-addr3 u3 -- c-addr4 u4 )
  {: str slen n dstr dlen | -- dstr len :}
  dstr dlen erase
  slen 0= n 0= or dlen 0= or if 0 0 exit then
  slen n < if str dstr slen move dstr slen exit then \ TODO: think about this one
  str dstr n dlen min move dstr n dlen min ;


\ >G LEN ( c-addr u -- u ) This is not really needed.

: LEN ( c-addr u -- u )
  swap drop ;


\ >G MID$ ( c-addr1 u1 u2 u3 c-addr4 u4 -- c-addr5 u4 )
\
\  substring = MID$(string, position [, length])
\
\ Returns a substring of string starting at position, counting
\ from 0 (u2) for length (u3). There is some range checking
\ here.
\
\ This could create a new string but I'm not messing with an
\ allocator yet. It returns a string pointer of
\
\ c-addr1 u2 + u3
\
\ In Basic:
\
\ a$="1234567890"
\ mid$(a$, 4, 8) -> "4567890"
\ mid$(a$, 6, 8) -> "67890"

: MID$ ( c-addr1 u1 u2 u3 c-addr u4 -- c-addr5 u4 )
  {: str slen spos sfor dstr dlen -- substr sfor :}
  spos sfor + slen > if ." MID$ invalid length" throw then
  sfor dlen > if ." MID$ invalid length" throw then
  str spos + dstr sfor move
  dstr sfor ;


\ >G RIGHT$ ( c-addr1 u1 u2 c-addr3 u3 -- c-addr4 u4 )
\
\ Returns the rightmost u2 characgers from 1 in 3. The start of
\ 4 is the same as 3. The length of 4 is the minimum of u1 and
\ u2.
\
\ If num_chars is zero or parent is empty, RIGHT$ returns an empty string.
\ If num_chars is greater than the length of parent, returns parent.

: RIGHT$ ( c-addr1 u1 u2 c-addr3 u3 -- c-addr4 u4 )
  {: str slen n dst dlen -- dst u :}
  n dlen > if dlen to n then
  slen 0= n 0= or if dst 0 exit then
  slen n < if str dst slen move dst slen exit then
  str slen + n - dst n move dst n ;










\ End of basica.fs.
