namespace Rising_Star_Pre_assignment.ViewModels
{
    public class DataPointViewModel : BaseViewModel
    {
        public double X { get; set; }
        public double Y { get; set; }

        public DateTime Date { get; set; }
        public double Price { get; set; }

        private bool isToolTipVisible;
        public bool IsToolTipVisible
        {
            get => isToolTipVisible;
            set
            {
                isToolTipVisible = value;
                OnPropertyChanged(nameof(IsToolTipVisible));
            }
        }

        public string ToolTipText => $"{Date:dd-MM-yyyy}\nPrice : {Price:F2} €";
    }
}
