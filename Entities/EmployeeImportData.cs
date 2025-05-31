using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class EmployeeImportData
    {
        public int Id { get; set; }
        public int? SchoolId { get; set; }
        public int? GuestPkId { get; set; }
        public int? UuId { get; set; }
        public int? GuestUserId { get; set; }
        public int? UserId { get; set; }
        public int? UserTypeId { get; set; }
        public int? RoleId { get; set; }
        public int? ChildCount { get; set; }
        public string JobCode { get; set; }
        public string EmployeeName { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int GenderId { get; set; }
        public int MaritalId { get; set; }
        public int ReligionId { get; set; }
        public int EmployeeTypeId { get; set; }
        public int GradeId { get; set; }
        public int DepartmentId { get; set; }
        public int DesignationId { get; set; }
        public int LocationId { get; set; }
        public DateTime JoiningDate { get; set; }
        public string JournalCode { get; set; }
        public string TinNo { get; set; }
        public string MobileNumber { get; set; }
        public string PresentAddress { get; set; }
        public string PermanentAddress { get; set; }
        public string Qualifications { get; set; }
        public string IdentityMarks { get; set; }
        public string Remarks { get; set; }
        public bool? TaxStatus { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? ActiveStatus { get; set; }
        public string? EmpSL { get; set; }
        public string? Nid { get; set; }
    }
}
