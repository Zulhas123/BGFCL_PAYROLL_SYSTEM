using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class SalarySettingsJuniorStaff:Common
    {
        public string? JobCode { get; set; }
        public double BasicSalary { get; set; }
        public double PersonalPay { get; set; }
        public double ConvenienceAllow { get; set; }
        public double OtherSalary { get; set; }
        public double HouseRentAllowRate { get; set; }
        public double HouseRentAllow { get; set; }
        public double FamilyMedicalAllow { get; set; }
        public double FuelReturn { get; set; }
        public double EducationAllow { get; set; }
        public double FieldAllow { get; set; }
        public double DormitoryDeduction { get; set; }
        public double ProvidentFund { get; set; }
        public double UtilityReturn { get; set; }
        public bool IsMemberPF { get; set; }
        public bool IsMemberWF { get; set; }
        public bool IsMemberCOS { get; set; }
        public bool IsMemberEmpClub { get; set; }
        public bool IsMemberEmpUnion { get; set; }
        public int PayModeId { get; set; }
        public int BankId { get; set; }
        public int BankBranchId { get; set; }
        public string? AccountNumber { get; set; }

        // new added for this project.
        public double ClassTeacherAllow { get; set; }
        public double ArrearAllow { get; set; }
        public double SpecialBenefits { get; set; }
        public double? RevenueStamp { get; set; }
        public double? per_attendence { get; set; }
        public double? Festival_bonus { get; set; }
        public double? eidul_ajha { get; set; }
        public double? eidul_fitar { get; set; }
        public double? baishakhi { get; set; }
        public bool? Is_Daily_Worker { get; set; }
    }
}
