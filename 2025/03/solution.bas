' solution.bas -- AoC 2025 03 Lobby -- T.Brumley.

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

' Input is multiple lines of 'n' digits. Digits are 1-9, no 0.
' Both parts of today's problem require identifying an optimal
' selection of digits and summing those selections.
'
' In the test data, we have 15 digit runs, while in the live
' data we have 40 digit runs.

' -------------------------------------------------------------------
' Types Constants and State

Const True = -1%
Const False = 0%

$LET RUNTYPE = LIVE
$IF RUNTYPE = LIVE THEN
Const InputFile = "../../../Advent-Data/2025/03/live.txt"
Const InputWidth = 40%
Const SelectedWidth = 12%
$ELSE
CONST InputFile = "../../../Advent-Data/2025/03/test.txt"
Const InputWidth = 15%
Const SelectedWidth = 12%
$END IF

Type BatteryBankType
    Raw As String
    Digits(InputWidth) as _Unsigned _byte
    Selected(SelectedWidth) as _Unsigned _byte
    Cooked as String
End Type

Dim range As BatteryBankType

Dim PartOne As _Integer64
Dim PartTwo As _Integer64

Dim Shared ifile As Long

' -------------------------------------------------------------------
' As with most Advent of Code problems, the driver is pretty much an
' old style Input-Process-Output.

ifile = FreeFile
Open InputFile For Input As ifile

PartOne = 0
PartTwo = 0

Print
Print "Advent of Code 2025 Day 3"
Print "        Lobby"
Print


While GetNextBank(range)

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
    Raw As String
    Digits(InputWidth) as _Unsigned _byte
    Selected(SelectedWidth) as _Unsigned _byte
    Cooked as String
E
Function GetNextBank (bank As BatteryBankType)
    bank.Raw = ""
    Dim i: For i = 1 to InputWidth: bank.Digits(i) = 0%%: Next i
    Dim j: For j = 1 to SelectedWidth: bank.Selected = 0%%: Next i
    bank.Cooked = ""
    If EOF(ifile) Then
        GetNextBank = False
    Else
        Input #file, bank.Raw
        For i = 1 to InputWidth:
            bank.Digits(i) = Mid$(bank.Raw, i, 1)
        next i
        GetNextRange = True
    End If
End Function

' -------------------------------------------------------------------
' Are all the digits in this part number string the same?

' -------------------------------------------------------------------
' Is the string s$ made up of n equal substrings of length l?

' End of solution.bas.
' -------------------------------------------------------------------

