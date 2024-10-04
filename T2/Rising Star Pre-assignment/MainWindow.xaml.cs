using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rising_Star_Pre_assignment
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Tuple<DateTime, double>> bitcoinPrices;
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void FetchData_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDate = startDatePicker.SelectedDate ?? DateTime.Now;
            DateTime endDate = endDatePicker.SelectedDate ?? DateTime.Now;
            await FetchBitcoinDataAsync(startDate, endDate);
        }

        private void DrawChart_Click(object sender, RoutedEventArgs e)
        {
            DrawPriceChart();
        }

        private void DrawPriceChart()
        {
            chartCanvas.Children.Clear();
            if (bitcoinPrices == null || !bitcoinPrices.Any()) return;
            double canvasWidth = chartCanvas.ActualWidth;
            double canvasHeight = chartCanvas.ActualHeight;
            double minPrice = bitcoinPrices.Min(p => p.Item2);
            double maxPrice = bitcoinPrices.Max(p => p.Item2);
            int gridLineAmount = 5;
            double priceRange = maxPrice - minPrice;
            double priceInterval = priceRange / gridLineAmount;
            for(int i = 0; i <= gridLineAmount; i++)
            {
                double price = minPrice + (i * priceInterval);
                double normalizedPrice = (price - minPrice) / priceRange;
                double y = canvasHeight - (normalizedPrice * canvasHeight);
                Line gridLine = new Line
                {
                    X1 = 0,
                    Y1 = y,
                    X2 = canvasWidth,
                    Y2 = y,
                    Stroke = Brushes.LightGray,
                    StrokeThickness = 1
                };
                chartCanvas.Children.Add(gridLine);
                TextBlock priceLabel = new TextBlock
                {
                    Text = price.ToString("F2"),
                    Foreground = Brushes.Black,
                    FontSize = 12
                };
                Canvas.SetLeft(priceLabel, -60);
                Canvas.SetTop(priceLabel, y - 10);
                chartCanvas.Children.Add(priceLabel);
            }
            double stepX = canvasWidth / (bitcoinPrices.Count - 1);
            Polyline polyline = new Polyline
            {
                Stroke = Brushes.Blue, 
                StrokeThickness = 2
            };
            for(int i = 0; i < bitcoinPrices.Count; i++)
            {
                double normalizedPrice = (bitcoinPrices[i].Item2 - minPrice) / priceRange;
                double x = i * stepX;
                double y = canvasHeight - (normalizedPrice * canvasHeight);
                polyline.Points.Add(new Point(x, y));
            }
            chartCanvas.Children.Add(polyline);
        }

        private async Task FetchBitcoinDataAsync(DateTime startDate, DateTime endDate)
        {
            long fromUnix = DateTimeToUnixTimestamp(startDate);
            long toUnix = DateTimeToUnixTimestamp(endDate.AddHours(1));
            string url = $"https://api.coingecko.com/api/v3/coins/bitcoin/market_chart/range?vs_currency=eur&from={fromUnix}&to={toUnix}";
            using(HttpClient client  = new HttpClient())
            {
                var response = await client.GetStringAsync(url);
                var marketData = JsonConvert.DeserializeObject<MarketData>(response);
                ProcessMarketData(marketData);
            }
        }

        public static long DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (long)(TimeZoneInfo.ConvertTimeToUtc(dateTime) - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        private void ProcessMarketData(MarketData marketData)
        {
            bitcoinPrices = new List<Tuple<DateTime, double>>();
            foreach(var priceData in marketData.prices)
            {
                DateTime date = UnixToDateTime(priceData[0]);
                double price = priceData[1];
                bitcoinPrices.Add(new Tuple<DateTime, double>(date, price));
            }
            resultTextBlock.Text = "Data fetched succesfully.";
        }

        public static DateTime UnixToDateTime(double unixTime)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds((long)unixTime);
            return dateTimeOffset.UtcDateTime;
        }
    }

    public class MarketData
    {
        public List<List<double>> prices { get; set; }
        public List<List<double>> total_volumes { get; set; }
    }
}