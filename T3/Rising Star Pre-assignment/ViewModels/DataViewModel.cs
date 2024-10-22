using Rising_Star_Pre_assignment.Commands;
using Rising_Star_Pre_assignment.Controllers;
using Rising_Star_Pre_assignment.Models;
using Rising_Star_Pre_assignment.Services;
using System.Collections.ObjectModel;
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
        
        private void OnBitcoinDataFetched(List<Tuple<DateTime, double>> bitcoinPrices)
        {
            if(bitcoinPrices != null)
            {
                DataPoints.Clear();
                DataPointPositions.Clear();
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
    }
}
