\ 2025 Day 2 parts 1 & 2 test cases -- T. Brumley.


\ 1-22,95-115,998-1012,1188511880-1188511890,222220-222224,
\ 1698522-1698528,446443-446449,38593856-38593862,
\ 565653-565659,824824821-824824827,2121212118-2121212124
\
\     11-22 has two invalid IDs, 11 and 22.
\
\     95-115 has one invalid ID, 99.
\ 95, 96, 97, 98, *99*, 100, 101, 102, 103, 104, 105, 106, 107,
\ 108, 109, 110, 111, 112, 113, 114, 115
\
\ So number is made up of entirely of a repeating sequence?
\ But are 3 or 4 repeats meaningful? ie, 123123123?
\
\     998-1012 has one invalid ID, 1010.
\     1188511880-1188511890 has one invalid ID, 1188511885.
\     222220-222224 has one invalid ID, 222222.
\     1698522-1698528 contains no invalid IDs.
\     446443-446449 has one invalid ID, 446446.
\     38593856-38593862 has one invalid ID, 38593859.

require test/ttester.fs

T{ 99 1 + dup -> 100 100 }T
T{ 99 2 2dup + -> 99 2 101 }T

\
\ no, in as string, called that way from valid-range
\ if length odd then false exit
\ split in half
\ return comparison
: ?valid-id ( c-addr u -- f )
  dup 1 and if false exit then    \ odd length
  \ 0110 6 011 3
  rshift                          \ half length c1 u1
  2dup +                          \ half length second half
  over swap                       \ c1 u1 c2 u2
  compare 0= ;

: ?valid-range ( s" ll-hh" -- flag )
  false ;

T{ 11 ?valid-id -> false }T
T{ 22 ?valid-id -> false }T
T{ s" 11-22" ?valid-range -> false }T
T{ s" 12-21" ?valid-range -> true }
T{ 95 ?valid-id -> true }T
T{ 99 ?valid-id? -> false }T
T{ 115 ?valid-id -> true }T
T{ s" 95-115" ?valid-range -> false }T
T{ s" 998" ?valid-id -> false }T
T{ s" 1012" ?valid-id -> false }T
T{ s" 998-1012" ?valid-range -> false }T

\ End of tests.fs.
