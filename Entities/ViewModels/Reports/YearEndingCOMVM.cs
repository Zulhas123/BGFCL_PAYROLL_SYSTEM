using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.Reports
{
    public class YearEndingCOMVM
    {
        public string JobCode { get; set; }
        public string EmployeeName { get; set; }
        public string DesignationName { get; set; }
        public decimal Installment { get; set; }
        public decimal Interest { get; set; }
        public decimal MonthId { get; set; }
    }
}
