using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Rising_Star_Pre_assignment.Services
{
    public class ChartInteractionService : IChartInteractionService
    {
        private Canvas chartCanvas;
        private Line horizontalInputLine = new Line();
        private Line verticalInputLine = new Line();
        private List<Ellipse> dataPoints;
        private List<Point> dataPointPositions;
        private int? currentDataPointIndex;

        private static Line CreateLine(double x1, double y1, double x2, double y2)
        {
            return new Line
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = Brushes.White,
                StrokeDashArray = [10, 10],
                StrokeThickness = 0.75
            };
        }

        public void Initialize(Canvas chartCanvas, List<Ellipse> dataPoints, List<Point> dataPointPositions)
        {
            this.chartCanvas = chartCanvas;
            this.dataPoints = dataPoints;
            this.dataPointPositions = dataPointPositions;
            currentDataPointIndex = null;
        }

        public void HandleMouseMove(Point mousePosition)
        {
            if (dataPoints == null || dataPointPositions == null || dataPoints.Count == 0) return;
            Point closestDataPoint = dataPointPositions.OrderBy(point => Math.Abs(point.X - mousePosition.X)).FirstOrDefault();
            int closestIndex = dataPointPositions.IndexOf(closestDataPoint);
            if(currentDataPointIndex != closestIndex)
            {
                currentDataPointIndex = closestIndex;
                UpdateToolTip(dataPoints[closestIndex]);
            }
            if(horizontalInputLine != null && verticalInputLine != null)
            {
                horizontalInputLine.Y1 = closestDataPoint.Y;
                horizontalInputLine.Y2 = closestDataPoint.Y;
                verticalInputLine.X1 = closestDataPoint.X;
                verticalInputLine.X2 = closestDataPoint.X;
                horizontalInputLine.Visibility = Visibility.Visible;
                verticalInputLine.Visibility = Visibility.Visible;
            }
        }
        
        public void HandleMouseEnter()
        {
            if(dataPointPositions == null || !dataPointPositions.Any()) return;
            double canvasWidth = chartCanvas.ActualWidth;
            double canvasHeight = chartCanvas.ActualHeight;
            horizontalInputLine = CreateLine(0, 0, canvasWidth, 0);
            verticalInputLine = CreateLine(0, 0, 0, canvasHeight);
            chartCanvas.Children.Add(horizontalInputLine);
            chartCanvas.Children.Add(verticalInputLine);
        }

        public void HandleMouseLeave()
        {
            if(chartCanvas != null && horizontalInputLine != null && verticalInputLine != null)
            {
                chartCanvas.Children.Remove(horizontalInputLine);
                chartCanvas.Children.Remove(verticalInputLine);
            }
            HideAllToolTips();
        }

        public void UpdateToolTip(Ellipse dataPoint)
        {
            HideAllToolTips();
            if(dataPoint.Tag is Tuple<DateTime, double> data)
            {
                string toolTipText = $"{data.Item1:dd-MM-yyyy HH:mm}\nPrice : {data.Item2:F2} €";
                ToolTip toolTip = new ToolTip
                {
                    Content = toolTipText,
                    Placement = System.Windows.Controls.Primitives.PlacementMode.Relative,
                    HorizontalOffset = 10,
                    VerticalOffset = -10
                };
                dataPoint.ToolTip = toolTip;
                Point dataPointPosition = new Point(Canvas.GetLeft(dataPoint) + dataPoint.Width / 2, Canvas.GetTop(dataPoint) + dataPoint.Height / 2);
                toolTip.PlacementTarget = chartCanvas;
                toolTip.HorizontalOffset = dataPointPosition.X + 10;
                toolTip.VerticalOffset = dataPointPosition.Y - 10;
                toolTip.IsOpen = true;
            }
        }

        public void HideAllToolTips()
        {
            if (dataPoints == null || dataPoints.Count == 0) return;
            foreach(var point in dataPoints)
            {
                if(point.ToolTip is ToolTip toolTip)
                {
                    toolTip.IsOpen = false;
                }
            }
        }
    }
}
