::
:: Stapelverarbeitungs-Skript für den Testabdeckungsbericht unter Windows (V. 1.1)
::

@echo off
echo.
echo --- OpenCover.bat ---
echo.

::
:: Folgende Tools müssen installiert sein:
::
:: !!!!!!!!!!!!!!!!!!!!!!!!!
::
:: NUnit (2.6.3)
:: OpenCover (4.5.1923)
:: ReportGenerator (dort hin tun: %ProgramFiles(x86)%\ReportGenerator)
::
:: !!!!!!!!!!!!!!!!!!!!!!!!!
::
::
:: Visual Studio: 
::
:: 1. Einstellungen treffen: (Projektmappe)
::
:: NUnit-Test-Coverage-Windows -(Rechtsklick)-> Eigenschaften ->
::
:: Befehlszeile für Postbuildereignis : "$(SolutionDir)NUnit-Test-Coverage-Windows\OpenCover.bat"
:: Postbuildereignis ausführen        : Immer
::
:: 2. Erstellen: (Visual Studio Fenster, Menüzeile oben)
::
:: Erstellen -> NUnit-Test-Coverage-Windows neu erstellen
::
:: ...
::
echo.
echo Settings:
echo.
echo Current path: %CD%
echo.

::
:: Standard-Installationsverzeichnisse für die Werkzeuge verwenden!
::
set PATH_TO_PROJECT=%~dp0..


set PATH_TO_OPENCOVER=%LOCALAPPDATA%\Apps\OpenCover
set PATH_TO_NUNIT=%ProgramFiles(x86)%\NUnit 2.6.3\bin
set PATH_TO_TESTS=%PATH_TO_PROJECT%\tests\bin\Debug
set PATH_TO_REPORTGENERATOR=%ProgramFiles(x86)%\ReportGenerator\bin
set PATH_TO_REPORT=%PATH_TO_PROJECT%\coverage\bin\Debug



set PATH_TO_CONFIGURATION_FILE=%USERPROFILE%\Documents\vs-crpc.txt

if exist %PATH_TO_CONFIGURATION_FILE% (
echo.
echo Visual Studio coverage report path configuration file found.
echo.
) else (
echo.
echo No configuration, using default settings.
echo.
)


::
:: Filter für OpenCover
::
set FILTER="+[*]* -[Knot3.Program]*"


echo Paths to ...
echo.
echo  NUnit           : %PATH_TO_NUNIT%
echo  Tests           : %PATH_TO_TESTS%
echo  OpenCover       : %PATH_TO_OPENCOVER%
echo  VS-Project      : %PATH_TO_PROJECT%
echo  ReportGenerator : %PATH_TO_REPORTGENERATOR%
echo  Report          : %PATH_TO_REPORT%
echo.

::
:: Hinweis: Auf einem 64-Bit-System ist nunit-console-x86.exe für 32-Bit-Projekte zu verwenden! (sonst tritt ein Fehler auf)
::
"%PATH_TO_OPENCOVER%\OpenCover.Console.exe" -target:"%PATH_TO_NUNIT%\nunit-console-x86.exe" -targetargs:"/noshadow "%PATH_TO_TESTS%\Knot3.UnitTests.dll"" -register:user -filter:%FILTER% -output:"%PATH_TO_REPORT%\NUnit_test_coverage.xml"
echo.

::
:: Ausgabe von ReportGenerator auf Konsole unterdrückt.
::
"%PATH_TO_REPORTGENERATOR%\ReportGenerator.exe" "%PATH_TO_REPORT%\NUnit_test_coverage.xml" "%PATH_TO_REPORT%"
REM >NUL
start "" "%PATH_TO_REPORT%\index.htm"