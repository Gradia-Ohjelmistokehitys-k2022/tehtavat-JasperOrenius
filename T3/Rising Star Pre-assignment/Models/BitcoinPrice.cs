using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Rising_Star_Pre_assignment.Models
{
    class BitcoinPrice
    {
        private List<Tuple<DateTime, double>> bitcoinPrices;
        private List<Tuple<DateTime, double>> bitcoinVolumes;
        public event Action<List<Tuple<DateTime, double>>, List<Tuple<DateTime, double>>> OnDataFetched;

        public async Task FetchBitcoinDataAsync(DateTime startDate, DateTime endDate)
        {
            bitcoinPrices = new List<Tuple<DateTime, double>>();

            DateTime currentStart = startDate;
            TimeSpan chunkSize = TimeSpan.FromDays(365);

            while(currentStart < endDate)
            {
                DateTime currentEnd = currentStart.Add(chunkSize);
                if(currentEnd > endDate) currentEnd = endDate;
                
                long fromUnix = DateTimeToUnixTimestamp(currentStart);
                long toUnix = DateTimeToUnixTimestamp(currentEnd);
                
                string url = $"https://api.coingecko.com/api/v3/coins/bitcoin/market_chart/range?vs_currency=eur&from={fromUnix}&to={toUnix}";
                
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        var response = await client.GetAsync(url);
                        if(response.IsSuccessStatusCode)
                        {
                            var responseBody = await response.Content.ReadAsStringAsync();
                            var marketData = JsonConvert.DeserializeObject<MarketData>(responseBody);
                            ProcessMarketData(marketData);
                        }
                        else
                        {
                            break;
                        }
                    }
                    catch(Exception ex)
                    {
                        break;
                    }
                }
                await Task.Delay(1000);
                currentStart = currentEnd;
            }
        }
        
        private void ProcessMarketData(MarketData marketData)
        {
            bitcoinPrices = new List<Tuple<DateTime, double>>();
            bitcoinVolumes = new List<Tuple<DateTime, double>>();
            foreach (var priceData in marketData.prices)
            {
                DateTime date = UnixToDateTime(priceData[0]);
                double price = priceData[1];
                bitcoinPrices.Add(new Tuple<DateTime, double>(date, price));
            }
            foreach(var volumeData in marketData.total_volumes)
            {
                DateTime date = UnixToDateTime(volumeData[0]);
                double volume = volumeData[1];
                bitcoinVolumes.Add(new Tuple<DateTime, double>(date, volume));
            }
            OnDataFetched?.Invoke(bitcoinPrices, bitcoinVolumes);
        }

        public static long DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (long)(TimeZoneInfo.ConvertTimeToUtc(dateTime) - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static DateTime UnixToDateTime(double unixTime)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds((long)unixTime);
            return dateTimeOffset.UtcDateTime;
        }
    }
}
