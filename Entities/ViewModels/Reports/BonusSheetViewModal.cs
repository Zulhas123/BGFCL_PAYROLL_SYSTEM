using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.Reports
{
    public class BonusSheetViewModal
    {
        public int Id { get; set; }
        public string? Month { get; set; }
        public int Year { get; set; }
        public int? PayableMonth { get; set; }
        public string? BonusTitle { get; set; }
        public string? Status { get; set; }
    }
}
