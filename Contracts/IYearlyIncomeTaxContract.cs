using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IYearlyIncomeTaxContract
    {
        public Task<List<YearlyIncomeTax>> GetYearlyIncomeTex(int fromMonthId, int toMonthId, string? department, string? designation);
    }
}
