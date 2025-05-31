using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.Reports
{
    public class IndividualMotorCycleLoanReportViewModel
    {
        public string? JobCode { get; set; }
        public string? DesignationName { get; set; }
        public string? EmployeeName { get; set; }
        public string? DepartmentName { get; set; }
        public decimal LoanTakenAmount { get; set; }
        public DateTime LoanTakenDate { get; set; }
        public int? InstallmentNo { get; set; }
        public double? RunningInstallment { get; set; }
        public int MonthId { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal RecoveryDuringYear { get; set; }
        public double? RecoveryAmount { get; set; }
        public decimal ClosingBalance { get; set; }
    }
}
