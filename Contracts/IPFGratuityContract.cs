using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IPFGratuityContract
    {
        public  Task<List<ProvidentFundData>> GetProvidentFund();
        public Task<ProvidentFundData> GetProvidentFundById(int providentId);
        public Task<int> UpdateProvidentFund(ProvidentFundData provident);
    }
}
