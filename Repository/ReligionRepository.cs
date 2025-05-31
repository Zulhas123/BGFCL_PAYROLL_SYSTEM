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
    public class ReligionRepository:IReligionContract
    {
        private readonly BgfclContext _context;

        public ReligionRepository(BgfclContext context)
        {
            _context = context;
        }
        public async Task<List<Religion>> GetReligions()
        {
            var query = "SELECT * FROM Religions";
            using (var connection = _context.CreateConnection())
            {
                var religions = await connection.QueryAsync<Religion>(query);
                return religions.ToList();
            }
        }
        public async Task<int> CreateReligion(Religion religion)
        {
            int result = 0;
            var query = "INSERT INTO Religions (SchoolId,RoleId,GuestPkId,ReligionName,Description,IsActive,CreatedBy,CreatedDate) VALUES (@schoolId,@roleId,@guestPkId,@religionName,@description,@isActive,@createdBy,@createdDate)";
            var parameters = new DynamicParameters();
            parameters.Add("schoolId", religion.SchoolId, DbType.Int32);
            parameters.Add("roleId", religion.RoleId, DbType.Int32);
            parameters.Add("guestPkId", religion.GuestPkId, DbType.Int32);
            parameters.Add("religionName", religion.ReligionName, DbType.String);
            parameters.Add("description", religion.Description, DbType.String);
            parameters.Add("isActive", religion.IsActive, DbType.Boolean);
            parameters.Add("createdBy", religion.CreatedBy, DbType.String);
            parameters.Add("createdDate", religion.CreatedDate, DbType.DateTime);
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
        public async Task<Religion> GetReligionById(int religionId)
        {
            var query = "SELECT * FROM Religions where  id=@id";
            using (var connection = _context.CreateConnection())
            {
                var religion = await connection.QuerySingleOrDefaultAsync<Religion>(query, new { id = religionId });
                return religion;
            }
        }
        public async Task<int> UpdateReligion(Religion religion)
        {
            var query = " UPDATE Religions SET SchoolId=@schoolId,RoleId=@roleId,GuestPkId=@guestPkId, IsActive = @isActive, ReligionName = @religionName, Description = @description, updatedby = @updatedby, updateddate = @updateddate WHERE id = @id";
            var parameters = new DynamicParameters();
            parameters.Add("schoolId", religion.SchoolId, DbType.Int32);
            parameters.Add("roleId", religion.RoleId, DbType.Int32);
            parameters.Add("guestPkId", religion.GuestPkId, DbType.Int32);
            parameters.Add("religionName", religion.ReligionName, DbType.String);
            parameters.Add("description", religion.Description, DbType.String);
            parameters.Add("isActive", religion.IsActive, DbType.Boolean);
            parameters.Add("updatedby", religion.UpdatedBy, DbType.String);
            parameters.Add("updateddate", religion.UpdatedDate, DbType.DateTime);
            parameters.Add("id", religion.Id, DbType.Int32);

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameters);
                return result; // This returns the number of rows affected
            }
        }
        public async Task<int> DeleteReligion(int id)
        {
            var query = "update Religions set isactive = 0 where id = @id";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteScalarAsync<int>(query, new { id });
                return result;
            }
        }
    }
}
