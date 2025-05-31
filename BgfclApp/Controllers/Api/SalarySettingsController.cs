using BgfclApp.ViewModels;
using Contracts;
using DocumentFormat.OpenXml.Math;
using Entities;
using Entities.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;
using System.Security.Policy;
using System.Text;
using System.Xml;

namespace BgfclApp.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SalarySettingsController : ControllerBase
    {
        private ISalarySettingContract _salarySettingContract;
        private ISalaryReportOfficerContract _salaryReportOfficerContract;
        private IAdvanceTaxContract _advanceTaxContract;
        private ILoanContract _loanContract;
        private IEmployeeContract _employeeContract;
        private ResponseViewModel _responseViewModel;
        public SalarySettingsController(ISalarySettingContract salarySettingContract,
            ResponseViewModel responseViewModel,
            IAdvanceTaxContract advanceTaxContract,
            ILoanContract loanContract
            ,IEmployeeContract employeeContract,
            ISalaryReportOfficerContract salaryReportOfficerContract)
        {
            _salarySettingContract = salarySettingContract;
            _responseViewModel = responseViewModel;
            _advanceTaxContract = advanceTaxContract;
            _loanContract = loanContract;
            _employeeContract = employeeContract;
            _salaryReportOfficerContract = salaryReportOfficerContract;

        }

        #region salary policy settings

        [HttpGet]
        public async Task<IActionResult> GetSalaryPolicySettings()
        {
            try
            {
                var salaryPolicySettings = await _salarySettingContract.GetSalaryPolicySettings();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = salaryPolicySettings;
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
        public async Task<IActionResult> UpdateSalaryPolicySetting([FromForm] SalaryPolicySetting salaryPolicySetting)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                salaryPolicySetting.UpdatedBy = "";
                salaryPolicySetting.UpdatedDate = DateTime.Now.Date;
                int result = await _salarySettingContract.UpdateSalaryPolicySetting(salaryPolicySetting);

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

        #endregion

        #region salary settings officer

        [HttpGet]
        public async Task<IActionResult> GetSalaryettingsOfficers()
        {
            try
            {
                var salarySettings = await _salarySettingContract.GetSalarySettingsOfficer();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = salarySettings;
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
        public async Task<IActionResult> GetSalaryettingsOfficersByJobCode(string jobCode)
        {
            try
            {
                var salarySettings = await _salarySettingContract.GetSalarySettingsOfficerByJobCode(jobCode);
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = salarySettings;
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
        public async Task<IActionResult> CreateSalarySettingsOfficer([FromForm] SalarySettingsOfficer salarySettingsOfficer)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                // validation logic
                {
                    if (salarySettingsOfficer.BasicSalary == null)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.BasicSalary) + "Error", "Basic Salary required");
                    }
                    if (salarySettingsOfficer.PersonalPay == null)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.PersonalPay) + "Error", "Personal Pay required");
                    }
                    if (salarySettingsOfficer.LikeBasic == null)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.LikeBasic) + "Error", "Like Basic required");
                    }
                    if (salarySettingsOfficer.OtherSalary == null)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.OtherSalary) + "Error", "Other Salary required");
                    }
                    if (salarySettingsOfficer.EducationAllow == null)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.EducationAllow) + "Error", "Education Allow required");
                    }
                    if (salarySettingsOfficer.WashAllow == null)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.WashAllow) + "Error", "Wash Allow required");
                    }
                    if (salarySettingsOfficer.HouseRentAllow == null)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.HouseRentAllow) + "Error", "House Rent Allow required");
                    }
                    if (salarySettingsOfficer.ConveyanceAllow == null)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.ConveyanceAllow) + "Error", "Conveyance Allow required");
                    }
                    if (salarySettingsOfficer.FamilyMedicalAllow == null)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.FamilyMedicalAllow) + "Error", "Family Medical Allow required");
                    }
                    if (salarySettingsOfficer.OfficerPF == null)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.OfficerPF) + "Error", "Officer PF required");
                    }
                    if (salarySettingsOfficer.FieldRiskAllow == null)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.FieldRiskAllow) + "Error", "Field Risk Allow required");
                    }
                    if (salarySettingsOfficer.ChargeAllow == null)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.ChargeAllow) + "Error", "Charge Allow required");
                    }
                    if (salarySettingsOfficer.DAidAllow == null)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.DAidAllow) + "Error", "DAid Allow required");
                    }
                    if (salarySettingsOfficer.DeputationAllow == null)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.DeputationAllow) + "Error", "Deputation Allow required");
                    }
                    if (salarySettingsOfficer.HouseRentReturn == null)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.HouseRentReturn) + "Error", "House Rent Return required");
                    }
                    if (salarySettingsOfficer.DormitoryDeduction == null)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.DormitoryDeduction) + "Error", "Dormitory Deduction required");
                    }
                    if (salarySettingsOfficer.FuelReturn == null)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.FuelReturn) + "Error", "Fuel Return required");
                    }
                    if (salarySettingsOfficer.CME == null)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.CME) + "Error", "CME required");
                    }
                    if (salarySettingsOfficer.BankId == 0)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.BankId) + "Error", "Bank required");
                    }
                    if (salarySettingsOfficer.BankBranchId == 0)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.BankBranchId) + "Error", "Bank Branch required");
                    }
                    if (String.IsNullOrEmpty(salarySettingsOfficer.AccountNumber))
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.AccountNumber) + "Error", "Account Number required");
                    }
                }

                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }

                salarySettingsOfficer.CreatedBy = "";
                salarySettingsOfficer.CreatedDate = DateTime.Now;
                int result = await _salarySettingContract.CreateSalarySettingsOfficer(salarySettingsOfficer);

                if (result>0)
                {
                    _responseViewModel.ResponseMessage = "Record saved successfully";
                    _responseViewModel.StatusCode = 201;
                    return Ok(_responseViewModel);
                }
                else
                {
                    _responseViewModel.ResponseMessage = "Something went wrong";
                    _responseViewModel.StatusCode = 500;
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

        [HttpPost]
        public async Task<IActionResult> UpdateSalarySettingsOfficer([FromForm] SalarySettingsOfficer salarySettingsOfficer)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                // validation logic
                {
                    if (salarySettingsOfficer.BasicSalary == null)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.BasicSalary) + "Error", "Basic Salary required");
                    }
                    if (salarySettingsOfficer.PersonalPay == null)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.PersonalPay) + "Error", "Personal Pay required");
                    }
                    if (salarySettingsOfficer.LikeBasic == null)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.LikeBasic) + "Error", "Like Basic required");
                    }
                    if (salarySettingsOfficer.OtherSalary == null)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.OtherSalary) + "Error", "Other Salary required");
                    }
                    if (salarySettingsOfficer.EducationAllow == null)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.EducationAllow) + "Error", "Education Allow required");
                    }
                    if (salarySettingsOfficer.WashAllow == null)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.WashAllow) + "Error", "Wash Allow required");
                    }
                    if (salarySettingsOfficer.HouseRentAllow == null)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.HouseRentAllow) + "Error", "House Rent Allow required");
                    }
                    if (salarySettingsOfficer.ConveyanceAllow == null)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.ConveyanceAllow) + "Error", "Conveyance Allow required");
                    }
                    if (salarySettingsOfficer.FamilyMedicalAllow == null)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.FamilyMedicalAllow) + "Error", "Family Medical Allow required");
                    }
                    if (salarySettingsOfficer.OfficerPF == null)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.OfficerPF) + "Error", "Officer PF required");
                    }
                    if (salarySettingsOfficer.FieldRiskAllow == null)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.FieldRiskAllow) + "Error", "Field Risk Allow required");
                    }
                    if (salarySettingsOfficer.ChargeAllow == null)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.ChargeAllow) + "Error", "Charge Allow required");
                    }
                    if (salarySettingsOfficer.DAidAllow == null)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.DAidAllow) + "Error", "DAid Allow required");
                    }
                    if (salarySettingsOfficer.DeputationAllow == null)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.DeputationAllow) + "Error", "Deputation Allow required");
                    }
                    if (salarySettingsOfficer.HouseRentReturn == null)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.HouseRentReturn) + "Error", "House Rent Return required");
                    }
                    if (salarySettingsOfficer.DormitoryDeduction == null)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.DormitoryDeduction) + "Error", "Dormitory Deduction required");
                    }
                    if (salarySettingsOfficer.FuelReturn == null)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.FuelReturn) + "Error", "Fuel Return required");
                    }
                    if (salarySettingsOfficer.CME == null)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.CME) + "Error", "CME required");
                    }
                    if (salarySettingsOfficer.BankId == 0)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.BankId) + "Error", "Bank required");
                    }
                    if (salarySettingsOfficer.BankBranchId == 0)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.BankBranchId) + "Error", "Bank Branch required");
                    }
                    if (String.IsNullOrEmpty(salarySettingsOfficer.AccountNumber))
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsOfficer.AccountNumber) + "Error", "Account Number required");
                    }
                }

                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }

                salarySettingsOfficer.UpdatedBy = "";
                salarySettingsOfficer.UpdatedDate = DateTime.Now;

                int result = await _salarySettingContract.UpdateSalarySettingsOfficer(salarySettingsOfficer);

                if (result>0)
                {
                    _responseViewModel.ResponseMessage = "Record saved successfully";
                    _responseViewModel.StatusCode = 201;
                    return Ok(_responseViewModel);
                }
                else
                {
                    _responseViewModel.ResponseMessage = "Couldn't update data";
                    _responseViewModel.StatusCode = 500;
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
        public async Task<IActionResult> GetSalaryettingsJuniorStaff()
        {
            try
            {
                var salarySettings = await _salarySettingContract.GetSalarySettingsJuniorStaff();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = salarySettings;
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
        public async Task<IActionResult> GetSalaryettingsworker()
        {
            try
            {
                var salarySettings = await _salarySettingContract.GetSalarySettingsWorker();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = salarySettings;
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
        public async Task<IActionResult> GetSalaryettingsJuniorStaffByJobCode(string jobCode)
        {
            try
            {
                var salarySettings = await _salarySettingContract.GetSalarySettingsJuniorStaffByJobCode(jobCode);
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = salarySettings;
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
        public async Task<IActionResult> CreateSalarySettingsJuniorStaff([FromForm] SalarySettingsJuniorStaff salarySettingsJuniorStaff)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                // validation logic
                {
                    if (String.IsNullOrEmpty(salarySettingsJuniorStaff.JobCode))
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsJuniorStaff.BasicSalary) + "Error", "Jobcode required");
                    }
                    if (salarySettingsJuniorStaff.PayModeId == 0)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsJuniorStaff.PayModeId) + "Error", "Paymode required");
                    }
                    if (salarySettingsJuniorStaff.BankId == 0)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsJuniorStaff.BankId) + "Error", "Bank required");
                    }
                    if (salarySettingsJuniorStaff.BankBranchId == 0)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsJuniorStaff.BankBranchId) + "Error", "Bank Branch required");
                    }
                    if (String.IsNullOrEmpty(salarySettingsJuniorStaff.AccountNumber))
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsJuniorStaff.AccountNumber) + "Error", "Account Number required");
                    }
                }

                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }

                salarySettingsJuniorStaff.CreatedBy = "";
                salarySettingsJuniorStaff.CreatedDate = DateTime.Now;
                int result = await _salarySettingContract.CreateSalarySettingsJuniorStaff(salarySettingsJuniorStaff);
                if (result>0)
                {
                    _responseViewModel.ResponseMessage = "Record saved successfully";
                    _responseViewModel.StatusCode = 201;
                    return Ok(_responseViewModel);
                }
                else
                {
                    _responseViewModel.ResponseMessage = "Something went wrong";
                    _responseViewModel.StatusCode = 500;
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

        [HttpPost]
        public async Task<IActionResult> UpdateSalarySettingsJuniorStaff([FromForm] SalarySettingsJuniorStaff salarySettingsJuniorStaff)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                // validation logic
                {
                    if (String.IsNullOrEmpty(salarySettingsJuniorStaff.JobCode))
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsJuniorStaff.BasicSalary) + "Error", "Jobcode required");
                    }
                    if (salarySettingsJuniorStaff.PayModeId == 0)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsJuniorStaff.PayModeId) + "Error", "Paymode required");
                    }
                    if (salarySettingsJuniorStaff.BankId == 0)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsJuniorStaff.BankId) + "Error", "Bank required");
                    }
                    if (salarySettingsJuniorStaff.BankBranchId == 0)
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsJuniorStaff.BankBranchId) + "Error", "Bank Branch required");
                    }
                    if (String.IsNullOrEmpty(salarySettingsJuniorStaff.AccountNumber))
                    {
                        _responseViewModel.Errors.Add(nameof(salarySettingsJuniorStaff.AccountNumber) + "Error", "Account Number required");
                    }
                    if(salarySettingsJuniorStaff.Is_Daily_Worker != true)
                    {
                        if (salarySettingsJuniorStaff.BasicSalary == null)
                        {
                            _responseViewModel.Errors.Add(nameof(salarySettingsJuniorStaff.BasicSalary) + "Error", "Basic Salary required");
                        }
                        if (salarySettingsJuniorStaff.PersonalPay == null)
                        {
                            _responseViewModel.Errors.Add(nameof(salarySettingsJuniorStaff.PersonalPay) + "Error", "Personal Pay required");
                        }
                        if (salarySettingsJuniorStaff.ConvenienceAllow == null)
                        {
                            _responseViewModel.Errors.Add(nameof(salarySettingsJuniorStaff.ConvenienceAllow) + "Error", "Convenience Allow required");
                        }
                        if (salarySettingsJuniorStaff.OtherSalary == null)
                        {
                            _responseViewModel.Errors.Add(nameof(salarySettingsJuniorStaff.OtherSalary) + "Error", "Other Salary required");
                        }
                        if (salarySettingsJuniorStaff.HouseRentAllowRate == null)
                        {
                            _responseViewModel.Errors.Add(nameof(salarySettingsJuniorStaff.HouseRentAllowRate) + "Error", "House Rent Allow Rate required");
                        }
                        if (salarySettingsJuniorStaff.HouseRentAllow == null)
                        {
                            _responseViewModel.Errors.Add(nameof(salarySettingsJuniorStaff.HouseRentAllow) + "Error", "House Rent Allow required");
                        }
                        if (salarySettingsJuniorStaff.FamilyMedicalAllow == null)
                        {
                            _responseViewModel.Errors.Add(nameof(salarySettingsJuniorStaff.FamilyMedicalAllow) + "Error", "Family Medical Allow required");
                        }
                        if (salarySettingsJuniorStaff.FuelReturn == null)
                        {
                            _responseViewModel.Errors.Add(nameof(salarySettingsJuniorStaff.FuelReturn) + "Error", "Fuel Return required");
                        }
                        if (salarySettingsJuniorStaff.EducationAllow == null)
                        {
                            _responseViewModel.Errors.Add(nameof(salarySettingsJuniorStaff.EducationAllow) + "Error", "Education Allow required");
                        }
                        if (salarySettingsJuniorStaff.FieldAllow == null)
                        {
                            _responseViewModel.Errors.Add(nameof(salarySettingsJuniorStaff.FieldAllow) + "Error", "Field Allow required");
                        }
                        if (salarySettingsJuniorStaff.DormitoryDeduction == null)
                        {
                            _responseViewModel.Errors.Add(nameof(salarySettingsJuniorStaff.DormitoryDeduction) + "Error", "Dormitory Deduction required");
                        }
                        if (salarySettingsJuniorStaff.ProvidentFund == null)
                        {
                            _responseViewModel.Errors.Add(nameof(salarySettingsJuniorStaff.ProvidentFund) + "Error", "Provident Fund required");
                        }
                    }
                    
                }

                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }

                salarySettingsJuniorStaff.UpdatedBy = "";
                salarySettingsJuniorStaff.UpdatedDate = DateTime.Now;
                int result = await _salarySettingContract.UpdateSalarySettingsJuniorStaff(salarySettingsJuniorStaff);
                if (result>0)
                {
                    _responseViewModel.ResponseMessage = "Record saved successfully";
                    _responseViewModel.StatusCode = 201;
                    return Ok(_responseViewModel);
                }
                else
                {
                    _responseViewModel.ResponseMessage = "Couldn't update data";
                    _responseViewModel.StatusCode = 500;
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
        public async Task<IActionResult> GetAdvanceTaxes()
        {
            try
            {
                var advanceTaxes = await _advanceTaxContract.GetAdvanceTaxes();
                if (advanceTaxes.Count > 0)
                {
                    foreach (var item in advanceTaxes)
                    {
                        item.Date = DateTime.ParseExact(item.Date.Split(' ')[0], "MM/dd/yyyy", CultureInfo.InvariantCulture).ToString("dd-MMM-yyyy");
                    }
                }
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = advanceTaxes;
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
        public async Task<IActionResult> CreateAdvanceTax([FromForm] AdvanceTax advanceTax)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (String.IsNullOrEmpty(advanceTax.LetterNo))
                {
                    _responseViewModel.Errors.Add(nameof(advanceTax.LetterNo) + "Error", "Letter No. required");
                }
                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                var advanceTaxes = await _advanceTaxContract.GetAdvanceTaxes();
                var existingBankTag = advanceTaxes.Where(a => a.LetterNo == advanceTax.LetterNo).SingleOrDefault();
                if (existingBankTag == null)
                {
                    advanceTax.CreatedBy = "";
                    advanceTax.CreatedDate = DateTime.Now;
                    int result = await _advanceTaxContract.CreateAdvanceTax(advanceTax);

                    _responseViewModel.ResponseMessage = "Record saved successfully";
                    _responseViewModel.StatusCode = 201;
                    return Ok(_responseViewModel);
                }
                else
                {
                    _responseViewModel.ResponseMessage = "Record already exists";
                    _responseViewModel.Errors.Add(nameof(advanceTax.LetterNo) + "Error", "Letter No. already exists");
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

        [HttpDelete]
        public async Task<IActionResult> RemoveAdvanceTax(int id)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                var advanceTaxes = await _advanceTaxContract.GetAdvanceTaxes();
                var advanceTax = advanceTaxes.Where(b => b.Id == id).SingleOrDefault();

                if (advanceTax == null)
                {
                    _responseViewModel.ResponseMessage = "Record not found";
                    _responseViewModel.StatusCode = 404;
                    return Ok(_responseViewModel);
                }

                int result = await _advanceTaxContract.RemoveAdvanceTax(advanceTax.Id);

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

        public async Task<IActionResult> GetEmployeesForSalarySetting(int employeeTypeId)
        {
            List<EmployeeViewModel> filteredEmployees = new List<EmployeeViewModel>();
            var employees = await _employeeContract.GetEmployeesByEmployeeType(employeeTypeId);

            // 1 = officer
            // 2 = staff
            // 3 = Daily Worker
            if (employeeTypeId==1)
            {
                
                var salarySettings = await _salarySettingContract.GetSalarySettingsOfficer();
                foreach (var employee in employees)
                {
                    var _setting = salarySettings.Where(s => s.JobCode == employee.JobCode).SingleOrDefault();
                    if (_setting==null)
                    {
                        filteredEmployees.Add(employee);
                    }
                }

            }
            else if(employeeTypeId == 2)
            {
                var salarySettings = await _salarySettingContract.GetSalarySettingsJuniorStaff();
                foreach (var employee in employees)
                {
                    var _setting = salarySettings.Where(s => s.JobCode == employee.JobCode).SingleOrDefault();
                    if (_setting == null)
                    {
                        filteredEmployees.Add(employee);
                    }
                }
            }
            else if(employeeTypeId == 3) 
            {
                var salarySettings = await _salarySettingContract.GetSalarySettingsWorker();
                foreach (var employee in employees)
                {
                    var _setting = salarySettings.Where(s => s.JobCode == employee.JobCode).SingleOrDefault();
                    if (_setting == null)
                    {
                        filteredEmployees.Add(employee);
                    }
                }
            }

            _responseViewModel.StatusCode = 200;
            _responseViewModel.Data = filteredEmployees;
            return Ok(_responseViewModel);
        }

        #endregion

        #region salary settings
        [HttpPost]
        public async Task<IActionResult> StoreMonthlySalarySettingOfficer(List<MonthlySalarySettingOfficer> monthlySalarySettings)
        {
            if (monthlySalarySettings.Count > 0)
            {
                foreach (var item in monthlySalarySettings)
                {
                    await _salarySettingContract.CreateMonthlySalarySettingOfficer(item);
                }
                _responseViewModel.ResponseMessage = "Operation succeed";
                _responseViewModel.StatusCode = 200;
                return Ok(_responseViewModel);
            }
            else
            {
                _responseViewModel.ResponseMessage = "Something went wrong";
                _responseViewModel.StatusCode = 500;
                return Ok(_responseViewModel);
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> StoreMonthlySalarySettingStaff(List<MonthlySalarySettingJuniorStaff> monthlySalarySettings)
        {
            if (monthlySalarySettings.Count > 0)
            {
                foreach (var item in monthlySalarySettings)
                {
                    await _salarySettingContract.CreateMonthlySalaryJuniorStaff(item);
                }
                _responseViewModel.ResponseMessage = "Operation succeed";
                _responseViewModel.StatusCode = 200;
                return Ok(_responseViewModel);
            }
            else
            {
                _responseViewModel.ResponseMessage = "Something went wrong";
                _responseViewModel.StatusCode = 500;
                return Ok(_responseViewModel);
            }

        }

        [HttpPost]
        public async Task<IActionResult> StoreMonthlyLoan(List<MonthlyLoan> monthlyLoans)
        {
            
            if (monthlyLoans.Count > 0)
            {
                
                foreach (var item in monthlyLoans)
                {
                    await _loanContract.CreateMonthlyLoan(item);
                }
                _responseViewModel.ResponseMessage = "Operation succeed";
                _responseViewModel.StatusCode = 200;
                return Ok(_responseViewModel);
            }
            else
            {
                _responseViewModel.ResponseMessage = "Something went wrong";
                _responseViewModel.StatusCode = 500;
                return Ok(_responseViewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSalarySettingsOfficerByJobCode(string jobCode)
        {

            if (!String.IsNullOrEmpty(jobCode))
            {
                var salarySettingOfficer = await _salarySettingContract.GetSalarySettingsOfficerByJobCode(jobCode);
                if (!String.IsNullOrEmpty(salarySettingOfficer.JobCode))
                {
                    _responseViewModel.Data = salarySettingOfficer;
                    _responseViewModel.ResponseMessage = "Operation succeed";
                    _responseViewModel.StatusCode = 200;
                    return Ok(_responseViewModel);
                }
                else
                {
                    _responseViewModel.ResponseMessage = "No data found";
                    _responseViewModel.StatusCode = 404;
                    return Ok(_responseViewModel);
                }

            }
            else
            {
                _responseViewModel.ResponseMessage = "Something went wrong";
                _responseViewModel.StatusCode = 500;
                return Ok(_responseViewModel);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetPayModes()
        {

            var payModes = await _salarySettingContract.GetPayModes();
            _responseViewModel.Data = payModes;
            _responseViewModel.ResponseMessage = "Operation succeed";
            _responseViewModel.StatusCode = 200;
            return Ok(_responseViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetSalaryProcess()
        {

            var payModes = await _salarySettingContract.GetSalaryProcess();
            _responseViewModel.Data = payModes;
            _responseViewModel.ResponseMessage = "Operation succeed";
            _responseViewModel.StatusCode = 200;
            return Ok(_responseViewModel);
        }
        [HttpGet]
        public async Task<IActionResult> GetSalaryProcessById( int id)
        {

            var process = await _salarySettingContract.GetSalaryProcessById(id);
            _responseViewModel.Data = process;
            _responseViewModel.ResponseMessage = "Operation succeed";
            _responseViewModel.StatusCode = 200;
            return Ok(_responseViewModel);
        }
        [HttpGet]
        public async Task<IActionResult> SalaryProcess(int monthId, int employeeTypeId)
        {
            try
            {
                var salaryJournal = await _salarySettingContract.GetSalaryJournalMaster(monthId, employeeTypeId);
                if (salaryJournal==null)
                {
                    var result = await _salarySettingContract.SalaryProcess(monthId, employeeTypeId);
                    // update loans
                    try
                    {
                        // hb loan
                        var hbLoans = await _loanContract.GetHbLoans(isActive: 1);
                        foreach (var item in hbLoans)
                        {
                            var installment = await _loanContract.GetInstallmentByHBLoanId_MonthId(item.Id, monthId);
                            if (installment != null)
                            {
                                await _loanContract.UpdateLoansInFinalAdjustment(item.JobCode, monthId, installment.InstallmentAmount, 1);
                            }

                        }

                        // mc loan
                        var mcLoans = await _loanContract.GetMcLoans(isActive: 1);
                        foreach (var item in mcLoans)
                        {
                            var installment = await _loanContract.GetInstallmentByMCLoanId_MonthId(item.Id, monthId);
                            if (installment != null)
                            {
                                await _loanContract.UpdateLoansInFinalAdjustment(item.JobCode, monthId, (double)installment.InstallmentAmount, 2);
                            }
                        }

                        // com loan
                        var comLoans = await _loanContract.GetComLoans(isActive: 1);
                        foreach (var item in comLoans)
                        {
                            var installment = await _loanContract.GetInstallmentByComLoanId_MonthId(item.Id, monthId);
                            if (installment != null)
                            {
                                await _loanContract.UpdateLoansInFinalAdjustment(item.JobCode, monthId, (double)(installment.InstallmentAmount + installment.InterestAmount), 4);
                            }
                        }

                        // car loan
                        var carLoans = await _loanContract.GetCarLoans(isActive: 1);
                        foreach (var item in carLoans)
                        {
                            var installment = await _loanContract.GetInstallmentByCarLoanId_MonthId(item.Id, monthId);
                            if (installment != null)
                            {
                                await _loanContract.UpdateLoansInFinalAdjustment(item.JobCode, monthId, installment.TotalPayment, 10);
                            }
                        }
                    }
                    catch (Exception ex) { }
                    _responseViewModel.StatusCode = 201;
                    _responseViewModel.ResponseMessage = "Operation competed";
                    return Ok(_responseViewModel);
                }
                else
                {
                    _responseViewModel.StatusCode = 500;
                    _responseViewModel.ResponseMessage = "Journal already processed";
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
        [HttpPost]

        [HttpPost]
        public async Task<IActionResult> UpdateSalaryProcessMaster([FromBody] SalaryProcessMaster process)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();

            try
            {
                if (process == null || process.SalaryProcessingID <= 0)
                {
                    _responseViewModel.ResponseMessage = "Invalid request data.";
                    _responseViewModel.StatusCode = 400;
                    return BadRequest(_responseViewModel);
                }

                var existingProcesses = await _salarySettingContract.GetSalaryProcessMaster();
                var existingPro = existingProcesses.SingleOrDefault(d => d.SalaryProcessingID == process.SalaryProcessingID);

                if (existingPro == null)
                {
                    _responseViewModel.ResponseMessage = "Process data not found.";
                    _responseViewModel.StatusCode = 404;
                    return NotFound(_responseViewModel);
                }
                existingPro.Status = 0;
                int result = await _salarySettingContract.UpdateSalaryProcessMaster(existingPro);

                if (result > 0)
                {
                    _responseViewModel.ResponseMessage = "Now Salary process is stop For this Month.";
                    _responseViewModel.StatusCode = 200;
                    return Ok(_responseViewModel);
                }
                else
                {
                    _responseViewModel.ResponseMessage = "Update failed.";
                    _responseViewModel.StatusCode = 500;
                    return StatusCode(500, _responseViewModel);
                }
            }
            catch (Exception ex)
            {
                _responseViewModel.ResponseMessage = "Something went wrong: " + ex.Message;
                _responseViewModel.StatusCode = 500;
                return StatusCode(500, _responseViewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSalaryProcessMaster()
        {
            try
            {
                var data = await _salarySettingContract.GetSalaryProcessMaster();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = data;
                return Ok(_responseViewModel);
            }
            catch (Exception ex)
            {
                _responseViewModel.StatusCode = 500;
                _responseViewModel.ResponseMessage = ex.Message;
                return Ok(_responseViewModel);
            }
        }
        public async Task<IActionResult> SmsService(int monthId, int employeeTypeId)

        {
            try
            {
                var smsServiceInfo = await _salarySettingContract.GetSmsService(monthId, employeeTypeId);
                _responseViewModel.Data = smsServiceInfo;
                _responseViewModel.StatusCode = 201;
                _responseViewModel.ResponseMessage = "Operation competed";
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
        public async Task<IActionResult> SendSms(int month,int year, int employeeTypeId, string jobCodes)

        {
            try
            {
                int monthId = year * 100 + month;
                string[] _jobCodes = jobCodes.Split(',');
                if (_jobCodes.Length > 0)
                {
                    if (employeeTypeId == 1)
                    {
                        foreach (var jobCode in _jobCodes)
                        {
                            var employeeInfo = await _employeeContract.GetEmployeeViewByJobCode(jobCode);
                            var salaryInformations = await _salarySettingContract.FinalAdjustmentOfficer(monthId);
                            var salaryInformation = salaryInformations.Where(s => s.JobCode == jobCode).SingleOrDefault();

                            StringBuilder builder = new StringBuilder();
                            builder.AppendLine("BGFCL");
                            builder.AppendLine("Salary Pay Slip (" + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month) + " - " + year + ")");
                            builder.AppendLine("");
                            builder.AppendLine(jobCode);
                            builder.AppendLine(salaryInformation.EmployeeName);
                            builder.AppendLine(salaryInformation.BankName);
                            builder.AppendLine(salaryInformation.BankBranchName);
                            builder.AppendLine(salaryInformation.AccountNumber);

                            if (salaryInformation.BasicSalary > 0)
                            {
                                builder.AppendLine("Basic " + salaryInformation.BasicSalary.ToString());
                            }
                            if (salaryInformation.LikeBasic > 0)
                            {
                                builder.AppendLine("Like Basic " + salaryInformation.LikeBasic.ToString());
                            }
                            if (salaryInformation.PersonalSalary > 0)
                            {
                                builder.AppendLine("Personal Salary " + salaryInformation.PersonalSalary.ToString());
                            }
                            if (salaryInformation.ArrearSalary > 0)
                            {
                                builder.AppendLine("Arrear Salary " + salaryInformation.ArrearSalary.ToString());
                            }
                            if (salaryInformation.OtherSalary > 0)
                            {
                                builder.AppendLine("Other Salary " + salaryInformation.OtherSalary.ToString());
                            }
                            if (salaryInformation.WorkDays > 0)
                            {
                                builder.AppendLine("Work Days " + salaryInformation.WorkDays.ToString());
                            }
                            if (salaryInformation.LunchAllow > 0)
                            {
                                builder.AppendLine("Lunch Allow " + salaryInformation.LunchAllow.ToString());
                            }
                            if (salaryInformation.TiffinAllow > 0)
                            {
                                builder.AppendLine("Tiffin Allow " + salaryInformation.TiffinAllow.ToString());
                            }
                            if (salaryInformation.SpecialBenefit > 0)
                            {
                                builder.AppendLine("Special Benefit " + salaryInformation.SpecialBenefit.ToString());
                            }
                            if (salaryInformation.HouseRentAllow > 0)
                            {
                                builder.AppendLine("HouseRent Allow " + salaryInformation.HouseRentAllow.ToString());
                            }
                            if (salaryInformation.Conveyance > 0)
                            {
                                builder.AppendLine("Conveyance " + salaryInformation.Conveyance.ToString());
                            }
                            if (salaryInformation.FMAllow > 0)
                            {
                                builder.AppendLine("FM Allow " + salaryInformation.FMAllow.ToString());
                            }
                            if (salaryInformation.WashAllow > 0)
                            {
                                builder.AppendLine("Wash Allow " + salaryInformation.WashAllow.ToString());
                            }
                            if (salaryInformation.EducationalAllow > 0)
                            {
                                builder.AppendLine("Educational Allow " + salaryInformation.EducationalAllow.ToString());
                            }
                            if (salaryInformation.FieldRiskAllow > 0)
                            {
                                builder.AppendLine("Field Risk Allow " + salaryInformation.FieldRiskAllow.ToString());
                            }
                            if (salaryInformation.ChargeAllow > 0)
                            {
                                builder.AppendLine("Charge Allow " + salaryInformation.ChargeAllow.ToString());
                            }
                            if (salaryInformation.DAidAllow > 0)
                            {
                                builder.AppendLine("DAid Allow " + salaryInformation.DAidAllow.ToString());
                            }
                            if (salaryInformation.DeputationAllow > 0)
                            {
                                builder.AppendLine("Deputation Allow " + salaryInformation.DeputationAllow.ToString());
                            }
                            if (salaryInformation.OtherAllow > 0)
                            {
                                builder.AppendLine("Other Allow " + salaryInformation.OtherAllow.ToString());
                            }
                            if (salaryInformation.CME > 0)
                            {
                                builder.AppendLine("CM Exp " + salaryInformation.CME.ToString());
                            }

                            decimal grossPay = salaryInformation.BasicSalary + salaryInformation.LikeBasic + salaryInformation.PersonalSalary + salaryInformation.ArrearSalary + salaryInformation.OtherSalary +
                                salaryInformation.LunchAllow + salaryInformation.TiffinAllow + salaryInformation.SpecialBenefit + salaryInformation.HouseRentAllow + salaryInformation.Conveyance +
                                salaryInformation.FMAllow + salaryInformation.WashAllow + salaryInformation.EducationalAllow + salaryInformation.FieldRiskAllow + salaryInformation.ChargeAllow +
                                salaryInformation.DAidAllow + salaryInformation.DeputationAllow + salaryInformation.OtherAllow + salaryInformation.CME;
                            builder.AppendLine("Gross Pay " + grossPay.ToString());
                            if (salaryInformation.RevenueStamp > 0)
                            {
                                builder.AppendLine("Revenue Stamp " + salaryInformation.RevenueStamp.ToString());
                            }
                            if (salaryInformation.WelfareFund > 0)
                            {
                                builder.AppendLine("Welfare Fund " + salaryInformation.WelfareFund.ToString());
                            }
                            if (salaryInformation.OfficerClub > 0)
                            {
                                builder.AppendLine("Officer Club " + salaryInformation.OfficerClub.ToString());
                            }
                            if (salaryInformation.OfficerAssociation > 0)
                            {
                                builder.AppendLine("Officer Association " + salaryInformation.OfficerAssociation.ToString());
                            }
                            if (salaryInformation.ProvidentFund > 0)
                            {
                                builder.AppendLine("Provident Fund " + salaryInformation.ProvidentFund.ToString());
                            }
                            if (salaryInformation.MedicalFund > 0)
                            {
                                builder.AppendLine("Medical Fund " + salaryInformation.MedicalFund.ToString());
                            }
                            if (salaryInformation.TMBill > 0)
                            {
                                builder.AppendLine("TM Bill " + salaryInformation.TMBill.ToString());
                            }
                            if (salaryInformation.Hospitalisation > 0)
                            {
                                builder.AppendLine("Hospitalisation " + salaryInformation.Hospitalisation.ToString());
                            }
                            if (salaryInformation.HouseRentReturn > 0)
                            {
                                builder.AppendLine("HouseRent Return " + salaryInformation.HouseRentReturn.ToString());
                            }
                            if (salaryInformation.Dormitory > 0)
                            {
                                builder.AppendLine("Dormitory " + salaryInformation.Dormitory.ToString());
                            }
                            if (salaryInformation.SpecialDeduction > 0)
                            {
                                builder.AppendLine("Special Deduction " + salaryInformation.SpecialDeduction.ToString());
                            }
                            if (salaryInformation.FuelReturn > 0)
                            {
                                builder.AppendLine("Fuel Return " + salaryInformation.FuelReturn.ToString());
                            }
                            if (salaryInformation.Advance > 0)
                            {
                                builder.AppendLine("Advance " + salaryInformation.Advance.ToString());
                            }
                            if (salaryInformation.Other > 0)
                            {
                                builder.AppendLine("Other Deduction " + salaryInformation.Other.ToString());
                            }
                            if (salaryInformation.HBLoan > 0)
                            {
                                builder.AppendLine("HB Loan " + salaryInformation.HBLoan.ToString());
                            }
                            if (salaryInformation.MCylLoan > 0)
                            {
                                builder.AppendLine("MCyl Loan " + salaryInformation.MCylLoan.ToString());
                            }
                            if (salaryInformation.BCylLoan > 0)
                            {
                                builder.AppendLine("BCyl Loan " + salaryInformation.BCylLoan.ToString());
                            }
                            if (salaryInformation.ComLoan > 0)
                            {
                                builder.AppendLine("Computer Loan " + salaryInformation.ComLoan.ToString());
                            }
                            if (salaryInformation.CarLoan > 0)
                            {
                                builder.AppendLine("Car Loan " + salaryInformation.CarLoan.ToString());
                            }
                            if (salaryInformation.PFLoan > 0)
                            {
                                builder.AppendLine("PF Loan " + salaryInformation.PFLoan.ToString());
                            }
                            if (salaryInformation.WPFLoan > 0)
                            {
                                builder.AppendLine("WPF Loan " + salaryInformation.WPFLoan.ToString());
                            }
                            if (salaryInformation.CosLoan > 0)
                            {
                                builder.AppendLine("COS Loan " + salaryInformation.CosLoan.ToString());
                            }
                            if (salaryInformation.OtherLoan > 0)
                            {
                                builder.AppendLine("Other Loan " + salaryInformation.OtherLoan.ToString());
                            }
                            if (salaryInformation.IncomeTax > 0)
                            {
                                builder.AppendLine("Income Tax " + salaryInformation.IncomeTax.ToString());
                            }
                            decimal totalDeduction = salaryInformation.RevenueStamp + salaryInformation.WelfareFund + salaryInformation.OfficerClub + salaryInformation.OfficerAssociation +

                                salaryInformation.ProvidentFund + salaryInformation.MedicalFund + salaryInformation.TMBill + salaryInformation.Hospitalisation + salaryInformation.HouseRentReturn +
                                salaryInformation.Dormitory + salaryInformation.SpecialDeduction + salaryInformation.FuelReturn + salaryInformation.Advance + salaryInformation.Other + salaryInformation.HBLoan +
                                salaryInformation.MCylLoan + salaryInformation.BCylLoan + salaryInformation.ComLoan + salaryInformation.CarLoan + salaryInformation.PFLoan + salaryInformation.WPFLoan +
                                salaryInformation.CosLoan + salaryInformation.OtherLoan + salaryInformation.IncomeTax;
                            builder.AppendLine("Total Deduction " + totalDeduction.ToString());
                            decimal netPay = grossPay - totalDeduction;
                            builder.AppendLine("Net Pay " + netPay.ToString());
                            //builder.ToString().Replace(".0000#", "").Replace("00#", "").Replace("#", "");


                            var clientForSMSSending = new HttpClient();
                            var response = clientForSMSSending.GetAsync("http://bulksms.smsvaults.work:7788/sendtext?apikey=93d0b5ff8e6cb424&secretkey=91ee219e&callerID=bgfcl&toUser=" + employeeInfo.MobileNumber + "&messageContent=" + builder);

                        }
                    }
                    else
                    {
                        foreach (var jobCode in _jobCodes)
                        {
                            var employeeInfo = await _employeeContract.GetEmployeeViewByJobCode(jobCode);
                            var salaryInformations = await _salarySettingContract.FinalAdjustmentJuniorStaff(monthId);
                            var salaryInformation = salaryInformations.Where(s => s.JobCode == jobCode).SingleOrDefault();

                            StringBuilder builder = new StringBuilder();
                            builder.AppendLine("BGFCL");
                            builder.AppendLine("Salary Pay Slip (" + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month) + " - " + year + ")");
                            builder.AppendLine("");
                            builder.AppendLine(jobCode);
                            builder.AppendLine(salaryInformation.EmployeeName);
                            builder.AppendLine(salaryInformation.BankName);
                            builder.AppendLine(salaryInformation.BankBranchName);
                            builder.AppendLine(salaryInformation.AccountNumber);

                            if (salaryInformation.BasicSalary > 0)
                            {
                                builder.AppendLine("Basic " + salaryInformation.BasicSalary.ToString());
                            }
                            if (salaryInformation.PersonalSalary > 0)
                            {
                                builder.AppendLine("Personal Salary " + salaryInformation.PersonalSalary.ToString());
                            }
                            if (salaryInformation.ArrearSalary > 0)
                            {
                                builder.AppendLine("Arrear Salary " + salaryInformation.ArrearSalary.ToString());
                            }
                            if (salaryInformation.OtherSalary > 0)
                            {
                                builder.AppendLine("Other Salary " + salaryInformation.OtherSalary.ToString());
                            }
                            if (salaryInformation.WorkDays > 0)
                            {
                                builder.AppendLine("Work Days " + salaryInformation.WorkDays.ToString());
                            }
                            if (salaryInformation.NumberOfShift > 0)
                            {
                                builder.AppendLine("Shift " + salaryInformation.NumberOfShift.ToString());
                            }
                            if (salaryInformation.OtSingle > 0)
                            {
                                builder.AppendLine("OT(S) " + salaryInformation.OtSingle.ToString());
                            }
                            if (salaryInformation.OtDouble > 0)
                            {
                                builder.AppendLine("OT(D) " + salaryInformation.OtDouble.ToString());
                            }
                            if (salaryInformation.SpecialBenefit > 0)
                            {
                                builder.AppendLine("Special Benefit " + salaryInformation.SpecialBenefit.ToString());
                            }
                            if (salaryInformation.HouseRentAllow > 0)
                            {
                                builder.AppendLine("HouseRent Allow " + salaryInformation.HouseRentAllow.ToString());
                            }
                            if (salaryInformation.ConvenienceAllow > 0)
                            {
                                builder.AppendLine("Convenience Allow " + salaryInformation.ConvenienceAllow.ToString());
                            }
                            if (salaryInformation.FamilyMedicalAllow > 0)
                            {
                                builder.AppendLine("FM Allow " + salaryInformation.FamilyMedicalAllow.ToString());
                            }
                            if (salaryInformation.LunchAllow > 0)
                            {
                                builder.AppendLine("Lunch Allow " + salaryInformation.LunchAllow.ToString());
                            }
                            if (salaryInformation.TiffinAllow > 0)
                            {
                                builder.AppendLine("Tiffin Allow " + salaryInformation.TiffinAllow.ToString());
                            }
                            if (salaryInformation.ShiftAllow > 0)
                            {
                                builder.AppendLine("Shift Allow " + salaryInformation.ShiftAllow.ToString());
                            }
                            if (salaryInformation.OtAllow > 0)
                            {
                                builder.AppendLine("OT Allow " + salaryInformation.OtAllow.ToString());
                            }
                            if (salaryInformation.EducationAllowance > 0)
                            {
                                builder.AppendLine("Education Allowance " + salaryInformation.EducationAllowance.ToString());
                            }
                            //if (salaryInformation.FuelReturn > 0)
                            //{
                            //    builder.AppendLine("FR Allow " + salaryInformation.FuelReturn.ToString());
                            //}
                            if (salaryInformation.FuelAllow > 0)
                            {
                                builder.AppendLine("Fuel Allow " + salaryInformation.FuelAllow.ToString());
                            }
                            if (salaryInformation.UtilityAllow > 0)
                            {
                                builder.AppendLine("Utility Allow " + salaryInformation.UtilityAllow.ToString());
                            }
                            if (salaryInformation.OtherAllow > 0)
                            {
                                builder.AppendLine("Other Allow " + salaryInformation.OtherAllow.ToString());
                            }

                            decimal grossPay = salaryInformation.BasicSalary + salaryInformation.PersonalSalary + salaryInformation.ArrearSalary + salaryInformation.OtherSalary + salaryInformation.OtSingle +

                                salaryInformation.OtDouble + salaryInformation.SpecialBenefit + salaryInformation.HouseRentAllow + salaryInformation.ConvenienceAllow + salaryInformation.FamilyMedicalAllow +
                                salaryInformation.LunchAllow + salaryInformation.TiffinAllow + salaryInformation.ShiftAllow + salaryInformation.OtAllow + salaryInformation.EducationAllowance +
                                salaryInformation.FuelAllow + salaryInformation.UtilityAllow + salaryInformation.OtherAllow;

                            builder.AppendLine("Gross Pay " + grossPay.ToString());

                            if (salaryInformation.RevenueStamp > 0)
                            {
                                builder.AppendLine("Revenue Stamp " + salaryInformation.RevenueStamp.ToString());
                            }
                            if (salaryInformation.WelfareFund > 0)
                            {
                                builder.AppendLine("Welfare Fund " + salaryInformation.WelfareFund.ToString());
                            }
                            if (salaryInformation.EmployeeClub > 0)
                            {
                                builder.AppendLine("Employee Club " + salaryInformation.EmployeeClub.ToString());
                            }
                            if (salaryInformation.EmployeeUnion > 0)
                            {
                                builder.AppendLine("Employee Union " + salaryInformation.EmployeeUnion.ToString());
                            }
                            if (salaryInformation.ProvidentFund > 0)
                            {
                                builder.AppendLine("Provident Fund " + salaryInformation.ProvidentFund.ToString());
                            }
                            if (salaryInformation.ProvidentFund > 0)
                            {
                                builder.AppendLine("Provident Fund " + salaryInformation.ProvidentFund.ToString());
                            }
                            if (salaryInformation.Dormitory > 0)
                            {
                                builder.AppendLine("Dormitory " + salaryInformation.Dormitory.ToString());
                            }
                            if (salaryInformation.HospitalDeduction > 0)
                            {
                                builder.AppendLine("Hospital Deduction " + salaryInformation.HospitalDeduction.ToString());
                            }
                            if (salaryInformation.FuelReturn > 0)
                            {
                                builder.AppendLine("Fuel Return " + salaryInformation.FuelReturn.ToString());
                            }
                            if (salaryInformation.SpecialDeduction > 0)
                            {
                                builder.AppendLine("Special Deduction " + salaryInformation.SpecialDeduction.ToString());
                            }
                            if (salaryInformation.Advance > 0)
                            {
                                builder.AppendLine("Advance " + salaryInformation.Advance.ToString());
                            }
                            if (salaryInformation.Others > 0)
                            {
                                builder.AppendLine("Other Deduction " + salaryInformation.Others.ToString());
                            }
                            if (salaryInformation.HBLoan > 0)
                            {
                                builder.AppendLine("HB Loan " + salaryInformation.HBLoan.ToString());
                            }
                            if (salaryInformation.MCylLoan > 0)
                            {
                                builder.AppendLine("Mcyl Loan " + salaryInformation.MCylLoan.ToString());
                            }
                            if (salaryInformation.BCylLoan > 0)
                            {
                                builder.AppendLine("BCyl Loan " + salaryInformation.BCylLoan.ToString());
                            }
                            if (salaryInformation.ComputerLoan > 0)
                            {
                                builder.AppendLine("Computer Loan " + salaryInformation.ComputerLoan.ToString());
                            }
                            if (salaryInformation.PFLoan > 0)
                            {
                                builder.AppendLine("PF Loan " + salaryInformation.PFLoan.ToString());
                            }
                            if (salaryInformation.WPFLoan > 0)
                            {
                                builder.AppendLine("WPF Loan " + salaryInformation.WPFLoan.ToString());
                            }
                            if (salaryInformation.CosLoan > 0)
                            {
                                builder.AppendLine("COS Loan " + salaryInformation.CosLoan.ToString());
                            }
                            if (salaryInformation.OtherLoan > 0)
                            {
                                builder.AppendLine("Other Loan " + salaryInformation.OtherLoan.ToString());
                            }

                            decimal totalDeduction = salaryInformation.RevenueStamp + salaryInformation.WelfareFund + salaryInformation.EmployeeClub + salaryInformation.EmployeeUnion + salaryInformation.ProvidentFund +
                                salaryInformation.Dormitory + salaryInformation.HospitalDeduction + salaryInformation.FuelReturn + salaryInformation.SpecialDeduction + salaryInformation.Advance +
                                salaryInformation.Others + salaryInformation.HBLoan + salaryInformation.MCylLoan + salaryInformation.BCylLoan + salaryInformation.ComputerLoan +
                                salaryInformation.PFLoan + salaryInformation.WPFLoan + salaryInformation.CosLoan + salaryInformation.OtherLoan;

                            builder.AppendLine("Total Deduction " + totalDeduction.ToString());
                            decimal netPay = grossPay - totalDeduction;
                            builder.AppendLine("Net Pay " + netPay.ToString());


                            var clientForSMSSending = new HttpClient();
                            var response = clientForSMSSending.GetAsync("http://bulksms.smsvaults.work:7788/sendtext?apikey=93d0b5ff8e6cb424&secretkey=91ee219e&callerID=bgfcl&toUser=" + employeeInfo.MobileNumber + "&messageContent=" + builder);

                        }
                    }
                }

                _responseViewModel.StatusCode = 200;
                _responseViewModel.ResponseMessage = "Operation competed";
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
        public async Task<IActionResult> SalaryProcessWithJobcode(int monthId, int employeeTypeId, List<string> jobCodes = null)
        {
            try
            {
                // Ensure jobCodes is initialized to an empty list if it is null
                jobCodes ??= new List<string>();

                // Call the service method with the initialized jobCodes parameter
                var result = await _salarySettingContract.SalaryProcesswithJobcode(monthId, employeeTypeId, jobCodes);

                _responseViewModel.StatusCode = 201;
                _responseViewModel.ResponseMessage = "Operation completed successfully";
                return Ok(_responseViewModel);
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                _responseViewModel.StatusCode = 500;
                _responseViewModel.ResponseMessage = "Something went wrong";
                return Ok(_responseViewModel);
            }
        }


        [HttpGet]
        public async Task<IActionResult> FinalAdjustmentOfficer(int monthId)
        {
            try
            {
                var finalAdjustments = await _salarySettingContract.FinalAdjustmentOfficer(monthId);
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = finalAdjustments;
                _responseViewModel.ResponseMessage = "Operation competed";
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
        public async Task<IActionResult> JournalAdjustmentOfficer(int monthId, int employeeTypeId)
        {
            try
            {
                var finalAdjustments = await _salarySettingContract.JournalAdjustmentOfficer(monthId, employeeTypeId);
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = finalAdjustments;
                _responseViewModel.ResponseMessage = "Operation competed";
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
        public async Task<IActionResult> CheckNetPayOfficer(int monthId)
        {
            try
            {
                var finalAdjustments = await _salarySettingContract.CheckNetPayOfficer(monthId);
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = finalAdjustments;
                _responseViewModel.ResponseMessage = "Operation competed";
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
        public async Task<IActionResult> CheckNetPayJS(int monthId)
        {
            try
            {
                var finalAdjustments = await _salarySettingContract.CheckNetPayJuniorStaff(monthId);
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = finalAdjustments;
                _responseViewModel.ResponseMessage = "Operation competed";
                return Ok(_responseViewModel);
            }
            catch (Exception ex)
            {
                _responseViewModel.StatusCode = 500;
                _responseViewModel.ResponseMessage = "Something went wrong";
                return Ok(_responseViewModel);
            }
        }



        public class FinalAdjustments
        {
            public List<FinalAdjustmentOfficer>? FinalAdjustmentOfficers { get; set; }
            public List<FinalAdjustmentJuniorStaff>? FinalAdjustmentJuniorStaff { get; set; }
            public List<JournalAdjustmentOfficer>? JournalAdjustment { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateFinalAdjustmentOfficer(FinalAdjustments finalAdjustments)
        {
            try
            {
                if (finalAdjustments.FinalAdjustmentOfficers.Count > 0)
                {
                    foreach (var finalAdjustmentOfficer in finalAdjustments.FinalAdjustmentOfficers)
                    {
                        _salarySettingContract.UpdateFinalAdjustmentOfficer(finalAdjustmentOfficer);
                    }
                }
                _responseViewModel.StatusCode = 200;
                _responseViewModel.ResponseMessage = "Operation competed";
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
        public async Task<IActionResult> FinalAdjustmentJuniorStaff(int monthId)
        {
            try
            {
                var finalAdjustments = await _salarySettingContract.FinalAdjustmentJuniorStaff(monthId);
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = finalAdjustments;
                _responseViewModel.ResponseMessage = "Operation competed";
                return Ok(_responseViewModel);
            }
            catch (Exception ex)
            {
                _responseViewModel.StatusCode = 500;
                _responseViewModel.ResponseMessage = "Something went wrong";
                return Ok(_responseViewModel);
            }
        }

        public async Task<IActionResult> UpdateFinalAdjustmentJuniorStaff(FinalAdjustments finalAdjustments)
        {
            try
            {
                if (finalAdjustments.FinalAdjustmentJuniorStaff.Count > 0)
                {
                    foreach (var finalAdjustmentJuniorStaff in finalAdjustments.FinalAdjustmentJuniorStaff)
                    {
                        _salarySettingContract.UpdateFinalAdjustmentJuniorStaff(finalAdjustmentJuniorStaff);
                    }
                }
                _responseViewModel.StatusCode = 200;
                _responseViewModel.ResponseMessage = "Operation competed";
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
        public async Task<IActionResult> UpdateJournalAdjustmentOfficer(FinalAdjustments journalAdjustment)
        {
            if (journalAdjustment.JournalAdjustment == null || journalAdjustment.JournalAdjustment.Count==0)
            {
                return BadRequest(new { Message = "The salaryJournals field is required." });
            }

            try
            {
                foreach (var salaryJournal in journalAdjustment.JournalAdjustment)
                {
                    await _salarySettingContract.UpdateJournalAdjustmentOfficer(salaryJournal);
                }

                _responseViewModel.StatusCode = 200;
                _responseViewModel.ResponseMessage = "Operation completed";
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
        public async Task<IActionResult> JournalPosting(int monthId, int employeeTypeId)
        {
            var journalMaster = await _salarySettingContract.GetSalaryJournalMaster(monthId, employeeTypeId);
            if (journalMaster==null)
            {
                try
                {
                    List<SalaryJournal> journals = new List<SalaryJournal>();

                    List<string> journalCodes = new List<string>();

                    if (employeeTypeId == 1)
                    {
                        var salaryData = await _salarySettingContract.FinalAdjustmentOfficer(monthId);
                        foreach (var item in salaryData)
                        {
                            if (!journalCodes.Contains(item.JournalNumber))
                            {
                                journalCodes.Add(item.JournalNumber);
                            }

                        }
                        // this is for Revenue Stamp.
                        var totalRevenue = salaryData.Sum(s => s.RevenueStamp);
                        journals.Add(new SalaryJournal
                        {
                            AccountNumber = "5804000000",
                            JournalCode = "99",
                            Details = "Revenue Stamp",
                            Debit = null,
                            Credit = totalRevenue

                        });
                        // for Payroll Suspense A/C
                        //journals.Add(new SalaryJournal
                        //{
                        //    AccountNumber = "5800500000",
                        //    JournalCode = "99",
                        //    Details = "Payroll Suspense A/C",
                        //    Debit = null,
                        //    Credit = 0

                        //});

                        foreach (var item in journalCodes)
                        {
                            decimal basic = 0, specialBenefit = 0, lunch = 0, wash = 0, houseRent = 0, conveyance = 0, education = 0, charge = 0;
                            var journalCodeWiseSalaryData = salaryData.Where(s => s.JournalNumber == item).ToList();
                            foreach (var data in journalCodeWiseSalaryData)
                            {
                                basic += data.BasicSalary;
                                specialBenefit += data.SpecialBenefit;
                                lunch += data.LunchAllow;
                                wash += data.WashAllow;
                                houseRent += data.HouseRentAllow;
                                conveyance += data.Conveyance;
                                education += data.EducationalAllow;
                                charge += data.ChargeAllow;
                            }
                            // for Basic
                            journals.Add(new SalaryJournal
                            {
                                AccountNumber = "8100500000",
                                JournalCode = item,
                                Details = "Basic",
                                Debit = basic,
                                Credit = null

                            });
                            // for Special Benefit
                            journals.Add(new SalaryJournal
                            {
                                AccountNumber = "8101500000",
                                JournalCode = item,
                                Details = "Special Benefit",
                                Debit = specialBenefit,
                                Credit = null

                            });
                            // for Lunch
                            journals.Add(new SalaryJournal
                            {
                                AccountNumber = "8127000000",
                                JournalCode = item,
                                Details = "Lunch",
                                Debit = lunch,
                                Credit = null

                            });
                            // for Wash
                            journals.Add(new SalaryJournal
                            {
                                AccountNumber = "8116500000",
                                JournalCode = item,
                                Details = "Wash",
                                Debit = wash,
                                Credit = null

                            });
                            // for House Rent
                            journals.Add(new SalaryJournal
                            {
                                AccountNumber = "8120000000",
                                JournalCode = item,
                                Details = "House Rent",
                                Debit = houseRent,
                                Credit = null

                            });
                            // for Conveyance
                            journals.Add(new SalaryJournal
                            {
                                AccountNumber = "8109000000",
                                JournalCode = item,
                                Details = "Conveyance",
                                Debit = conveyance,
                                Credit = null

                            });
                            // for Education Allawance
                            journals.Add(new SalaryJournal
                            {
                                AccountNumber = "8129010000",
                                JournalCode = item,
                                Details = "Education Allawance",
                                Debit = education,
                                Credit = null

                            });
                            // for Charge Allawance
                            journals.Add(new SalaryJournal
                            {
                                AccountNumber = "8103500000",
                                JournalCode = item,
                                Details = "Charge Allawance",
                                Debit = charge,
                                Credit = null

                            });

                        }


                        // For TM bill
                        var mobileBill = salaryData.Sum(s => s.TMBill);
                        journals.Add(new SalaryJournal
                        {
                            AccountNumber = "9117000000",
                            JournalCode = "8062",
                            Details = "Mobile/Tel bill recovery",
                            Debit = null,
                            Credit = mobileBill

                        });


                        // For Con. Pension fund
                        var pensionFund = salaryData.Sum(s => s.PensionOfficer);
                        journals.Add(new SalaryJournal
                        {
                            AccountNumber = "6104500000",
                            JournalCode = "8028",
                            Details = "Co. Cont. to Pension",
                            Debit = null,
                            Credit = pensionFund,

                        });


                        // For Provident fund
                  
                        var prodFund = salaryData.Sum(s => s.ProvidentFund);
                        journals.Add(new SalaryJournal
                        {
                            AccountNumber = "8123500000",
                            JournalCode = "8028",
                            Details = "Co. Cont. to CPF",
                            Debit = null,
                            Credit = prodFund,

                        });

                        // For Income tax
                        var incomeTex = salaryData.Sum(s => s.IncomeTax);
                        journals.Add(new SalaryJournal
                        {
                            AccountNumber = "5804500000",
                            JournalCode = "99",
                            Details = "Income Tax",
                            Debit = null,
                            Credit = incomeTex

                        });

                        // For Welfare fund
                        var welfareFund = salaryData.Sum(s => s.WelfareFund);
                        journals.Add(new SalaryJournal
                        {
                            AccountNumber = "5805500000",
                            JournalCode = "99",
                            Details = "Welfare Fund",
                            Debit = null,
                            Credit = welfareFund

                        });


                        // For Medical  fund
                        var OffMedical = salaryData.Sum(s => s.MedicalFund);
                        journals.Add(new SalaryJournal
                        {
                            AccountNumber = "5808501000",
                            JournalCode = "99",
                            Details = "Off.Medical Fund",
                            Debit = null,
                            Credit = OffMedical

                        });

                        // For Officer Association
                        var OfficerAsso = salaryData.Sum(s => s.OfficerAssociation);
                        journals.Add(new SalaryJournal
                        {
                            AccountNumber = "5808502000",
                            JournalCode = "99",
                            Details = "Off.welfare association",
                            Debit = null,
                            Credit = OfficerAsso

                        });


                        // For Officer Club
                        var OfficerClub = salaryData.Sum(s => s.OfficerClub);
                        journals.Add(new SalaryJournal
                        {
                            AccountNumber = "5808503000",
                            JournalCode = "99",
                            Details = "Officer's Club",
                            Debit = null,
                            Credit = OfficerClub

                        });


                        // For MCL
                        var mcl = salaryData.Sum(s => s.MCylLoan);
                        journals.Add(new SalaryJournal
                        {
                            AccountNumber = "1802020000",
                            JournalCode = "99",
                            Details = "MCL",
                            Debit = null,
                            Credit = mcl

                        });


                        // For HBL
                        var hbl = salaryData.Sum(s => s.HBLoan);
                        journals.Add(new SalaryJournal
                        {
                            AccountNumber = "1801020000",
                            JournalCode = "99",
                            Details = "HBL",
                            Debit = null,
                            Credit = hbl

                        });
                        // For Cos loan
                        var cosloan = salaryData.Sum(s => s.CosLoan);
                        journals.Add(new SalaryJournal
                        {
                            AccountNumber = "5604001000",
                            JournalCode = "99",
                            Details = "Cos Loan",
                            Debit = null,
                            Credit = cosloan

                        });

                        // For WPF loan
                        var wpf = salaryData.Sum(s => s.WPFLoan);
                        journals.Add(new SalaryJournal
                        {
                            AccountNumber = "5604003000",
                            JournalCode = "99",
                            Details = "WPF Loan ",
                            Debit = null,
                            Credit = wpf

                        });
                        // For Car loan
                        var carloan = salaryData.Sum(s => s.CarLoan);
                        journals.Add(new SalaryJournal
                        {
                            AccountNumber = "5604004000",
                            JournalCode = "99",
                            Details = "Car Loan ",
                            Debit = null,
                            Credit = carloan

                        });

                        // Calculate total debit and credit
                        decimal totalDebit = journals.Where(j => j.Debit.HasValue).Sum(j => j.Debit.Value);
                        decimal totalCredit = journals.Where(j => j.Credit.HasValue).Sum(j => j.Credit.Value);

                        // Calculate the difference for Payroll Suspense A/C
                        decimal suspenseValue = Math.Abs(totalDebit - totalCredit);

                        // Add Payroll Suspense A/C based on which total is larger
                        if (totalDebit > totalCredit)
                        {
                            journals.Add(new SalaryJournal
                            {
                                AccountNumber = "5800500000",
                                JournalCode = "99",
                                Details = "Payroll Suspense A/C",
                                Debit = null,
                                Credit = suspenseValue
                            });
                        }
                        else
                        {
                            journals.Add(new SalaryJournal
                            {
                                AccountNumber = "5800500000",
                                JournalCode = "99",
                                Details = "Payroll Suspense A/C",
                                Debit = suspenseValue,
                                Credit = null
                            });
                        }


                        SalaryJournalMaster salaryJournalMaster = new SalaryJournalMaster();
                        salaryJournalMaster.MonthId = monthId;
                        salaryJournalMaster.EmployeeTypeId = employeeTypeId;
                        salaryJournalMaster.CreatedBy = "";
                        salaryJournalMaster.CreatedDate = DateTime.Now;
                        var journalMasterId = await _salarySettingContract.CreateSalaryJournalMaster(salaryJournalMaster);

                        if (journalMasterId > 0)
                        {
                            foreach (var journal in journals)
                            {
                                journal.JournalMasterId = journalMasterId;
                                await _salarySettingContract.CreateSalaryJournal(journal);
                            }
                        }
                        _responseViewModel.StatusCode = 201;
                        _responseViewModel.ResponseMessage = "Operation competed";
                        return Ok(_responseViewModel);
                    }
                    else
                    {
                        var salaryData = await _salarySettingContract.FinalAdjustmentJuniorStaff(monthId);
                        foreach (var item in salaryData)
                        {
                            if (!journalCodes.Contains(item.JournalNumber))
                            {
                                journalCodes.Add(item.JournalNumber);
                            }

                        }
                        // this is for Revenue Stamp.
                        var totalRevenue = salaryData.Sum(s => s.RevenueStamp);
                        journals.Add(new SalaryJournal
                        {
                            AccountNumber = "5804000000",
                            JournalCode = "99",
                            Details = "Revenue Stamp",
                            Debit = null,
                            Credit = totalRevenue

                        });
                        foreach (var item in journalCodes)
                        {
                            decimal basic = 0, specialBenefit = 0, lunch = 0, tiffin = 0,shift=0, wash = 0, houseRent = 0, conveyance = 0,fmedical =0,overtime=0,GasSubsidy=0,utility=0, education = 0, charge = 0;
                            var journalCodeWiseSalaryData = salaryData.Where(s => s.JournalNumber == item).ToList();
                            foreach (var data in journalCodeWiseSalaryData)
                            {
                                basic += data.BasicSalary;
                                specialBenefit += data.SpecialBenefit;
                                lunch += data.LunchAllow;
                                tiffin += data.TiffinAllow;
                                shift += data.ShiftAllow;
                                houseRent += data.HouseRentAllow;
                                conveyance += data.ConvenienceAllow;
                                fmedical += data.FamilyMedicalAllow;
                                education += data.EducationAllowance;
                                overtime += 0;
                                GasSubsidy += 0;
                                utility += data.UtilityAllow;
                            }
                            // for Basic
                            journals.Add(new SalaryJournal
                            {
                                AccountNumber = "8101000000",
                                JournalCode = item,
                                Details = "Basic",
                                Debit = basic,
                                Credit = null

                            });
                            // for Special Benefit
                            journals.Add(new SalaryJournal
                            {
                                AccountNumber = "8102000000",
                                JournalCode = item,
                                Details = "Special Benefit",
                                Debit = specialBenefit,
                                Credit = null

                            });
                            // for Lunch
                            journals.Add(new SalaryJournal
                            {
                                AccountNumber = "8127500000",
                                JournalCode = item,
                                Details = "Lunch",
                                Debit = lunch,
                                Credit = null

                            });
                            // for tiffin
                            journals.Add(new SalaryJournal
                            {
                                AccountNumber = "8113000000",
                                JournalCode = item,
                                Details = "Tiffin",
                                Debit = tiffin,
                                Credit = null

                            });
                            // for Shift
                            journals.Add(new SalaryJournal
                            {
                                AccountNumber = "8115000000",
                                JournalCode = item,
                                Details = "Shift",
                                Debit = shift,
                                Credit = null

                            });
                            // for House rent
                            journals.Add(new SalaryJournal
                            {
                                AccountNumber = "8120500000",
                                JournalCode = item,
                                Details = "Houser Rent",
                                Debit = houseRent,
                                Credit = null

                            });
                            // for Convience
                            journals.Add(new SalaryJournal
                            {
                                AccountNumber = "8109500000",
                                JournalCode = item,
                                Details = "Conveyance",
                                Debit = conveyance,
                                Credit = null

                            });
                            // for Femil Medical
                            journals.Add(new SalaryJournal
                            {
                                AccountNumber = "8112510000",
                                JournalCode = item,
                                Details = "Femily Medical",
                                Debit = fmedical,
                                Credit = null

                            });
                           
                            // for Education Allawance
                            journals.Add(new SalaryJournal
                            {
                                AccountNumber = "8129510000",
                                JournalCode = item,
                                Details = "Education Allawance",
                                Debit = education,
                                Credit = null

                            });
                            // for Over time
                            journals.Add(new SalaryJournal
                            {
                                AccountNumber = "8103000000",
                                JournalCode = item,
                                Details = "Over Time",
                                Debit = overtime,
                                Credit = null

                            });
                            // for Gas Subsidy
                            journals.Add(new SalaryJournal
                            {
                                AccountNumber = "8111500000",
                                JournalCode = item,
                                Details = "Gas Subsidy",
                                Debit = GasSubsidy,
                                Credit = null

                            });
                            // for Utility
                            journals.Add(new SalaryJournal
                            {
                                AccountNumber = "8136500000",
                                JournalCode = item,
                                Details = "Utility",
                                Debit = utility,
                                Credit = null

                            });
                        }

                        // For Con. Pension fund
                        var pensionFund = salaryData.Sum(s => s.PensionCom);
                        journals.Add(new SalaryJournal
                        {
                            AccountNumber = "8123000000",
                            JournalCode = "8025",
                            Details = "Co. Cont. to Pension",
                            Debit = null,
                            Credit = pensionFund,

                        });


                        // For Provident fund
                        var prodFund = salaryData.Sum(s => s.ProvidentFund);
                        journals.Add(new SalaryJournal
                        {
                            AccountNumber = "8123500000",
                            JournalCode = "8025",
                            Details = "Co. Cont. to CPF",
                            Debit = null,
                            Credit = prodFund,

                        });
                        // For EMP union
                        var empunion = salaryData.Sum(s => s.EmployeeUnion);
                        journals.Add(new SalaryJournal
                        {
                            AccountNumber = "5810001000",
                            JournalCode = "99",
                            Details = "EMP Union",
                            Debit = null,
                            Credit = empunion,

                        });
                        // For Welfare fund
                        var welfareFund = salaryData.Sum(s => s.WelfareFund);
                        journals.Add(new SalaryJournal
                        {
                            AccountNumber = "5805500000",
                            JournalCode = "99",
                            Details = "Welfare Fund",
                            Debit = null,
                            Credit = welfareFund,

                        });
                        // ForEMP Club
                        var empClub = salaryData.Sum(s => s.EmployeeClub);
                        journals.Add(new SalaryJournal
                        {
                            AccountNumber = "5810003000",
                            JournalCode = "99",
                            Details = "Emp Club",
                            Debit = null,
                            Credit = empClub,

                        });
                        // For CPF Contribution & loan
                        var cpfLoan = salaryData.Sum(s => s.EmployeeClub);
                        journals.Add(new SalaryJournal
                        {
                            AccountNumber = "5707500000",
                            JournalCode = "99",
                            Details = "CPF Contribution & loan",
                            Debit = null,
                            Credit = 0,//cpfLoan,

                        });
                        // For GPF Contribution & loan
                        var gpfLoan = salaryData.Sum(s => s.EmployeeClub);
                        journals.Add(new SalaryJournal
                        {
                            AccountNumber = "5704000000",
                            JournalCode = "99",
                            Details = "GPF Contribution & loan",
                            Debit = null,
                            Credit = 0,//gpfLoan,

                        });
                        // For pension
                        var pension = salaryData.Sum(s => s.PensionCom);
                        journals.Add(new SalaryJournal
                        {
                            AccountNumber = "6104500000",
                            JournalCode = "99",
                            Details = "Pension",
                            Debit = null,
                            Credit = pension,

                        });

                        // For MCL
                        var mcl = salaryData.Sum(s => s.MCylLoan);
                        journals.Add(new SalaryJournal
                        {
                            AccountNumber = "1802030000",
                            JournalCode = "99",
                            Details = "MCL",
                            Debit = null,
                            Credit = mcl

                        });
                        // For BCL
                        var bcl = salaryData.Sum(s => s.BCylLoan);
                        journals.Add(new SalaryJournal
                        {
                            AccountNumber = "1802510000",
                            JournalCode = "99",
                            Details = "BCL",
                            Debit = null,
                            Credit = bcl

                        });

                        // For HBL
                        var hbl = salaryData.Sum(s => s.HBLoan);
                        journals.Add(new SalaryJournal
                        {
                            AccountNumber = "1801030000",
                            JournalCode = "99",
                            Details = "HBL",
                            Debit = null,
                            Credit = hbl

                        });
                        // For Cos loan
                        var cosloan = salaryData.Sum(s => s.CosLoan);
                        journals.Add(new SalaryJournal
                        {
                            AccountNumber = "5604001000",
                            JournalCode = "99",
                            Details = "Cos Loan",
                            Debit = null,
                            Credit = cosloan

                        });

                        // For WPF loan
                        var wpf = salaryData.Sum(s => s.WPFLoan);
                        journals.Add(new SalaryJournal
                        {
                            AccountNumber = "5604003000",
                            JournalCode = "99",
                            Details = "WPF Loan ",
                            Debit = null,
                            Credit = wpf

                        });
                        // For com loan
                        var comloan = salaryData.Sum(s => s.ComputerLoan);
                        journals.Add(new SalaryJournal
                        {
                            AccountNumber = "1803030000",
                            JournalCode = "99",
                            Details = "Computer Loan ",
                            Debit = null,
                            Credit = comloan,

                        });
                        // For Hospitalization
                        var hospitalization = salaryData.Sum(s => s.HospitalDeduction);
                        journals.Add(new SalaryJournal
                        {
                            AccountNumber = "8130500000",
                            JournalCode = "8054",
                            Details = "Hospitalization ",
                            Debit = null,
                            Credit = hospitalization,

                        });
                        // Calculate total debit and credit
                        decimal totalDebit = journals.Where(j => j.Debit.HasValue).Sum(j => j.Debit.Value);
                        decimal totalCredit = journals.Where(j => j.Credit.HasValue).Sum(j => j.Credit.Value);

                        // Calculate the difference for Payroll Suspense A/C
                        decimal suspenseValue = Math.Abs(totalDebit - totalCredit);

                        // Add Payroll Suspense A/C based on which total is larger
                        if (totalDebit > totalCredit)
                        {
                            journals.Add(new SalaryJournal
                            {
                                AccountNumber = "5804000000",
                                JournalCode = "99",
                                Details = "Payroll Suspense A/C",
                                Debit = null,
                                Credit = suspenseValue
                            });
                        }
                        else
                        {
                            journals.Add(new SalaryJournal
                            {
                                AccountNumber = "5804000000",
                                JournalCode = "99",
                                Details = "Payroll Suspense A/C",
                                Debit = suspenseValue,
                                Credit = null
                            });
                        }

                        SalaryJournalMaster salaryJournalMaster = new SalaryJournalMaster();
                        salaryJournalMaster.MonthId = monthId;
                        salaryJournalMaster.EmployeeTypeId = employeeTypeId;
                        salaryJournalMaster.CreatedBy = "";
                        salaryJournalMaster.CreatedDate = DateTime.Now;
                        var journalMasterId = await _salarySettingContract.CreateSalaryJournalMaster(salaryJournalMaster);

                        var journalMaster1 = await _salarySettingContract.GetSalaryJournalMaster(monthId, employeeTypeId);

                        if (journalMasterId > 0)
                        {
                            foreach (var journal in journals)
                            {
                                // journal.JournalMasterId = journalMasterId;
                                journal.JournalMasterId = journalMaster1.Id;
                                await _salarySettingContract.CreateSalaryJournal(journal);
                            }
                        }
                        _responseViewModel.StatusCode = 201;
                        _responseViewModel.ResponseMessage = "Operation competed";
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
            else
            {
                _responseViewModel.StatusCode = 500;
                _responseViewModel.ResponseMessage = "Data already exist";
                return Ok(_responseViewModel);
            }
           
        }

        [HttpPost]
        public async Task<IActionResult> InsertJournalAdjustmentOfficer(List<JournalAdjustmentOfficer> SalaryJournals)
        {
            try
            {
                // Insert the new salary journal and retrieve the new ID
                foreach (var item in SalaryJournals)
                {
                    var newRecord = await _salarySettingContract.InsertNewSalaryJournal(item);
                }

                _responseViewModel.StatusCode = 200;
                _responseViewModel.ResponseMessage = "Operation completed";
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
        public async Task<IActionResult> DeleteSalaryjournalOfficer(int id)
        {
            try
            {
                var result = await _salarySettingContract.DeleteSalaryJournalOff(id);

                if (result > 0) // Check if rows were affected
                {
                    _responseViewModel.StatusCode = 200;
                    _responseViewModel.ResponseMessage = "Operation completed";
                }
                else
                {
                    _responseViewModel.StatusCode = 404;
                    _responseViewModel.ResponseMessage = "Record not found";
                }

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
        public async Task<IActionResult> LatestSalaryProcess()
        {
            try
            {
                var data = await _salarySettingContract.GetLastMonthSalaryProcess();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = data;
                _responseViewModel.ResponseMessage = "Operation competed";
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
        public async Task<IActionResult> GetNetpayOf()
        {
            try
            {
                var data = await _salarySettingContract.GetSalaryOfNetpayAsync();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = data;
                _responseViewModel.ResponseMessage = "Operation competed";
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
        public async Task<IActionResult> GetPayrollData(int monthId)
        {
            try
            {
                var data = await _salarySettingContract.GetPayrollData(monthId);
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = data;
                _responseViewModel.ResponseMessage = "Operation competed";
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
        public async Task<IActionResult> GetSalaryNetpayPermanent()
        {
            try
            {
                var data = await _salarySettingContract.GetSalaryNetpayPermanent();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = data;
                _responseViewModel.ResponseMessage = "Operation competed";
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
        public async Task<IActionResult> GetSalaryGrossPermanent()
        {
            try
            {
                var data = await _salarySettingContract.GetSalaryGrossPermanent();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = data;
                _responseViewModel.ResponseMessage = "Operation competed";
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
        public async Task<IActionResult> GetSalaryNetpayContract()
        {
            try
            {
                var data = await _salarySettingContract.GetSalaryNetpayContract();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = data;
                _responseViewModel.ResponseMessage = "Operation competed";
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
        public async Task<IActionResult> GetSalaryGrossContract()
        {
            try
            {
                var data = await _salarySettingContract.GetSalaryGrossContract();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = data;
                _responseViewModel.ResponseMessage = "Operation competed";
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
        public async Task<IActionResult> GetRevenueStampPermanent()
        {
            try
            {
                var data = await _salarySettingContract.GetRevenueStampPermanent();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = data;
                _responseViewModel.ResponseMessage = "Operation competed";
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
        public async Task<IActionResult> GetRevenueStampContruct()
        {
            try
            {
                var data = await _salarySettingContract.GetRevenueStampContruct();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = data;
                _responseViewModel.ResponseMessage = "Operation competed";
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
        public async Task<IActionResult> GetPFPermanent()
        {
            try
            {
                var data = await _salarySettingContract.GetPFPermanent();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = data;
                _responseViewModel.ResponseMessage = "Operation competed";
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
        public async Task<IActionResult> GetPFContruct()
        {
            try
            {
                var data = await _salarySettingContract.GetPFContruct();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = data;
                _responseViewModel.ResponseMessage = "Operation competed";
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
        public async Task<IActionResult> GetTotalDeductionPermanent()
        {
            try
            {
                var data = await _salarySettingContract.GetTotalDeductionPermanent();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = data;
                _responseViewModel.ResponseMessage = "Operation competed";
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
        public async Task<IActionResult> GetTotalDeductionContruct()
        {
            try
            {
                var data = await _salarySettingContract.GetTotalDeductionContruct();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = data;
                _responseViewModel.ResponseMessage = "Operation competed";
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
        public async Task<IActionResult> GetNetpayJs()
        {
            try
            {
                var data = await _salarySettingContract.GetSalaryNetpayJS();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = data;
                _responseViewModel.ResponseMessage = "Operation competed";
                return Ok(_responseViewModel);
            }
            catch (Exception ex)
            {
                _responseViewModel.StatusCode = 500;
                _responseViewModel.ResponseMessage = "Something went wrong";
                return Ok(_responseViewModel);
            }
        }
        #endregion
        public async Task<IActionResult> GetSalaryReportOfficerData()
        {
            try
            {
                var data = await _salaryReportOfficerContract.GetSalaryReportOfficerData();

                if (data == null || !data.Any())
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        ResponseMessage = "No data found",
                        Errors = new { },
                        Data = new List<SalaryReportOfficer>()
                    });
                }

                var response = new
                {
                    StatusCode = 200,
                    ResponseMessage = "Operation succeeded",
                    Errors = new { },
                    Data = data
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    ResponseMessage = "Internal Server Error",
                    Errors = new { Message = ex.Message },
                    Data = new List<SalaryReportOfficer>()
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetreportOfficerWithFilter(int employeeTypeId, int school_filter, int role_filter, int department_filter, int designation_filter)
        {
            try
            {
                var employees = await _salaryReportOfficerContract.GetreportOfficerWithFilter(
                    employeeTypeId,
                    school_filter > 0 ? school_filter : (int?)null,
                    role_filter > 0 ? role_filter : (int?)null,
                    department_filter > 0 ? department_filter : (int?)null,
                    designation_filter > 0 ? designation_filter : (int?)null
                );

                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = employees;
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
        public async Task<IActionResult> GetSalaryReportContractData()
        {

            var data = await _salaryReportOfficerContract.GetSalaryReportContractData();
            _responseViewModel.Data = data;
            _responseViewModel.ResponseMessage = "Operation succeed";
            _responseViewModel.StatusCode = 200;
            return Ok(_responseViewModel);
        }
        [HttpGet]
        public async Task<IActionResult> GetreportContractWithFilter(int employeeTypeId, int school_filter, int role_filter, int department_filter, int designation_filter)
        {
            try
            {
                var employees = await _salaryReportOfficerContract.GetreportContractWithFilter(
                    employeeTypeId,
                    school_filter > 0 ? school_filter : (int?)null,
                    role_filter > 0 ? role_filter : (int?)null,
                    department_filter > 0 ? department_filter : (int?)null,
                    designation_filter > 0 ? designation_filter : (int?)null
                );

                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = employees;
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
        public async Task<IActionResult> SalarySettinsConfig([FromBody] List<SalarySettingConfigDetails> salarysettings)
        {
            var result = await _salarySettingContract.SalarySettinsConfig(salarysettings);

            _responseViewModel.Data = result; // result = number of inserted records
            _responseViewModel.ResponseMessage = "Operation succeeded";
            _responseViewModel.StatusCode = 200;

            return Ok(_responseViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> InvestmentSchedule([FromBody] InvestmentSchedule investment)
        {
            var result = "";// await _salarySettingContract.InvestmentSchedule(investment);

            _responseViewModel.Data = result; 
            _responseViewModel.ResponseMessage = "Operation succeeded";
            _responseViewModel.StatusCode = 200;

            return Ok(_responseViewModel);
        }


    }
}
