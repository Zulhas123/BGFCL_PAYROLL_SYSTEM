using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class SalarySettingConfigDetails:Common
    {
        public int Id { get; set; }
        public string Field { get; set; }
        public string InputType { get; set; }
        public string Value { get; set; }
        public int EmployeeTypeId { get; set; }
    }
}
