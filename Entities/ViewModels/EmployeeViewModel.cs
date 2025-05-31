using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.ViewModels
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }
        public string JobCode { get; set; }

        public string EmployeeName { get; set; }

        public string FatherName { get; set; }

        public string MotherName { get; set; }

        public string DateOfBirth { get; set; }

        public string GenderName { get; set; }

        public string MaritalName { get; set; }

        public string ReligionName { get; set; }
        public int EmployeeTypeId { get; set; }

        public string EmployeeTypeName { get; set; }

        public string GradeName { get; set; }

        public string DepartmentName { get; set; }

        public string DesignationName { get; set; }

        public string LocationName { get; set; }

        public string JoiningDate { get; set; }

        public string JournalCode { get; set; }

        public string TinNo { get; set; }

        public string MobileNumber { get; set; }

        public string PresentAddress { get; set; }

        public string PermanentAddress { get; set; }

        public string Qualifications { get; set; }

        public string IdentityMarks { get; set; }

        public string Remarks { get; set; }

        public string TaxStatus { get; set; }
        public int ActiveStatus { get; set; }
        public string Nid { get; set; }
    }
}
