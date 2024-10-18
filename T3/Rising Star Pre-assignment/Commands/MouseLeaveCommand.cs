using Rising_Star_Pre_assignment.Controllers;
using Rising_Star_Pre_assignment.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Rising_Star_Pre_assignment.Commands
{
    class MouseLeaveCommand : BaseCommand
    {
        private readonly ChartInteractionService chartInteractionService;

        public MouseLeaveCommand(ChartInteractionService chartInteractionService)
        {
            this.chartInteractionService = chartInteractionService;
        }

        public override void Execute(object? parameter)
        {
            chartInteractionService.HandleMouseLeave();
        }
    }
}
