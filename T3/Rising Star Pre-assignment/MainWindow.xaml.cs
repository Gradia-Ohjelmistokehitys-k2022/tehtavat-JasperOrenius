using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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

        private void DataPoint_MouseEnter(object sender, MouseEventArgs e)
        {
            if(sender is Ellipse dataPoint && dataPoint.Tag is Tuple<DateTime, double> data)
            {
                string toolTipText = $"{data.Item1:dd-MM-yyyy HH:mm}\nPrice : {data.Item2:F2} €";
                ToolTip toolTip = new ToolTip { Content = toolTipText };
                dataPoint.ToolTip = toolTip;
            }
        }

        private void DataPoint_MouseMove(object sender, MouseEventArgs e)
        {
            if(sender is Ellipse dataPoint)
            {
                ToolTip toolTip = dataPoint.ToolTip as ToolTip;
                if(toolTip != null)
                {
                    toolTip.IsOpen = true;
                }
            }
        }

        private void DrawPriceChart()
        {
            chartCanvas.Children.Clear();
            if (bitcoinPrices == null || !bitcoinPrices.Any()) return;
            double canvasWidth = chartCanvas.ActualWidth;
            double canvasHeight = chartCanvas.ActualHeight;
            double minPrice = bitcoinPrices.Min(p => p.Item2);
            double maxPrice = bitcoinPrices.Max(p => p.Item2);
            int gridLineAmount = 10;
            double priceRange = maxPrice - minPrice;
            double priceInterval = priceRange / gridLineAmount;
            int dateLineAmount = 10;
            int dateInterval = (bitcoinPrices.Count - 1) / (dateLineAmount - 1);
            for(int i = 0; i <= gridLineAmount; i++)
            {
                double price = minPrice + (i * priceInterval);
                double normalizedPrice = (price - minPrice) / priceRange;
                double y = canvasHeight - (normalizedPrice * canvasHeight);
                TextBlock priceLabel = new TextBlock
                {
                    Text = price.ToString("F2") + " €",
                    Foreground = Brushes.Black,
                    FontSize = 12
                };
                Line gridLine = new Line
                {
                    X1 = 0,
                    Y1 = y,
                    X2 = canvasWidth,
                    Y2 = y,
                    Stroke = Brushes.Black,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    StrokeThickness = 1
                };
                Canvas.SetLeft(priceLabel, -60);
                Canvas.SetTop(priceLabel, y - 10);
                chartCanvas.Children.Add(priceLabel);
                chartCanvas.Children.Add(gridLine);
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
                Ellipse dataPoint = new Ellipse
                {
                    Width = 6,
                    Height = 6,
                    Fill = Brushes.Red,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1,
                    Tag = new Tuple<DateTime, double>(bitcoinPrices[i].Item1, bitcoinPrices[i].Item2)
                };
                Canvas.SetLeft(dataPoint, x - 3);
                Canvas.SetTop(dataPoint, y - 3);
                dataPoint.MouseEnter += DataPoint_MouseEnter;
                dataPoint.MouseMove += DataPoint_MouseMove;
                chartCanvas.Children.Add(dataPoint);
                if(i == 0 || i == bitcoinPrices.Count - 1 || (i % dateInterval == 0 && i != 0))
                {
                    TextBlock dateLabel = new TextBlock
                    {
                        Text = bitcoinPrices[i].Item1.ToString("dd-MM-yyyy"),
                        Foreground = Brushes.Black,
                        FontSize = 10
                    };
                    Line dateLine = new Line
                    {
                        X1 = x,
                        Y1 = 0,
                        X2 = x,
                        Y2 = canvasHeight,
                        Stroke = Brushes.Black,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Top,
                        StrokeThickness = 1
                    };
                    Canvas.SetLeft(dateLabel, x - 20);
                    Canvas.SetTop(dateLabel, canvasHeight + 5);
                    chartCanvas.Children.Add(dateLabel);
                    chartCanvas.Children.Add(dateLine);
                }
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
            DrawPriceChart();
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