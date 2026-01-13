\ solution.fs -- AoC 2025 02 GIft Shop -- T.Brumley.

\ 1-22,95-115,998-1012,1188511880-1188511890,222220-222224,
\ 1698522-1698528,446443-446449,38593856-38593862,
\ 565653-565659,824824821-824824827,2121212118-2121212124
\
\ (The ID ranges are wrapped here for legibility; in your input
\ they appear on a single long line.)
\
\ first-last,
\
\ Invalid patterns:
\
\ * Repeated substrings (55, 6464, 123123 are all invalid).
\ * May not start with 0.
\
\ Your job is to find all of the invalid IDs that appear in the
\ given ranges. In the above example:
\
\     11-22 has two invalid IDs, 11 and 22.
\     95-115 has one invalid ID, 99.
\     998-1012 has one invalid ID, 1010.
\     1188511880-1188511890 has one invalid ID, 1188511885.
\     222220-222224 has one invalid ID, 222222.
\     1698522-1698528 contains no invalid IDs.
\     446443-446449 has one invalid ID, 446446.
\     38593856-38593862 has one invalid ID, 38593859.
\     The rest of the ranges contain no invalid IDs.

\ Adding up all the invalid IDs in this example produces
\ 1227775554.
\ Common utility definitions: ---------------------------------

\ This is a version of read-line that eats blank lines. Its
\ interface is the same as read-line.

: read-line-skip-empty ( c-addr u1 fd -- u2 flag ior )
  { ubuf ulen ufd }
  begin
    ubuf ulen ufd read-line { rlen rgot rior }
          rior if true else
       rgot 0= if true else
               rlen then then
  until
  rlen rgot rior ;

\ Copy a string to a counted string. I could have just copied
\ the s-string to a variable and the length into another, but
\ this was more fun.

: s$>c$ ( s-addr s-u c-addr c-u -- )
   2dup erase                      \ clear work, s su c cu
   1- swap 1+ swap                 \ s u c1 u1 adjust for count
   rot min                         \ s c m
   2dup swap 1- c!                 \ s c m count in dest
   move ;                          \ and move


\ Problem related definitions: --------------------------------

\ Parse ^[LR]d+$ to find the magnitude of a turn of the dial
\ (d+$) and its direction ([Ll] = left = -1).

: parse-input ( s-addr u -- n )
  { str len } 1 { dir }
  str c@ 'l' = if -1 to dir then
  str c@ 'L' = if -1 to dir then
  str char+ len 1- 0 0 2swap >number 2drop drop
  dir swap * ;

\ Problem state: ----------------------------------------------

\ Definitions prior to this should be pure functions, or as
\ pure as an i/o operation can be.

create in-fn 256 chars allot
80 constant in-max   variable in-len
create in-buf in-max 2 chars + allot
0 value in-fd

variable part-one  variable part-two

\ Driver for the solution. Use run-live/test interactively to
\ avoid typing long paths.

: solve ( s" input.txt" -- )

  \ Get input file from s" path", persist, and open the file.
  in-fn 256 s$>c$  in-fn count  r/o open-file throw  to in-fd

  0 part-one ! 0 part-two !

  begin
    in-buf in-max in-fd read-line-skip-empty
    throw  swap  in-len !         \ leaves eof flag
  while
    in-buf in-len @  parse-input  clicks !

  repeat

  \ And out.
  in-fd close-file throw
  cr cr ." 2025 day 1 part 1 answer: "
  part-one @ 6 .r
  cr ."            part 2 answer: "
  part-two @ 6 .r cr
  ;

: run-test
  s" ../../../Advent-Data/2025/02/test.txt" solve ;

: run-live
  s" ../../../Advent-Data/2025/02/live.txt" solve ;

\ End of solution.fs
