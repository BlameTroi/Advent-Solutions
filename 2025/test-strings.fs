\ test-strings.fs -- exercise strings -- T.Brumley.

require test/ttester.fs

DECIMAL

\ Test data.

: sasdf s" asdf" ;       \ 4
: sqwerty s" qwerty" ;   \ 5
: sIJKL s" IJKL" ;       \ 4
: smixed s" This is a mixed case sentence." ;
: supper s" THIS IS A MIXED CASE SENTENCE." ;
: slower s" this is a mixed case sentence." ;
: salpha s" all alphaBETIC" ;
: snumeric s" 12345" ;
: salphanumeric s" The item costs 123 USD" ;
: s1234 s" 1234" ;
: s12345678 s" 12345678" ;
: s12341234 s" 12341234" ;

\ Helpers.

: bl-pad pad 84 blank ;
: 00-pad pad 84 erase ;

\ Types.

cr ." Testing string$ type: " cr

\ The third test coincidentally tests string is all the same
\ character and fetch next character from string.

T{ 256 string$ s256 -> }T   \ TODO: address of?
T{ s256 swap drop -> 256 }T
T{ s256 ?all-same-char$ -> true }T
T{ 'a' S256 drop c! s256 ?all-same-char$ -> false }T

\ Number to string.

cr ." Testing number to string: " cr

T{ 00-pad 1234 pad 84 u>$ -> pad 4 }T
T{ 00-pad 1234 pad 84 u>$ 2drop pad 4 + c@ -> 0 }T
T{ 00-pad 123456 pad 4 u>$ -> pad 4 }T
T{ 00-pad 123456 pad 4 u>$ s" 1234" compare -> 0 }T

\ Case conversion.

cr ." Testing case conversion: " cr

T{ 00-pad smixed pad swap move pad smixed swap drop s>upper$
  supper dup pad swap compare -> 0 }T
T{ 00-pad supper pad swap move pad supper swap drop s>lower$
  slower dup pad swap compare -> 0 }T

cr ." Testing string to counted string: " cr

\ s12345678 is 8 bytes, so it needs byte area to fully store as
\ a counted string. Second and third tests show that there is
\ no overflow of the destination. The fourth test shows that
\ the space for the counted string is erased.

T{ 00-pad s12345678 pad 9 s$>c$ pad count s12345678 compare -> 0 }T
T{ 00-pad s12345678 pad 5 s$>c$ pad count s12345678 4 - compare -> 0 }T
T{ pad c@ pad 1+ c@ pad 4 + c@ pad 5 + c@ -> 4 '1' '4' 0 }T
T{ bl-pad s1234 pad 9 pad 5 + 'x' swap c! pad 6 + 'y' swap c! s$>c$ pad count s1234 compare pad 5 + c@ pad 6 + c@ pad 9 + c@ -> 0 0 0 32 }T

cr ." Testing fetch next character from string: " cr

\ The second and third tests show that the fetch will not read
\ past the end of the string.

T{ 00-pad S1234 pad swap move pad 4 c@-next$ -> pad 1+ 3 '1' }T
T{ 00-pad S1234 drop pad 2 move pad 2 c@-next$ drop c@-next$ -> pad 2 + 0 '2' }T
T{ 00-pad S1234 pad swap move pad 2 c@-next$ drop c@-next$ drop c@-next$ -> pad 2 + 0 0 }T

cr ." Testing string chop and adjacency definitions: " cr

T{ S1234 1 ?n-sub$ -> 4 true }T
T{ S1234 2 ?n-sub$ -> 2 true }T
T{ s1234 3 ?n-sub$ -> 1 false }T
T{ s1234 4 ?n-sub$ -> 1 true }T
T{ s12341234 drop 2 ?adjacent$-equal -> false }T
T{ s12341234 drop 4 ?adjacent$-equal -> true }T
T{ s12345678 drop 4 ?adjacent$-equal -> false }T

cr ." Testing single character predicates (for completeness): " cr

T{ '0' ?isdigit '9' ?isdigit 'a' ?isdigit -> true true false }T

cr ." Testing cin$: " cr ( c c-addr u -- c-addr2 u2 flag )

: test98 s" 987654321111111" ;
: test89 s" 811111111111119" ;
: test78 s" 234234234234278" ;
: test92 s" 818181911112111" ;

verbose on

\ Find or not find from start:

T{ '9' test98 cin$ cr .s cr -> test98 drop 1+ test98 swap drop 1- true }T
T{ '8' test98 cin$ cr .s cr -> true }T
T{ '0' test98 cin$ cr .s cr -> false }T

\ \ Try repeated find:

\ cr ." Seek 98 in " test98 type cr

\ T{
\   '9' test98 cin$ drop ( flag )
\   hex cr .s cr decimal
\   '8' -rot cin$
\   hex cr .s cr decimal
\   -> true
\   }T

\ cr ." Seek 89 in " test89 type cr

\ T{
\   '9' test89 cin$ drop ( flag )
\   hex cr .s cr decimal
\   '8' -rot cin$
\   hex cr .s cr decimal
\   -> false
\   }T

cr ." End of strings.fs tests! " cr

\ End of test-strings.fs