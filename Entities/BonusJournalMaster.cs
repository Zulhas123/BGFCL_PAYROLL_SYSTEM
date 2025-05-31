using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class BonusJournalMaster:Common
    {
        public int Id { get; set; }
        public int MonthId { get; set; }
        public int EmployeeTypeId { get; set; }
        public int BonusId { get; set; }
    }
}
