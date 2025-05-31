using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.Reports
{
    public class LoanSheetViewModel
    {
        public string JobCode { get; set; }
        public string EmployeeName { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }
        public double HBLoan { get; set; }
        public double MCycleLoan { get; set; }
        public double BCycleLoan { get; set; }
        public double ComLoan { get; set; }
        public double PFLoan { get; set; }
        public double WPFLoan { get; set; }
        public double COSLoan { get; set; }
        public double CarLoan { get; set; }
        public double OtherLoan { get; set; }
    }
}
