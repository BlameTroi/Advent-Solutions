\ legion.fs -- consolidating my helpers -- T.Brumley

\ "our name is legion for we are many"

\ (c) 2026 Troy Brumley
\ All my stuff is public domain via the unlicense, or if you
\ don't like that you can choose the MIT license.

\ Rather than struggle with finding the perfect grouping of
\ definitions by use, I'm pulling everything into this one
\ file and including unit tests. There will be some attempt
\ at keeping related definitions in close proximity, usually
\ by what it is (predicate, converter) or wht it operates on
\ (characters, strings, numbers).


\ What and Why ===============================================

\ I have two idea sources for this library.
\
\ * As I go through Advent of Code I look for definitions that
\   can be reused.
\ * I have a lot of Basic and Pascal habits built up over the
\   years (ok, decades). I'm implementing some of the functions
\   and procedures I found useful.
\
\ Any "original" definitions for Advent of Code will adhere to
\ Forth conventions to the extent I understand them. Naming is
\ still evolving.
\
\ Any Basicisms will follow the Microsft Basic language from
\ the 1970s and 1980s. I'm using PC-BASIC as my reference.
\
\ Any Pascalisms will be pre-Object Pascal.

\ Style and Conventions: =====================================

\ The token >G at the start of a line comment is used to
\ identify comments used to build a glossary. The format is
\ rather strict: a line comment at the start of the line
\ followed by \b>G\b identifies the glossary text:
\
\  "\ >G WORD ( in -- out ) \ and maybe other text too".
\
\ >G is case sensitive.
\
\ I know that gforth has \G for documentation generation but
\ I may want to do things that \G doesn't support.
\
\ Colon definition names should be in upper case in the
\ definition and any mention of the definition in this file.
\
\ This is meant to improve readability.
\
\ TODO: blocks? mini-glossary and a fuller "dictionary"?
\
\ Other conventions:
\
\ * I refer to a c-addr u pair as a string or string pointr.
\ * Definition names in () are to be considered private.
\ * Local variables using {: args | values -- results :} are
\   allowed but are to be avoided when possible.
\ * I strive to keep code line length limited to 64 characters
\   as that is the old blocks convention. This is a guidance
\   and not a hard rule.


\ Unit Testing ===============================================

\ The Hayes based ttester.fs exists in several places on the
\ internet and a version is supplied with gforth. To include
\ and run the tests, set the VALUE LEGION-TESTS to true.
\
\ Words controlling code inclusion will be in UPPER case.
\
\ good: LEGION-TESTS [IF] test code block [THEN]
\  bad: LEGION-TESTS [if] test code block [then]
\
\ These are VALUEs instead of CONSTANTs so they be toggled in
\ client code.

TRUE VALUE LEGION-TESTS      \ CHANGE ME AS NEEDED*********

LEGION-TESTS [IF]
  require test/ttester.fs
[THEN]


\ Some globals for testing. TODO: eliminate as many of these
\ as possible *OR* group them with the tests that use them.

LEGION-TESTS [IF]
  create blanks 128 allot
  blanks 128 bl fill
  : SASDF s" asdf" ;       \ 4
  : SQWERTY s" qwerty" ;   \ 5
  : SIJKL s" IJKL" ;       \ 4
  : SMIXED s" This is a mixed case sentence." ;
  : SUPPER s" THIS IS A MIXED CASE SENTENCE." ;
  : SLOWER s" this is a mixed case sentence." ;
  : SALPHA s" all alphaBETIC" ;
  : SNUMERIC s" 12345" ;
  : SALPHANUMERIC s" The item costs 123 USD" ;
  : S1234 s" 1234" ;
  : S12345678 s" 12345678" ;
  : S12341234 s" 12341234" ;
[THEN]


\ The Really Small Stuff =====================================
\
\ These are minor definitions and types. These don't generally
\ have unit tests. They are small, "intuitively obvious", and
\ exercised by later defintions that are unit tested.
\
\ I also create wrappers for some standard words where the
\ preamble stack set up rarely or never changes.


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


\ >G BL>PAD ( -- )

: BL>PAD pad 84 blank ;


\ >G NUL>PAD ( -- )
: NUL>PAD pad 84 erase ;


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


\ >G BEGINS$ ( c-addr1 u1 c-addr2 u2 -- flag )
\ Does 1 occupy the leftmost part of 2?

: BEGINS$ ( c-addr1 u1 c-addr2 u2 -- flag )
  {: beg blen str slen | -- flag :}
  blen slen <= if beg blen str blen compare 0=
               else false then ;

LEGION-TESTS [IF]
  cr ." Running BEGINS$ tests..."
  T{ s" 123" s" 123456789" BEGINS$ -> true }T
  T{ s" 456" s" 123456789" BEGINS$ -> false }T
  T{ s" 789" s" 123456789" BEGINS$ -> false }T
  cr
[THEN]


\ >G ENDS$ ( c-addr1 u1 c-addr2 u2 -- flag )
\ Does 1 occupy the rightmost part of 2?

: ENDS$ ( c-adr1 u1 c-addr2 u2 -- flag )
  {: end elen str slen | -- flag :}
  elen slen <= if end elen str slen + elen - elen compare 0=
             else false then ;
LEGION-TESTS [IF]
  cr ." Running ENDS$ tests..."
  T{ s" 123" s" 123456789" ENDS$ -> false }T
  T{ s" 456" s" 123456789" ENDS$ -> false }T
  T{ s" 789" s" 123456789" ENDS$ -> true }T
  cr
[THEN]


\ >G N-SUB4 ( c-addr u n - u/n flag )
\ Can this string be split into equal sized chunks?

: ?N-SUB$ ( c-addr u n -- un/ flag ) ( c-addr discarded )
  rot drop /mod swap 0= ;


\ Sometimes a repeating pattern in a string must be handled.
\ This pair take a string and compare one piece of it to
\ another. Given c-addr u this comparison is:
\
\ c-addr u c-addr+u u compare 0=


\ >G ?ADJACENT$-EQUAL ( c-addr u -- flag )
: ?ADJACENT$-EQUAL ( c-addr u -- flag )
  2dup dup >r + r> compare 0= ;


\ >G ?2-SUB$-EQUAL ( c-addr u -- flag )
: ?2-SUB$-EQUAL ( c-addr u -- flag )
    1 rshift  ?ADJACENT$-EQUAL ;



\ String/Character Iterators =================================


\ A general iterator for string predicates. c@-next$ updates
\ the string pointer with each call. Returns false with the
\ string pointer to the failing character or true with the
\ string pointer pastt he end of the original string.

\ >G ?ALL-SATISFY-PRED$ ( c-addr1 u1 xt -- c-addr2 u2 flag )
\ Do the characters of a string satisfy a predicate (given as
\ an execute token)?

: ?ALL-SATISFY-PRED$ ( c-addr1 u1 xt -- c-addr2 u2 flag )
  {: str len xt | result -- str2 len2 flag :}
  str len true to result
  dup 0 do
    C@-NEXT$ xt execute 0=
    if 1+ swap 1- swap false to result leave then
  loop result ;


\ >G ?ALL-SAME-CHAR$ ( c-addr1 u1 -- c-addr2 u2 flag )
\ Are all of the characters in a string the same?
\
\ Returns false and the substring starting from mismatched
\ character or true and the string pointer past the end of
\ the original string.

: ?ALL-SAME-CHAR$ ( c-addr1 u1 -- c-addr2 u2 flag )
  {: str len | chr result -- str2 len2 flag :}
  str len true to result
  over c@ to chr
  dup 0 do
    C@-NEXT$ chr <>
    if 1+ swap 1- swap false to result leave then
  loop result ;


\ >G ?N-SUB$-EQUAL ( c-addr u n -- flag )
\ Are the n adjacent substrings starting at c-addr the same?

: ?N-SUB$-EQUAL ( c-addr u n -- flag )
  {: ( str len n ) | result -- flag :}
  true to result
  1 do ( n segments require n 1- compares, so start at 1 )
    2dup 2dup + over
    compare if false to result leave then
    swap over + swap
  loop
  2drop result ;


\ Obvious Predicates. ========================================


\ Single character predicates and then full string versions
\ using ?ALL-SATISFY-PRED$.
\
\ For the string version the string pointer is updated
\ as in ?ALL-SATISFY-PRED$.


\ >G ?DIGIT ( c -- f )
: ?DIGIT ( c -- f )  48 58 within ;

\ >G ?ISLOWER ( c -- f )
: ?LOWER ( c -- f )  97 123 within ;

\ >G ?ISUPPER ( c -- f )
: ?UPPER ( c -- f )  65 91 within ;

\ >G ?WHITE ( c -- f )
: ?WHITE ( c -- f ) dup 8 14 within swap 32 = or ;

\ >G ?ALPHA ( c -- f )
: ?ALPHA ( c -- f ) dup ?LOWER swap ?UPPER or ;

\ >G ?ALPHANUM ( c -- f )
: ?ALPHANUM ( c -- f ) dup ?DIGIT swap ?ALPHA or ;


\ >G ?DIGIT$ ( c-addr u -- c-addr2 u2 f )
: ?DIGIT$ ['] ?DIGIT ?ALL-SATISFY-PRED$ ;

\ >G ?LOWER$ ( c-addr u -- c-addr2 u2 f )
: ?LOWER$ ['] ?LOWER ?ALL-SATISFY-PRED$ ;

\ >G ?UPPER$ ( c-addr u -- c-addr2 u2 f )
: ?UPPER$ ['] ?UPPER ?ALL-SATISFY-PRED$ ;

\ >G ?ALPHA$ ( c-addr u -- c-addr2 u2 f )
: ?ALPHA$ ['] ?ALPHA ?ALL-SATISFY-PRED$ ;

\ >G ?ALPHANUM$ ( c -- f )
: ?ALPHANUM$ ['] ?ALPHANUM ?ALL-SATISFY-PRED$ ;


\ Obvious Converters. ========================================

\ Several common character converters. These naturally factor
\ into three definitions per conversion. From most to least
\ priitive:
\
\ * A single character converter C>SOMETHING.
\ * A single character converter at a position in a string
\   C>SOMETHING$.
\ * A convert the whole string using the prior two definitions
\   S>SOMETHING$.


\ >G C>UPPER ( c -- C )
: C>UPPER ( c -- C )  dup ?LOWER if 32 - then ;

\ >G C>LOWER ( C -- c )
: C>LOWER ( C -- c )  dup ?UPPER if 32 + then ;


\ For the character in string definitions, the string pointer
\ is advanced. The flag becomes false once the string is
\ consumed.

\ >G C>UPPER$ ( c-addr u -- c-addr+1 u-1 flag )
: C>UPPER$ ( c-addr u -- c-addr+1 u-1 flag )
  dup 0< if false else
  over dup c@ C>UPPER swap c! 1- swap 1+ swap true then ;

\ >G C>LOWER$ ( c-addr u -- c-addr+1 u-1 flag )
: C>LOWER$ ( c-addr u -- c-addr+1 u-1 flag )
  dup 0< if false else
  over dup c@ C>LOWER swap c! 1- swap 1+ swap true then ;


\ The string definitions pass each character to the appropriate
\ s>$ converters.

\ >G S>UPPER$ ( c-addr u -- )
: s>upper$ ( c-addr u -- )
  begin C>UPPER$ 0= until 2drop ;

\ >G S>LOWER$ ( c-addr u -- )
: S>LOWER$ ( c-addr u -- )
  begin C>LOWER$ 0= until 2drop ;


\ Explicitly Basic Definitions ===============================
\
\ Here are string manipulation definitions that follow
\ Microsoft's PC era BASICA. In that parlance, a statement is
\ a modifier or l-value, while a function is r-values.
\
\ Statement: LSET some string$ = some string expression
\
\ Function:  INSTR(string to find, string to search)
\
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

LEGION-TESTS [IF]
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


\ >G MSET ( c-addr1 u1 c-addr2 u2 u3 -- )
\ Copy 1 into 2+u3, watching for overflows.
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

LEGION-TESTS [IF]
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


\ >G RSET ( c-addr1 u1 c-addr2 u2 -- )
\ right justify 1 inoto 2 padding with blanks
\
\ Copies a string1 to string2 right justified with leading
\ blanks if string1 is shorter than string2. If string1 is
\ longer than string2, it is quietly truncated.

\ Private helpers for length relationships:
\ source <=> destination.

: (RSET-equal) ( c-addr1 u1 c-addr2 u2 -- )
  drop swap move ;

: (RSET-less) ( c-addr1 u1 c-addr2 u2 -- )
  {: src slen dst dlen -- :}
  dst dlen + slen - to dst
  src dst slen move ;

: (RSET-more) ( c-addr1 u1 c-addr2 u2 -- )
  {: src slen dst dlen -- :}
  src slen + dlen - dst dlen move ;

\ The callable RSET.

: RSET ( c-addr1 u1 c-addr2 u2 -- )
  {: src slen dst dlen -- :}
  dst dlen blank
  src slen dst dlen
  slen dlen = if (RSET-equal) else
  slen dlen < if (RSET-less) else
                 (RSET-MORE) then then ;

LEGION-TESTS [IF]
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


\ >G INSTR ( c-addr1 u1 c-addr2 u2 -- u3 )
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

LEGION-TESTS [IF]
  cr ." Running INSTR tests..."
  T{ s" 124" s" 012345" INSTR -> -1 }T   ( not found )
  T{ s" 123" s" 012345" INSTR -> 1 }T    ( found @ 1 )
  T{ s" 012" s" 012345" INSTR -> 0 }T    ( found @ 0 )
  T{ s" 013" s" 012345" INSTR -> -1 }T   ( not found )
  T{ s" 123" s" 12"     INSTR -> -1 }T   ( fails target short )
  T{ s" 45"  s" 012345" INSTR -> 4 }T    ( check tail boundary )
  T{ s" 5"   s" 012345" INSTR -> 5 }T
  cr
[THEN]


\ >G LEFT$  ( c-addr1 u1 u2 c-addr3 u3 -- c-addr4 u4 )
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

LEGION-TESTS [IF]
  cr ." Running LEFT$ tests..."
  T{ pad 84 erase   ( truncate in destination )
     s" 0123456789" ( source string )
     dup            ( use its entire length )
     pad 84         ( and place it in a larger string )
     LEFT$
     -> pad s" 0123456789" swap drop }T \ pad l'source
  T{ pad 84 erase     ( source smaller than destination )
     s" 0123456789" dup      ( source length, length to use )
     pad over 2/             ( destination is half the length of source )
     LEFT$
     s" 0123456789" 2/ compare
     pad 5 + c@ -> 0 0 }T
  cr
[THEN]


\ >G LEN ( c-addr u -- u )
\ String used length. Included for completeness but I don't
\ test it and don't expect to use it. TODO: with or without
\ -TRAILING?

: LEN ( c-addr u -- u )
  swap drop ;


\ >G MID$ ( c-addr1 u1 u2 u3 c-addr4 u4 -- c-addr5 u4 )
\ Return a substring of string starting at position, counting
\ from 0 (u2) for length (u3).
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

LEGION-TESTS [IF]
  cr ." Running MID$ tests..."
  \ Some of these aren't realistic...checking edges.
  T{ pad 84 erase            ( lengths equal )
     s" 0123456789" 0 10 pad 10 MID$
     s" 0123456789" compare -> 0 }T
  \ zero length
  T{ pad 84 erase            ( at least one length zero )
     s" 0123456789" 0 0 pad 10 MID$
     s" 0123456789" drop 0 compare -> 0 }T
  T{ pad 84 erase            ( show's truncating output )
     s" 0123456789" 0 3 pad 10 MID$
     3 = swap pad = and
     s" 0123456789" drop 3 pad 3 compare -> -1 0 }T
   cr
[THEN]


\ >G RIGHT$ ( c-addr1 u1 u2 c-addr3 u3 -- c-addr4 u4 )
\ Return the rightmost u2 characters of string 3.
\
\ Returns the rightmost u2 characters from 1 in 3. The start of
\ 4 is the same as 3. The length of 4 is the minimum of u1 and
\ u2.
\
\ If u2 is zero or parent is empty, returns an empty string.
\ If u2 is greater than the length of parent, returns parent.

: RIGHT$ ( c-addr1 u1 u2 c-addr3 u3 -- c-addr4 u4 )
  {: str slen n dst dlen -- dst u :}
  n dlen > if dlen to n then
  slen 0= n 0= or if dst 0 exit then
  slen n < if str dst slen move dst slen exit then
  str slen + n - dst n move dst n ;

LEGION-TESTS [IF]
  cr ." Running RIGHT$ tests..."
  T{ pad 84 erase            ( truncate if no room )
     s" 0123456789" 10 pad 3 RIGHT$
     s" 789" compare -> 0 }T
  T{ pad 84 erase            ( empty if source length 0 )
     s" 0123456789" drop 0 10 pad 10 RIGHT$
     -> pad 0 }T
  T{ pad 84 erase            ( empty if destination length 0 )
     s" 0123456789" 0 pad 10 RIGHT$
     -> pad 0 }T
  T{ pad 84 erase            ( non edge case )
     s" 0123456789" 5 pad 10 RIGHT$
     2dup s" 56789" compare
     -> pad 5 0 }T
  cr
[THEN]


\ Converters =================================================

\ Copy a string to a counted string. Will not overflow the
\ allowed length of the counted string (u2).

: S$>C$ ( c-addr1 u1 c-addr2 u2 -- )
   2dup erase  1- swap 1+ swap  rot min
   2dup swap 1- c!  move ;

\ >G U>$ ( u c-addr u1 -- c-addr u2 )
\ Convert an unsigned single to a string and save it in the
\ supplied buffer. The buffer is set to blanks before the
\ conversion. Returns string address and length of the result,
\ which is left justified in buffer. The length is exact, so
\ there is no trailing blank in the length.
\
\ A buffer overflow is silently truncated.

: U>$ ( u c-addr u1 -- c-addr u2 )
  2dup blank                     \ erase
  rot 0 <# #s #>                 \ u c-addr1 u1 c-addr2 u2
  swap >r min 2dup r> -rot       \ addr addr um
  move ;                         \


\ General String Definitions =================================

\ >G SAFE-MOVE$ ( c-addr1 u1 c-addr2 u2 -- )
\ safe-move copies up to u2 characters from addr1 to addr2.
\ If u1 < u2, only u1 characters are copied. If u2 > u1, only
\ u1 characters are copied and the remaining portion at addr2
\ is left unchanged.

: SAFE-MOVE$ ( c-addr1 u1 c-addr2 u2 -- )
  {: str1 len1 str2 len2 -- :}
  len1 len2 min str1 swap str2 over move ;


\ Finders: ===================================================

\ >G CPOSIN$ ( c-caddr1 u1 -- c-addr1+? u1-? )
\ Find the location of character c in a standard string. The
\ return is a string starting from that character. If the
\ character is not found, a string of length 0 is returned.
\
\ For repeated calls forward through a string the caller must
\ adjust the string.
\
\ '9' ." 9823" CPOSIN$ '8' -rot 1- swap 1+ swap CPOSIN$

\ TODO: Pick one of CPOSIN$ and CIN$, use name CIN$.

: CPOSIN$ ( c c-addr u -- c-addr2 u2 , c-addr+u 0 if not found )
  rot >r  ( stash char ) 1+ swap 1- swap ( fix up for start )
  begin
    1- swap 1+ swap                   ( adjust )
    2dup 0= if drop true else c@ r@ = then ( match? )
  until
  r> drop ( discard char ) ;


\ >G CIN$ ( c c-addr1 u1 -- c-addr2 u2 flag )
\ Find the first occurrence of character c in a string.
\
\ If not found, flag is false and the string pointer is past
\ the end of the original string. If found, flag is true and
\ the string pointer is positioned on that character.

: CIN$ ( c c-addr1 u1 -- c-addr2 u2 flag )
  {: chr str len | idx flag -- str2 len2 flag :}
  false to flag len to idx
  str len 0 do
    dup i + c@ chr = if i to idx true to flag leave then
  loop
  idx + len idx - flag ;


\ >G LARGEST-CHAR-IN ( c-addr1 u1 -- c-addr1+? u1-? )
\ Find the largest character in a string, returning the string
\ pointer on the largest found.

: LARGEST-CHAR-IN ( c-addr1 u1 -- c-addr1+? u1-? )
  {: str len | chr idx -- str2 len2 :}
  str c@ to chr 0 to idx ( assume first is largest )
  len 1 do
    str i + c@ dup chr > if to chr i to idx else drop then
  loop
  str idx + len idx - ( locate the character ) ;


\ >G CAPPEND$ ( c c-addr u -- c-addr u+1 )
\ Append a character to a string. There are no overflow checks.

\ TODO: Can I do this better?

: CAPPEND$ ( c c-addr u -- c-addr u+1 )
  >r  ( c c-addr ; r: u )
  swap over ( c-addr c c-addr ; r: u )
  r@ + ( c-addr c c-addr2 ; r: u )
  c! ( c-addr ; r: u )
  r@ + r> ; ( c-addr u2 )








\ Futures: ===================================================

\ Common List Operations =====================================
\
\ As in SML or Scheme. Not impemented yet, this is more of a to
\ do list.
\
\ In order to implement these I need to decide what constitutes
\ a 'list' for these functions.
\
\ APPEND (given two lists, add all items in the second list to
\ the end of the first list).
\
\ CONCATENATE (given a series of lists, combine all items in
\ all lists into one flattened list);
\
\ FILTER (given a predicate and a list, return the list of all
\ items for which predicate(item) is True);
\
\ LENGTH (given a list, return the total number of items within
\ it);
\
\ MAP (given a function and a list, return the list of the
\ results of applying function(item) on all items);
\
\ FOLDL (given a function, a list, and initial accumulator,
\ fold (reduce) each item into the accumulator from the left);
\
\ FOLDR (given a function, a list, and an initial accumulator,
\ fold (reduce) each item into the accumulator from the right);
\
\ REVERSE (given a list, return a list with all the original
\ items, but in reversed order).




\ End of legion.fs
