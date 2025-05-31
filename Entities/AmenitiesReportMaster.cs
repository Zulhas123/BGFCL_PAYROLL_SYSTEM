using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class AmenitiesReportMaster
    {
        public int Id { get; set; }
        public int MonthId { get; set; }
        public DateTime ProcessDate { get; set; }
        public int Status { get; set; }
    }
}
