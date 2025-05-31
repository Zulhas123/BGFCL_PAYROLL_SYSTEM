using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels
{
    public class BasicViewModel
    {
        public int Id { get; set; }
        public string? GradeName { get; set; }
        public int? GradeId { get; set; }
        public decimal? BasicAmount { get; set; }
        public int? EmployeeTypeId { get; set; }
        public bool IsActive { get; set; }
    }
}
