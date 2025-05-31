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
    public class DesignationRepository:IDesignationContract
    {
        private readonly BgfclContext _context;

        public DesignationRepository(BgfclContext context)
        {
            _context = context;
        }

        public async Task<int> CreateDesignation(Designation designation)
        {
            int result = 0;
            var query = @"INSERT INTO Designations 
        (UserId, SchoolId, RoleId, GuestPkId, DesignationName, MultiDesignation, Description, IsActive, CreatedBy, CreatedDate, EmployeeTypeId) 
        VALUES 
        (@userId, @schoolId, @roleId, @guestPkId, @designationName, @multiDesignation, @description, @isActive, @createdBy, @createdDate, @employeeTypeId)";

            var parameters = new DynamicParameters();
            parameters.Add("userId", designation.UserId, DbType.Int32);
            parameters.Add("schoolId", designation.SchoolId, DbType.Int32);
            parameters.Add("roleId", designation.RoleId, DbType.Int32);
            parameters.Add("guestPkId", designation.GuestPkId, DbType.Int32);
            parameters.Add("designationName", designation.DesignationName, DbType.String);
            parameters.Add("multiDesignation", designation.MultiDesignation, DbType.String);  // Add this line
            parameters.Add("description", designation.Description, DbType.String);
            parameters.Add("isActive", designation.IsActive, DbType.Boolean);
            parameters.Add("createdBy", designation.CreatedBy, DbType.String);
            parameters.Add("createdDate", designation.CreatedDate, DbType.DateTime);
            parameters.Add("employeeTypeId", designation.EmployeeTypeId, DbType.Int32);

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    result = await connection.ExecuteAsync(query, parameters);
                }
                catch (Exception ex)
                {
                    // Handle exception (log it or throw)
                }
            }
            return result;
        }


        public async Task<List<Designation>> GetDesignations()
        {
            var query = "SELECT Designations.*,EmployeeTypes.EmployeeTypeName FROM Designations join EmployeeTypes on Designations.EmployeeTypeId = EmployeeTypes.Id where Designations.isactive in(0,1)";
            using (var connection = _context.CreateConnection())
            {
                var designations = await connection.QueryAsync<Designation>(query);
                return designations.ToList();
            }
        }

        public async Task<List<Designation>> GetDesignationsByEmployeeType(int employeeTypeId)
        {
            var query = "SELECT Designations.*,EmployeeTypes.EmployeeTypeName FROM Designations join EmployeeTypes on Designations.EmployeeTypeId = EmployeeTypes.Id where Designations.isactive=1 and Designations.EmployeeTypeId=@employeeTypeId";
            using (var connection = _context.CreateConnection())
            {
                var designations = await connection.QueryAsync<Designation>(query, new { employeeTypeId });
                return designations.ToList();
            }
        }

        public async Task<Designation> GetDesignationById(int designationId)
        {
            var query = "SELECT Designations.*,EmployeeTypes.EmployeeTypeName FROM Designations join EmployeeTypes on Designations.EmployeeTypeId = EmployeeTypes.Id where  Designations.id=@id";
            using (var connection = _context.CreateConnection())
            {
                var designation = await connection.QuerySingleOrDefaultAsync<Designation>(query, new { id = designationId });
                return designation;
            }
        }

        public async Task<int> UpdateDesignation(Designation designation)
        {
            var query = "update Designations set UserId=@userId,SchoolId=@schoolId,RoleId=@roleId,GuestPkId=@guestPkId, IsActive=@isactive, DesignationName = @designationName,multiDesignation=@multiDesignation,Description=@description,updatedby = @updatedby, updateddate = @updateddate, EmployeeTypeId=@employeeTypeId where id = @id";
            var parameters = new DynamicParameters();
            parameters.Add("userId", designation.UserId, DbType.Int32);
            parameters.Add("schoolId", designation.SchoolId, DbType.Int32);
            parameters.Add("roleId", designation.RoleId, DbType.Int32);
            parameters.Add("guestPkId", designation.GuestPkId, DbType.Int32);
            parameters.Add("designationName", designation.DesignationName, DbType.String);
            parameters.Add("multiDesignation", designation.MultiDesignation, DbType.String);
            parameters.Add("description", designation.Description, DbType.String);
            parameters.Add("updatedby", designation.UpdatedBy, DbType.String);
            parameters.Add("updateddate", designation.UpdatedDate, DbType.DateTime);
            parameters.Add("employeeTypeId", designation.EmployeeTypeId, DbType.Int32);
            parameters.Add("isactive", designation.IsActive, DbType.Boolean);
            parameters.Add("id", designation.Id, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteScalarAsync<int>(query, parameters);
                return result;
            }
        }

        public async Task<int> RemoveDesignation(int id)
        {
            var query = "update Designations set isactive = 0 where id = @id";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteScalarAsync<int>(query, new { id });
                return result;
            }
        }

        // Secondary Designation 
        public async Task<List<Designation>> GetSecondaryDesignations()
        {
            var query = "SELECT SecondaryDesignations.*,EmployeeTypes.EmployeeTypeName FROM Designations join EmployeeTypes on Designations.EmployeeTypeId = EmployeeTypes.Id where SecondaryDesignations.isactive in(1)";
            using (var connection = _context.CreateConnection())
            {
                var designations = await connection.QueryAsync<Designation>(query);
                return designations.ToList();
            }
        }
        public async Task<Designation> GetSecondaryDesignationById(int designationId)
        {
            var query = "SELECT SecondaryDesignations.*,EmployeeTypes.EmployeeTypeName FROM Designations join EmployeeTypes on Designations.EmployeeTypeId = EmployeeTypes.Id where  SecondaryDesignations.id=@id";
            using (var connection = _context.CreateConnection())
            {
                var designation = await connection.QuerySingleOrDefaultAsync<Designation>(query, new { id = designationId });
                return designation;
            }
        }
        public async Task<int> CreateSecondaryDesignation(Designation designation)
        {
            int result = 0;
            var query = @"INSERT INTO SecondaryDesignations 
        (UserId, SchoolId, RoleId, GuestPkId, DesignationName, MultiDesignation, Description, IsActive, CreatedBy, CreatedDate, EmployeeTypeId) 
        VALUES 
        (@userId, @schoolId, @roleId, @guestPkId, @designationName, @multiDesignation, @description, @isActive, @createdBy, @createdDate, @employeeTypeId)";

            var parameters = new DynamicParameters();
            parameters.Add("userId", designation.UserId, DbType.Int32);
            parameters.Add("schoolId", designation.SchoolId, DbType.Int32);
            parameters.Add("roleId", designation.RoleId, DbType.Int32);
            parameters.Add("guestPkId", designation.GuestPkId, DbType.Int32);
            parameters.Add("designationName", designation.DesignationName, DbType.String);
            parameters.Add("multiDesignation", designation.MultiDesignation, DbType.String);  // Add this line
            parameters.Add("description", designation.Description, DbType.String);
            parameters.Add("isActive", designation.IsActive, DbType.Boolean);
            parameters.Add("createdBy", designation.CreatedBy, DbType.String);
            parameters.Add("createdDate", designation.CreatedDate, DbType.DateTime);
            parameters.Add("employeeTypeId", designation.EmployeeTypeId, DbType.Int32);

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    result = await connection.ExecuteAsync(query, parameters);
                }
                catch (Exception ex)
                {
                    // Handle exception (log it or throw)
                }
            }
            return result;
        }
        public async Task<int> UpdateSecondaryDesignation(Designation designation)
        {
            var query = "update SecondaryDesignations set UserId=@userId,SchoolId=@schoolId,RoleId=@roleId,GuestPkId=@guestPkId, IsActive=@isactive, DesignationName = @designationName,multiDesignation=@multiDesignation,Description=@description,updatedby = @updatedby, updateddate = @updateddate, EmployeeTypeId=@employeeTypeId where id = @id";
            var parameters = new DynamicParameters();
            parameters.Add("userId", designation.UserId, DbType.Int32);
            parameters.Add("schoolId", designation.SchoolId, DbType.Int32);
            parameters.Add("roleId", designation.RoleId, DbType.Int32);
            parameters.Add("guestPkId", designation.GuestPkId, DbType.Int32);
            parameters.Add("designationName", designation.DesignationName, DbType.String);
            parameters.Add("multiDesignation", designation.MultiDesignation, DbType.String);
            parameters.Add("description", designation.Description, DbType.String);
            parameters.Add("updatedby", designation.UpdatedBy, DbType.String);
            parameters.Add("updateddate", designation.UpdatedDate, DbType.DateTime);
            parameters.Add("employeeTypeId", designation.EmployeeTypeId, DbType.Int32);
            parameters.Add("isactive", designation.IsActive, DbType.Boolean);
            parameters.Add("id", designation.Id, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteScalarAsync<int>(query, parameters);
                return result;
            }
        }
        public async Task<int> RemoveSecondaryDesignation(int id)
        {
            var query = "update SecondaryDesignations set isactive = 0 where id = @id";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteScalarAsync<int>(query, new { id });
                return result;
            }
        }
    }
}
