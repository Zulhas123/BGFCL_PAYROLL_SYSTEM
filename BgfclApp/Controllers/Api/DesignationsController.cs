using BgfclApp.ViewModels;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BgfclApp.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DesignationsController : ControllerBase
    {
        private IDesignationContract _designationContract;
        private ResponseViewModel _responseViewModel;
        public DesignationsController(IDesignationContract designationContract, ResponseViewModel responseViewModel)
        {
            _designationContract = designationContract;
            _responseViewModel = responseViewModel;
        }


        [HttpGet]
        public async Task<IActionResult> GetDesignations()
        {
            try
            {
                var designations = await _designationContract.GetDesignations();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = designations;
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
        public async Task<IActionResult> GetDesignationsByEmployeeType(int employeeTypeId)
        {
            try
            {
                var designations = await _designationContract.GetDesignationsByEmployeeType(employeeTypeId);
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = designations;
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
        public async Task<IActionResult> GetDesignationById(int id)
        {

            try
            {
                var designation = await _designationContract.GetDesignationById(id);
                if (designation == null)
                {
                    _responseViewModel.StatusCode = 404;
                    _responseViewModel.ResponseMessage = "Data not found";
                    return Ok(_responseViewModel);
                }
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = designation;
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
        public async Task<IActionResult> CreateDesignation([FromForm] Designation designation)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (designation.EmployeeTypeId==0)
                {
                    _responseViewModel.Errors.Add(nameof(designation.EmployeeTypeId) + "Error", "Employee Type required");
                }
                if (String.IsNullOrEmpty(designation.DesignationName))
                {
                    _responseViewModel.Errors.Add(nameof(designation.DesignationName) + "Error", "Designation name required");
                }
                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                var existingDesignations = await _designationContract.GetDesignations();
                var existingDesignation = existingDesignations.Where(d => d.DesignationName == designation.DesignationName && d.EmployeeTypeId == designation.EmployeeTypeId).SingleOrDefault();
                if (existingDesignation == null)
                {
                    designation.CreatedBy = "";
                    designation.CreatedDate = DateTime.Now;
                    designation.IsActive = true;
                    designation.DesignationName = designation.DesignationName.Trim();
                    int result = await _designationContract.CreateDesignation(designation);
                    if (result>0)
                    {
                        _responseViewModel.ResponseMessage = "Record saved successfully";
                        _responseViewModel.StatusCode = 201;
                        return Ok(_responseViewModel);
                    }
                    else
                    {
                        _responseViewModel.ResponseMessage = "Could not insert data";
                        _responseViewModel.StatusCode = 500;
                        return Ok(_responseViewModel);
                    }


                }
                else
                {
                    _responseViewModel.ResponseMessage = "Record already exists";
                    _responseViewModel.Errors.Add(nameof(designation.DesignationName) + "Error", "Designation name already exists");
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
        public async Task<IActionResult> UpdateDesignation([FromForm] Designation designation)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (designation.EmployeeTypeId == 0)
                {
                    _responseViewModel.Errors.Add(nameof(designation.EmployeeTypeId) + "Error", "Employee Type required");
                }
                if (String.IsNullOrEmpty(designation.DesignationName))
                {
                    _responseViewModel.Errors.Add(nameof(designation.DesignationName) + "Error", "Designation name required");
                }
                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                var existingDesignations = await _designationContract.GetDesignations();
                var existingDesignation = existingDesignations.Where(d => d.Id == designation.Id).SingleOrDefault();

                existingDesignation.UpdatedBy = "";
                existingDesignation.UpdatedDate = DateTime.Now;
                existingDesignation.DesignationName = designation.DesignationName;
                existingDesignation.Description = designation.Description;
                existingDesignation.EmployeeTypeId = designation.EmployeeTypeId;
                existingDesignation.RoleId = designation.RoleId;
                existingDesignation.UserId = designation.UserId;
                existingDesignation.SchoolId = designation.SchoolId;
                existingDesignation.GuestPkId = designation.GuestPkId;
                existingDesignation.IsActive = designation.IsActive;

                int result = await _designationContract.UpdateDesignation(existingDesignation);

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
        public async Task<IActionResult> RemoveDesignation(int id)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                var existingDesignations = await _designationContract.GetDesignations();
                var existingDesignation = existingDesignations.Where(d => d.Id == id).SingleOrDefault();

                if (existingDesignation == null)
                {
                    _responseViewModel.ResponseMessage = "Record not found";
                    _responseViewModel.StatusCode = 404;
                    return Ok(_responseViewModel);
                }

                int result = await _designationContract.RemoveDesignation(existingDesignation.Id);

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

        // Secondary Designation
        [HttpGet]
        public async Task<IActionResult> GetSecondaryDesignations()
        {
            try
            {
                var designations = await _designationContract.GetSecondaryDesignations();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = designations;
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
        public async Task<IActionResult> GetSecondaryDesignationById(int id)
        {

            try
            {
                var designation = await _designationContract.GetSecondaryDesignationById(id);
                if (designation == null)
                {
                    _responseViewModel.StatusCode = 404;
                    _responseViewModel.ResponseMessage = "Data not found";
                    return Ok(_responseViewModel);
                }
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = designation;
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
        public async Task<IActionResult> CreateSecondaryDesignation([FromForm] Designation designation)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (designation.EmployeeTypeId == 0)
                {
                    _responseViewModel.Errors.Add(nameof(designation.EmployeeTypeId) + "Error", "Employee Type required");
                }
                if (String.IsNullOrEmpty(designation.DesignationName))
                {
                    _responseViewModel.Errors.Add(nameof(designation.DesignationName) + "Error", "Designation name required");
                }
                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                var existingDesignations = await _designationContract.GetSecondaryDesignations();
                var existingDesignation = existingDesignations.Where(d => d.DesignationName == designation.DesignationName && d.EmployeeTypeId == designation.EmployeeTypeId).SingleOrDefault();
                if (existingDesignation == null)
                {
                    designation.CreatedBy = "";
                    designation.CreatedDate = DateTime.Now;
                    designation.IsActive = true;
                    designation.DesignationName = designation.DesignationName.Trim();
                    int result = await _designationContract.CreateSecondaryDesignation(designation);
                    if (result > 0)
                    {
                        _responseViewModel.ResponseMessage = "Record saved successfully";
                        _responseViewModel.StatusCode = 201;
                        return Ok(_responseViewModel);
                    }
                    else
                    {
                        _responseViewModel.ResponseMessage = "Could not insert data";
                        _responseViewModel.StatusCode = 500;
                        return Ok(_responseViewModel);
                    }


                }
                else
                {
                    _responseViewModel.ResponseMessage = "Record already exists";
                    _responseViewModel.Errors.Add(nameof(designation.DesignationName) + "Error", "Designation name already exists");
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
        public async Task<IActionResult> UpdateSecondaryDesignation([FromForm] Designation designation)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (designation.EmployeeTypeId == 0)
                {
                    _responseViewModel.Errors.Add(nameof(designation.EmployeeTypeId) + "Error", "Employee Type required");
                }
                if (String.IsNullOrEmpty(designation.DesignationName))
                {
                    _responseViewModel.Errors.Add(nameof(designation.DesignationName) + "Error", "Designation name required");
                }
                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                var existingDesignations = await _designationContract.GetSecondaryDesignations();
                var existingDesignation = existingDesignations.Where(d => d.Id == designation.Id).SingleOrDefault();

                existingDesignation.UpdatedBy = "";
                existingDesignation.UpdatedDate = DateTime.Now;
                existingDesignation.DesignationName = designation.DesignationName;
                existingDesignation.Description = designation.Description;
                existingDesignation.EmployeeTypeId = designation.EmployeeTypeId;
                existingDesignation.RoleId = designation.RoleId;
                existingDesignation.UserId = designation.UserId;
                existingDesignation.SchoolId = designation.SchoolId;
                existingDesignation.GuestPkId = designation.GuestPkId;
                existingDesignation.IsActive = designation.IsActive;

                int result = await _designationContract.UpdateSecondaryDesignation(existingDesignation);

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
        public async Task<IActionResult> RemoveSecondaryDesignation(int id)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                var existingDesignations = await _designationContract.GetSecondaryDesignations();
                var existingDesignation = existingDesignations.Where(d => d.Id == id).SingleOrDefault();

                if (existingDesignation == null)
                {
                    _responseViewModel.ResponseMessage = "Record not found";
                    _responseViewModel.StatusCode = 404;
                    return Ok(_responseViewModel);
                }

                int result = await _designationContract.RemoveSecondaryDesignation(existingDesignation.Id);

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
