using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class BankAccounts:Common
    {
        public int Id { get; set; }
        public int? BankId { get; set; }
        public int? BranchId { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public string? AccountType { get; set; }
        public decimal? OpeningBalance { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public int? BankTypeId { get; set; }
    }
}
