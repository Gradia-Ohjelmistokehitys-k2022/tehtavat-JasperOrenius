using Rising_Star_Pre_assignment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace Rising_Star_Pre_assignment.Services
{
    public class ChartService
    {
        public static readonly DependencyProperty BitcoinPricesProperty = DependencyProperty.RegisterAttached("BitcoinPrices", typeof(List<Tuple<DateTime, double>>), typeof(ChartService), new PropertyMetadata(null, OnBitcoinPricesChanged));

        public static List<Tuple<DateTime, double>> GetBitcoinPrices(DependencyObject obj)
        {
            return (List<Tuple<DateTime, double>>)obj.GetValue(BitcoinPricesProperty);
        }

        public static void SetBitcoinPrices(DependencyObject obj, List<Tuple<DateTime, double>> value)
        {
            obj.SetValue(BitcoinPricesProperty, value);
        }

        private static void OnBitcoinPricesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var canvas = d as Canvas;
            var bitcoinPrices = e.NewValue as List<Tuple<DateTime, double>>;
            if(canvas != null && bitcoinPrices != null)
            {
                //DrawPriceChart();
            }
        }

        public void DrawPriceChart(Canvas chartCanvas, List<Tuple<DateTime, double>> bitcoinPrices, List<Ellipse> dataPoints, List<double> dataPointPositions)
        {
            chartCanvas.Children.Clear();
            if (dataPointPositions != null) dataPointPositions.Clear();
            if (bitcoinPrices == null || !bitcoinPrices.Any()) return;
            double canvasWidth = chartCanvas.ActualWidth;
            double canvasHeight = chartCanvas.ActualHeight;
            double minPrice = bitcoinPrices.Min(p => p.Item2);
            double maxPrice = bitcoinPrices.Max(p => p.Item2);
            int gridLineAmount = 10;
            double priceRange = maxPrice - minPrice;
            double priceInterval = priceRange / gridLineAmount;
            int dateLineAmount = 5;
            int dateInterval = Math.Max((bitcoinPrices.Count - 1) / (dateLineAmount - 1), 1);
            double previousX = double.MinValue;
            for (int i = 0; i <= gridLineAmount; i++)
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
                    Stroke = (Brush)new BrushConverter().ConvertFrom("#313436"),
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
                Stroke = (Brush)new BrushConverter().ConvertFrom("#81c995"),
                StrokeThickness = 2
            };
            for (int i = 0; i < bitcoinPrices.Count; i++)
            {
                double normalizedPrice = (bitcoinPrices[i].Item2 - minPrice) / priceRange;
                double x = i * stepX;
                double y = canvasHeight - (normalizedPrice * canvasHeight);
                polyline.Points.Add(new Point(x, y));
                Ellipse dataPoint = new Ellipse
                {
                    Width = 6,
                    Height = 6,
                    Fill = (Brush)new BrushConverter().ConvertFrom("#81c995"),
                    Stroke = (Brush)new BrushConverter().ConvertFrom("#81c995"),
                    StrokeThickness = 1,
                    Tag = new Tuple<DateTime, double>(bitcoinPrices[i].Item1, bitcoinPrices[i].Item2)
                };
                Canvas.SetLeft(dataPoint, x - 3);
                Canvas.SetTop(dataPoint, y - 3);
                dataPointPositions.Add(x);
                dataPoints.Add(dataPoint);
                chartCanvas.Children.Add(dataPoint);
                if (i == 0 || i == bitcoinPrices.Count - 1 || (i % dateInterval == 0 && i != 0))
                {
                    if (Math.Abs(x - previousX) < 40) continue;
                    previousX = x;
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
                        Stroke = (Brush)new BrushConverter().ConvertFrom("#313436"),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Top,
                        StrokeThickness = 1
                    };
                    var labelWidth = dateLabel.ActualHeight;
                    Canvas.SetLeft(dateLabel, x - (labelWidth / 2));
                    Canvas.SetTop(dateLabel, canvasHeight + 5);
                    chartCanvas.Children.Add(dateLabel);
                    chartCanvas.Children.Add(dateLine);
                }
            }
            chartCanvas.Children.Add(polyline);
        }
    }
}
