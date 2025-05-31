using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.Reports
{
    public class BonusControlSheetViewModel
    {
        public int BonusSheetId { get; set; }
        public int BonusId { get; set; }
        public int MonthId { get; set; }
        public string JobCode { get; set; }
        public string EmployeeName { get; set; }
        public int EmployeeId { get; set; }
        public string DesignationName { get; set; }
        public int DesignationId { get; set; } = 0;
        public string DepartmentName { get; set; }
        public int DepartmentId { get; set; }
        public int BankId { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string? BankBranchName { get; set; }
        public string? BankAddress { get; set; }
        public double FestivalBonus { get; set; }
        public double BasicSalary { get; set; }
        public double IncentiveBonus { get; set; }
        public double HonorariumBonus { get; set; }
        public double ScholarshipBonus { get; set; }
        public double Deduction { get; set; }
        public double NetBonus { get; set; }
        public double RevStamp { get; set; }
        public string JournalCode { get; set; }
        public string TIN { get; set; }
    }
}
