using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class SalaryReportOfficer
    {
        public int Id { get; set; }
        public int? MonthId { get; set; }
        public int? SchoolId { get; set; }
        public int? UuId { get; set; }
        public int? RoleId { get; set; }
        public string JobCode { get; set; }
        public string EmployeeName { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string BankBranchName { get; set; }
        public decimal? BasicSalary { get; set; }
        public decimal? PersonalSalary { get; set; }
        public decimal? ArrearSalary { get; set; }
        public decimal? WorkDays { get; set; }
        public decimal? LikeBasic { get; set; }
        public decimal? OtherSalary { get; set; }
        public decimal? LunchAllow { get; set; }
        public decimal? TiffinAllow { get; set; }
        public decimal? WashAllow { get; set; }
        public decimal? HouseRentAllow { get; set; }
        public decimal? Conveyance { get; set; }
        public decimal? FMAllow { get; set; }
        public decimal? EducationalAllow { get; set; }
        public decimal? FieldRiskAllow { get; set; }
        public decimal? ChargeAllow { get; set; }
        public decimal? DAidAllow { get; set; }
        public decimal? DeputationAllow { get; set; }
        public decimal? OtherAllow { get; set; }
        public decimal? RevenueStamp { get; set; }
        public decimal? ProvidentFund { get; set; }
        public decimal? PensionOfficer { get; set; }
        public decimal? WelfareFund { get; set; }
        public decimal? OfficerClub { get; set; }
        public decimal? OfficerAssociation { get; set; }
        public decimal? MedicalFund { get; set; }
        public decimal? TMBill { get; set; }
        public decimal? Dormitory { get; set; }
        public decimal? Hospitalisation { get; set; }
        public decimal? HouseRentReturn { get; set; }
        public decimal? SpecialDeduction { get; set; }
        public decimal? FuelReturn { get; set; }
        public decimal? HBLoan { get; set; }
        public decimal? MCylLoan { get; set; }
        public decimal? BCylLoan { get; set; }
        public decimal? ComLoan { get; set; }
        public decimal? CarLoan { get; set; }
        public decimal? PFLoan { get; set; }
        public decimal? WPFLoan { get; set; }
        public decimal? CosLoan { get; set; }
        public decimal? OtherLoan { get; set; }
        public decimal? Advance { get; set; }
        public decimal? Other { get; set; }
        public string JournalNumber { get; set; }
        public string? TIN { get; set; }
        public decimal? IncomeTax { get; set; }
        public decimal SpecialBenefit { get; set; }
        public decimal CME { get; set; }


        //Others fields
        public decimal? GrossPay { get;set; }
        public decimal? TotalDeduction { get; set;}
        public decimal? NetPay { get; set; }
        public decimal? Pension { get; set; }
        public decimal? TotalCamp { get; set;}
        public string? DistACNo { get; set; }
        public string? TaxIdNo { get;set; }
        //public string MonthYear
        //{
        //    get
        //    {
        //        int year = (int)(MonthID / 100);
        //        int month = (int)(MonthID % 100);
        //        return $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)}-{year}";
        //    }
        //}
        public decimal? CPF { get; set; }
        public decimal? GPF { get; set; }

        public decimal? PensionFromCompany { get; set; }
        public decimal? PfFromCompany { get; set; }
        public decimal? TotalCompanyLiabilities { get; set; }

    }

}
