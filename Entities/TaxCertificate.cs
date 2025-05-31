using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class TaxCertificate
    {
        public string JobCode { get; set; }
        public string EmployeeName { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }
        public string TIN { get; set; }
        public decimal IncomeTax { get; set; }
        public string IncomeTaxInWords { get; set; }
    }
}
