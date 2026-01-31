\ strings.fs -- Some common words -- T.Brumley

BASE @
DECIMAL


\ A character string with a maximum length and placed on the
\ stack as c-addr u. The string is initialized to blanks.
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

: safe-move$ {: str1 len1 str2 len2 -- :}
  len1 len2 min str1 swap str2 over move ;


\ Copy a string to a counted string. Will not overflow the
\ allowed length of the counted string (u2).

: s$>c$ ( c-addr1 u1 c-addr2 u2 -- )
   2dup erase  1- swap 1+ swap  rot min
   2dup swap 1- c!  move ;


\ Fetch the next character from a string, returns 0 if length
\ remaining < 1. This is meant to be part of an iterator over
\ a string.

: nextpos$ ( c-addr u -- c-addr+1 u-1 , 0 if can't advance )
  dup if 1- swap 1+ swap then ;

: c@-next$ ( c-addr u -- c-addr2 u2 c )
  dup 1 < if 0 else over c@ -rot 1- swap 1+ swap rot then ;



\ Find the location of character c in a standard string. The
\ return is a string starting from that character. If the
\ character is not found, a string of length 0 is returned.
\
\ For repeated calls forward through a string the caller must
\ adjust the string.
\
\ '9' ." 9823" cposin$ '8' -rot 1- swap 1+ swap cposin$

: cposin$ ( c c-addr u -- c-addr2 u2 , c-addr+u 0 if not found )
  rot >r  ( stash char ) 1+ swap 1- swap ( fix up for start )
  begin
    1- swap 1+ swap                   ( adjust )
    2dup 0= if drop true else c@ r@ = then ( match? )
  until
  r> drop ( discard char ) ;


\ Find the largest character in a string, returning the string
\ pointer on the largest found.

: largest-char-in {: str len | chr idx -- str2 len2 :}
  str c@ to chr 0 to idx ( assume first is largest )
  len 1 do
    str i + c@ dup chr > if to chr i to idx else drop then
  loop
  str idx + len idx - ( locate the character ) ;

\ Append a character to a string. There are no overflow checks.

\ this is the right general idea but I'm too fuzzy headed to
\ get it right. Later.
: cappend$ ( c c-addr u -- c-addr u+1 )
  >r  ( c c-addr ; r: u )
  swap over ( c-addr c c-addr ; r: u )
  r@ + ( c-addr c c-addr2 ; r: u )
  c! ( c-addr ; r: u )
  r@ + r> ; ( c-addr u2 )

\ Iterate over the characters in a string and perform some
\ test.

\ Is this string a single repeating character? Returns false
\ and the substring starting from mismatched character or true
\ and the string pointer past the end of the original string.

: ?all-same-char$ {: str len | chr result -- str2 len2 flag :}
  str len true to result
  over c@ to chr
  dup 0 do
    c@-next$ chr <> if 1+ swap 1- swap false to result leave then
  loop result ;


\ A general iterator for string predicates. c@-next$ updates
\ the string pointer with each call. Returns false with the
\ string pointer to the failing character or true with the
\ string pointer pastt he end of the original string.

: ?all-satisfy-pred$ {: str len xt | result -- str2 len2 flag :}
  str len true to result
  dup 0 do
    c@-next$ xt execute 0= if 1+ swap 1- swap false to result leave then
  loop result ;


\ Convert an unsigned single to a string and save it in the
\ supplied buffer. The buffer is set to blanks before the
\ conversion. Returns string address and length of the result,
\ which is left justified in buffer. The length is exact, so
\ unlike "." there is no trailing blank in the length. If the
\ result would overflow the buffer it is quietly truncated.

: u>$ ( u c-addr u1 -- c-addr u2 )
  2dup blank                     \ erase
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

: ?n-sub$-equal {: ( str len n ) | result -- flag :}
  true to result
  1 do ( n segments require n 1- compares, so start at 1 )
    2dup 2dup + over
    compare if false to result leave then
    swap over + swap
  loop
  2drop result ;


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
: ?isdigit$ ['] ?isdigit ?all-satisfy-pred$ ;
: ?islower$ ['] ?islower ?all-satisfy-pred$ ;
: ?isupper$ ['] ?isupper ?all-satisfy-pred$ ;
: ?isalpha$ ['] ?isalpha ?all-satisfy-pred$ ;
: ?isalphanum$ ['] ?isalphanum ?all-satisfy-pred$ ;


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


\ Find the first occurrence of character c in a string. Return
\ an updated string pointer and a flag. If the string was not
\ found the string poiner points just past the input string.


: cin$ {: chr str len | idx flag -- str2 len2 flag :}
  false to flag len to idx
  str len 0 do
    dup i + c@ chr = if i to idx true to flag leave then
  loop
  idx + len idx - flag ;


BASE !

\ End of strings.fs