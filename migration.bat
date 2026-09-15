@echo off
setlocal

echo.
set /p MIGRATION_NAME=Enter migration name: 

if "%MIGRATION_NAME%"=="" (
    echo Migration name cannot be empty.
    pause
    exit /b 1
)

echo.
echo Creating migration "%MIGRATION_NAME%"...
echo.

dotnet ef migrations add "%MIGRATION_NAME%" ^
    --project ".\ShuttleVNBackend.Infrastructure" ^
    --startup-project ".\ShuttleVNBackend.Api"

if errorlevel 1 (
    echo.
    echo Migration creation failed.
    pause
    exit /b 1
)

echo.
echo Migration "%MIGRATION_NAME%" created successfully.
pause