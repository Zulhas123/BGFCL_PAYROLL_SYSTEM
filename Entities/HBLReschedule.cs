using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class HBLReschedule
    {
        public double RemainingLoanAmount { get; set; }
        public double RescheduledLoanAmount { get; set; }
        public double TotalLoanAmount { get; set; }
        public double RemainingInterest { get; set; }
        public int InstallmentNo { get; set; }
        public double PrincipalInstallmentAmount { get; set; }
        public int InterestInstallmentNo { get; set; }
        public DateTime? LoanTakenDate { get; set; }
        public DateTime? LastProcessingDate { get; set; }
        public double InterestRate { get; set; }
        public int HBLoanId { get; set; }


        // for display data
        public string? LoanTakenDateString { get; set; }

    }
}
