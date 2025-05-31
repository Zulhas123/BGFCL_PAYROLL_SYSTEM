using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class SalaryReportItem
    {
        public string JobCode { get; set; }
        public string EmployeeName { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string BankBranchName { get; set; }
        public decimal GrossPay { get; set; }
        public decimal TotalDeduction { get; set; }
        public decimal NetPay { get; set; }
    }
}
