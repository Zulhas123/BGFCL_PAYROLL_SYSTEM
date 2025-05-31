using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Roles
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? GuestPkId { get; set; }
        public int? SchoolId { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string? Notes { get; set; }
        public bool? IsEmployee { get; set; }
        public bool? IsAuthority { get; set; }
        public bool? IsStaff { get; set; }
        public bool? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

}
