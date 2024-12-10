using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Rising_Star_Pre_assignment.Models
{
    class BitcoinPrice
    {
        private List<Tuple<DateTime, double>> bitcoinPrices;
        private List<Tuple<DateTime, double>> bitcoinVolumes;
        public event Action<List<Tuple<DateTime, double>>, List<Tuple<DateTime, double>>> OnDataFetched;

        public async Task FetchBitcoinDataAsync(DateTime startDate, DateTime endDate)
        {
            long fromUnix = DateTimeToUnixTimestamp(startDate);
            long toUnix = DateTimeToUnixTimestamp(endDate);
            
            string url = $"https://api.coingecko.com/api/v3/coins/bitcoin/market_chart/range?vs_currency=eur&from={fromUnix}&to={toUnix}";
           
            try
            {
                using(HttpClient client = new HttpClient())
                {
                    var response = await client.GetStringAsync(url);
                    var marketData = JsonConvert.DeserializeObject<MarketData>(response);
                    ProcessMarketData(marketData);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Something went wrong!");
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
