using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Rising_Star_Pre_assignment.Models
{
    class BitcoinPrice
    {
        private List<Tuple<DateTime, double>> bitcoinPrices;
        public event Action<List<Tuple<DateTime, double>>> OnDataFetched;

        public async Task FetchBitcoinDataAsync(DateTime startDate, DateTime endDate)
        {
            long fromUnix = DateTimeToUnixTimestamp(startDate);
            long toUnix = DateTimeToUnixTimestamp(endDate.AddHours(1));
            string url = $"https://api.coingecko.com/api/v3/coins/bitcoin/market_chart/range?vs_currency=eur&from={fromUnix}&to={toUnix}";
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetStringAsync(url);
                var marketData = JsonConvert.DeserializeObject<MarketData>(response);
                ProcessMarketData(marketData);
            }
        }
        
        private void ProcessMarketData(MarketData marketData)
        {
            bitcoinPrices = new List<Tuple<DateTime, double>>();
            foreach (var priceData in marketData.prices)
            {
                DateTime date = UnixToDateTime(priceData[0]);
                double price = priceData[1];
                bitcoinPrices.Add(new Tuple<DateTime, double>(date, price));
            }
            OnDataFetched?.Invoke(bitcoinPrices);
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
