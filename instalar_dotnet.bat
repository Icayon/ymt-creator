@echo off
echo ============================================================
echo  YMT Creador - Instalador de .NET 8 SDK
echo ============================================================
echo.
echo Este script descarga e instala el .NET 8 SDK necesario
echo para compilar y ejecutar la herramienta.
echo.
echo Descargando instalador de .NET 8 SDK...
curl -L -o "%TEMP%\dotnet-sdk-installer.exe" "https://aka.ms/dotnet/8.0/dotnet-sdk-win-x64.exe"
echo.
echo Ejecutando instalador...
"%TEMP%\dotnet-sdk-installer.exe"
echo.
echo Instalacion completada. Ahora ejecuta: compilar.bat
pause
