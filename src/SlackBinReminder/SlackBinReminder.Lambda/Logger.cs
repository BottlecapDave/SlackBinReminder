using Amazon.Lambda.Core;
using System;

namespace SlackBinReminder.Lambda
{
    public class Logger : ILogger
    {
        private ILambdaContext _context;

        public Logger(ILambdaContext context)
        {
            _context = context;
        }

        public void Log(string message, params object[] args)
        {
            if (args == null || args.Length < 1)
            {
                _context.Logger.Log(message);
            }
            else
            {
                _context.Logger.Log(String.Format(message, args));
            }
        }
    }
}
