using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.Reports
{
    public class ComLoanInfoVM
    {
        public string JobCode { get; set; }
        public string DesignationName { get; set; }
        public string EmployeeName { get; set; }
        public string DepartmentName { get; set; }
        public string LoanTakenDate { get; set; }
        public int InstallmentNo { get; set; }

        public int MonthId { get; set; }
        public double OpeningBalance { get; set; }
        public decimal LoanTakenAmount { get; set; }
        public int RunningInstallment { get; set; }
        public decimal RecoveryAmounts { get; set; }
        public decimal RecoveryInterests { get; set; }
        public decimal Principal { get; set; }
        public decimal Interest { get; set; }
        public double ClosingBalance { get; set; }
        public decimal Total { get; set; }
    }

}
