using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class BonusControlSheet
    {
        public int? BonusSheetId { get; set; }
        public int? SchoolId { get; set; }
        public int? RoleId { get; set; }
        public int? GuestPkId { get; set; }
        public int? UserId { get; set; }
        public int? BonusId { get; set; }
        public int? MonthId { get; set; }
        public string? JobCode { get; set; }
        public string? EmployeeName { get; set; }
        public int? EmployeeId { get; set; }
        public string? DesignationName { get; set; }
        public int? DesignationId { get; set; } = 0;
        public string? DepartmentName { get; set; }
        public int? DepartmentId { get; set; }
        public string? AccountNumber { get; set; }
        public int? BankId { get; set; }
        public string? BankName { get; set; }
        public string? BankBranchName { get; set; }
        public string? BankAddress { get; set; }

        public decimal? FestivalBonus { get; set; }
        public decimal? IncentiveBonus { get; set; }
        public decimal? HonorariumBonus { get; set; }
        public decimal? Scholarship { get; set; }
        public decimal? OtherDeduction { get; set; }
        public decimal? RevenueStamp { get; set; }
        public decimal? NetBonus { get; set; }
        public string? JournalCode { get; set; }
        public string? TIN { get; set; }
        public int? EmployeeTypeId { get; set; }
    }
}
