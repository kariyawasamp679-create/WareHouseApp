@echo off
rem Creates and starts the LocalDB instance that WareHouseApp uses.
rem Double-click this once, then run WareHouseApp.exe.
set SQLLOCALDB="%ProgramFiles%\Microsoft SQL Server\160\Tools\Binn\SqlLocalDB.exe"
if not exist %SQLLOCALDB% (
  echo SQL Server LocalDB is not installed at the expected location.
  pause
  exit /b 1
)
%SQLLOCALDB% create WarehouseLocal 16.0
%SQLLOCALDB% start WarehouseLocal
%SQLLOCALDB% info WarehouseLocal
echo.
echo Done. If the lines above show "Name: WarehouseLocal" you can now run WareHouseApp.exe.
pause
