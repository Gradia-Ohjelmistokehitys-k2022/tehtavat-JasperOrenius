using Rising_Star_Pre_assignment.Commands;
using Rising_Star_Pre_assignment.Controllers;
using Rising_Star_Pre_assignment.Models;
using Rising_Star_Pre_assignment.Services;
using System.Collections.ObjectModel;
using System.Security.AccessControl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Rising_Star_Pre_assignment.ViewModels
{
    public class DataViewModel : BaseViewModel
    {
        private readonly ChartInteractionService chartInteractionService;

        private DateTime? startDate;
        private DateTime? endDate;
        private readonly BitcoinPrice bitcoinPrice;

        public ICommand FetchData { get; }
        public ICommand MouseMoveCommand { get; private set; }
        public ICommand MouseEnterCommand { get; private set; }
        public ICommand MouseLeaveCommand { get; private set; }

        private double inputLinePosition;
        public double InputLinePosition
        {
            get => inputLinePosition;
            set
            {
                inputLinePosition = value;
                OnPropertyChanged(nameof(InputLinePosition));
            }
        }

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

        private bool isLineVisible;
        public bool IsLineVisible
        {
            get => isLineVisible;
            set
            {
                isLineVisible = value;
                OnPropertyChanged(nameof(IsLineVisible));
            }
        }

        public ObservableCollection<DataPointViewModel> DataPoints { get; private set; } = new ObservableCollection<DataPointViewModel>();
        public ObservableCollection<double> DataPointPositions { get; private set; } = new ObservableCollection<double>();

        public event EventHandler<List<Tuple<DateTime, double>>> ChartUpdated;

        public DataViewModel(ChartInteractionService chartInteractionService)
        {
            this.chartInteractionService = chartInteractionService;
            bitcoinPrice = new BitcoinPrice();
            FetchData = new FetchDataCommand(FetchBitcoinDataAsync);
            MouseEnterCommand = new MouseEnterCommand(chartInteractionService);
            MouseLeaveCommand = new MouseLeaveCommand(chartInteractionService);
            MouseMoveCommand = new MouseMoveCommand(chartInteractionService);
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
