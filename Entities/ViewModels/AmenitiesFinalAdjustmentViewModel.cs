using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels
{
    public class AmenitiesFinalAdjustmentViewModel
    {
        public int AmenitiesReportId { get; set; }
        public string? JobCode { get; set; }
        public string? EmployeeName { get; set; }
        public string? DesignationName { get; set; }
        public string? DepartmentName { get; set; }
        public string? AccountNumber { get; set; }
        public string? BankName { get; set; }
        public string? BankBranchName { get; set; }
        public string? RoutingNumber { get; set; }
        public double WageGMSCB { get; set; }
        public double HouseKeepUp { get; set; }
        public double FuelSubsidy { get; set; }
        public double WESubsidy { get; set; }
        public double OtherDeduction { get; set; }
        public double OtherPay { get; set; }
        public double RevenueStamp { get; set; }
        public string? JournalCode { get; set; }
    }
}
