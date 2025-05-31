using Contracts;
using Dapper;
using Entities;
using Entities.ViewModels;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class EmployeeRepository:IEmployeeContract
    {
        private readonly BgfclContext _context;

        public EmployeeRepository(BgfclContext context)
        {
            _context = context;
        }

        //public async Task<List<EmployeeViewModel>> GetEmployees(int activeStatus,int employeeTypeId)
        //{
        //    string query = "";
        //    if (activeStatus==3)
        //    {
        //        if (employeeTypeId == 1)
        //        {
        //            query = "select Employees.Id, Employees.JobCode,Employees.EmployeeName, Designations.DesignationName, Departments.DepartmentName,ActiveStatus from Employees join Designations on Employees.DesignationId = Designations.Id join Departments on Employees.DepartmentId = Departments.Id where Employees.ActiveStatus=3 and Employees.EmployeeTypeId in (1,3) order by CONVERT(INT, Employees.EmpSl) ASC;";
        //        }
        //        else
        //        {
        //            query = "select Employees.Id, Employees.JobCode,Employees.EmployeeName, Designations.DesignationName, Departments.DepartmentName,ActiveStatus from Employees join Designations on Employees.DesignationId = Designations.Id join Departments on Employees.DepartmentId = Departments.Id where Employees.ActiveStatus=3 and Employees.EmployeeTypeId in (2,4) order by CONVERT(INT, Employees.EmpSl) ASC;";
        //        }
        //    }
        //    else
        //    {
        //        if (employeeTypeId == 1)
        //        {
        //            query = "select Employees.Id, Employees.JobCode,Employees.EmployeeName, Designations.DesignationName, Departments.DepartmentName,ActiveStatus from Employees join Designations on Employees.DesignationId = Designations.Id join Departments on Employees.DepartmentId = Departments.Id where Employees.ActiveStatus in (1,2,3) and Employees.EmployeeTypeId in (1,3) order by CONVERT(INT, Employees.EmpSl) ASC;";
        //        }
        //        else
        //        {
        //            query = "select Employees.Id, Employees.JobCode,Employees.EmployeeName, Designations.DesignationName, Departments.DepartmentName,ActiveStatus from Employees join Designations on Employees.DesignationId = Designations.Id join Departments on Employees.DepartmentId = Departments.Id where Employees.ActiveStatus in (1,2,3) and Employees.EmployeeTypeId in (2,4) order by CONVERT(INT, Employees.EmpSl) ASC;";
        //        }
        //    }
        //    using (var connection = _context.CreateConnection())
        //    {
        //        var employees = await connection.QueryAsync<EmployeeViewModel>(query,new { activeStatus, employeeTypeId });
        //        return employees.ToList();
        //    }
        //}
        public async Task<List<EmployeeViewModel>> GetEmployees(int activeStatus, int employeeTypeId)
        {
            string query = @"
                    SELECT Employees.Id, Employees.JobCode, Employees.EmployeeName, 
                           Designations.DesignationName, Departments.DepartmentName, Employees.ActiveStatus 
                    FROM Employees 
                    JOIN Designations ON Employees.DesignationId = Designations.Id 
                    JOIN Departments ON Employees.DepartmentId = Departments.Id 
                    WHERE Employees.EmployeeTypeId = @EmployeeTypeId";

            query += " ORDER BY TRY_CAST(Employees.EmpSl AS INT) ASC;";

            using (var connection = _context.CreateConnection())
            {
                var employees = await connection.QueryAsync<EmployeeViewModel>(query, new { EmployeeTypeId = employeeTypeId, ActiveStatus = activeStatus });
                return employees.ToList();
            }
        }


        public async Task<List<EmployeeViewModel>> GetEmployeeswithFilter(int activeStatus, int employeeTypeId, int? schoolId, int? roleId, int? departmentId, int? designationId)
        {
            string query = @"
                            SELECT Employees.Id, Employees.JobCode, Employees.EmployeeName, 
                                   Designations.DesignationName, Departments.DepartmentName, Employees.ActiveStatus 
                            FROM Employees 
                            JOIN Designations ON Employees.DesignationId = Designations.Id 
                            JOIN Departments ON Employees.DepartmentId = Departments.Id 
                            WHERE Employees.EmployeeTypeId = @EmployeeTypeId";

            if (schoolId.HasValue)
                query += " AND Employees.SchoolId = @SchoolId";
            if (roleId.HasValue)
                query += " AND Employees.RoleId = @RoleId";
            if (departmentId.HasValue)
                query += " AND Employees.DepartmentId = @DepartmentId";
            if (designationId.HasValue)
                query += " AND Employees.DesignationId = @DesignationId";

            query += " ORDER BY TRY_CAST(Employees.EmpSl AS INT) ASC;";

            using (var connection = _context.CreateConnection())
            {
                var employees = await connection.QueryAsync<EmployeeViewModel>(query, new
                {
                    EmployeeTypeId = employeeTypeId,
                    ActiveStatus = activeStatus,
                    SchoolId = schoolId,
                    RoleId = roleId,
                    DepartmentId = departmentId,
                    DesignationId = designationId
                });

                return employees.ToList();
            }
        }



        public async Task<List<EmployeeViewModel>> GetInactiveEmployees( int employeeTypeId)
        {
            string query = "";
      
                if (employeeTypeId == 1)
                {
                    query = "select Employees.Id, Employees.JobCode,Employees.EmployeeName, Designations.DesignationName, Departments.DepartmentName from Employees join Designations on Employees.DesignationId = Designations.Id join Departments on Employees.DepartmentId = Departments.Id where Employees.ActiveStatus=3 and Employees.EmployeeTypeId in (1,3)";
                }
                else
                {
                    query = "select Employees.Id, Employees.JobCode,Employees.EmployeeName, Designations.DesignationName, Departments.DepartmentName from Employees join Designations on Employees.DesignationId = Designations.Id join Departments on Employees.DepartmentId = Departments.Id where Employees.ActiveStatus=3 and Employees.EmployeeTypeId in (2,4)";
                }
            using (var connection = _context.CreateConnection())
            {
                var employees = await connection.QueryAsync<EmployeeViewModel>(query, new {employeeTypeId });
                return employees.ToList();
            }
        }
        public async Task<List<EmployeeViewModel>> GetEmployeesByEmployeeType(int employeeTypeId)
        {
            string query = "";
            if (employeeTypeId==1)
            {
                query = "select Employees.Id, Employees.JobCode,Employees.EmployeeName, Designations.DesignationName, Departments.DepartmentName from Employees join Designations on Employees.DesignationId = Designations.Id join Departments on Employees.DepartmentId = Departments.Id where Employees.EmployeeTypeId  = 1 and Employees.ActiveStatus in (1,2)";
            }
            else if(employeeTypeId==2)
            {
                query = "select Employees.Id, Employees.JobCode,Employees.EmployeeName, Designations.DesignationName, Departments.DepartmentName from Employees join Designations on Employees.DesignationId = Designations.Id join Departments on Employees.DepartmentId = Departments.Id where Employees.EmployeeTypeId = 2 and Employees.ActiveStatus in (1,2)";
            }
            else 
            {
                query = "select Employees.Id, Employees.JobCode,Employees.EmployeeName, Designations.DesignationName, Departments.DepartmentName from Employees join Designations on Employees.DesignationId = Designations.Id join Departments on Employees.DepartmentId = Departments.Id where Employees.EmployeeTypeId  = 3  and Employees.ActiveStatus in (1,2)";
            }
            using (var connection = _context.CreateConnection())
            {
                var employees = await connection.QueryAsync<EmployeeViewModel>(query);
                return employees.ToList();
            }
        }

        public async Task<List<Employee>> GetEmployeeCode()
        {
            var query = "SELECT Id, JobCode, EmployeeName FROM Employees WHERE ActiveStatus = 1";

            using (var connection = _context.CreateConnection())
            {
                var employees = await connection.QueryAsync<Employee>(query);
                return employees.ToList();
            }
        }

        public async Task<List<Employee>> GetAllEmployees()
        {
            var query = "SELECT Id, JobCode, EmployeeName FROM Employees";

            using (var connection = _context.CreateConnection())
            {
                var employees = await connection.QueryAsync<Employee>(query);
                return employees.ToList();
            }
        }
        public async Task<List<Employee>> GetAllEmployeesByType(int employeeTypeId)
        {
            var query = @"
                        SELECT Id, JobCode, EmployeeName FROM Employees 
                        WHERE EmployeeTypeId = @employeeTypeId";

            using (var connection = _context.CreateConnection())
            {
                var employees = await connection.QueryAsync<Employee>(query, new { employeeTypeId });
                return employees.ToList();
            }
        }


        public async Task<List<Employee>> GetAllOfficer()
        {
            var query = "SELECT * FROM Employees WHERE EmployeeTypeId in(1,3)";

            using (var connection = _context.CreateConnection())
            {
                var employees = await connection.QueryAsync<Employee>(query);
                return employees.ToList();
            }
        }
        public async Task<string> GetLastPermanetEmployee()
        {
            var query = "SELECT TOP 1 EmployeeName FROM Employees WHERE EmployeeTypeId = 1 ORDER BY Id DESC";

            using (var connection = _context.CreateConnection())
            {
                var employeeName = await connection.QueryFirstOrDefaultAsync<string>(query);
                return employeeName;
            }
        }

        public async Task<string> GetLastContractEmployee()
        {
            var query = "SELECT TOP 1 EmployeeName FROM Employees where EmployeeTypeId=2 ORDER BY id DESC";

            using (var connection = _context.CreateConnection())
            {
                var employeeName = await connection.QueryFirstOrDefaultAsync<string>(query);
                return employeeName;
            }
        }
        public async Task<List<Employee>> GetAllJuniorStaff()
        {
            var query = "SELECT * FROM Employees WHERE EmployeeTypeId in(2,4)";

            using (var connection = _context.CreateConnection())
            {
                var employees = await connection.QueryAsync<Employee>(query);
                return employees.ToList();
            }
        }
        public async Task<List<Employee>> GetAllPensionOFJS()
        {
            var query = "SELECT * FROM Employees WHERE EmployeeTypeId in(3,4)";

            using (var connection = _context.CreateConnection())
            {
                var employees = await connection.QueryAsync<Employee>(query);
                return employees.ToList();
            }
        }
        public async Task<int> CreateEmployee(Employee employee)
        {
            int result = 0;
            var query = "INSERT INTO Employees (SchoolId,GuestPkId,UuId,GuestUserId,UserId,UserTypeId,RoleId,ChildCount,JobCode,EmployeeName,FatherName,MotherName,DateOfBirth,GenderId,MaritalId,ReligionId,EmployeeTypeId,GradeId,DepartmentId,DesignationId,LocationId,JoiningDate,JournalCode,TinNo,MobileNumber,PresentAddress,PermanentAddress,Qualifications,IdentityMarks,Remarks,TaxStatus,ActiveStatus,CreatedBy,CreatedDate,Nid,EmpSL) VALUES " +
                "(@schoolId,@guestPkId,@uuId,@guestUserId,@userId,@userTypeId,@roleId,@childCount,@jobCode,@employeeName,@fatherName,@motherName,@dateOfBirth,@genderId,@maritalId,@religionId,@employeeTypeId,@gradeId,@departmentId,@designationId,@locationId,@joiningDate,@journalCode,@tinNo,@mobileNumber,@presentAddress,@permanentAddress,@qualifications,@identityMarks,@remarks,@taxStatus,@activeStatus,@createdBy,@createdDate,@nid,@empSl)";

            var parameters = new DynamicParameters();
            parameters.Add("jobCode", employee.JobCode, DbType.String);
            parameters.Add("employeeName", employee.EmployeeName, DbType.String);
            parameters.Add("fatherName", employee.FatherName, DbType.String);
            parameters.Add("motherName", employee.MotherName, DbType.String);
            if (employee.EmployeeTypeId == 3)
            {
                parameters.Add("dateOfBirth", DateTime.Now.Date, DbType.Date);
                parameters.Add("joiningDate", DateTime.Now.Date, DbType.Date);
            }
            else
            {
                parameters.Add("dateOfBirth", DateTime.ParseExact(employee.DateOfBirth, "dd/MM/yyyy", CultureInfo.InvariantCulture), DbType.Date);
                parameters.Add("joiningDate", DateTime.ParseExact(employee.JoiningDate, "dd/MM/yyyy", CultureInfo.InvariantCulture), DbType.Date);
            }
            parameters.Add("schoolId", employee.SchoolId, DbType.Int32);
            parameters.Add("guestPkId", employee.GuestPkId, DbType.Int32);
            parameters.Add("uuId", employee.UuId, DbType.Int32);
            parameters.Add("guestUserId", employee.GuestUserId, DbType.Int32);
            parameters.Add("userId", employee.UserId, DbType.Int32);
            parameters.Add("userTypeId", employee.UserTypeId, DbType.Int32);
            parameters.Add("roleId", employee.RoleId, DbType.Int32);
            parameters.Add("childCount", employee.ChildCount, DbType.Int32);
            parameters.Add("genderId", employee.GenderId, DbType.Int32);
            parameters.Add("maritalId", employee.MaritalId, DbType.Int32);
            parameters.Add("religionId", employee.ReligionId, DbType.Int32);
            parameters.Add("employeeTypeId", employee.EmployeeTypeId, DbType.Int32);
            parameters.Add("gradeId", employee.GradeId, DbType.Int32);
            parameters.Add("departmentId", employee.DepartmentId, DbType.Int32);
            parameters.Add("designationId", employee.DesignationId, DbType.Int32);
            parameters.Add("locationId", employee.LocationId, DbType.Int32);
            parameters.Add("journalCode", employee.JournalCode, DbType.String);
            parameters.Add("tinNo", employee.TinNo, DbType.String);
            parameters.Add("mobileNumber", employee.MobileNumber, DbType.String);
            parameters.Add("presentAddress", employee.PresentAddress, DbType.String);
            parameters.Add("permanentAddress", employee.PermanentAddress, DbType.String);
            parameters.Add("qualifications", employee.Qualifications, DbType.String);

            parameters.Add("identityMarks", employee.IdentityMarks, DbType.String);
            parameters.Add("remarks", employee.Remarks, DbType.String);
            parameters.Add("taxStatus", employee.TaxStatus, DbType.Boolean);
            parameters.Add("activeStatus", employee.ActiveStatus, DbType.Int32);
            parameters.Add("createdBy", employee.CreatedBy, DbType.String);
            parameters.Add("createdDate", employee.CreatedDate, DbType.DateTime);
            parameters.Add("nid", employee.Nid, DbType.String);
            parameters.Add("empSl", employee.EmpSL, DbType.String);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    result = await connection.ExecuteAsync(query, parameters);
                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }
        public async Task<int> CreateEmployees(List<Employee> employees)
        {
            int result = 0;

            var insertQuery = @"
                            INSERT INTO Employees 
                            (SchoolId, GuestPkId, UuId, GuestUserId, UserId, UserTypeId, RoleId, ChildCount, JobCode, EmployeeName, FatherName, MotherName, DateOfBirth, GenderId, MaritalId, ReligionId, EmployeeTypeId, GradeId, DepartmentId, DesignationId, LocationId, JoiningDate, JournalCode, TinNo, MobileNumber, PresentAddress, PermanentAddress, Qualifications, IdentityMarks, Remarks, TaxStatus, ActiveStatus, CreatedBy, CreatedDate, Nid, EmpSL) 
                            VALUES 
                            (@schoolId, @guestPkId, @uuId, @guestUserId, @userId, @userTypeId, @roleId, @childCount, @jobCode, @employeeName, @fatherName, @motherName, @dateOfBirth, @genderId, @maritalId, @religionId, @employeeTypeId, @gradeId, @departmentId, @designationId, @locationId, @joiningDate, @journalCode, @tinNo, @mobileNumber, @presentAddress, @permanentAddress, @qualifications, @identityMarks, @remarks, @taxStatus, @activeStatus, @createdBy, @createdDate, @nid, @empSl)";

            var existsQuery = "SELECT COUNT(1) FROM Employees WHERE JobCode = @jobCode";

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    foreach (var employee in employees)
                    {
                        // Step 1: Check if JobCode exists for the same EmployeeType
                        var exists = await connection.ExecuteScalarAsync<int>(existsQuery, new
                        {
                            jobCode = employee.JobCode
                        });

                        if (exists > 0)
                            continue; // Skip duplicate

                        // Step 2: Build insert parameters
                        var parameters = new DynamicParameters();

                        parameters.Add("schoolId", employee.SchoolId);
                        parameters.Add("guestPkId", employee.GuestPkId);
                        parameters.Add("uuId", employee.UuId);
                        parameters.Add("guestUserId", employee.GuestUserId);
                        parameters.Add("userId", employee.UserId);
                        parameters.Add("userTypeId", employee.UserTypeId);
                        parameters.Add("roleId", employee.RoleId);
                        parameters.Add("childCount", employee.ChildCount);
                        parameters.Add("jobCode", employee.JobCode);
                        parameters.Add("employeeName", employee.EmployeeName);
                        parameters.Add("fatherName", employee.FatherName);
                        parameters.Add("motherName", employee.MotherName);
                        parameters.Add("dateOfBirth", DateTime.ParseExact(employee.DateOfBirth, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                        parameters.Add("genderId", employee.GenderId);
                        parameters.Add("maritalId", employee.MaritalId);
                        parameters.Add("religionId", employee.ReligionId);
                        parameters.Add("employeeTypeId", employee.EmployeeTypeId);
                        parameters.Add("gradeId", employee.GradeId);
                        parameters.Add("departmentId", employee.DepartmentId);
                        parameters.Add("designationId", employee.DesignationId);
                        parameters.Add("locationId", employee.LocationId);
                        parameters.Add("joiningDate", DateTime.ParseExact(employee.JoiningDate, "dd/MM/yyyy", CultureInfo.InvariantCulture));
                        parameters.Add("journalCode", employee.JournalCode);
                        parameters.Add("tinNo", employee.TinNo);
                        parameters.Add("mobileNumber", employee.MobileNumber);
                        parameters.Add("presentAddress", employee.PresentAddress);
                        parameters.Add("permanentAddress", employee.PermanentAddress);
                        parameters.Add("qualifications", employee.Qualifications);
                        parameters.Add("identityMarks", employee.IdentityMarks);
                        parameters.Add("remarks", employee.Remarks);
                        parameters.Add("taxStatus", employee.TaxStatus);
                        parameters.Add("activeStatus", employee.ActiveStatus);
                        parameters.Add("createdBy", employee.CreatedBy);
                        parameters.Add("createdDate", employee.CreatedDate);
                        parameters.Add("nid", employee.Nid);
                        parameters.Add("empSl", employee.EmpSL);

                        // Step 3: Insert the employee
                        result += await connection.ExecuteAsync(insertQuery, parameters);
                    }
                }
                catch (Exception ex)
                {
                    // Consider using a logger instead of Console
                    Console.WriteLine("Error inserting employees: " + ex.Message);
                }
            }

            return result;
        }




        public async Task<int> UpdateEmployee(Employee employee)
        {
            var query = "Update Employees set JobCode=@jobCode," +
                "EmployeeName=@employeeName," +
                "FatherName=@fatherName," +
                "MotherName=@motherName," +
                "DateOfBirth=@dateOfBirth," +
                "GenderId=@genderId," +
                "MaritalId=@maritalId," +
                "ReligionId=@religionId," +
                "EmployeeTypeId=@employeeTypeId," +
                "SchoolId=@schoolId," +
                "GuestPkId=@guestPkId," +
                "UuId=@uuId," +
                "GuestUserId=@guestUserId," +
                "UserId=@userId," +
                "UserTypeId=@userTypeId," +
                "RoleId=@roleId," +
                "ChildCount=@childCount," +
                "GradeId=@gradeId," +
                "DepartmentId=@departmentId," +
                "DesignationId=@designationId," +
                "LocationId=@locationId," +
                "JoiningDate=@joiningDate," +
                "JournalCode=@journalCode," +
                "TinNo=@tinNo," +
                "MobileNumber=@mobileNumber," +
                "PresentAddress=@presentAddress," +
                "PermanentAddress=@permanentAddress," +
                "Qualifications=@qualifications," +
                "IdentityMarks=@identityMarks," +
                "Remarks=@remarks," +
                "TaxStatus=@taxStatus," +
                "ActiveStatus=@activeStatus," +
                "Nid=@nid," +
                "EmpSL=@empSl," +
                "UpdatedBy=@updatedBy," +
                "UpdatedDate=@updatedDate where id = @id";

            var parameters = new DynamicParameters();
            parameters.Add("jobCode", employee.JobCode, DbType.String);
            parameters.Add("employeeName", employee.EmployeeName, DbType.String);
            parameters.Add("fatherName", employee.FatherName, DbType.String);
            parameters.Add("motherName", employee.MotherName, DbType.String);
            parameters.Add("dateOfBirth", DateTime.ParseExact(employee.DateOfBirth, "dd/MM/yyyy", CultureInfo.InvariantCulture), DbType.Date);

            parameters.Add("genderId", employee.GenderId, DbType.Int32);
            parameters.Add("schoolId", employee.SchoolId, DbType.Int32);
            parameters.Add("guestPkId", employee.GuestPkId, DbType.Int32);
            parameters.Add("uuId", employee.UuId, DbType.Int32);
            parameters.Add("guestUserId", employee.GuestUserId, DbType.Int32);
            parameters.Add("userId", employee.UserId, DbType.Int32);
            parameters.Add("userTypeId", employee.UserTypeId, DbType.Int32);
            parameters.Add("roleId", employee.RoleId, DbType.Int32);
            parameters.Add("childCount", employee.ChildCount, DbType.Int32);
            parameters.Add("maritalId", employee.MaritalId, DbType.Int32);
            parameters.Add("religionId", employee.ReligionId, DbType.Int32);
            parameters.Add("employeeTypeId", employee.EmployeeTypeId, DbType.Int32);
            parameters.Add("gradeId", employee.GradeId, DbType.Int32);

            parameters.Add("departmentId", employee.DepartmentId, DbType.Int32);
            parameters.Add("designationId", employee.DesignationId, DbType.Int32);
            parameters.Add("locationId", employee.LocationId, DbType.Int32);
            parameters.Add("joiningDate", DateTime.ParseExact(employee.JoiningDate, "dd/MM/yyyy", CultureInfo.InvariantCulture), DbType.Date);
            parameters.Add("journalCode", employee.JournalCode, DbType.String);

            parameters.Add("tinNo", employee.TinNo, DbType.String);
            parameters.Add("mobileNumber", employee.MobileNumber, DbType.String);
            parameters.Add("presentAddress", employee.PresentAddress, DbType.String);
            parameters.Add("permanentAddress", employee.PermanentAddress, DbType.String);
            parameters.Add("qualifications", employee.Qualifications, DbType.String);

            parameters.Add("identityMarks", employee.IdentityMarks, DbType.String);
            parameters.Add("remarks", employee.Remarks, DbType.String);
            parameters.Add("taxStatus", employee.TaxStatus, DbType.Boolean);
            parameters.Add("activeStatus", employee.ActiveStatus, DbType.Int32);
            parameters.Add("nid", employee.Nid, DbType.String);
            parameters.Add("empSl", employee.EmpSL, DbType.String);
            parameters.Add("updatedBy", employee.UpdatedBy, DbType.String);
            parameters.Add("updatedDate", employee.UpdatedDate, DbType.DateTime);
            parameters.Add("id", employee.Id, DbType.Int32);

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteScalarAsync<int>(query, parameters);
                return result;
            }
        }

        public async Task<int> UpdateInactiveEmployee(InactiveEmployeeOf employee)
        {
            var query = "Update Employees set JobCode=@jobCode," +
                "EmployeeName=@employeeName," +          
                "ActiveStatus=@activeStatus," +
                "UpdatedBy=@updatedBy," +
                "UpdatedDate=@updatedDate where id = @id";

            var parameters = new DynamicParameters();
            parameters.Add("jobCode", employee.JobCode, DbType.String);
            parameters.Add("employeeName", employee.EmployeeName, DbType.String);
            parameters.Add("activeStatus", employee.ActiveStatus, DbType.Int32);
            parameters.Add("updatedBy", employee.UpdatedBy, DbType.String);
            parameters.Add("updatedDate", employee.UpdatedDate, DbType.DateTime);
            parameters.Add("id", employee.Id, DbType.Int32);

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteScalarAsync<int>(query, parameters);
                return result;
            }
        }


        public async Task<Employee> GetEmployeeById(int employeeId)
        {
            var query = "SELECT * FROM Employees where id=@id";
            using (var connection = _context.CreateConnection())
            {
                var employee = await connection.QuerySingleOrDefaultAsync<Employee>(query, new { id = employeeId });
                return employee;
            }
        }
        public async Task<Employee> GetEmployeeByjobCode(string jobcode)
        {
            var query = "SELECT * FROM Employees where JobCode=@jobcode";
            using (var connection = _context.CreateConnection())
            {
                var employee = await connection.QuerySingleOrDefaultAsync<Employee>(query, new { jobcode });
                return employee;
            }
        }

        public async Task<EmployeeViewModel> GetEmployeeViewById(int employeeId)
        {
            var query = "select Employees.Id, Employees.JobCode,Employees.EmployeeName, Designations.DesignationName, Departments.DepartmentName from Employees join Designations on Employees.DesignationId = Designations.Id join Departments on Employees.DepartmentId = Departments.Id where Employees.Id = @id";

            using (var connection = _context.CreateConnection())
            {
                var employee = await connection.QueryFirstOrDefaultAsync<EmployeeViewModel>(query, new { id = employeeId });
                return employee;
            }
        }

        public async Task<EmployeeViewModel> GetEmployeeViewByJobCode(string jobCode)
        {
            var query = "select Employees.Id, Employees.JobCode,Employees.EmployeeName, Employees.EmployeeTypeId, Designations.DesignationName, Departments.DepartmentName, Employees.MobileNumber from Employees join Designations on Employees.DesignationId = Designations.Id join Departments on Employees.DepartmentId = Departments.Id where Employees.JobCode = @jobCode";

            using (var connection = _context.CreateConnection())
            {
                var employee = await connection.QueryFirstOrDefaultAsync<EmployeeViewModel>(query, new { jobCode = jobCode });
                return employee;
            }
        }


        public async Task<EmployeeViewModel> GetEmployeeForView(int id)
        {
            var query = @"select Employees.Id,
                        Employees.JobCode,
                        Employees.EmployeeName,
                        Designations.DesignationName,
                        Departments.DepartmentName,
                        Employees.FatherName,
                        Employees.MotherName,
                        Employees.DateOfBirth,
                        Genders.GenderName,
                        Maritals.MaritalName,
                        Religions.ReligionName,
                        EmployeeTypes.EmployeeTypeName,
                        Grades.GradeName,
                        Locations.LocationName,
                        Employees.JoiningDate,
                        Employees.JournalCode,
                        ISNULL(Employees.TinNo,'') as TinNo,
                        ISNULL(Employees.Nid,'') as Nid,
                        Employees.MobileNumber,
                        Employees.PresentAddress,
                        Employees.PermanentAddress,
                        Employees.Qualifications,
                        Employees.IdentityMarks,
                        Employees.Remarks,
                        Employees.TaxStatus
                        from Employees 
                        join Designations on Employees.DesignationId = Designations.Id 
                        join Departments on Employees.DepartmentId = Departments.Id 
                        join Genders on Employees.GenderId = Genders.Id
                        join Maritals on Employees.MaritalId = Maritals.Id
                        join Religions on Employees.ReligionId = Religions.Id
                        join EmployeeTypes on Employees.EmployeeTypeId = EmployeeTypes.Id
                        join Grades on Employees.GradeId = Grades.Id
                        join Locations on Employees.LocationId = Locations.Id
                        where Employees.Id=@id";
            using (var connection = _context.CreateConnection())
            {
                var employee = await connection.QuerySingleOrDefaultAsync<EmployeeViewModel>(query, new { id });
                return employee;
            }
        }
        public async Task<int> DeleteEmployee(int id)
        {
            var query = "update Employees set ActiveStatus = 3 where id = @id";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteScalarAsync<int>(query, new { id });
                return result;
            }
        }
    }
}
