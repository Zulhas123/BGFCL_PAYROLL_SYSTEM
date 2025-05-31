using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class MclInstallment
    {
        public int Id { get; set; }
        public string JobCode { get; set; }
        public double? InstallmentAmount { get; set; }
        public int MonthId { get; set; }
        public int MclId { get; set; }
    }
}
