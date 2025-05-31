using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class SalarySettingsOfficer:Common
    {
        public string? JobCode { get; set; }
        public double BasicSalary { get; set; }
        public double PersonalPay { get; set; }
        public double LikeBasic { get; set; }
        public double OtherSalary { get; set; }
        public double EducationAllow { get; set; }
        public double WashAllow { get; set; }
        public double HouseRentAllowRate { get; set; }
        public double HouseRentAllow { get; set; }
        public double ConveyanceAllow { get; set; }
        public double FamilyMedicalAllow { get; set; }
        public double OfficerPF { get; set; }
        public double FieldRiskAllow { get; set; }
        public double ChargeAllow { get; set; }
        public double DAidAllow { get; set; }
        public double DeputationAllow { get; set; }
        public double HouseRentReturn { get; set; }
        public double DormitoryDeduction { get; set; }
        public double FuelReturn { get; set; }
        public bool IsMemberPF { get; set; }
        public bool IsMemberWF { get; set; }
        public bool IsMemberCOS { get; set; }
        public bool IsMemberMedical { get; set; }
        public bool IsMemberOffClub { get; set; }
        public bool IsMemberOffAsso { get; set; }
        public int PayModeId { get; set; }
        public int BankId { get; set; }
        public int BankBranchId { get; set; }
        public string? AccountNumber { get; set; }
        public double MonthlyTaxDeduction { get; set; }
        public double CME { get; set; }
    }
}
