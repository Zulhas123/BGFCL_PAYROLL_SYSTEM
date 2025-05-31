using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class EmployeeType : Common
    {
        public int Id { get; set; }
        public int? SchoolId { get; set; }
        public int? RoleId { get; set; }
        public int? GuestPkId { get; set; }
        public string EmployeeTypeName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
