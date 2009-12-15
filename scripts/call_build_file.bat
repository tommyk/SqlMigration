@echo on

@echo on
set baseDir=%CD%\..\
cd %baseDir%

%baseDir%\lib\nant\nant.exe -buildfile:build.build %1 %2 %3