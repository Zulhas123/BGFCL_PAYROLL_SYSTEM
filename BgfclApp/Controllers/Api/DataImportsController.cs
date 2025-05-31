using BgfclApp.ViewModels;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BgfclApp.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DataImportsController : ControllerBase
    {
        private IDataImportContract _dataImportContract;
        private IEmployeeContract _employeeContract;
        private ISalarySettingContract _salarySettingContract;
        private ResponseViewModel _responseViewModel;

        public DataImportsController( ResponseViewModel responseViewModel, IDataImportContract dataImportContract, IEmployeeContract employeeContract)
        {

            _responseViewModel = responseViewModel;
            _employeeContract = employeeContract;
            _dataImportContract = dataImportContract;

        }
        [HttpPost]
        public async Task<IActionResult> ImportEmployeeData([FromBody] List<EmployeeImportData> employeeData)
        {

            _responseViewModel.Errors = new Dictionary<string, string>();
            if (employeeData == null || !employeeData.Any())
            {
                _responseViewModel.ResponseMessage = "No data provided.";
                _responseViewModel.StatusCode = 400;
                return BadRequest(_responseViewModel);
            }

            try
            {
                foreach (var data in employeeData)
                {
                    var result = await _dataImportContract.ImportEmployeeData(data);

                    if (result <= 0)
                    {
                        _responseViewModel.ResponseMessage = "Could not Import some entries.";
                        _responseViewModel.StatusCode = 500; // Internal Server Error
                        continue;
                    }
                }

                _responseViewModel.ResponseMessage = "Data Import successfully.";
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
        [HttpPost]
        public async Task<IActionResult> ImportDepartmentData([FromBody] List<DepartmentImportData> departmentData)
        {

            _responseViewModel.Errors = new Dictionary<string, string>();
            if (departmentData == null || !departmentData.Any())
            {
                _responseViewModel.ResponseMessage = "No data provided.";
                _responseViewModel.StatusCode = 400;
                return BadRequest(_responseViewModel);
            }

            try
            {
                foreach (var data in departmentData)
                {
                    var result = await _dataImportContract.ImportDepartmentData(data);

                    if (result <= 0)
                    {
                        _responseViewModel.ResponseMessage = "Could not Import some entries.";
                        _responseViewModel.StatusCode = 500; // Internal Server Error
                        continue;
                    }
                }

                _responseViewModel.ResponseMessage = "Data Import successfully.";
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
        [HttpPost]
        public async Task<IActionResult> ImportDesignationData([FromBody] List<DesignationImportData> designationData)
        {

            _responseViewModel.Errors = new Dictionary<string, string>();
            if (designationData == null || !designationData.Any())
            {
                _responseViewModel.ResponseMessage = "No data provided.";
                _responseViewModel.StatusCode = 400;
                return BadRequest(_responseViewModel);
            }

            try
            {
                foreach (var data in designationData)
                {
                    var result = await _dataImportContract.ImportDesignationData(data);

                    if (result <= 0)
                    {
                        _responseViewModel.ResponseMessage = "Could not Import some entries.";
                        _responseViewModel.StatusCode = 500; // Internal Server Error
                        continue;
                    }
                }

                _responseViewModel.ResponseMessage = "Data Import successfully.";
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
        [HttpPost]
        public async Task<IActionResult> ImportRoleData([FromBody] List<Roles> role)
        {

            _responseViewModel.Errors = new Dictionary<string, string>();
            if (role == null || !role.Any())
            {
                _responseViewModel.ResponseMessage = "No data provided.";
                _responseViewModel.StatusCode = 400;
                return BadRequest(_responseViewModel);
            }

            try
            {
                foreach (var data in role)
                {
                    var result = await _dataImportContract.ImportRoleData(data);

                    if (result <= 0)
                    {
                        _responseViewModel.ResponseMessage = "Could not Import some entries.";
                        _responseViewModel.StatusCode = 500; // Internal Server Error
                        continue;
                    }
                }

                _responseViewModel.ResponseMessage = "Data Import successfully.";
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
    }
}
