using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.Reports
{
    public class PFSheetViewModel
    {
        public string JobCode { get; set; }
        public int? MonthID { get; set; }
        public string EmployeeName { get; set; }
        public string DesignationName { get; set; }
        public double BasicSalary { get; set; }
        public double OwnContribution { get; set; }
        public double CompanyContribution { get; set; }
        public double NetPay { get; set; }
        //public string MonthYear
        //{
        //    get
        //    {
        //        int year = (int)(MonthID / 100);
        //        int month = (int)(MonthID % 100);
        //        return $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)}-{year}";
        //    }
        //}
    }
}
