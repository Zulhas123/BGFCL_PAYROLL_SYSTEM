using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class SmsService
    {
        public int? Id { get; set; }
        public string? JobCode { get; set; }
        public int? employeeTypeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? DesignationName { get; set; }
        public string? DepartmentName { get; set; }
        public string? MobileNumber { get; set; }

    }
}
