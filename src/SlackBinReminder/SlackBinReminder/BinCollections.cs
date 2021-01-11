using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bottlecap.SouthGloucestershireBinCollection;
using RestSharp;

namespace SlackBinReminder
{
    public class BinCollections : SouthGloucestershireBinCollections
    {
        private readonly ILogger _logger;
        private readonly string _adjustmentDatesUrl;

        public BinCollections(ILogger logger, string adjustmentDatesUrl)
        {
            _logger = logger;
            _adjustmentDatesUrl = adjustmentDatesUrl;
        }

        protected override async Task<IEnumerable<DateAdjustment>> GetDateAdjustmentsAsync()
        {
            List<DateAdjustment> adjustmentDates;
            if (String.IsNullOrEmpty(_adjustmentDatesUrl) == false)
            {
                var client = new RestClient(_adjustmentDatesUrl);
                client.AddHandler("application/json", () => UKJsonSerializer.Default);

                var request = new RestRequest();
                request.AddHeader("Content-Type", "application/json");

                adjustmentDates = await client.GetAsync<List<DateAdjustment>>(request);
            }
            else
            {
                adjustmentDates = new List<DateAdjustment>();
            }

            foreach (var adjustment in adjustmentDates)
            {
                _logger.Log($"Date Adjustment - Original Date: {adjustment.OriginalDate}; New Date: {adjustment.NewDate}\n");
            }

            return adjustmentDates;
        }
    }
}
