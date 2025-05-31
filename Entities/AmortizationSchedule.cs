using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class AmortizationSchedule
    {
        public int NoOfInterest { get; set; }
        public string MonthYear { get; set; }
        public decimal Principal { get; set; }
        public decimal Installment { get; set; }
        public decimal Interest { get; set; }
        public decimal NewPrincipal { get; set; }
        public decimal InterestInstallment { get; set; }
    }
}
