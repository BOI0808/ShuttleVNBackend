@echo off
setlocal

echo Applying migrations...

dotnet ef database update ^
    --project ".\ShuttleVNBackend.Infrastructure" ^
    --startup-project ".\ShuttleVNBackend.Api"

if errorlevel 1 (
    echo.
    echo Applying migrations failed.
    pause
    exit /b 1
)

echo.
echo Applying migrations successfully.
pause