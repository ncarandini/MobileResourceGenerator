To build a self contained exe file, use this command:

dotnet publish -r win-x64 -c Release /p:PublishSingleFile=true /p:PublishTrimmed=true