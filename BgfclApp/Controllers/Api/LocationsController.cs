using BgfclApp.ViewModels;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BgfclApp.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private ILocationContract _locationContract;
        private ResponseViewModel _responseViewModel;
        public LocationsController(ILocationContract locationContract, ResponseViewModel responseViewModel)
        {
            _locationContract = locationContract;
            _responseViewModel = responseViewModel;
        }


        [HttpGet]
        public async Task<IActionResult> GetLocations()
        {
            try
            {
                var locations = await _locationContract.GetLocations();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = locations;
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
        public async Task<IActionResult> GetLocationById(int id)
        {

            try
            {
                var location = await _locationContract.GetLocationById(id);
                if (location == null)
                {
                    _responseViewModel.StatusCode = 404;
                    _responseViewModel.ResponseMessage = "Data not found";
                    return Ok(_responseViewModel);
                }
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = location;
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
        public async Task<IActionResult> CreateLocation([FromForm] Location location)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (String.IsNullOrEmpty(location.LocationName))
                {
                    _responseViewModel.Errors.Add(nameof(location.LocationName) + "Error", "Location name required");
                }
                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                var existingLocations = await _locationContract.GetLocations();
                var existingLocation = existingLocations.Where(l => l.LocationName == location.LocationName).SingleOrDefault();
                if (existingLocation == null)
                {
                    location.CreatedBy = "";
                    location.CreatedDate = DateTime.Now;
                    location.IsActive = true;
                    int result = await _locationContract.CreateLocation(location);

                    _responseViewModel.ResponseMessage = "Record saved successfully";
                    _responseViewModel.StatusCode = 201;
                    return Ok(_responseViewModel);
                }
                else
                {
                    _responseViewModel.ResponseMessage = "Record already exists";
                    _responseViewModel.Errors.Add(nameof(location.LocationName) + "Error", "Location name already exists");
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
        public async Task<IActionResult> UpdateLocation([FromForm] Location location)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (String.IsNullOrEmpty(location.LocationName))
                {
                    _responseViewModel.Errors.Add(nameof(location.LocationName) + "Error", "Location name required");
                }
                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                var existingLocation = await _locationContract.GetLocationById(location.Id);

                existingLocation.UpdatedBy = "";
                existingLocation.UpdatedDate = DateTime.Now;
                existingLocation.LocationName = location.LocationName;
                existingLocation.IsActive = location.IsActive;
     

                int result = await _locationContract.UpdateLocation(existingLocation);

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
        public async Task<IActionResult> RemoveLocation(int id)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                var existingLocation = await _locationContract.GetLocationById(id);

                if (existingLocation == null)
                {
                    _responseViewModel.ResponseMessage = "Record not found";
                    _responseViewModel.StatusCode = 404;
                    return Ok(_responseViewModel);
                }

                int result = await _locationContract.RemoveLocation(existingLocation.Id);

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
