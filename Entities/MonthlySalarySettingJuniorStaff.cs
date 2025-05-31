using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class MonthlySalarySettingJuniorStaff:Common
    {
        public string JobCode { get; set; }
        public int MonthId { get; set; }
        public int WorkDays { get; set; }
        public int NumberOfShift { get; set; }
        public double ArrearSalary { get; set; }
        public double OtSingle { get; set; }
        public double OtDouble { get; set; }
        public double OtherAllow { get; set; }
        public double AdvanceDeduction { get; set; }
        public double OtherDeduction { get; set; }
        public double SpecialDeduction { get; set; }
        public double HospitalDeduction { get; set; }
    }
}
