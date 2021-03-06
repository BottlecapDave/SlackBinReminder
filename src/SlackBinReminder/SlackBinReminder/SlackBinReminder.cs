﻿using SlackAPI;
using Bottlecap.SouthGloucestershireBinCollection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace SlackBinReminder
{
    public class SlackBinReminder
    {
        private readonly ILogger _logger;
        private readonly string _slackApiToken;
        private readonly string _targetChannel;
        private BinCollections _binCollections;

        public SlackBinReminder(ILogger logger, string slackApiToken, string targetChannel, string adjustmentDatesUrl)
        {
            if (String.IsNullOrEmpty(slackApiToken))
            {
                throw new ArgumentException(nameof(slackApiToken));
            }
            else if (String.IsNullOrEmpty(targetChannel))
            {
                throw new ArgumentException(nameof(targetChannel));
            }

            _logger = logger;
            _slackApiToken = slackApiToken;
            _targetChannel = targetChannel;

            _binCollections = new BinCollections(logger, adjustmentDatesUrl);
        }

        public async Task RemindTestAsync(string houseNumber, string postcode)
        {
            var collectionDates = await this.RemindAsync(houseNumber, postcode);
            if (collectionDates != null)
            {
                var builder = new StringBuilder();
                builder.AppendLine("@channel Testing bin reminder");
                builder.AppendLine(String.Format("*Refuse* - {0}", collectionDates.Refuse1));
                builder.AppendLine(String.Format("*Garden* - {0}", collectionDates.GardenWaste1));
                builder.AppendLine(String.Format("*Recycling* - {0}", collectionDates.Recycling1));

                await this.SendSlackMessageAsync(builder.ToString());
            }
            else
            {
                _logger.Log("No collection dates retrieved");
            }
               
        }

        public async Task RemindTomorrowAsync(string houseNumber, string postcode)
        {
            var collectionDates = await this.RemindAsync(houseNumber, postcode);
            if (collectionDates != null)
            {
                var typesBeingCollected = this.GetCollectedTypes(DateTime.UtcNow.Date.AddDays(1), collectionDates);
                if (typesBeingCollected.FirstOrDefault() != null)
                {
                    await this.SendSlackMessageAsync(String.Format("@channel Just a reminder that {0} will be collected tomorrow",
                                                                   this.BuildTypesToString(typesBeingCollected)));
                }
                else
                {
                    _logger.Log("Nothing is being collected");
                }
            }
            else
            {
                _logger.Log("No collection dates retrieved");
            }
        }

        public async Task RemindTodayAsync(string houseNumber, string postcode)
        {
            var collectionDates = await this.RemindAsync(houseNumber, postcode);
            if (collectionDates != null)
            {
                var typesBeingCollected = this.GetCollectedTypes(DateTime.UtcNow.Date, collectionDates);
                if (typesBeingCollected.FirstOrDefault() != null)
                {
                    await this.SendSlackMessageAsync(String.Format("@channel Gentle reminder that {0} will be collected today",
                                                                   this.BuildTypesToString(typesBeingCollected)));
                }
                else
                {
                    _logger.Log("Nothing is being collected");
                }
            }
            else
            {
                _logger.Log("No collection dates retrieved");
            }
        }

        private string BuildTypesToString(IReadOnlyList<string> typesBeingCollected)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < typesBeingCollected.Count; i++)
            {
                var type = typesBeingCollected[i];
                if (i == typesBeingCollected.Count - 1)
                {
                    builder.AppendFormat(" and {0}", type);
                }
                else if (i == 0)
                {
                    builder.Append(type);
                }
                else
                {
                    builder.AppendFormat(", {0}", type);
                }
            }

            return builder.ToString();
        }

        private IReadOnlyList<string> GetCollectedTypes(DateTime targetDate, CollectionDates collectionDates)
        {
            var typesBeingCollected = new List<string>();
            if (collectionDates.Recycling1.HasValue &&
                collectionDates.Recycling1.Value.Date == targetDate.Date)
            {
                typesBeingCollected.Add("*Recycling*");
                typesBeingCollected.Add("*Food Waste*");
            }

            if (collectionDates.Refuse1.HasValue &&
                collectionDates.Refuse1.Value.Date == targetDate.Date)
            {
                typesBeingCollected.Add("*Refuse*");
            }

            if (collectionDates.GardenWaste1.HasValue &&
                collectionDates.GardenWaste1.Value.Date == targetDate.Date)
            {
                typesBeingCollected.Add("*Garden Waste*");
            }

            

            return typesBeingCollected;
        }

        private async Task<CollectionDates> RemindAsync(string houseNumber, string postcode)
        {
            if (String.IsNullOrEmpty(houseNumber))
            {
                throw new ArgumentException(nameof(houseNumber));
            }
            else if (String.IsNullOrEmpty(postcode))
            {
                throw new ArgumentException(nameof(postcode));
            }

            var addresses = await _binCollections.GetAddressesAsync(new Address()
            {
                Property = houseNumber,
                Postcode = postcode
            });

            if (addresses.Count() != 1)
            {
                await this.SendSlackMessageAsync("Unable to find address");
            }

            var targetAddress = addresses.FirstOrDefault();

            var collections = await _binCollections.GetCollectionDatesAsync(targetAddress.Uprn);
            if (collections == null)
            {
                await this.SendSlackMessageAsync("Unable to find bin collection dates");
            }

            return collections;
        }

        private async Task SendSlackMessageAsync(string message)
        {
            var client = new SlackTaskClient(_slackApiToken);
            _logger.Log("Sending message '{0}'", message);
            var response = await client.PostMessageAsync(_targetChannel, message, linkNames: true, as_user: true);
            response.AssertOk();
        }
    }
}
