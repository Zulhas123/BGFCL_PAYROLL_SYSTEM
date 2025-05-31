using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IBankTagContract
    {
        public Task<int> CreateBankTag(BankTag bankTag);

        public Task<List<BankTag>> GetBankTags();

        public Task<BankTag> GetBankTagById(int bankTagId);

        public Task<int> UpdateBankTag(BankTag bankTag);

        public Task<int> RemoveBankTag(int id);
    }
}
