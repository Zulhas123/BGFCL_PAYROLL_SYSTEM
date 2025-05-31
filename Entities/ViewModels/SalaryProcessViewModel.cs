using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels
{
    public class SalaryProcessViewModel
    {
        public int SalaryProcessingID { get; set; }
        public string Month { get; set; }
        public int Year { get; set; }
        public string EmployeeType { get; set; }
        public string Status { get; set; }
    }

}
