using Contracts;
using Dapper;
using Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class DataImportRepository: IDataImportContract
    {
        private readonly BgfclContext _context;

        public DataImportRepository(BgfclContext context)
        {
            _context = context;
        }
        public async Task<int> ImportEmployeeData(EmployeeImportData employeeData)
        {
            int result = 0;
            var parameters = new DynamicParameters();
            string query = @"
                            INSERT INTO Employees 
                            (SchoolId, GuestPkId, UuId, GuestUserId, UserId, UserTypeId, RoleId, ChildCount, JobCode, EmployeeName, FatherName, MotherName, 
                             DateOfBirth, GenderId, MaritalId, ReligionId, EmployeeTypeId, GradeId, DepartmentId, DesignationId, LocationId, JoiningDate, 
                             JournalCode, TinNo, MobileNumber, PresentAddress, PermanentAddress, Qualifications, IdentityMarks, Remarks, TaxStatus, 
                             CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, ActiveStatus, EmpSL, Nid) 
                            VALUES 
                            (@SchoolId, @GuestPkId, @UuId, @GuestUserId, @UserId, @UserTypeId, @RoleId, @ChildCount, @JobCode, @EmployeeName, @FatherName, @MotherName, 
                             @DateOfBirth, @GenderId, @MaritalId, @ReligionId, @EmployeeTypeId, @GradeId, @DepartmentId, @DesignationId, @LocationId, @JoiningDate, 
                             @JournalCode, @TinNo, @MobileNumber, @PresentAddress, @PermanentAddress, @Qualifications, @IdentityMarks, @Remarks, @TaxStatus, 
                             @CreatedBy, @CreatedDate, @UpdatedBy, @UpdatedDate, @ActiveStatus, @EmpSL, @Nid);
                            SELECT CAST(SCOPE_IDENTITY() as int);
                            ";

            parameters.Add("@SchoolId", employeeData.SchoolId, DbType.Int32);
            parameters.Add("@GuestPkId", employeeData.GuestPkId, DbType.Int32);
            parameters.Add("@UuId", employeeData.UuId, DbType.Int32);
            parameters.Add("@GuestUserId", employeeData.GuestUserId, DbType.Int32);
            parameters.Add("@UserId", employeeData.UserId, DbType.Int32);
            parameters.Add("@UserTypeId", employeeData.UserTypeId, DbType.Int32);
            parameters.Add("@RoleId", employeeData.RoleId, DbType.Int32);
            parameters.Add("@ChildCount", employeeData.ChildCount, DbType.Int32);
            parameters.Add("@JobCode", employeeData.JobCode, DbType.String);
            parameters.Add("@EmployeeName", employeeData.EmployeeName, DbType.String);
            parameters.Add("@FatherName", employeeData.FatherName, DbType.String);
            parameters.Add("@MotherName", employeeData.MotherName, DbType.String);
            parameters.Add("@DateOfBirth", employeeData.DateOfBirth, DbType.DateTime);
            parameters.Add("@GenderId", employeeData.GenderId, DbType.Int32);
            parameters.Add("@MaritalId", employeeData.MaritalId, DbType.Int32);
            parameters.Add("@ReligionId", employeeData.ReligionId, DbType.Int32);
            parameters.Add("@EmployeeTypeId", employeeData.EmployeeTypeId, DbType.Int32);
            parameters.Add("@GradeId", employeeData.GradeId, DbType.Int32);
            parameters.Add("@DepartmentId", employeeData.DepartmentId, DbType.Int32);
            parameters.Add("@DesignationId", employeeData.DesignationId, DbType.Int32);
            parameters.Add("@LocationId", employeeData.LocationId, DbType.Int32);
            parameters.Add("@JoiningDate", employeeData.JoiningDate, DbType.DateTime);
            parameters.Add("@JournalCode", employeeData.JournalCode, DbType.String);
            parameters.Add("@TinNo", employeeData.TinNo, DbType.String);
            parameters.Add("@MobileNumber", employeeData.MobileNumber, DbType.String);
            parameters.Add("@PresentAddress", employeeData.PresentAddress, DbType.String);
            parameters.Add("@PermanentAddress", employeeData.PermanentAddress, DbType.String);
            parameters.Add("@Qualifications", employeeData.Qualifications, DbType.String);
            parameters.Add("@IdentityMarks", employeeData.IdentityMarks, DbType.String);
            parameters.Add("@Remarks", employeeData.Remarks, DbType.String);
            parameters.Add("@TaxStatus", employeeData.TaxStatus ?? false, DbType.Boolean);
            parameters.Add("@CreatedBy", employeeData.CreatedBy, DbType.String);
            parameters.Add("@CreatedDate", employeeData.CreatedDate, DbType.DateTime);
            parameters.Add("@UpdatedBy", employeeData.UpdatedBy, DbType.String);
            parameters.Add("@UpdatedDate", employeeData.UpdatedDate, DbType.DateTime);
            parameters.Add("@ActiveStatus", employeeData.ActiveStatus ?? false, DbType.Boolean);
            parameters.Add("@EmpSL", string.IsNullOrWhiteSpace(employeeData.EmpSL) ? DBNull.Value : employeeData.EmpSL.Trim(), DbType.String);
            parameters.Add("@Nid", string.IsNullOrWhiteSpace(employeeData.Nid) ? DBNull.Value : employeeData.Nid.Trim(), DbType.String);

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    result = await connection.ExecuteScalarAsync<int>(query, parameters);
                }
                catch (Exception ex)
                {
                    // Log the error or handle it as needed
                    throw;
                }
            }

            return result;
        }

        public async Task<int> ImportDepartmentData(DepartmentImportData departmentData)
        {
            int result = 0;
            var parameters = new DynamicParameters();

            string query = @"
                            INSERT INTO Departments (
                                UserId, SchoolId, RoleId, GuestPkId, DepartmentName, 
                                JournalCode, Description, IsActive, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate
                            ) 
                            VALUES (
                                @UserId, @SchoolId, @RoleId, @GuestPkId, @DepartmentName, 
                                @JournalCode, @Description, @IsActive, @CreatedBy, @CreatedDate, @UpdatedBy, @UpdatedDate
                            ); 
                            SELECT CAST(SCOPE_IDENTITY() AS INT);
                        ";

            parameters.Add("@UserId", departmentData.UserId, DbType.Int32);
            parameters.Add("@SchoolId", departmentData.SchoolId, DbType.Int32);
            parameters.Add("@RoleId", departmentData.RoleId, DbType.Int32);
            parameters.Add("@GuestPkId", departmentData.GuestPkId, DbType.Int32);
            parameters.Add("@DepartmentName", departmentData.DepartmentName, DbType.String);
            parameters.Add("@JournalCode", departmentData.JournalCode, DbType.String);
            parameters.Add("@Description", departmentData.Description, DbType.String);
            parameters.Add("@IsActive", departmentData.IsActive, DbType.Boolean);
            parameters.Add("@CreatedBy", departmentData.CreatedBy, DbType.String);
            parameters.Add("@UpdatedBy", departmentData.UpdatedBy, DbType.String);
            parameters.Add("@CreatedDate", departmentData.CreatedDate != DateTime.MinValue ? departmentData.CreatedDate : (object)DBNull.Value, DbType.DateTime);
            parameters.Add("@UpdatedDate", departmentData.UpdatedDate != DateTime.MinValue ? departmentData.UpdatedDate : (object)DBNull.Value, DbType.DateTime);


            using (var connection = _context.CreateConnection())
            {
                try
                {
                    result = await connection.ExecuteScalarAsync<int>(query, parameters);
                }
                catch (Exception ex)
                {
                    // Log the exception for debugging
                    Console.WriteLine("Error inserting department: " + ex.Message);
                    throw;
                }
            }

            return result;
        }

        public async Task<int> ImportDesignationData(DesignationImportData designationData)
        {
            int result = 0;
            var parameters = new DynamicParameters();

            string query = @"
                            INSERT INTO Designations 
                            (UserId, SchoolId, RoleId, GuestPkId, DesignationName, Description, MultiDesignation, 
                             IsActive, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, EmployeeTypeId)
                            VALUES 
                            (@UserId, @SchoolId, @RoleId, @GuestPkId, @DesignationName, @Description, @MultiDesignation, 
                             @IsActive, @CreatedBy, @CreatedDate, @UpdatedBy, @UpdatedDate, @EmployeeTypeId);
        
                            SELECT CAST(SCOPE_IDENTITY() as int);
                        ";

            parameters.Add("@UserId", designationData.UserId, DbType.Int32);
            parameters.Add("@SchoolId", designationData.SchoolId, DbType.Int32);
            parameters.Add("@RoleId", designationData.RoleId, DbType.Int32);
            parameters.Add("@GuestPkId", designationData.GuestPkId, DbType.Int32);
            parameters.Add("@DesignationName", designationData.DesignationName, DbType.String);
            parameters.Add("@Description", designationData.Description, DbType.String);
            parameters.Add("@MultiDesignation", designationData.MultiDesignation, DbType.String);
            parameters.Add("@IsActive", designationData.IsActive, DbType.Boolean);
            parameters.Add("@CreatedBy", designationData.CreatedBy, DbType.String);
            parameters.Add("@CreatedDate", designationData.CreatedDate ?? DateTime.Now, DbType.DateTime); // Handle NULL
            parameters.Add("@UpdatedBy", designationData.UpdatedBy, DbType.String);
            parameters.Add("@UpdatedDate", designationData.UpdatedDate ?? (object)DBNull.Value, DbType.DateTime); // Allow NULL
            parameters.Add("@EmployeeTypeId", designationData.EmployeeTypeId, DbType.Int32);


            using (var connection = _context.CreateConnection())
            {
                try
                {
                    result = await connection.ExecuteScalarAsync<int>(query, parameters);
                }
                catch (Exception ex)
                {
                    // Log the error if needed
                    throw;
                }
            }

            return result;
        }
        public async Task<int> ImportRoleData(Roles role)
        {
            int result = 0;
            var parameters = new DynamicParameters();
            string query = @"
                            INSERT INTO Roles 
                            (user_id, guest_pk_id, school_id, title, slug, notes, 
                             is_employee, is_authority, is_staff, is_active, 
                             CreatedBy,updatedby)
                            VALUES 
                            (@UserId, @GuestPkId, @SchoolId, @Title, @Slug, @Notes, 
                             @IsEmployee, @IsAuthority, @IsStaff, @IsActive, 
                             @CreatedBy, @UpdatedBy);
                            SELECT CAST(SCOPE_IDENTITY() AS INT);
                        ";

            parameters.Add("@UserId", role.UserId, DbType.Int32);
            parameters.Add("@GuestPkId", role.GuestPkId, DbType.Int32);
            parameters.Add("@SchoolId", role.SchoolId, DbType.Int32);
            parameters.Add("@Title", role.Title, DbType.String);
            parameters.Add("@Slug", role.Slug, DbType.String);
            parameters.Add("@Notes", role.Notes, DbType.String);
            parameters.Add("@IsEmployee", role.IsEmployee, DbType.Boolean);
            parameters.Add("@IsAuthority", role.IsAuthority, DbType.Boolean);
            parameters.Add("@IsStaff", role.IsStaff, DbType.Boolean);
            parameters.Add("@IsActive", role.IsActive, DbType.Boolean);
            parameters.Add("@CreatedBy", role.CreatedBy, DbType.String);
            parameters.Add("@UpdatedBy", role.UpdatedBy, DbType.String);

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    result = await connection.ExecuteScalarAsync<int>(query, parameters);
                }
                catch (Exception ex)
                {
                    // Log the error or handle it as needed
                    throw;
                }
            }

            return result;
        }

    }
}
