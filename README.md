# Slack Bin Reminder
This code base houses the logic for issuing bin collection reminders from my local council via Slack integration. It is split into two parts:

The main logic utilises [SouthGloucestershireBinCollections](https://github.com/BottlecapDave/SGC-Bin-Collection) and [SlackAPI](https://github.com/Inumedia/SlackAPI).

The second part is an AWS Lambda which is run on a daily schedule, with the ability to issue reminders on the day before or the day of the collection.

## Build

Currently this is built and deployed via Visual Studio and the AWS SDK.
