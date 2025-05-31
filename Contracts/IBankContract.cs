using Entities;
using Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IBankContract
    {
        public Task<int> CreateBank(Bank bank);

        public Task<List<BankViewModel>> GetBanks();

        public Task<Bank> GetBankById(int bankId);

        public Task<int> UpdateBank(Bank bank);

        public Task<int> RemoveBank(int id);
        // Account
        public Task<int> CreateAccount(BankAccounts account);
        public Task<List<BankAccountDto>> GetAccounts();
        public Task<BankAccounts> GetAccountById(int accountId);
        public Task<int> UpdateAccount(BankAccounts account);
        public Task<int> RemoveAccount(int id);
    }
}
