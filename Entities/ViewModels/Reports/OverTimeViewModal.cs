using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.Reports
{
    public class OverTimeViewModal
    {
        public string? DepartmentName { get; set; }
        public string? JournalNumber { get; set; }
        public int NumberOfEmployees { get; set; }

        // Current month data
        public int CurrentMonthOtHours { get; set; }
        public decimal CurrentMonthOtAllow { get; set; }

        // July to current month cumulative data
        public int TotalOtHoursFromJuly { get; set; }
        public decimal TotalOtAllowFromJuly { get; set; }

    }
}
