using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Basic:Common
    {
        public int Id { get; set; }
        public double BasicAmount { get; set; }
        public int GradeId { get; set; }
    }
}
