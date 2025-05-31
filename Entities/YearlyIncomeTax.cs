using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class YearlyIncomeTax
    {
        public int Id { get; set; }
        public int MonthId { get; set; }
        public string JobCode { get; set; }
        public string EmployeeName { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }
        public string TIN { get; set; }
        public decimal? IncomeTax { get; set; }
        public string MonthYear
        {
            get
            {
                int year = (int)(MonthId / 100);
                int month = (int)(MonthId % 100);
                return $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)}-{year}";
            }
        }
    }
}
