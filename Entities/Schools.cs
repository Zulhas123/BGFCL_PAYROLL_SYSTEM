using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Schools:Common
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? GuestPkId { get; set; }
        public int? ShortId { get; set; }
        public string Title { get; set; }
        public string? Notes { get; set; }
        public bool? has_erp { get; set; }
        public bool? has_payroll { get; set; }
        public bool? IsActive { get; set; }

    }
}
