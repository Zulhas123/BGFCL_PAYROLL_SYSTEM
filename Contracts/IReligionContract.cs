using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IReligionContract
    {
        public Task<List<Religion>> GetReligions();
        public Task<int> CreateReligion(Religion religion);
        public Task<Religion> GetReligionById(int religionId);
        public Task<int> UpdateReligion(Religion religion);
        public Task<int> DeleteReligion(int id);
    }
}
