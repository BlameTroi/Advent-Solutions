\ common.fs -- common code accumulated during AoC -- T.Brumley

\ My standard input file fields. Not all are used in every
\ program.

create in-fn 256 chars allot

\ allows for override...
[UNDEFINED] in-max [IF]
80 constant in-max
[THEN]

variable in-len
create in-buf in-max 2 chars + allot
0 value in-fd

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
