using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IAdvanceTaxContract
    {
        public Task<int> CreateAdvanceTax(AdvanceTax advanceTax);
        public Task<List<AdvanceTax>> GetAdvanceTaxes();
        public Task<AdvanceTax> GetAdvanceTaxByMonthId(int monthId);
        public Task<int> RemoveAdvanceTax(int id);
    }
}
