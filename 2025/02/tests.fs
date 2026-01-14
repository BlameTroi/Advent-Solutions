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

\ unsigned cell is 20 decimal digits 184....


create swork 4096 allot

\ Part one rule for valid is not a repeating pattern.
\ In other words, the first half of the digits does not
\ equal the second half of the digits.
\
\ any odd length is automatically valid
\ 99 -> 9|9 -> 9=9 so invalid
\ 123124 -> 123|124 -> 123<>124 so valid

: ?valid-id$ ( c-addr u -- f )
  dup 1 and if 2drop false exit then \ odd length?
  1 rshift                        \ half length c1 u1
  2dup +                          \ half length second half
  over                            \ c1 u1 c2 u2
  compare 0<> ;                   \ proper true/false

\ Given an unsigned double, is it a valid part id? The actual
\ rule is in ?valid-id$. Convert ud to string and then check
\ validity.

create $id 256 allot
: ?valid-id ( ud -- f )           \ sign ignored
  $id 256 blank                   \ as string in safe location
  <# #s #> dup >r                 \ addr u, r: u
  $id swap move $id r>            \ addr u
  ?valid-id$ ;

\ A range of part ids is a string s" ll-hh,". Convert this to a
\ pair of unsigned doubles and iterate checking each from ll to
\ hh for validity. Prints invalid ids.
\
\ Assumes that the caller will send us one pair of ids at a
\ time.

: ?valid-range ( ud ud -- flag )
  2drop 2drop
  false ;

: ?valid-range$ ( s" ll-hh," -- flag )
  2dup cr ." # " type cr
  \ more to come
  2drop
  \
  false ;

T{ 11 0 ?valid-id -> false }T
T{ 22 0 ?valid-id -> false }T
T{ s" 11-22" ?valid-range$ -> false }T
T{ s" 12-21" ?valid-range$ -> true }T
T{ 95 0 ?valid-id -> true }T
T{ 99 0 ?valid-id -> false }T
T{ 115 0 ?valid-id -> true }T
T{ s" 95-115" ?valid-range$ -> false }T
T{ s" 998" ?valid-id$ -> true }T
T{ s" 1012" ?valid-id$ -> true }T
T{ s" 998-1012" ?valid-range$ -> false }T
T{ 998 0 1012 0 ?valid-range -> false }T

\ End of tests.fs.
