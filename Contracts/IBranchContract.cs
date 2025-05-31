using Entities;
using Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
	public interface IBranchContract
	{
		public Task<int> CreateBranch(Branch branch);

		public Task<List<BranchViewModel>> GetBranches();

		public Task<Branch> GetBranchesById(int branchId);

		public Task<List<Branch>> GetBranchesByBankId(int bankId);


        public Task<int> UpdateBranch(Branch branch);

		public Task<int> RemoveBranch(int id);
	}
}
