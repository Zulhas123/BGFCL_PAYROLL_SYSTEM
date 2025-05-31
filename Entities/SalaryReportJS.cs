using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class SalaryReportJS
    {
        public int Id { get; set; }
        public int MonthId { get; set; }
        public string JobCode { get; set; }
        public string EmployeeName { get; set; }
        public int EmployeeId { get; set; }
        public string DesignationName { get; set; }
        public int DesignationId { get; set; } = 0;
        public string DepartmentName { get; set; }
        public int DepartmentId { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string? BankBranchName { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal LikeBasic { get; set; }
        public decimal PersonalSalary { get; set; }
        public decimal ArrearSalary { get; set; }
        public decimal OtherSalary { get; set; }
        public int WorkDays { get; set; }
        public int NumberOfShift { get; set; }

        // Allowance
        public decimal ConvenienceAllow { get; set; }
        public decimal LunchAllow { get; set; }
        public decimal TiffinAllow { get; set; }
        public decimal ShiftAllow { get; set; }
        public decimal OtherAllow { get; set; }
        public decimal HouseRentAllow { get; set; }
        public decimal FamilyMedicalAllow { get; set; }
        public decimal EducationAllowance { get; set; }
        public decimal FieldAllow { get; set; }
        public decimal OtSingle { get; set; }
        public decimal OtDouble { get; set; }
        public decimal OtAllow { get; set; }
        public decimal FuelAllow { get; set; }
        public decimal UtilityAllow { get; set; }
        public decimal OtherrAllow { get; set; }
        public decimal SpecialBenefit { get; set; }


        //Deduction
        public decimal RevenueStamp { get; set; }
        public decimal ProvidentFund { get; set; }
        public decimal WelfareFund { get; set; }
        public decimal EmployeeClub { get; set; }
        public double EmployeeUnion { get; set; }
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
        public decimal UtilityReturn { get; set; }
        
        //Others fields
        public decimal? GrossPay { get; set; }
        public decimal? TotalDeduction { get; set; }
        public decimal? NetPay { get; set; }
        public decimal JournalNumber { get; set; }
        public string? TIN { get; set; }
        public decimal PensionCom { get; set; }
        public decimal? TotalCamp { get; set; }
        public string? DistACNo { get; set; }
        public string? TaxIdNo { get; set; }
        public decimal? CPF { get; set; }
        public decimal? GPF { get; set; }
        public decimal? PensionFromCompany { get; set; }
        public decimal? PfFromCompany { get; set; }
        public decimal? TotalCompanyLiabilities { get; set; }
        //public string MonthYear
        //{
        //    get
        //    {
        //        int year = (int)(MonthId / 100);
        //        int month = (int)(MonthId % 100);
        //        return $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)}-{year}";
        //    }
        //}
    }
}
