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
    public class BankTagRepository:IBankTagContract
    {

        private readonly BgfclContext _context;

        public BankTagRepository(BgfclContext context)
        {
            _context = context;
        }

        public async Task<int> CreateBankTag(BankTag bankTag)
        {
            int result = 0;
            var query = "INSERT INTO BankTags (BankTagName,Description,IsActive,CreatedBy,CreatedDate) VALUES (@bankTagName,@description,@isActive,@createdBy,@createdDate)";
            var parameters = new DynamicParameters();
            parameters.Add("bankTagName", bankTag.BankTagName, DbType.String);
            parameters.Add("description", bankTag.Description, DbType.String);
            parameters.Add("isActive", bankTag.IsActive, DbType.Boolean);
            parameters.Add("createdBy", bankTag.CreatedBy, DbType.String);
            parameters.Add("createdDate", bankTag.CreatedDate, DbType.DateTime);
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

        public async Task<List<BankTag>> GetBankTags()
        {
            var query = "SELECT * FROM BankTags where isactive in(0,1)";
            using (var connection = _context.CreateConnection())
            {
                var bankTags = await connection.QueryAsync<BankTag>(query);
                return bankTags.ToList();
            }
        }

        public async Task<BankTag> GetBankTagById(int bankTagid)
        {
            var query = "SELECT * FROM BankTags where id=@id";
            using (var connection = _context.CreateConnection())
            {
                var bankTag = await connection.QuerySingleOrDefaultAsync<BankTag>(query, new { id = bankTagid });
                return bankTag;
            }
        }

        public async Task<int> UpdateBankTag(BankTag bankTag)
        {
            var query = "update BankTags set IsActive=@isActive, BankTagName = @bankTagName,Description=@description,updatedby = @updatedby, updateddate = @updateddate where id = @id";
            var parameters = new DynamicParameters();
            parameters.Add("bankTagName", bankTag.BankTagName, DbType.String);
            parameters.Add("description", bankTag.Description, DbType.String);
            parameters.Add("isActive", bankTag.IsActive, DbType.Boolean);
            parameters.Add("updatedby", bankTag.UpdatedBy, DbType.String);
            parameters.Add("updateddate", bankTag.UpdatedDate, DbType.DateTime);
            parameters.Add("id", bankTag.Id, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteScalarAsync<int>(query, parameters);
                return result;
            }
        }

        public async Task<int> RemoveBankTag(int id)
        {
            var query = "update BankTags set isactive = 0 where id = @id";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteScalarAsync<int>(query, new { id });
                return result;
            }
        }
    }
}
