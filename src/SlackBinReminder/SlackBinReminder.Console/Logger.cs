using System;

namespace SlackBinReminder.Console
{
    public class Logger : ILogger
    {
        public void Log(string message, params object[] args)
        {
            if (args == null || args.Length < 1)
            {
                System.Console.WriteLine(message);
            }
            else
            {
                System.Console.WriteLine(String.Format(message, args));
            }
        }
    }
}
