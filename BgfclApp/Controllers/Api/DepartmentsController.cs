using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BgfclApp.ViewModels;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace BgfclApp.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private IDepartmentContract _departmentContract;
        private ResponseViewModel _responseViewModel;
        public DepartmentsController(IDepartmentContract departmentContract, ResponseViewModel responseViewModel)
        {
            _departmentContract = departmentContract;
            _responseViewModel = responseViewModel;
        }


        [HttpGet]
        public async Task<IActionResult> GetDepartments()
        {
            try
            {
                var departments = await _departmentContract.GetDepartments();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = departments;
                return Ok(_responseViewModel);
            }
            catch (Exception ex)
            {
                _responseViewModel.StatusCode = 500;
                _responseViewModel.ResponseMessage = ex.Message;
                return Ok(_responseViewModel);
            }
        }

        //[HttpGet("{id}", Name = "DepartmentById")]
        [HttpGet]
        public async Task<IActionResult> GetDepartmentById(int id)
        {

            try
            {
                var department = await _departmentContract.GetDepartmentById(id);
                if (department == null)
                {
                    _responseViewModel.StatusCode = 404;
                    _responseViewModel.ResponseMessage = "Data not found";
                    return Ok(_responseViewModel);
                }
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = department;
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
        public async Task<IActionResult> CreateDepartment([FromForm] Department department)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (String.IsNullOrEmpty(department.DepartmentName))
                {
                    _responseViewModel.Errors.Add(nameof(department.DepartmentName)+"Error", "Department name required");
                }
                //if (String.IsNullOrEmpty(department.JournalCode))
                //{
                //    _responseViewModel.Errors.Add(nameof(department.JournalCode) + "Error", "Journal code required");
                //}
                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                var existingDepartments = await _departmentContract.GetDepartments();
                var existingDepartment = existingDepartments.Where(d => d.DepartmentName == department.DepartmentName).SingleOrDefault();
                if (existingDepartment == null)
                {
                    department.CreatedBy = "";
                    department.CreatedDate = DateTime.Now;
                    department.IsActive = true;
                    int result = await _departmentContract.CreateDepartment(department);
                    
                    _responseViewModel.ResponseMessage = "Record saved successfully";
                    _responseViewModel.StatusCode= 201;
                    return Ok(_responseViewModel);
                }
                else
                {
                    _responseViewModel.ResponseMessage = "Record already exists";
                    _responseViewModel.Errors.Add(nameof(department.DepartmentName) + "Error", "Department name already exists");
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
        public async Task<IActionResult> UpdateDepartment([FromForm] Department department)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (String.IsNullOrEmpty(department.DepartmentName))
                {
                    _responseViewModel.Errors.Add(nameof(department.DepartmentName) + "Error", "Department name required");
                }
                //if (String.IsNullOrEmpty(department.JournalCode))
                //{
                //    _responseViewModel.Errors.Add(nameof(department.JournalCode) + "Error", "Journal Code required");
                //}
                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                var existingDepartments = await _departmentContract.GetDepartments();
                var existingDepartment = existingDepartments.Where(d => d.Id == department.Id).SingleOrDefault();

                existingDepartment.UpdatedBy = "";
                existingDepartment.UpdatedDate = DateTime.Now;
                existingDepartment.UserId = department.UserId;
                existingDepartment.SchoolId = department.SchoolId;
                existingDepartment.RoleId = department.RoleId;
                existingDepartment.GuestPkId = department.GuestPkId;
                existingDepartment.DepartmentName = department.DepartmentName;
                existingDepartment.JournalCode = department.JournalCode;
                existingDepartment.Description = department.Description;
                existingDepartment.IsActive = department.IsActive;

                int result = await _departmentContract.UpdateDepartment(existingDepartment);

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
        public async Task<IActionResult> RemoveDepartment(int id)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                var existingDepartments = await _departmentContract.GetDepartments();
                var existingDepartment = existingDepartments.Where(d => d.Id == id).SingleOrDefault();

                if (existingDepartment == null)
                {
                    _responseViewModel.ResponseMessage = "Record not found";
                    _responseViewModel.StatusCode = 404;
                    return Ok(_responseViewModel);
                }

                int result = await _departmentContract.RemoveDepartment(existingDepartment.Id);

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
