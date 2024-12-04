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
using System.Windows.Navigation;

namespace Rising_Star_Pre_assignment.Services
{
    public class ChartService
    {
        public void DrawPriceChart(Canvas chartCanvas, List<Tuple<DateTime, double>> bitcoinPrices, List<Ellipse> dataPoints, List<Point> dataPointPositions)
        {
            chartCanvas.Children.Clear();
            if (dataPointPositions != null) dataPointPositions.Clear();
            if (bitcoinPrices == null || !bitcoinPrices.Any()) return;

            int maxDataPoints = 50;
            List<Tuple<DateTime, double>> sampledPrices;
            if(bitcoinPrices.Count > maxDataPoints)
            {
                int interval = bitcoinPrices.Count / maxDataPoints;
                sampledPrices = bitcoinPrices.Where((_, index) => index % interval == 0).ToList();
                if(!sampledPrices.Contains(bitcoinPrices.Last()))
                {
                    sampledPrices.Add(bitcoinPrices.Last());
                }
            }
            else
            {
                sampledPrices = bitcoinPrices;
            }

            double canvasWidth = chartCanvas.ActualWidth;
            double canvasHeight = chartCanvas.ActualHeight;

            double minPrice = sampledPrices.Min(p => p.Item2);
            double maxPrice = sampledPrices.Max(p => p.Item2);

            int gridLineAmount = 10;
            double priceRange = maxPrice - minPrice;
            double priceInterval = priceRange / gridLineAmount;

            int dateLineAmount = 5;
            int dateInterval = Math.Max((sampledPrices.Count - 1) / (dateLineAmount - 1), 1);

            double previousX = double.MinValue;

            for (int i = 0; i <= gridLineAmount; i++)
            {
                double price = minPrice + (i * priceInterval);
                double normalizedPrice = (price - minPrice) / priceRange;
                double y = canvasHeight - (normalizedPrice * canvasHeight);

                TextBlock priceLabel = new TextBlock
                {
                    Text = price.ToString("F2") + " €",
                    Foreground = Brushes.LightGray,
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

            double stepX = canvasWidth / (sampledPrices.Count - 1);

            for (int i = 0; i < sampledPrices.Count; i++)
            {
                double normalizedPrice = (sampledPrices[i].Item2 - minPrice) / priceRange;
                double x = i * stepX;
                double y = canvasHeight - (normalizedPrice * canvasHeight);

                Ellipse dataPoint = new Ellipse
                {
                    Width = 2,
                    Height = 2,
                    Fill = (Brush)new BrushConverter().ConvertFrom("#81c995"),
                    Stroke = (Brush)new BrushConverter().ConvertFrom("#81c995"),
                    StrokeThickness = 1,
                    Tag = new Tuple<DateTime, double>(sampledPrices[i].Item1, sampledPrices[i].Item2)
                };

                Canvas.SetLeft(dataPoint, x - 1);
                Canvas.SetTop(dataPoint, y - 1);

                dataPointPositions.Add(new Point(x, y));
                dataPoints.Add(dataPoint);

                chartCanvas.Children.Add(dataPoint);

                if(i > 0)
                {
                    var previousPrice = sampledPrices[i - 1].Item2;
                    var currentPrice = sampledPrices[i].Item2;

                    Brush lineColor = currentPrice > previousPrice ? (Brush)new BrushConverter().ConvertFrom("#81c995") : (Brush)new BrushConverter().ConvertFrom("#e74c3c");

                    Line segment = new Line
                    {
                        X1 = (i - 1) * stepX,
                        Y1 = canvasHeight - ((sampledPrices[i - 1].Item2 - minPrice) / priceRange * canvasHeight),
                        X2 = x,
                        Y2 = y,
                        Stroke = lineColor,
                        StrokeThickness = 2
                    };

                    chartCanvas.Children.Add(segment);
                }

                if (i == 0 || i == sampledPrices.Count - 1 || (i % dateInterval == 0 && i != 0))
                {
                    if (Math.Abs(x - previousX) < 40) continue;
                    previousX = x;
                    TextBlock dateLabel = new TextBlock
                    {
                        Text = sampledPrices[i].Item1.ToString("dd-MM-yyyy"),
                        Foreground = Brushes.LightGray,
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
        }
    }
}
