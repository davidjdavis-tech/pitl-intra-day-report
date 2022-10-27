Logs will be made to "C:\ProgramData\DavidIntraDayReport\log.txt"

I'm sure you know this, but for sake of completion, after building and publishing, install service with the command 
sc.exe create DavidIntraDayReport binPath="pathTo DavidIntraDayReport.exe"

Report directory is stored in appsettings.json