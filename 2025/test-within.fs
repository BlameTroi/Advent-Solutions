\ test-within.fs -- Test ranging functions -- T.Brumley.

BASE @
DECIMAL

\ So far all I have here is WITHIN? which caps WITHIN so that
\ a fully closed interval check is done. WITHIN does a right
\ open check.

require test/ttester.fs

T{ 0 1 9 within -> false }T
T{ 1 1 9 within -> true }T
T{ 5 1 9 within -> true }T
T{ 9 1 9 within -> false }T
T{ 10 1 9 within -> false }T

T{ 0 1 9 within? -> false }T
T{ 1 1 9 within? -> true }T
T{ 5 1 9 within? -> true }T
T{ 9 1 9 within? -> true }T
T{ 10 1 9 within? -> false }T

BASE !

\ End of test-within.fs.
