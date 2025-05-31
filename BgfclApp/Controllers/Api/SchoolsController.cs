using BgfclApp.ViewModels;
using Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BgfclApp.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SchoolsController : ControllerBase
    {

        private IUserContract _userContract;
        private ISchoolContract _schoolContract;
        private ResponseViewModel _responseViewModel;
        public SchoolsController(IUserContract userContract, ISchoolContract schoolContract, ResponseViewModel responseViewModel)
        {
            _userContract = userContract;
            _schoolContract = schoolContract;
            _responseViewModel = responseViewModel;
        }

        [HttpGet]
        public async Task<IActionResult> GetSchools()
        {
            try
            {
                var schools = await _schoolContract.GetSchools();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = schools;
                return Ok(_responseViewModel);
            }
            catch (Exception ex)
            {
                _responseViewModel.StatusCode = 500;
                _responseViewModel.ResponseMessage = ex.Message;
                return Ok(_responseViewModel);
            }
        }
        [HttpGet]
        public async Task<IActionResult> GetSchoolById(int id)
        {

            try
            {
                var school = await _schoolContract.GetSchoolById(id);
                if (school == null)
                {
                    _responseViewModel.StatusCode = 404;
                    _responseViewModel.ResponseMessage = "Data not found";
                    return Ok(_responseViewModel);
                }

                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = school;
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
        public async Task<IActionResult> CreateSchool([FromBody] Entities.Schools school)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();

            try
            {
                // Check for required fields and validate them
                if (string.IsNullOrEmpty(school.Title))
                {
                    _responseViewModel.Errors.Add(nameof(school.Title) + "Error", "Title is required");
                }
                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }

                // Check if the Title  already exists in the database
                var existingSchools = await _schoolContract.GetSchools();
                var existingSchool = existingSchools.SingleOrDefault(u => u.Title.Equals(school.Title, StringComparison.OrdinalIgnoreCase));


                if (existingSchool == null)
                {
                    // Create the role in the database
                    int result = await _schoolContract.CreateSchool(school);

                    // Return success message
                    _responseViewModel.ResponseMessage = "School created successfully";
                    _responseViewModel.StatusCode = 201;
                    return Ok(_responseViewModel);
                }
                else
                {
                    // If user already exists, return conflict status
                    _responseViewModel.ResponseMessage = "School already exists";
                    _responseViewModel.Errors.Add(nameof(school.Title) + "Error", "Title already taken");
                    _responseViewModel.StatusCode = 409;
                    return Ok(_responseViewModel);
                }
            }
            catch (Exception ex)
            {
                // If an exception occurs, return the error message with a 500 status
                _responseViewModel.ResponseMessage = ex.Message;
                _responseViewModel.StatusCode = 500;
                return Ok(_responseViewModel);
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateSchool([FromBody] Entities.Schools school)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();

            try
            {
                // ✅ Validate user data
                if (string.IsNullOrEmpty(school.Title))
                    _responseViewModel.Errors.Add("Title", "Title is required");

                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return BadRequest(_responseViewModel);
                }

                var existingSchools = await _schoolContract.GetSchools();
                var existingschool = existingSchools.SingleOrDefault(d => d.Id == school.Id);
                if (existingschool == null)
                {
                    _responseViewModel.ResponseMessage = "Role not found";
                    _responseViewModel.StatusCode = 404;
                    return NotFound(_responseViewModel);
                }

                // ✅ Update user properties
                existingschool.UserId = school.UserId;
                existingschool.GuestPkId = school.GuestPkId;
                existingschool.Title = school.Title;
                existingschool.ShortId = school.ShortId;
                existingschool.Notes = school.Notes;
                existingschool.has_erp = school.has_erp;
                existingschool.has_payroll = school.has_payroll;
                existingschool.IsActive = school.IsActive;

                int result = await _schoolContract.UpdateSchool(existingschool);

                _responseViewModel.ResponseMessage = "Record updated successfully";
                _responseViewModel.StatusCode = 200;
                return Ok(_responseViewModel);
            }
            catch (Exception ex)
            {
                _responseViewModel.ResponseMessage = "Something went wrong: " + ex.Message;
                _responseViewModel.StatusCode = 500;
                return StatusCode(500, _responseViewModel);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteSchool(int id)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                var existingSchools = await _schoolContract.GetSchools();
                var existingSchool = existingSchools.Where(d => d.Id == id).SingleOrDefault();

                if (existingSchool == null)
                {
                    _responseViewModel.ResponseMessage = "Record not found";
                    _responseViewModel.StatusCode = 404;
                    return Ok(_responseViewModel);
                }

                int result = await _schoolContract.DeleteSchool(existingSchool.Id);

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
