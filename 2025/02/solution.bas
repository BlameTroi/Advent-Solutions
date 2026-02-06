' solution.bas -- AoC 2025 02 Gift SHopw -- T.Brumley.

' -------------------------------------------------------------------
' Standard settings:

'$Console
'_Console On
'_Dest _Console

Option _Explicit
DefLng A-Z ' Long 4 bytes &, but many must be _Integer64 8 bytes &&

$VersionInfo:FileDescription='Advent of Code 2025 Day 2 in QB64'
$VersionInfo:LegalCopyright='(c) 2026 Troy Brumley, License is MIT or Unlicense, your choice'
$VersionInfo:OriginalFilename='solution.bas'
$VersionInfo:Comments='Advent of Code solutions, free for anyone to use.'

' -------------------------------------------------------------

' Input is a single line of ranges separated by commas. A
' range is "low-high". The problem refers to the numbers as
' "part ids" but I prefer "part numbers." For both parts of
' the problem specific digit patterns in a part number marks
' the number as invalid. For each part, sum the invalid part
' numbers.
'
' In the data, the longest run of digits is 10. Since the
' possible pattern arrangements are fixed for a length a
' case structure makes sense.
'
' length     # sequences    sequence lengths
'   10           2 5           5 2
'    9           3             3
'    8           2 4           4 2
'    7           7             1
'    6           2 3           3 2
'    5           5             1
'    4           2             2
'    3           3             1
'    2           2             1
'    -- 1 unclear from spec but they don't count --
'    -- 0 not possible --

' -------------------------------------------------------------------
' Types Constants and State

Const True = -1%
Const False = 0%

Type RangeType
    Low As _Integer64
    High As _Integer64
End Type

Dim range As RangeType

Dim PartOne As _Integer64
Dim PartTwo As _Integer64

Dim Shared ifile As Long

' -------------------------------------------------------------------
' As with most Advent of Code problems, the driver is pretty much an
' old style Input-Process-Output.

ifile = FreeFile
Open "../../../Advent-Data/2025/02/live.txt" For Input As ifile

PartOne = 0
PartTwo = 0

Print
Print "Advent of Code 2025 Day 2"
Print "      Gift Shop"
Print


While GetNextRange(range)
    Dim pn As _Integer64
    For pn = range.Low To range.High
        Dim ps As String: ps = Str$(pn): ps = Right$(ps, Len(ps) - 1)
        Select Case Len(ps)
            Case 10
                If Left$(ps, 5) = Right$(ps, 5) Then
                    PartOne = PartOne + pn
                    PartTwo = PartTwo + pn
                ElseIf NSubEqual(ps, 5, 2) Then
                    PartTwo = PartTwo + pn
                End If
            Case 9
                If NSubEqual(ps, 3, 3) Then PartTwo = PartTwo + pn
            Case 8
                If Left$(ps, 4) = Right$(ps, 4) Then
                    PartOne = PartOne + pn
                    PartTwo = PartTwo + pn
                ElseIf NSubEqual(ps, 4, 2) Then
                    PartTwo = PartTwo + pn
                End If
            Case 7
                If AllSameCharacter(ps) Then PartTwo = PartTwo + pn
            Case 6
                If Left$(ps, 3) = Right$(ps, 3) Then
                    PartOne = PartOne + pn
                    PartTwo = PartTwo + pn
                ElseIf NSubEqual(ps, 3, 2) Then
                    PartTwo = PartTwo + pn
                End If
            Case 5
                If AllSameCharacter(ps) Then PartTwo = PartTwo + pn
            Case 4
                If Left$(ps, 2) = Right$(ps, 2) Then
                    PartOne = PartOne + pn
                    PartTwo = PartTwo + pn
                End If
            Case 3
                If AllSameCharacter(ps) Then PartTwo = PartTwo + pn
            Case 2
                If Left$(ps, 1) = Right$(ps, 1) Then
                    PartOne = PartOne + pn
                    PartTwo = PartTwo + pn
                End If
            Case Else
                ' The longest part number is 10, the shortest is 1.
                ' These are both ignored.
        End Select
    Next pn

Wend

Close ifile

Print
Print Using "&  ############"; "Part One:"; PartOne
Print Using "&  ############"; "Part Two:"; PartTwo
Print

End

' -------------------------------------------------------------------
' Read the next part number range. Input is a file of a single line
' of text with part number ranges (lo-high) separated by commas.

Function GetNextRange (range As RangeType)
    range.Low = -1
    range.High = -1
    If EOF(ifile) Then
        GetNextRange = False
    Else
        Dim s$: Input #ifile, s$
        Dim i%: i% = InStr(s$, "-")
        range.Low = Val(Left$(s$, i% - 1))
        range.High = Val(Right$(s$, Len(s$) - i%))
        GetNextRange = True
    End If
End Function

' -------------------------------------------------------------------
' Are all the digits in this part number string the same?

Function AllSameCharacter (s$)
    AllSameCharacter = True
    Dim i: Dim c$: c$ = Left$(s$, 1)
    For i = 2 To Len(s$)
        If c$ <> Mid$(s$, i, 1) Then
            AllSameCharacter = False
            Exit Function
        End If
    Next i
End Function

' -------------------------------------------------------------------
' Is the string s$ made up of n equal substrings of length l?

Function NSubEqual (s$, n, l)
    NSubEqual = False
    If n * l <> Len(s$) Then Exit Function
    Dim i: Dim c$: c$ = Left$(s$, l)
    For i = 1 To n - 1
        If Mid$(s$, 1 + l * i, l) <> c$ Then Exit Function
    Next i
    NSubEqual = True
End Function

' End of solution.bas.
' -------------------------------------------------------------------

