using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class BankPaymentSummary
    {
        public string BankName { get; set; }
        public string BankAddress { get; set; }
        public string BankBranch { get; set; }
        public decimal NetPay { get; set; }
    }
}
