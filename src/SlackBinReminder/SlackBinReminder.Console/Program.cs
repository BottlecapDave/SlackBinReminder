using System;
using System.Threading.Tasks;

namespace SlackBinReminder.Console
{
    public class Program
    {
        public const string ENVIRONMENT_VARIABLE_NAME_SLACK_API_TOKEN = "SLACK_API_TOKEN";
        public const string ENVIRONMENT_VARIABLE_NAME_TARGET_SLACK_CHANNEL = "TARGET_SLACK_CHANNEL";
        public const string ENVIRONMENT_VARIABLE_ADJUSTMENT_DATES_URL = "ADJUSTMENT_DATES_URL";

        public const string ENVIRONMENT_VARIABLE_NAME_HOUSE_NUMBER = "HOUSE_NUMBER";
        public const string ENVIRONMENT_VARIABLE_NAME_POSTCODE = "POSTCODE";

        public const string ENVIRONMENT_VARIABLE_NAME_REMINDER_TARGET = "REMINDER_TARGET";

        static void Main(string[] args)
        {
            var logger = new Logger();
            var slackApiToken = Environment.GetEnvironmentVariable(Program.ENVIRONMENT_VARIABLE_NAME_SLACK_API_TOKEN);
            var targetSlackChannel = Environment.GetEnvironmentVariable(Program.ENVIRONMENT_VARIABLE_NAME_TARGET_SLACK_CHANNEL);
            var adjustmentDatesUrl = Environment.GetEnvironmentVariable(Program.ENVIRONMENT_VARIABLE_ADJUSTMENT_DATES_URL);
            var houseNumber = Environment.GetEnvironmentVariable(Program.ENVIRONMENT_VARIABLE_NAME_HOUSE_NUMBER);
            var postcode = Environment.GetEnvironmentVariable(Program.ENVIRONMENT_VARIABLE_NAME_POSTCODE);
            var target = Environment.GetEnvironmentVariable(Program.ENVIRONMENT_VARIABLE_NAME_REMINDER_TARGET);

            logger.Log($"{Program.ENVIRONMENT_VARIABLE_NAME_SLACK_API_TOKEN}: {slackApiToken}");
            logger.Log($"{Program.ENVIRONMENT_VARIABLE_NAME_TARGET_SLACK_CHANNEL}: {targetSlackChannel}");
            logger.Log($"{Program.ENVIRONMENT_VARIABLE_ADJUSTMENT_DATES_URL}: {adjustmentDatesUrl}");
            logger.Log($"{Program.ENVIRONMENT_VARIABLE_NAME_HOUSE_NUMBER}: {houseNumber}");
            logger.Log($"{Program.ENVIRONMENT_VARIABLE_NAME_POSTCODE}: {postcode}");
            logger.Log($"{Program.ENVIRONMENT_VARIABLE_NAME_REMINDER_TARGET}: {target}");

            var reminder = new SlackBinReminder(logger,
                                                slackApiToken,
                                                targetSlackChannel,
                                                adjustmentDatesUrl);



            switch (target)
            {
                case "today":
                    Task.WaitAll(reminder.RemindTodayAsync(houseNumber, postcode));
                    break;
                case "tomorrow":
                    Task.WaitAll(reminder.RemindTomorrowAsync(houseNumber, postcode));
                    break;
                case "test":
                    Task.WaitAll(reminder.RemindTestAsync(houseNumber, postcode));
                    break;
                default:
                    logger.Log(String.Format("Unexpected target {0}", target));
                    break;
            }
        }
    }
}
