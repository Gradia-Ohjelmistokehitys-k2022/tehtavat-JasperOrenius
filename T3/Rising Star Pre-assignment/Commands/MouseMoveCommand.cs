using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Rising_Star_Pre_assignment.Controllers
{
    public class MouseMoveCommand : BaseCommand
    {
        private readonly Canvas chartCanvas;
        Line inputLine = new Line();
        private List<Ellipse> dataPoints = new List<Ellipse>();
        private List<double> dataPointPositions = new List<double>();
        private int? currentDataPointIndex = null;

        public MouseMoveCommand(Canvas chartCanvas, Line inputLine, List<Ellipse> dataPoints, List<double> dataPointPositions)
        {
            this.chartCanvas = chartCanvas;
            this.inputLine = inputLine;
            this.dataPoints = dataPoints;
            this.dataPointPositions = dataPointPositions;
        }

        public override void Execute(object? parameter)
        {
            if(parameter is MouseEventArgs e)
            {
                Point mousePosition = Mouse.GetPosition(chartCanvas);
                double[] dataPoints = dataPointPositions.ToArray();
                double closestDataPoint = dataPoints.MinBy(x => Math.Abs((long)x - mousePosition.X));
                int newIndex = dataPointPositions.IndexOf(closestDataPoint);
                if (currentDataPointIndex != newIndex)
                {
                    currentDataPointIndex = newIndex;
                    UpdateToolTip(this.dataPoints[newIndex], this.dataPoints);
                }
                inputLine.X1 = closestDataPoint;
                inputLine.X2 = closestDataPoint;
            }
        }

        private void UpdateToolTip(Ellipse dataPoint, List<Ellipse> dataPoints)
        {
            foreach (Ellipse point in dataPoints)
            {
                ToolTip toolTip = point.ToolTip as ToolTip;
                if (toolTip != null)
                {
                    toolTip.IsOpen = false;
                }
            }
            if (dataPoint.Tag is Tuple<DateTime, double> data)
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
    }
}
