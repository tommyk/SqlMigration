@echo on

@echo on
set baseDir=%CD%\..\
cd %baseDir%

%baseDir%\tools\nant-0.91\NAnt.exe -buildfile:build.build %1 %2 %3