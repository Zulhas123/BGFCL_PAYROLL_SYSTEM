using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class MonthlySalarySettingOfficer:Common
    {
        // Jobcode and MonthId are composit primary key.
        public string JobCode { get; set; }
        public int MonthId { get; set; }
        public int WorkDays { get; set; }
        public double ArrearSalary { get; set; }
        public double AdvanceSalary { get; set; }
        public double OtherAllow { get; set; }
        public double TMBill { get; set; }
        public double HospitalBill { get; set; }
        public double SpecialDeduction { get; set; }
        public double AdvanceDeduction { get; set; }
        public double OtherDeduction { get; set; }
    }
}
