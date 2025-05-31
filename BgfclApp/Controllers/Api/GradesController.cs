using BgfclApp.ViewModels;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;

namespace BgfclApp.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GradesController : ControllerBase
    {
        private IGradeContract _gradeContract;
        private ResponseViewModel _responseViewModel;
        public GradesController(IGradeContract gradeContract, ResponseViewModel responseViewModel)
        {
            _gradeContract = gradeContract;
            _responseViewModel = responseViewModel;
        }


        [HttpGet]
        public async Task<IActionResult> GetGradesByEmployeeType(int employeeTypeId)
        {
            try
            {
                var grades = await _gradeContract.GetGradesByEmployeeType(employeeTypeId);
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = grades;
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
        public async Task<IActionResult> GetGrades()
        {
            try
            {
                var grades = await _gradeContract.GetGrades();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = grades;
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
        public async Task<IActionResult> GetGradeById(int id)
        {

            try
            {
                var grade = await _gradeContract.GetGradeById(id);
                if (grade == null)
                {
                    _responseViewModel.StatusCode = 404;
                    _responseViewModel.ResponseMessage = "Data not found";
                    return Ok(_responseViewModel);
                }
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = grade;
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
        public async Task<IActionResult> CreateGrade([FromForm] Grade grade)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (String.IsNullOrEmpty(grade.GradeName))
                {
                    _responseViewModel.Errors.Add(nameof(grade.GradeName) + "Error", "Grade name required");
                }
                if (grade.EmployeeTypeId == 0)
                {
                    _responseViewModel.Errors.Add(nameof(grade.EmployeeTypeId) + "Error", "Employee Type required");
                }
                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                var existingGrades = await _gradeContract.GetGradesByEmployeeType(grade.EmployeeTypeId);
                var existingGrade = existingGrades.Where(g => g.GradeName == grade.GradeName).SingleOrDefault();
                if (existingGrade == null)
                {
                    grade.CreatedBy = "";
                    grade.CreatedDate = DateTime.Now;
                    grade.IsActive = true;
                    int result = await _gradeContract.CreateGrade(grade);

                    _responseViewModel.ResponseMessage = "Record saved successfully";
                    _responseViewModel.StatusCode = 201;
                    return Ok(_responseViewModel);
                }
                else
                {
                    _responseViewModel.ResponseMessage = "Record already exists";
                    _responseViewModel.Errors.Add(nameof(grade.GradeName) + "Error", "Grade name already exists");
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
        public async Task<IActionResult> UpdateGrade([FromForm] Grade grade)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (String.IsNullOrEmpty(grade.GradeName))
                {
                    _responseViewModel.Errors.Add(nameof(grade.GradeName) + "Error", "Grade name required");
                }
                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                var existingGrade = await _gradeContract.GetGradeById(grade.Id);

                existingGrade.UpdatedBy = "";
                existingGrade.UpdatedDate = DateTime.Now;
                existingGrade.GradeName = grade.GradeName;
                existingGrade.Description = grade.Description;
                existingGrade.EmployeeTypeId = grade.EmployeeTypeId;

                int result = await _gradeContract.UpdateGrade(existingGrade);

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
        public async Task<IActionResult> RemoveGrade(int id)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                var existingGrade = await _gradeContract.GetGradeById(id);

                if (existingGrade == null)
                {
                    _responseViewModel.ResponseMessage = "Record not found";
                    _responseViewModel.StatusCode = 404;
                    return Ok(_responseViewModel);
                }

                int result = await _gradeContract.RemoveGrade(existingGrade.Id);

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

        [HttpGet]
        public async Task<IActionResult> GetBasics()
        {
            try
            {
                var grades = await _gradeContract.GetBasics();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = grades;
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
        public async Task<IActionResult> GetBasicsByGradeId(int? gradeId)
        {
            try
            {
                var basics = await _gradeContract.GetBasics();
                basics = basics.Where(b => b.GradeId == gradeId).ToList();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = basics;
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
        public async Task<IActionResult> CreateBasic([FromForm] Basic basic)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (basic.BasicAmount<=0)
                {
                    _responseViewModel.Errors.Add(nameof(basic.BasicAmount) + "Error", "Basic amount required");
                }
                if (basic.GradeId == 0)
                {
                    _responseViewModel.Errors.Add(nameof(basic.GradeId) + "Error", "Pay Scale required");
                }
                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                var existingBasics = await _gradeContract.GetBasicsByGradeId(basic.GradeId);
                var existingBasic = existingBasics.Where(b=>b.BasicAmount == basic.BasicAmount).SingleOrDefault();
                if (existingBasic == null)
                {
                    basic.CreatedBy = "";
                    basic.CreatedDate = DateTime.Now;
                    int result = await _gradeContract.CreateBasic(basic);

                    _responseViewModel.ResponseMessage = "Record saved successfully";
                    _responseViewModel.StatusCode = 201;
                    return Ok(_responseViewModel);
                }
                else
                {
                    _responseViewModel.ResponseMessage = "Record already exists";
                    _responseViewModel.Errors.Add(nameof(basic.BasicAmount) + "Error", "Basic amount already exists");
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
        public async Task<IActionResult> UpdateBasic([FromForm] Basic basic)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (String.IsNullOrEmpty(basic.BasicAmount.ToString()))
                {
                    _responseViewModel.Errors.Add(nameof(basic.BasicAmount) + "Error", "Basic Amount name required");
                }
                if (String.IsNullOrEmpty(basic.GradeId.ToString()))
                {
                    _responseViewModel.Errors.Add(nameof(basic.GradeId) + "Error", "Grade name required");
                }
                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                var existingGrade = await _gradeContract.GetBasicById(basic.Id);

                //existingGrade.UpdatedBy = "";
                //existingGrade.UpdatedDate = DateTime.Now;
                existingGrade.BasicAmount= basic.BasicAmount;
                existingGrade.GradeId = basic.GradeId;
                int result = await _gradeContract.UpdateBasic(existingGrade);

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
        public async Task<IActionResult> RemoveBasic(int id)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                var existingGrade = await _gradeContract.GetBasicById(id);

                if (existingGrade == null)
                {
                    _responseViewModel.ResponseMessage = "Record not found";
                    _responseViewModel.StatusCode = 404;
                    return Ok(_responseViewModel);
                }

                int result = await _gradeContract.RemoveBasic(existingGrade.Id);

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
        [HttpGet]
        public async Task<IActionResult> GetBasicById(int id)
        {

            try
            {
                var basic = await _gradeContract.GetBasicById(id);
                if (basic == null)
                {
                    _responseViewModel.StatusCode = 404;
                    _responseViewModel.ResponseMessage = "Data not found";
                    return Ok(_responseViewModel);
                }
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = basic;
                return Ok(_responseViewModel);
            }
            catch (Exception ex)
            {
                _responseViewModel.StatusCode = 500;
                _responseViewModel.ResponseMessage = "Something went wrong";
                return Ok(_responseViewModel);
            }
        }
    }
}
