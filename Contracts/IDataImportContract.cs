using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IDataImportContract
    {
        public Task<int> ImportEmployeeData(EmployeeImportData employeeData);
        public Task<int> ImportDepartmentData(DepartmentImportData departmentData);
        public Task<int> ImportDesignationData(DesignationImportData designationData);
        public Task<int> ImportRoleData(Roles role);
    }
}
