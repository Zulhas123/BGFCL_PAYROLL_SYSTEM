using Dapper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.Reports
{
    public class AmenitiesControlSheetViewModel
    {
        public string JobCode { get; set; }
        public int? MonthID { get; set; }
        public string EmployeeName { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string BankBranchName { get; set; }
        public double WageGMSCB { get; set; }
        public double HouseKeepUp { get; set; }
        public double FuelSubsidy { get; set; }
        public double WESubsidy { get; set; }
        public double OtherDeduction { get; set; }
        public double OtherPay { get; set; }
        public double RevenueStamp { get; set; }
        public string JournalCode { get; set; }
        public string MonthYear
        {
            get
            {
                if (MonthID.HasValue)
                {
                    int year = (int)(MonthID.Value / 100);  // extract year
                    int month = (int)(MonthID.Value % 100); // extract month
                    var date = new DateTime(year, month, 1);
                    var dateString = date.ToString("MMM yyyy", CultureInfo.InvariantCulture);
                    return dateString;
                }
                return string.Empty; // Return empty string if MonthID is null
            }
        }
    }
}
