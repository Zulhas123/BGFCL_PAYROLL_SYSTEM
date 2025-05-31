using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class LoanProcessHistory
    {
        public int Id { get; set; }
        public int LoanId { get; set; }
        public int MonthId { get; set; }
    }
}
