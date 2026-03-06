@echo off
echo ================================
echo   Clean Architecture Generator
echo ================================
echo.

set /p ProjectName="Enter project name: "
set /p OutputPath="Enter output path (e.g. C:\Projects): "

dotnet new cleanarch -n %ProjectName% -o "%OutputPath%\%ProjectName%"

echo.
echo ✅ Project "%ProjectName%" created at "%OutputPath%\%ProjectName%"
pause
```

لما تشغله:
```
Enter project name: AwesomeApp
Enter output path: C:\Projects

✅ Project "AwesomeApp" created at "C:\Projects\AwesomeApp"