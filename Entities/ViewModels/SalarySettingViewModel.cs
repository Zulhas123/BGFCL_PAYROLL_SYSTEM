using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels
{
    public class SalarySettingViewModel
    {
        public string JobCode { get; set; }
        public string EmployeeName { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
        public decimal GpfRate { get; set; }
    }
}
