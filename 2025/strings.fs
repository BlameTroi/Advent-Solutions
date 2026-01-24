\ strings.fs -- Some common words -- T.Brumley

BASE @
DECIMAL


\ A character string with a maximum length and placed on the
\ stack as c-addr u.
\
\ TODO: alternate initializers.
\ TODO: byte strings.

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
   drop swap      ( s: a1 u? a2 )
   move ;


\ Copy a string to a counted string. Will not overflow the
\ allowed length of the counted string (u2).

: s$>c$ ( c-addr1 u1 c-addr2 u2 -- )
   2dup erase  1- swap 1+ swap  rot min
   2dup swap 1- c!  move ;


\ Fetch the next character from a string, returns 0 if length
\ remaining < 1. This is meant to be part of an iterator over
\ a string.

: c@-next$ ( c-addr u -- c-addr2 u2 c )
  dup 1 < if 0 else over c@ -rot 1- swap 1+ swap rot then ;


\ Iterate over the characters in a string and perform some
\ test.

\ Is this string a single repeating character?

0 value asc-char
0 value asc-result
: ?all-same-char$ ( c-addr u -- flag )
  c@-next$ to asc-char true to asc-result
  dup 0 do
    c@-next$ asc-char <> if false to asc-result leave then
  loop 2drop asc-result ;

\ A general iterator for string predicates.

0 value asp-result
0 value asp-pred
: ?all-satisfy-pred$ ( c-addr u xt -- c-addr2 u2 flag )
  to asp-pred true to asp-result
  dup 0 do
    c@-next$ asp-pred execute 0= if false to asp-result leave then
  loop asp-result ;


\ Convert an unsigned single to a string and save it in the
\ supplied buffer. Returns string address and length of the
\ result, which is left justified in buffer. If the result
\ would overflow the buffer it is quietly truncated. Unlike
\ "." there is no trailing blank added and any unused bytes
\ in the destination are left unchanged.

: u>$ ( u c-addr u1 -- c-addr u2 )
  rot 0 <# #s #>                 \ u c-addr1 u1 c-addr2 u2
  swap >r min 2dup r> -rot       \ addr addr um
  move ;                         \


\ Can this string be divided into n equally sized substrings?
\ If so, how long are the substrings?

: ?n-sub$ ( c-addr u n -- un/ flag ) ( c-addr discarded )
  rot drop /mod swap 0= ;

\ Compare two adjacent strings of length u.

: ?adjacent$-equal ( c-addr u -- flag )
  2dup dup >r + r> compare 0= ;

\ A special case I kept because it was already baked in.

: ?2-sub$-equal ( c-addr u -- flag )
    1 rshift  ?adjacent$-equal ;

\ Compare n adjacent strings of length u. So the entire string
\ would be n u * bytes long.

variable n-sub$-equal ( unfortunately I needed a variable )
: ?n-sub$-equal ( c-addr u n )
  n-sub$-equal on
  1 do ( n segments require n 1- compares, so start at 1 )
    2dup 2dup + over
    compare if n-sub$-equal off leave then
    swap over + swap
  loop
  2drop n-sub$-equal @ ;


\ Several common character predicates.

: ?isdigit ( c -- f )  48 58 within ;
: ?islower ( c -- f )  97 123 within ;
: ?isupper ( c -- f )  65 91 within ;
: ?iswhite ( c -- f ) dup 8 14 within swap 32 = or ;
: ?isalpha ( c -- f ) dup ?islower swap ?isupper or ;
: ?isalphanum ( c -- f ) dup ?isdigit swap ?isalpha or ;


\ And now predicates for strings. There's an opportunity for
\ a defining word or some other way of passing an xt to the
\ ?all-satisfy-pred$ definition, but for now I just use brute
\ force.
\
\ Given a string, returns the position where the predicate
\ failed if false, or the end of the string if the predicate
\ succeeded.

\ All of these: ( c-addr u -- c-addr2 u2 f )
: ?s>isdigit$ ['] ?isdigit ?all-satisfy-pred$ ;
: ?s>islower$ ['] ?islower ?all-satisfy-pred$ ;
: ?s>isupper$ ['] ?isupper ?all-satisfy-pred$ ;
: ?s>isalpha$ ['] ?isalpha ?all-satisfy-pred$ ;
: ?s>isalphanum ['] ?isalphanum ?all-satisfy-pred$ ;


\ Several common character converters. The final s>.....$
\ definitions use the c> and c>$ definitions.
\
\ The general structure is:
\
\ A single character converter c>something.
\
\ A single character converter at a position in a string
\ c>something$.
\
\ A convert the whole string using the prior two definitions
\ s>something$.

: c>upper ( c -- C )  dup ?islower if 32 - then ;

: c>lower ( C -- c )  dup ?isupper if 32 + then ;

: c>upper$ ( c-addr u -- c-addr2 u2 f )
  dup 0< if false else
  over dup c@ c>upper swap c! 1- swap 1+ swap true then ;

: c>lower$ ( c-addr u -- c-addr2 u2 f )
  dup 0< if false else
  over dup c@ c>lower swap c! 1- swap 1+ swap true then ;

: s>upper$ ( c-addr u -- )
  begin c>upper$ 0= until 2drop ;

: s>lower$ ( c-addr u -- )
  begin c>lower$ 0= until 2drop ;


\ Find the first occurrence of character c in a string.

\ TODO: This is very unsatisfying, but it does work. It needs a
\ rewrite.

: ?cin$ ( c c-addr u -- c-addr u flag )
  2dup { c c-addr u saveu savea } false { result }
  begin
    u 1- dup to u               ( adjust remaining after this )
    0< if                       ( we ran out of string )
      savea saveu true          ( reset to not found config and exit until )
    else
      c-addr dup c@ swap 1+ to c-addr ( get char, bump to next )
      c = if                    ( match )
        true to result          ( remember )
        c-addr u true           ( reset to found config and exit until )
      else
        false                   ( continue )
      then
    then
  until
  result ;

BASE !

\ End of strings.fs