using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Location:Common
    {
        public int Id { get; set; }
        public string? LocationName { get; set; }
        public bool IsActive { get; set; }
    }
}
