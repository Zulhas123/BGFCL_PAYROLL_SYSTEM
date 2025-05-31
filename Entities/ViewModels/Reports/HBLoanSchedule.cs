using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.Reports
{
    public class HBLoanSchedule
    {
        public string EmployeeName { get; set; }          
        public string JobCode { get; set; }              
        public DateTime LoanTakenDate { get; set; }       
        public decimal OpeningPrincipalBalance { get; set; } 
        public decimal ReScheduleLoanTakenThisYear { get; set; } 
        public decimal TotalBalancePrincipal { get; set; }    
        public decimal OpeningBalanceInterest { get; set; }   
        public decimal InterestChargeDuringYear { get; set; } 
        public decimal RecoveryPrincipal { get; set; }        
        public decimal RecoveryInterest { get; set; }         
        public decimal Total { get; set; }                    
        public decimal ClosingPrincipal { get; set; }         
        public decimal TotalLoanLiability { get; set; }       
    }

}
