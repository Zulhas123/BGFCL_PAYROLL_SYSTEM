using BgfclApp.ViewModels;
using Contracts;
using DocumentFormat.OpenXml.Bibliography;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BgfclApp.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReligionsController : ControllerBase
    {
        private IReligionContract _religionContract;
        private ResponseViewModel _responseViewModel;
        public ReligionsController(IReligionContract religionContract, ResponseViewModel responseViewModel)
        {
            _religionContract = religionContract;
            _responseViewModel = responseViewModel;
        }

        [HttpGet]
        public async Task<IActionResult> GetReligions()
        {
            try
            {
                var religion = await _religionContract.GetReligions();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = religion;
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
        public async Task<IActionResult> GetReligionById(int id)
        {

            try
            {
                var religion = await _religionContract.GetReligionById(id);
                if (religion == null)
                {
                    _responseViewModel.StatusCode = 404;
                    _responseViewModel.ResponseMessage = "Data not found";
                    return Ok(_responseViewModel);
                }
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = religion;
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
        public async Task<IActionResult> CreateReligion([FromForm] Religion religion)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (String.IsNullOrEmpty(religion.ReligionName))
                {
                    _responseViewModel.Errors.Add(nameof(religion.ReligionName) + "Error", "Religion name required");
                }
                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                var existingReligions = await _religionContract.GetReligions();
                var existingReligion = existingReligions.Where(d => d.ReligionName == religion.ReligionName).SingleOrDefault();
                if (existingReligion == null)
                {
                    religion.CreatedBy = "";
                    religion.CreatedDate = DateTime.Now;
                    religion.IsActive = true;
                    int result = await _religionContract.CreateReligion(religion);

                    _responseViewModel.ResponseMessage = "Record saved successfully";
                    _responseViewModel.StatusCode = 201;
                    return Ok(_responseViewModel);
                }
                else
                {
                    _responseViewModel.ResponseMessage = "Record already exists";
                    _responseViewModel.Errors.Add(nameof(religion.ReligionName) + "Error", "Religion name already exists");
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
        public async Task<IActionResult> UpdateReligion([FromForm] Religion religion)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (String.IsNullOrEmpty(religion.ReligionName))
                {
                    _responseViewModel.Errors.Add(nameof(religion.ReligionName) + "Error", "Religion name required");
                }

                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                var existingReligions = await _religionContract.GetReligions();
                var existingReligion = existingReligions.Where(d => d.Id == religion.Id).SingleOrDefault();

                existingReligion.UpdatedBy = "";
                existingReligion.UpdatedDate = DateTime.Now;
                existingReligion.SchoolId = religion.SchoolId;
                existingReligion.RoleId = religion.RoleId;
                existingReligion.GuestPkId = religion.GuestPkId;
                existingReligion.ReligionName = religion.ReligionName;
                existingReligion.Description = religion.Description;
                existingReligion.IsActive = religion.IsActive;

                int result = await _religionContract.UpdateReligion(existingReligion);

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
        public async Task<IActionResult> DeleteReligion(int id)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                var existingReligions = await _religionContract.GetReligions();
                var existingReligion = existingReligions.Where(d => d.Id == id).SingleOrDefault();

                if (existingReligion == null)
                {
                    _responseViewModel.ResponseMessage = "Record not found";
                    _responseViewModel.StatusCode = 404;
                    return Ok(_responseViewModel);
                }

                int result = await _religionContract.DeleteReligion(existingReligion.Id);

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

