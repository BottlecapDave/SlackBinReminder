dotnet build ./src/SlackBinReminder/SlackBinReminder.Lambda -c Release -o out
dotnet publish ./src/SlackBinReminder/SlackBinReminder.Lambda -c Release -o out /p:GenerateRuntimeConfigurationFiles=true