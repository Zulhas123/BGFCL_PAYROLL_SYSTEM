using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class DepreciationInstallment
    {
        public int Id { get; set; }
        public int CarLoanId { get; set; }
        public int MonthId { get; set; }
        public double DepreciationAmount { get; set; }
        public bool IsPaid { get; set; }
    }
}
