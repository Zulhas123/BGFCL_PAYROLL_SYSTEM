using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels.Reports
{
    public class OfficerAssoOrClub
    {
        public string JobCode { get; set; }
        public int? MonthID { get; set; }
        public string EmployeeName { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }
        public double OfficerAssociation { get; set; }
        public double OfficerClub { get; set; }
    }
}
