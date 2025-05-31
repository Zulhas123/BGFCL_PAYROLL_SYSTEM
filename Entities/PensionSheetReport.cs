using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class PensionSheetReport
    {
        public int Id { get; set; }
        public int MonthId { get; set; }
        public string JobCode { get; set; }
        public string EmployeeName { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal PensionOfficer { get; set; }
        public decimal OwnContribution { get; set; }
        public decimal CompanyContribution { get; set; }
    }
}
