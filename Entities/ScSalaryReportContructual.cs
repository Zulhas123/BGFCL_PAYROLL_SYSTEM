using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ScSalaryReportContructual
    {
        public string JobCode { get; set; }
        public string EmployeeName { get; set; }
        public string DesignationName { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string JoiningDate { get; set; }
        public string PayScale { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal HouseRentAllow { get; set; }
        public decimal FamilyMedicalAllow { get; set; }
        public decimal ConvenienceAllow { get; set; }
        public decimal ElectricityAllow { get; set; }
        public decimal GasAllow { get; set; }
        public decimal CtAllow { get; set; }
        public decimal AcAllow { get; set; } = 0; // Default value
        public decimal ArrearAllow { get; set; }
        public decimal PF { get; set; }
        public decimal RevenueStamp { get; set; }
        public decimal OtherDeduction { get; set; }
        public decimal SpecialBenefit { get; set; }
        public decimal TotalDeduction { get; set; } = 0; // Default value
        public decimal GrossPay { get; set; } = 0; // Default value
        public decimal NetPay { get; set; } = 0; // Default value
        public decimal InstituteLib { get; set; } = 0; // Default value
        public string FormattedJoiningDate
        {
            get
            {
                if (DateTime.TryParse(JoiningDate, out var parsedDate))
                {
                    return parsedDate.ToString("dd MMM yyyy");
                }
                return JoiningDate ?? string.Empty; // Return the original string if parsing fails
            }
        }
    }
}
