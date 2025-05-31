using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.Reports
{
    public class IncomeTaxMonthlyViewModel
    {
        public int Id { get; set; }
        public int MonthId { get; set; }
        public string JobCode { get; set; }
        public string EmployeeName { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }
        public string TIN { get; set; }
        public double IncomeTax { get; set; }
    }
}
