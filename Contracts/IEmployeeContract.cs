using Entities;
using Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IEmployeeContract
    {
        public Task<int> CreateEmployee(Employee employee);
        public Task<int> CreateEmployees(List<Employee> employees);

        public Task<List<EmployeeViewModel>> GetEmployees(int activeStatus, int employeeTypeId);
        public Task<List<EmployeeViewModel>> GetEmployeeswithFilter(int activeStatus, int employeeTypeId, int? schoolId, int? roleId, int? departmentId, int? designationId);
        public Task<List<EmployeeViewModel>> GetInactiveEmployees(int employeeTypeId);
        public Task<List<EmployeeViewModel>> GetEmployeesByEmployeeType(int employeeTypeId);
        public Task<Employee> GetEmployeeByjobCode(string jobcode);

        public Task<Employee> GetEmployeeById(int employeeId);

        public Task<EmployeeViewModel> GetEmployeeViewById(int employeeId);

        public Task<int> UpdateEmployee(Employee employee);
        public Task<int> UpdateInactiveEmployee(InactiveEmployeeOf employee);
        public Task<int> DeleteEmployee(int id);
        public Task<EmployeeViewModel> GetEmployeeForView(int id);
        public Task<EmployeeViewModel> GetEmployeeViewByJobCode(string jobCode);
        public  Task<List<Employee>> GetEmployeeCode();
        public Task<List<Employee>> GetAllEmployees();
        public Task<List<Employee>> GetAllEmployeesByType(int employeeTypeId);
        public Task<List<Employee>> GetAllOfficer();
        public  Task<string> GetLastPermanetEmployee();
        public Task<string> GetLastContractEmployee();
        public Task<List<Employee>> GetAllJuniorStaff();
        public Task<List<Employee>> GetAllPensionOFJS();

    }
}
