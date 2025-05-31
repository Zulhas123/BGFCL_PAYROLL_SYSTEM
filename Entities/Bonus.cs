using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Bonus:Common
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? BonusId { get; set; }
        public int? SchoolId { get; set; }
        public int? RoleId { get; set; }
        public int? employeeType { get; set; }
        public int? GuestPkId { get; set; }
        public string? BonusTitle { get; set; }
        public int PayableMonth { get; set; }
        public decimal TotalBonusAmount { get; set; }
        public int StatusOF { get; set; }
        public int StatusJS { get; set; }
        public bool IsActive { get; set; }
        public bool IsFestival { get; set; }
        public bool IsIncentive { get; set; }
        public bool IsHonorarium { get; set; }
        public bool IsScholarship { get; set; }
    }
}
