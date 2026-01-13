\ 2025 Day 1 parts 1 & 2 test cases -- T. Brumley.

require test/ttester.fs

\ These begin with some restatements of the problem's examples.
\ As I got stuck on part 2 (who didn't) there are a bunch of
\ part 2 special cases. Many are redundant, I lifted them from
\ reddit help requests. Always use tests!

T{ 50 -68 calculate-one -> 0 }T
T{ 50 -68 calculate-two -> 1 }T
T{ 50 -168 calculate-one -> 0 }T
T{ 50 -168 calculate-two -> 2 }T
T{ 50 68 calculate-one -> 0 }T
T{ 50 68 calculate-two -> 1 }T
T{ 50 168 calculate-one -> 0 }T
T{ 50 168 calculate-two -> 2 }T

T{ 52 48 calculate-one -> 1 }T
T{ 52 48 calculate-two -> 1 }T
T{ 52 148 calculate-one -> 1 }T
T{ 52 148 calculate-two -> 2 }T
T{ 52 248 calculate-one -> 1 }T
T{ 52 248 calculate-two -> 3 }T

T{ 52 -52 calculate-one -> 1 }T
T{ 52 -52 calculate-two -> 1 }T
T{ 52 -152 calculate-one -> 1 }T
T{ 52 -152 calculate-two -> 2 }T
T{ 52 -252 calculate-one -> 1 }T
T{ 52 -252 calculate-two -> 3 }T

T{ 50 -150 calculate-one -> 1 }T
T{ 50 -150 calculate-two -> 2 }T
T{ 50 50 calculate-one -> 1 }T
T{ 50 50 calculate-two -> 1 }T
T{ 50 150 calculate-one -> 1 }T
T{ 50 150 calculate-two -> 2 }T
T{ 50 -50 calculate-one -> 1 }T
T{ 50 -50 calculate-two -> 1 }T
T{ 50 150 calculate-one -> 1 }T
T{ 50 150 calculate-two -> 2 }T
T{ 50 50 calculate-one -> 1 }T
T{ 50 50 calculate-two -> 1 }T
T{ 95 60 calculate-one -> 0 }T
T{ 95 60 calculate-two -> 1 }T
T{ 55 -55 calculate-one -> 1 }T
T{ 55 -55 calculate-two -> 1 }T
T{ 99 -99 calculate-one -> 1 }T
T{ 99 -99 calculate-two -> 1 }T
T{ 14 -82 calculate-one -> 0 }T
T{ 14 -82 calculate-two -> 1 }T

T{ 0 -100 calculate-two -> 1 }T
T{ 0 -123 calculate-two -> 1 }T
T{ 0 -200 calculate-two -> 2 }T
T{ 0 -258 calculate-two -> 2 }T
T{ 0 -400 calculate-two -> 4 }T
T{ 0 1 calculate-two -> 0 }T
T{ 0 100 calculate-two -> 1 }T
T{ 0 123 calculate-two -> 1 }T
T{ 0 200 calculate-two -> 2 }T
T{ 0 258 calculate-two -> 2 }T
T{ 0 400 calculate-two -> 4 }T
T{ 1 3 calculate-two -> 0 }T
T{ 2 -1 calculate-two -> 0 }T
T{ 50 -10 calculate-two -> 0 }T
T{ 50 -100 calculate-two -> 1 }T
T{ 50 -150 calculate-two -> 2 }T
T{ 50 -200 calculate-two -> 2 }T
T{ 50 -250 calculate-two -> 3 }T
T{ 50 -50 calculate-two -> 1 }T
T{ 50 -50 calculate-two -> 1 }T
T{ 50 100 calculate-two -> 1 }T
T{ 50 150 calculate-two -> 2 }T
T{ 50 200 calculate-two -> 2 }T
T{ 50 250 calculate-two -> 3 }T
T{ 50 49 calculate-two -> 0 }T
T{ 50 50 calculate-two -> 1 }T
T{ 99 -98 calculate-two -> 0 }T
T{ 99 1 calculate-two -> 1 }T
T{ 99 2 calculate-two -> 1 }T




