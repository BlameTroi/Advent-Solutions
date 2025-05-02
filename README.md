# Advent of Code

My on again off again work on Advent of Code. I started in 2022 with
golang, didn't enjoy working in the language and stopped. I later
restarted with Pascal and both 2022 and 2023 will be done in Pascal.

There's nothing special here, just me using AoC as writing prompts.
The goal is to have fun and stretch my long dormant coding muscles. I
will often take the path of greatest resistance. Overcoding, odd
datatype choices, and repeated violation of 'dry' and 'yagni' will be
common.

This is about practice and habit. I'll certainly learn other things
along the way, but the goal is to keep the fingers moving writing
code.

## Progress

Year    Stars
2015    50
2016
2017
2018
2019
2020
2021
2022    30
2023    22
2024

## TODO

Some text files are in org format, some in markdown, but I'll be
slowly converting everything to markdown. I prefer plain old text
files, but I'll go with the flow.

## Repository and Project Structure

To honor the request that problem text and datasets not be publicly
visible, I'm rebuilding the project and splitting it into three
separate repositories.

- Advent-Problems (private)
- Advent-Data (private)
- Advent-Solutions

I expect the three repositories to sit parallel to each other in my
projects folder. I can get to the data by relative paths, but I may
break down and set some environment variables.

Within each repository, the structure will be:

- Repository root
  - Year
    - Days 1-25
  - Common

Where common will hold bits of common code or libraries applicable to
the year's code.

Within each day's directory there should multiple source files and a
some build instructions or artifacts. Expect to find at least:

| File name    | Description |
|:-------------|:------------------------------------------------|
| problem.txt   | A brief problem statement with no data or worked examples. |
| notes.txt     | any notes I make. |
| solution.?    | The main logic for the solution. |
| driver.?      | A common launcher for each day's problem parts. |
| output.txt    | An answer but no meaningful data. |

## Implementation Languages

Each year will likely be monolingual. I use an odd mix of languages:
I've done some of these in Pascal, Fortran, and C. Next up is likely
Python (I need to learn the packaging and configuration side of
things). I also hope to get to Odin, Scheme, and ARM assembly. Heck, I
might even try mainframe assembly or PL/I.

## Problem Statement and Test Data

The AoC site requests that we not post the full datasets in public
view. Since AI crawlers have been introduced, I believe this request
extends to the detailed problem statements. I group these in two other
private repositories with a parallel directory structure.

I may create smaller test data sets for testing.

## Notes on Style

I'm using 1980ish tooling with a Neovim, text files, and old school
structured programming. Objects? We don't need no stinking objects.

    Ok, I may use objects if the implementation language expects them.

I started out in Pascal, switched to Fortran, and then to C. I redid
the Pascal code in Fortran but then switched to C. I'm doing my best
to follow the Cxx standards and avoiding extensions.

I found Free Pascal to be too intertwined with Lazarus to be
enjoyable. It's a pitty, I prefer using Pascal.

If I get stupidly ambitious I may roll my own ISO Pascal. There are
some good starting implementations out there.

### Pascal

The compiler was Free Pascal 3.22 in with runtime checks enabled, a
limited number of extensions, and usually ShortStrings. Integers are
cast to 64 bits--I prefer a dash of retro with my code, but I'm not a
purist.

- integers are longints (4 bytes).
- strings are classic string[255]. no trailing nulls are guaranteed.
  FPC AnsiStrings -an be turned on explicitly, as can PChar.
- dynamic arrays are available but will be avoided.
- exceptions are available but will also be avoided. If you need them
  for AoC you are doing it wrong.
- `break` and `continue` are available in loops, `exit` is available
  for early exit from procedures and functions, but I steadfastly refuse
  to use `result`.
- boolean evaluation short circuiting is disabled as it's non-standard
  in a way that is visible in the code.

### Fortran

About half way through the 2022 and 2023 I decided to switch to
Fortran. I've been meaning give Fortran a try for a while and after
seeing enough of the present state of the compilers and standards I
feel they are superior to Pascal for my needs. I still love Pascal,
but the direction of Delphi and Lazarus is heavy objects and forms
based. I'm more comfortable at lower levels.

The general approach is the same as with Pascal, but given the strong
standards support I don't have to add cruft to get the language in
what I consider proper shape.

There are two compilers I am alternating between: gfortran 13.2 and
lfortran. lfortran is still in a pre-beta state but as gfortran and
lldb on MacOs don't work without jumping through hoops, I expect to
use lfortran if I have to use a debugger.

### C

I started on 2015 using `C`. At this time clang 15 is the version
Apple provides. All my code is compiled with `-Wall -O0 -g3
-fsanitize=address --std=c18 --pedantic-errors`.

### Editor/IDE

Neovim with a pretty simple setup. Highlighting is turned on and I
take advantage of text objects and such, but I don't take advantage of
the LSP support.

## Dependencies

I strive for no external (to me) dependencies. The Fortran and Pascal
based solutions carry common code and shells in directories parallel
with each day's code.

For `C` I'm using `minunit` (a header only unit testing harness) from
[MinUnit](https://github.com/siu/minunit). I also have started putting
my own `C` common code in header only libraries on GitHub as
[TXBLIBS](https://github.com/BlameTroi/txblibs). I don't add those as
formal dependencies in Git but the headers are in my local include
directory.

## Licensing and copyright

All my code is released with the unlicense. To whatever extent a
copyright is needed, this is copyright 2024 by Troy Brumley.

Troy Brumley  
[My Email](blametroi@gmail.com)  
  So let it be written.  
  So let it be done.  
