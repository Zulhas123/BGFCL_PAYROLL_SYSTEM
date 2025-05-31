using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ScSalaryReportUnified
    {
        public string JobCode { get; set; }
        public string EmployeeName { get; set; }
        public string DesignationName { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string JoiningDate { get; set; }
        public string PayScale { get; set; } // For Permanent (GradeName), '0.00' for Contractual
        public decimal BasicSalary { get; set; }
        public decimal OtherSalary { get; set; } // Only for Permanent
        public decimal HouseRentAllow { get; set; }
        public decimal FMAllow { get; set; } // Permanent: FMAllow, Contractual: FamilyMedicalAllow
        public decimal Conveyance { get; set; } // Permanent: Conveyance, Contractual: ConvenienceAllow
        public decimal ElectricityAllow { get; set; } // PersonalSalary
        public decimal GasAllow { get; set; } // LikeBasic / UtilityReturn
        public decimal CtAllow { get; set; } // DAidAllow / otherAllow
        public decimal AcAllow { get; set; } // Always '0.00'
        public decimal ArrearAllow { get; set; } // DeputationAllow / ArrearSalary
        public decimal PF { get; set; } // ProvidentFund
        public decimal RevenueStamp { get; set; }
        public decimal OtherDeduction { get; set; } // Dormitory
        public decimal SpecialBenefit { get; set; }
        public decimal TotalDeduction { get; set; } // 0
        public decimal GrossPay { get; set; } // 0
        public decimal NetPay { get; set; } // 0
        public decimal InstituteLib { get; set; } // 0
        public string EmpSl { get; set; }
    }

}
