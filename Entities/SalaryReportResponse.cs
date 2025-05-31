using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class SalaryReportResponse
    {
        public List<SalaryReportItem> IndividualSalaries { get; set; } = new List<SalaryReportItem>();
        public decimal GrandGrossPay { get; set; }
        public decimal GrandTotalDeduction { get; set; }
        public decimal GrandNetPay { get; set; }
        public int TotalCount { get; set; }
    }
}
