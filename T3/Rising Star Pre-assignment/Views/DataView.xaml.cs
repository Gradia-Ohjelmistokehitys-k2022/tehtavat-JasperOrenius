using Rising_Star_Pre_assignment.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rising_Star_Pre_assignment.ViewModels
{
    /// <summary>
    /// Interaction logic for DataView.xaml
    /// </summary>
    public partial class DataView : UserControl
    {
        private readonly ChartService chartService;
        public DataView()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            chartService = new ChartService();
        }

        private void DataViewModel_ChartUpdated(object? sender, List<Tuple<DateTime, double>> bitcoinPrices)
        {
            List<Ellipse> dataPoints = new List<Ellipse>();
            List<double> dataPointPositions = new List<double>();
            chartService.DrawPriceChart(chartCanvas, bitcoinPrices, dataPoints, dataPointPositions);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var viewModel = (DataViewModel)DataContext;
            if(viewModel != null )
            {
                viewModel.ChartUpdated += DataViewModel_ChartUpdated;
            }
        }
    }
}
