\ basica.fs -- An implementation of MS Basic String functions -- T.Brumley

BASE @
DECIMAL


\ Some string functions and procedures from the old BASIC days.
\
\ Procedure   Description
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


\ Statements:

\ LSET ( c-addr1 u1 c-addr2 u2 -- )
\
\ Copies a string1 to string2 left justified and blank
\ padded if string1 is shorter than string2. If string1
\ is longer than string2, it is quietly truncated.

: LSET ( c-addr1 u1 caddr2 u2 -- ) \ {: src slen dst dlen -- :}
  2dup blank
  rot min move ;


\ In Basic, MID$ is a function when an r-value and a statement
\ when an l-value. I am renaming the statement form as:
\
\ MSET ( c-addr1 u1 c-addr2 u2 u3 -- )
\
\ Copy sub to dst+dpos clipping sub to fit from dpos to dlen.

: MSET {: sub slen dst dlen dpos | droom actlen -- :}
  dpos dlen >= dpos 0< or if ." MSET invalid arguments." throw 1 then
  dlen dpos - ( determine destination room )
  to droom
  droom slen min to actlen
  sub dst dpos + actlen move ;


\ RSET ( c-addr1 u1 c-addr2 u2 -- )
\
\ Copies a string1 to string2 right justified with leading
\ blanks if string1 is shorter than string2. If string1 is
\ longer than string2, it is quietly truncated.
\ off by one, move to target is 1 less than it should be,
\ and the move length is 1 more than it should be.

\ Exact fit helper.
: (RSET-equal) ( src slen dst dlen -- )
  drop swap move ;

\ Source shorter than destination helper.
: (RSET-less) {: src slen dst dlen -- :}
  dst dlen + slen - to dst
  src dst slen move ;

\ Source longer than destination helper.
: (RSET-more) {: src slen dst dlen -- :}
  src slen + dlen - dst dlen move ;

: RSET {: src slen dst dlen -- :}
  dst dlen blank
  src slen dst dlen
  slen dlen = if (RSET-equal) else
  slen dlen < if (RSET-less) else
                 (RSET-MORE) then then ;


\ Functions:


\ INSTR ( u c-addr1 u1 c-addr2 u2 -- u3 )
\
\ Return the offset of 1 in 2 starting from u. If the
\ string is not found return -1.

: INSTR {: cstr clen str slen | result -- u :}
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


\ LEFT$  ( from-str from-u n dest-str dest-u )
\
\ Returns the leftmost n characters of from-str.
\
\ If n is zero or from_str is empty, LEFT$ returns an empty string.
\ If n is greater than the length of parent, returns parent.

: LEFT$ {: str slen n dstr dlen | -- dstr len :}
  dstr dlen erase
  slen 0= n 0= or dlen 0= or if 0 0 exit then
  slen n < if str dstr slen move dstr slen exit then \ TODO: think about this one
  str dstr n dlen min move dstr n dlen min ;


\ LEN ( c-addr u -- u ) Yes, it's silly.

: LEN ( c-addr u -- u )
  swap drop ;


\ MID$ ( c-addr1 u1 u2 u3 c-addr4 u4 -- c-addr5 u4 )
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

: MID$ {: str slen spos sfor dstr dlen -- substr sfor :}
  spos sfor + slen > if ." MID$ invalid length" throw then
  sfor dlen > if ." MID$ invalid length" throw then
  str spos + dstr sfor move
  dstr sfor ;


\ RIGHT$ ( c-addr1 u1 n c-addr2 u2 -- c-addr2 min(u1 n) )
\
\ Returns the rightmost num_chars characters of parent.
\ If num_chars is zero or parent is empty, RIGHT$ returns an empty string.
\ If num_chars is greater than the length of parent, returns parent.

: RIGHT$ {: str slen n dst dlen -- dst u :}
  n dlen > if dlen to n then
  slen 0= n 0= or if dst 0 exit then
  slen n < if str dst slen move dst slen exit then
  str slen + n - dst n move dst n ;


\ Both of these are variations on fill. No testing needed.
\ SPACE$      Repeat spaces
\ STRING$     Repeat characters

: SPACE$ ( c-addr u n -- c-addr min-un )
  min 2dup bl fill ;

: STRING$ ( c-addr u n c -- c-addr min-un )
  >r min 2dup r> fill ;







BASE !

\ End of basica.fs.