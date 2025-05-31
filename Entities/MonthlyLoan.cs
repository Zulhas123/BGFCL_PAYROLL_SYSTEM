using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class MonthlyLoan
    {
        public string? JobCode { get; set; }
        public int MonthId { get; set; }
        public string? LoanName { get; set; }
        public double Amount { get; set; }
        public int EmployeeType { get; set; }
    }
}
