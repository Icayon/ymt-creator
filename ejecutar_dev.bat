@echo off
echo ============================================================
echo  YMT Creador - Ejecutar (modo dev, requiere .NET 8 SDK)
echo ============================================================
cd /d "%~dp0"
dotnet run --project YMTCreator\YMTCreator.csproj
pause
