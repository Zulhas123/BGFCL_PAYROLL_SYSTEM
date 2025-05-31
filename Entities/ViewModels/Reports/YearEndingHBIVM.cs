using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.Reports
{
    public class YearEndingHBIVM
    {
        public string JobCode { get; set; }
        public DateTime LoanTakenDate { get; set; }
        public decimal OpeningPrincipal { get; set; }
        public decimal OpeningInterest { get; set; }
        public decimal AdditionPrincipal { get; set; }
        public decimal AdditionInterest { get; set; }
        public decimal RecoveryPrincipal { get; set; }
        public decimal RecoveryInterest { get; set; }
        public decimal ClosingPrincipal { get; set; }
        public decimal ClosingInterest { get; set; }
        public decimal TotalLoanAmount { get; set; }

    }
}
