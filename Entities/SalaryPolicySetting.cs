using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class SalaryPolicySetting:Common
    {
        public int Id { get; set; }
        public int EmployeeTypeId { get; set; }
        public double FuelAllow { get; set; }
        public double SpecialBenefit { get; set; }

        public double LunchRate { get; set; }

        public double TiffinRate { get; set; }

        public double OfficerClub { get; set; }
        public double OfficerAssociation { get; set; }

        public double ShiftAllowRate { get; set; }

        public double OtSingle { get; set; }

        public double OtDouble { get; set; }

        public double EmployeeClub { get; set; }

        public double UtilityAllow { get; set; }

        public double EmployeeUnion { get; set; }

        public double WelfareFund { get; set; }

        public double ProvidentFundRate { get; set; }

        public double RevenueStamp { get; set; }

        public double PensionRate { get; set; }
        public double Medical { get; set; }
    }
}
