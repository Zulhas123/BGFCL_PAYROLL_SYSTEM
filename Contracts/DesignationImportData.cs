using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public class DesignationImportData
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? SchoolId { get; set; }
        public int? RoleId { get; set; }
        public int? GuestPkId { get; set; }
        public string DesignationName { get; set; }
        public string Description { get; set; }
        public string? MultiDesignation { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? EmployeeTypeId { get; set; }
    }
}
