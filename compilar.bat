@echo off
echo ============================================================
echo  YMT Creador - Compilar
echo ============================================================
echo.
cd /d "%~dp0"
dotnet publish YMTCreator\YMTCreator.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o dist
if %errorlevel% NEQ 0 (
    echo.
    echo ERROR al compilar. Asegurate de tener .NET 8 SDK instalado.
    echo Ejecuta: instalar_dotnet.bat
    pause
    exit /b 1
)
echo.
echo ============================================================
echo  Compilado correctamente en: dist\YMTCreator.exe
echo ============================================================
pause
