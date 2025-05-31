using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ISchoolContract
    {
        public Task<int> CreateSchool(Schools school);

        public Task<int> DeleteSchool(int id);
        public Task<List<Schools>> GetSchools();
        public Task<Schools> GetSchoolById(int id);
        public Task<int> UpdateSchool(Schools school);
    }
}
