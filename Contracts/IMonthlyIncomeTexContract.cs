using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IMonthlyIncomeTexContract
    {
        public Task<List<MonthlyIncomeTex>> GetMonthlyIncomeTex(int monthId, string? department, string? designation);
    }
}
