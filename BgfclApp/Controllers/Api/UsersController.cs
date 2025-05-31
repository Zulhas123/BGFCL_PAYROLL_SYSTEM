using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using BgfclApp.ViewModels;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace BgfclApp.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private static readonly string EncryptionKey = "MySecretKey12345"; // Must be 16, 24, or 32 bytes long
        private IUserContract _userContract;
        private ResponseViewModel _responseViewModel;
        private IEmployeeContract _employeeContract;
        public UsersController(IUserContract userContract,
            ResponseViewModel responseViewModel,
            IEmployeeContract employeeContract)
        {
            _userContract = userContract;
            _responseViewModel = responseViewModel;
            _employeeContract = employeeContract;
        }
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            try
            {
                var users = await _userContract.GetUser();
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
        public async Task<IActionResult> GetUserById(int id)
        {

            try
            {
                var user = await _userContract.GetUserById(id);
                if (user == null)
                {
                    _responseViewModel.StatusCode = 404;
                    _responseViewModel.ResponseMessage = "Data not found";
                    return Ok(_responseViewModel);
                }
                user.Password = DecryptPassword(user.Password);
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = user;
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
        public async Task<IActionResult> CreateUser([FromBody] Entities.User user)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();

            try
            {
                // Check for required fields and validate them
                if (string.IsNullOrEmpty(user.Username))
                {
                    _responseViewModel.Errors.Add(nameof(user.Username) + "Error", "Username is required");
                }

                if (string.IsNullOrEmpty(user.Password))
                {
                    _responseViewModel.Errors.Add(nameof(user.Password) + "Error", "Password is required");
                }

                if (string.IsNullOrEmpty(user.Email))
                {
                    _responseViewModel.Errors.Add(nameof(user.Email) + "Error", "Email is required");
                }

                // If there are validation errors, return them with a 400 status
                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }

                // Check if the username or email already exists in the database
                var existingUsers = await _userContract.GetUser();
                var existingUser = existingUsers.SingleOrDefault(u => u.Username == user.Username || u.Email == user.Email);

                if (existingUser == null)
                {
                    // Encrypt the password before saving (Optional) 1234567
                    user.Password = EncryptPassword(user.Password);

                    // Create the user in the database
                    int result = await _userContract.CreateUser(user);

                    // Return success message
                    _responseViewModel.ResponseMessage = "User created successfully";
                    _responseViewModel.StatusCode = 201;
                    return Ok(_responseViewModel);
                }
                else
                {
                    // If user already exists, return conflict status
                    _responseViewModel.ResponseMessage = "User already exists";
                    _responseViewModel.Errors.Add(nameof(user.Username) + "Error", "Username or Email already taken");
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
        public async Task<IActionResult> UpdateUser([FromBody] Entities.User user) 
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            
            try
            {
                // ✅ Validate user data
                if (string.IsNullOrEmpty(user.Username))
                    _responseViewModel.Errors.Add("Username", "Username is required");

                if (string.IsNullOrEmpty(user.Password))
                    _responseViewModel.Errors.Add("Password", "Password is required");

                if (string.IsNullOrEmpty(user.Email))
                    _responseViewModel.Errors.Add("Email", "Email is required");

                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return BadRequest(_responseViewModel); 
                }

                var existingUsers = await _userContract.GetUser();
                var existingUser = existingUsers.SingleOrDefault(d => d.UserId == user.UserId);
                user.Password = EncryptPassword(user.Password);
                if (existingUser == null)
                {
                    _responseViewModel.ResponseMessage = "User not found";
                    _responseViewModel.StatusCode = 404;
                    return NotFound(_responseViewModel);
                }

                // ✅ Update user properties
                existingUser.UuId = user.UuId;
                existingUser.GuestPkId = user.GuestPkId;
                existingUser.SchoolId = user.SchoolId;
                existingUser.Username = user.Username;
                existingUser.Password = user.Password;
                existingUser.Email = user.Email;
                existingUser.IsActive = user.IsActive;

                int result = await _userContract.UpdateUser(existingUser);

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
        public async Task<IActionResult> DeleteUser(int id)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                var existingUsers = await _userContract.GetUser();
                var existingUser = existingUsers.Where(d => d.UserId == id).SingleOrDefault();

                if (existingUser == null)
                {
                    _responseViewModel.ResponseMessage = "Record not found";
                    _responseViewModel.StatusCode = 404;
                    return Ok(_responseViewModel);
                }

                int result = await _userContract.DeleteUser(existingUser.UserId);

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
        // Sample password hashing function
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
        //public static string EncryptPassword(string password)
        //{
        //    var plaintext = System.Text.Encoding.UTF8.GetBytes(password);
        //    return System.Convert.ToBase64String(plaintext);
        //}
        //public static string DecryptPassword(string password)
        //{
        //    var baseEncode = System.Convert.FromBase64String(password);
        //    return System.Text.Encoding.UTF8.GetString(baseEncode);
        //}


        // Encrypt password using AES
        public static string EncryptPassword(string password)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(EncryptionKey); // 16-byte key (must be 16, 24, or 32 bytes long)
                aesAlg.IV = new byte[16]; // AES IV should be 16 bytes (zeroed for simplicity)

                using (var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV))
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
                    return Convert.ToBase64String(encryptedBytes); // Base64 encode to return as string
                }
            }
        }

        // Decrypt password using AES
        public static string DecryptPassword(string encryptedPassword)
        {
            try
            {
                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Encoding.UTF8.GetBytes(EncryptionKey);
                    aesAlg.IV = new byte[16]; // AES IV should be 16 bytes (zeroed for simplicity)

                    using (var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                    {
                        byte[] encryptedBytes = Convert.FromBase64String(encryptedPassword); // Convert from Base64
                        byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                        return Encoding.UTF8.GetString(decryptedBytes); // Return decrypted string
                    }
                }
            }
            catch
            {
                return "Decryption Failed"; // Handle errors
            }
        }

        // Attendence ====
        [HttpGet]
        public async Task<IActionResult> GetAttendanceByMonthId(int monthId)
        {
            try
            {
                var attendence = await _userContract.GetAttendanceByMonthId(monthId);
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = attendence;
                return Ok(_responseViewModel);
            }
            catch (Exception ex)
            {
                _responseViewModel.StatusCode = 500;
                _responseViewModel.ResponseMessage = ex.Message;
                return StatusCode(500, _responseViewModel);
            }
        }


        [HttpPost]
        public async Task<IActionResult> SaveAttendance([FromBody] List<AttendanceRecord> attendanceData)
        {
            if (attendanceData == null || !attendanceData.Any())
            {
                return BadRequest("No data received.");
            }

            try
            {
                await _userContract.SaveAttendance(attendanceData);
                return Ok(new { success = true, message = "Attendance saved successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> UpdateAttendance([FromBody] List<AttendanceRecord> attendanceData)
        {
            if (attendanceData == null || !attendanceData.Any())
            {
                return BadRequest("No data received.");
            }

            try
            {
                await _userContract.UpdateAttendance(attendanceData);
                return Ok(new { success = true, message = "Attendance saved successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAttendenceMaster([FromBody] AttendanceMaster attendence)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();

            try
            {
                // Validation
                //if (attendence.MonthId <= 0)
                //{
                //    _responseViewModel.Errors.Add(nameof(attendence.MonthId) + "Error", "Month is required");
                //}
               
                //if (attendence.StartDate == default || attendence.EndDate == default)
                //{
                //    _responseViewModel.Errors.Add("DateRangeError", "Start Date and End Date are required");
                //}

                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }

                // Check for existing attendance
                var existingRecords = await _userContract.GetAttendenceMaster();
                var existingRecord = existingRecords.SingleOrDefault(u =>
                    u.MonthId == attendence.MonthId);

                if (existingRecord == null)
                {
                    int result = await _userContract.CreateAttendenceMaster(attendence);

                    _responseViewModel.ResponseMessage = "Attendance record created successfully";
                    _responseViewModel.StatusCode = 201;
                }
                else
                {
                    _responseViewModel.ResponseMessage = "Attendance record already exists";
                    _responseViewModel.Errors.Add("DuplicateError", "Record for this month and year already exists");
                    _responseViewModel.StatusCode = 409;
                }

                return Ok(_responseViewModel);
            }
            catch (Exception ex)
            {
                _responseViewModel.ResponseMessage = ex.Message;
                _responseViewModel.StatusCode = 500;
                return Ok(_responseViewModel);
            }
        }



        [HttpPut]
        public async Task<IActionResult> UpdateAttendenceMaster([FromBody] AttendanceMaster attendence)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();

            try
            {
                // ✅ Validate input fields
                if (attendence.Id <= 0)
                    _responseViewModel.Errors.Add(nameof(attendence.Id), "Invalid ID");

                if (attendence.MonthId <= 0)
                    _responseViewModel.Errors.Add(nameof(attendence.MonthId), "Month is required");

                if (attendence.StartDate == default || attendence.EndDate == default)
                    _responseViewModel.Errors.Add("DateRange", "Start and End dates are required");

                //if (_responseViewModel.Errors.Count > 0)
                //{
                //    _responseViewModel.ResponseMessage = "Validation failed";
                //    _responseViewModel.StatusCode = 400;
                //    return BadRequest(_responseViewModel);
                //}

                // ✅ Check if the record exists
                var existingRecords = await _userContract.GetAttendenceMaster();
                var existingRecord = existingRecords.SingleOrDefault(x => x.Id == attendence.Id);

                if (existingRecord == null)
                {
                    _responseViewModel.ResponseMessage = "Attendance record not found";
                    _responseViewModel.StatusCode = 404;
                    return NotFound(_responseViewModel);
                }

                // ✅ Update properties
                existingRecord.MonthId = attendence.MonthId;
                existingRecord.StartDate = attendence.StartDate;
                existingRecord.EndDate = attendence.EndDate;

                // ✅ Save to DB
                int result = await _userContract.UpdateAttendenceMaster(existingRecord);

                _responseViewModel.ResponseMessage = "Attendance record updated successfully";
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
        public async Task<IActionResult> DeleteAttendenceMaster(int id)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();

            try
            {
                // ✅ Check if the record exists
                var existingRecords = await _userContract.GetAttendenceMaster();
                var existingRecord = existingRecords.SingleOrDefault(d => d.Id == id);

                if (existingRecord == null)
                {
                    _responseViewModel.ResponseMessage = "Attendance record not found";
                    _responseViewModel.StatusCode = 404;
                    return NotFound(_responseViewModel);
                }

                // ✅ Call delete method
                int result = await _userContract.DeleteAttendenceMaster(id);

                _responseViewModel.ResponseMessage = "Attendance record deleted successfully";
                _responseViewModel.StatusCode = 200;
                return Ok(_responseViewModel);
            }
            catch (Exception ex)
            {
                _responseViewModel.ResponseMessage = "Error: " + ex.Message;
                _responseViewModel.StatusCode = 500;
                return StatusCode(500, _responseViewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAttendenceMaster()
        {
            try
            {
                var attendence = await _userContract.GetAttendenceMaster();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = attendence;
                return Ok(_responseViewModel);
            }
            catch (Exception ex)
            {
                _responseViewModel.StatusCode = 500;
                _responseViewModel.ResponseMessage = ex.Message;
                return StatusCode(500, _responseViewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAttenceMasterById(int id)
        {
            try
            {
                var attendence = await _userContract.GetAttenceMasterById(id);
                if (attendence == null)
                {
                    _responseViewModel.StatusCode = 404;
                    _responseViewModel.ResponseMessage = "Record not found";
                    return NotFound(_responseViewModel);
                }

                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = attendence;
                return Ok(_responseViewModel);
            }
            catch (Exception ex)
            {
                _responseViewModel.StatusCode = 500;
                _responseViewModel.ResponseMessage = $"Something went wrong: {ex.Message}";
                return StatusCode(500, _responseViewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAttendanceByMonthAndMaster(int monthId, int attendenceMasterId)
        {
            try
            {
                var attendence = await _userContract.GetAttendanceByMonthAndMaster(monthId, attendenceMasterId);

                if (attendence == null)
                {
                    _responseViewModel.StatusCode = 404;
                    _responseViewModel.ResponseMessage = "No attendance records found.";
                    return NotFound(_responseViewModel);
                }

                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = attendence;
                return Ok(_responseViewModel);
            }
            catch (Exception ex)
            {
                _responseViewModel.StatusCode = 500;
                _responseViewModel.ResponseMessage = $"Something went wrong: {ex.Message}";
                return StatusCode(500, _responseViewModel);
            }
        }


    }
}
