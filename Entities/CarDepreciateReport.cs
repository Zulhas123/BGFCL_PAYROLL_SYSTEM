using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class CarDepreciateReport
    {
        public int? MonthId { get; set; }
        public decimal DepreciationAmount { get; set; }
        public bool IsPaid { get; set; }
    }
}
