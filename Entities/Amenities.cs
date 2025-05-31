using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Amenities:Common
    {
        public int Id { get; set; }
        public string? JobCode { get; set; }
        public double WageGMSCB { get; set; }
        public double HouseKeepUp { get; set; }
        public double FuelSubsidy { get; set; }
        public double WESubsidy { get; set; }
        public double OtherDeduction { get; set; }
        public double OtherPay { get; set; }
        public int PayModeId { get; set; }
        public int? BankId { get; set; }
        public int? BankBranchId { get; set; }
        public string? AccountNumber { get; set; } = "0";
    }
}
