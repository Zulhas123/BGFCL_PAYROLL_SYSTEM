using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class CarInstallmentReport
    {
        public int? MonthId { get; set; }
        public decimal PrincipalAmount { get; set; }
        public decimal InterestAmount { get; set; }
        public decimal TotalPayment { get; set; }
        public decimal RemainingBalance { get; set; }
        public decimal DepreciationAmount { get; set; }
        public bool IsPaid { get; set; }
    }
   
}
