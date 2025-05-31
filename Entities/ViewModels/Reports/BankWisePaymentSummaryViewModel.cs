using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.Reports
{
    public class BankWisePaymentSummaryViewModel
    {
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public double NetPay { get; set; }
    }
}
