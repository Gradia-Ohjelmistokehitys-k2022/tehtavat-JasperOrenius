using Rising_Star_Pre_assignment.Controllers;
using Rising_Star_Pre_assignment.Services;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Rising_Star_Pre_assignment.Commands
{
    class MouseEnterCommand : BaseCommand
    {
        private readonly ChartInteractionService chartInteractionService;

        public MouseEnterCommand(ChartInteractionService chartInteractionService)
        {
            this.chartInteractionService = chartInteractionService;
        }

        public override void Execute(object? parameter)
        {
            chartInteractionService.HandleMouseEnter();
        }
    }
}