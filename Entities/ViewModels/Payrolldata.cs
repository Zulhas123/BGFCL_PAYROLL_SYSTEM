using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels
{
    public class Payrolldata
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? GuestPkId { get; set; }
        public int? SchoolId { get; set; }
        public decimal GrossPay { get; set; }
        public decimal Deduction { get; set; }
        public decimal NetPay { get; set; }
    }
}
