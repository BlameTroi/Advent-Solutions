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
\ probably in part 2 :/
\
\     998-1012 has one invalid ID, 1010.
\     1188511880-1188511890 has one invalid ID, 1188511885.
\     222220-222224 has one invalid ID, 222222.
\     1698522-1698528 contains no invalid IDs.
\     446443-446449 has one invalid ID, 446446.
\     38593856-38593862 has one invalid ID, 38593859.

require test/ttester.fs

T{ 11 ?bad-id -> false }T
T{ 22 ?bad-id -> false }T
T{ s" 11-22" ?parse-range$ 2swap 2drop -> 11 22 }T
T{ s" 11" ?parse-number$ -> 11 }T
T{ s" 12-21" ?parse-range$ 2swap 2drop -> 12 21 }T
T{ 95 ?bad-id -> false }T
T{ 99 ?bad-id -> true }T
T{ 115 ?bad-id -> false }T

\ End of tests.fs.
