using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class BankTag:Common
    {
        public int Id { get; set; }
        public string? BankTagName { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
