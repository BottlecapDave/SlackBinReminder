# Slack Bin Reminder
This is an `AWS Lambda` which sends reminders for bin collections from South Gloucestershire Council to a configured Slack account.

The main logic for getting the bin collection dates is from [SouthGloucestershireBinCollections](https://github.com/BottlecapDave/SGC-Bin-Collection) and the Slack integration is achieved using [SlackAPI](https://github.com/Inumedia/SlackAPI).

I have the `AWS Lambda` configured to execute on a daily schedule using a `CloudWatch` CRON job.

## Build

Currently this is built and deployed via Visual Studio and the AWS SDK.

## Usage

The AWS Lambda takes the following environment variables

| Environment Variable  | Description |
|-----------------------|-------------|
| SLACK_API_TOKEN       | The API token for the Slack Bot that the reminders will be sent as |
| TARGET_SLACK_CHANNEL  | The channel that the reminder should be sent to. This is the channel name without the `#` |
| HOUSE_NUMBER          | The house number/first line of address of the house that the reminders are for |
| POSTCODE              | The postcode of the house that the reminders are for |
| REMINDER_TARGET       | The type of reminder to send; `tomorrow` - send the reminder if any bins are being collected the next day; `today` - send the reminder if any bins are being collected today; `test` - send a test reminder detailing the next date each bin type is being collected | 
| ADJUSTMENT_DATES_URL  | The url to download adjustment dates to apply to any retrieved collection dates. This is used to solve SGC not updating their dates for events like christmas. The content must be a list in JSON format with "originalDate" and "newDate" fields. |