using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
	public class Branch:Common
	{
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? SchoolId { get; set; }
        public int? RoleId { get; set; }
        public int? GuestPkId { get; set; }
        public string? BranchName { get; set; }
        public int BankId { get; set; }
        public string? RoutingNumber { get; set; }
        public bool IsActive { get; set; }
        public string? Address { get; set; }
    }
}
