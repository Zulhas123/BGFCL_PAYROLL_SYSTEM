using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.Reports
{
    public class PFDeductionViewModel
    {
        public string JobCode { get; set; }
        public int MonthId { get; set; }
        public string EmployeeName { get; set; }
        public string DesignationName { get; set; }
        public double OwnContribution { get; set; }
        public decimal GpfRate { get; set; }
        public decimal BasicSalary { get; set; }
        public string? MonthYear
        {
            get
            {
                if (MonthId <= 0)
                {
                    return null; 
                }

                int year = (int)(MonthId / 100);
                int month = (int)(MonthId % 100);
                if (month < 1 || month > 12)
                {
                    return null;
                }

                return $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)}-{year}";
            }
        }


    }
}
