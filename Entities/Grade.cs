using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Grade:Common
    {
        public int Id { get; set; }
        public string? GradeName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int EmployeeTypeId { get; set; }

    }
}
