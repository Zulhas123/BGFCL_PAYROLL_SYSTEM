using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IRolesContract
    {
        public Task<List<Roles>> GetRole();

        public Task<Roles> GetRoleById(int id);
        public Task<int> CreateRole(Roles role);
        public Task<int> UpdateRole(Roles role);
        public Task<int> DeleteRole(int id);
    }
}
