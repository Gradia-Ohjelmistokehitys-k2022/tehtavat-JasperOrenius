using Rising_Star_Pre_assignment.Controllers;
using Rising_Star_Pre_assignment.Models;
using Rising_Star_Pre_assignment.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Rising_Star_Pre_assignment.ViewModels
{
    public class DataViewModel : BaseViewModel
    {
        private DateTime? startDate;
        private DateTime? endDate;
        private readonly ChartService chartService;
        private readonly BitcoinPrice bitcoinPrice;

        public ICommand FetchData { get; }
        public ICommand MouseMoveCommand { get; private set; }

        private List<Ellipse> dataPoints = new List<Ellipse>();
        private List<double> dataPointPositions = new List<double>();
        private Canvas chartCanvas;

        public DataViewModel()
        {
            chartService = new ChartService();
            bitcoinPrice = new BitcoinPrice();
            FetchData = new FetchDataCommand(FetchBitcoinDataAsync);
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
            if(bitcoinPrices != null && chartCanvas != null)
            {
                dataPoints.Clear();
                dataPointPositions.Clear();
                chartService.DrawPriceChart(chartCanvas, bitcoinPrices, dataPoints, dataPointPositions);
            }
        }

        public void SetChartCanvas(Canvas canvas)
        {
            chartCanvas = canvas;
        }
    }
}
