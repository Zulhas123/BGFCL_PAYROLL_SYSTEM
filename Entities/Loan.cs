using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Loan
    {
        public int Id { get; set; }
        public string LoanName { get; set; }
        public string LoanType { get; set; }
        public bool IsActive { get; set; }
    }
}
