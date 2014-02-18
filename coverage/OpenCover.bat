@echo off
MODE CON COLS=100 LINES=50
title Code-Coverage Report Build-Script

::
:: Stapelverarbeitungs-Skript für den Testabdeckungsbericht unter Windows (V. 1.3)
::
echo.
echo.
echo  --- OpenCover.bat ---
echo.
echo.

::
:: Anleitung:
::

::
:: -----------------------------------------------------------------------------
::
:: (1) Installieren von NUnit, OpenCover, ReportGenerator.
:: (2) Visual Studio konfigurieren:
::
:: In der Projektmappe mit der rechten Maustaste auf das Projekt klicken,
:: welches später die Testabdeckung umfassen soll. Es erscheint ein Menü, 
:: dort mit der linken Maustaste auf "Eigenschaften" klicken.
::
:: Unter "Befehlszeile für Postbuildereignis" folgendes eingeben:
:: call "$(SolutionDir)<Projektordner>\OpenCover.bat"
::
:: Unter "Postbuildereignis ausführen" den Eintrag "Immer" auswählen und 
:: mit der linken Maustaste anklicken.
::
:: (3) Diese Stapelverarbeitungsdatei in das Projektverzeichnis legen.
:: (4) Beim "Erstellen" des Projekts oder durch direktes Starten von 
::     OpenCover.bat über einen Doppelklick, wird nun diese Stapel-
::     verarbeitungsdatei ausgeführt und so der Testabdeckungsbericht 
::     erstellt.
::
:: -----------------------------------------------------------------------------
::

echo.
echo  Settings:
:: echo.
:: echo Erstes Argument: "%1"
:: echo.
echo.
echo.

::
:: Projekt-Pfade, relativ zum Speicherort dieser Stapelverarbeitungsdatei.
::
set PATH_TO_PROJECT=%~dp0..
set PATH_TO_TESTS=%PATH_TO_PROJECT%\tests\bin\Debug
set PATH_TO_RAW_REPORTDATA=%PATH_TO_PROJECT%\coverage\bin\Debug
set PATH_TO_HTML_REPORT=%PATH_TO_PROJECT%\coverage\bin\Debug
set PATH_TO_LATEX_REPORT=%PATH_TO_PROJECT%\..\knot-qualitaetssicherung\Bericht\Inhalt\Tests\Abdeckung

set LATEX_FULL_REPORT_NAME=OpenCover_Bericht_komplett.tex
set LATEX_SUMM_REPORT_NAME=OpenCover_Bericht_uebersicht.tex
set LATEX_FULL_REPORT="%PATH_TO_LATEX_REPORT%\%LATEX_FULL_REPORT_NAME%"
set LATEX_SUMM_REPORT="%PATH_TO_LATEX_REPORT%\%LATEX_SUMM_REPORT_NAME%"

::
:: Standard-Installationsverzeichnisse der Werkzeuge:
::
set PATH_TO_NUNIT=%ProgramFiles(x86)%\NUnit 2.6.3\bin
set PATH_TO_OPENCOVER=%LOCALAPPDATA%\Apps\OpenCover
set PATH_TO_REPORTGENERATOR=%ProgramFiles(x86)%\ReportGenerator\bin


REM OPTIONAL TODO
goto :SKIP
::
:: Konfigurationsdatei.
:: Es ist möglich lokal (%USERPROFILE\Documents) eine Konfigurationsdatei anzugeben,
:: in der die Pfade zu den Werkzeugen stehen. Alternativ können lokal auch harte Links
:: verwendet werden ;)
::
set PATH_TO_CONFIGURATION_FILE=%USERPROFILE%\Documents\vs-crpc.txt

if exist %PATH_TO_CONFIGURATION_FILE% (
echo.
echo  Visual Studio coverage report path configuration file found.
echo.

for /F "eol=# tokens=1,2* delims==" %%i in (%PATH_TO_CONFIGURATION_FILE%) do (
echo %%i %%j
)

) else (
echo.
echo  No configuration, using default settings.
echo.
)
:SKIP

echo.
echo  Paths to ...
echo.
for %%p in ("%CD%") do (
echo    Curr. Location  : %%~sp
)

echo.

for %%p in ("%PATH_TO_NUNIT%") do (
echo    NUnit           : %%~sp
)
for %%p in ("%PATH_TO_OPENCOVER%") do (
echo    OpenCover       : %%~sp
)
for %%p in ("%PATH_TO_REPORTGENERATOR%") do (
echo    ReportGenerator : %%~sp
)

echo.

for %%p in ("%PATH_TO_PROJECT%") do (
echo    VS-Project      : %%~sp
)
for %%p in ("%PATH_TO_TESTS%") do (
echo    Tests           : %%~sp
)
for %%p in ("%PATH_TO_RAW_REPORTDATA%") do (
echo    Rep. Dsrc.      : %%~sp
)
for %%p in ("%PATH_TO_HTML_REPORT%") do (
echo    Rep. Dst. ^(HTM^) : %%~sp
) 
for %%p in ("%PATH_TO_LATEX_REPORT%") do (
echo    Rep. Dst. ^(TEX^) : %%~sp
)
echo.

::
:: Filter für OpenCover hier einstellen:
::
set COMPONENT_FILTER=+[Knot3]*
::
:: [ExcludeFromCodeCoverageAttribute]
:: GetHashCode, ToString, Update, Draw
::
set ATTRIBUTE_FILTER=System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute
echo.
echo  OpenCover-Filters:
echo.
echo    Components:
echo.
::
set ECHO=%COMPONENT_FILTER%
:NEXT_COMPONENT
for /f "delims=;" %%c in ("%ECHO%") do echo    %%c
set TMP_ECHO=%ECHO%
set ECHO=%ECHO:*;=%
if not "%TMP_ECHO:;=%"=="%TMP_ECHO%" goto :NEXT_COMPONENT
::
echo.
echo    Attributes:
echo.
::
set ECHO=%ATTRIBUTE_FILTER%
:NEXT_ATTRIBUTE
for /f "delims=;" %%a in ("%ECHO%") do echo    %%a
set TMP_ECHO=%ECHO%
set ECHO=%ECHO:*;=%
if not "%TMP_ECHO:;=%"=="%TMP_ECHO%" goto :NEXT_ATTRIBUTE
::
echo.
echo.
echo  ... Running tests, checking coverage ...
echo.
::
:: Hinweis: Auf einem 64-Bit-System ist nunit-console-x86.exe für 32-Bit-Projekte zu verwenden! (sonst tritt ein Fehler auf)
::
"%PATH_TO_OPENCOVER%\OpenCover.Console.exe" -target:"%PATH_TO_NUNIT%\nunit-console-x86.exe" -targetargs:"/noshadow "%PATH_TO_TESTS%\Knot3.UnitTests.dll"" -register:user -filter:"%COMPONENT_FILTER%" -excludebyattribute:"%ATTRIBUTE_FILTER%" -output:"%PATH_TO_RAW_REPORTDATA%\NUnit_test_coverage.xml">NUL

echo.
echo  ... Generating report ...
echo.
echo      ... HTML ...
echo.
::
:: Ausgabe von ReportGenerator auf der Konsole wird durch ">NUL" unterdrückt.
:: Erstellung des Berichts als Html:
::
"%PATH_TO_REPORTGENERATOR%\ReportGenerator.exe" -reporttypes:"Html" -reports:"%PATH_TO_RAW_REPORTDATA%\NUnit_test_coverage.xml" -targetdir:"%PATH_TO_HTML_REPORT%">NUL

echo      ... LaTeX ...
echo.
::
:: Erstellung des Berichts als LaTeX (vollständig):
::
if exist %LATEX_FULL_REPORT% (
del /F %LATEX_FULL_REPORT%
)
"%PATH_TO_REPORTGENERATOR%\ReportGenerator.exe" -reporttypes:"Latex" -reports:"%PATH_TO_RAW_REPORTDATA%\NUnit_test_coverage.xml" -targetdir:"%PATH_TO_LATEX_REPORT%">NUL
ren "%PATH_TO_LATEX_REPORT%\summary.tex" %LATEX_FULL_REPORT_NAME%
::
:: Erstellung des Berichts als LaTeX (nur Übersicht):
::
if exist %LATEX_SUMM_REPORT% (
del /F %LATEX_SUMM_REPORT%
)
"%PATH_TO_REPORTGENERATOR%\ReportGenerator.exe" -reporttypes:"LatexSummary" -reports:"%PATH_TO_RAW_REPORTDATA%\NUnit_test_coverage.xml" -targetdir:"%PATH_TO_LATEX_REPORT%">NUL
ren "%PATH_TO_LATEX_REPORT%\summary.tex" %LATEX_SUMM_REPORT_NAME%

echo.
echo  ... Showing report.
echo.
::
:: Startet das Standardprogramm für das Betrachten von .htm:
::
start "" "%PATH_TO_HTML_REPORT%\index.htm"

::
:: 5 Sekunden Pause.
::
ping -n 4 127.0.0.1>NUL
PAUSE>NUL

REM pause
exit