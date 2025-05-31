using Contracts;
using Dapper;
using Entities;
using Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class BankTypeRepository:IBankTypeContract
    {
        private readonly BgfclContext _context;

        public BankTypeRepository(BgfclContext context)
        {
            _context = context;
        }
        public async Task<List<BankType>> GetBankTypes()
        {
            var query = "SELECT * FROM BankTypes";
            using (var connection = _context.CreateConnection())
            {
                var bankTypes = await connection.QueryAsync<BankType>(query);
                return bankTypes.ToList();
            }
        }

        public async Task<int> CreateBankType(BankType bankType)
        {
            int result = 0;
            var query = "INSERT INTO BankTypes (SchoolId,GuestPkId,RoleId,UserId,BankTypeName,Description,IsActive,CreatedBy,CreatedDate) VALUES (@schoolId,@guestPkId,@roleId,@userId,@bankTypeName,@description,@isActive,@createdBy,@createdDate)";
            var parameters = new DynamicParameters();
            parameters.Add("schoolId", bankType.SchoolId, DbType.Int32);
            parameters.Add("guestPkId", bankType.GuestPkId, DbType.Int32);
            parameters.Add("roleId", bankType.RoleId, DbType.Int32);
            parameters.Add("userId", bankType.UserId, DbType.Int32);
            parameters.Add("bankTypeName", bankType.BankTypeName, DbType.String);
            parameters.Add("description", bankType.Description, DbType.String);
            parameters.Add("isActive", bankType.IsActive, DbType.Boolean);
            parameters.Add("createdBy", bankType.CreatedBy, DbType.String);
            parameters.Add("createdDate", bankType.CreatedDate, DbType.DateTime);
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

        public async Task<BankType> GetBankTypeById(int bankTypeId)
        {
            var query = "SELECT * FROM BankTypes WHERE id=@id";
            using (var connection = _context.CreateConnection())
            {
                var bankType = await connection.QuerySingleOrDefaultAsync<BankType>(query, new { id = bankTypeId });
                return bankType;
            }
        }


        public async Task<int> UpdateBankType(BankType bankType)
        {
            var query = "update BankTypes set IsActive=@isActive, SchoolId=@schoolId,GuestPkId=@guestPkId,RoleId=@roleId,UserId=@userId,Description=@description, BankTypeName = @bankTypeName,updatedby = @updatedby, updateddate = @updateddate where id = @id";
            var parameters = new DynamicParameters();
            parameters.Add("schoolId", bankType.SchoolId, DbType.Int32);
            parameters.Add("guestPkId", bankType.GuestPkId, DbType.Int32);
            parameters.Add("roleId", bankType.RoleId, DbType.Int32);
            parameters.Add("userId", bankType.UserId, DbType.Int32);
            parameters.Add("bankTypeName", bankType.BankTypeName, DbType.String);
            parameters.Add("description", bankType.Description, DbType.String);
            parameters.Add("isActive", bankType.IsActive, DbType.Boolean);
            parameters.Add("updatedby", bankType.UpdatedBy, DbType.String);
            parameters.Add("updateddate", bankType.UpdatedDate, DbType.DateTime);
            parameters.Add("id", bankType.Id, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteScalarAsync<int>(query, parameters);
                return result;
            }
        }

        public async Task<int> RemoveBankType(int id)
        {
            var query = "update BankTypes set isactive = 0 where id = @id";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteScalarAsync<int>(query, new { id });
                return result;
            }
        }


    }
}
