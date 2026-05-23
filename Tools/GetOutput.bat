@echo off
setlocal enabledelayedexpansion
set "all_output="
set "first_line=1"

for /f "delims=" %%i in ('shutdown /? 2^>^&1') do (
    set "line=%%i"
    set "line=!line:\n=\\n!"
    
    if defined first_line (
        set "all_output=!line!"
        set "first_line="
    ) else (
        set "all_output=!all_output!\n!line!"
    )
)

echo !all_output! > output.txt

echo Done. Saved to output.txt.
pause
