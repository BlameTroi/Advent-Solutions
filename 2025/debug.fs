\ debug.fs -- Any debug helpers I accumulate -- T.Brumley

BASE @
DECIMAL

\ Assorted things I have found helpful when debugging.

\ Helpfulish debug trace.

false value do.diag.s
: diag.s ( c-addr u -- , print .s with tag )
  do.diag.s if -trailing cr type space .s else 2drop then ;

BASE !

\ End of debug.fs.