using Newtonsoft.Json;
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
        public MainWindow()
        {
            InitializeComponent();
            //startDatePicker.SelectedDate = DateTime.Today.AddDays(-1);
            //endDatePicker.SelectedDate = DateTime.Today;
        }
        /*
        private async void FetchData_Click(object sender, RoutedEventArgs e)
        {
            DateTime startDate = startDatePicker.SelectedDate ?? DateTime.Now;
            DateTime endDate = endDatePicker.SelectedDate ?? DateTime.Now;
            await FetchBitcoinDataAsync(startDate, endDate);
        }

        private void Chart_MouseEnter(object sender, MouseEventArgs e)
        {
            double canvasHeight = chartCanvas.ActualHeight;
            Point mousePosition = Mouse.GetPosition(chartCanvas);
            inputLine.X1 = mousePosition.X;
            inputLine.Y1 = 0;
            inputLine.X2 = mousePosition.X;
            inputLine.Y2 = canvasHeight;
            inputLine.Stroke = Brushes.White;
        }

        private void Chart_MouseLeave(object sender, MouseEventArgs e)
        {
            chartCanvas.Children.Remove(inputLine);
            foreach(Ellipse dataPoint in dataPoints)
            {
                ToolTip toolTip = dataPoint.ToolTip as ToolTip;
                if (toolTip != null)
                {
                    toolTip.IsOpen = false;
                }
            }
        }
        */
    }
}