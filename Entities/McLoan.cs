using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class McLoan:Common
    {
        public int? Id { get; set; }
        public string? JobCode { get; set; }
        public double? TotalLoanAmount { get; set; }
        public double? RemainingLoanAmount { get; set; }
        public DateTime? LoanTakenDate { get; set; }
        public int? InstallmentNo { get; set; }
        public int? RemainingInstallmentNo { get; set; }
        public DateTime? LastProcessingDate { get; set; }
        public int? LoanTypeId { get; set; }
        public bool IsPaused { get; set; }
        public double? InstallmentAmount { get; set; }
        public bool IsActive { get; set; }
        public DateTime? DeactivatedDate { get; set; }

        // for display in the form
        public string? LoanTakenDateString { get; set; }

    }
}
