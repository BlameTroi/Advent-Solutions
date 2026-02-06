' solution.bas -- AoC 2025 01 Secret Entrance -- T.Brumley.

' -------------------------------------------------------------------
' Standard settings:

'$Console
'_Console On
'_Dest _Console

Option _Explicit
DefLng A-Z ' Long 4 bytes &, but many must be _Integer64 8 bytes &&

$VersionInfo:FileDescription='Advent of Code 2025 Day 1 in QB64'
$VersionInfo:LegalCopyright='(c) 2026 Troy Brumley, License is MIT or Unlicense, your choice'
$VersionInfo:OriginalFilename='solution.bas'
$VersionInfo:Comments='Advent of Code solutions, free for anyone to use.'

' -------------------------------------------------------------------
' Types Constants and State

Const True = -1%
Const False = 0%

Type TurnType
    Direction As String * 1
    Clicks As Integer
End Type

Dim tt As TurnType

Dim Dial As Long
Dim NextDial As Long

Dim PartOne As _Integer64
Dim PartTwo As _Integer64

Dim Shared ifile As Long

' -------------------------------------------------------------------
' As with most Advent of Code problems, the driver is pretty much an
' old style Input-Process-Output.

ifile = FreeFile
Open "../../../Advent-Data/2025/01/live.txt" For Input As ifile

Dial = 50
PartOne = 0
PartTwo = 0
Dim c1, c2

Print
Print "Advent of Code 2025 Day 1"
Print "    Secret Entrance"
Print

' Print " Dial  D  Click  Next    P1    P2"

While GetNextTurn(tt)
    NextDial = NormalizeDial(Dial + tt.Clicks)
    c1 = CalculateOne(NextDial)
    c2 = CalculateTwo(Dial, tt.Clicks)

    ' Print Using " ####  &  #####  ####  ####  ####"; Dial; tt.Direction; tt.Clicks; NextDial; c1; c2

    PartOne = PartOne + c1
    PartTwo = PartTwo + c2
    Dial = NextDial
Wend

Close ifile

Print
Print Using "&  ############"; "Part One:"; PartOne
Print Using "&  ############"; "Part Two:"; PartTwo
Print

End

' -------------------------------------------------------------------
' Read the next instruction and parse it into a TurnType. Returns
' True if an instruction was read or False on EOF.

Function GetNextTurn (turn As TurnType)
    turn.Direction = "E"
    turn.Clicks = 0
    If EOF(ifile) Then
        GetNextTurn = False
    Else
        Dim s$: Line Input #ifile, s$
        turn.Direction = Left$(s$, 1)
        turn.Clicks = Val(Right$(s$, Len(s$) - 1), Integer)
        If turn.Direction = "L" Or turn.Direction = "l" Then turn.Clicks = -turn.Clicks
        GetNextTurn = True
    End If
End Function

' -------------------------------------------------------------------
' Normalize the dial. IE, bring it back within the range
' of [0, 100). Yeah, silly algorithm.

Function NormalizeDial (n)
    While n < 0 Or n > 99
        If n < 0 Then
            n = n + 100
        ElseIf n > 99 Then
            n = n - 100
        End If
    Wend
    NormalizeDial = n
End Function

' -------------------------------------------------------------------
' Did the dial stop on zero?

Function CalculateOne (n)
    CalculateOne = 0
    If 0 = n Then
        CalculateOne = 1
    End If
End Function


' -------------------------------------------------------------------
' How many clicks "touch" zero? This includes a stop at but not
' a start from zero.

Function CalculateTwo (dial, clicks) ' 50, -168
    Dim part, full, normed
    full = Int(Abs(clicks) \ 100) ' 1
    part = Abs(clicks) Mod 100 ' 68
    normed = NormalizeDial(dial + clicks) ' -118 ->

    ' Special case, landed on 0.
    If normed = 0 Then
        If part = 0 Then
            CalculateTwo = full
        Else
            CalculateTwo = full + 1
        End If
        Exit Function
    End If

    ' More complex case.
    Dim adj: adj = 0
    If clicks < 0 Then
        If normed + part > 100 Then
            adj = 1
        Else
            adj = 0
        End If
    Else
        If normed - part < 0 Then
            adj = 1
        Else
            adj = 0
        End If
    End If

    CalculateTwo = full + adj

End Function

' End of solution.bas.
' -------------------------------------------------------------------

