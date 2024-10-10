using Rising_Star_Pre_assignment.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Rising_Star_Pre_assignment.ViewModels
{
    public class DataViewModel : BaseViewModel
    {
        public ICommand FetchData { get; }

        public ICommand MouseMove { get; }
        public ICommand MouseEnter { get; }
        public ICommand MouseLeave { get; }

        public DataViewModel()
        {
            FetchData = new FetchDataCommand();
        }
    }
}
