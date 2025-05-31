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
    public class EmployeeTypeRepository: IEmployeeTypeContract
    {
        private readonly BgfclContext _context;

        public EmployeeTypeRepository(BgfclContext context)
        {
            _context = context;
        }
        public async Task<List<EmployeeType>> GetEmployeeTypes()
        {
            var query = "SELECT * FROM EmployeeTypes";
            using (var connection = _context.CreateConnection())
            {
                var employeeTypes = await connection.QueryAsync<EmployeeType>(query);
                return employeeTypes.ToList();
            }
        }
        public async Task<int> CreateEmployeeType(EmployeeType employeeType)
        {
            int result = 0;
            var query = "INSERT INTO EmployeeTypes (SchoolId,GuestPkId,RoleId,EmployeeTypeName,Description,IsActive,CreatedBy,CreatedDate) VALUES (@schoolId,@guestPkId,@roleId,@employeeTypeName,@description,@isActive,@createdBy,@createdDate)";
            var parameters = new DynamicParameters();
            parameters.Add("schoolId", employeeType.SchoolId, DbType.Int32);
            parameters.Add("guestPkId", employeeType.GuestPkId, DbType.Int32);
            parameters.Add("employeeTypeName", employeeType.EmployeeTypeName, DbType.String);
            parameters.Add("description", employeeType.Description, DbType.String);
            parameters.Add("roleId", employeeType.RoleId, DbType.String);
            parameters.Add("isActive", employeeType.IsActive, DbType.Boolean);
            parameters.Add("createdBy", employeeType.CreatedBy, DbType.String);
            parameters.Add("createdDate", employeeType.CreatedDate, DbType.DateTime);
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

        public async Task<EmployeeType> GetEmployeeTypeId(int id)
        {
            var query = "SELECT * FROM EmployeeTypes where  id=@id";
            using (var connection = _context.CreateConnection())
            {
                var type = await connection.QuerySingleOrDefaultAsync<EmployeeType>(query, new { id });
                return type;
            }
        }

        public async Task<int> UpdateEmployeeType(EmployeeType employeeType)
        {
            var query = "UPDATE EmployeeTypes SET SchoolId=@schoolId,Description=@description,GuestPkId=@guestPkId,RoleId=@roleId,EmployeeTypeName=@employeeTypeName, IsActive = @isActive,updatedby = @updatedby, updateddate = @updateddate WHERE id = @id";
            var parameters = new DynamicParameters();
            parameters.Add("schoolId", employeeType.SchoolId, DbType.Int32);
            parameters.Add("guestPkId", employeeType.GuestPkId, DbType.Int32);
            parameters.Add("roleId", employeeType.RoleId, DbType.Int32);
            parameters.Add("guestPkId", employeeType.GuestPkId, DbType.Int32);
            parameters.Add("employeeTypeName", employeeType.EmployeeTypeName, DbType.String);
            parameters.Add("description", employeeType.Description, DbType.String);
            parameters.Add("isActive", employeeType.IsActive, DbType.Boolean);
            parameters.Add("updatedby", employeeType.UpdatedBy, DbType.String);
            parameters.Add("updateddate", employeeType.UpdatedDate, DbType.DateTime);
            parameters.Add("id", employeeType.Id, DbType.Int32);

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameters);
                return result; // This returns the number of rows affected
            }
        }
        public async Task<int> DeleteEmployeeType(int id)
        {
            var query = "update EmployeeTypes set isactive = 0 where id = @id";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteScalarAsync<int>(query, new { id });
                return result;
            }
        }
    }
}
