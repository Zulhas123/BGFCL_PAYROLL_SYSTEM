using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.Reports
{
    public class SummaryMotorCycleLoanViewModel
    {
        public string? EmployeeName { get; set; }
        public string? JobCode { get; set; }
        public string? LoanTakenDate { get; set; }
        public double? OpeningBalance { get; set; }
        public double? RecoveryAmount { get; set; }
        public double? ClosingBalance { get; set; }
    }
}
