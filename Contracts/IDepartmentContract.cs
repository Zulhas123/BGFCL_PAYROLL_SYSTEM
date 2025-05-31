using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace Contracts
{
    public interface IDepartmentContract
    {
        public Task<int> CreateDepartment(Department department);

        public Task<List<Department>> GetDepartments();

        public Task<Department> GetDepartmentById(int departmentId);
        public Task<Department> GetDepartmentByJournalCode(string journalCode);

        public Task<int> UpdateDepartment(Department department);

        public Task<int> RemoveDepartment(int id);
    }
}
