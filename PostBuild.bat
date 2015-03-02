REM Needs to be supplied with quotes around the solution directory.
set SolutionDir=%1
set SolutionName=%2
set ProjectName=%3
set ConfigurationName=%4
REM Needs to be supplied with quotes around the target path.
set TargetPath=%5
set TargetFileName=%6
REM Change to the solution drive
%SolutionDir:~1,2%
REM Change to the solution directory
cd %SolutionDir%
REM Make any directories required
mkdir "..\SharedLibraries\%ConfigurationName%\%SolutionName%"
REM Copy the built DLL to the SharedLibraries directory
copy %TargetPath% "..\SharedLibraries\%ConfigurationName%\%SolutionName%\%TargetFileName%"
REM Copy the built XML Documentation file to the ShareLibraries directory if this is a Release build
if %ConfigurationName%==Release copy "%SolutionDir:~1,-1%%ProjectName%\bin\%ConfigurationName%\%TargetFileName:~0,-4%.xml" "..\SharedLibraries\Release\%SolutionName%\%TargetFileName:~0,-4%.xml"