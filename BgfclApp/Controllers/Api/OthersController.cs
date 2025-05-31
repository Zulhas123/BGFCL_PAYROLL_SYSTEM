using BgfclApp.ViewModels;
using Contracts;
using Entities;
using Entities.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BgfclApp.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OthersController : ControllerBase
    {
        private IMaritalContract _maritalContract;
        private IGenderContract _genderContract;
        private IReligionContract _religionContract;
        private IActiveStatusContract _activeStatusContract;
        private ILoanContract _loanContract;
        private ResponseViewModel _responseViewModel;

        public OthersController(IMaritalContract maritalContract,IGenderContract genderContract, IReligionContract religionContract, ResponseViewModel responseViewModel, IActiveStatusContract activeStatusContract, ILoanContract loanContract)
        {
            _maritalContract = maritalContract;
            _genderContract = genderContract;
            _religionContract = religionContract;
            _responseViewModel = responseViewModel;
            _activeStatusContract = activeStatusContract;
            _loanContract = loanContract;
        }


        [HttpGet]
        public async Task<IActionResult> GetMaritals()
        {
            try
            {
                var maritals = await _maritalContract.GetMaritals();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = maritals;
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
        public async Task<IActionResult> GetGenders()
        {
            try
            {
                var genders = await _genderContract.GetGenders();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = genders;
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
        public async Task<IActionResult> GetReligions()
        {
            try
            {
                var religions = await _religionContract.GetReligions();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = religions;
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
        public async Task<IActionResult> GetActiveStatus()
        {
            try
            {
                var activeStatus = await _activeStatusContract.GetActiveStatus();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = activeStatus;
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
        public async Task<IActionResult> GetMonths()
        {
            List<Month> listMonth = new List<Month>();
            string[] monthString = { "Select", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            for (int index = 0; index <= 12; index++)
            {
                Month month = new Month();
                month.MonthId = index;
                month.MonthName = monthString[index];
                listMonth.Add(month);

            }
            _responseViewModel.StatusCode = 200;
            _responseViewModel.Data = listMonth;
            return Ok(_responseViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetYears(int offSet=5)
        {
            List<int> listYear = new List<int>();
            int currentYear = DateTime.Now.Year;
            for (int index = currentYear - 5; index <= currentYear + 5; index++)
            {
                listYear.Add(index);
            }
            _responseViewModel.StatusCode = 200;
            _responseViewModel.Data = listYear;
            return Ok(_responseViewModel);
        }
        [HttpGet]
        public async Task<IActionResult> GetLoans(string loanType="internal")
        {

            List<Loan> loans = await _loanContract.GetLoans(loanType);
            _responseViewModel.StatusCode = 200;
            _responseViewModel.Data = loans;
            return Ok(_responseViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetLoanHeadByEmpType(int employeeTypeId)
        {

            List<LoanHeadViewModel> loans = await _loanContract.GetLoanHeadByEmpType(employeeTypeId,"Loan");
            _responseViewModel.StatusCode = 200;
            _responseViewModel.Data = loans;
            return Ok(_responseViewModel);
        }
    }
}
