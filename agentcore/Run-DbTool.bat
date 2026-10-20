@echo off
title agentcore Database Administrator (phpMyAdmin Style)
echo ========================================================
echo    agentcore Database Administrator (phpMyAdmin Style)
echo    City Computers Marot - 03447436314
echo ========================================================
echo.
echo Starting Database Management Tool...
cd /d "%~dp0agentcore.DbTool\bin\Debug\net8.0-windows"
start "" "agentcore.DbTool.exe"
exit
