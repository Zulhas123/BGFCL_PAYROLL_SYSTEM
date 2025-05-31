using BgfclApp.ViewModels;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BgfclApp.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BanksController : ControllerBase
    {
        private IBankTagContract _bankTagContract;
        private IBranchContract _branchContract;
        private IBankContract _bankContract;
        private IBankTypeContract _bankTypeContract;
        private ResponseViewModel _responseViewModel;

        public BanksController(IBankTagContract bankTagContract, ResponseViewModel responseViewModel,IBankContract bankContract,IBankTypeContract bankTypeContract,IBranchContract branchContract)
        {
            _bankTagContract = bankTagContract;
            _responseViewModel = responseViewModel;
            _bankContract = bankContract;
            _bankTypeContract = bankTypeContract;
            _branchContract = branchContract;
        }


        #region Bank Tag

        [HttpGet]
        public async Task<IActionResult> GetBankTags()
        {
            try
            {
                var bankTags = await _bankTagContract.GetBankTags();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = bankTags;
                return Ok(_responseViewModel);
            }
            catch (Exception ex)
            {
                _responseViewModel.StatusCode = 500;
                _responseViewModel.ResponseMessage = "Something went wrong";
                return Ok(_responseViewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBankTagById(int id)
        {

            try
            {
                var bankTag = await _bankTagContract.GetBankTagById(id);
                if (bankTag == null)
                {
                    _responseViewModel.StatusCode = 404;
                    _responseViewModel.ResponseMessage = "Data not found";
                    return Ok(_responseViewModel);
                }
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = bankTag;
                return Ok(_responseViewModel);
            }
            catch (Exception ex)
            {
                _responseViewModel.StatusCode = 500;
                _responseViewModel.ResponseMessage = "Something went wrong";
                return Ok(_responseViewModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateBankTag([FromForm] BankTag bankTag)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (String.IsNullOrEmpty(bankTag.BankTagName))
                {
                    _responseViewModel.Errors.Add(nameof(bankTag.BankTagName) + "Error", "Bank Tag name required");
                }
                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                var existingBankTags = await _bankTagContract.GetBankTags();
                var existingBankTag= existingBankTags.Where(b => b.BankTagName == bankTag.BankTagName).SingleOrDefault();
                if (existingBankTag == null)
                {
                    bankTag.CreatedBy = "";
                    bankTag.CreatedDate = DateTime.Now;
                    bankTag.IsActive = true;
                    int result = await _bankTagContract.CreateBankTag(bankTag);

                    _responseViewModel.ResponseMessage = "Record saved successfully";
                    _responseViewModel.StatusCode = 201;
                    return Ok(_responseViewModel);
                }
                else
                {
                    _responseViewModel.ResponseMessage = "Record already exists";
                    _responseViewModel.Errors.Add(nameof(bankTag.BankTagName) + "Error", "Bank Tag name already exists");
                    _responseViewModel.StatusCode = 409;
                    return Ok(_responseViewModel);
                }


            }
            catch (Exception ex)
            {
                _responseViewModel.ResponseMessage = ex.Message;
                _responseViewModel.StatusCode = 500;
                return Ok(_responseViewModel);
            }
        }


        [HttpPut]
        public async Task<IActionResult> UpdateBankTag([FromForm] BankTag bankTag)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (String.IsNullOrEmpty(bankTag.BankTagName))
                {
                    _responseViewModel.Errors.Add(nameof(bankTag.BankTagName) + "Error", "Bank Tag name required");
                }
                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                var existingBankTags = await _bankTagContract.GetBankTags();
                var existingBankTag = existingBankTags.Where(b => b.Id == bankTag.Id).SingleOrDefault();

                existingBankTag.UpdatedBy = "";
                existingBankTag.UpdatedDate = DateTime.Now;
                existingBankTag.BankTagName = bankTag.BankTagName;
                existingBankTag.Description = bankTag.Description;
                existingBankTag.IsActive=bankTag.IsActive;

                int result = await _bankTagContract.UpdateBankTag(existingBankTag);

                _responseViewModel.ResponseMessage = "Record updated successfully";
                _responseViewModel.StatusCode = 201;
                return Ok(_responseViewModel);

            }
            catch (Exception ex)
            {
                _responseViewModel.ResponseMessage = "Something went wrong";
                _responseViewModel.StatusCode = 500;
                return Ok(_responseViewModel);
            }
        }


        [HttpDelete]
        public async Task<IActionResult> RemoveBankTag(int id)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                var existingBankTags = await _bankTagContract.GetBankTags();
                var existingBankTag = existingBankTags.Where(b => b.Id == id).SingleOrDefault();

                if (existingBankTag == null)
                {
                    _responseViewModel.ResponseMessage = "Record not found";
                    _responseViewModel.StatusCode = 404;
                    return Ok(_responseViewModel);
                }

                int result = await _bankTagContract.RemoveBankTag(existingBankTag.Id);

                _responseViewModel.ResponseMessage = "Record removed successfully";
                _responseViewModel.StatusCode = 200;
                return Ok(_responseViewModel);

            }
            catch (Exception ex)
            {
                _responseViewModel.ResponseMessage = ex.Message;
                _responseViewModel.StatusCode = 500;
                return Ok(_responseViewModel);
            }
        }

        #endregion

        #region Bank
        [HttpPost]
        public async Task<IActionResult> CreateBank([FromForm] Bank bank)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (String.IsNullOrEmpty(bank.BankName))
                {
                    _responseViewModel.Errors.Add(nameof(bank.BankName) + "Error", "Bank name required");
                }
                //if (bank.BankTagId == 0)
                //{
                //    _responseViewModel.Errors.Add(nameof(bank.BankTagId) + "Error", "Bank Tag required");
                //}
                if (bank.BankTypeId == 0)
                {
                    _responseViewModel.Errors.Add(nameof(bank.BankTypeId) + "Error", "Bank Type required");
                }
                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                var existingBanks = await _bankContract.GetBanks();
                var existingBank = existingBanks.Where(b => b.BankName == bank.BankName).SingleOrDefault();
                if (existingBank == null)
                {
                    bank.CreatedBy = "";
                    bank.CreatedDate = DateTime.Now;
                    bank.IsActive = true;
                    int result = await _bankContract.CreateBank(bank);

                    _responseViewModel.ResponseMessage = "Record saved successfully";
                    _responseViewModel.StatusCode = 201;
                    return Ok(_responseViewModel);
                }
                else
                {
                    _responseViewModel.ResponseMessage = "Record already exists";
                    _responseViewModel.Errors.Add(nameof(bank.BankName) + "Error", "Bank name already exists");
                    _responseViewModel.StatusCode = 409;
                    return Ok(_responseViewModel);
                }


            }
            catch (Exception ex)
            {
                _responseViewModel.ResponseMessage = ex.Message;
                _responseViewModel.StatusCode = 500;
                return Ok(_responseViewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBanks()
        {
            try
            {
                var banks = await _bankContract.GetBanks();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = banks;
                return Ok(_responseViewModel);
            }
            catch (Exception ex)
            {
                _responseViewModel.StatusCode = 500;
                _responseViewModel.ResponseMessage = "Something went wrong";
                return Ok(_responseViewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBankById(int id)
        {

            try
            {
                var bank = await _bankContract.GetBankById(id);
                if (bank == null)
                {
                    _responseViewModel.StatusCode = 404;
                    _responseViewModel.ResponseMessage = "Data not found";
                    return Ok(_responseViewModel);
                }
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = bank;
                return Ok(_responseViewModel);
            }
            catch (Exception ex)
            {
                _responseViewModel.StatusCode = 500;
                _responseViewModel.ResponseMessage = "Something went wrong";
                return Ok(_responseViewModel);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateBank([FromForm] Bank bank)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (String.IsNullOrEmpty(bank.BankName))
                {
                    _responseViewModel.Errors.Add(nameof(bank.BankName) + "Error", "Bank name required");
                }
                //if (bank.BankTagId == 0)
                //{
                //    _responseViewModel.Errors.Add(nameof(bank.BankTagId) + "Error", "Bank Tag required");
                //}
                if (bank.BankTypeId == 0)
                {
                    _responseViewModel.Errors.Add(nameof(bank.BankTypeId) + "Error", "Bank Type required");
                }
                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                var existingBank = await _bankContract.GetBankById(bank.Id);

                existingBank.UpdatedBy = "";
                existingBank.UpdatedDate = DateTime.Now;
                existingBank.BankName = bank.BankName;
                existingBank.BankTagId = bank.BankTagId;
                existingBank.BankTypeId = bank.BankTypeId;
                existingBank.SchoolId = bank.SchoolId;
                existingBank.UserId = bank.UserId;
                existingBank.RoleId = bank.RoleId;
                existingBank.GuestPkId = bank.GuestPkId;
                existingBank.IsActive = bank.IsActive;

                int result = await _bankContract.UpdateBank(existingBank);

                _responseViewModel.ResponseMessage = "Record updated successfully";
                _responseViewModel.StatusCode = 201;
                return Ok(_responseViewModel);

            }
            catch (Exception ex)
            {
                _responseViewModel.ResponseMessage = "Something went wrong";
                _responseViewModel.StatusCode = 500;
                return Ok(_responseViewModel);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveBank(int id)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                var existingBank = await _bankContract.GetBankById(id);

                if (existingBank == null)
                {
                    _responseViewModel.ResponseMessage = "Record not found";
                    _responseViewModel.StatusCode = 404;
                    return Ok(_responseViewModel);
                }

                int result = await _bankContract.RemoveBank(existingBank.Id);

                _responseViewModel.ResponseMessage = "Record removed successfully";
                _responseViewModel.StatusCode = 200;
                return Ok(_responseViewModel);

            }
            catch (Exception ex)
            {
                _responseViewModel.ResponseMessage = ex.Message;
                _responseViewModel.StatusCode = 500;
                return Ok(_responseViewModel);
            }
        }

        #endregion

        #region Bank Type
        [HttpGet]
        public async Task<IActionResult> GetBankTypes()
        {
            try
            {
                var bankTypes = await _bankTypeContract.GetBankTypes();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = bankTypes;
                return Ok(_responseViewModel);
            }
            catch (Exception ex)
            {
                _responseViewModel.StatusCode = 500;
                _responseViewModel.ResponseMessage = "Something went wrong";
                return Ok(_responseViewModel);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateBankType([FromForm] BankType bankType)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (String.IsNullOrEmpty(bankType.BankTypeName))
                {
                    _responseViewModel.Errors.Add(nameof(bankType.BankTypeName) + "Error", "Bank Type  required");
                }
                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                var existingBankTypes = await _bankTypeContract.GetBankTypes();
                var existingBankType = existingBankTypes.Where(b => b.BankTypeName == bankType.BankTypeName).SingleOrDefault();
                if (existingBankType == null)
                {
                    bankType.CreatedBy = "";
                    bankType.CreatedDate = DateTime.Now;
                    bankType.IsActive = true;
                    int result = await _bankTypeContract.CreateBankType(bankType);

                    _responseViewModel.ResponseMessage = "Record saved successfully";
                    _responseViewModel.StatusCode = 201;
                    return Ok(_responseViewModel);
                }
                else
                {
                    _responseViewModel.ResponseMessage = "Record already exists";
                    _responseViewModel.Errors.Add(nameof(bankType.BankTypeName) + "Error", "Bank Type already exists");
                    _responseViewModel.StatusCode = 409;
                    return Ok(_responseViewModel);
                }


            }
            catch (Exception ex)
            {
                _responseViewModel.ResponseMessage = ex.Message;
                _responseViewModel.StatusCode = 500;
                return Ok(_responseViewModel);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetBankTypeById(int id)
        {

            try
            {
                var bank = await _bankTypeContract.GetBankTypeById(id);
                if (bank == null)
                {
                    _responseViewModel.StatusCode = 404;
                    _responseViewModel.ResponseMessage = "Data not found";
                    return Ok(_responseViewModel);
                }
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = bank;
                return Ok(_responseViewModel);
            }
            catch (Exception ex)
            {
                _responseViewModel.StatusCode = 500;
                _responseViewModel.ResponseMessage = "Something went wrong";
                return Ok(_responseViewModel);
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateBankType([FromForm] BankType bankType)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (String.IsNullOrEmpty(bankType.BankTypeName))
                {
                    _responseViewModel.Errors.Add(nameof(bankType.BankTypeName) + "Error", "Bank Type required");
                }
                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                var existingBankType = await _bankTypeContract.GetBankTypeById(bankType.Id);

                existingBankType.UpdatedBy = "";
                existingBankType.UpdatedDate = DateTime.Now;
                existingBankType.BankTypeName = bankType.BankTypeName;
                existingBankType.SchoolId = bankType.SchoolId;
                existingBankType.UserId = bankType.UserId;
                existingBankType.RoleId = bankType.RoleId;
                existingBankType.GuestPkId = bankType.GuestPkId;
                existingBankType.IsActive = bankType.IsActive;

                int result = await _bankTypeContract.UpdateBankType(existingBankType);

                _responseViewModel.ResponseMessage = "Record updated successfully";
                _responseViewModel.StatusCode = 201;
                return Ok(_responseViewModel);

            }
            catch (Exception ex)
            {
                _responseViewModel.ResponseMessage = "Something went wrong";
                _responseViewModel.StatusCode = 500;
                return Ok(_responseViewModel);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveBankType(int id)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                var existingBank = await _bankTypeContract.GetBankTypeById(id);

                if (existingBank == null)
                {
                    _responseViewModel.ResponseMessage = "Record not found";
                    _responseViewModel.StatusCode = 404;
                    return Ok(_responseViewModel);
                }

                int result = await _bankTypeContract.RemoveBankType(existingBank.Id);

                _responseViewModel.ResponseMessage = "Record removed successfully";
                _responseViewModel.StatusCode = 200;
                return Ok(_responseViewModel);

            }
            catch (Exception ex)
            {
                _responseViewModel.ResponseMessage = ex.Message;
                _responseViewModel.StatusCode = 500;
                return Ok(_responseViewModel);
            }
        }
        #endregion

        #region Branch
        [HttpPost]
        public async Task<IActionResult> CreateBranch([FromForm] Branch branch)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (String.IsNullOrEmpty(branch.BranchName))
                {
                    _responseViewModel.Errors.Add(nameof(branch.BranchName) + "Error", "Branch name required");
                }
                if (branch.BankId == 0)
                {
                    _responseViewModel.Errors.Add(nameof(branch.BankId) + "Error", "Bank required");
                }
                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                var existingBranches = await _branchContract.GetBranches();
                var existingBank = existingBranches.Where(b => b.BranchName == branch.BranchName && b.BankId==branch.BankId).SingleOrDefault();
                if (existingBank == null)
                {
                    branch.CreatedBy = "";
                    branch.CreatedDate = DateTime.Now;
                    branch.IsActive = true;
                    int result = await _branchContract.CreateBranch(branch);

                    _responseViewModel.ResponseMessage = "Record saved successfully";
                    _responseViewModel.StatusCode = 201;
                    return Ok(_responseViewModel);
                }
                else
                {
                    _responseViewModel.ResponseMessage = "Record already exists";
                    _responseViewModel.Errors.Add(nameof(branch.BranchName) + "Error", "Branch already exists");
                    _responseViewModel.StatusCode = 409;
                    return Ok(_responseViewModel);
                }


            }
            catch (Exception ex)
            {
                _responseViewModel.ResponseMessage = "Something went wrong";
                _responseViewModel.StatusCode = 500;
                return Ok(_responseViewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBranches()
        {
            try
            {
                var branches = await _branchContract.GetBranches();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = branches;
                return Ok(_responseViewModel);
            }
            catch (Exception ex)
            {
                _responseViewModel.StatusCode = 500;
                _responseViewModel.ResponseMessage = "Something went wrong";
                return Ok(_responseViewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBranchById(int id)
        {

            try
            {
                var branch = await _branchContract.GetBranchesById(id);
                if (branch == null)
                {
                    _responseViewModel.StatusCode = 404;
                    _responseViewModel.ResponseMessage = "Data not found";
                    return Ok(_responseViewModel);
                }
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = branch;
                return Ok(_responseViewModel);
            }
            catch (Exception ex)
            {
                _responseViewModel.StatusCode = 500;
                _responseViewModel.ResponseMessage = "Something went wrong";
                return Ok(_responseViewModel);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetBranchesByBankId(int bankId)
        {

            try
            {
                var branches = await _branchContract.GetBranchesByBankId(bankId);
                if (branches == null)
                {
                    _responseViewModel.StatusCode = 404;
                    _responseViewModel.ResponseMessage = "Data not found";
                    return Ok(_responseViewModel);
                }
                else
                {
                    _responseViewModel.StatusCode = 200;
                    _responseViewModel.Data = branches;
                    return Ok(_responseViewModel);
                }

            }
            catch (Exception ex)
            {
                _responseViewModel.StatusCode = 500;
                _responseViewModel.ResponseMessage = "Something went wrong";
                return Ok(_responseViewModel);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateBranch([FromForm] Branch branch)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (String.IsNullOrEmpty(branch.BranchName))
                {
                    _responseViewModel.Errors.Add(nameof(branch.BranchName) + "Error", "Branch name required");
                }
                if (branch.BankId == 0)
                {
                    _responseViewModel.Errors.Add(nameof(branch.BankId) + "Error", "Bank required");
                }

                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                var existingBranch = await _branchContract.GetBranchesById(branch.Id);

                existingBranch.UpdatedBy = "";
                existingBranch.UpdatedDate = DateTime.Now;
                existingBranch.BranchName = branch.BranchName;
                existingBranch.RoutingNumber = branch.RoutingNumber;
                existingBranch.BankId = branch.BankId;
                existingBranch.Address = branch.Address;

                int result = await _branchContract.UpdateBranch(existingBranch);

                _responseViewModel.ResponseMessage = "Record updated successfully";
                _responseViewModel.StatusCode = 201;
                return Ok(_responseViewModel);

            }
            catch (Exception ex)
            {
                _responseViewModel.ResponseMessage = "Something went wrong";
                _responseViewModel.StatusCode = 500;
                return Ok(_responseViewModel);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveBranch(int id)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                var existingBranch = await _branchContract.GetBranchesById(id);

                if (existingBranch == null)
                {
                    _responseViewModel.ResponseMessage = "Record not found";
                    _responseViewModel.StatusCode = 404;
                    return Ok(_responseViewModel);
                }

                int result = await _branchContract.RemoveBranch(existingBranch.Id);

                _responseViewModel.ResponseMessage = "Record removed successfully";
                _responseViewModel.StatusCode = 200;
                return Ok(_responseViewModel);

            }
            catch (Exception ex)
            {
                _responseViewModel.ResponseMessage = ex.Message;
                _responseViewModel.StatusCode = 500;
                return Ok(_responseViewModel);
            }
        }
        #endregion
        // Account ***********
        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromForm] BankAccounts account)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (String.IsNullOrEmpty(account.AccountName))
                {
                    _responseViewModel.Errors.Add(nameof(account.AccountName) + "Error", "Account name required");
                }
                if (account.BankId == 0)
                {
                    _responseViewModel.Errors.Add(nameof(account.BankId) + "Error", "Bank required");
                }
                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                var existingAccounts = await _bankContract.GetAccounts();
                var existingAccount = existingAccounts.Where(b => b.AccountName == account.AccountName && b.BankId == account.BankId).SingleOrDefault();
                if (existingAccount == null)
                {
                    account.CreatedBy = "";
                    account.CreatedDate = DateTime.Now;
                    account.IsActive = true;
                    int result = await _bankContract.CreateAccount(account);

                    _responseViewModel.ResponseMessage = "Record saved successfully";
                    _responseViewModel.StatusCode = 201;
                    return Ok(_responseViewModel);
                }
                else
                {
                    _responseViewModel.ResponseMessage = "Record already exists";
                    _responseViewModel.Errors.Add(nameof(account.AccountName) + "Error", "Account already exists");
                    _responseViewModel.StatusCode = 409;
                    return Ok(_responseViewModel);
                }


            }
            catch (Exception ex)
            {
                _responseViewModel.ResponseMessage = "Something went wrong";
                _responseViewModel.StatusCode = 500;
                return Ok(_responseViewModel);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetAccounts()
        {
            try
            {
                var accounts = await _bankContract.GetAccounts();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = accounts;
                return Ok(_responseViewModel);
            }
            catch (Exception ex)
            {
                _responseViewModel.StatusCode = 500;
                _responseViewModel.ResponseMessage = "Something went wrong";
                return Ok(_responseViewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAccountById(int id)
        {

            try
            {
                var branch = await _bankContract.GetAccountById(id);
                if (branch == null)
                {
                    _responseViewModel.StatusCode = 404;
                    _responseViewModel.ResponseMessage = "Data not found";
                    return Ok(_responseViewModel);
                }
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = branch;
                return Ok(_responseViewModel);
            }
            catch (Exception ex)
            {
                _responseViewModel.StatusCode = 500;
                _responseViewModel.ResponseMessage = "Something went wrong";
                return Ok(_responseViewModel);
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateAccount([FromForm] BankAccounts account)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (String.IsNullOrEmpty(account.AccountName))
                {
                    _responseViewModel.Errors.Add(nameof(account.AccountName) + "Error", "Account name required");
                }
                if (account.BankId == 0)
                {
                    _responseViewModel.Errors.Add(nameof(account.BankId) + "Error", "Bank required");
                }

                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                var existingAccount = await _bankContract.GetAccountById(account.Id);

                existingAccount.UpdatedBy = "";
                existingAccount.UpdatedDate = DateTime.Now;
                existingAccount.AccountName = account.AccountName;
                existingAccount.AccountNumber = account.AccountNumber;
                existingAccount.BankId = account.BankId;
                existingAccount.BranchId = account.BranchId;
                existingAccount.Notes = account.Notes;

                int result = await _bankContract.UpdateAccount(existingAccount);

                _responseViewModel.ResponseMessage = "Record updated successfully";
                _responseViewModel.StatusCode = 201;
                return Ok(_responseViewModel);

            }
            catch (Exception ex)
            {
                _responseViewModel.ResponseMessage = "Something went wrong";
                _responseViewModel.StatusCode = 500;
                return Ok(_responseViewModel);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveAccount(int id)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                var existingAccount = await _bankContract.GetAccountById(id);

                if (existingAccount == null)
                {
                    _responseViewModel.ResponseMessage = "Record not found";
                    _responseViewModel.StatusCode = 404;
                    return Ok(_responseViewModel);
                }

                int result = await _bankContract.RemoveAccount(existingAccount.Id);

                _responseViewModel.ResponseMessage = "Record removed successfully";
                _responseViewModel.StatusCode = 200;
                return Ok(_responseViewModel);

            }
            catch (Exception ex)
            {
                _responseViewModel.ResponseMessage = ex.Message;
                _responseViewModel.StatusCode = 500;
                return Ok(_responseViewModel);
            }
        }
    }
}
