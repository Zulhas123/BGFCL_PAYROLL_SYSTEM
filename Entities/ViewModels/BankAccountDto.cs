using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels
{
    public class BankAccountDto
    {
        public int Id { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public decimal? OpeningBalance { get; set; }
        public string? Notes { get; set; }
        public string? BankName { get; set; }
        public string? BranchName { get; set; }
        public int? BankId { get; set; }
        public int? BranchId { get; set; }
    }

}
