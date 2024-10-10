using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rising_Star_Pre_assignment.Models
{
    class MarketData
    {
        public List<List<double>> prices { get; set; }
        public List<List<double>> total_volumes { get; set; }
    }
}