using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.Reports
{
    public class TaxRecordViewModel
    {
        public string? Month { get; set; }
        public decimal? IncomeTax { get; set; }
        public string? ChalanNo { get; set; }
        public DateTime? Date { get; set; }
    }
}
