using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class InactiveEmployeeOf : Common
    {
        public int Id { get; set; }
        public string? JobCode { get; set; }
        public string? EmployeeName { get; set; }
        public int ActiveStatus { get; set; }
    }
}
