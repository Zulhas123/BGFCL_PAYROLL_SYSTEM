using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Employee:Common
    {
        public int Id { get; set; }
        public string? JobCode { get; set; }

        public string? EmployeeName { get; set; }

        public string? FatherName { get; set; }

        public string? MotherName { get; set; }

        public string? DateOfBirth { get; set; }

        public int? SchoolId { get; set; }
        public int GenderId { get; set; }
        public int? GuestPkId { get; set; }
        public int? UuId { get; set; }
        public int? GuestUserId { get; set; }
        public int? UserId { get; set; }
        public int? UserTypeId { get; set; }
        public int? RoleId { get; set; }
        public int? ChildCount { get; set; }

        public int MaritalId { get; set; }

        public int ReligionId { get; set; }

        public int EmployeeTypeId { get; set; }

        public int GradeId { get; set; }

        public int DepartmentId { get; set; }

        public int DesignationId { get; set; }

        public int LocationId { get; set; }

        public string? JoiningDate { get; set; }

        public string? JournalCode { get; set; }

        public string? TinNo { get; set; }

        public string? MobileNumber { get; set; }

        public string? PresentAddress { get; set; }

        public string? PermanentAddress { get; set; }

        public string? Qualifications { get; set; }

        public string? IdentityMarks { get; set; }

        public string? Remarks { get; set; }

        public bool TaxStatus { get; set; }

        public int ActiveStatus { get; set; }
        public string? Nid { get; set; }
        public string? EmpSL { get; set; }

    }
}
