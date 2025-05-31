using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class CarLoan:Common
    {
        public int Id { get; set; }
        public string JobCode { get; set; }
        public double TotalLoanAmount { get; set; }
        public double DepreciationAmount { get; set; }
        public double ActualAmount { get; set; }
        public double InterestRate { get; set; }
        public double RemainingDepreciationAmount { get; set; }
        public double RemainingActualAmount { get; set; }
        public int InstallmentNo { get; set; }
        public int RemainingInstallmentNo { get; set; }
        public DateTime LoanTakenDate { get; set; }
        public bool IsActive { get; set; }
        public int LoanTypeId { get; set; }
        public DateTime LastProcessingDate { get; set; }
        public DateTime? DeactivatedDate { get; set; }
        

        // only for display data and others
        public string LoanTakenDateString { get; set; }
        public string EmiStartMonthString { get; set; }
        public DateTime EmiStartMonth { get; set; }
    }
}
