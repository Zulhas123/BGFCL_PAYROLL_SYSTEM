using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.Reports
{
    public class YearEndingMCLVM
    {
        public string JobCode { get; set; }
        public string EmployeeName { get; set; }
        public string DesignationName { get; set; }
        public decimal Total { get; set; }
    }
}
