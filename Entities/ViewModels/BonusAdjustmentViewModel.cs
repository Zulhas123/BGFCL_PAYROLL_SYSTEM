using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels
{
    public class BonusAdjustmentViewModel
    {
        public int BonusSheetId { get; set; }
        public string? JobCode { get; set; }
        public string? EmployeeName { get; set; }
        public string? DesignationName { get; set; }
        public string? DepartmentName { get; set; }
        public string? AccountNumber { get; set; }
        public string? BankName { get; set; }
        public string? BankBranchName { get; set; }
        public double FestivalBonus { get; set; }
        public double IncentiveBonus { get; set; }
        public double HonorariumBonus { get; set; }
        public double ScholarshipBonus { get; set; }
        public double Deduction { get; set; }
        public double RevStamp { get; set; }
        public string? JournalCode { get; set; }

    }
}
