using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class HblIndividual
    {
        public string JobCode { get; set; }
        public string DesignationName { get; set; }
        public string EmployeeName { get; set; }
        public string DepartmentName { get; set; }
        public int MonthId { get; set; }
        public double OpeningPrincipal { get; set; }
        public double OpeningInterest { get; set; }
        public double OpeningReSchedule { get; set; }
        public decimal RecoveryPrincipal { get; set; }
        public decimal RecoveryInterests { get; set; }
        public decimal RecoveryTotal { get; set; }
        public int InstallmentNoPrincipal { get; set; }
        public int InstallmentNoInterest { get; set; }
        public int RunningInstallment { get; set; }
        public double ClosiingPrincipal { get; set; }
        public double ClosingInterest { get; set; }
        public double TotalLoanLiability { get; set; }
    }
}
