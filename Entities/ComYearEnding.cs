using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ComYearEnding
    {
        public int Id { get; set; }
        public int LoanId { get; set; }
        public double ClosingPrincipal { get; set; }
        public int MonthId { get; set; }
    }
}
