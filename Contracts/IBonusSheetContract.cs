using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IBonusSheetContract
    {
        public Task<List<BonusControlSheet>> GetBonusControlSheet(int monthid,int bonus, int employeeType, int? departmentId);
        public Task<List<BonusControlSheet>> GetBonusBankForward(int monthId, int bonus, int employeeType, string? bank);
    }
}
