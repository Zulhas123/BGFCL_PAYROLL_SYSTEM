using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Dapper;
using Entities;

namespace Repositories
{
    public class DepartmentRepository:IDepartmentContract
    {
        private readonly BgfclContext _context;

        public DepartmentRepository(BgfclContext context)
        {
            _context = context;
        }

        public async Task<int> CreateDepartment(Department department)
        {
            int result = 0;
            var query = "INSERT INTO Departments (UserId,SchoolId,RoleId,GuestPkId,DepartmentName,JournalCode,Description,IsActive,CreatedBy,CreatedDate) VALUES (@userId,@schoolId,@roleId,@guestPkId,@departmentName,@journalCode,@description,@isActive,@createdBy,@createdDate)";
            var parameters = new DynamicParameters();
            parameters.Add("userId", department.UserId, DbType.Int32);
            parameters.Add("schoolId", department.SchoolId, DbType.Int32);
            parameters.Add("roleId", department.RoleId, DbType.Int32);
            parameters.Add("guestPkId", department.GuestPkId, DbType.Int32);
            parameters.Add("departmentName", department.DepartmentName, DbType.String);
            parameters.Add("journalCode", department.JournalCode, DbType.String);
            parameters.Add("description", department.Description, DbType.String);
            parameters.Add("isActive", department.IsActive, DbType.Boolean);
            parameters.Add("createdBy", department.CreatedBy, DbType.String);
            parameters.Add("createdDate", department.CreatedDate, DbType.DateTime);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    result = await connection.ExecuteScalarAsync<int>(query, parameters);
                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }

        public async Task<List<Department>> GetDepartments()
        {
            var query = "SELECT * FROM Departments";
            using (var connection = _context.CreateConnection())
            {
                var departments = await connection.QueryAsync<Department>(query);
                return departments.ToList();
            }
        }

        public async Task<Department> GetDepartmentById(int departmentId)
        {
            var query = "SELECT * FROM Departments where  id=@id";
            using (var connection = _context.CreateConnection())
            {
                var department = await connection.QuerySingleOrDefaultAsync<Department>(query,new {id = departmentId});
                return department;
            }
        }

        public async Task<Department> GetDepartmentByJournalCode(string journalCode)
        {
            var query = "SELECT * FROM Departments where  JournalCode=@journalCode";
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var department = await connection.QuerySingleOrDefaultAsync<Department>(query, new { journalCode });
                    return department;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
        }

        public async Task<int> UpdateDepartment(Department department)
        {
            var query = "UPDATE departments SET UserId=@userId,SchoolId=@schoolId,RoleId=@roleId,GuestPkId=@guestPkId, IsActive = @isActive, DepartmentName = @departmentName, JournalCode = @journalCode, Description = @description, updatedby = @updatedby, updateddate = @updateddate WHERE id = @id";
            var parameters = new DynamicParameters();
            parameters.Add("userId", department.UserId, DbType.Int32);
            parameters.Add("schoolId", department.SchoolId, DbType.Int32);
            parameters.Add("roleId", department.RoleId, DbType.Int32);
            parameters.Add("guestPkId", department.GuestPkId, DbType.Int32);
            parameters.Add("departmentName", department.DepartmentName, DbType.String);
            parameters.Add("journalCode", department.JournalCode, DbType.String);
            parameters.Add("description", department.Description, DbType.String);
            parameters.Add("isActive", department.IsActive, DbType.Boolean);
            parameters.Add("updatedby", department.UpdatedBy, DbType.String);
            parameters.Add("updateddate", department.UpdatedDate, DbType.DateTime);
            parameters.Add("id", department.Id, DbType.Int32);

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameters);
                return result; // This returns the number of rows affected
            }
        }


        public async Task<int> RemoveDepartment(int id)
        {
            var query = "update departments set isactive = 0 where id = @id";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteScalarAsync<int>(query, new { id });
                return result;
            }
        }
    }
}
