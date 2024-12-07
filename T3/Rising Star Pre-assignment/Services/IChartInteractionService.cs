using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Rising_Star_Pre_assignment.Services
{
    public interface IChartInteractionService
    {
        void Initialize(Canvas chartCanvas, List<Ellipse> dataPoints, List<Point> dataPointPositions);
        void HandleMouseMove(Point mousePosition);
        void HandleMouseEnter(Point mousePosition);
        void HandleMouseLeave();
        void UpdateToolTip(Ellipse dataPoint);
    }
}
