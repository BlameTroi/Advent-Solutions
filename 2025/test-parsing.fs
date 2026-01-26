\ test-parsing.fs -- Test my parse helpers -- T.Brumley

BASE @
DECIMAL

\ Bits of code for parsing input data. Usually for Advent of
\ Code problems. These are mostly wrappers around >number that
\ deal with common AoC formats.

require test/ttester.fs

verbose on

: s1 s" 1" ;
: s2 s" 1 2" ;
: s3 s" 1-2,3-4" ;
: s4 s"   12   will not parse" ;
: s5 s" 802,123" ;

T{ s1 parse->number$ -> 1 0 s1 1- swap 1+ swap }T
T{ s2 parse->number$ -> 1 0 s2 1- swap 1+ swap }T
T{ s3 parse->number$ -> 1 0 s3 1- swap 1+ swap }T
T{ s4 parse->number$ -> 0 0 s4 }T
T{ s5 parse->number$ -> 802 0 s5 3 - swap 3 + swap }T
T{ s3 parse-range$ -> s3 3 - swap 3 + swap 1 2 }T

verbose off

BASE !

\ End of test-parsing.fs