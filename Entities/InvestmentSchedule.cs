using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class InvestmentSchedule
    {
        public int Id { get; set; }
        public int? MonthId { get; set; }
        public string? JobCode { get; set; }
        public string? FdrAccountNo { get; set; }
        public string? BankName { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime? MaturityDate { get; set; }
        public int? PeriodOfInvestment { get; set; } 
        public decimal? InterestRate { get; set; }

        public decimal? OpeningPrincipal { get; set; }
        public decimal? OpeningInterest { get; set; }
        public decimal? OpeningTotal { get; set; }

        public decimal? AdditionPrincipal { get; set; }
        public decimal? InterestReceived { get; set; }
        public decimal? AccruedInterest { get; set; }
        public decimal? TotalInterest { get; set; }

        public decimal? RenewPrincipal { get; set; }
        public decimal? CapitalizedInterest { get; set; }
        public decimal? TdsExpense { get; set; }
        public decimal? ExciseDuty { get; set; }
        public decimal? RenewTotal { get; set; }

        public decimal? ClosingPrincipal { get; set; }
        public decimal? ClosingInterest { get; set; }
        public decimal? ClosingTotal { get; set; }
    }

}
