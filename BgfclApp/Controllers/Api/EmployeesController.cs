using BgfclApp.ViewModels;
using Contracts;
using DocumentFormat.OpenXml.Bibliography;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace BgfclApp.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private IEmployeeTypeContract _employeeTypeContract;
        private IEmployeeContract _employeeContract;
        private ResponseViewModel _responseViewModel;
        public EmployeesController(IEmployeeTypeContract employeeTypeContract, IEmployeeContract employeeContract, ResponseViewModel responseViewModel)
        {
            _employeeTypeContract = employeeTypeContract;
            _employeeContract = employeeContract;
            _responseViewModel = responseViewModel;
        }
        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromForm] Employee employee)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (String.IsNullOrEmpty(employee.EmployeeName))
                {
                    _responseViewModel.Errors.Add(nameof(employee.EmployeeName) + "Error", "Employee name required");
                }
                if (String.IsNullOrEmpty(employee.JobCode))
                {
                    _responseViewModel.Errors.Add(nameof(employee.JobCode) + "Error", "Jobcode required");
                }
                if (employee.ReligionId == 0)
                {
                    _responseViewModel.Errors.Add(nameof(employee.ReligionId) + "Error", "Religion required");
                }
                if (employee.EmployeeTypeId == 0)
                {
                    _responseViewModel.Errors.Add(nameof(employee.EmployeeTypeId) + "Error", "Employee Type required");
                }

                if(employee.EmployeeTypeId != 3)
                {
                    if (employee.DepartmentId == 0)
                    {
                        _responseViewModel.Errors.Add(nameof(employee.DepartmentId) + "Error", "Department required");
                    }
                    if (String.IsNullOrEmpty(employee.JoiningDate))
                    {
                        _responseViewModel.Errors.Add(nameof(employee.JoiningDate) + "Error", "Joining Date required");
                    }
                    if (String.IsNullOrEmpty(employee.DateOfBirth))
                    {
                        _responseViewModel.Errors.Add(nameof(employee.DateOfBirth) + "Error", "Date of Birth required");
                    }
                    if (employee.DesignationId == 0)
                    {
                        _responseViewModel.Errors.Add(nameof(employee.DesignationId) + "Error", "Designation required");
                    }
                }
                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                var existingEmployees = await _employeeContract.GetEmployees(1, employee.EmployeeTypeId);
                var existingEmployee = existingEmployees.Where(e => e.JobCode.ToLower() == employee.JobCode.ToLower()).SingleOrDefault();
                if (existingEmployee == null)
                {
                    employee.CreatedBy = "";
                    employee.CreatedDate = DateTime.Now;
                    int result = await _employeeContract.CreateEmployee(employee);

                    _responseViewModel.ResponseMessage = "Record saved successfully";
                    _responseViewModel.StatusCode = 201;
                    return Ok(_responseViewModel);
                }
                else
                {
                    _responseViewModel.ResponseMessage = "Record already exists";
                    _responseViewModel.Errors.Add(nameof(employee.JobCode) + "Error", "Jobcode already exists");
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

        [HttpPost]
        public async Task<IActionResult> CreateEmployees([FromBody] List<Employee> employees)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            _responseViewModel.ErrorsList = new List<Dictionary<string, string>>();
            var validEmployees = new List<Employee>();
            var employeeTypeId = employees.FirstOrDefault()?.EmployeeTypeId ?? 0;

            try
            {
                // Step 1: Fetch all existing JobCodes once for this employee type
                var existingEmployees = await _employeeContract.GetAllEmployeesByType(employeeTypeId);
                var existingJobCodes = new HashSet<string>(existingEmployees
                    .Where(e => !string.IsNullOrEmpty(e.JobCode))
                    .Select(e => e.JobCode.ToLower()));

                // Step 2: Count job codes from request to detect duplicates within the request
                var jobCodeCounts = employees
                    .Where(e => !string.IsNullOrEmpty(e.JobCode))
                    .GroupBy(e => e.JobCode.ToLower())
                    .ToDictionary(g => g.Key, g => g.Count());

                // Step 3: Validate each employee (one-time per employee)
                for (int i = 0; i < employees.Count; i++)
                {
                    var employee = employees[i];
                    var errors = new Dictionary<string, string>();

                    if (string.IsNullOrEmpty(employee.EmployeeName))
                        errors.Add(nameof(employee.EmployeeName) + "Error", "Employee name required");

                    if (string.IsNullOrEmpty(employee.JobCode))
                        errors.Add(nameof(employee.JobCode) + "Error", "Jobcode required");

                    if (string.IsNullOrEmpty(employee.DateOfBirth))
                        errors.Add(nameof(employee.DateOfBirth) + "Error", "Date of Birth required");

                    if (employee.ReligionId == 0)
                        errors.Add(nameof(employee.ReligionId) + "Error", "Religion required");

                    if (employee.EmployeeTypeId == 0)
                        errors.Add(nameof(employee.EmployeeTypeId) + "Error", "Employee Type required");

                    if (employee.DepartmentId == 0)
                        errors.Add(nameof(employee.DepartmentId) + "Error", "Department required");

                    if (employee.DesignationId == 0)
                        errors.Add(nameof(employee.DesignationId) + "Error", "Designation required");

                    if (string.IsNullOrEmpty(employee.JoiningDate))
                        errors.Add(nameof(employee.JoiningDate) + "Error", "Joining Date required");

                    // JobCode duplication checks (only once per employee)
                    if (!string.IsNullOrEmpty(employee.JobCode))
                    {
                        var jobCodeLower = employee.JobCode.ToLower();

                        if (existingJobCodes.Contains(jobCodeLower))
                        {
                            errors.Add($"JobCode[{i}]Error", $"Jobcode '{employee.JobCode}' already exists at index {i}");
                        }
                        else if (jobCodeCounts.ContainsKey(jobCodeLower) && jobCodeCounts[jobCodeLower] > 1)
                        {
                            errors.Add($"JobCode[{i}]Error", $"Jobcode '{employee.JobCode}' is duplicated in the request at index {i}");
                        }
                    }

                    if (errors.Any())
                    {
                        _responseViewModel.ErrorsList.Add(errors);
                    }
                    else
                    {
                        employee.CreatedBy = ""; // Optionally set current user
                        employee.CreatedDate = DateTime.Now;
                        validEmployees.Add(employee);
                    }
                }

                // Step 4: Insert valid employees
                if (validEmployees.Any())
                {
                    await _employeeContract.CreateEmployees(validEmployees);
                }

                // Step 5: Prepare response
                if (_responseViewModel.ErrorsList.Any())
                {
                    int failedCount = _responseViewModel.ErrorsList.Count;
                    int savedCount = validEmployees.Count;

                    _responseViewModel.StatusCode = 207; // Multi-Status
                    _responseViewModel.ResponseMessage = $"{savedCount} record(s) saved successfully. Some records failed validation.";
                }
                else
                {
                    _responseViewModel.StatusCode = 201;
                    _responseViewModel.ResponseMessage = "All records saved successfully";
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
        [HttpGet]
        public async Task<IActionResult> GetEmployees(int employeeTypeId)
        {
            try
            {
                var employees = await _employeeContract.GetEmployees(1, employeeTypeId);
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = employees;
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
        public async Task<IActionResult> GetEmployeesWithFilter(int employeeTypeId, int school_filter, int role_filter, int department_filter, int designation_filter)
        {
            try
            {
                var employees = await _employeeContract.GetEmployeeswithFilter(1,
                    employeeTypeId,
                    school_filter > 0 ? school_filter : (int?)null,
                    role_filter > 0 ? role_filter : (int?)null,
                    department_filter > 0 ? department_filter : (int?)null,
                    designation_filter > 0 ? designation_filter : (int?)null
                );

                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = employees;
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
        public async Task<IActionResult> GetAllEmployees()
        {
            try
            {
                var employees = await _employeeContract.GetAllEmployees();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = employees;
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
        public async Task<IActionResult> GetAllOfficer()
        {
            try
            {
                var employees = await _employeeContract.GetAllOfficer();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = employees;
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
        public async Task<IActionResult> GetLastPermanetEmployee()
        {
            try
            {
                var employees = await _employeeContract.GetLastPermanetEmployee();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = employees;
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
        public async Task<IActionResult> GetLastContractEmployee()
        {
            try
            {
                var employees = await _employeeContract.GetLastContractEmployee();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = employees;
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
        public async Task<IActionResult> GetAllJuniorStaff()
        {
            try
            {
                var employees = await _employeeContract.GetAllJuniorStaff();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = employees;
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
        public async Task<IActionResult> GetAllPensionOFJS()
        {
            try
            {
                var employees = await _employeeContract.GetAllPensionOFJS();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = employees;
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
        public async Task<IActionResult> GetInactiveEmployees(int employeeTypeId)
        {
            try
            {
                var employees = await _employeeContract.GetInactiveEmployees(employeeTypeId);
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = employees;
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
        public async Task<IActionResult> GetEmployeesByEmployeeType(int employeeTypeId)
        {
            try
            {
                var employees = await _employeeContract.GetEmployeesByEmployeeType(employeeTypeId);
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = employees;
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
        public async Task<IActionResult> GetEmployeeById(int id)
        {

            try
            {
                var employee = await _employeeContract.GetEmployeeById(id);
                employee.DateOfBirth = DateTime.ParseExact(employee.DateOfBirth.Split(' ')[0].ToString(), "MM/dd/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
                employee.JoiningDate = DateTime.ParseExact(employee.JoiningDate.Split(' ')[0].ToString(), "MM/dd/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
                if (employee == null)
                {
                    _responseViewModel.StatusCode = 404;
                    _responseViewModel.ResponseMessage = "Data not found";
                    return Ok(_responseViewModel);
                }
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = employee;
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
        public async Task<IActionResult> GetEmployeeViewById(int id)
        {

            try
            {
                var employee = await _employeeContract.GetEmployeeViewById(id);
                if (employee == null)
                {
                    _responseViewModel.StatusCode = 404;
                    _responseViewModel.ResponseMessage = "Data not found";
                    return Ok(_responseViewModel);
                }
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = employee;
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
        public async Task<IActionResult> GetEmployeeViewByJobCode(string jobCode)
        {

            try
            {
                var employee = await _employeeContract.GetEmployeeViewByJobCode(jobCode);
                if (employee == null)
                {
                    _responseViewModel.StatusCode = 404;
                    _responseViewModel.ResponseMessage = "Data not found";
                    return Ok(_responseViewModel);
                }
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = employee;
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
        public async Task<IActionResult> UpdateEmployee([FromForm] Employee employee)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (String.IsNullOrEmpty(employee.EmployeeName))
                {
                    _responseViewModel.Errors.Add(nameof(employee.EmployeeName) + "Error", "Employee name required");
                }
                if (String.IsNullOrEmpty(employee.JobCode))
                {
                    _responseViewModel.Errors.Add(nameof(employee.JobCode) + "Error", "Jobcode required");
                }
                if (String.IsNullOrEmpty(employee.DateOfBirth))
                {
                    _responseViewModel.Errors.Add(nameof(employee.DateOfBirth) + "Error", "Date of Birth required");
                }
                //if (employee.GenderId == 0)
                //{
                //    _responseViewModel.Errors.Add(nameof(employee.GenderId) + "Error", "Gender required");
                //}
                //if (employee.ReligionId == 0)
                //{
                //    _responseViewModel.Errors.Add(nameof(employee.ReligionId) + "Error", "Religion required");
                //}
                if (employee.EmployeeTypeId == 0)
                {
                    _responseViewModel.Errors.Add(nameof(employee.EmployeeTypeId) + "Error", "Employee Type required");
                }
                //if (employee.GradeId == 0)
                //{
                //    _responseViewModel.Errors.Add(nameof(employee.GradeId) + "Error", "Grade required");
                //}
                if (employee.DepartmentId == 0)
                {
                    _responseViewModel.Errors.Add(nameof(employee.DepartmentId) + "Error", "Department required");
                }
                if (employee.DesignationId == 0)
                {
                    _responseViewModel.Errors.Add(nameof(employee.DesignationId) + "Error", "Designation required");
                }
                //if (employee.LocationId == 0)
                //{
                //    _responseViewModel.Errors.Add(nameof(employee.LocationId) + "Error", "Location required");
                //}
                //if (String.IsNullOrEmpty(employee.JoiningDate))
                //{
                //    _responseViewModel.Errors.Add(nameof(employee.JoiningDate) + "Error", "Joining Date required");
                //}
                if (String.IsNullOrEmpty(employee.JoiningDate))
                {
                    _responseViewModel.Errors.Add(nameof(employee.JoiningDate) + "Error", "Joining Date required");
                }
                //if (String.IsNullOrEmpty(employee.JournalCode))
                //{
                //    _responseViewModel.Errors.Add(nameof(employee.JournalCode) + "Error", "Journal Code required");
                //}
                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                var existingEmployee = await _employeeContract.GetEmployeeById(employee.Id);
                if (existingEmployee==null)
                {
                    _responseViewModel.ResponseMessage = "Not found";
                    _responseViewModel.StatusCode = 404;
                    return Ok(_responseViewModel);
                }

                employee.UpdatedBy = "";
                employee.UpdatedDate = DateTime.Now;

                int result = await _employeeContract.UpdateEmployee(employee);

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

        [HttpPut]
        public async Task<IActionResult> UpdateInactiveEmployee([FromForm] InactiveEmployeeOf employee)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (String.IsNullOrEmpty(employee.EmployeeName))
                {
                    _responseViewModel.Errors.Add(nameof(employee.EmployeeName) + "Error", "Employee name required");
                }
                if (String.IsNullOrEmpty(employee.JobCode))
                {
                    _responseViewModel.Errors.Add(nameof(employee.JobCode) + "Error", "Jobcode required");
                }

                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                var existingEmployee = await _employeeContract.GetEmployeeById(employee.Id);
                if (existingEmployee == null)
                {
                    _responseViewModel.ResponseMessage = "Not found";
                    _responseViewModel.StatusCode = 404;
                    return Ok(_responseViewModel);
                }

                employee.UpdatedBy = "";
                employee.UpdatedDate = DateTime.Now;

                int result = await _employeeContract.UpdateInactiveEmployee(employee);

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

        [HttpGet]
        public async Task<IActionResult> GetEmployeeForView(int id)
        {

            try
            {
                var employee = await _employeeContract.GetEmployeeForView(id);
                employee.DateOfBirth = DateTime.ParseExact(employee.DateOfBirth.Split(' ')[0].ToString(), "MM/dd/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
                employee.JoiningDate = DateTime.ParseExact(employee.JoiningDate.Split(' ')[0].ToString(), "MM/dd/yyyy", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");

                if (employee == null)
                {
                    _responseViewModel.StatusCode = 404;
                    _responseViewModel.ResponseMessage = "Data not found";
                    return Ok(_responseViewModel);
                }
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = employee;
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
        public async Task<IActionResult> GetEmployeeTypeById(int id)
        {

            try
            {
                var employeeType = await _employeeTypeContract.GetEmployeeTypeId(id);
                if (employeeType == null)
                {
                    _responseViewModel.StatusCode = 404;
                    _responseViewModel.ResponseMessage = "Data not found";
                    return Ok(_responseViewModel);
                }
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = employeeType;
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
        public async Task<IActionResult> GetEmployeeTypes()
        {
            try
            {
                var employeeTypes = await _employeeTypeContract.GetEmployeeTypes();
                _responseViewModel.StatusCode = 200;
                _responseViewModel.Data = employeeTypes;
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
        public async Task<IActionResult> CreateEmployeeType([FromForm] EmployeeType employeeType)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                if (String.IsNullOrEmpty(employeeType.EmployeeTypeName))
                {
                    _responseViewModel.Errors.Add(nameof(employeeType.EmployeeTypeName) + "Error", "Employee Type name required");
                }
                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }
                var existingTypes = await _employeeTypeContract.GetEmployeeTypes();
                var existingType = existingTypes.Where(d => d.EmployeeTypeName == employeeType.EmployeeTypeName).SingleOrDefault();
                if (existingType == null)
                {
                    existingType = new EmployeeType();
                    existingType.CreatedBy = null;
                    existingType.CreatedDate = DateTime.Now;
                    existingType.EmployeeTypeName = employeeType.EmployeeTypeName;
                    existingType.Description = employeeType.Description;
                    existingType.IsActive = true;

                    int result = await _employeeTypeContract.CreateEmployeeType(existingType);

                    _responseViewModel.ResponseMessage = "Record saved successfully";
                    _responseViewModel.StatusCode = 201;
                    return Ok(_responseViewModel);
                }
                else
                {
                    _responseViewModel.ResponseMessage = "Record already exists";
                    _responseViewModel.Errors.Add(nameof(existingType.EmployeeTypeName) + "Error", "Employee Type name already exists");
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
        public async Task<IActionResult> UpdateEmployeeType([FromForm] EmployeeType employeeType)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();

            try
            {
                // Validation: Check if EmployeeTypeName is empty
                if (string.IsNullOrEmpty(employeeType.EmployeeTypeName))
                {
                    _responseViewModel.Errors.Add(nameof(employeeType.EmployeeTypeName) + "Error", "Employee Type name is required");
                }

                // If there are validation errors, return response
                if (_responseViewModel.Errors.Count > 0)
                {
                    _responseViewModel.ResponseMessage = "Validation failed";
                    _responseViewModel.StatusCode = 400;
                    return Ok(_responseViewModel);
                }

                // Retrieve all employee types
                var existingTypes = await _employeeTypeContract.GetEmployeeTypes();
                var existingType = existingTypes.Where(d => d.Id == employeeType.Id).SingleOrDefault();

                if (existingType != null)
                {
                    // Updating existing employee type
                    existingType.UpdatedBy = "Admin"; // Set valid updatedBy value
                    existingType.UpdatedDate = DateTime.Now;
                    existingType.SchoolId = employeeType.SchoolId;
                    existingType.RoleId = employeeType.RoleId;
                    existingType.GuestPkId = employeeType.GuestPkId;
                    existingType.EmployeeTypeName = employeeType.EmployeeTypeName;
                    existingType.Description = employeeType.Description;
                    existingType.IsActive = employeeType.IsActive;

                    int result = await _employeeTypeContract.UpdateEmployeeType(existingType);

                    _responseViewModel.ResponseMessage = "Record updated successfully";
                    _responseViewModel.StatusCode = 200;
                    return Ok(_responseViewModel);
                }
                else
                {
                    // If no record is found, return not found response
                    _responseViewModel.ResponseMessage = "Employee Type not found";
                    _responseViewModel.StatusCode = 404;
                    return Ok(_responseViewModel);
                }
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                Console.WriteLine($"Error in UpdateEmployeeType: {ex.Message}");

                _responseViewModel.ResponseMessage = "Something went wrong";
                _responseViewModel.StatusCode = 500;
                return Ok(_responseViewModel);
            }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                var existingEmployees = await _employeeContract.GetAllEmployees();
                var existingEmployee= existingEmployees.Where(d => d.Id == id).SingleOrDefault();

                if (existingEmployee == null)
                {
                    _responseViewModel.ResponseMessage = "Record not found";
                    _responseViewModel.StatusCode = 404;
                    return Ok(_responseViewModel);
                }

                int result = await _employeeContract.DeleteEmployee(existingEmployee.Id);

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

        [HttpDelete]
        public async Task<IActionResult> DeleteEmployeeType(int id)
        {
            _responseViewModel.Errors = new Dictionary<string, string>();
            try
            {
                var existingTypes = await _employeeTypeContract.GetEmployeeTypes();
                var existingType = existingTypes.Where(d => d.Id == id).SingleOrDefault();

                if (existingType == null)
                {
                    _responseViewModel.ResponseMessage = "Record not found";
                    _responseViewModel.StatusCode = 404;
                    return Ok(_responseViewModel);
                }

                int result = await _employeeTypeContract.DeleteEmployeeType(existingType.Id);

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
