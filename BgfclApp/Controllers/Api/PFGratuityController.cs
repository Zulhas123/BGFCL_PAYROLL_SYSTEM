using BgfclApp.ViewModels;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BgfclApp.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PFGratuityController : ControllerBase
    {

        private ResponseViewModel _responseViewModel;
        private IPFGratuityContract _pfContract;
        public PFGratuityController(IPFGratuityContract pfContract,
            ResponseViewModel responseViewModel,
            IEmployeeContract employeeContract)
        {

            _responseViewModel = responseViewModel;
            _pfContract = pfContract;
        }

        [HttpGet]
        public async Task<IActionResult> GetProvidentFund()
        {
            try
            {
                var pf = await _pfContract.GetProvidentFund();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = pf;
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
        public async Task<IActionResult> GetProvidentFundById(int id)
        {

            try
            {
                var pf = await _pfContract.GetProvidentFundById(id);
                if (pf == null)
                {
                    _responseViewModel.StatusCode = 404;
                    _responseViewModel.ResponseMessage = "Data not found";
                    return Ok(_responseViewModel);
                }
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = pf;
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
        public async Task<IActionResult> UpdateProvidentFund([FromForm] ProvidentFundData provident)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                var existingProvident = await _pfContract.GetProvidentFundById(provident.Id);

                if (existingProvident == null)
                {
                    _responseViewModel.ResponseMessage = "Record not found";
                    _responseViewModel.StatusCode = 404;
                    return Ok(_responseViewModel);
                }

                // Update fields
                existingProvident.InterestRate = provident.InterestRate;
                existingProvident.EmpOpeningContribution = provident.EmpOpeningContribution;
                existingProvident.EmpCurrentYearContribution = provident.EmpCurrentYearContribution;
                existingProvident.EmpEndingContribution = provident.EmpEndingContribution;
                existingProvident.EmpSubTotal = provident.EmpSubTotal;
                existingProvident.CompanyOpeningContribution = provident.CompanyOpeningContribution;
                existingProvident.CompanyCurrentYearContribution = provident.CompanyCurrentYearContribution;
                existingProvident.CompanyEndingContribution = provident.CompanyEndingContribution;
                existingProvident.CompanySubTotal = provident.CompanySubTotal;
                existingProvident.InterestAsOpening = provident.InterestAsOpening;
                existingProvident.InterestAsEnding = provident.InterestAsEnding;
                existingProvident.InterestAsYear = provident.InterestAsYear;
                existingProvident.TotalContribution = provident.TotalContribution;
                existingProvident.GrandTotal = provident.GrandTotal;


                int result = await _pfContract.UpdateProvidentFund(existingProvident);

                _responseViewModel.ResponseMessage = "Record updated successfully";
                _responseViewModel.StatusCode = 200;
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
