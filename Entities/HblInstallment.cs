using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class HblInstallment
    {
        public int Id { get; set; }
        public string? JobCode { get; set; }
        public double InstallmentAmount { get; set; }
        public double InterestAmount { get; set; }
        public int MonthId { get; set; }
        public int HbLoanId { get; set; }
        public string? InstallmentType { get; set; }
    }
}
