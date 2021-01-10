dotnet build ./src/SlackBinReminder -c Release -o out
dotnet publish ./src/SlackBinReminder -c Release -o out /p:GenerateRuntimeConfigurationFiles=true