using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class SalaryProcessMaster
    {
        public int SalaryProcessingID { get; set; }
        public int? MonthID { get; set; }
        public int? EmployeeTypeID { get; set; }
        public string? ProcessingDate { get; set; }
        public int? Status { get; set; }
    }
}
