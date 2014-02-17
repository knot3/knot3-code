@echo off

::
:: Stapelverarbeitungs-Skript für den Testabdeckungsbericht unter Windows (V. 1.2)
::
echo.
echo --- OpenCover.bat ---
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
echo Settings:
echo.
echo Erstes Argument: "%1"
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

::
:: Standard-Installationsverzeichnisse der Werkzeuge:
::
set PATH_TO_NUNIT=%ProgramFiles(x86)%\NUnit 2.6.3\bin
set PATH_TO_OPENCOVER=%LOCALAPPDATA%\Apps\OpenCover
set PATH_TO_REPORTGENERATOR=%ProgramFiles(x86)%\ReportGenerator\bin


::
:: Konfigurationsdatei.
:: Es ist möglich lokal (%USERPROFILE\Documents) eine Konfigurationsdatei anzugeben,
:: in der die Pfade zu den Werkzeugen stehen. Alternativ können lokal auch harte Links
:: verwendet werden ;)
::
set PATH_TO_CONFIGURATION_FILE=%USERPROFILE%\Documents\vs-crpc.txt

if exist %PATH_TO_CONFIGURATION_FILE% (
echo.
echo Visual Studio coverage report path configuration file found.
echo.

for /F "eol=# tokens=1,2* delims==" %%i in (%PATH_TO_CONFIGURATION_FILE%) do (
echo %%i %%j
)

) else (
echo.
echo No configuration, using default settings.
echo.
)

echo.
echo Paths:
echo.
echo   Current         : %CD%
echo   NUnit           : %PATH_TO_NUNIT%
echo   Tests           : %PATH_TO_TESTS%
echo   OpenCover       : %PATH_TO_OPENCOVER%
echo   VS-Project      : %PATH_TO_PROJECT%
echo   ReportGenerator : %PATH_TO_REPORTGENERATOR%
echo   Raw-Reportdata  : %PATH_TO_RAW_REPORTDATA%
echo   HTML-Report     : %PATH_TO_HTML_REPORT%
echo   LaTeX-Report    : %PATH_TO_LATEX_REPORT%
echo.

::
:: Filter für OpenCover hier einstellen:
::
set FILTER=+[Knot3]* -[Knot3]Knot3.Program -[Knot3]Knot3.Development.* -[Knot3]Knot3.Widgets.* -[Knot3]Knot3.Screens.* -[Knot3]Knot3.RenderEffects.* -[Knot3]Knot3.GameObjects.* -[Knot3]Knot3.Input.* -[Knot3]Knot3.Utilities.ShaderHelper -[Knot3]Knot3.Utilities.ModelHelper -[Knot3]Knot3.Core.Knot3Game -[Knot3]Knot3.Core.GameScreen
echo.
echo OpenCover-Filter-Argument:
echo.
echo %FILTER%
echo.

echo ... Running tests, checking coverage ...
echo.
::
:: Hinweis: Auf einem 64-Bit-System ist nunit-console-x86.exe für 32-Bit-Projekte zu verwenden! (sonst tritt ein Fehler auf)
::
"%PATH_TO_OPENCOVER%\OpenCover.Console.exe" -target:"%PATH_TO_NUNIT%\nunit-console-x86.exe" -targetargs:"/noshadow "%PATH_TO_TESTS%\Knot3.UnitTests.dll"" -register:user -filter:"%FILTER%" -output:"%PATH_TO_RAW_REPORTDATA%\NUnit_test_coverage.xml">NUL

echo ... Generating report ...
echo.
echo - HTML
echo.
::
:: Ausgabe von ReportGenerator auf der Konsole wird durch ">NUL" unterdrückt.
:: Erstellung des Berichts als Html:
::
"%PATH_TO_REPORTGENERATOR%\ReportGenerator.exe" -reporttypes:"Html" -reports:"%PATH_TO_RAW_REPORTDATA%\NUnit_test_coverage.xml" -targetdir:"%PATH_TO_HTML_REPORT%"

echo - LaTeX
echo.
::
:: Erstellung des Berichts als LaTeX:
::
"%PATH_TO_REPORTGENERATOR%\ReportGenerator.exe" -reporttypes:"Latex" -reports:"%PATH_TO_RAW_REPORTDATA%\NUnit_test_coverage.xml" -targetdir:"%PATH_TO_LATEX_REPORT%"


echo ... Showing report.
echo.
::
:: Startet das Standardprogramm für das Betrachten von .htm:
::
start "" "%PATH_TO_HTML_REPORT%\index.htm"

pause
exit