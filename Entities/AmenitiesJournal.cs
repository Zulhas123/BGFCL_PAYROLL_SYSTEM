using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class AmenitiesJournal
    {
        public string AccountNumber { get; set; }
        public string JournalCode { get; set; }
        public string Details { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
        public int JournalMasterId { get; set; }
    }
}
