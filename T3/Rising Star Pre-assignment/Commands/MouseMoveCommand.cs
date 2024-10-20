using Rising_Star_Pre_assignment.Services;
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
        private readonly ChartInteractionService chartInteractionService;
        private readonly Canvas chartCanvas;

        public MouseMoveCommand(ChartInteractionService chartInteractionService, Canvas chartCanvas)
        {
            this.chartInteractionService = chartInteractionService;
            this.chartCanvas = chartCanvas;
        }

        public override void Execute(object? parameter)
        {
            if(parameter is MouseEventArgs e)
            {
                Point mousePosition = e.GetPosition(chartCanvas);
                chartInteractionService.HandleMouseMove(mousePosition);
            }
        }
    }
}
