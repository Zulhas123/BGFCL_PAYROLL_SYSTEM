using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IDesignationContract
    {
        public Task<int> CreateDesignation(Designation designation);

        public Task<List<Designation>> GetDesignations();
        public Task<List<Designation>> GetDesignationsByEmployeeType(int employeeTypeId);

        public Task<Designation> GetDesignationById(int designationId);

        public Task<int> UpdateDesignation(Designation designation);
        public Task<int> RemoveDesignation(int id);
        public Task<List<Designation>> GetSecondaryDesignations();
        public Task<Designation> GetSecondaryDesignationById(int designationId);
        public Task<int> CreateSecondaryDesignation(Designation designation);
        public Task<int> UpdateSecondaryDesignation(Designation designation);
        public  Task<int> RemoveSecondaryDesignation(int id);
    }
}
