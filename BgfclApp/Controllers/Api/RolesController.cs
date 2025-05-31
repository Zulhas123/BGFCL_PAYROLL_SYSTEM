using BgfclApp.ViewModels;
using Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BgfclApp.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private IUserContract _userContract;
        private IRolesContract _rolesContract;
        private ResponseViewModel _responseViewModel;
        public RolesController(IUserContract userContract, IRolesContract rolesContracy, ResponseViewModel responseViewModel)
        {
            _userContract = userContract;
            _rolesContract = rolesContracy;
            _responseViewModel = responseViewModel;
        }
        [HttpGet]
        public async Task<IActionResult> GetRole()
        {
            try
            {
                var users = await _rolesContract.GetRole();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = users;
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
        public async Task<IActionResult> GetRoleById(int id)
        {

            try
            {
                var role = await _rolesContract.GetRoleById(id);
                if (role == null)
                {
                    _responseViewModel.StatusCode = 404;
                    _responseViewModel.ResponseMessage = "Data not found";
                    return Ok(_responseViewModel);
                }

                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = role;
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
        public async Task<IActionResult> CreateRole([FromBody] Entities.Roles role)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();

            try
            {
                // Check for required fields and validate them
                if (string.IsNullOrEmpty(role.Title))
                {
                    _responseViewModel.Errors.Add(nameof(role.Title) + "Error", "Title is required");
                }

                if (string.IsNullOrEmpty(role.Slug))
                {
                    _responseViewModel.Errors.Add(nameof(role.Slug) + "Error", "Slug is required");
                }

               

                // If there are validation errors, return them with a 400 status
                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }

                // Check if the Title  already exists in the database
                var existingRoles = await _rolesContract.GetRole();
                var existingRole = existingRoles.SingleOrDefault(u => u.Title == role.Title);

                if (existingRole == null)
                {
                    // Create the role in the database
                    int result = await _rolesContract.CreateRole(role);

                    // Return success message
                    _responseViewModel.ResponseMessage = "Role created successfully";
                    _responseViewModel.StatusCode = 201;
                    return Ok(_responseViewModel);
                }
                else
                {
                    // If user already exists, return conflict status
                    _responseViewModel.ResponseMessage = "Role already exists";
                    _responseViewModel.Errors.Add(nameof(role.Title) + "Error", "Title already taken");
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
        public async Task<IActionResult> UpdateRole([FromBody] Entities.Roles role)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();

            try
            {
                // ✅ Validate user data
                if (string.IsNullOrEmpty(role.Title))
                    _responseViewModel.Errors.Add("Title", "Title is required");

                if (string.IsNullOrEmpty(role.Slug))
                    _responseViewModel.Errors.Add("Slug", "Slug is required");


                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return BadRequest(_responseViewModel);
                }

                var existingRoles = await _rolesContract.GetRole();
                var existingRole = existingRoles.SingleOrDefault(d => d.Id == role.Id);
                if (existingRole == null)
                {
                    _responseViewModel.ResponseMessage = "Role not found";
                    _responseViewModel.StatusCode = 404;
                    return NotFound(_responseViewModel);
                }

                // ✅ Update user properties
                existingRole.Id = role.Id;
                existingRole.UserId = role.UserId;
                existingRole.GuestPkId = role.GuestPkId;
                existingRole.SchoolId = role.SchoolId;
                existingRole.Title = role.Title;
                existingRole.Slug = role.Slug;
                existingRole.Notes = role.Notes;
                existingRole.IsEmployee = role.IsEmployee;
                existingRole.IsAuthority = role.IsAuthority;
                existingRole.IsStaff = role.IsStaff;
                existingRole.IsActive = role.IsActive;

                int result = await _rolesContract.UpdateRole(existingRole);

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
        public async Task<IActionResult> DeleteRole(int id)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                var existingRoles = await _rolesContract.GetRole();
                var existingRole = existingRoles.Where(d => d.Id == id).SingleOrDefault();

                if (existingRole == null)
                {
                    _responseViewModel.ResponseMessage = "Record not found";
                    _responseViewModel.StatusCode = 404;
                    return Ok(_responseViewModel);
                }

                int result = await _rolesContract.DeleteRole(existingRole.Id);

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
