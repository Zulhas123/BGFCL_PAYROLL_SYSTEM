using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class HbLoan:Common
    {
        public int Id { get; set; }
        public string? JobCode { get; set; }
        public DateTime? LoanTakenDate { get; set; }
        public double TotalLoanAmount { get; set; }
        public double RemainingLoanAmount { get; set; }
        public double InterestRate { get; set; }
        public double TotalInterest { get; set; }
        public double RemainingInterest { get; set; }
        public int InstallmentNo { get; set; }
        public int RemainingInstallmentNo { get; set; }
        public int InterestInstallmentNo { get; set; }
        public int RemainingInterestInstallmentNo { get; set; }
        public double PrincipalInstallmentAmount { get; set; }
        public double InterestInstallmentAmount { get; set; }
        public bool IsActive { get; set; }
        public bool IsPaused { get; set; }
        public DateTime? DeactivatedDate { get; set; }
        public DateTime? LastProcessingDate { get; set; }
        public int LoanTypeId { get; set; }
        public DateTime? DateAfterPeriod { get; set; }
        public bool IsRescheduled { get; set; }

        // for display data
        public string? LoanTakenDateString { get; set; }

    }
}
