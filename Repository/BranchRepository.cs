using Contracts;
using Dapper;
using Entities.ViewModels;
using Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
	public class BranchRepository:IBranchContract
	{
		private readonly BgfclContext _context;

		public BranchRepository(BgfclContext context)
		{
			_context = context;
		}

		public async Task<int> CreateBranch(Branch branch)
		{
			int result = 0;
			var query = "INSERT INTO Branches (SchoolId,GuestPkId,RoleId,UserId,BranchName,RoutingNumber,BankId,IsActive,CreatedBy,CreatedDate,Address) VALUES (@schoolId,@guestPkId,@roleId,@userId,@branchName,@routingNumber,@bankId,@isActive,@createdBy,@createdDate,@address)";
			var parameters = new DynamicParameters();
            parameters.Add("schoolId", branch.SchoolId, DbType.Int32);
            parameters.Add("guestPkId", branch.GuestPkId, DbType.Int32);
            parameters.Add("roleId", branch.RoleId, DbType.Int32);
            parameters.Add("userId", branch.UserId, DbType.Int32);
            parameters.Add("branchName", branch.BranchName, DbType.String);
			parameters.Add("routingNumber", branch.RoutingNumber, DbType.String);
			parameters.Add("bankId", branch.BankId, DbType.Int32);
			parameters.Add("isActive", branch.IsActive, DbType.Boolean);
			parameters.Add("createdBy", branch.CreatedBy, DbType.String);
			parameters.Add("createdDate", branch.CreatedDate, DbType.DateTime);
            parameters.Add("address", branch.Address, DbType.String);
            using (var connection = _context.CreateConnection())
			{
				try
				{
					result = await connection.ExecuteAsync(query, parameters);
				}
				catch (Exception ex)
				{
					throw new Exception();
				}
			}
			return result;
		}

		public async Task<List<BranchViewModel>> GetBranches()
		{
			var query = "select Branches.Id, Branches.BranchName, Banks.BankName, Branches.RoutingNumber,Branches.IsActive, Branches.Address from Branches join Banks on Branches.BankId = Banks.Id  where Branches.IsActive=1";
			using (var connection = _context.CreateConnection())
			{
				var branches = await connection.QueryAsync<BranchViewModel>(query);
				return branches.ToList();
			}
		}

		public async Task<Branch> GetBranchesById(int branchId)
		{
			var query = "SELECT * FROM Branches where isactive=1 and id=@id";
			using (var connection = _context.CreateConnection())
			{
				var branch = await connection.QuerySingleOrDefaultAsync<Branch>(query, new { id = branchId });
				return branch;
			}
		}

        public async Task<List<Branch>> GetBranchesByBankId(int bankId)
        {
			IEnumerable<Branch> branches = null;
            var query = "SELECT * FROM Branches where isactive=1 and BankId=@bankId";
			try
			{
                using (var connection = _context.CreateConnection())
                {
                     branches = await connection.QueryAsync<Branch>(query, new { bankId });
                }
            }
			catch (Exception ex)
			{

			}
            return branches.ToList();
        }

        public async Task<int> UpdateBranch(Branch branch)
		{
			var query = "update Branches set SchoolId = @schoolId,GuestPkId=@guestPkId,RoleId=@roleId,UserId=@userId,BranchName = @branchName,RoutingNumber=@routingNumber,BankId=@bankId,updatedby = @updatedby, updateddate = @updateddate, Address = @address where id = @id";
			var parameters = new DynamicParameters();
            parameters.Add("schoolId", branch.SchoolId, DbType.Int32);
            parameters.Add("guestPkId", branch.GuestPkId, DbType.Int32);
            parameters.Add("roleId", branch.RoleId, DbType.Int32);
            parameters.Add("userId", branch.UserId, DbType.Int32);
            parameters.Add("branchName", branch.BranchName, DbType.String);
			parameters.Add("routingNumber", branch.RoutingNumber, DbType.String);
			parameters.Add("bankId", branch.BankId, DbType.Int32);
			parameters.Add("updatedby", branch.UpdatedBy, DbType.String);
			parameters.Add("updateddate", branch.UpdatedDate, DbType.DateTime);
            parameters.Add("address", branch.Address, DbType.String);
            parameters.Add("id", branch.Id, DbType.Int32);
			using (var connection = _context.CreateConnection())
			{
                var result = await connection.ExecuteAsync(query, parameters);
                return result;
			}
		}

		public async Task<int> RemoveBranch(int id)
		{
			var query = "update Branches set isactive = 0 where id = @id";
			using (var connection = _context.CreateConnection())
			{
				var result = await connection.ExecuteScalarAsync<int>(query, new { id });
				return result;
			}
		}
	}
}
