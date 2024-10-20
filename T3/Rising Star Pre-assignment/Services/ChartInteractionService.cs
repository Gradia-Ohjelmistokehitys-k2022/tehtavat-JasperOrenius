using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Rising_Star_Pre_assignment.Services
{
    public class ChartInteractionService : IChartInteractionService
    {
        private Canvas chartCanvas;
        private Line inputLine;
        private List<Ellipse> dataPoints;
        private List<double> dataPointPositions;
        private int? currentDataPointIndex;

        public void Initialize(Canvas chartCanvas, Line inputLine, List<Ellipse> dataPoints, List<double> dataPointPositions)
        {
            this.chartCanvas = chartCanvas;
            this.inputLine = inputLine;
            this.dataPoints = dataPoints;
            this.dataPointPositions = dataPointPositions;
            currentDataPointIndex = null;
        }

        public void HandleMouseMove(Point mousePosition)
        {
            if (dataPoints == null || dataPointPositions == null || dataPoints.Count == 0) return;
            double closestDataPoint = dataPointPositions.MinBy(x => Math.Abs(x - mousePosition.X));
            int closestIndex = dataPointPositions.IndexOf(closestDataPoint);
            if(currentDataPointIndex != closestIndex)
            {
                currentDataPointIndex = closestIndex;
                UpdateToolTip(dataPoints[closestIndex]);
            }
            if(inputLine != null)
            {
                inputLine.X1 = mousePosition.X;
                inputLine.X2 = mousePosition.X;
                inputLine.Visibility = Visibility.Visible;
            }
        }
        
        public void HandleMouseEnter()
        {
            if(inputLine != null)
            {
                inputLine.Visibility = Visibility.Visible;
            }
        }

        public void HandleMouseLeave()
        {
            if(inputLine != null)
            {
                inputLine.Visibility = Visibility.Hidden;
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
