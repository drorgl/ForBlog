@echo off
set archivepath=c:\
set fullbackupfilename="%archivepath%\Backups\backupfull.7z"

set diffbackupfilename=%archivepath%\Backups\backupdiff

for /f "delims=/ tokens=1-3" %%a in ("%DATE:~4%") do (
	for /f "delims=:. tokens=1-4" %%m in ("%TIME: =0%") do (
		set FILENAME=%diffbackupfilename%-%%c-%%b-%%a-%%m%%n%%o%%p
   )
)

set diffbackupfilename="%filename%.7z"


set backupfolders="C:\Users\Username" "C:\Documents"

if exist %fullbackupfilename%  goto diff_backup

7z.exe a -p -mhe -ms=off -ssw -mx=7 -xr!*.vhd %fullbackupfilename% %backupfolders% 
goto end
:diff_backup

7z.exe u -p -mhe -ms=off -ssw %fullbackupfilename%  %backupfolders% -xr!*.vhd -mx=9 -t7z -u- -up0q3r2x2y2z0w2!%diffbackupfilename% 

:end

