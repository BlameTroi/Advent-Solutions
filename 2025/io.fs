\ io.fs -- General I/O helpers mostly for AOC -- T.Brumley

BASE @
DECIMAL

\ My standard input file fields. Not all are used in every
\ program.  When reading by line, the input buffer must
\ include two extra bytes for possible CRLF.

create in-fn 256 chars allot
[UNDEFINED] in-max [IF]
  80 constant in-max
[THEN]
variable in-len
create in-buf in-max 2 chars + allot
0 value in-fd
false value in-eof

\ Save input file name and open that file as read only.

: open-input ( c-addr u -- )
  in-fn 256 s$>c$ in-fn count r/o open-file throw to in-fd ;

\ Close input file (for symmetry).

: close-input ( -- )
  in-fd close-file throw ;

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
  0 here c! here 1 in-fd read-file throw  here c@ swap ;

BASE !

\ End of io.fs.