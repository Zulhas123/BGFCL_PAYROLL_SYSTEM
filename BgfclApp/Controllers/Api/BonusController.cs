using BgfclApp.ViewModels;
using Contracts;
using Entities;
using Entities.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BgfclApp.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BonusController : ControllerBase
    {
        private ResponseViewModel _responseViewModel;
        private IBonusContract _bonusContract;
        private IEmployeeContract _employeeContract;
        private ISalarySettingContract _salarySettingContract;
        private IDepartmentContract _departmentContract;
        private IDesignationContract _designationContract;
        private IBankContract _bankContract;
        private IBranchContract _branchContract;
        public BonusController(ResponseViewModel responseViewModel, IBonusContract bonusContract, IEmployeeContract employeeContract, ISalarySettingContract salarySettingContract, IDepartmentContract departmentContract,IDesignationContract designationContract,IBankContract bankContract, IBranchContract branchContract)
        {
            _responseViewModel = responseViewModel;
            _bonusContract = bonusContract;
            _employeeContract = employeeContract;
            _salarySettingContract = salarySettingContract;
            _departmentContract = departmentContract;
            _designationContract = designationContract;
            _bankContract = bankContract;
            _branchContract = branchContract;

        }
        [HttpPost]
        public async Task<IActionResult> CreateBonus([FromForm] Bonus bonus)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (System.String.IsNullOrEmpty(bonus.BonusTitle))
                {
                    _responseViewModel.Errors.Add(nameof(bonus.BonusTitle) + "Error", "Bonus Title required");
                }
                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                var bonusList = await _bonusContract.GetBonus();
                var existingBonus = bonusList.Where(b=>b.PayableMonth==bonus.PayableMonth && b.BonusTitle==bonus.BonusTitle).SingleOrDefault();
                if (existingBonus == null)
                {
                    bonus.CreatedBy = "";
                    bonus.CreatedDate = DateTime.Now;
                    bonus.IsActive = true;
                    int result = await _bonusContract.CreateBonus(bonus);

                    _responseViewModel.ResponseMessage = "Record saved successfully";
                    _responseViewModel.StatusCode = 201;
                    return Ok(_responseViewModel);
                }
                else
                {
                    _responseViewModel.ResponseMessage = "Record already exists";
                    _responseViewModel.Errors.Add(nameof(bonus.BonusTitle) + "Error", "Bonus Title exists");
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
        public async Task<IActionResult> GetBonus()
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                var bonusList = await _bonusContract.GetBonus();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = bonusList;
                return Ok(_responseViewModel);


            }
            catch (Exception ex)
            {
                _responseViewModel.ResponseMessage = "Something went wrong";
                _responseViewModel.StatusCode = 500;
                return Ok(_responseViewModel);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetLastBonusName()
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                var bonusList = await _bonusContract.GetLastBonusName();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = bonusList;
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
        public async Task<IActionResult> RemoveBonus(int id)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                var existingBonus = await _bonusContract.GetBonusById(id);

                if (existingBonus == null)
                {
                    _responseViewModel.ResponseMessage = "Record not found";
                    _responseViewModel.StatusCode = 404;
                    return Ok(_responseViewModel);
                }

                int result = await _bonusContract.RemoveBonus(existingBonus.Id);

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

        [HttpPut]
        public async Task<IActionResult> UpdateBonus([FromForm] Bonus bonus)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (System.String.IsNullOrEmpty(bonus.BonusTitle))
                {
                    _responseViewModel.Errors.Add(nameof(bonus.BonusTitle) + "Error", "Bonus Title required");
                }
                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                var existingBonus = await _bonusContract.GetBonusById(bonus.Id);
                if (existingBonus != null)
                {
                    bonus.UpdatedBy = "";
                    bonus.UpdatedDate = DateTime.Now;
                    int result = await _bonusContract.UpdateBonus(bonus);

                    _responseViewModel.ResponseMessage = "Record updated successfully";
                    _responseViewModel.StatusCode = 201;
                    return Ok(_responseViewModel);
                }
                else
                {
                    _responseViewModel.ResponseMessage = "No record found";
                    _responseViewModel.Errors.Add(nameof(bonus.BonusTitle) + "Error", "Bonus Title exists");
                    _responseViewModel.StatusCode = 404;
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
        public async Task<IActionResult> GetBonusById(int id)
        {
            try
            {
                var existingBonus = await _bonusContract.GetBonusById(id);
                if (existingBonus==null)
                {
                    _responseViewModel.ResponseMessage = "No data found";
                    _responseViewModel.StatusCode = 404;
                    return Ok(_responseViewModel);
                }
                else
                {
                    _responseViewModel.StatusCode = 200;
                    _responseViewModel.Data = existingBonus;
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
        public async Task<IActionResult> GetBonusByMonthId(int monthId)
        {
            try
            {
                var bonus = await _bonusContract.GetBonusByMonthId(monthId);

                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = bonus;
                return Ok(_responseViewModel);

            }
            catch (Exception ex)
            {
                _responseViewModel.ResponseMessage = "Something went wrong";
                _responseViewModel.StatusCode = 500;
                return Ok(_responseViewModel);
            }
        }

        public class BonusProcessData
        {
            public string? BonusName { get; set; }
            public int EmployeeTypeId { get; set; }
            public int BonusId { get; set; }
            public bool Hindu { get; set; }
            public bool Muslim { get; set; }
            public bool Buddist { get; set; }
            public bool Christian { get; set; }
            public double Basic { get; set; }
            public string? DataType { get; set; }

            public string? DataString { get; set; }

        }

        public class BonusAmount
        {
            public string JobCode { get; set; }
            public decimal Amount { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> ProcessBonus([FromForm] BonusProcessData bonusProcessData)
        {
            try
            {
                await _bonusContract.DeleteBonusSheet(bonusProcessData.BonusId, bonusProcessData.EmployeeTypeId);
                var employeeList = await _employeeContract.GetEmployees(1, bonusProcessData.EmployeeTypeId);
                //List<SalaryPolicySetting> salaryPolicySettings = await _salarySettingContract.GetSalaryPolicySettings();

                if (bonusProcessData.BonusName == "festival")
                {
                    if (bonusProcessData.Muslim)
                    {
                        List<Employee> filteredEmployee = new List<Employee>();
                        foreach (var item in employeeList)
                        {
                            Employee employee = await _employeeContract.GetEmployeeById(item.Id);
                            if (employee.ReligionId == 1)
                            { 
                                filteredEmployee.Add(employee);
                            }
                        }

                        foreach (var item in filteredEmployee)
                        {
                            Department department = await _departmentContract.GetDepartmentById(item.DepartmentId);
                            Designation designation = await _designationContract.GetDesignationById(item.DesignationId);

                            BonusControlSheet bonusControlSheet = new BonusControlSheet();

                            bonusControlSheet.IncentiveBonus = 0;
                            bonusControlSheet.Scholarship = 0;
                            bonusControlSheet.HonorariumBonus = 0;
                            bonusControlSheet.BonusId = bonusProcessData.BonusId;
                            bonusControlSheet.JobCode = item.JobCode;
                            bonusControlSheet.EmployeeName = item.EmployeeName;
                            bonusControlSheet.DepartmentName = department.DepartmentName;
                            bonusControlSheet.DesignationName = designation.DesignationName;
                            bonusControlSheet.JournalCode = item.JournalCode;
                            bonusControlSheet.TIN = item.TinNo;
                            bonusControlSheet.OtherDeduction = 0;
                            bonusControlSheet.SchoolId = item.SchoolId;
                            bonusControlSheet.UserId = item.UserId;
                            bonusControlSheet.RoleId = item.RoleId;
                            bonusControlSheet.GuestPkId = item.GuestPkId;




                            if (bonusProcessData.EmployeeTypeId == 1)
                            {

                               // SalaryPolicySetting salaryPolicySetting = salaryPolicySettings.Where(s => s.EmployeeTypeId == 1).SingleOrDefault();
                                SalarySettingsOfficer salarySettingsOfficer = await _salarySettingContract.GetSalarySettingsOfficerByJobCode(item.JobCode);
                                Bank bank = await _bankContract.GetBankById(salarySettingsOfficer.BankId);
                                Branch branch = await _branchContract.GetBranchesById(salarySettingsOfficer.BankBranchId);
                                if (bank == null)
                                {
                                    bonusControlSheet.BankName = "";
                                    bonusControlSheet.BankId = 0;
                                }
                                else
                                {
                                    bonusControlSheet.BankName = bank.BankName;
                                    bonusControlSheet.BankId = bank.Id;
                                }

                                if (branch == null)
                                {
                                    bonusControlSheet.BankBranchName = "";
                                }
                                else
                                {
                                    bonusControlSheet.BankBranchName = branch.BranchName;
                                }
                                bonusControlSheet.AccountNumber = salarySettingsOfficer.AccountNumber;
                                bonusControlSheet.EmployeeTypeId = 1;
                                //bonusControlSheet.RevenueStamp = (decimal)salaryPolicySetting.RevenueStamp;
                                bonusControlSheet.FestivalBonus = (decimal)(salarySettingsOfficer.BasicSalary * (bonusProcessData.Basic / 100));


                            }
                            else
                            {
                                //SalaryPolicySetting salaryPolicySetting = salaryPolicySettings.Where(s => s.EmployeeTypeId == 2).SingleOrDefault();
                                SalarySettingsJuniorStaff salarySettingsJuniorStaff = await _salarySettingContract.GetSalarySettingsJuniorStaffByJobCode(item.JobCode);
                                Bank bank = await _bankContract.GetBankById(salarySettingsJuniorStaff.BankId);
                                Branch branch = await _branchContract.GetBranchesById(salarySettingsJuniorStaff.BankBranchId);
                                if (bank == null)
                                {
                                    bonusControlSheet.BankName = "";
                                    bonusControlSheet.BankId = 0;
                                }
                                else
                                {
                                    bonusControlSheet.BankName = bank.BankName;
                                    bonusControlSheet.BankId = bank.Id;
                                }

                                if (branch == null)
                                {
                                    bonusControlSheet.BankBranchName = "";
                                }
                                else
                                {
                                    bonusControlSheet.BankBranchName = branch.BranchName;
                                }
                                bonusControlSheet.AccountNumber = salarySettingsJuniorStaff.AccountNumber;
                                bonusControlSheet.EmployeeTypeId = 2;
                                //bonusControlSheet.RevenueStamp = (decimal)salaryPolicySetting.RevenueStamp;
                                bonusControlSheet.FestivalBonus = (decimal)(salarySettingsJuniorStaff.BasicSalary * (bonusProcessData.Basic / 100));
                            }

                            await _bonusContract.CreateBonusSheet(bonusControlSheet);
                        }

                    }
                    if (bonusProcessData.Hindu)
                    {
                        List<Employee> filteredEmployee = new List<Employee>();
                        foreach (var item in employeeList)
                        {
                            Employee employee = await _employeeContract.GetEmployeeById(item.Id);
                            if (employee.ReligionId == 2)
                            {
                                filteredEmployee.Add(employee);
                            }
                        }

                        foreach (var item in filteredEmployee)
                        {
                            Department department = await _departmentContract.GetDepartmentById(item.DepartmentId);
                            Designation designation = await _designationContract.GetDesignationById(item.DesignationId);

                            BonusControlSheet bonusControlSheet = new BonusControlSheet();

                            bonusControlSheet.IncentiveBonus = 0;
                            bonusControlSheet.Scholarship = 0;
                            bonusControlSheet.HonorariumBonus = 0;
                            bonusControlSheet.BonusId = bonusProcessData.BonusId;
                            bonusControlSheet.JobCode = item.JobCode;
                            bonusControlSheet.EmployeeName = item.EmployeeName;
                            bonusControlSheet.DepartmentName = department.DepartmentName;
                            bonusControlSheet.DesignationName = designation.DesignationName;
                            bonusControlSheet.JournalCode = item.JournalCode;
                            bonusControlSheet.TIN = item.TinNo;
                            bonusControlSheet.OtherDeduction = 0;



                            if (bonusProcessData.EmployeeTypeId == 1)
                            {

                                //SalaryPolicySetting salaryPolicySetting = salaryPolicySettings.Where(s => s.EmployeeTypeId == 1).SingleOrDefault();
                                SalarySettingsOfficer salarySettingsOfficer = await _salarySettingContract.GetSalarySettingsOfficerByJobCode(item.JobCode);
                                Bank bank = await _bankContract.GetBankById(salarySettingsOfficer.BankId);
                                Branch branch = await _branchContract.GetBranchesById(salarySettingsOfficer.BankBranchId);
                                if (bank == null)
                                {
                                    bonusControlSheet.BankName = "";
                                    bonusControlSheet.BankId = 0;
                                }
                                else
                                {
                                    bonusControlSheet.BankName = bank.BankName;
                                    bonusControlSheet.BankId = bank.Id;
                                }

                                if (branch == null)
                                {
                                    bonusControlSheet.BankBranchName = "";
                                }
                                else
                                {
                                    bonusControlSheet.BankBranchName = branch.BranchName;
                                }
                                bonusControlSheet.AccountNumber = salarySettingsOfficer.AccountNumber;
                                bonusControlSheet.EmployeeTypeId = 1;
                               // bonusControlSheet.RevenueStamp = (decimal)salaryPolicySetting.RevenueStamp;
                                bonusControlSheet.FestivalBonus = (decimal)(salarySettingsOfficer.BasicSalary * (bonusProcessData.Basic / 100));


                            }
                            else
                            {
                                //SalaryPolicySetting salaryPolicySetting = salaryPolicySettings.Where(s => s.EmployeeTypeId == 2).SingleOrDefault();
                                SalarySettingsJuniorStaff salarySettingsJuniorStaff = await _salarySettingContract.GetSalarySettingsJuniorStaffByJobCode(item.JobCode);
                                Bank bank = await _bankContract.GetBankById(salarySettingsJuniorStaff.BankId);
                                Branch branch = await _branchContract.GetBranchesById(salarySettingsJuniorStaff.BankBranchId);
                                if (bank == null)
                                {
                                    bonusControlSheet.BankName = "";
                                    bonusControlSheet.BankId = 0;
                                }
                                else
                                {
                                    bonusControlSheet.BankName = bank.BankName;
                                    bonusControlSheet.BankId = bank.Id;
                                }

                                if (branch == null)
                                {
                                    bonusControlSheet.BankBranchName = "";
                                }
                                else
                                {
                                    bonusControlSheet.BankBranchName = branch.BranchName;
                                }
                                bonusControlSheet.AccountNumber = salarySettingsJuniorStaff.AccountNumber;
                                bonusControlSheet.EmployeeTypeId = 2;
                                //bonusControlSheet.RevenueStamp = (decimal)salaryPolicySetting.RevenueStamp;
                                bonusControlSheet.FestivalBonus = (decimal)(salarySettingsJuniorStaff.BasicSalary * (bonusProcessData.Basic / 100));
                            }

                            await _bonusContract.CreateBonusSheet(bonusControlSheet);
                        }
                    }
                    if (bonusProcessData.Buddist)
                    {
                        List<Employee> filteredEmployee = new List<Employee>();
                        foreach (var item in employeeList)
                        {
                            Employee employee = await _employeeContract.GetEmployeeById(item.Id);
                            if (employee.ReligionId == 3)
                            {
                                filteredEmployee.Add(employee);
                            }
                        }

                        foreach (var item in filteredEmployee)
                        {
                            Department department = await _departmentContract.GetDepartmentById(item.DepartmentId);
                            Designation designation = await _designationContract.GetDesignationById(item.DesignationId);

                            BonusControlSheet bonusControlSheet = new BonusControlSheet();

                            bonusControlSheet.IncentiveBonus = 0;
                            bonusControlSheet.Scholarship = 0;
                            bonusControlSheet.HonorariumBonus = 0;
                            bonusControlSheet.BonusId = bonusProcessData.BonusId;
                            bonusControlSheet.JobCode = item.JobCode;
                            bonusControlSheet.EmployeeName = item.EmployeeName;
                            bonusControlSheet.DepartmentName = department.DepartmentName;
                            bonusControlSheet.DesignationName = designation.DesignationName;
                            bonusControlSheet.JournalCode = item.JournalCode;
                            bonusControlSheet.TIN = item.TinNo;
                            bonusControlSheet.OtherDeduction = 0;



                            if (bonusProcessData.EmployeeTypeId == 1)
                            {

                                //SalaryPolicySetting salaryPolicySetting = salaryPolicySettings.Where(s => s.EmployeeTypeId == 1).SingleOrDefault();
                                SalarySettingsOfficer salarySettingsOfficer = await _salarySettingContract.GetSalarySettingsOfficerByJobCode(item.JobCode);
                                Bank bank = await _bankContract.GetBankById(salarySettingsOfficer.BankId);
                                Branch branch = await _branchContract.GetBranchesById(salarySettingsOfficer.BankBranchId);
                                if (bank == null)
                                {
                                    bonusControlSheet.BankName = "";
                                    bonusControlSheet.BankId = 0;
                                }
                                else
                                {
                                    bonusControlSheet.BankName = bank.BankName;
                                    bonusControlSheet.BankId = bank.Id;
                                }

                                if (branch == null)
                                {
                                    bonusControlSheet.BankBranchName = "";
                                }
                                else
                                {
                                    bonusControlSheet.BankBranchName = branch.BranchName;
                                }
                                bonusControlSheet.AccountNumber = salarySettingsOfficer.AccountNumber;
                                bonusControlSheet.EmployeeTypeId = 1;
                                //bonusControlSheet.RevenueStamp = (decimal)salaryPolicySetting.RevenueStamp;
                                bonusControlSheet.FestivalBonus = (decimal)(salarySettingsOfficer.BasicSalary * (bonusProcessData.Basic / 100));


                            }
                            else
                            {
                                //SalaryPolicySetting salaryPolicySetting = salaryPolicySettings.Where(s => s.EmployeeTypeId == 2).SingleOrDefault();
                                SalarySettingsJuniorStaff salarySettingsJuniorStaff = await _salarySettingContract.GetSalarySettingsJuniorStaffByJobCode(item.JobCode);
                                Bank bank = await _bankContract.GetBankById(salarySettingsJuniorStaff.BankId);
                                Branch branch = await _branchContract.GetBranchesById(salarySettingsJuniorStaff.BankBranchId);
                                if (bank == null)
                                {
                                    bonusControlSheet.BankName = "";
                                    bonusControlSheet.BankId = 0;
                                }
                                else
                                {
                                    bonusControlSheet.BankName = bank.BankName;
                                    bonusControlSheet.BankId = bank.Id;
                                }

                                if (branch == null)
                                {
                                    bonusControlSheet.BankBranchName = "";
                                }
                                else
                                {
                                    bonusControlSheet.BankBranchName = branch.BranchName;
                                }
                                bonusControlSheet.AccountNumber = salarySettingsJuniorStaff.AccountNumber;
                                bonusControlSheet.EmployeeTypeId = 2;
                                //bonusControlSheet.RevenueStamp = (decimal)salaryPolicySetting.RevenueStamp;
                                bonusControlSheet.FestivalBonus = (decimal)(salarySettingsJuniorStaff.BasicSalary * (bonusProcessData.Basic / 100));
                            }

                            await _bonusContract.CreateBonusSheet(bonusControlSheet);
                        }
                    }
                    if (bonusProcessData.Christian)
                    {
                        List<Employee> filteredEmployee = new List<Employee>();
                        foreach (var item in employeeList)
                        {
                            Employee employee = await _employeeContract.GetEmployeeById(item.Id);
                            if (employee.ReligionId == 4)
                            {
                                filteredEmployee.Add(employee);
                            }
                        }

                        foreach (var item in filteredEmployee)
                        {
                            Department department = await _departmentContract.GetDepartmentById(item.DepartmentId);
                            Designation designation = await _designationContract.GetDesignationById(item.DesignationId);

                            BonusControlSheet bonusControlSheet = new BonusControlSheet();

                            bonusControlSheet.IncentiveBonus = 0;
                            bonusControlSheet.Scholarship = 0;
                            bonusControlSheet.HonorariumBonus = 0;
                            bonusControlSheet.BonusId = bonusProcessData.BonusId;
                            bonusControlSheet.JobCode = item.JobCode;
                            bonusControlSheet.EmployeeName = item.EmployeeName;
                            bonusControlSheet.DepartmentName = department.DepartmentName;
                            bonusControlSheet.DesignationName = designation.DesignationName;
                            bonusControlSheet.JournalCode = item.JournalCode;
                            bonusControlSheet.TIN = item.TinNo;
                            bonusControlSheet.OtherDeduction = 0;



                            if (bonusProcessData.EmployeeTypeId == 1)
                            {

                                //SalaryPolicySetting salaryPolicySetting = salaryPolicySettings.Where(s => s.EmployeeTypeId == 1).SingleOrDefault();
                                SalarySettingsOfficer salarySettingsOfficer = await _salarySettingContract.GetSalarySettingsOfficerByJobCode(item.JobCode);
                                Bank bank = await _bankContract.GetBankById(salarySettingsOfficer.BankId);
                                Branch branch = await _branchContract.GetBranchesById(salarySettingsOfficer.BankBranchId);
                                if (bank == null)
                                {
                                    bonusControlSheet.BankName = "";
                                    bonusControlSheet.BankId = 0;
                                }
                                else
                                {
                                    bonusControlSheet.BankName = bank.BankName;
                                    bonusControlSheet.BankId = bank.Id;
                                }

                                if (branch == null)
                                {
                                    bonusControlSheet.BankBranchName = "";
                                }
                                else
                                {
                                    bonusControlSheet.BankBranchName = branch.BranchName;
                                }
                                bonusControlSheet.AccountNumber = salarySettingsOfficer.AccountNumber;
                                bonusControlSheet.EmployeeTypeId = 1;
                               // bonusControlSheet.RevenueStamp = (decimal)salaryPolicySetting.RevenueStamp;
                                bonusControlSheet.FestivalBonus = (decimal)(salarySettingsOfficer.BasicSalary * (bonusProcessData.Basic / 100));


                            }
                            else
                            {
                                //SalaryPolicySetting salaryPolicySetting = salaryPolicySettings.Where(s => s.EmployeeTypeId == 2).SingleOrDefault();
                                SalarySettingsJuniorStaff salarySettingsJuniorStaff = await _salarySettingContract.GetSalarySettingsJuniorStaffByJobCode(item.JobCode);
                                Bank bank = await _bankContract.GetBankById(salarySettingsJuniorStaff.BankId);
                                Branch branch = await _branchContract.GetBranchesById(salarySettingsJuniorStaff.BankBranchId);
                                if (bank == null)
                                {
                                    bonusControlSheet.BankName = "";
                                    bonusControlSheet.BankId = 0;
                                }
                                else
                                {
                                    bonusControlSheet.BankName = bank.BankName;
                                    bonusControlSheet.BankId = bank.Id;

                                }

                                if (branch == null)
                                {
                                    bonusControlSheet.BankBranchName = "";
                                }
                                else
                                {
                                    bonusControlSheet.BankBranchName = branch.BranchName;
                                }
                                bonusControlSheet.AccountNumber = salarySettingsJuniorStaff.AccountNumber;
                                bonusControlSheet.EmployeeTypeId = 2;
                                //bonusControlSheet.RevenueStamp = (decimal)salaryPolicySetting.RevenueStamp;
                                bonusControlSheet.FestivalBonus = (decimal)(salarySettingsJuniorStaff.BasicSalary * (bonusProcessData.Basic / 100));
                            }

                            await _bonusContract.CreateBonusSheet(bonusControlSheet);
                        }
                    }


                }
                if (bonusProcessData.BonusName == "incentive")
                {

                    if (bonusProcessData.DataType == "f")
                    {
                        List<BonusAmount> bonusAmounts = new List<BonusAmount>();

                        string[] _data = bonusProcessData.DataString.Split('#');

                        for (int i = 0; i < _data.Length - 1; i++)
                        {
                            string[] _amountData = _data[i].Split('_');
                            BonusAmount bonusAmount = new BonusAmount();
                            bonusAmount.JobCode = _amountData[0];
                            bonusAmount.Amount = Convert.ToDecimal(_amountData[1]);
                            bonusAmounts.Add(bonusAmount);
                        }
                        foreach (var item in bonusAmounts)
                        {
                            Employee employee = await _employeeContract.GetEmployeeByjobCode(item.JobCode.Trim());

                            Department department = await _departmentContract.GetDepartmentById(employee.DepartmentId);
                            Designation designation = await _designationContract.GetDesignationById(employee.DesignationId);

                            BonusControlSheet bonusControlSheet = new BonusControlSheet();

                            bonusControlSheet.HonorariumBonus = 0;
                            bonusControlSheet.Scholarship = 0;
                            bonusControlSheet.FestivalBonus = 0;
                            bonusControlSheet.BonusId = bonusProcessData.BonusId;
                            bonusControlSheet.JobCode = item.JobCode;
                            bonusControlSheet.EmployeeName = employee.EmployeeName;
                            bonusControlSheet.DepartmentName = department.DepartmentName;
                            bonusControlSheet.DesignationName = designation.DesignationName;
                            bonusControlSheet.JournalCode = employee.JournalCode;
                            bonusControlSheet.TIN = employee.TinNo;
                            bonusControlSheet.OtherDeduction = 0;



                            if (bonusProcessData.EmployeeTypeId == 1)
                            {

                                //SalaryPolicySetting salaryPolicySetting = salaryPolicySettings.Where(s => s.EmployeeTypeId == 1).SingleOrDefault();
                                SalarySettingsOfficer salarySettingsOfficer = await _salarySettingContract.GetSalarySettingsOfficerByJobCode(item.JobCode);
                                Bank bank = await _bankContract.GetBankById(salarySettingsOfficer.BankId);
                                Branch branch = await _branchContract.GetBranchesById(salarySettingsOfficer.BankBranchId);
                                if (bank == null)
                                {
                                    bonusControlSheet.BankName = "";
                                    bonusControlSheet.BankId = 0;
                                }
                                else
                                {
                                    bonusControlSheet.BankName = bank.BankName;
                                    bonusControlSheet.BankId = bank.Id;
                                }

                                if (branch == null)
                                {
                                    bonusControlSheet.BankBranchName = "";
                                }
                                else
                                {
                                    bonusControlSheet.BankBranchName = branch.BranchName;
                                }
                                bonusControlSheet.AccountNumber = salarySettingsOfficer.AccountNumber;
                                bonusControlSheet.EmployeeTypeId = 1;
                               // bonusControlSheet.RevenueStamp = (decimal)salaryPolicySetting.RevenueStamp;
                                bonusControlSheet.IncentiveBonus = item.Amount;


                            }
                            else
                            {
                                //SalaryPolicySetting salaryPolicySetting = salaryPolicySettings.Where(s => s.EmployeeTypeId == 2).SingleOrDefault();
                                SalarySettingsJuniorStaff salarySettingsJuniorStaff = await _salarySettingContract.GetSalarySettingsJuniorStaffByJobCode(item.JobCode);
                                Bank bank = await _bankContract.GetBankById(salarySettingsJuniorStaff.BankId);
                                Branch branch = await _branchContract.GetBranchesById(salarySettingsJuniorStaff.BankBranchId);
                                if (bank == null)
                                {
                                    bonusControlSheet.BankName = "";
                                    bonusControlSheet.BankId = 0;
                                }
                                else
                                {
                                    bonusControlSheet.BankName = bank.BankName;
                                    bonusControlSheet.BankId = bank.Id;
                                }

                                if (branch == null)
                                {
                                    bonusControlSheet.BankBranchName = "";
                                }
                                else
                                {
                                    bonusControlSheet.BankBranchName = branch.BranchName;
                                }
                                bonusControlSheet.AccountNumber = salarySettingsJuniorStaff.AccountNumber;
                                bonusControlSheet.EmployeeTypeId = 2;
                                //bonusControlSheet.RevenueStamp = (decimal)salaryPolicySetting.RevenueStamp;
                                bonusControlSheet.IncentiveBonus = item.Amount;
                            }

                            await _bonusContract.CreateBonusSheet(bonusControlSheet);


                        }
                    }
                    else
                    {
                        List<Employee> filteredEmployee = new List<Employee>();
                        foreach (var item in employeeList)
                        {
                            Employee employee = await _employeeContract.GetEmployeeById(item.Id);
                            filteredEmployee.Add(employee);
                        }

                        foreach (var item in filteredEmployee)
                        {
                            Department department = await _departmentContract.GetDepartmentById(item.DepartmentId);
                            Designation designation = await _designationContract.GetDesignationById(item.DesignationId);

                            BonusControlSheet bonusControlSheet = new BonusControlSheet();

                            bonusControlSheet.FestivalBonus = 0;
                            bonusControlSheet.Scholarship = 0;
                            bonusControlSheet.HonorariumBonus = 0;
                            bonusControlSheet.BonusId = bonusProcessData.BonusId;
                            bonusControlSheet.JobCode = item.JobCode;
                            bonusControlSheet.EmployeeName = item.EmployeeName;
                            bonusControlSheet.DepartmentName = department.DepartmentName;
                            bonusControlSheet.DesignationName = designation.DesignationName;
                            bonusControlSheet.JournalCode = item.JournalCode;
                            bonusControlSheet.TIN = item.TinNo;
                            bonusControlSheet.OtherDeduction = 0;



                            if (bonusProcessData.EmployeeTypeId == 1)
                            {

                               // SalaryPolicySetting salaryPolicySetting = salaryPolicySettings.Where(s => s.EmployeeTypeId == 1).SingleOrDefault();
                                SalarySettingsOfficer salarySettingsOfficer = await _salarySettingContract.GetSalarySettingsOfficerByJobCode(item.JobCode);
                                Bank bank = await _bankContract.GetBankById(salarySettingsOfficer.BankId);
                                Branch branch = await _branchContract.GetBranchesById(salarySettingsOfficer.BankBranchId);
                                if (bank == null)
                                {
                                    bonusControlSheet.BankName = "";
                                    bonusControlSheet.BankId = 0;
                                }
                                else
                                {
                                    bonusControlSheet.BankName = bank.BankName;
                                    bonusControlSheet.BankId = bank.Id;
                                }

                                if (branch == null)
                                {
                                    bonusControlSheet.BankBranchName = "";
                                }
                                else
                                {
                                    bonusControlSheet.BankBranchName = branch.BranchName;
                                }


                                bonusControlSheet.AccountNumber = salarySettingsOfficer.AccountNumber;
                                bonusControlSheet.EmployeeTypeId = 1;
                                //bonusControlSheet.RevenueStamp = (decimal)salaryPolicySetting.RevenueStamp;
                                bonusControlSheet.IncentiveBonus = (decimal)(salarySettingsOfficer.BasicSalary * (bonusProcessData.Basic / 100));


                            }
                            else
                            {
                                //SalaryPolicySetting salaryPolicySetting = salaryPolicySettings.Where(s => s.EmployeeTypeId == 2).SingleOrDefault();
                                SalarySettingsJuniorStaff salarySettingsJuniorStaff = await _salarySettingContract.GetSalarySettingsJuniorStaffByJobCode(item.JobCode);
                                Bank bank = await _bankContract.GetBankById(salarySettingsJuniorStaff.BankId);
                                Branch branch = await _branchContract.GetBranchesById(salarySettingsJuniorStaff.BankBranchId);
                                if (bank == null)
                                {
                                    bonusControlSheet.BankName = "";
                                    bonusControlSheet.BankId = 0;
                                }
                                else
                                {
                                    bonusControlSheet.BankName = bank.BankName;
                                    bonusControlSheet.BankId = bank.Id;
                                }

                                if (branch == null)
                                {
                                    bonusControlSheet.BankBranchName = "";
                                }
                                else
                                {
                                    bonusControlSheet.BankBranchName = branch.BranchName;
                                }
                                bonusControlSheet.AccountNumber = salarySettingsJuniorStaff.AccountNumber;
                                bonusControlSheet.EmployeeTypeId = 2;
                                //bonusControlSheet.RevenueStamp = (decimal)salaryPolicySetting.RevenueStamp;
                                bonusControlSheet.IncentiveBonus = (decimal)(salarySettingsJuniorStaff.BasicSalary * (bonusProcessData.Basic / 100));
                            }

                            await _bonusContract.CreateBonusSheet(bonusControlSheet);
                        }
                    }

                    
                }
                if (bonusProcessData.BonusName == "honorarium")
                {
                    if (bonusProcessData.DataType == "f")
                    {
                        List<BonusAmount> bonusAmounts = new List<BonusAmount>();

                        string[] _data = bonusProcessData.DataString.Split('#');

                            for (int i = 0; i < _data.Length - 1; i++)
                            {
                                string[] _amountData = _data[i].Split('_');
                                BonusAmount bonusAmount = new BonusAmount();
                                bonusAmount.JobCode = _amountData[0];
                                bonusAmount.Amount = Convert.ToDecimal(_amountData[1]);
                                bonusAmounts.Add(bonusAmount);
                            }
                        foreach (var item in bonusAmounts)
                        {
                            Employee employee = await _employeeContract.GetEmployeeByjobCode(item.JobCode.Trim());

                            Department department = await _departmentContract.GetDepartmentById(employee.DepartmentId);
                            Designation designation = await _designationContract.GetDesignationById(employee.DesignationId);

                            BonusControlSheet bonusControlSheet = new BonusControlSheet();

                            bonusControlSheet.IncentiveBonus = 0;
                            bonusControlSheet.Scholarship = 0;
                            bonusControlSheet.FestivalBonus = 0;
                            bonusControlSheet.BonusId = bonusProcessData.BonusId;
                            bonusControlSheet.JobCode = item.JobCode;
                            bonusControlSheet.EmployeeName = employee.EmployeeName;
                            bonusControlSheet.DepartmentName = department.DepartmentName;
                            bonusControlSheet.DesignationName = designation.DesignationName;
                            bonusControlSheet.JournalCode = employee.JournalCode;
                            bonusControlSheet.TIN = employee.TinNo;
                            bonusControlSheet.OtherDeduction = 0;



                            if (bonusProcessData.EmployeeTypeId == 1)
                            {

                                //SalaryPolicySetting salaryPolicySetting = salaryPolicySettings.Where(s => s.EmployeeTypeId == 1).SingleOrDefault();
                                SalarySettingsOfficer salarySettingsOfficer = await _salarySettingContract.GetSalarySettingsOfficerByJobCode(item.JobCode);
                                Bank bank = await _bankContract.GetBankById(salarySettingsOfficer.BankId);
                                Branch branch = await _branchContract.GetBranchesById(salarySettingsOfficer.BankBranchId);
                                if (bank == null)
                                {
                                    bonusControlSheet.BankName = "";
                                    bonusControlSheet.BankId = 0;
                                }
                                else
                                {
                                    bonusControlSheet.BankName = bank.BankName;
                                    bonusControlSheet.BankId = bank.Id;
                                }

                                if (branch == null)
                                {
                                    bonusControlSheet.BankBranchName = "";
                                }
                                else
                                {
                                    bonusControlSheet.BankBranchName = branch.BranchName;
                                }
                                bonusControlSheet.AccountNumber = salarySettingsOfficer.AccountNumber;
                                bonusControlSheet.EmployeeTypeId = 1;
                               // bonusControlSheet.RevenueStamp = (decimal)salaryPolicySetting.RevenueStamp;
                                bonusControlSheet.HonorariumBonus = item.Amount;


                            }
                            else
                            {
                                //SalaryPolicySetting salaryPolicySetting = salaryPolicySettings.Where(s => s.EmployeeTypeId == 2).SingleOrDefault();
                                SalarySettingsJuniorStaff salarySettingsJuniorStaff = await _salarySettingContract.GetSalarySettingsJuniorStaffByJobCode(item.JobCode);
                                Bank bank = await _bankContract.GetBankById(salarySettingsJuniorStaff.BankId);
                                Branch branch = await _branchContract.GetBranchesById(salarySettingsJuniorStaff.BankBranchId);
                                if (bank == null)
                                {
                                    bonusControlSheet.BankName = "";
                                    bonusControlSheet.BankId = 0;
                                }
                                else
                                {
                                    bonusControlSheet.BankName = bank.BankName;
                                    bonusControlSheet.BankId = bank.Id;
                                }

                                if (branch == null)
                                {
                                    bonusControlSheet.BankBranchName = "";
                                }
                                else
                                {
                                    bonusControlSheet.BankBranchName = branch.BranchName;
                                }
                                bonusControlSheet.AccountNumber = salarySettingsJuniorStaff.AccountNumber;
                                bonusControlSheet.EmployeeTypeId = 2;
                                //bonusControlSheet.RevenueStamp = (decimal)salaryPolicySetting.RevenueStamp;
                                bonusControlSheet.HonorariumBonus = item.Amount;
                            }

                            await _bonusContract.CreateBonusSheet(bonusControlSheet);


                        }
                    }
                    else
                    {
                        List<Employee> filteredEmployee = new List<Employee>();
                        foreach (var item in employeeList)
                        {
                            Employee employee = await _employeeContract.GetEmployeeById(item.Id);
                            filteredEmployee.Add(employee);



                        }

                        foreach (var item in filteredEmployee)
                        {
                            Department department = await _departmentContract.GetDepartmentById(item.DepartmentId);
                            Designation designation = await _designationContract.GetDesignationById(item.DesignationId);

                            BonusControlSheet bonusControlSheet = new BonusControlSheet();

                            bonusControlSheet.IncentiveBonus = 0;
                            bonusControlSheet.Scholarship = 0;
                            bonusControlSheet.FestivalBonus = 0;
                            bonusControlSheet.BonusId = bonusProcessData.BonusId;
                            bonusControlSheet.JobCode = item.JobCode;
                            bonusControlSheet.EmployeeName = item.EmployeeName;
                            bonusControlSheet.DepartmentName = department.DepartmentName;
                            bonusControlSheet.DesignationName = designation.DesignationName;
                            bonusControlSheet.JournalCode = item.JournalCode;
                            bonusControlSheet.TIN = item.TinNo;
                            bonusControlSheet.OtherDeduction = 0;



                            if (bonusProcessData.EmployeeTypeId == 1)
                            {

                                //SalaryPolicySetting salaryPolicySetting = salaryPolicySettings.Where(s => s.EmployeeTypeId == 1).SingleOrDefault();
                                SalarySettingsOfficer salarySettingsOfficer = await _salarySettingContract.GetSalarySettingsOfficerByJobCode(item.JobCode);
                                Bank bank = await _bankContract.GetBankById(salarySettingsOfficer.BankId);
                                Branch branch = await _branchContract.GetBranchesById(salarySettingsOfficer.BankBranchId);
                                if (bank == null)
                                {
                                    bonusControlSheet.BankName = "";
                                    bonusControlSheet.BankId = 0;
                                }
                                else
                                {
                                    bonusControlSheet.BankName = bank.BankName;
                                    bonusControlSheet.BankId = bank.Id;
                                }

                                if (branch == null)
                                {
                                    bonusControlSheet.BankBranchName = "";
                                }
                                else
                                {
                                    bonusControlSheet.BankBranchName = branch.BranchName;
                                }
                                bonusControlSheet.AccountNumber = salarySettingsOfficer.AccountNumber;
                                bonusControlSheet.EmployeeTypeId = 1;
                                //bonusControlSheet.RevenueStamp = (decimal)salaryPolicySetting.RevenueStamp;
                                bonusControlSheet.HonorariumBonus = (decimal)(salarySettingsOfficer.BasicSalary * (bonusProcessData.Basic / 100));


                            }
                            else
                            {
                               // SalaryPolicySetting salaryPolicySetting = salaryPolicySettings.Where(s => s.EmployeeTypeId == 2).SingleOrDefault();
                                SalarySettingsJuniorStaff salarySettingsJuniorStaff = await _salarySettingContract.GetSalarySettingsJuniorStaffByJobCode(item.JobCode);
                                Bank bank = await _bankContract.GetBankById(salarySettingsJuniorStaff.BankId);
                                Branch branch = await _branchContract.GetBranchesById(salarySettingsJuniorStaff.BankBranchId);
                                if (bank == null)
                                {
                                    bonusControlSheet.BankName = "";
                                    bonusControlSheet.BankId = 0;
                                }
                                else
                                {
                                    bonusControlSheet.BankName = bank.BankName;
                                    bonusControlSheet.BankId = bank.Id;
                                }

                                if (branch == null)
                                {
                                    bonusControlSheet.BankBranchName = "";
                                }
                                else
                                {
                                    bonusControlSheet.BankBranchName = branch.BranchName;
                                }
                                bonusControlSheet.AccountNumber = salarySettingsJuniorStaff.AccountNumber;
                                bonusControlSheet.EmployeeTypeId = 2;
                                //bonusControlSheet.RevenueStamp = (decimal)salaryPolicySetting.RevenueStamp;
                                bonusControlSheet.HonorariumBonus = (decimal)(salarySettingsJuniorStaff.BasicSalary * (bonusProcessData.Basic / 100));
                            }

                            await _bonusContract.CreateBonusSheet(bonusControlSheet);
                        }
                    }
                    
                }
                if (bonusProcessData.BonusName == "scholarship")
                {
                    if (bonusProcessData.DataType == "f")
                    {
                        List<BonusAmount> bonusAmounts = new List<BonusAmount>();

                        string[] _data = bonusProcessData.DataString.Split('#');

                        for (int i = 0; i < _data.Length - 1; i++)
                        {
                            string[] _amountData = _data[i].Split('_');
                            BonusAmount bonusAmount = new BonusAmount();
                            bonusAmount.JobCode = _amountData[0];
                            bonusAmount.Amount = Convert.ToDecimal(_amountData[1]);
                            bonusAmounts.Add(bonusAmount);
                        }
                        foreach (var item in bonusAmounts)
                        {
                            Employee employee = await _employeeContract.GetEmployeeByjobCode(item.JobCode.Trim());

                            Department department = await _departmentContract.GetDepartmentById(employee.DepartmentId);
                            Designation designation = await _designationContract.GetDesignationById(employee.DesignationId);

                            BonusControlSheet bonusControlSheet = new BonusControlSheet();

                            bonusControlSheet.IncentiveBonus = 0;
                            bonusControlSheet.HonorariumBonus = 0;
                            bonusControlSheet.FestivalBonus = 0;
                            bonusControlSheet.BonusId = bonusProcessData.BonusId;
                            bonusControlSheet.JobCode = item.JobCode;
                            bonusControlSheet.EmployeeName = employee.EmployeeName;
                            bonusControlSheet.DepartmentName = department.DepartmentName;
                            bonusControlSheet.DesignationName = designation.DesignationName;
                            bonusControlSheet.JournalCode = employee.JournalCode;
                            bonusControlSheet.TIN = employee.TinNo;
                            bonusControlSheet.OtherDeduction = 0;



                            if (bonusProcessData.EmployeeTypeId == 1)
                            {

                                //SalaryPolicySetting salaryPolicySetting = salaryPolicySettings.Where(s => s.EmployeeTypeId == 1).SingleOrDefault();
                                SalarySettingsOfficer salarySettingsOfficer = await _salarySettingContract.GetSalarySettingsOfficerByJobCode(item.JobCode);
                                Bank bank = await _bankContract.GetBankById(salarySettingsOfficer.BankId);
                                Branch branch = await _branchContract.GetBranchesById(salarySettingsOfficer.BankBranchId);
                                if (bank == null)
                                {
                                    bonusControlSheet.BankName = "";
                                    bonusControlSheet.BankId = 0;
                                }
                                else
                                {
                                    bonusControlSheet.BankName = bank.BankName;
                                    bonusControlSheet.BankId = bank.Id;
                                }

                                if (branch == null)
                                {
                                    bonusControlSheet.BankBranchName = "";
                                }
                                else
                                {
                                    bonusControlSheet.BankBranchName = branch.BranchName;
                                }
                                bonusControlSheet.AccountNumber = salarySettingsOfficer.AccountNumber;
                                bonusControlSheet.EmployeeTypeId = 1;
                               // bonusControlSheet.RevenueStamp = (decimal)salaryPolicySetting.RevenueStamp;
                                bonusControlSheet.Scholarship = item.Amount;


                            }
                            else
                            {
                               // SalaryPolicySetting salaryPolicySetting = salaryPolicySettings.Where(s => s.EmployeeTypeId == 2).SingleOrDefault();
                                SalarySettingsJuniorStaff salarySettingsJuniorStaff = await _salarySettingContract.GetSalarySettingsJuniorStaffByJobCode(item.JobCode);
                                Bank bank = await _bankContract.GetBankById(salarySettingsJuniorStaff.BankId);
                                Branch branch = await _branchContract.GetBranchesById(salarySettingsJuniorStaff.BankBranchId);
                                if (bank == null)
                                {
                                    bonusControlSheet.BankName = "";
                                    bonusControlSheet.BankId = 0;
                                }
                                else
                                {
                                    bonusControlSheet.BankName = bank.BankName;
                                    bonusControlSheet.BankId = bank.Id;
                                }

                                if (branch == null)
                                {
                                    bonusControlSheet.BankBranchName = "";
                                }
                                else
                                {
                                    bonusControlSheet.BankBranchName = branch.BranchName;
                                }
                                bonusControlSheet.AccountNumber = salarySettingsJuniorStaff.AccountNumber;
                                bonusControlSheet.EmployeeTypeId = 2;
                               // bonusControlSheet.RevenueStamp = (decimal)salaryPolicySetting.RevenueStamp;
                                bonusControlSheet.Scholarship = item.Amount;
                            }

                            await _bonusContract.CreateBonusSheet(bonusControlSheet);
                        }
                    }
                    else
                    {
                        List<Employee> filteredEmployee = new List<Employee>();
                        foreach (var item in employeeList)
                        {
                            Employee employee = await _employeeContract.GetEmployeeById(item.Id);
                            filteredEmployee.Add(employee);
                        }

                        foreach (var item in filteredEmployee)
                        {
                            Department department = await _departmentContract.GetDepartmentById(item.DepartmentId);
                            Designation designation = await _designationContract.GetDesignationById(item.DesignationId);

                            BonusControlSheet bonusControlSheet = new BonusControlSheet();

                            bonusControlSheet.IncentiveBonus = 0;
                            bonusControlSheet.FestivalBonus = 0;
                            bonusControlSheet.HonorariumBonus = 0;
                            bonusControlSheet.BonusId = bonusProcessData.BonusId;
                            bonusControlSheet.JobCode = item.JobCode;
                            bonusControlSheet.EmployeeName = item.EmployeeName;
                            bonusControlSheet.DepartmentName = department.DepartmentName;
                            bonusControlSheet.DesignationName = designation.DesignationName;
                            bonusControlSheet.JournalCode = item.JournalCode;
                            bonusControlSheet.TIN = item.TinNo;
                            bonusControlSheet.OtherDeduction = 0;



                            if (bonusProcessData.EmployeeTypeId == 1)
                            {

                                //SalaryPolicySetting salaryPolicySetting = salaryPolicySettings.Where(s => s.EmployeeTypeId == 1).SingleOrDefault();
                                SalarySettingsOfficer salarySettingsOfficer = await _salarySettingContract.GetSalarySettingsOfficerByJobCode(item.JobCode);
                                Bank bank = await _bankContract.GetBankById(salarySettingsOfficer.BankId);
                                Branch branch = await _branchContract.GetBranchesById(salarySettingsOfficer.BankBranchId);
                                if (bank == null)
                                {
                                    bonusControlSheet.BankName = "";
                                    bonusControlSheet.BankId = 0;
                                }
                                else
                                {
                                    bonusControlSheet.BankName = bank.BankName;
                                    bonusControlSheet.BankId = bank.Id;
                                }

                                if (branch == null)
                                {
                                    bonusControlSheet.BankBranchName = "";
                                }
                                else
                                {
                                    bonusControlSheet.BankBranchName = branch.BranchName;
                                }
                                bonusControlSheet.AccountNumber = salarySettingsOfficer.AccountNumber;
                                bonusControlSheet.EmployeeTypeId = 1;
                               // bonusControlSheet.RevenueStamp = (decimal)salaryPolicySetting.RevenueStamp;
                                bonusControlSheet.Scholarship = (decimal)(salarySettingsOfficer.BasicSalary * (bonusProcessData.Basic / 100));


                            }
                            else
                            {
                                //SalaryPolicySetting salaryPolicySetting = salaryPolicySettings.Where(s => s.EmployeeTypeId == 2).SingleOrDefault();
                                SalarySettingsJuniorStaff salarySettingsJuniorStaff = await _salarySettingContract.GetSalarySettingsJuniorStaffByJobCode(item.JobCode);
                                Bank bank = await _bankContract.GetBankById(salarySettingsJuniorStaff.BankId);
                                Branch branch = await _branchContract.GetBranchesById(salarySettingsJuniorStaff.BankBranchId);
                                if (bank == null)
                                {
                                    bonusControlSheet.BankName = "";
                                    bonusControlSheet.BankId = 0;
                                }
                                else
                                {
                                    bonusControlSheet.BankName = bank.BankName;
                                    bonusControlSheet.BankId = bank.Id;
                                }

                                if (branch == null)
                                {
                                    bonusControlSheet.BankBranchName = "";
                                }
                                else
                                {
                                    bonusControlSheet.BankBranchName = branch.BranchName;
                                }
                                bonusControlSheet.AccountNumber = salarySettingsJuniorStaff.AccountNumber;
                                bonusControlSheet.EmployeeTypeId = 2;
                                //bonusControlSheet.RevenueStamp = (decimal)salaryPolicySetting.RevenueStamp;
                                bonusControlSheet.Scholarship = (decimal)(salarySettingsJuniorStaff.BasicSalary * (bonusProcessData.Basic / 100));
                            }

                            await _bonusContract.CreateBonusSheet(bonusControlSheet);
                        }
                    }
                   
                }


                _responseViewModel.ResponseMessage = "Operation completed";
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

        [HttpPost]

        public async Task<IActionResult> ProcessBonusData([FromBody] BonusControlSheet bonusProcessData)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();

            // Basic presence checks
            if (bonusProcessData.BonusId == null)
                _responseViewModel.Errors["BonusId"] = "BonusId is required.";
            if (bonusProcessData.EmployeeTypeId == null)
                _responseViewModel.Errors["EmployeeTypeId"] = "EmployeeTypeId is required.";

            if (_responseViewModel.Errors.Any())
            {
                _responseViewModel.ResponseMessage = "Validation failed";
                _responseViewModel.StatusCode = 400;
                return BadRequest(_responseViewModel);
            }

            try
            {
                // Now safe to cast
                await _bonusContract.DeleteBonusSheet(
                    bonusProcessData.BonusId.Value,
                    bonusProcessData.EmployeeTypeId.Value
                );

                int result = await _bonusContract.CreateBonusSheetData(bonusProcessData);

                _responseViewModel.ResponseMessage = "Record saved successfully";
                _responseViewModel.StatusCode = 201;
            }
            catch (Exception ex)
            {
                _responseViewModel.ResponseMessage = "An error occurred while processing bonus data.";
                _responseViewModel.Errors.Add("Exception", ex.Message);
                _responseViewModel.StatusCode = 500;
            }

            return Ok(_responseViewModel);
        }





        [HttpPost]
        public async Task<IActionResult> XProcessBonus([FromForm] BonusProcessData bonusProcessData)
        {
            try
            {
                var settingResult = await _bonusContract.BonusSetting(bonusProcessData.BonusId, bonusProcessData.EmployeeTypeId);
                if (settingResult > 0)
                {
                    if (bonusProcessData.BonusName == "festival")
                    {
                        if (bonusProcessData.Muslim)
                        {
                            var result = await _bonusContract.BonusProcess(bonusProcessData.BonusId, bonusProcessData.EmployeeTypeId, 0, 1, bonusProcessData.Basic);
                        }
                        if (bonusProcessData.Hindu)
                        {
                            var result = await _bonusContract.BonusProcess(bonusProcessData.BonusId, bonusProcessData.EmployeeTypeId, 0, 2, bonusProcessData.Basic);
                        }
                        if (bonusProcessData.Buddist)
                        {
                            var result = await _bonusContract.BonusProcess(bonusProcessData.BonusId, bonusProcessData.EmployeeTypeId, 0, 3, bonusProcessData.Basic);
                        }
                        if (bonusProcessData.Christian)
                        {
                            var result = await _bonusContract.BonusProcess(bonusProcessData.BonusId, bonusProcessData.EmployeeTypeId, 0, 4, bonusProcessData.Basic);
                        }
                    }
                    if (bonusProcessData.BonusName == "incentive")
                    {
                        if (bonusProcessData.DataType == "f")
                        {
                            List<BonusAmount> bonusAmounts = new List<BonusAmount>();

                            string[] _data = bonusProcessData.DataString.Split('#');

                            if (_data.Length > 0)
                            {
                                for (int i=0; i<_data.Length-1;i++)
                                {
                                    string[] _amountData = _data[i].Split('_');
                                    BonusAmount bonusAmount = new BonusAmount();
                                    bonusAmount.JobCode = _amountData[0];
                                    bonusAmount.Amount = Convert.ToDecimal(_amountData[1]);
                                    bonusAmounts.Add(bonusAmount);
                                }

                                List<BonusAdjustmentViewModel> bonusAdjustmentViewModels = await _bonusContract.GetBonusProcessData(bonusProcessData.BonusId, bonusProcessData.EmployeeTypeId);
                                foreach (var item in bonusAdjustmentViewModels)
                                {
                                    var bonusAmount = bonusAmounts.Where(ba => ba.JobCode == item.JobCode).SingleOrDefault();
                                    if (bonusAmount == null)
                                    {
                                        await _bonusContract.DeleteBonus(item.BonusSheetId);
                                    }
                                    else
                                    {
                                        item.IncentiveBonus = (double)bonusAmount.Amount;
                                        await _bonusContract.UpdateBonusSheet(item);
                                    }
                                }

                            }

                        }
                        else
                        {
                            var result = await _bonusContract.BonusProcess(bonusProcessData.BonusId, bonusProcessData.EmployeeTypeId, 1, 0, bonusProcessData.Basic);
                        }

                    }
                    if (bonusProcessData.BonusName == "honorarium")
                    {
                        if (bonusProcessData.DataType == "f")
                        {
                            List<BonusAmount> bonusAmounts = new List<BonusAmount>();

                            string[] _data = bonusProcessData.DataString.Split('#');

                            if (_data.Length > 0)
                            {
                                foreach (var item in _data)
                                {
                                    string[] _amountData = item.Split('_');

                                    // Check if both JobCode and Amount exist
                                    if (_amountData.Length == 2 && !string.IsNullOrEmpty(_amountData[1]))
                                    {
                                        BonusAmount bonusAmount = new BonusAmount
                                        {
                                            JobCode = _amountData[0],
                                            Amount = Convert.ToDecimal(_amountData[1]) // Safely convert the amount
                                        };
                                        bonusAmounts.Add(bonusAmount);
                                    }
                                    else
                                    {
                                       
                                        Console.WriteLine($"Invalid data format or missing amount for item: {item}");
                                        
                                        continue;
                                    }
                                }

                                List<BonusAdjustmentViewModel> bonusAdjustmentViewModels = await _bonusContract.GetBonusProcessData(bonusProcessData.BonusId, bonusProcessData.EmployeeTypeId);

                                foreach (var item in bonusAdjustmentViewModels)
                                {
                                    var bonusAmount = bonusAmounts.Where(ba => ba.JobCode == item.JobCode).SingleOrDefault();
                                    if (bonusAmount == null)
                                    {
                                        await _bonusContract.DeleteBonus(item.BonusSheetId);
                                    }
                                    else
                                    {
                                        item.HonorariumBonus = (double)bonusAmount.Amount;
                                        await _bonusContract.UpdateBonusSheet(item);
                                    }
                                }
                            }
                        }
                        else
                        {
                            var result = await _bonusContract.BonusProcess(bonusProcessData.BonusId, bonusProcessData.EmployeeTypeId, 2, 0, bonusProcessData.Basic);
                        }
                    }

                    if (bonusProcessData.BonusName == "scholarship")
                    {
                        if (bonusProcessData.DataType == "f")
                        {
                            List<BonusAmount> bonusAmounts = new List<BonusAmount>();

                            string[] _data = bonusProcessData.DataString.Split('#');

                            if (_data.Length > 0)
                            {
                                foreach (var item in _data)
                                {
                                    string[] _amountData = item.Split('_');
                                    // Check if both JobCode and Amount exist
                                    if (_amountData.Length == 2 && !string.IsNullOrEmpty(_amountData[1]))
                                    {
                                        BonusAmount bonusAmount = new BonusAmount
                                        {
                                            JobCode = _amountData[0],
                                            Amount = Convert.ToDecimal(_amountData[1]) 
                                        };
                                        bonusAmounts.Add(bonusAmount);
                                    }
                                    else
                                    {

                                        Console.WriteLine($"Invalid data format or missing amount for item: {item}");

                                        continue;
                                    }
                                    //BonusAmount bonusAmount = new BonusAmount();
                                    //bonusAmount.JobCode = _amountData[0];
                                    //bonusAmount.Amount = Convert.ToDecimal(_amountData[1]);
                                    //bonusAmounts.Add(bonusAmount);
                                }

                                List<BonusAdjustmentViewModel> bonusAdjustmentViewModels = await _bonusContract.GetBonusProcessData(bonusProcessData.BonusId, bonusProcessData.EmployeeTypeId);
                                foreach (var item in bonusAdjustmentViewModels)
                                {
                                    var bonusAmount = bonusAmounts.Where(ba => ba.JobCode == item.JobCode).SingleOrDefault();
                                    if (bonusAmount == null)
                                    {
                                        await _bonusContract.DeleteBonus(item.BonusSheetId);
                                    }
                                    else
                                    {
                                        item.ScholarshipBonus = (double)bonusAmount.Amount;
                                        await _bonusContract.UpdateBonusSheet(item);
                                    }
                                }

                            }

                        }
                        else
                        {
                            var result = await _bonusContract.BonusProcess(bonusProcessData.BonusId, bonusProcessData.EmployeeTypeId, 3, 0, bonusProcessData.Basic);
                        }

                    }
                }


                _responseViewModel.ResponseMessage = "Operation completed";
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

        [HttpGet]
        public async Task<IActionResult> GetBonusFinalAdjustment(int bonusId,int employeeTypeId)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                var bonusList = await _bonusContract.GetBonusProcessData(bonusId,employeeTypeId);
                if (bonusList.Count>0)
                {
                    _responseViewModel.StatusCode = 200;
                    _responseViewModel.Data = bonusList;
                    return Ok(_responseViewModel);
                }
                else
                {
                    _responseViewModel.StatusCode = 404;
                    _responseViewModel.ResponseMessage = "No data found";
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
        public async Task<IActionResult> GetAllBonusProcessData()
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                var bonusList = await _bonusContract.GetAllBonusProcessData();
                if (bonusList.Count > 0)
                {
                    _responseViewModel.StatusCode = 200;
                    _responseViewModel.Data = bonusList;
                    return Ok(_responseViewModel);
                }
                else
                {
                    _responseViewModel.StatusCode = 404;
                    _responseViewModel.ResponseMessage = "No data found";
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
        public async Task<IActionResult> GetBonusProcessDataWithFilter(
                 int employeeTypeId,
                 int school_filter,
                 int role_filter,
                 string? department_filter = null,
                 string? designation_filter = null)
        {
            try
            {
                var bonusdata = await _bonusContract.GetBonusProcessDataWithFilter(
                    employeeTypeId,
                    school_filter > 0 ? school_filter : (int?)null,
                    role_filter > 0 ? role_filter : (int?)null,
                    !string.IsNullOrWhiteSpace(department_filter) ? department_filter.Trim() : null,
                    !string.IsNullOrWhiteSpace(designation_filter) ? designation_filter.Trim() : null
                );

                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = bonusdata;
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
        public async Task<IActionResult> UpdateBonusFinalAdjustment([FromForm] List<BonusAdjustmentViewModel>? bonusAdjustments)
        {
            int result = 0;
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (bonusAdjustments.Count > 0)
                {
                    foreach (var item in bonusAdjustments)
                    {
                         result += await _bonusContract.UpdateBonusSheet(item);
                    }
                    if (result>0)
                    {
                        _responseViewModel.ResponseMessage = result+" Rows updated";
                        _responseViewModel.StatusCode = 201;
                        return Ok(_responseViewModel);
                    }
                    else
                    {
                        _responseViewModel.ResponseMessage = "Could not update data";
                        _responseViewModel.StatusCode = 500;
                        return Ok(_responseViewModel);
                    }
                    
                }
                else
                {
                    _responseViewModel.ResponseMessage = "No data found to update";
                    _responseViewModel.StatusCode = 404;
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
        public async Task<IActionResult> JournalPosting(int monthId, int employeeTypeId,int bonusId)
        {

            var bonusMaster = await _bonusContract.GetBonusJournalMaster(monthId, employeeTypeId,bonusId);
            var checkjournaldata = await _bonusContract.CheckBonusJournalData(employeeTypeId, bonusId);
            if (checkjournaldata.Count > 0)
            {
                _responseViewModel.StatusCode = 500;
                _responseViewModel.ResponseMessage = "Journal already created";
                return Ok(_responseViewModel);
            }
            else
            {
                if (bonusMaster == null)
                {
                    var bonusSheetData = await _bonusContract.GetBonusProcessData(bonusId, employeeTypeId);
                    if (bonusSheetData.Count > 0)
                    {
                        try
                        {
                            List<BonusJournal> journals = new List<BonusJournal>();
                            string bonusName = "";
                            List<string> journalCodes = new List<string>();


                            foreach (var item in bonusSheetData)
                            {
                                if (!journalCodes.Contains(item.JournalCode))
                                {
                                    journalCodes.Add(item.JournalCode);
                                }

                            }
                            // this is for Revenue Stamp.
                            var totalRevenue = (decimal)bonusSheetData.Sum(s => s.RevStamp);
                            journals.Add(new BonusJournal
                            {
                                AccountNumber = "5804000000",
                                JournalCode = "99",
                                Details = "Revenue Stamp",
                                Debit = null,
                                Credit = totalRevenue

                            });
                            // for Payroll Suspense A/C
                            journals.Add(new BonusJournal
                            {
                                AccountNumber = "5800500000",
                                JournalCode = "99",
                                Details = "Payroll Suspense A/C",
                                Debit = null,
                                Credit = 0

                            });

                            if (bonusSheetData[0].FestivalBonus > 0)
                            {
                                bonusName = "Festival Bonus";
                            }
                            if (bonusSheetData[0].HonorariumBonus > 0)
                            {
                                bonusName = "Honorarium Bonus";
                            }
                            if (bonusSheetData[0].IncentiveBonus > 0)
                            {
                                bonusName = "Incentive Bonus";
                            }
                            if (bonusSheetData[0].ScholarshipBonus > 0)
                            {
                                bonusName = "Scholarship Bonus";
                            }

                            decimal totalAmountForSuspenseAccount = 0;
                            foreach (var item in journalCodes)
                            {
                                decimal bonusAmount = 0;

                                var journalCodeWiseSalaryData = bonusSheetData.Where(s => s.JournalCode == item).ToList();
                                foreach (var data in journalCodeWiseSalaryData)
                                {
                                    if (data.FestivalBonus > 0)
                                    {
                                        bonusAmount += (decimal)data.FestivalBonus + (decimal)data.RevStamp;
                                        totalAmountForSuspenseAccount += (decimal)data.FestivalBonus;
                                    }
                                    if (data.HonorariumBonus > 0)
                                    {
                                        bonusAmount += (decimal)data.HonorariumBonus + (decimal)data.RevStamp;
                                        totalAmountForSuspenseAccount += (decimal)data.HonorariumBonus;
                                    }
                                    if (data.IncentiveBonus > 0)
                                    {
                                        bonusAmount += (decimal)data.IncentiveBonus + (decimal)data.RevStamp;
                                        totalAmountForSuspenseAccount += (decimal)data.IncentiveBonus;
                                    }
                                    if (data.ScholarshipBonus > 0)
                                    {
                                        bonusAmount += (decimal)data.ScholarshipBonus + (decimal)data.RevStamp;
                                        totalAmountForSuspenseAccount += (decimal)data.ScholarshipBonus;
                                    }

                                }
                                // for Basic
                                journals.Add(new BonusJournal
                                {
                                    AccountNumber = "8108000000",
                                    JournalCode = item,
                                    Details = bonusName,
                                    Debit = bonusAmount,
                                    Credit = null

                                });
                            }

                            journals[1].Credit = totalAmountForSuspenseAccount;

                            BonusJournalMaster bonusJournalMaster = new BonusJournalMaster();
                            bonusJournalMaster.MonthId = monthId;
                            bonusJournalMaster.EmployeeTypeId = employeeTypeId;
                            bonusJournalMaster.CreatedBy = "";
                            bonusJournalMaster.CreatedDate = DateTime.Now;
                            bonusJournalMaster.BonusId = bonusId;
                            var journalMasterId = await _bonusContract.CreateBonusJournalMaster(bonusJournalMaster);
                            if (journalMasterId > 0)
                            {
                                foreach (var journal in journals)
                                {
                                    journal.JournalMasterId = journalMasterId;
                                    await _bonusContract.CreateBonusJournal(journal);
                                }
                            }
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
                    else
                    {
                        _responseViewModel.StatusCode = 404;
                        _responseViewModel.ResponseMessage = "No data found to post";
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

           

        }

        [HttpGet]
        public async Task<IActionResult> GetLastBonus()
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                var bonusList = await _bonusContract.GetLastBonus();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = bonusList;
                return Ok(_responseViewModel);


            }
            catch (Exception ex)
            {
                _responseViewModel.ResponseMessage = "Something went wrong";
                _responseViewModel.StatusCode = 500;
                return Ok(_responseViewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetLastBonusAmount()
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                var bonusList = await _bonusContract.GetLastBonusAmountAsync();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = bonusList;
                return Ok(_responseViewModel);


            }
            catch (Exception ex)
            {
                _responseViewModel.ResponseMessage = "Something went wrong";
                _responseViewModel.StatusCode = 500;
                return Ok(_responseViewModel);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetLatestBonusAmount()
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                var bonusList = await _bonusContract.GetLastBonusAmount();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = bonusList;
                return Ok(_responseViewModel);


            }
            catch (Exception ex)
            {
                _responseViewModel.ResponseMessage = "Something went wrong";
                _responseViewModel.StatusCode = 500;
                return Ok(_responseViewModel);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetPayableBonusList()
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                var bonusList = await _bonusContract.GetPayableBonusList();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = bonusList;
                return Ok(_responseViewModel);


            }
            catch (Exception ex)
            {
                _responseViewModel.ResponseMessage = "Something went wrong";
                _responseViewModel.StatusCode = 500;
                return Ok(_responseViewModel);
            }
        }

    }
}
