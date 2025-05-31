using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class FinalAdjustmentJuniorStaff
    {
        public int Id { get; set; }
        public int? MonthId { get; set; }
        public string? JobCode { get; set; }
        public string? EmployeeName { get; set; }
        public string? DesignationName { get; set; }
        public string? DepartmentName { get; set; }
        public string? AccountNumber { get; set; }
        public string? BankName { get; set; }
        public string? BankBranchName { get; set; }
        public string? RoutingNumber { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal PersonalSalary { get; set; }
        public decimal ConvenienceAllow { get; set; }
        public decimal ArrearSalary { get; set; }
        public decimal WorkDays { get; set; }
        public decimal NumberOfShift { get; set; }
        public decimal OtherSalary { get; set; }
        public decimal SpecialBenefit { get; set; }
        public decimal LunchAllow { get; set; }
        public decimal TiffinAllow { get; set; }
        public decimal ShiftAllow { get; set; }
        public decimal HouseRentAllow { get; set; }
        public decimal FamilyMedicalAllow { get; set; }
        public decimal EducationAllowance { get; set; }
        public decimal FieldAllow { get; set; }
        public decimal OtSingle { get; set; }
        public decimal OtDouble { get; set; }
        public decimal OtAllow { get; set; }
        public decimal FuelAllow { get; set; }
        public decimal UtilityAllow { get; set; }
        public decimal OtherAllow { get; set; }
        public decimal RevenueStamp { get; set; }
        public decimal ProvidentFund { get; set; }
        public decimal UtilityReturn { get; set; }
        public decimal WelfareFund { get; set; }
        public decimal EmployeeClub { get; set; }
        public decimal EmployeeUnion { get; set; }
        public decimal Dormitory { get; set; }
        public decimal HospitalDeduction { get; set; }
        public decimal SpecialDeduction { get; set; }
        public decimal FuelReturn { get; set; }
        public decimal HBLoan { get; set; }
        public decimal MCylLoan { get; set; }
        public decimal BCylLoan { get; set; }
        public decimal ComputerLoan { get; set; }
        public decimal PFLoan { get; set; }
        public decimal WPFLoan { get; set; }
        public decimal CosLoan { get; set; }
        public decimal OtherLoan { get; set; }
        public decimal Advance { get; set; }
        public decimal Others { get; set; }
        public string? JournalNumber { get; set; }
        public string? TIN { get; set; }
        public decimal PensionCom { get; set; }
        public decimal NetPay { get; set; }
    }
}
