using Rising_Star_Pre_assignment.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rising_Star_Pre_assignment.ViewModels
{
    public class MainViewModel
    {
        public BaseViewModel CurrentViewModel { get; }

        public MainViewModel()
        {
            ChartInteractionService chartInteractionService = new ChartInteractionService();
            CurrentViewModel = new DataViewModel(chartInteractionService);
        }
    }
}
