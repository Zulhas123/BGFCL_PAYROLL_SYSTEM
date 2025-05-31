using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IBankTypeContract
    {
        public Task<List<BankType>> GetBankTypes();
        public Task<int> CreateBankType(BankType bankType);
        public Task<BankType> GetBankTypeById(int bankTypeId);
        public Task<int> UpdateBankType(BankType bankType);
        public Task<int> RemoveBankType(int id);
    }
}
