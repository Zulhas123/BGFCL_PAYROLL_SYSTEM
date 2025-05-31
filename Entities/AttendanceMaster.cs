using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class AttendanceMaster:Common
    {
        
        public int   Id { get; set; }
        public int? MonthId { get; set; }
        public int? TotalDay { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Boolean IsActive { get; set; }=true;

    }
}
