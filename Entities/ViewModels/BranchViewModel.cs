using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels
{
	public class BranchViewModel
	{
		public int Id { get; set; }
		public string? BranchName { get; set; }
		public int BankId { get; set; }
		public string? BankName { get; set; }
		public string? RoutingNumber { get; set; }
        public string? Address { get; set; }
    }
}
