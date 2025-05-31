using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class AttendanceRecord:Common
    {

        public int Id { get; set; }
        public int MonthId { get; set; }
        public int AttendenceMasterId { get; set; }
        public int EmployeeId { get; set; }
        public string JobCode { get; set; }
        public string EmployeeName { get; set; }  
        public DateTime AttendanceDate { get; set; }
        public int DayCount { get; set; }
        public bool IsPresent { get; set; } = true;
    }
}
