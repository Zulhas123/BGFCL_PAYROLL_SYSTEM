using BgfclApp.ViewModels;
using Contracts;
using Entities;
using Entities.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace BgfclApp.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private ILoanContract _loanContract;
        private IEmployeeContract _employeeContract;
        private ISalarySettingContract _salarySettingContract;
        private ResponseViewModel _responseViewModel;

        public LoansController(ILoanContract loanContract, ResponseViewModel responseViewModel, IEmployeeContract employeeContract, ISalarySettingContract salarySettingContract)
        {
            _loanContract = loanContract;
            _responseViewModel = responseViewModel;
            _employeeContract = employeeContract;
            _salarySettingContract = salarySettingContract;
        }

        [HttpGet]
        public async Task<IActionResult> GetLoans(string loanType)
        {
            try
            {
                var loans = await _loanContract.GetLoans(loanType);
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = loans;
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
        public async Task<IActionResult> GetEmployeesForMcLoan(int employeeTypeId)
        {
            List<EmployeeViewModel> filteredEmployees = new List<EmployeeViewModel>();
            var employees = await _employeeContract.GetEmployeesByEmployeeType(employeeTypeId);

            var loans = await _loanContract.GetMcLoans(isActive: 1);
            foreach (var employee in employees)
            {
                var _setting = loans.Where(l => l.JobCode == employee.JobCode).SingleOrDefault();
                if (_setting == null)
                {
                    filteredEmployees.Add(employee);
                }
            }

            _responseViewModel.StatusCode = 200;
            _responseViewModel.Data = filteredEmployees;
            return Ok(_responseViewModel);
        }
        public async Task<IActionResult> GetEmployeesForMcLoanReport(int employeeTypeId)
        {
            List<EmployeeViewModel> filteredEmployees = new List<EmployeeViewModel>();
            var employees = await _employeeContract.GetEmployeesByEmployeeType(employeeTypeId);

            var loans = await _loanContract.GetMcLoans(isActive: 1);
            foreach (var employee in employees)
            {
                var _setting = loans.Where(l => l.JobCode == employee.JobCode).SingleOrDefault();
                if (_setting != null)
                {
                    filteredEmployees.Add(employee);
                }
            }

            _responseViewModel.StatusCode = 200;
            _responseViewModel.Data = filteredEmployees;
            return Ok(_responseViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMcLoan([FromForm] McLoan mclLoan)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (String.IsNullOrEmpty(mclLoan.JobCode))
                {
                    _responseViewModel.Errors.Add(nameof(mclLoan.JobCode) + "Error", "JobCode required");
                }
                if (mclLoan.TotalLoanAmount==0)
                {
                    _responseViewModel.Errors.Add(nameof(mclLoan.TotalLoanAmount) + "Error", "Amount can not be zero or empty");
                }
                if (String.IsNullOrEmpty(mclLoan.LoanTakenDateString))
                {
                    _responseViewModel.Errors.Add(nameof(mclLoan.LoanTakenDateString) + "Error", "Invalid Loan Taken Date");
                }
                else
                {
                    DateTime convertedDate = DateTime.ParseExact(mclLoan.LoanTakenDateString,"dd/MM/yyyy",CultureInfo.InvariantCulture);
                    mclLoan.LoanTakenDate = convertedDate;


                }
                if (mclLoan.InstallmentNo == 0)
                {
                    _responseViewModel.Errors.Add(nameof(mclLoan.InstallmentNo) + "Error", "InstallmentNo can not be zero or empty");
                }
                if (mclLoan.InstallmentAmount == 0)
                {
                    _responseViewModel.Errors.Add(nameof(mclLoan.InstallmentAmount) + "Error", "Installment Amount can not be zero or empty");
                }

                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                var existingLoans = await _loanContract.GetMcLoans(isActive:1);
                var existingLoan = existingLoans.Where(l => l.JobCode == mclLoan.JobCode).SingleOrDefault();
                if (existingLoan == null)
                {

                    mclLoan.RemainingLoanAmount = mclLoan.TotalLoanAmount;
                    mclLoan.RemainingInstallmentNo = mclLoan.InstallmentNo;
                    mclLoan.LoanTypeId = 2; // Motorcycle Loan
                    mclLoan.IsActive = true;
                    mclLoan.CreatedBy = "";
                    mclLoan.CreatedDate = DateTime.Now;

                    int result = await _loanContract.CreateMcLoan(mclLoan);

                    _responseViewModel.ResponseMessage = "Record saved successfully";
                    _responseViewModel.StatusCode = 201;
                    return Ok(_responseViewModel);
                }
                else
                {
                    _responseViewModel.ResponseMessage = "Record already exists";
                    _responseViewModel.Errors.Add(nameof(mclLoan.JobCode) + "Error", "Loan already exists");
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
        public async Task<IActionResult> GetMcLoans()
        {
            try
            {
                var loans = await _loanContract.GetMcLoans(isActive:1);
                if (loans.Count>0)
                {
                    int count = 1;
                    foreach (var loan in loans)
                    {
                        loan.Sl = count;
                        loan.LoanTakenDate = Convert.ToDateTime(loan.LoanTakenDate).ToString("dd-MMM-yyyy");
                        count++;
                    }
                }
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = loans;
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
        public async Task<IActionResult> GetMcLoanById(int id)
        {
            var mcLoan = await _loanContract.GetMcLoanById(id);
            mcLoan.LoanTakenDateString = mcLoan.LoanTakenDate.Value.ToString("dd-MMM-yyyy");
            _responseViewModel.StatusCode = 200;
            _responseViewModel.Data = mcLoan;
            return Ok(_responseViewModel);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateMcLoan([FromForm] McLoan mclLoan)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (mclLoan.InstallmentAmount == 0)
                {
                    _responseViewModel.Errors.Add(nameof(mclLoan.InstallmentAmount) + "Error_M", "Installment Amount can not be zero or empty");
                }

                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                mclLoan.UpdatedBy = "";
                mclLoan.UpdatedDate = DateTime.Now.Date;
                var result = await _loanContract.UpdateMcLoan(mclLoan);

                if (result > 0)
                {
                    _responseViewModel.ResponseMessage = "Data updated successfully";
                    _responseViewModel.StatusCode = 201;
                    return Ok(_responseViewModel);
                }
                else
                {
                    _responseViewModel.ResponseMessage = "Could not update";
                    _responseViewModel.StatusCode = 500;
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
        public async Task<IActionResult> GetEmployeesForComLoan(int employeeTypeId)
        {
            List<EmployeeViewModel> filteredEmployees = new List<EmployeeViewModel>();
            var employees = await _employeeContract.GetEmployeesByEmployeeType(employeeTypeId);

            var loans = await _loanContract.GetComLoans(isActive: 1);
            foreach (var employee in employees)
            {
                var _setting = loans.Where(l => l.JobCode == employee.JobCode).SingleOrDefault();
                if (_setting == null)
                {
                    filteredEmployees.Add(employee);
                }
            }

            _responseViewModel.StatusCode = 200;
            _responseViewModel.Data = filteredEmployees;
            return Ok(_responseViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateComLoan([FromForm] ComLoan comLoan)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (String.IsNullOrEmpty(comLoan.JobCode))
                {
                    _responseViewModel.Errors.Add(nameof(comLoan.JobCode) + "Error", "JobCode required");
                }
                if (comLoan.TotalLoanAmount == 0)
                {
                    _responseViewModel.Errors.Add(nameof(comLoan.TotalLoanAmount) + "Error", "Amount can not be zero or empty");
                }
                if (String.IsNullOrEmpty(comLoan.LoanTakenDateString))
                {
                    _responseViewModel.Errors.Add(nameof(comLoan.LoanTakenDateString) + "Error", "Invalid Loan Taken Date");
                }
                else
                {
                    DateTime convertedDate = DateTime.ParseExact(comLoan.LoanTakenDateString, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    comLoan.LoanTakenDate = convertedDate;
                    comLoan.LastProcessingDate = convertedDate;

                }
                if (comLoan.InterestRate == 0)
                {
                    _responseViewModel.Errors.Add(nameof(comLoan.InterestRate) + "Error", "InterestRate can not be zero or empty");
                }
                if (comLoan.InstallmentNo == 0)
                {
                    _responseViewModel.Errors.Add(nameof(comLoan.InstallmentNo) + "Error", "InstallmentNo can not be zero or empty");
                }
                if (comLoan.InstallmentAmount == 0)
                {
                    _responseViewModel.Errors.Add(nameof(comLoan.InstallmentAmount) + "Error", "Installment Amount can not be zero or empty");
                }

                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                var existingLoans = await _loanContract.GetComLoans(isActive: 1);
                var existingLoan = existingLoans.Where(l => l.JobCode == comLoan.JobCode).SingleOrDefault();
                if (existingLoan == null)
                {

                    comLoan.RemainingLoanAmount = comLoan.TotalLoanAmount;
                    comLoan.RemainingInstallmentNo = comLoan.InstallmentNo;
                    comLoan.LoanTypeId = 4; // Computer Loan
                    comLoan.IsActive = true;
                    comLoan.CreatedBy = "";
                    comLoan.CreatedDate = DateTime.Now;

                    int result = await _loanContract.CreateComLoan(comLoan);

                    _responseViewModel.ResponseMessage = "Record saved successfully";
                    _responseViewModel.StatusCode = 201;
                    return Ok(_responseViewModel);
                }
                else
                {
                    _responseViewModel.ResponseMessage = "Record already exists";
                    _responseViewModel.Errors.Add(nameof(comLoan.JobCode) + "Error", "Loan already exists");
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
        public async Task<IActionResult> GetComLoans()
        {
            try
            {
                var loans = await _loanContract.GetComLoans(isActive: 1);
                if (loans.Count > 0)
                {
                    int count = 1;
                    foreach (var loan in loans)
                    {
                        loan.Sl = count;
                        loan.LoanTakenDate = Convert.ToDateTime(loan.LoanTakenDate).ToString("dd-MMM-yyyy");
                        count++;
                    }
                }
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = loans;
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
        public async Task<IActionResult> GetComLoanById(int id)
        {
            var comLoan = await _loanContract.GetComLoanById(id);
            comLoan.LoanTakenDateString = comLoan.LoanTakenDate.Value.ToString("dd-MMM-yyyy");
            _responseViewModel.StatusCode = 200;
            _responseViewModel.Data = comLoan;
            return Ok(_responseViewModel);
        }


        [HttpPut]
        public async Task<IActionResult> UpdateComLoan([FromForm] ComLoan comLoan)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (comLoan.InstallmentAmount == 0)
                {
                    _responseViewModel.Errors.Add(nameof(comLoan.InstallmentAmount) + "Error_M", "Installment Amount can not be zero or empty");
                }

                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                comLoan.UpdatedBy = "";
                comLoan.UpdatedDate = DateTime.Now.Date;
                var result = await _loanContract.UpdateComLoan(comLoan);

                if (result > 0)
                {
                    _responseViewModel.ResponseMessage = "Data updated successfully";
                    _responseViewModel.StatusCode = 201;
                    return Ok(_responseViewModel);
                }
                else
                {
                    _responseViewModel.ResponseMessage = "Could not update";
                    _responseViewModel.StatusCode = 500;
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

        [HttpPost]
        public async Task<IActionResult> UpdateYearEndingData([FromBody] List<YearEndingData> loanData)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            if (loanData == null || !loanData.Any())
            {
                _responseViewModel.ResponseMessage = "No data provided.";
                _responseViewModel.StatusCode = 400; 
                return BadRequest(_responseViewModel);
            }

            try
            {
                foreach (var data in loanData)
                {
                    var result = await _loanContract.UpdateYearEndingData(data);

                    if (result <= 0)
                    {
                        _responseViewModel.ResponseMessage = "Could not update some entries.";
                        _responseViewModel.StatusCode = 500; // Internal Server Error
                        continue;
                    }
                }

                _responseViewModel.ResponseMessage = "Data updated successfully.";
                _responseViewModel.StatusCode = 200; // OK
                return Ok(_responseViewModel);
            }
            catch (Exception ex)
            {
                _responseViewModel.ResponseMessage = ex.Message;
                _responseViewModel.StatusCode = 500; // Internal Server Error
                return Ok(_responseViewModel);
            }
        }




        [HttpGet]
        public async Task<IActionResult> GetEmployeesForCarLoan(int employeeTypeId)
        {
            List<EmployeeViewModel> filteredEmployees = new List<EmployeeViewModel>();
            var employees = await _employeeContract.GetEmployeesByEmployeeType(employeeTypeId);

            var loans = await _loanContract.GetCarLoans(isActive: 1);
            foreach (var employee in employees)
            {
                var _setting = loans.Where(l => l.JobCode == employee.JobCode).SingleOrDefault();
                if (_setting == null)
                {
                    filteredEmployees.Add(employee);
                }
            }

            _responseViewModel.StatusCode = 200;
            _responseViewModel.Data = filteredEmployees;
            return Ok(_responseViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetCarLoans()
        {
            try
            {
                var loans = await _loanContract.GetCarLoans(isActive: 1);
                if (loans.Count > 0)
                {
                    int count = 1;
                    foreach (var loan in loans)
                    {
                        loan.Sl = count;
                        loan.LoanTakenDate = Convert.ToDateTime(loan.LoanTakenDate).ToString("dd-MMM-yyyy");
                        count++;
                    }
                }
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = loans;
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
        public async Task<IActionResult> CreateCarLoan([FromForm] CarLoan carLoan)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (String.IsNullOrEmpty(carLoan.JobCode))
                {
                    _responseViewModel.Errors.Add(nameof(carLoan.JobCode) + "Error", "JobCode required");
                }
                if (carLoan.TotalLoanAmount == 0)
                {
                    _responseViewModel.Errors.Add(nameof(carLoan.TotalLoanAmount) + "Error", "Amount can not be zero or empty");
                }
                if (String.IsNullOrEmpty(carLoan.LoanTakenDateString))
                {
                    _responseViewModel.Errors.Add(nameof(carLoan.LoanTakenDateString) + "Error", "Invalid Loan Taken Date");
                }
                else
                {
                    DateTime convertedDate = DateTime.ParseExact(carLoan.LoanTakenDateString, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    carLoan.LoanTakenDate = convertedDate;
                    carLoan.LastProcessingDate = convertedDate;

                }
                if (carLoan.InterestRate == 0)
                {
                    _responseViewModel.Errors.Add(nameof(carLoan.InterestRate) + "Error", "InterestRate can not be zero or empty");
                }
                if (carLoan.InstallmentNo == 0)
                {
                    _responseViewModel.Errors.Add(nameof(carLoan.InstallmentNo) + "Error", "InstallmentNo can not be zero or empty");
                }
                if (carLoan.DepreciationAmount == 0)
                {
                    _responseViewModel.Errors.Add(nameof(carLoan.DepreciationAmount) + "Error", "Depreciation Amount can not be zero or empty");
                }
                if (carLoan.ActualAmount == 0)
                {
                    _responseViewModel.Errors.Add(nameof(carLoan.ActualAmount) + "Error", "Actual Amount can not be zero or empty");
                }
                if (String.IsNullOrEmpty(carLoan.EmiStartMonthString))
                {
                    _responseViewModel.Errors.Add(nameof(carLoan.EmiStartMonthString) + "Error", "Invalid EMI Start Date");
                }
                else
                {
                    DateTime convertedDate = DateTime.ParseExact(carLoan.EmiStartMonthString, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    carLoan.EmiStartMonth = convertedDate;

                }

                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                var existingLoans = await _loanContract.GetCarLoans(isActive: 1);
                var existingLoan = existingLoans.Where(l => l.JobCode == carLoan.JobCode).SingleOrDefault();
                if (existingLoan == null)
                {

                    carLoan.RemainingDepreciationAmount = carLoan.DepreciationAmount;
                    carLoan.RemainingActualAmount = carLoan.ActualAmount;
                    carLoan.RemainingInstallmentNo = carLoan.InstallmentNo;
                    carLoan.LoanTypeId = 10; // Car Loan
                    carLoan.IsActive = true;
                    carLoan.CreatedBy = "";
                    carLoan.CreatedDate = DateTime.Now;
                    
                    int result = await _loanContract.CreateCarLoan(carLoan);
                    
                    if (result > 0)
                    {
                        carLoan.Id = result;
                        var installments = LoansController.CalculateAmortizationSchedule(carLoan);
                        if (installments.Count > 0)
                        {
                            foreach (var item in installments)
                            {
                                _loanContract.CreateCarLoanInstallment(item);
                            }
                        }
                        var depriciationInstallments = LoansController.CalculateDepreciation(carLoan);
                        if (depriciationInstallments.Count > 0)
                        {
                            foreach (var item in depriciationInstallments)
                            {
                                _loanContract.CreateCarLoanDepreciationInstallment(item);
                            }
                        }
                    }

                    _responseViewModel.ResponseMessage = "Record saved successfully";
                    _responseViewModel.StatusCode = 201;
                    return Ok(_responseViewModel);
                }
                else
                {
                    _responseViewModel.ResponseMessage = "Record already exists";
                    _responseViewModel.Errors.Add(nameof(carLoan.JobCode) + "Error", "Loan already exists");
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
        public async Task<IActionResult> GetCarLoanInstallments(int loanId)
        {
            try
            {
                CarLoan carLoan = await _loanContract.GetCarLoanById(loanId);
                var installments = await _loanContract.GetCarLoanInstallments(loanId);
                
                EmployeeViewModel employee = await _employeeContract.GetEmployeeViewByJobCode(carLoan.JobCode);
                var loanInfo = new 
                {
                    carLoan=carLoan,
                    installments = installments,
                    employee= employee
                };

                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = loanInfo;
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
        public async Task<IActionResult> GetCarLoanDepreciationInstallments(int loanId)
        {
            try
            {
                //var loans = await _loanContract.GetCarLoanDepreciationInstallments(loanId);

                CarLoan carLoan = await _loanContract.GetCarLoanById(loanId);
                var depreciationInstallments = await _loanContract.GetCarLoanDepreciationInstallments(loanId);

                EmployeeViewModel employee = await _employeeContract.GetEmployeeViewByJobCode(carLoan.JobCode);
                var loanInfo = new
                {
                    carLoan = carLoan,
                    depreciationInstallments = depreciationInstallments,
                    employee = employee
                };

                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = loanInfo;
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
        public async Task<IActionResult> GetEmployeesForHbLoan(int employeeTypeId)
        {
            List<EmployeeViewModel> filteredEmployees = new List<EmployeeViewModel>();
            var employees = await _employeeContract.GetEmployeesByEmployeeType(employeeTypeId);

            var loans = await _loanContract.GetHbLoans(isActive: 1);
            foreach (var employee in employees)
            {
                var _setting = loans.Where(l => l.JobCode == employee.JobCode).SingleOrDefault();
                if (_setting == null)
                {
                    filteredEmployees.Add(employee);
                }
            }

            _responseViewModel.StatusCode = 200;
            _responseViewModel.Data = filteredEmployees;
            return Ok(_responseViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetHbLoanById(int id)
        {
            var loan = await _loanContract.GetHbLoanById(id);
            loan.LoanTakenDateString = loan.LoanTakenDate.Value.ToString("dd-MMM-yyyy");
            _responseViewModel.StatusCode = 200;
            _responseViewModel.Data = loan;
            return Ok(_responseViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetHbLoansByEmployeeId(int id)
        {
            try
            {
                var loans = await _loanContract.GetHbLoansByEmployeeId(id);
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = loans;
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
        public async Task<IActionResult> GetHbLoansByType(int loanTypeId)
        {
            var loans = await _loanContract.GetHbLoansByLoanType(isActive: 1, loanTypeId:loanTypeId);
            foreach( var item in loans)
            {
                if (decimal.TryParse(item.TotalLoanAmount, out decimal loanAmount))
                {
                    
                    item.TotalLoanAmount = loanAmount.ToString("N2");
                }
                else
                {
                    item.TotalLoanAmount = "0.00";
                }
            }
            if (loans.Count>0)
            {
                int count = 1;

                foreach (var item in loans)
                {
                    item.Sl = count;
                    item.LoanTakenDate = Convert.ToDateTime(item.LoanTakenDate).ToString("dd-MMM-yyyy");
                    count++;
                }

            }
            _responseViewModel.StatusCode = 200;
            _responseViewModel.Data = loans;
            return Ok(_responseViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateHbLoan([FromForm] HbLoan hbLoan)
        {

            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (String.IsNullOrEmpty(hbLoan.JobCode))
                {
                    _responseViewModel.Errors.Add(nameof(hbLoan.JobCode) + "Error", "JobCode required");
                }
                if (hbLoan.TotalLoanAmount == 0)
                {
                    _responseViewModel.Errors.Add(nameof(hbLoan.TotalLoanAmount) + "Error", "Amount can not be zero or empty");
                }
                if (String.IsNullOrEmpty(hbLoan.LoanTakenDateString))
                {
                    _responseViewModel.Errors.Add(nameof(hbLoan.LoanTakenDateString) + "Error", "Invalid Loan Taken Date");
                }
                else
                {
                    DateTime convertedDate = DateTime.ParseExact(hbLoan.LoanTakenDateString, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    hbLoan.LoanTakenDate = convertedDate;
                    hbLoan.LastProcessingDate = convertedDate;

                }
                if (hbLoan.InterestRate == 0)
                {
                    _responseViewModel.Errors.Add(nameof(hbLoan.InterestRate) + "Error", "InterestRate can not be zero or empty");
                }
                if (hbLoan.InstallmentNo == 0)
                {
                    _responseViewModel.Errors.Add(nameof(hbLoan.InstallmentNo) + "Error", "InstallmentNo can not be zero or empty");
                }
                if (hbLoan.PrincipalInstallmentAmount == 0)
                {
                    _responseViewModel.Errors.Add(nameof(hbLoan.PrincipalInstallmentAmount) + "Error", "Principal Installment Amount can not be zero or empty");
                }

                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                var existingLoans = await _loanContract.GetHbLoans(isActive: 1);
                var existingLoan = existingLoans.Where(l => l.JobCode == hbLoan.JobCode).SingleOrDefault();
                if (existingLoan == null)
                {

                    hbLoan.RemainingLoanAmount = hbLoan.TotalLoanAmount;
                    hbLoan.RemainingInstallmentNo = hbLoan.InstallmentNo;
                    hbLoan.TotalInterest = 0;
                    hbLoan.RemainingInterest = 0;
                    hbLoan.RemainingInterestInstallmentNo = hbLoan.InterestInstallmentNo;
                    
                    hbLoan.IsActive = true;
                    hbLoan.LastProcessingDate = hbLoan.LoanTakenDate;
                    if (hbLoan.LoanTypeId==1)
                    {
                        hbLoan.DateAfterPeriod = hbLoan.LoanTakenDate.Value.AddMonths(11);
                    }
                    if (hbLoan.LoanTypeId == 9)
                    {
                        hbLoan.DateAfterPeriod = hbLoan.LoanTakenDate.Value.AddMonths(1);
                    }

                    hbLoan.CreatedBy = "";
                    hbLoan.CreatedDate = DateTime.Now;

                    int result = await _loanContract.CreateHbLoan(hbLoan);
                    if (result > 0)
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
                else
                {
                    _responseViewModel.ResponseMessage = "Record already exists";
                    _responseViewModel.Errors.Add(nameof(hbLoan.JobCode) + "Error", "Loan already exists");
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
        public async Task<IActionResult> UpdateHbLoan([FromForm] HbLoan hbLoan)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                //if (hbLoan.PrincipalInstallmentAmount == 0)
                //{
                //    _responseViewModel.Errors.Add(nameof(hbLoan.PrincipalInstallmentAmount) + "Error_M", "Principal Installment Amount can not be zero or empty");
                //}
                //if (hbLoan.InterestInstallmentAmount == 0)
                //{
                //    _responseViewModel.Errors.Add(nameof(hbLoan.InterestInstallmentAmount) + "Error_M", "Interest Installment Amount can not be zero or empty");
                //}
                //if (hbLoan.InterestInstallmentNo == 0)
                //{
                //    _responseViewModel.Errors.Add(nameof(hbLoan.InterestInstallmentNo) + "Error_M", "Interest Installment No can not be zero or empty");
                //}
                //if (hbLoan.RemainingInterestInstallmentNo == 0)
                //{
                //    _responseViewModel.Errors.Add(nameof(hbLoan.RemainingInterestInstallmentNo) + "Error_M", "Remaining Interest Installment No can not be zero or empty");
                //}

                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                hbLoan.UpdatedBy = "";
                hbLoan.UpdatedDate = DateTime.Now.Date;
                var result = await _loanContract.UpdateHbLoan(hbLoan);

                if (result > 0)
                {
                    _responseViewModel.ResponseMessage = "Data updated successfully";
                    _responseViewModel.StatusCode = 201;
                    return Ok(_responseViewModel);
                }
                else
                {
                    _responseViewModel.ResponseMessage = "Could not update";
                    _responseViewModel.StatusCode = 500;
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
        public async Task<IActionResult> CreateHBLReschedule([FromForm] HBLReschedule reschedule)
        {

            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (reschedule.RescheduledLoanAmount == 0)
                {
                    _responseViewModel.Errors.Add(nameof(reschedule.RescheduledLoanAmount) + "Error", "Amount can not be zero or empty");
                }
                if (String.IsNullOrEmpty(reschedule.LoanTakenDateString))
                {
                    _responseViewModel.Errors.Add(nameof(reschedule.LoanTakenDateString) + "Error", "Invalid Loan Taken Date");
                }
                else
                {
                    DateTime convertedDate = DateTime.ParseExact(reschedule.LoanTakenDateString, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    reschedule.LoanTakenDate = convertedDate;
                    reschedule.LastProcessingDate = convertedDate;

                }
                if (reschedule.InterestRate == 0)
                {
                    _responseViewModel.Errors.Add(nameof(reschedule.InterestRate) + "Error", "InterestRate can not be zero or empty");
                }
                if (reschedule.InstallmentNo == 0)
                {
                    _responseViewModel.Errors.Add(nameof(reschedule.InstallmentNo) + "Error", "InstallmentNo can not be zero or empty");
                }
                if (reschedule.PrincipalInstallmentAmount == 0)
                {
                    _responseViewModel.Errors.Add(nameof(reschedule.PrincipalInstallmentAmount) + "Error", "Principal Installment Amount can not be zero or empty");
                }

                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }

                HbLoan _hbLoan = await _loanContract.GetHbLoanById(reschedule.HBLoanId);
                var result = await _loanContract.CreateHbLoanHistory(_hbLoan);

                // need to check result
                HbLoan _rescheduledLoan = new HbLoan();
                _rescheduledLoan.Id = reschedule.HBLoanId;
                _rescheduledLoan.LoanTakenDate = reschedule.LoanTakenDate;
                _rescheduledLoan.LastProcessingDate = reschedule.LoanTakenDate;
                _rescheduledLoan.TotalLoanAmount = reschedule.TotalLoanAmount;
                _rescheduledLoan.RemainingLoanAmount = reschedule.TotalLoanAmount;
                _rescheduledLoan.InterestRate = reschedule.InterestRate;
                _rescheduledLoan.TotalInterest = reschedule.RemainingInterest;
                _rescheduledLoan.RemainingInterest = reschedule.RemainingInterest;
                _rescheduledLoan.InstallmentNo = reschedule.InstallmentNo;
                _rescheduledLoan.RemainingInstallmentNo = reschedule.InstallmentNo;
                _rescheduledLoan.InterestInstallmentNo = reschedule.InterestInstallmentNo;
                _rescheduledLoan.RemainingInterestInstallmentNo = reschedule.InterestInstallmentNo;
                _rescheduledLoan.PrincipalInstallmentAmount = reschedule.PrincipalInstallmentAmount;
                _rescheduledLoan.IsPaused = false;
                _rescheduledLoan.IsRescheduled = true;
                _rescheduledLoan.UpdatedBy = "";
                _rescheduledLoan.UpdatedDate = DateTime.Now;
                if (_hbLoan.JobCode.ToLower().Contains("js"))
                {
                    _rescheduledLoan.DateAfterPeriod = _rescheduledLoan.LoanTakenDate.Value.AddMonths(11);
                }
                else
                {
                    _rescheduledLoan.DateAfterPeriod = _rescheduledLoan.LoanTakenDate.Value.AddMonths(1);
                }

                // need to check result
                var updateResult = await _loanContract.UpdateHbLoanForReschedule(_rescheduledLoan);

                _responseViewModel.ResponseMessage = "Data updated successfully";
                _responseViewModel.StatusCode = 201;
                return Ok(_responseViewModel);

            }
            catch (Exception ex)
            {
                _responseViewModel.ResponseMessage = ex.Message;
                _responseViewModel.StatusCode = 500;
                return Ok(_responseViewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> LoanProcess(int month, int year, int loanId)
        {
            try
            {
                int monthId = (year * 100) + month;
                //var officerSalaryData = await _salarySettingContract.FinalAdjustmentOfficer(monthId);
                //var juniorStaffSalaryData = await _salarySettingContract.FinalAdjustmentJuniorStaff(monthId);
                //if (officerSalaryData.Count>0 && juniorStaffSalaryData.Count>0)
                //{
                    if (loanId == 1)// hbl
                    {
                        var histories = await _loanContract.GetLoanProcessHistory(monthId, loanId);
                        if (histories.Count > 0)
                        {
                            _responseViewModel.StatusCode = 500;
                            _responseViewModel.ResponseMessage = "Loan already processed";
                        }
                        else
                        {
                            DateTime proccessDate = DateTime.Now.Date;
                            //int _year = monthId / 100;
                            //int _day = monthId % 100;
                            //DateTime proccessDate = new DateTime(_year, _day, 25);


                            var loans = await _loanContract.GetHbLoansByLoanType(isActive: 1, loanId);
                            if (loans.Count > 0)
                            {
                                var lastDateOfMonth = new DateTime(year, month, DateTime.DaysInMonth(year, month));

                                foreach (var item in loans)
                                {
                                    var hbLoan = await _loanContract.GetHbLoanById(item.Id);

                                    var lastProcessingDate = hbLoan.LastProcessingDate;
                                    var dateDifference = lastDateOfMonth - lastProcessingDate;

                                    if (proccessDate > hbLoan.DateAfterPeriod)
                                    {
                                        HblInstallment hblInstallment = new HblInstallment();
                                        hblInstallment.HbLoanId = item.Id;
                                        hblInstallment.MonthId = monthId;
                                        hblInstallment.JobCode = item.JobCode;


                                        if (hbLoan.IsPaused)
                                        {
                                            if (hbLoan.RemainingInstallmentNo > 0)
                                            {
                                                hblInstallment.InstallmentAmount = 0;
                                                hblInstallment.InterestAmount = (hbLoan.RemainingLoanAmount * (hbLoan.InterestRate / 100) / 365) * dateDifference.Value.Days;
                                                hblInstallment.InstallmentType = "principal";
                                                await _loanContract.CreateHbLoanInstallment(hblInstallment);

                                                hbLoan.LastProcessingDate = lastDateOfMonth;
                                                hbLoan.IsActive = true;
                                                hbLoan.UpdatedBy = "";
                                                hbLoan.UpdatedDate = DateTime.Now;
                                                hbLoan.DeactivatedDate = null;
                                                hbLoan.TotalInterest += hblInstallment.InterestAmount;
                                                hbLoan.RemainingInterest += hblInstallment.InterestAmount;

                                                await _loanContract.UpdateHbLoanByProcess(hbLoan);
                                            }
                                            else
                                            {
                                                hblInstallment.InstallmentAmount = 0;
                                                hblInstallment.InterestAmount = 0;
                                                hblInstallment.InstallmentType = "interest";
                                                await _loanContract.CreateHbLoanInstallment(hblInstallment);

                                                hbLoan.LastProcessingDate = lastDateOfMonth;
                                                hbLoan.IsActive = true;
                                                hbLoan.UpdatedBy = "";
                                                hbLoan.UpdatedDate = DateTime.Now;
                                                hbLoan.DeactivatedDate = null;
                                                await _loanContract.UpdateHbLoanByProcess(hbLoan);
                                            }

                                        }
                                        else
                                        {
                                            if (hbLoan.RemainingInstallmentNo > 0)
                                            {
                                                hblInstallment.InstallmentAmount = hbLoan.PrincipalInstallmentAmount;
                                                hblInstallment.InterestAmount = (hbLoan.RemainingLoanAmount * (hbLoan.InterestRate / 100) / 365) * dateDifference.Value.Days; ;
                                                hblInstallment.InstallmentType = "principal";
                                                await _loanContract.CreateHbLoanInstallment(hblInstallment);
                                                //await _loanContract.UpdateLoansInFinalAdjustment(item.JobCode, monthId, hblInstallment.InstallmentAmount, loanId);

                                                hbLoan.LastProcessingDate = lastDateOfMonth;
                                                hbLoan.IsActive = true;
                                                hbLoan.UpdatedBy = "";
                                                hbLoan.UpdatedDate = DateTime.Now;
                                                hbLoan.DeactivatedDate = null;
                                                hbLoan.TotalInterest += hblInstallment.InterestAmount;
                                                hbLoan.RemainingInterest += hblInstallment.InterestAmount;
                                                hbLoan.RemainingLoanAmount = hbLoan.RemainingLoanAmount - hbLoan.PrincipalInstallmentAmount;
                                                hbLoan.RemainingInstallmentNo = hbLoan.RemainingInstallmentNo - 1;

                                                await _loanContract.UpdateHbLoanByProcess(hbLoan);

                                            }
                                            else
                                            {
                                                hblInstallment.InstallmentAmount = hbLoan.InterestInstallmentAmount;
                                                hblInstallment.InterestAmount = 0;
                                                hblInstallment.InstallmentType = "interest";
                                                await _loanContract.CreateHbLoanInstallment(hblInstallment);
                                                //await _loanContract.UpdateLoansInFinalAdjustment(item.JobCode, monthId, hblInstallment.InstallmentAmount, loanId);

                                                if (hbLoan.RemainingInterestInstallmentNo == 1)
                                                {
                                                    hbLoan.LastProcessingDate = lastDateOfMonth;
                                                    hbLoan.IsActive = false;
                                                    hbLoan.UpdatedBy = "";
                                                    hbLoan.UpdatedDate = DateTime.Now;
                                                    hbLoan.DeactivatedDate = DateTime.Now.Date;
                                                    hbLoan.RemainingInterest = hbLoan.RemainingInterest - hbLoan.InterestInstallmentAmount;
                                                    hbLoan.RemainingInterestInstallmentNo = 0;
                                                    await _loanContract.UpdateHbLoanByProcess(hbLoan);
                                                }
                                                else
                                                {
                                                    hbLoan.LastProcessingDate = lastDateOfMonth;
                                                    hbLoan.IsActive = true;
                                                    hbLoan.UpdatedBy = "";
                                                    hbLoan.UpdatedDate = DateTime.Now;
                                                    hbLoan.DeactivatedDate = null;
                                                    hbLoan.RemainingInterest = hbLoan.RemainingInterest - hbLoan.InterestInstallmentAmount;
                                                    hbLoan.RemainingInterestInstallmentNo = hbLoan.RemainingInterestInstallmentNo - 1;
                                                    await _loanContract.UpdateHbLoanByProcess(hbLoan);
                                                }

                                            }
                                        }
                                    }
                                    else
                                    {
                                        HblInstallment hblInstallment = new HblInstallment();
                                        hblInstallment.HbLoanId = item.Id;
                                        hblInstallment.MonthId = monthId;
                                        hblInstallment.JobCode = item.JobCode;

                                        hblInstallment.InstallmentAmount = 0;
                                        hblInstallment.InterestAmount = (hbLoan.RemainingLoanAmount * (hbLoan.InterestRate / 100) / 365) * dateDifference.Value.Days;
                                        hblInstallment.InstallmentType = "beforeprincipal";
                                        await _loanContract.CreateHbLoanInstallment(hblInstallment);

                                        hbLoan.LastProcessingDate = lastDateOfMonth;
                                        hbLoan.IsActive = true;
                                        hbLoan.UpdatedBy = "";
                                        hbLoan.UpdatedDate = DateTime.Now;
                                        hbLoan.DeactivatedDate = null;
                                        hbLoan.TotalInterest += hblInstallment.InterestAmount;
                                        hbLoan.RemainingInterest += hblInstallment.InterestAmount;

                                        await _loanContract.UpdateHbLoanByProcess(hbLoan);
                                    }

                                }
                            }

                            await _loanContract.CreateloanProcessHistory(new LoanProcessHistory { LoanId = loanId, MonthId = monthId });
                            _responseViewModel.StatusCode = 201;
                            _responseViewModel.ResponseMessage = "Operation completed";

                        }
                    }
                    if (loanId == 2)//mcl
                    {
                        var histories = await _loanContract.GetLoanProcessHistory(monthId, loanId);
                        if (histories.Count > 0)
                        {
                            _responseViewModel.StatusCode = 500;
                            _responseViewModel.ResponseMessage = "Loan already processed";
                        }
                        else
                        {
                            var mclLoans = await _loanContract.GetMcLoans(isActive: 1);
                            if (mclLoans.Count > 0)
                            {
                                var lastDateOfMonth = new DateTime(year, month, DateTime.DaysInMonth(year, month));

                                foreach (var item in mclLoans)
                                {
                                    var mclLoan = await _loanContract.GetMcLoanById(item.Id);

                                    if (mclLoan != null && mclLoan.IsPaused == false)
                                    {
                                        if (mclLoan.RemainingInstallmentNo == 1)
                                        {
                                            mclLoan.RemainingInstallmentNo = 0;
                                            mclLoan.DeactivatedDate = DateTime.Now;
                                            mclLoan.IsActive = false;
                                            //mclLoan.LastProcessingDate = DateTime.Now;
                                            mclLoan.RemainingLoanAmount = mclLoan.RemainingLoanAmount - mclLoan.InstallmentAmount;

                                        }
                                        else
                                        {
                                            mclLoan.RemainingInstallmentNo = mclLoan.RemainingInstallmentNo - 1;
                                            mclLoan.IsActive = true;
                                            //mclLoan.LastProcessingDate = DateTime.Now;
                                            mclLoan.RemainingLoanAmount = mclLoan.RemainingLoanAmount - mclLoan.InstallmentAmount;

                                        }
                                        mclLoan.UpdatedBy = "";
                                        mclLoan.UpdatedDate = DateTime.Now;
                                        mclLoan.LastProcessingDate = lastDateOfMonth;
                                        var result = await _loanContract.UpdateMcLoanByProcess(mclLoan);
                                        if (result > 0)
                                        {
                                            MclInstallment mclInstallment = new MclInstallment
                                            {
                                                MonthId = monthId,
                                                InstallmentAmount = mclLoan.InstallmentAmount,
                                                JobCode = item.JobCode,
                                                MclId = item.Id
                                            };
                                            await _loanContract.CreateMclInstallment(mclInstallment);
                                            //await _loanContract.UpdateLoansInFinalAdjustment(item.JobCode, monthId, (double)mclLoan.InstallmentAmount, loanId);
                                        }
                                    }
                                }

                                await _loanContract.CreateloanProcessHistory(new LoanProcessHistory { LoanId = loanId, MonthId = monthId });
                                _responseViewModel.StatusCode = 201;
                                _responseViewModel.ResponseMessage = "Operation completed";
                            }
                        }

                    }
                    if (loanId == 3)//bycycle
                    {
                        // no implementation logic here.
                    }
                    if (loanId == 4)//com
                    {
                        var histories = await _loanContract.GetLoanProcessHistory(monthId, loanId);
                        if (histories.Count > 0)
                        {
                            _responseViewModel.StatusCode = 500;
                            _responseViewModel.ResponseMessage = "Loan already processed";
                        }
                        else
                        {
                            var comLoans = await _loanContract.GetComLoans(isActive: 1);
                            if (comLoans.Count > 0)
                            {
                                var lastDateOfMonth = new DateTime(year, month, DateTime.DaysInMonth(year, month));

                                foreach (var item in comLoans)
                                {
                                    var comLoan = await _loanContract.GetComLoanById(item.Id);

                                    ComInstallment comInstallment = new ComInstallment();
                                    comInstallment.ComLoanId = item.Id;
                                    comInstallment.MonthId = monthId;
                                    comInstallment.JobCode = item.JobCode;

                                    var lastProcessingDate = comLoan.LastProcessingDate;

                                    var dateDifference = lastDateOfMonth - lastProcessingDate;


                                    if (comLoan != null)
                                    {
                                        if (comLoan.IsPaused)
                                        {
                                            comInstallment.InstallmentAmount = 0;
                                            comInstallment.InterestAmount = (comLoan.RemainingLoanAmount * (comLoan.InterestRate / 100) / 365) * dateDifference.Value.Days;
                                            var result = await _loanContract.CreateComLoanInstallment(comInstallment);

                                        }
                                        else
                                        {
                                            if (comLoan.RemainingInstallmentNo == 1)
                                            {
                                                comLoan.RemainingInstallmentNo = 0;
                                                comLoan.DeactivatedDate = DateTime.Now;
                                                comLoan.IsActive = false;
                                                //comLoan.LastProcessingDate = DateTime.Now;


                                                comInstallment.InstallmentAmount = comLoan.InstallmentAmount;
                                                comInstallment.InterestAmount = (comLoan.RemainingLoanAmount * (comLoan.InterestRate / 100) / 365) * dateDifference.Value.Days;
                                                comLoan.RemainingLoanAmount = comLoan.RemainingLoanAmount - comLoan.InstallmentAmount;


                                            }
                                            else
                                            {
                                                comLoan.RemainingInstallmentNo = comLoan.RemainingInstallmentNo - 1;
                                                comLoan.IsActive = true;
                                                //comLoan.LastProcessingDate = DateTime.Now;

                                                comInstallment.InstallmentAmount = comLoan.InstallmentAmount;
                                                comInstallment.InterestAmount = (comLoan.RemainingLoanAmount * (comLoan.InterestRate / 100) / 365) * dateDifference.Value.Days;
                                                comLoan.RemainingLoanAmount = comLoan.RemainingLoanAmount - comLoan.InstallmentAmount;
                                            }

                                            comLoan.UpdatedBy = "";
                                            comLoan.UpdatedDate = DateTime.Now.Date;
                                            comLoan.LastProcessingDate = lastDateOfMonth;
                                            var result = await _loanContract.UpdateComLoanByProcess(comLoan);
                                            if (result > 0)
                                            {
                                                await _loanContract.CreateComLoanInstallment(comInstallment);
                                                //await _loanContract.UpdateLoansInFinalAdjustment(item.JobCode, monthId, (double)(comInstallment.InstallmentAmount + comInstallment.InterestAmount), loanId);
                                            }

                                        }
                                    }
                                }

                                await _loanContract.CreateloanProcessHistory(new LoanProcessHistory { LoanId = loanId, MonthId = monthId });
                                _responseViewModel.StatusCode = 201;
                                _responseViewModel.ResponseMessage = "Operation completed";
                            }
                        }
                    }
                    if (loanId == 9)//flat
                    {
                        var histories = await _loanContract.GetLoanProcessHistory(monthId, loanId);
                        if (histories.Count > 0)
                        {
                            _responseViewModel.StatusCode = 500;
                            _responseViewModel.ResponseMessage = "Loan already processed";
                        }
                        else
                        {
                            DateTime proccessDate = DateTime.Now.Date;

                            var loans = await _loanContract.GetHbLoansByLoanType(isActive: 1, loanId);
                            if (loans.Count > 0)
                            {
                                foreach (var item in loans)
                                {
                                    var hbLoan = await _loanContract.GetHbLoanById(item.Id);

                                    var lastProcessingDate = hbLoan.LastProcessingDate;
                                    var lastDateOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                                    var dateDifference = lastDateOfMonth - lastProcessingDate;

                                    if (proccessDate > hbLoan.DateAfterPeriod)
                                    {
                                        HblInstallment hblInstallment = new HblInstallment();
                                        hblInstallment.HbLoanId = item.Id;
                                        hblInstallment.MonthId = monthId;
                                        hblInstallment.JobCode = item.JobCode;


                                        if (hbLoan.IsPaused)
                                        {
                                            if (hbLoan.RemainingInstallmentNo > 0)
                                            {
                                                hblInstallment.InstallmentAmount = 0;
                                                hblInstallment.InterestAmount = (hbLoan.RemainingLoanAmount * (hbLoan.InterestRate / 100) / 365) * dateDifference.Value.Days;
                                                hblInstallment.InstallmentType = "principal";
                                                await _loanContract.CreateHbLoanInstallment(hblInstallment);

                                                hbLoan.LastProcessingDate = lastDateOfMonth.AddDays(1);
                                                hbLoan.IsActive = true;
                                                hbLoan.UpdatedBy = "";
                                                hbLoan.UpdatedDate = DateTime.Now;
                                                hbLoan.DeactivatedDate = null;
                                                hbLoan.TotalInterest += hblInstallment.InterestAmount;
                                                hbLoan.RemainingInterest += hblInstallment.InterestAmount;

                                                await _loanContract.UpdateHbLoanByProcess(hbLoan);
                                            }
                                            else
                                            {
                                                hblInstallment.InstallmentAmount = 0;
                                                hblInstallment.InterestAmount = 0;
                                                hblInstallment.InstallmentType = "interest";
                                                await _loanContract.CreateHbLoanInstallment(hblInstallment);

                                                hbLoan.LastProcessingDate = lastDateOfMonth.AddDays(1);
                                                hbLoan.IsActive = true;
                                                hbLoan.UpdatedBy = "";
                                                hbLoan.UpdatedDate = DateTime.Now;
                                                hbLoan.DeactivatedDate = null;
                                                await _loanContract.UpdateHbLoanByProcess(hbLoan);
                                            }

                                        }
                                        else
                                        {
                                            if (hbLoan.RemainingInstallmentNo > 0)
                                            {
                                                hblInstallment.InstallmentAmount = hbLoan.PrincipalInstallmentAmount;
                                                hblInstallment.InterestAmount = (hbLoan.RemainingLoanAmount * (hbLoan.InterestRate / 100) / 365) * dateDifference.Value.Days; ;
                                                hblInstallment.InstallmentType = "principal";
                                                await _loanContract.CreateHbLoanInstallment(hblInstallment);
                                                await _loanContract.UpdateLoansInFinalAdjustment(item.JobCode, monthId, hblInstallment.InstallmentAmount, loanId);

                                                hbLoan.LastProcessingDate = lastDateOfMonth.AddDays(1);
                                                hbLoan.IsActive = true;
                                                hbLoan.UpdatedBy = "";
                                                hbLoan.UpdatedDate = DateTime.Now;
                                                hbLoan.DeactivatedDate = null;
                                                hbLoan.TotalInterest += hblInstallment.InterestAmount;
                                                hbLoan.RemainingInterest += hblInstallment.InterestAmount;
                                                hbLoan.RemainingLoanAmount = hbLoan.RemainingLoanAmount - hbLoan.PrincipalInstallmentAmount;
                                                hbLoan.RemainingInstallmentNo = hbLoan.RemainingInstallmentNo - 1;

                                                await _loanContract.UpdateHbLoanByProcess(hbLoan);
                                            }
                                            else
                                            {
                                                hblInstallment.InstallmentAmount = hbLoan.InterestInstallmentAmount;
                                                hblInstallment.InterestAmount = 0;
                                                hblInstallment.InstallmentType = "interest";
                                                await _loanContract.CreateHbLoanInstallment(hblInstallment);
                                                await _loanContract.UpdateLoansInFinalAdjustment(item.JobCode, monthId, hblInstallment.InstallmentAmount, loanId);
                                                if (hbLoan.RemainingInterestInstallmentNo == 1)
                                                {
                                                    hbLoan.LastProcessingDate = lastDateOfMonth.AddDays(1);
                                                    hbLoan.IsActive = false;
                                                    hbLoan.UpdatedBy = "";
                                                    hbLoan.UpdatedDate = DateTime.Now;
                                                    hbLoan.DeactivatedDate = DateTime.Now.Date;
                                                    hbLoan.RemainingInterest = hbLoan.RemainingInterest - hbLoan.InterestInstallmentAmount;
                                                    hbLoan.RemainingInterestInstallmentNo = 0;
                                                    await _loanContract.UpdateHbLoanByProcess(hbLoan);
                                                }
                                                else
                                                {
                                                    hbLoan.LastProcessingDate = lastDateOfMonth.AddDays(1);
                                                    hbLoan.IsActive = true;
                                                    hbLoan.UpdatedBy = "";
                                                    hbLoan.UpdatedDate = DateTime.Now;
                                                    hbLoan.DeactivatedDate = null;
                                                    hbLoan.RemainingInterest = hbLoan.RemainingInterest - hbLoan.InterestInstallmentAmount;
                                                    hbLoan.RemainingInterestInstallmentNo = hbLoan.RemainingInterestInstallmentNo - 1;
                                                    await _loanContract.UpdateHbLoanByProcess(hbLoan);
                                                }

                                            }
                                        }
                                    }
                                    else
                                    {
                                        HblInstallment hblInstallment = new HblInstallment();
                                        hblInstallment.HbLoanId = item.Id;
                                        hblInstallment.MonthId = monthId;
                                        hblInstallment.JobCode = item.JobCode;

                                        hblInstallment.InstallmentAmount = 0;
                                        hblInstallment.InterestAmount = (hbLoan.RemainingLoanAmount * (hbLoan.InterestRate / 100) / 365) * dateDifference.Value.Days;
                                        hblInstallment.InstallmentType = "beforeprincipal";
                                        await _loanContract.CreateHbLoanInstallment(hblInstallment);

                                        hbLoan.LastProcessingDate = lastDateOfMonth.AddDays(1);
                                        hbLoan.IsActive = true;
                                        hbLoan.UpdatedBy = "";
                                        hbLoan.UpdatedDate = DateTime.Now;
                                        hbLoan.DeactivatedDate = null;
                                        hbLoan.TotalInterest += hblInstallment.InterestAmount;
                                        hbLoan.RemainingInterest += hblInstallment.InterestAmount;

                                        await _loanContract.UpdateHbLoanByProcess(hbLoan);
                                    }

                                }
                            }

                            await _loanContract.CreateloanProcessHistory(new LoanProcessHistory { LoanId = loanId, MonthId = monthId });
                            _responseViewModel.StatusCode = 201;
                            _responseViewModel.ResponseMessage = "Operation completed";

                        }
                    }
                    if (loanId == 10)//car
                    {
                        var histories = await _loanContract.GetLoanProcessHistory(monthId, loanId);
                        if (histories.Count > 0)
                        {
                            _responseViewModel.StatusCode = 500;
                            _responseViewModel.ResponseMessage = "Loan already processed";
                        }
                        else
                        {
                            var carLoans = await _loanContract.GetCarLoans(isActive: 1);
                            if (carLoans.Count > 0)
                            {
                                var lastDateOfMonth = new DateTime(year, month, DateTime.DaysInMonth(year, month));

                                foreach (var item in carLoans)
                                {
                                    var nextMonth = Convert.ToDateTime(item.LoanTakenDate).AddMonths(1);
                                    var startMonthId = nextMonth.Year * 100 + nextMonth.Month;
                                    var endMonthId = monthId;

                                    var carLoan = await _loanContract.GetCarLoanById(item.Id);
                                    var carLoanInstallments = await _loanContract.GetCarLoanInstallments(item.Id);
                                    var carLoanInstallment = carLoanInstallments.Where(ci => ci.MonthId == monthId).SingleOrDefault();

                                    if (carLoanInstallment != null)
                                    {
                                        var carLoanDepreciations = await _loanContract.GetCarLoanDepreciationInstallmentsByMonth(item.Id, startMonthId, endMonthId);
                                        var totaldepreciation = carLoanDepreciations.Sum(x => x.DepreciationAmount);

                                        if (carLoan.RemainingInstallmentNo == 1)
                                        {
                                            carLoan.IsActive = false;
                                            carLoan.DeactivatedDate = DateTime.Now;
                                            carLoan.RemainingInstallmentNo = 0;
                                            carLoan.RemainingActualAmount = carLoan.RemainingActualAmount - carLoanInstallment.PrincipalAmount;
                                            carLoan.RemainingDepreciationAmount = carLoan.RemainingDepreciationAmount - totaldepreciation;

                                        }
                                        else
                                        {
                                            carLoan.RemainingInstallmentNo = carLoan.RemainingInstallmentNo - 1;
                                            carLoan.RemainingActualAmount = carLoan.RemainingActualAmount - carLoanInstallment.PrincipalAmount;
                                            carLoan.RemainingDepreciationAmount = carLoan.RemainingDepreciationAmount - totaldepreciation;
                                        }

                                        carLoan.UpdatedBy = "";
                                        carLoan.UpdatedDate = DateTime.Now;
                                        carLoan.LastProcessingDate = lastDateOfMonth;

                                        var result = await _loanContract.UpdateCarLoan(carLoan);
                                        if (result > 0)
                                        {
                                            carLoanInstallment.IsPaid = true;
                                            carLoanInstallment.DepreciationAmount = totaldepreciation;
                                            var installmentUpdateResult = await _loanContract.UpdateCarLoanInstallment(carLoanInstallment);
                                            //await _loanContract.UpdateLoansInFinalAdjustment(item.JobCode, monthId, (carLoanInstallment.PrincipalAmount + carLoanInstallment.InterestAmount), loanId);

                                            foreach (var carLoanDepreciation in carLoanDepreciations)
                                            {
                                                carLoanDepreciation.IsPaid = true;
                                                var depreciationUpdateResult = await _loanContract.UpdateCarLoanDepreciationInstallment(carLoanDepreciation);
                                            }

                                        }
                                    }

                                }

                                await _loanContract.CreateloanProcessHistory(new LoanProcessHistory { LoanId = loanId, MonthId = monthId });
                                _responseViewModel.StatusCode = 201;
                                _responseViewModel.ResponseMessage = "Operation completed";
                            }
                        }

                    }

                    return Ok(_responseViewModel);
                //}
                //else
                //{
                //    _responseViewModel.StatusCode = 500;
                //    _responseViewModel.ResponseMessage = "Salary is not processed yet";
                //    return Ok(_responseViewModel);
                //}
               
            }
            catch (Exception ex)
            {
                _responseViewModel.StatusCode = 500;
                _responseViewModel.ResponseMessage = "Something went wrong";
                return Ok(_responseViewModel);
            }
        }


        
        public async Task<IActionResult> TempLoanProcess()
        {
            int monthId = 202411;
            // hb loan
            var hbLoans = await _loanContract.GetHbLoans(isActive:1);
            foreach (var item in hbLoans)
            {
                var installment = await _loanContract.GetInstallmentByHBLoanId_MonthId(item.Id, monthId);
                if (installment!=null)
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

            return Ok("done");
        }




        // internal methods.

        public static List<CarLoanInstallment> CalculateAmortizationSchedule(CarLoan loan)
        {

            var amortizationSchedule = new List<CarLoanInstallment>();
            var monthlyInterestRate = (double)(loan.InterestRate / 12.0 / 100.0);
            var monthlyPayment = LoansController.CalculateMonthlyPayment(loan.ActualAmount, monthlyInterestRate, loan.InstallmentNo);

            var remainingBalance = loan.ActualAmount;
            DateTime _fromDate = new DateTime(loan.EmiStartMonth.Year, loan.EmiStartMonth.Month, 1);

            for (var number = 1; number <= loan.InstallmentNo; number++)
            {
                
                var interestPayment = remainingBalance * monthlyInterestRate;
                var principalPayment = monthlyPayment - interestPayment;

                remainingBalance -= principalPayment;
                if (remainingBalance < principalPayment)
                {
                    principalPayment += remainingBalance;
                    monthlyPayment += remainingBalance;
                    remainingBalance = 0;
                }

                var payment = new CarLoanInstallment
                {
                    CarLoanId = loan.Id,
                    MonthId = LoansController.GetMonthID(_fromDate.Month, _fromDate.Year),
                    PrincipalAmount = principalPayment,
                    InterestAmount = interestPayment,
                    TotalPayment = monthlyPayment,
                    RemainingBalance = remainingBalance,
                    IsPaid = false
                };

                _fromDate = _fromDate.AddMonths(1);
                amortizationSchedule.Add(payment);
            }

            return amortizationSchedule;
        }

        public static List<DepreciationInstallment> CalculateDepreciation(CarLoan carLoan)
        {
            var depreciationInstallments = new List<DepreciationInstallment>();
            double remainingAmount = carLoan.TotalLoanAmount;
            double depreciationPercentage = 0.10;

            DateTime startDate = carLoan.LoanTakenDate.AddMonths(1);


            for (int i = 0; i < 8; i++) // Loop for 8 years
            {
                // Calculate the annual depreciation
                double annualDepreciation = remainingAmount * depreciationPercentage;

                // Monthly depreciation amount
                double monthlyDepreciation = annualDepreciation / 12;

                // Create and add the DepreciationInstallment for each month of the current year
                for (int month = 1; month <= 12; month++)
                {

                    int monthId = startDate.Year * 100 + startDate.Month; // Format YYYYMM

                    var installment = new DepreciationInstallment
                    {
                        Id = (i * 12) + month,
                        CarLoanId = carLoan.Id, 
                        MonthId = monthId,
                        DepreciationAmount = monthlyDepreciation,
                        IsPaid = false 
                    };

                    depreciationInstallments.Add(installment);
                    startDate = startDate.AddMonths(1);
                   
                }

                remainingAmount -= annualDepreciation;
            }

            return depreciationInstallments;
        }

        public static double CalculateMonthlyPayment(double loanAmount, double monthlyInterestRate, int numberOfPayments)
        {
            if (monthlyInterestRate <= 0)
                return loanAmount / numberOfPayments;

            var power = (double)Math.Pow(1 + (double)monthlyInterestRate, numberOfPayments);
            return loanAmount * (monthlyInterestRate * power) / (power - 1);
        }

        public static int GetMonthID(int month, int year)
        {
            return (year * 100 + month);
        }
    }
}
