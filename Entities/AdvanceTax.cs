using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class AdvanceTax:Common
    {
        public int Id { get; set; }
        public string LetterNo { get; set; }
        public double Amount { get; set; }
        public string? Date { get; set; }
        public int MonthId { get; set; }
    }
}
