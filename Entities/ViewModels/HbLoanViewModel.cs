using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels
{
    public class HbLoanViewModel
    {
        public int Sl { get; set; }
        public int Id { get; set; }
        public string JobCode { get; set; }
        public string EmployeeName { get; set; }
        public string TotalLoanAmount { get; set; }
        public double RemainingLoanAmount { get; set; }
        public string? LoanTakenDate { get; set; }
        public double PrincipalInstallmentAmount { get; set; }
        public double InstallmentNo { get; set; }
        public double RemainingInstallmentNo { get; set; }
        public double InterestRate { get; set; }
        public double TotalInterest { get; set; }
        public double RemainingInterest { get; set; }
        public double InterestInstallmentNo { get; set; }
        public double RemainingInterestInstallmentNo { get; set; }
        public double InterestInstallmentAmount { get; set; }

    }
}
