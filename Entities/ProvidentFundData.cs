using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ProvidentFundData
    {
        public int Id { get; set; }
        public int? MonthId { get; set; }
        public string? Month { get; set; }
        public string? Year { get; set; }
        public string? JobCode { get; set; }
        public string? EmployeeName { get; set; }
        public string? DesignationName { get; set; }
        public decimal? BasicSalary { get; set; }
        public decimal? EmpOpeningContribution { get; set; }
        public decimal? EmpCurrentYearContribution { get; set; }
        public decimal? EmpEndingContribution { get; set; }
        public decimal? EmpSubTotal { get; set; }
        public decimal? CompanyOpeningContribution { get; set; }
        public decimal? CompanyCurrentYearContribution { get; set; }
        public decimal? CompanyEndingContribution { get; set; }
        public decimal? CompanySubTotal { get; set; }

        public decimal? InterestRate { get; set; }
        public decimal? InterestAsOpening { get; set; }
        public decimal? InterestAsEnding { get; set; }
        public decimal? InterestAsYear { get; set; }
        public decimal? TotalContribution { get; set; }
        public decimal? GrandTotal { get; set; }


    }
}
