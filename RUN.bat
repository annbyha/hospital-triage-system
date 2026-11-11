@echo off
title Hospital Patient Triage System
cd /d "%~dp0"
where dotnet >nul 2>nul
if errorlevel 1 (
  echo The .NET SDK is not installed. Download .NET 10 SDK from https://dotnet.microsoft.com/download
  pause
  exit /b 1
)
echo Building and starting Hospital Patient Triage System...
dotnet run --project HospitalTriageGUI\HospitalTriageGUI.csproj
if errorlevel 1 pause
