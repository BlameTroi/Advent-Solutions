\ common.fs -- common code accumulated during AoC -- T.Brumley


\ Helpfulish debug trace.

false value do.diag.s
: diag.s ( c-addr u -- , print .s with tag )
  if do.diag.s cr type space .s else 2drop then ;

\ My standard input file fields. Not all are used in every
\ program.  When reading by line, the input buffer must
\ include two extra bytes for possible CRLF.

create in-fn 256 chars allot     \ c" name"
[UNDEFINED] in-max [IF]
80 constant in-max
[THEN]
variable in-len
create in-buf in-max 2 chars + allot
0 value in-fd
false value in-eof

\ This is a version of read-line that eats blank lines. Its
\ interface is the same as read-line.
\ TODO: get rid of local variables!

: read-line-skip-empty ( c-addr u1 -- u2 flag )
  { ubuf ulen }
  begin
    ubuf ulen in-fd read-line throw { rlen rgot }
       rgot 0= if true else
               rlen then
  until
  rlen rgot ;

\ Reading the file as bytes r/o bin. A return of 0 0 from here
\ means logical end of file.
\
\ Assumes that HERE will not move across a call from this to
\ READ-FILE.

: read-file-next-byte ( -- c f/u )
  0 here c! here 1  in-fd read-file throw  here c@ swap ;

\ Copy a string to a counted string. Will not overflow the
\ allowed length of the counted string (u2).

: s$>c$ ( c-addr1 u1 c-addr2 u2 -- )
   2dup erase  1- swap 1+ swap  rot min
   2dup swap 1- c!  move ;

\ Given a string run it through >NUMBER. The return is
\ identical to >NUMBER.

: parse->number$ ( c-addr1 u1 -- c-addr2 u2 ud )
  0 0 2swap >number ;

\ Save input file name and open that file as read only.

: open-input ( c-addr u -- )
  in-fn 256 s$>c$  in-fn count  r/o open-file throw  to in-fd ;

\ Close input file (for symmetry).

: close-input ( -- )
  in-fd close-file throw ;

\ A range is a string of two numbers separated by a dash. While
\ >NUMBER deals in doubles I drop them to single cells. That's
\ 64 bits which I hope is wide enough.

: parse-range$ ( s-addr u -- s-addr u lo hi )
  parse->number$ 2swap drop >r     ( stash lo )
  1- swap 1+ swap                  ( len -1 addr +1 for '-' )
  parse->number$ 2swap drop
  r> swap ;                        ( set return add u lo hi )

\ Fetch the next character from a string, returns 0 if length
\ remaining < 1.

: c@-next ( c-addr u -- c-addr2 u2 c )
  dup 1 < if 0 else over c@ -rot 1- swap 1+ swap rot then ;

\ Is this string a single repeating character?

0 value asc-char
0 value asc-result
: ?all-same-character ( c-addr u -- f )
  c@-next to asc-char true to asc-result
  dup 0 do
    c@-next asc-char <> if false to asc-result leave then
  loop 2drop asc-result ;

\ Convert an unsigned single to a string and save it in the
\ supplied buffer. Result is left justified in buffer and is
\ quietly truncated if it would overflow the buffer.

: u>$ ( u c-addr u1 -- c-addr u2 )
  2dup blank rot 0 <# #s #>      \ u c-addr1 u1 c-addr2 u2
  swap >r min 2dup r> -rot       \ addr addr um
  move ;                         \

