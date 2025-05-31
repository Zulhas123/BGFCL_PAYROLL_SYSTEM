using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IEmployeeTypeContract
    {
        public Task<List<EmployeeType>> GetEmployeeTypes();
        public Task<int> CreateEmployeeType(EmployeeType employeeType);
        public Task<EmployeeType> GetEmployeeTypeId(int id);
        public Task<int> UpdateEmployeeType(EmployeeType employeeType);
        public Task<int> DeleteEmployeeType(int id);
    }
}
