using Rising_Star_Pre_assignment.Commands;
using Rising_Star_Pre_assignment.Controllers;
using Rising_Star_Pre_assignment.Models;
using Rising_Star_Pre_assignment.Services;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rising_Star_Pre_assignment.ViewModels
{
    public class DataViewModel : BaseViewModel
    {
        public readonly ChartInteractionService chartInteractionService;

        private DateTime? startDate;
        private DateTime? endDate;
        private readonly BitcoinPrice bitcoinPrice;

        private double? lowestPrice;
        public double? LowestPrice
        {
            get => lowestPrice;
            set
            {
                lowestPrice = value;
                OnPropertyChanged(nameof(LowestPrice));
            }
        }

        private DateTime? lowestPriceDate;
        public DateTime? LowestPriceDate
        {
            get => lowestPriceDate;
            set { 
                lowestPriceDate = value; 
                OnPropertyChanged(nameof(LowestPriceDate)); 
            }
        }

        private double? highestPrice;
        public double? HighestPrice
        {
            get => lowestPrice;
            set
            {
                lowestPrice = value;
                OnPropertyChanged(nameof(HighestPrice));
            }
        }

        private DateTime? highestPriceDate;
        public DateTime? HighestPriceDate
        {
            get => highestPriceDate;
            set
            {
                lowestPriceDate = value;
                OnPropertyChanged(nameof(HighestPriceDate));
            }
        }

        private double? lowestVolume;
        public double? LowestVolume
        {
            get => lowestVolume;
            set
            {
                lowestVolume = value;
                OnPropertyChanged(nameof(LowestVolume));
            }
        }

        private DateTime? lowestVolumeDate;
        public DateTime? LowestVolumeDate
        {
            get => lowestVolumeDate;
            set
            {
                lowestVolumeDate = value;
                OnPropertyChanged(nameof(LowestVolumeDate));
            }
        }

        private double? highestVolume;
        public double? HighestVolume
        {
            get => highestVolume;
            set
            {
                highestVolume = value;
                OnPropertyChanged(nameof(HighestVolume));
            }
        }

        private DateTime? highestVolumeDate;
        public DateTime? HighestVolumeDate
        {
            get => highestVolumeDate;
            set
            {
                highestVolumeDate = value;
                OnPropertyChanged(nameof(HighestVolumeDate));
            }
        }

        private string? longestBullishTrendStart;
        public string? LongestBullishTrendStart
        {
            get => longestBullishTrendStart;
            set
            {
                longestBullishTrendStart = value;
                OnPropertyChanged(nameof(LongestBullishTrendStart));
            }
        }

        private string? longestBullishTrendEnd;
        public string? LongestBullishTrendEnd
        {
            get => longestBullishTrendEnd;
            set
            {
                longestBullishTrendEnd = value;
                OnPropertyChanged(nameof(LongestBullishTrendEnd));
            }
        }

        private string? longestBearishTrendStart;
        public string? LongestBearishTrendStart
        {
            get => longestBearishTrendStart;
            set
            {
                longestBearishTrendStart = value;
                OnPropertyChanged(nameof(LongestBearishTrendStart));
            }
        }

        private string? longestBearishTrendEnd;
        public string? LongestBearishTrendEnd
        {
            get => longestBearishTrendEnd;
            set
            {
                longestBearishTrendEnd = value;
                OnPropertyChanged(nameof(LongestBearishTrendEnd));
            }
        }

        private string? bestBuyDate;
        public string? BestBuyDate
        {
            get => bestBuyDate;
            set
            {
                bestBuyDate = value;
                OnPropertyChanged(nameof(BestBuyDate));
            }
        }

        private string? bestSellDate;
        public string? BestSellDate
        {
            get => bestSellDate;
            set
            {
                bestSellDate = value;
                OnPropertyChanged(nameof(BestSellDate));
            }
        }

        private string? bestSellFirstDate;
        public string? BestSellFirstDate
        {
            get => bestSellFirstDate;
            set
            {
                bestSellFirstDate = value;
                OnPropertyChanged(nameof(BestSellFirstDate));
            }
        }

        private string? bestBuyBackDate;
        public string? BestBuyBackDate
        {
            get => bestBuyBackDate;
            set
            {
                bestBuyBackDate = value;
                OnPropertyChanged(nameof(BestBuyBackDate));
            }
        }

        public ICommand FetchData { get; }
        public ICommand MouseMoveCommand { get; private set; }
        public ICommand MouseEnterCommand { get; private set; }
        public ICommand MouseLeaveCommand { get; private set; }

        private int? currentDataPointIndex;
        public int? CurrentDataPointIndex
        {
            get => currentDataPointIndex;
            set
            {
                currentDataPointIndex = value;
                OnPropertyChanged(nameof(CurrentDataPointIndex));
            }
        }

        public ObservableCollection<DataPointViewModel> DataPoints { get; private set; } = new ObservableCollection<DataPointViewModel>();
        public ObservableCollection<double> DataPointPositions { get; private set; } = new ObservableCollection<double>();

        public event EventHandler<List<Tuple<DateTime, double>>> ChartUpdated;

        public DataViewModel(ChartInteractionService chartInteractionService, Canvas? chartCanvas)
        {
            this.chartInteractionService = chartInteractionService;
            startDate = DateTime.Today.AddDays(-1);
            endDate = DateTime.Today;
            bitcoinPrice = new BitcoinPrice();
            FetchData = new FetchDataCommand(FetchBitcoinDataAsync);
            MouseEnterCommand = new MouseEnterCommand(chartInteractionService);
            MouseLeaveCommand = new MouseLeaveCommand(chartInteractionService);
            MouseMoveCommand = new MouseMoveCommand(chartInteractionService, chartCanvas);
            bitcoinPrice.OnDataFetched += OnBitcoinDataFetched;
        }

        public DateTime? StartDate
        {
            get => startDate;
            set
            {
                startDate = value;
                OnPropertyChanged(nameof(StartDate));
            }
        }

        public DateTime? EndDate
        {
            get => endDate;
            set
            {
                endDate = value;
                OnPropertyChanged(nameof(EndDate));
            }
        }

        private async Task FetchBitcoinDataAsync()
        {
            if(StartDate.HasValue && EndDate.HasValue)
            {
                await bitcoinPrice.FetchBitcoinDataAsync(StartDate.Value, EndDate.Value);
            }
        }
        
        private void OnBitcoinDataFetched(List<Tuple<DateTime, double>> bitcoinPrices, List<Tuple<DateTime, double>> bitcoinVolumes)
        {
            if(bitcoinPrices != null)
            {
                DataPoints.Clear();
                DataPointPositions.Clear();

                var minPrice = bitcoinPrices.MinBy(data => data.Item2);
                var maxPrice = bitcoinPrices.MaxBy(data => data.Item2);

                LowestPrice = minPrice?.Item2;
                LowestPriceDate = minPrice?.Item1;

                HighestPrice = maxPrice?.Item2;
                HighestPriceDate = maxPrice?.Item1;

                var minVolume = bitcoinVolumes.MinBy(data => data.Item2);
                var maxVolume = bitcoinVolumes.MaxBy(data => data.Item2);

                LowestVolume = minVolume?.Item2;
                LowestVolumeDate = minVolume?.Item1;

                HighestVolume = maxVolume?.Item2;
                HighestVolumeDate = maxVolume?.Item1;

                var bullishTrend = CalculateTrendLenght(bitcoinPrices, true);
                var bearishTrend = CalculateTrendLenght(bitcoinPrices, false);

                LongestBullishTrendStart = bullishTrend.start?.ToString("dd/MM/yyyy hh:mm");
                LongestBullishTrendEnd = bullishTrend.end?.ToString("dd/MM/yyyy hh:mm");
                LongestBearishTrendStart = bearishTrend.start?.ToString("dd/MM/yyyy hh:mm");
                LongestBearishTrendEnd = bearishTrend.end?.ToString("dd/MM/yyyy hh:mm");

                var bestBuySell = FindBestBuySellDays(bitcoinPrices);
                BestBuyDate = bestBuySell.Item1?.ToString("dd/MM/yyyy hh:mm");
                BestSellDate = bestBuySell.Item2?.ToString("dd/MM/yyyy hh:mm");

                var bestSellBuy = FindBestSellBuyDays(bitcoinPrices);
                BestSellFirstDate = bestSellBuy.Item1?.ToString("dd/MM/yyyy hh:mm");
                BestBuyBackDate = bestSellBuy.Item2?.ToString("dd/MM/yyyy hh:mm");

                foreach (var bitcoinData in bitcoinPrices)
                {
                    var dateTime = bitcoinData.Item1;
                    var price = bitcoinData.Item2;
                    var dataPoint = new DataPointViewModel
                    {
                        Date = dateTime,
                        Price = price,
                        X = dateTime.ToOADate(),
                        Y = price
                    };
                    DataPoints.Add(dataPoint);
                    DataPointPositions.Add(dataPoint.X);
                }
                OnPropertyChanged(nameof(DataPoints));
                OnPropertyChanged(nameof(DataPointPositions));
                ChartUpdated?.Invoke(this, bitcoinPrices);
            }
        }

        private (int length, DateTime? start, DateTime? end) CalculateTrendLenght(List<Tuple<DateTime, double>> bitcoinPrices, bool isBullish)
        {
            if(bitcoinPrices == null || bitcoinPrices.Count < 2) return (0, null, null);
            int longestTrend = 0;
            int currentTrend = 0;
            DateTime? currentStart = null;
            DateTime? longestStart = null;
            DateTime? longestEnd = null;
            for(int i = 1; i < bitcoinPrices.Count; i++)
            {
                if((isBullish && bitcoinPrices[i].Item2 > bitcoinPrices[i - 1].Item2) || (!isBullish && bitcoinPrices[i].Item2 < bitcoinPrices[i - 1].Item2))
                {
                    currentTrend++;
                    if(currentStart == null)
                    {
                        currentStart = bitcoinPrices[i - 1].Item1;
                    }
                }
                else
                {
                    if(currentTrend > longestTrend)
                    {
                        longestTrend = currentTrend;
                        longestStart = currentStart;
                        longestEnd = bitcoinPrices[i - 1].Item1;
                    }
                    currentTrend = 0;
                    currentStart = null;
                }
            }
            if(currentTrend > longestTrend)
            {
                longestTrend = currentTrend;
                longestStart = currentStart;
                longestEnd = bitcoinPrices[^1].Item1;
            }
            return (longestTrend, longestStart, longestEnd);
        }

        private Tuple<DateTime?, DateTime?> FindBestBuySellDays(List<Tuple<DateTime, double>> bitcoinPrices)
        {
            if(bitcoinPrices == null || bitcoinPrices.Count < 2) return Tuple.Create<DateTime?, DateTime?>(null, null);
            double minPrice = double.MaxValue;
            double maxProfit = 0;
            DateTime? buyDate = null;
            DateTime? sellDate = null;
            foreach(var bitcoinData in bitcoinPrices)
            {
                if(bitcoinData.Item2 < minPrice)
                {
                    minPrice = bitcoinData.Item2;
                    buyDate = bitcoinData.Item1;
                }
                double profit = bitcoinData.Item2 - minPrice;
                if(profit > maxProfit)
                {
                    maxProfit = profit;
                    sellDate = bitcoinData.Item1;
                }
            }
            return Tuple.Create(buyDate, sellDate);
        }

        private Tuple<DateTime?, DateTime?> FindBestSellBuyDays(List<Tuple<DateTime, double>> bitcoinPrices)
        {
            if (bitcoinPrices == null || bitcoinPrices.Count < 2) return Tuple.Create<DateTime?, DateTime?>(null, null);
            double maxPrice = double.MinValue;
            double maxLoss = 0;
            DateTime? sellDate = null;
            DateTime? buyDate = null;
            foreach(var bitcoinData in bitcoinPrices)
            {
                if(bitcoinData.Item2 > maxPrice)
                {
                    maxPrice = bitcoinData.Item2;
                    sellDate = bitcoinData.Item1;
                }
                double loss = maxPrice - bitcoinData.Item2;
                if(loss > maxLoss)
                {
                    maxLoss = loss;
                    buyDate = bitcoinData.Item1;
                }
            }
            return Tuple.Create(sellDate, buyDate);
        }
    }
}
