using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Amenities1
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

        // *******//
        public decimal? Mali { get; set; }
        public decimal? HouseUpKeep { get; set; }
        public decimal? WaterElecticitySubsidy { get; set; }
        public decimal? OtherPay { get; set; }
        public decimal? OtherDeduction { get; set; }
        public decimal? RevenueStamp { get; set; }
        public decimal? FuelSubsidy { get; set; }
        public string? JournalCode { get; set; }


    }
}
