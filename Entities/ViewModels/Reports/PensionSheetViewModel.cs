using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.Reports
{
    public class PensionSheetViewModel
    {
        public string JobCode { get; set; }
        public int? MonthID { get; set; }
        public string EmployeeName { get; set; }
        public string DesignationName { get; set; }
        public double CompanyContribution { get; set; }
        public string MonthYear
        {
            get
            {
                if (MonthID.HasValue)
                {
                    int year = MonthID.Value / 100;
                    int month = MonthID.Value % 100;
                    return $"{CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month)} '{year % 100:00}";
                }
                return "No Data"; // Or any default value you prefer
            }
        }


        public double PensionRate { get; set; }
        public decimal BasicSalary { get; set; }
    }
}
