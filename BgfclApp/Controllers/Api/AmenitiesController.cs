using BgfclApp.ViewModels;
using Contracts;
using DocumentFormat.OpenXml.Math;
using Entities;
using Entities.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace BgfclApp.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AmenitiesController : ControllerBase
    {
        private IEmployeeContract _employeeContract;
        private IAmenitiesContract _amenitiesContract;
        private ResponseViewModel _responseViewModel;
        public AmenitiesController(IEmployeeContract employeeContract, ResponseViewModel responseViewModel,IAmenitiesContract amenitiesContract)
        {
            _employeeContract = employeeContract;
            _responseViewModel = responseViewModel;
            _amenitiesContract = amenitiesContract;
        }
        [HttpGet]
        public async Task<IActionResult> GetEmployeesForAmenities(int employeeTypeId)
        {
            List<EmployeeViewModel> filteredEmployees = new List<EmployeeViewModel>();
            var employees = await _employeeContract.GetEmployeesByEmployeeType(employeeTypeId);

            // amenities is only for officer
            var amenities = await _amenitiesContract.GetAmenities();
            foreach (var employee in employees)
            {
                var _setting = amenities.Where(s => s.JobCode == employee.JobCode).SingleOrDefault();
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
        public async Task<IActionResult> GetAmenities()
        {
            try
            {
                var amenities = await _amenitiesContract.GetAmenities();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = amenities;
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
        public async Task<IActionResult> GetAmenitiesLastMonthProcess()
        {
            try
            {
                var amenities = await _amenitiesContract.GetLastMonthAmenitiesProcess();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = amenities;
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
        public async Task<IActionResult> GetAmenitiesLastMonthAmount()
        {
            try
            {
                var amenities = await _amenitiesContract.GetLastMonthAmenitiesAmount();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = amenities;
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
        public async Task<IActionResult> amenitiesJournalAdjustment(int monthId)
        {
            try
            {
                var finalAdjustments = await _amenitiesContract.JournalAdjustment(monthId);
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
        [HttpPost]
        public async Task<IActionResult> UpdateJournalAdjustment(FinalAdjustments journalAdjustment)
        {
            if (journalAdjustment.JournalAdjustment == null || journalAdjustment.JournalAdjustment.Count == 0)
            {
                return BadRequest(new { Message = "The salaryJournals field is required." });
            }

            try
            {
                foreach (var salaryJournal in journalAdjustment.JournalAdjustment)
                {
                    await _amenitiesContract.UpdateJournalAdjustment(salaryJournal);
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
        public async Task<IActionResult> InsertJournalAdjustment(List<JournalAdjustmentOfficer> amenitiesJournals)
        {
            try
            {
                // Insert the new salary journal and retrieve the new ID
                foreach (var item in amenitiesJournals)
                {
                    var newRecord = await _amenitiesContract.InsertNewamenitiesJournal(item);
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
        public async Task<IActionResult> DeleteAmenitiesjournal(int id)
        {
            try
            {
                var result = await _amenitiesContract.DeleteAmenitiesJournal(id);

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

        [HttpPost]
        public async Task<IActionResult> CreateAmenities([FromForm] Amenities amenities )
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            if (amenities.AccountNumber == null)
            {
                amenities.AccountNumber = "0.00";
            }
            try
            {
                // validation logic
                {
                    if (String.IsNullOrEmpty(amenities.JobCode))
                    {
                        _responseViewModel.Errors.Add(nameof(amenities.JobCode) + "Error", "Jobcode required");
                    }
                    if (amenities.PayModeId == 0)
                    {
                        _responseViewModel.Errors.Add(nameof(amenities.PayModeId) + "Error", "Paymode required");
                    }
                    if (amenities.BankId == 0)
                    {
                        _responseViewModel.Errors.Add(nameof(amenities.BankId) + "Error", "Bank required");
                    }
                    if (amenities.BankBranchId == 0)
                    {
                        _responseViewModel.Errors.Add(nameof(amenities.BankBranchId) + "Error", "Bank Branch required");
                    }
                    if (String.IsNullOrEmpty(amenities.AccountNumber))
                    {
                        _responseViewModel.Errors.Add(nameof(amenities.AccountNumber) + "Error", "Account Number required");
                    }
                }

                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }

                amenities.CreatedBy = "";
                amenities.CreatedDate = DateTime.Now;
                var existingData = await _amenitiesContract.GetAmenitiesByJobCode(amenities.JobCode);

                if (existingData == null)
                {
                        int result = await _amenitiesContract.CreateAmenities(amenities);
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

        [HttpPut]
        public async Task<IActionResult> UpdateAmenities([FromForm] Amenities amenities)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                // validation logic
                {
                    if (amenities.PayModeId == 0)
                    {
                        _responseViewModel.Errors.Add(nameof(amenities.PayModeId) + "Error", "Paymode required");
                    }
                    if (amenities.BankId == 0)
                    {
                        _responseViewModel.Errors.Add(nameof(amenities.BankId) + "Error", "Bank required");
                    }
                    if (amenities.BankBranchId == 0)
                    {
                        _responseViewModel.Errors.Add(nameof(amenities.BankBranchId) + "Error", "Bank Branch required");
                    }
                    if (String.IsNullOrEmpty(amenities.AccountNumber))
                    {
                        _responseViewModel.Errors.Add(nameof(amenities.AccountNumber) + "Error", "Account Number required");
                    }
                }

                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }

                amenities.UpdatedBy = "";
                amenities.UpdatedDate = DateTime.Now;
                int result = await _amenitiesContract.UpdateAmenities(amenities);
                if (result > 0)
                {
                    _responseViewModel.ResponseMessage = "Record updated successfully";
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

        [HttpGet]
        public async Task<IActionResult> GetAmenitiesById(int amenitiesId)
        {
            try
            {
                var amenities = await _amenitiesContract.GetAmenitiesById(amenitiesId);
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = amenities;
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
        public async Task<IActionResult> ProcessAmenities(int monthId,int status)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (status == 2)
                {
                    var result = await _amenitiesContract.ProcessAmenities(monthId, status);
                    if (result>0)
                    {
                        _responseViewModel.ResponseMessage = "Operation successfull";
                        _responseViewModel.StatusCode = 201;
                        return Ok(_responseViewModel);
                    }
                    else
                    {
                        _responseViewModel.ResponseMessage = "Could not process amenities";
                        _responseViewModel.StatusCode = 500;
                        return Ok(_responseViewModel);
                    }
                }

                var amenitiesMaster = await _amenitiesContract.GetAmenitiesReportMasterByMonthId(monthId);
                if(amenitiesMaster!=null && amenitiesMaster.Status==1)
                {
                    _responseViewModel.ResponseMessage = "Amenities already processed, do you want to continue?";
                    _responseViewModel.StatusCode = 200;
                    return Ok(_responseViewModel);
                }
                else if(amenitiesMaster != null && amenitiesMaster.Status == 2)
                {
                    _responseViewModel.ResponseMessage = "Amenities already processed, can not process again.";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                else
                {
                    var result = await _amenitiesContract.ProcessAmenities(monthId, status);
                    if (result > 0)
                    {
                        _responseViewModel.ResponseMessage = "Operation successfull";
                        _responseViewModel.StatusCode = 201;
                        return Ok(_responseViewModel);
                    }
                    {
                        _responseViewModel.ResponseMessage = "Could not process amenities";
                        _responseViewModel.StatusCode = 500;
                        return Ok(_responseViewModel);
                    }
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
        public async Task<IActionResult> GetAmenitiesFinalAdjustmentsByMonthId(int monthId)
        {
            try
            {
                var amenitiesFinalAdjustments = await _amenitiesContract.GetAmenitiesFinalAdjustmentsByMonthId(monthId);

                if (amenitiesFinalAdjustments.Count>0)
                {
                    _responseViewModel.StatusCode = 200;
                    _responseViewModel.Data = amenitiesFinalAdjustments;
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
                _responseViewModel.StatusCode = 500;
                _responseViewModel.ResponseMessage = "Something went wrong";
                return Ok(_responseViewModel);
            }

        }

        public class FinalAdjustments
        {
            public List<AmenitiesFinalAdjustmentViewModel>? AmenitiesFinalAdjustments { get; set; }
            public List<JournalAdjustmentOfficer>? JournalAdjustment { get; set; }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAmenitiesFinalAdjustment(FinalAdjustments finalAdjustments)
        {
            int result = 0;

            try
            {
                if (finalAdjustments.AmenitiesFinalAdjustments.Count>0)
                {
                    foreach (var item in finalAdjustments.AmenitiesFinalAdjustments)
                    {
                        result += await _amenitiesContract.UpdateAmenitiesFinalAdjustment(item);
                    }

                    if (result > 0)
                    {
                        _responseViewModel.ResponseMessage = result + " Rows updated";
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
        public async Task<IActionResult> AmenitiesJournalPosting(int monthId, int employeeTypeId)
        {
            var journalMaster = await _amenitiesContract.GetAmenitiesJournalMaster(monthId, employeeTypeId);
            if (journalMaster == null)
            {
                try
                {
                    List<AmenitiesJournal> journals = new List<AmenitiesJournal>();
                    List<string> journalCodes = new List<string>();

                    if (employeeTypeId == 1)
                    {
                        var amenitiesData = await _amenitiesContract.GetAmenitiesFinalAdjustmentsByMonthId(monthId);
                        foreach (var item in amenitiesData)
                        {
                            if (!journalCodes.Contains(item.JournalCode))
                            {
                                journalCodes.Add(item.JournalCode);
                            }

                        }

                        journalCodes = journalCodes.OrderBy(j => j).ToList();


                        // this is for Revenue Stamp.
                        decimal totalRevenue = (decimal)amenitiesData.Sum(s => s.RevenueStamp);
                        journals.Add(new AmenitiesJournal
                        {
                            AccountNumber = "5804000000",
                            JournalCode = "99",
                            Details = "Revenue Stamp",
                            Debit = null,
                            Credit = totalRevenue
                        });

                        // Other Deduction
                        decimal totalOtherDeduction = (decimal)amenitiesData.Sum(s => s.OtherDeduction);
                        journals.Add(new AmenitiesJournal
                        {
                            AccountNumber = "9999999999",
                            JournalCode = "99",
                            Details = "Other Deduction",
                            Debit = null,
                            Credit = totalOtherDeduction

                        });

                        // this is for Suspense A/C.
                        journals.Add(new AmenitiesJournal
                        {
                            AccountNumber = "5800500000",
                            JournalCode = "99",
                            Details = "Suspense A/C",
                            Debit = null,
                            Credit = 0
                        });

                        decimal totalCredit = 0;

                        foreach (var item in journalCodes)
                        {
                            var journalCodeWiseSalaryData = amenitiesData.Where(s => s.JournalCode == item).ToList();

                            // Servant Allowance
                            decimal _servantAllow = (decimal)journalCodeWiseSalaryData.Sum(s => s.WageGMSCB);
                            journals.Add(new AmenitiesJournal
                            {
                                AccountNumber = "8122500000",
                                JournalCode = item,
                                Details = "Servant Allowance",
                                Debit = _servantAllow,
                                Credit = null

                            });
                            totalCredit = totalCredit + _servantAllow;

                            // House Up-keeping
                            decimal _houseKeepUp = (decimal)journalCodeWiseSalaryData.Sum(s => s.HouseKeepUp);
                            journals.Add(new AmenitiesJournal
                            {
                                AccountNumber = "8704010000",
                                JournalCode = item,
                                Details = "House Up-keeping",
                                Debit = _houseKeepUp,
                                Credit = null

                            });
                            totalCredit = totalCredit + _houseKeepUp;

                            // Gas Subsidies
                            decimal _fuelSubsidy = (decimal)journalCodeWiseSalaryData.Sum(s => s.FuelSubsidy);
                            journals.Add(new AmenitiesJournal
                            {
                                AccountNumber = "8111000000",
                                JournalCode = item,
                                Details = "Gas Subsidies",
                                Debit = _fuelSubsidy,
                                Credit = null

                            });
                            totalCredit = totalCredit + _fuelSubsidy;

                            // Water & Electricity
                            decimal weSubsidy = (decimal)journalCodeWiseSalaryData.Sum(s => s.WESubsidy);
                            journals.Add(new AmenitiesJournal
                            {
                                AccountNumber = "8122000000",
                                JournalCode = item,
                                Details = "Water & Electricity",
                                Debit = weSubsidy,
                                Credit = null

                            });
                            totalCredit = totalCredit + weSubsidy;
                        }

                        journals[2].Credit = totalCredit - totalRevenue - totalOtherDeduction;


                        AmenitiesJournalMaster amenitiesJournalMaster = new AmenitiesJournalMaster();
                        amenitiesJournalMaster.MonthId = monthId;
                        amenitiesJournalMaster.EmployeeTypeId = employeeTypeId;
                        amenitiesJournalMaster.CreatedBy = "";
                        amenitiesJournalMaster.CreatedDate = DateTime.Now;
                        var journalMasterId = await _amenitiesContract.CreateAmenitiesJournalMaster(amenitiesJournalMaster);

                        if (journalMasterId > 0)
                        {
                            foreach (var journal in journals)
                            {
                                // journal.JournalMasterId = journalMasterId;
                                journal.JournalMasterId = journalMasterId;
                                await _amenitiesContract.CreateAmenitiesJournal(journal);
                            }
                        }
                        _responseViewModel.StatusCode = 201;
                        _responseViewModel.ResponseMessage = "Operation competed";
                        return Ok(_responseViewModel);
                    }
                    else
                    {
                        _responseViewModel.StatusCode = 500;
                        _responseViewModel.ResponseMessage = "Not Valid for Junior Staff";
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
            else{
                _responseViewModel.StatusCode = 500;
                _responseViewModel.ResponseMessage = "Data already exist";
                return Ok(_responseViewModel);
            }
        }


    }
}
