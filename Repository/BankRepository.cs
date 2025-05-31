using Contracts;
using Dapper;
using Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.ViewModels;

namespace Repositories
{
    public class BankRepository:IBankContract
    {
        private readonly BgfclContext _context;

        public BankRepository(BgfclContext context)
        {
            _context = context;
        }

        public async Task<int> CreateBank(Bank bank)
        {
            int result = 0;
            var query = "INSERT INTO Banks (SchoolId,GuestPkId,RoleId,UserId,BankName,BankTypeId,IsActive,CreatedBy,CreatedDate) VALUES (@schoolId,@guestPkId,@roleId,@userId,@bankName,@bankTypeId,@isActive,@createdBy,@createdDate)";
            var parameters = new DynamicParameters();
            parameters.Add("schoolId", bank.SchoolId, DbType.Int32);
            parameters.Add("guestPkId", bank.GuestPkId, DbType.Int32);
            parameters.Add("roleId", bank.RoleId, DbType.Int32);
            parameters.Add("userId", bank.UserId, DbType.Int32);
            parameters.Add("bankName", bank.BankName, DbType.String);
            parameters.Add("bankTypeId", bank.BankTypeId, DbType.String);
            parameters.Add("isActive", bank.IsActive, DbType.Boolean);
            parameters.Add("createdBy", bank.CreatedBy, DbType.String);
            parameters.Add("createdDate", bank.CreatedDate, DbType.DateTime);
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

        public async Task<List<BankViewModel>> GetBanks()
        {
            var query = "select Banks.*, BankTypes.BankTypeName from Banks join BankTypes on Banks.BankTypeId = BankTypes.Id where Banks.IsActive in(0,1)";
            using (var connection = _context.CreateConnection())
            {
                var banks = await connection.QueryAsync<BankViewModel>(query);
                return banks.ToList();
            }
        }

        public async Task<Bank> GetBankById(int bankId)
        {
            var query = "SELECT * FROM Banks WHERE id=@id";
            using (var connection = _context.CreateConnection())
            {
                var bank = await connection.QuerySingleOrDefaultAsync<Bank>(query, new { id = bankId });
                return bank;
            }
        }


        public async Task<int> UpdateBank(Bank bank)
        {
            var query = "update Banks set IsActive=@isActive, SchoolId=@schoolId,GuestPkId=@guestPkId,RoleId=@roleId,UserId=@userId, BankName = @bankName,BankTagId=@bankTagId,BankTypeId=@bankTypeId,updatedby = @updatedby, updateddate = @updateddate where id = @id";
            var parameters = new DynamicParameters();
            parameters.Add("schoolId", bank.SchoolId, DbType.Int32);
            parameters.Add("guestPkId", bank.GuestPkId, DbType.Int32);
            parameters.Add("roleId", bank.RoleId, DbType.Int32);
            parameters.Add("userId", bank.UserId, DbType.Int32);
            parameters.Add("bankName", bank.BankName, DbType.String);
            parameters.Add("bankTagId", bank.BankTagId, DbType.Int32);
            parameters.Add("bankTypeId", bank.BankTypeId, DbType.Int32);
            parameters.Add("isActive", bank.IsActive, DbType.Boolean);
            parameters.Add("updatedby", bank.UpdatedBy, DbType.String);
            parameters.Add("updateddate", bank.UpdatedDate, DbType.DateTime);
            parameters.Add("id", bank.Id, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteScalarAsync<int>(query, parameters);
                return result;
            }
        }

        public async Task<int> RemoveBank(int id)
        {
            var query = "update Banks set isactive = 0 where id = @id";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteScalarAsync<int>(query, new { id });
                return result;
            }
        }
        // ************* Account **********
        public async Task<int> CreateAccount(BankAccounts account)
        {
            int result = 0;
            var query = "INSERT INTO BankAccounts (BankId,BranchId,AccountName,AccountNumber,AcountType,OpeningBalance,Notes,BankTypeId,IsActive,CreatedBy,CreatedDate) VALUES (@bankId,@branchId,@accountName,@accountNumber,@acountType,@openingBalance,@notes,@bankTypeId,@isActive,@createdBy,@createdDate)";
            var parameters = new DynamicParameters();
            parameters.Add("BankId", account.BankId, DbType.Int32);
            parameters.Add("BranchId", account.BranchId, DbType.Int32);
            parameters.Add("AccountName", account.AccountName, DbType.String);
            parameters.Add("AccountNumber", account.AccountNumber, DbType.String);
            parameters.Add("AcountType", account.AccountType, DbType.String);
            parameters.Add("OpeningBalance", account.OpeningBalance, DbType.Decimal);
            parameters.Add("Notes", account.Notes, DbType.String);
            parameters.Add("BankTypeId", account.BankTypeId, DbType.Int32);
            parameters.Add("isActive", account.IsActive, DbType.Boolean);
            parameters.Add("createdBy", account.CreatedBy, DbType.String);
            parameters.Add("createdDate", account.CreatedDate, DbType.DateTime);
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

        public async Task<List<BankAccountDto>> GetAccounts()
        {
            var query = @"
                SELECT 
                    a.Id,
                    a.AccountName,
                    a.AccountNumber,
                    a.OpeningBalance,
                    a.Notes,
                    b.id As BankId,
                    b.BankName,
                    br.Id As BranchId,
                    br.BranchName
                FROM BankAccounts AS a
                INNER JOIN Banks AS b ON a.BankId = b.Id
                INNER JOIN Branches AS br ON a.BranchId = br.Id
                WHERE a.IsActive = 1";

            using (var connection = _context.CreateConnection())
            {
                var accounts = await connection.QueryAsync<BankAccountDto>(query);
                return accounts.ToList();
            }
        }


        public async Task<BankAccounts> GetAccountById(int accountId)
        {
            var query = "SELECT * FROM BankAccounts WHERE id=@id";
            using (var connection = _context.CreateConnection())
            {
                var account = await connection.QuerySingleOrDefaultAsync<BankAccounts>(query, new { id = accountId });
                return account;
            }
        }


        public async Task<int> UpdateAccount(BankAccounts account)
        {
            var query = " update BankAccounts set IsActive=@isActive, BankId=@bankId,BranchId=@branchId,AccountName=@accountName,AccountNumber=@accountNumber, AcountType = @acountType,OpeningBalance=@openingBalance,Notes=@notes,BankTypeId=@bankTypeId,updatedby = @updatedby, updateddate = @updateddate where id = @id";
            var parameters = new DynamicParameters();
            parameters.Add("BankId", account.BankId, DbType.Int32);
            parameters.Add("BranchId", account.BranchId, DbType.Int32);
            parameters.Add("AccountName", account.AccountName, DbType.String);
            parameters.Add("AccountNumber", account.AccountNumber, DbType.String);
            parameters.Add("AcountType", account.AccountType, DbType.String);
            parameters.Add("OpeningBalance", account.OpeningBalance, DbType.Decimal);
            parameters.Add("Notes", account.Notes, DbType.String);
            parameters.Add("BankTypeId", account.BankTypeId, DbType.Int32);
            parameters.Add("isActive", account.IsActive, DbType.Boolean);
            parameters.Add("updatedBy", account.CreatedBy, DbType.String);
            parameters.Add("updatedDate", account.CreatedDate, DbType.DateTime);
            parameters.Add("id", account.Id, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteScalarAsync<int>(query, parameters);
                return result;
            }
        }

        public async Task<int> RemoveAccount(int id)
        {
            var query = "update BankAccounts set isactive = 0 where id = @id";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteScalarAsync<int>(query, new { id });
                return result;
            }
        }
    }
}
