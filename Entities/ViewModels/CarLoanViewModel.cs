using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels
{
    public class CarLoanViewModel
    {
        public int Sl { get; set; }
        public int Id { get; set; }
        public string JobCode { get; set; }
        public string EmployeeName { get; set; }
        public double TotalLoanAmount { get; set; }
        public double DepreciationAmount { get; set; }
        public double RemainingDepreciationAmount { get; set; }
        public double ActualAmount { get; set; }
        public double RemainingActualAmount { get; set; }
        public double InterestRate { get; set; }
        public string LoanTakenDate { get; set; }
        public double InstallmentNo { get; set; }
        public double RemainingInstallmentNo { get; set; }
    }
}
