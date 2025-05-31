using AspNetCore.Reporting;
using AspNetCore.Reporting.ReportExecutionService;
using BgfclApp.ViewModels;
using Contracts;
using Entities;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp;
using System.Globalization;
using Microsoft.Reporting.NETCore;
using System.Drawing.Imaging;
using System;
using System.Reflection.Emit;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Entities.ViewModels;
using Microsoft.Extensions.Options;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Data;
using BgfclApp.Service;
using static BgfclApp.Service.DataTableHelper;

namespace BgfclApp.Controllers.Report
{
    public class SalaryReportController : Controller
    {

        private readonly IWebHostEnvironment _webHostEnvironment;
        private IDepartmentContract _departmentContract;
        private IEmployeeTypeContract _employeeTypeContract;
        private ISalaryReportOfficerContract _salaryReportOfficerContract;
        private IBankContract _bankContract;
        private IEmployeeContract _employeeContract;
        private ISalarySettingContract _salarySettingContract;
        private IUserContract _userContract;
        public SalaryReportController(IWebHostEnvironment webHostEnvironment, 
                                     IDepartmentContract departmentContract, 
                                     ISalaryReportOfficerContract salaryReportOfficerContract,
                                     IEmployeeTypeContract employeeTypeContract,
                                     IBankContract bankContract,
                                     IEmployeeContract employeeContract,
                                     ISalarySettingContract salarySettingContract,
                                     IUserContract userContract
                                   )
        {

            _webHostEnvironment = webHostEnvironment;
            _departmentContract = departmentContract;
            _salaryReportOfficerContract = salaryReportOfficerContract;
            _employeeTypeContract = employeeTypeContract;
            _bankContract = bankContract;
            _employeeContract = employeeContract;
            _salarySettingContract = salarySettingContract;
            _userContract = userContract;
        }
        public IActionResult Index()
        {
            return View();
        }
        
        public class AttendanceRaw
        {
            public string JobCode { get; set; }
            public string EmployeeName { get; set; }
            public string DesignationName { get; set; }
            public DateTime AttendanceDate { get; set; }
            public bool IsPresent { get; set; } // e.g., "P", "A", etc.
        }
        public class AttendancePivotRow
        {
            public string JobCode { get; set; }
            public string EmployeeName { get; set; }
            public string? Day1 { get; set; }
            public string? Day2 { get; set; }
            public string? Day3 { get; set; }
            public string? Day4 { get; set; }
            public string? Day5 { get; set; }
            public string? Day6 { get; set; }
            public string? Day7 { get; set; }
            public string? Day8 { get; set; }
            public string? Day9 { get; set; }
            public string? Day10 { get; set; }
            public string? Day11 { get; set; }
            public string? Day12 { get; set; }
            public string? Day13 { get; set; }
            public string? Day14 { get; set; }
            public string? Day15 { get; set; }
            public string? Day16 { get; set; }
            public string? Day17 { get; set; }
            public string? Day18 { get; set; }
            public string? Day19 { get; set; }
            public string? Day20 { get; set; }
            public string? Day21 { get; set; }
            public string? Day22 { get; set; }
            public string? Day23 { get; set; }
            public string? Day24 { get; set; }
            public string? Day25 { get; set; }
            public string? Day26 { get; set; }
            public string? Day27 { get; set; }
            public string? Day28 { get; set; }
            public string? Day29 { get; set; }
            public string? Day30 { get; set; }
            public string? Day31 { get; set; }
            public string? TotalDays { get; set; }
            public string? AttendanceDate { get; set; }
            public Dictionary<string, string> DayStatus { get; set; } = new();
        }

        [HttpGet]
        public async Task<IActionResult> MonthlyAtteendance()
        {

            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> MonthlyAtteendance(List<string> jobCodes,int month, int year, string isExcel)
        {
            int monthId = year * 100 + month;
            var monthlyAttendance = await _userContract.GetAttendanceByMonthId(monthId);
            if (monthlyAttendance == null || !monthlyAttendance.Any())
                return NotFound("No attendance records found for the selected month.");

            // Step 1: Get all distinct dates to form dynamic columns
            var allDates = monthlyAttendance
                .Select(x => x.AttendanceDate.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToList();
            string firstDate = allDates.First().ToString("dd.MM.yyyy");
            string lastDate = allDates.Last().ToString("dd.MM.yyyy");

            var groupedData = monthlyAttendance
                .GroupBy(x => new { x.JobCode, x.EmployeeName })
                .ToList();

            var pivotRows = new List<AttendancePivotRow>();

            foreach (var group in groupedData)
            {
                var pivotRow = new AttendancePivotRow
                {
                    JobCode = group.Key.JobCode,
                    EmployeeName = group.Key.EmployeeName
                };

                foreach (var record in group)
                {
                    int day = record.AttendanceDate.Day;
                    string propertyName = $"Day{day}";

                    var property = typeof(AttendancePivotRow).GetProperty(propertyName);
                    if (property != null)
                    {
                        property.SetValue(pivotRow, record.DayCount.ToString());
                    }
                }
                // Calculate the total of DayCount for the employee
                decimal totalDayCount = group.Sum(x => x.DayCount);
                pivotRow.TotalDays = totalDayCount.ToString();
                pivotRows.Add(pivotRow);
            }
            if (!string.IsNullOrEmpty(isExcel))
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using var package = new ExcelPackage();

                var worksheet = package.Workbook.Worksheets.Add("Monthly Attendance");

                // Number of columns = 2 (Job Code, Employee Name) + number of dates + 1 (Total Days)
                int totalCols = 2 + allDates.Count + 1;

                // Set header text lines in the first 4 rows and merge cells across totalCols
                worksheet.Cells[1, 1, 1, totalCols].Merge = true;
                worksheet.Cells[1, 1].Value = "Bangladesh Gas Fields School & College";

                worksheet.Cells[2, 1, 2, totalCols].Merge = true;
                worksheet.Cells[2, 1].Value = "Birashar, Brahmanbaria.";

                worksheet.Cells[3, 1, 3, totalCols].Merge = true;
                worksheet.Cells[3, 1].Value = "Monthly Report of Daily Worker";

                worksheet.Cells[4, 1, 4, totalCols].Merge = true;
                worksheet.Cells[4, 1].Value = $"Period: {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)}-{year}";

                // Apply styling - center alignment and bold
                for (int row1 = 1; row1 <= 4; row1++)
                {
                    var headerRow = worksheet.Cells[row1, 1, row1, totalCols];
                    headerRow.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    headerRow.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    headerRow.Style.Font.Bold = true;
                    headerRow.Style.Font.Size = 14;
                }

                // Leave a blank row after header before column headers
                int startRow = 6;

                int col = 1;
                worksheet.Cells[startRow, col++].Value = "Job Code";
                worksheet.Cells[startRow, col++].Value = "Employee Name";

                foreach (var date in allDates)
                {
                    worksheet.Cells[startRow, col++].Value = date.ToString("yyyy-MM-dd"); // full date here
                }

                worksheet.Cells[startRow, col].Value = "Total Days";

                int row = startRow + 1;
                foreach (var pivotRow in pivotRows)
                {
                    col = 1;
                    worksheet.Cells[row, col++].Value = pivotRow.JobCode;
                    worksheet.Cells[row, col++].Value = pivotRow.EmployeeName;

                    foreach (var date in allDates)
                    {
                        string propertyName = $"Day{date.Day}";
                        var property = typeof(AttendancePivotRow).GetProperty(propertyName);
                        var value = property?.GetValue(pivotRow);
                        worksheet.Cells[row, col++].Value = value ?? "";
                    }

                    worksheet.Cells[row, col].Value = pivotRow.TotalDays;
                    row++;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                var excelBytes = package.GetAsByteArray();
                string fileName = $"MonthlyAttendance_{year}_{month}.xlsx";

                return File(excelBytes,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            fileName);
            }


            try
            {
               
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
                string printDate = DateTime.Now.ToString("M/d/yy, h:mm tt");

                var parameters = new[]
                    {
                    new Microsoft.Reporting.NETCore.ReportParameter("firstDate",firstDate.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("lastDate",lastDate.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("printDate",printDate.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("month",monthName.ToString().ToUpper()),
                    new Microsoft.Reporting.NETCore.ReportParameter("year",year.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("visibleDays", new[] { allDates.Count.ToString() })


                };

                string renderFormat = "PDF";
                string mimtype = "application/pdf";
                using var report = new Microsoft.Reporting.NETCore.LocalReport();
                report.EnableExternalImages = true;
                string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptMonthlyAttendance.rdlc";
                report.DataSources.Add(new ReportDataSource("dsMonthlyAttendance", pivotRows));
                report.ReportPath = rptPath;
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                return File(pdf, mimtype);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw; // Or return BadRequest(ex.Message)
            }

            // Step 4: Prepare RDLC parameters
            

        }
        //**************** Salary  Control Sheet Officer Report Start ****************
        public async Task<IActionResult> GetSalaryControlSheetOfficer()
        {

            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            var employeeType= await _employeeTypeContract.GetEmployeeTypes();
            var empTypeList = employeeType.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.EmployeeTypeName,
            }).ToList();

            ViewBag.employeeType = empTypeList;

            var departments = await _departmentContract.GetDepartments();
            var departmentList = departments.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.DepartmentName
            }).ToList();

            ViewBag.Departments = departmentList;
            var jobCode = await _employeeContract.GetEmployeeCode();
            var CodeList = jobCode.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.JobCode
            }).ToList();

            ViewBag.EmployeeCode = CodeList;
            return View();
        }
        private string GetEmployeeTypeName(int employeeTypeId)
        {
            return employeeTypeId == 1 ? "Permanent" :
                   employeeTypeId == 2 ? "Contract" :
                   employeeTypeId == 3 ? "Daily Worker" :
                   string.Empty;
        }

        [HttpPost]
        public async Task<IActionResult>GetSalaryControlSheetOfficer(List<string> jobCodes, int EmployeeTypeId, int month, int year, int? department,string isExcel)
         {
            string monthsError = "";
            string yearsError = "";
            string employeeTypeIdError = "";
            if (month == 0)
            {
                monthsError = "Select a Month";

            }
            if (year == 0)
            {
                yearsError = "Select a Year";
            }
            //if (EmployeeTypeId == 0)
            //{
            //    employeeTypeIdError = "Select Employee Type";
            //}

            if (monthsError.Length > 0 || yearsError.Length > 0)
            {
                ViewBag.monthsError = monthsError;
                ViewBag.yearsError = yearsError;
                ViewBag.employeeTypeIdError = employeeTypeIdError;
                return View();
            }

            string monthName1 = new DateTime(year, month, 1).ToString("MMMM");
            string empTypeName1 = EmployeeTypeId == 1 ? "Permanent" :
                                 EmployeeTypeId == 2 ? "Contract" : "Unknown";

            var processDataList = await _salarySettingContract.GetSalaryProcess(); 

            var isProcessed = processDataList.Any(p =>
                p.Month.Equals(monthName1, StringComparison.OrdinalIgnoreCase) &&
                p.Year == year &&
                //p.EmployeeType.Equals(empTypeName1, StringComparison.OrdinalIgnoreCase) &&
                p.Status.Equals("Salary already processsed", StringComparison.OrdinalIgnoreCase)
            );

            if (!isProcessed)
            {
                ViewBag.ProcessMessage = $"Salary not processed for {empTypeName1} Employee for {monthName1} {year} ";
                return View(); 
            }
            if (EmployeeTypeId == 1)
            {
                // Create the monthid in the format YYYYMM
                int monthid = year * 100 + month;
               // var source = await _salaryReportOfficerContract.GetSalaryReportOfficer(monthid, department);
                var source = await _salaryReportOfficerContract.GetScSalarySheetPermanent(jobCodes, monthid);


                foreach (var item in source)
                {
                    if (DateTime.TryParse(item.JoiningDate, out var parsedDate))
                    {
                        item.JoiningDate = parsedDate.ToString("dd MMM yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    }
                }

                // Initialize the grand total variables
                decimal grandGrossPay = 0m;
                decimal grandTotalDeduction = 0m;
                decimal grandNetPay = 0m;
                int totalCount = 0;

                // Calculate Gross Pay, Total Deduction, and Net Pay for each record
                foreach (var item in source)
                {
                    EmployeeViewModel employee = await _employeeContract.GetEmployeeViewByJobCode(item.JobCode);
                    SalarySettingsOfficer salarySettingsOfficer = await _salarySettingContract.GetSalarySettingsOfficerByJobCode(item.JobCode);

                    // Ensure all values are of type decimal before addition
                    item.GrossPay = (Convert.ToDecimal(item.BasicSalary)) +
                                    (Convert.ToDecimal(item.HouseRentAllow)) +
                                    (Convert.ToDecimal(item.FMAllow)) +
                                    (Convert.ToDecimal(item.Conveyance)) +
                                    (Convert.ToDecimal(item.ElectricityAllow)) +
                                    (Convert.ToDecimal(item.GasAllow)) +
                                    (Convert.ToDecimal(item.CtAllow)) +
                                    (Convert.ToDecimal(item.AcAllow)) +
                                     (Convert.ToDecimal(item.OtherSalary)) +
                                    (Convert.ToDecimal(item.ArrearAllow)) +
                                    (Convert.ToDecimal(item.SpecialBenefit));


                    // Calculate Total Deduction
                    item.TotalDeduction =
                                    (Convert.ToDecimal(item.PF)) +
                                    (Convert.ToDecimal(item.RevenueStamp)) +
                                    (Convert.ToDecimal(item.OtherDeduction));

                    // Calculate Net Pay
                    item.NetPay = item.GrossPay - item.TotalDeduction;
                    item.InstituteLib = item.GrossPay + (Convert.ToDecimal(item.PF)) +
                                    (Convert.ToDecimal(item.RevenueStamp));

                    // Accumulate grand totals
                    grandGrossPay += item.GrossPay;
                    grandTotalDeduction += item.TotalDeduction;
                    grandNetPay += item.NetPay;
                    totalCount++;
                }

                if (!string.IsNullOrEmpty(isExcel))
                {
                    // Set EPPlus license context
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    // Create a new Excel package
                    using var package = new ExcelPackage();
                    var worksheet = package.Workbook.Worksheets.Add("Overtime Report");

                    // Define headers
                    string[] headers = {
                            "EMPLOYEE CODE", "EMP. NAME", "DESIGNATION", " ACCOUNT NO","BANK NAME","JOINING DATE","PAY SCALE",
                            "BASIC PAY", "HOUSE RENT", "MED. ALW", " CONV ALW","ELE. SUB","GAS SUB","CT. ALW","AC ALW",
                            "AREAR", "OTHER PAY", "SPECIAL BENIFIT", "GROSS PAY","PF","REV.STAMP","OTHER DEDCTION","TOTAL DEDUCTION",
                            "NET PAY", "INSTITUTE LIAB"
                            };

                    // Add headers to the worksheet
                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = headers[i];
                        worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                        worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        worksheet.Cells[1, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    // Load data into the worksheet
                    int row = 2;
                    foreach (var items in source)
                    {
                        worksheet.Cells[row, 1].Value = items.JobCode;
                        worksheet.Cells[row, 2].Value = items.EmployeeName;
                        worksheet.Cells[row, 3].Value = items.DesignationName;
                        worksheet.Cells[row, 4].Value = items.AccountNumber;
                        worksheet.Cells[row, 5].Value = items.BankName;
                        worksheet.Cells[row, 6].Value = items.JoiningDate;
                        worksheet.Cells[row, 7].Value = items.PayScale;

                        worksheet.Cells[row, 8].Value = items.BasicSalary;
                        worksheet.Cells[row, 9].Value = items.HouseRentAllow;
                        worksheet.Cells[row, 10].Value = items.FMAllow;
                        worksheet.Cells[row, 11].Value = items.Conveyance;
                        worksheet.Cells[row, 12].Value = items.ElectricityAllow;
                        worksheet.Cells[row, 13].Value = items.GasAllow;
                        worksheet.Cells[row, 14].Value = items.CtAllow;
                        worksheet.Cells[row, 15].Value = 0;

                        worksheet.Cells[row, 15].Value = items.ArrearAllow;
                        worksheet.Cells[row, 17].Value = items.OtherSalary;
                        worksheet.Cells[row, 18].Value = items.SpecialBenefit;
                        worksheet.Cells[row, 19].Value = items.GrossPay;
                        worksheet.Cells[row, 20].Value = items.PF;
                        worksheet.Cells[row, 21].Value = items.RevenueStamp;
                        worksheet.Cells[row, 22].Value = items.OtherDeduction;
                        worksheet.Cells[row, 23].Value = items.TotalDeduction;
                        worksheet.Cells[row, 24].Value = items.NetPay;
                        worksheet.Cells[row, 25].Value = items.InstituteLib;
                        row++;
                    }

                    // Auto-fit columns for better visibility
                    worksheet.Cells.AutoFitColumns();

                    // Save the package to a MemoryStream
                    using var stream = new MemoryStream();
                    package.SaveAs(stream);
                    var content = stream.ToArray();

                    // Return the Excel file as a download
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Salary_Sheet_Permanent.xlsx");
                }
                var empType = "";
                if (EmployeeTypeId == 1)
                {
                    empType = "Permanent";
                }
                else
                {
                    empType = "Contract";
                }
                string netPayInWords = NumberToWordsConverter.Convert(grandNetPay) + " Only";
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
                DateTime currentDate = DateTime.Now;
                string printDate = currentDate.ToString("M/d/yy, h:mm tt");


                var parameters = new[]
                {
                    new Microsoft.Reporting.NETCore.ReportParameter("printDate",printDate.ToString()),
                     new Microsoft.Reporting.NETCore.ReportParameter("month",monthName.ToString().ToUpper()),
                    new Microsoft.Reporting.NETCore.ReportParameter("year",year.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("empType",empType.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("netPay",netPayInWords.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("totalCount",totalCount.ToString()),
                };

                string renderFormat = "PDF";
                string mimtype = "application/pdf";
                using var report = new Microsoft.Reporting.NETCore.LocalReport();
                report.EnableExternalImages = true;
                string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptSalaryControlSheetOF.rdlc";
                report.DataSources.Add(new ReportDataSource("dsSalaryReportOF", source));
                report.ReportPath = rptPath;
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                return File(pdf, mimtype);

            }
            if (EmployeeTypeId == 2)
            {
                int monthid = year * 100 + month;
                //var source = await _salaryReportOfficerContract.GetSalaryReportJS(monthid, department);
                var source = await _salaryReportOfficerContract.GetScSalarySheetContructual(jobCodes, monthid);
                foreach (var item in source)
                {
                    if (DateTime.TryParse(item.JoiningDate, out var parsedDate))
                    {
                        item.JoiningDate = parsedDate.ToString("dd MMM yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    }
                }

                // Initialize the grand total variables
                decimal grandGrossPay = 0m;
                decimal grandTotalDeduction = 0m;
                decimal grandNetPay = 0m;
                int totalCount = 0;
                //GetSalarySettingsJuniorStaffByJobCode
                // Calculate Gross Pay, Total Deduction, and Net Pay for each record
                foreach (var item in source)
                {
                    EmployeeViewModel employee = await _employeeContract.GetEmployeeViewByJobCode(item.JobCode);
                    SalarySettingsJuniorStaff salarySettingsJS = await _salarySettingContract.GetSalarySettingsJuniorStaffByJobCode(item.JobCode);
                   
                    // Ensure all values are of type decimal before addition
                    item.GrossPay = (Convert.ToDecimal(item.BasicSalary)) +
                                    (Convert.ToDecimal(item.HouseRentAllow)) +
                                    (Convert.ToDecimal(item.FamilyMedicalAllow)) +
                                    (Convert.ToDecimal(item.ConvenienceAllow)) +
                                    (Convert.ToDecimal(item.ElectricityAllow)) +
                                    (Convert.ToDecimal(item.GasAllow)) +
                                    (Convert.ToDecimal(item.CtAllow)) +
                                    (Convert.ToDecimal(item.AcAllow)) +
                                    (Convert.ToDecimal(item.ArrearAllow)) +
                                    (Convert.ToDecimal(item.SpecialBenefit));

                    // Calculate Total Deduction
                    item.TotalDeduction = (Convert.ToDecimal(item.PF)) +
                                    (Convert.ToDecimal(item.RevenueStamp)) +
                                    (Convert.ToDecimal(item.OtherDeduction));

                    item.InstituteLib = item.GrossPay + (Convert.ToDecimal(item.PF)) +
                                    (Convert.ToDecimal(item.RevenueStamp));
                    // Calculate Net Pay
                    item.NetPay = item.GrossPay - item.TotalDeduction;

                    // Accumulate grand totals
                    grandGrossPay += item.GrossPay;
                    grandTotalDeduction += item.TotalDeduction;
                    grandNetPay += item.NetPay;
                    totalCount++;
                }

                if (!string.IsNullOrEmpty(isExcel))
                {
                    // Set EPPlus license context
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    // Create a new Excel package
                    using var package = new ExcelPackage();
                    var worksheet = package.Workbook.Worksheets.Add("Overtime Report");

                    // Define headers
                    string[] headers = {
                            "EMPLOYEE CODE", "EMP. NAME", "DESIGNATION", " ACCOUNT NO","BANK NAME","JOINING DATE","PAY SCALE",
                            "BASIC PAY", "HOUSE RENT", "MED. ALW", " CONV ALW","ELE. SUB","GAS SUB","CT. ALW","AC ALW",
                            "AREAR", "OTHER PAY", "SPECIAL BENIFIT", "GROSS PAY","PF","REV.STAMP","OTHER DEDCTION","TOTAL DEDUCTION",
                            "NET PAY", "INSTITUTE LIAB"
                            };

                    // Add headers to the worksheet
                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = headers[i];
                        worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                        worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        worksheet.Cells[1, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    // Load data into the worksheet
                    int row = 2;
                    foreach (var items in source)
                    {
                        worksheet.Cells[row, 1].Value = items.JobCode;
                        worksheet.Cells[row, 2].Value = items.EmployeeName;
                        worksheet.Cells[row, 3].Value = items.DesignationName;
                        worksheet.Cells[row, 4].Value = items.AccountNumber;
                        worksheet.Cells[row, 5].Value = items.BankName;
                        worksheet.Cells[row, 6].Value = items.JoiningDate;
                        worksheet.Cells[row, 7].Value = items.PayScale;

                        worksheet.Cells[row, 8].Value = items.BasicSalary;
                        worksheet.Cells[row, 9].Value = items.HouseRentAllow;
                        worksheet.Cells[row, 10].Value = items.FamilyMedicalAllow;
                        worksheet.Cells[row, 11].Value = items.ConvenienceAllow;
                        worksheet.Cells[row, 12].Value = items.ElectricityAllow;
                        worksheet.Cells[row, 13].Value = items.GasAllow;
                        worksheet.Cells[row, 14].Value = items.CtAllow;
                        worksheet.Cells[row, 15].Value = 0;

                        worksheet.Cells[row, 15].Value = items.ArrearAllow;
                        worksheet.Cells[row, 17].Value = items.AcAllow;
                        worksheet.Cells[row, 18].Value = items.SpecialBenefit;
                        worksheet.Cells[row, 19].Value = items.GrossPay;
                        worksheet.Cells[row, 20].Value = items.PF;
                        worksheet.Cells[row, 21].Value = items.RevenueStamp;
                        worksheet.Cells[row, 22].Value = items.OtherDeduction;
                        worksheet.Cells[row, 23].Value = items.TotalDeduction;
                        worksheet.Cells[row, 24].Value = items.NetPay;
                        worksheet.Cells[row, 25].Value = items.InstituteLib;
                        row++;
                    }

                    // Auto-fit columns for better visibility
                    worksheet.Cells.AutoFitColumns();

                    // Save the package to a MemoryStream
                    using var stream = new MemoryStream();
                    package.SaveAs(stream);
                    var content = stream.ToArray();

                    // Return the Excel file as a download
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Salary_Sheet_Contract.xlsx");
                }


                string netPayInWords = NumberToWordsConverter.Convert(grandNetPay) + " Only";
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
                DateTime currentDate = DateTime.Now; 
                string printDate = currentDate.ToString("M/d/yy, h:mm:ss tt");
                var parameters = new[]
                {
                    new Microsoft.Reporting.NETCore.ReportParameter("printDate",printDate.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("month",monthName.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("year",year.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("netPay",netPayInWords.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("totalCount",totalCount.ToString()),
                };

                string renderFormat = "PDF";
                string mimtype = "application/pdf";
                using var report = new Microsoft.Reporting.NETCore.LocalReport();
                report.EnableExternalImages = true;
                string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptSalaryControlSheetJS.rdlc";
                report.DataSources.Add(new ReportDataSource("dsSalaryControlSheetJS", source));
                report.ReportPath = rptPath;
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                return File(pdf, mimtype);

            }
            if (EmployeeTypeId == 3)
            {
                int monthid = year * 100 + month;
                var source = await _salaryReportOfficerContract.GetScSalarySheetDailyWorker(jobCodes, monthid);
                decimal grandGrossPay = 0m;
                decimal grandNetPay = 0m;
                int totalCount = 0;
                if (!string.IsNullOrEmpty(isExcel))
                {
                    // Set EPPlus license context
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    // Create a new Excel package
                    using var package = new ExcelPackage();
                    var worksheet = package.Workbook.Worksheets.Add("Overtime Report");

                    // Define headers
                    string[] headers = {
                            "EMPLOYEE CODE", "Worker. NAME", "DESIGNATION", " ACCOUNT NO","BANK NAME","Attendance","Per Attendance",
                            "Gross PAY", "Rev.Stamp", "NET PAY", "INSTITUTE LIAB"
                            };

                    // Add headers to the worksheet
                    for (int i = 0; i < headers.Length; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = headers[i];
                        worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                        worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                        worksheet.Cells[1, i + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }

                    // Load data into the worksheet
                    int row = 2;
                    foreach (var items in source)
                    {
                        worksheet.Cells[row, 1].Value = items.JobCode;
                        worksheet.Cells[row, 2].Value = items.EmployeeName;
                        worksheet.Cells[row, 3].Value = items.DesignationName;
                        worksheet.Cells[row, 4].Value = items.AccountNumber;
                        worksheet.Cells[row, 5].Value = items.BankName;
                        worksheet.Cells[row, 21].Value = items.Attendance;
                        worksheet.Cells[row, 21].Value = items.PerAttendance;
                        worksheet.Cells[row, 21].Value = items.GrossPay;
                        worksheet.Cells[row, 21].Value = items.RevenueStamp;
                        worksheet.Cells[row, 24].Value = items.NetPay;
                        worksheet.Cells[row, 25].Value = items.InstituteLib;
                        row++;
                    }

                    // Auto-fit columns for better visibility
                    worksheet.Cells.AutoFitColumns();

                    // Save the package to a MemoryStream
                    using var stream = new MemoryStream();
                    package.SaveAs(stream);
                    var content = stream.ToArray();

                    // Return the Excel file as a download
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Wage_Sheet.xlsx");
                }

                 grandNetPay = source.Sum(x => x.NetPay ?? 0);
                string netPayInWords = NumberToWordsConverter.Convert(grandNetPay) + " Only";
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
                DateTime currentDate = DateTime.Now;
                string printDate = currentDate.ToString("M/d/yy, h:mm tt");
                var parameters = new[]
                {
                    new Microsoft.Reporting.NETCore.ReportParameter("printDate",printDate.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("month",monthName.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("year",year.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("netPay",netPayInWords.ToString()),

                };

                string renderFormat = "PDF";
                string mimtype = "application/pdf";
                using var report = new Microsoft.Reporting.NETCore.LocalReport();
                report.EnableExternalImages = true;
                string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptWageSheet.rdlc";
                report.DataSources.Add(new ReportDataSource("dsWageSheet", source));
                report.ReportPath = rptPath;
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                return File(pdf, mimtype);

            }
            else
            {
                int monthid = year * 100 + month;
                var source = await _salaryReportOfficerContract.GetAllSalarySheetUnified(jobCodes, monthid);
                foreach (var item in source)
                {
                    if (DateTime.TryParse(item.JoiningDate, out var parsedDate))
                    {
                        item.JoiningDate = parsedDate.ToString("dd MMM yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    }
                }

              var  dailyWorker = await _salaryReportOfficerContract.GetScSalarySheetDailyWorker(jobCodes, monthid);
               var grandNetPay = dailyWorker.Sum(x => x.NetPay ?? 0);


                string netPayInWords = NumberToWordsConverter.Convert(grandNetPay) + " Only";
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
                DateTime currentDate = DateTime.Now;
                string printDate = currentDate.ToString("M/d/yy, h:mm:ss tt");
                var parameters = new[]
                {
                    new Microsoft.Reporting.NETCore.ReportParameter("printDate",printDate.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("month",monthName.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("year",year.ToString()),
                    //new Microsoft.Reporting.NETCore.ReportParameter("netPay",netPayInWords.ToString()),
                    //new Microsoft.Reporting.NETCore.ReportParameter("totalCount",totalCount.ToString()),
                };

                string renderFormat = "PDF";
                string mimtype = "application/pdf";
                using var report = new Microsoft.Reporting.NETCore.LocalReport();
                report.EnableExternalImages = true;
                string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptSalaryControlSheetJS.rdlc";
                report.DataSources.Add(new ReportDataSource("dsSalaryControlSheetJS", source));
                report.ReportPath = rptPath;
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                return File(pdf, mimtype);
            }
        }


        public async Task<IActionResult> GetSalaryForwarding()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            var employeeType = await _employeeTypeContract.GetEmployeeTypes();
            var empTypeList = employeeType.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.EmployeeTypeName,
            }).ToList();

            ViewBag.employeeType = empTypeList;

            var departments = await _departmentContract.GetDepartments();
            var departmentList = departments.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.DepartmentName
            }).ToList();

            ViewBag.Departments = departmentList;
            var jobCode = await _employeeContract.GetEmployeeCode();
            var CodeList = jobCode.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.JobCode
            }).ToList();

            ViewBag.EmployeeCode = CodeList;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetSalaryForwarding(List<string> jobCodes, int EmployeeTypeId, int month, int year, int? department,string Accounts)
        {
            string monthsError = "";
            string yearsError = "";
            string employeeTypeIdError = "";
            if (month == 0)
            {
                monthsError = "Select a Month";

            }
            if (year == 0)
            {
                yearsError = "Select a Year";
            }
            if (EmployeeTypeId == 0)
            {
                employeeTypeIdError = "Select Employee Type";
            }

            if (monthsError.Length > 0 || yearsError.Length > 0 || employeeTypeIdError.Length > 0)
            {
                ViewBag.monthsError = monthsError;
                ViewBag.yearsError = yearsError;
                ViewBag.employeeTypeIdError = employeeTypeIdError;
                return View();
            }
            string monthName1 = new DateTime(year, month, 1).ToString("MMMM");
            string empTypeName1 = EmployeeTypeId == 1 ? "Permanent" :
                                  EmployeeTypeId == 2 ? "Contract" :
                                  EmployeeTypeId == 3 ? "Daily Worker" : "Unknown";

            var processDataList = await _salarySettingContract.GetSalaryProcess();

            var isProcessed = processDataList.Any(p =>
                p.Month.Equals(monthName1, StringComparison.OrdinalIgnoreCase) &&
                p.Year == year &&
                p.EmployeeType.Equals(empTypeName1, StringComparison.OrdinalIgnoreCase)&&
                p.Status.Equals("Salary already processsed", StringComparison.OrdinalIgnoreCase)
            );

            if (!isProcessed)
            {
                ViewBag.ProcessMessage = $"Salary not processed for {empTypeName1} Employee for {monthName1} {year} ";
                return View();
            }
            if (EmployeeTypeId == 1)
            {
                // Create the monthid in the format YYYYMM
                int monthid = year * 100 + month;
                // var source = await _salaryReportOfficerContract.GetSalaryReportOfficer(monthid, department);
                var source = await _salaryReportOfficerContract.GetScSalarySheetPermanent(jobCodes, monthid);

                // Initialize the grand total variables
                decimal grandGrossPay = 0m;
                decimal grandTotalDeduction = 0m;
                decimal grandNetPay = 0m;
                int totalCount = 0;

                // Calculate Gross Pay, Total Deduction, and Net Pay for each record
                foreach (var item in source)
                {
                    EmployeeViewModel employee = await _employeeContract.GetEmployeeViewByJobCode(item.JobCode);
                    SalarySettingsOfficer salarySettingsOfficer = await _salarySettingContract.GetSalarySettingsOfficerByJobCode(item.JobCode);

                    // Ensure all values are of type decimal before addition
                    item.GrossPay = (Convert.ToDecimal(item.BasicSalary)) +
                                    (Convert.ToDecimal(item.HouseRentAllow)) +
                                    (Convert.ToDecimal(item.FMAllow)) +
                                    (Convert.ToDecimal(item.Conveyance)) +
                                    (Convert.ToDecimal(item.ElectricityAllow)) +
                                    (Convert.ToDecimal(item.GasAllow)) +
                                    (Convert.ToDecimal(item.CtAllow)) +
                                    (Convert.ToDecimal(item.AcAllow)) +
                                    (Convert.ToDecimal(item.OtherSalary)) +
                                    (Convert.ToDecimal(item.ArrearAllow)) +
                                    (Convert.ToDecimal(item.SpecialBenefit));


                    // Calculate Total Deduction
                    item.TotalDeduction =
                                    (Convert.ToDecimal(item.PF)) +
                                    (Convert.ToDecimal(item.RevenueStamp)) +
                                    (Convert.ToDecimal(item.OtherDeduction));

                    // Calculate Net Pay
                    item.NetPay = item.GrossPay - item.TotalDeduction;

                    // Accumulate grand totals
                    grandGrossPay += item.GrossPay;
                    grandTotalDeduction += item.TotalDeduction;
                    grandNetPay += item.NetPay;
                    totalCount++;
                }


                string netPayInWords = NumberToWordsConverter.Convert(grandNetPay);
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
                DateTime currentDate = DateTime.Now;
                string printDateTime = currentDate.ToString("M/d/yy, h:mm tt");

                
                string printDate = currentDate.ToString("MMMM dd, yyyy");
                var empType = "";
                if (EmployeeTypeId == 1)
                {
                    empType = "Salary/Permanent";
                }
                if (EmployeeTypeId == 2)
                {
                    empType = "Salary/Contract";
                }
                string monthYear = $"{monthName}-{year}";
                var parameters = new[]
                {
                    new Microsoft.Reporting.NETCore.ReportParameter("Accounts",Accounts.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("printDate",printDate.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("printDateTime",printDateTime.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("empType",empType.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("netPayInWords",netPayInWords.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("monthYear",monthYear.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("month",monthName.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("year",year.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("grandNetPay", grandNetPay.ToString("N2", CultureInfo.InvariantCulture)),

                };

                string renderFormat = "PDF";
                string mimtype = "application/pdf";
                using var report = new Microsoft.Reporting.NETCore.LocalReport();
                report.EnableExternalImages = true;
                string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptSalaryForwarding.rdlc";
                report.DataSources.Add(new ReportDataSource("dsSalaryReportOF", source));
                report.ReportPath = rptPath;
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                return File(pdf, mimtype);

            }
            else if (EmployeeTypeId == 2)
            {
                int monthid = year * 100 + month;
                //var source = await _salaryReportOfficerContract.GetSalaryReportJS(monthid, department);
                var source = await _salaryReportOfficerContract.GetScSalarySheetContructual(jobCodes, monthid);
                // Initialize the grand total variables
                decimal grandGrossPay = 0m;
                decimal grandTotalDeduction = 0m;
                decimal grandNetPay = 0;
                int totalCount = 0;
                //GetSalarySettingsJuniorStaffByJobCode
                // Calculate Gross Pay, Total Deduction, and Net Pay for each record
                foreach (var item in source)
                {
                    EmployeeViewModel employee = await _employeeContract.GetEmployeeViewByJobCode(item.JobCode);
                    SalarySettingsJuniorStaff salarySettingsJS = await _salarySettingContract.GetSalarySettingsJuniorStaffByJobCode(item.JobCode);

                    // Ensure all values are of type decimal before addition
                    item.GrossPay = (Convert.ToDecimal(item.BasicSalary)) +
                                    (Convert.ToDecimal(item.HouseRentAllow)) +
                                    (Convert.ToDecimal(item.FamilyMedicalAllow)) +
                                    (Convert.ToDecimal(item.ConvenienceAllow)) +
                                    (Convert.ToDecimal(item.ElectricityAllow)) +
                                    (Convert.ToDecimal(item.GasAllow)) +
                                    (Convert.ToDecimal(item.CtAllow)) +
                                    (Convert.ToDecimal(item.AcAllow)) +
                                    (Convert.ToDecimal(item.ArrearAllow)) +
                                    (Convert.ToDecimal(item.SpecialBenefit));

                    // Calculate Total Deduction
                    item.TotalDeduction = (Convert.ToDecimal(item.PF)) +
                                    (Convert.ToDecimal(item.RevenueStamp)) +
                                    (Convert.ToDecimal(item.OtherDeduction));


                    // Calculate Net Pay
                    item.NetPay = item.GrossPay - item.TotalDeduction;

                    // Accumulate grand totals
                    grandGrossPay += item.GrossPay;
                    grandTotalDeduction += item.TotalDeduction;
                    grandNetPay += item.NetPay;
                    totalCount++;
                }
                string netPayInWords = NumberToWordsConverter.Convert(grandNetPay);
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
                DateTime currentDate = DateTime.Now;
                string printDate = currentDate.ToString("MMMM dd, yyyy");
                string printDateTime = currentDate.ToString("M/d/yy, h:mm tt");
                var empType = "";
                if (EmployeeTypeId == 1)
                {
                    empType = "Permanent";
                }
                if (EmployeeTypeId == 2)
                {
                    empType = "Salary/Contract";
                }
                string monthYear = $"{monthName} - {year}";
                var parameters = new[]
                {
                    new Microsoft.Reporting.NETCore.ReportParameter("Accounts",Accounts.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("printDate",printDate.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("printDateTime",printDateTime.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("empType",empType.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("netPayInWords",netPayInWords.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("monthYear",monthYear.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("month",monthName.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("year",year.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("grandNetPay", grandNetPay.ToString("N2", CultureInfo.InvariantCulture)),
                };

                string renderFormat = "PDF";
                string mimtype = "application/pdf";
                using var report = new Microsoft.Reporting.NETCore.LocalReport();
                report.EnableExternalImages = true;
                string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptSalaryForwarding.rdlc";
                report.DataSources.Add(new ReportDataSource("dsSalaryControlSheetJS", source));
                report.ReportPath = rptPath;
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                return File(pdf, mimtype);

            }
            else
            {
                int monthid = year * 100 + month;
                string netPayInWords = "";
                decimal grandNetPay = 0;
                var source = await _salaryReportOfficerContract.GetScSalarySheetDailyWorker(jobCodes, monthid);
                grandNetPay = source.Sum(x => x.NetPay ?? 0);
                netPayInWords = NumberToWordsConverter.Convert(grandNetPay);
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
                DateTime currentDate = DateTime.Now;
                string printDate = currentDate.ToString("MMMM dd, yyyy");
                string printDateTime = currentDate.ToString("M/d/yy, h:mm tt");
                var empType = "";
                if (EmployeeTypeId == 1)
                {
                    empType = "Permanent";
                }
                if (EmployeeTypeId == 2)
                {
                    empType = "Contructual";
                }
                if (EmployeeTypeId == 3)
                {
                    empType = "Wages/Daily Worker";
                }
                string monthYear = $"{monthName} - {year}";
                var parameters = new[]
                {
                    new Microsoft.Reporting.NETCore.ReportParameter("Accounts",Accounts.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("printDate",printDate.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("printDateTime",printDateTime.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("empType",empType.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("netPayInWords",netPayInWords.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("monthYear",monthYear.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("month",monthName.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("year",year.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("grandNetPay", grandNetPay.ToString("N2", CultureInfo.InvariantCulture)),
                };

                string renderFormat = "PDF";
                string mimtype = "application/pdf";
                using var report = new Microsoft.Reporting.NETCore.LocalReport();
                report.EnableExternalImages = true;
                string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptSalaryForwarding.rdlc";
                report.DataSources.Add(new ReportDataSource("dsSalaryControlSheetJS", source));
                report.ReportPath = rptPath;
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                return File(pdf, mimtype);

            }


        }


        [HttpGet]
        public async Task<IActionResult> YearlySalaryControlSheet()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            var employeeType = await _employeeTypeContract.GetEmployeeTypes();
            var empTypeList = employeeType.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.EmployeeTypeName,
            }).ToList();

            ViewBag.employeeType = empTypeList;

            var departments = await _departmentContract.GetDepartments();
            var departmentList = departments.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.DepartmentName
            }).ToList();

            ViewBag.Departments = departmentList;

            var jobCode = await _employeeContract.GetEmployeeCode();
            var CodeList = jobCode.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.JobCode
            }).ToList();

            ViewBag.EmployeeCode = CodeList;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> YearlySalaryControlSheet(List<string> jobCode, int EmployeeTypeId,int fmonth,int fyear,int tmonth,int tyear, string department,string isExcel)
        {
            if (EmployeeTypeId == 1)
            {
                // Create the monthid in the format YYYYMM
                int fmonthid = fyear * 100 + fmonth;
                int tmonthid=tyear* 100 + tmonth;

                var data = await _salaryReportOfficerContract.GetYearlySalaryControlSheetOF(jobCode, fmonthid, tmonthid, department);

                // Initialize the grand total variables
                decimal grandGrossPay = 0m;
                decimal grandTotalDeduction = 0m;
                decimal grandNetPay = 0m;
                int totalCount = 0;

                // Calculate Gross Pay, Total Deduction, and Net Pay for each record
                foreach (var item in data)
                {
                    SalarySettingsOfficer salarySettingsOfficer = await _salarySettingContract.GetSalarySettingsOfficerByJobCode(item.JobCode);
                    // Ensure all values are of type decimal before addition
                    item.GrossPay = (Convert.ToDecimal(item.BasicSalary)) +
                                    (Convert.ToDecimal(item.PersonalSalary)) +
                                    (Convert.ToDecimal(item.ArrearSalary)) +
                                    (Convert.ToDecimal(item.LikeBasic)) +
                                    (Convert.ToDecimal(item.OtherSalary)) +
                                    (Convert.ToDecimal(item.LunchAllow)) +
                                    (Convert.ToDecimal(item.TiffinAllow)) +
                                    (Convert.ToDecimal(item.SpecialBenefit)) +
                                    (Convert.ToDecimal(item.HouseRentAllow)) +
                                    (Convert.ToDecimal(item.FMAllow)) +
                                    (Convert.ToDecimal(item.WashAllow)) +
                                    (Convert.ToDecimal(item.EducationalAllow)) +
                                    (Convert.ToDecimal(item.FieldRiskAllow)) +
                                    (Convert.ToDecimal(item.ChargeAllow)) +
                                    (Convert.ToDecimal(item.DAidAllow)) +
                                    (Convert.ToDecimal(item.DeputationAllow)) +
                                    (Convert.ToDecimal(item.OtherAllow)) +
                                    (Convert.ToDecimal(item.Conveyance)) +
                                    (Convert.ToDecimal(item.CME));

                    // Calculate Total Deduction
                    item.TotalDeduction = (Convert.ToDecimal(item.RevenueStamp)) +
                                          (Convert.ToDecimal(item.WelfareFund)) +
                                          (Convert.ToDecimal(item.OfficerClub)) +
                                          (Convert.ToDecimal(item.OfficerAssociation)) +
                                          (Convert.ToDecimal(item.MedicalFund)) +
                                          (Convert.ToDecimal(item.ProvidentFund)) +
                                          (Convert.ToDecimal(item.Advance)) +
                                          (Convert.ToDecimal(item.IncomeTax)) +
                                          (Convert.ToDecimal(item.Other)) +
                                          (Convert.ToDecimal(item.SpecialDeduction)) +
                                          (Convert.ToDecimal(item.Hospitalisation)) +
                                          (Convert.ToDecimal(item.TMBill)) +
                                          (Convert.ToDecimal(item.HouseRentReturn)) +
                                          (Convert.ToDecimal(item.Dormitory)) +
                                          (Convert.ToDecimal(item.FuelReturn)) +
                                           (Convert.ToDecimal(item.HBLoan)) +
                                          (Convert.ToDecimal(item.MCylLoan)) +
                                          (Convert.ToDecimal(item.BCylLoan)) +
                                          (Convert.ToDecimal(item.ComLoan)) +
                                          (Convert.ToDecimal(item.PFLoan)) +
                                          (Convert.ToDecimal(item.WPFLoan)) +
                                          (Convert.ToDecimal(item.CosLoan)) +
                                          (Convert.ToDecimal(item.CarLoan)) +
                                          (Convert.ToDecimal(item.OtherLoan));



                    if (salarySettingsOfficer.OfficerPF > 0)
                    {
                        item.GPF = item.ProvidentFund;
                        item.CPF = 0;
                        item.PensionFromCompany = item.PensionOfficer;
                        item.TotalCompanyLiabilities = item.GrossPay + item.PensionOfficer;
                    }
                    else
                    {
                        item.CPF = item.ProvidentFund;
                        item.GPF = 0;
                        item.PfFromCompany = item.ProvidentFund;
                        item.TotalCompanyLiabilities = item.GrossPay + item.ProvidentFund;
                    }

                    // Calculate Net Pay
                    item.NetPay = item.GrossPay - item.TotalDeduction;

                    // Accumulate grand totals
                    grandGrossPay += item.GrossPay ?? 0m;
                    grandTotalDeduction += item.TotalDeduction ?? 0m;
                    grandNetPay += item.NetPay ?? 0m;
                    totalCount++;
                }

                string netPayInWords = NumberToWordsConverter.Convert(grandNetPay);
                string fmonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(fmonth);
                string tmonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(tmonth);

                DateTime now = DateTime.Now;
                int currentMonth = now.Month;
                int currentYear = now.Year;

                if (!string.IsNullOrEmpty(isExcel))
                {
                    var workBook = new XLWorkbook();
                    var workSheet = workBook.Worksheets.Add("Sheet1");
                   // workSheet.Cell(1, 1).Value = "Month Year";
                    workSheet.Cell(1, 2).Value = "JobCode";
                    workSheet.Cell(1, 3).Value = "EmployeeName";
                    workSheet.Cell(1, 4).Value = "Designation";
                    workSheet.Cell(1, 5).Value = "Department";
                    workSheet.Cell(1, 6).Value = "BasicSalary";
                    workSheet.Cell(1, 7).Value = "PersonalSalary";
                    workSheet.Cell(1, 8).Value = "ArrearSalary";
                    workSheet.Cell(1, 9).Value = "LikeBasic";
                    workSheet.Cell(1, 10).Value = "OtherSalary";
                    workSheet.Cell(1, 11).Value = "LunchAllow";
                    workSheet.Cell(1, 12).Value = "TiffinAllow";
                    workSheet.Cell(1, 13).Value = "SpecialBenefit";
                    workSheet.Cell(1, 14).Value = "HouseRentAllow";
                    workSheet.Cell(1, 15).Value = "FMAllow";
                    workSheet.Cell(1, 16).Value = "WashAllow";
                    workSheet.Cell(1, 17).Value = "EducationalAllow";
                    workSheet.Cell(1, 18).Value = "FieldRiskAllow";
                    workSheet.Cell(1, 19).Value = "ChargeAllow";
                    workSheet.Cell(1, 20).Value = "DAidAllow";
                    workSheet.Cell(1, 21).Value = "DeputationAllow";
                    workSheet.Cell(1, 22).Value = "OtherAllow";
                    workSheet.Cell(1, 23).Value = "Conveyance";
                    workSheet.Cell(1, 24).Value = "CME";
                    workSheet.Cell(1, 25).Value = "Gross Pay";
                    workSheet.Cell(1, 26).Value = "RevenueStamp";
                    workSheet.Cell(1, 27).Value = "WelfareFund";
                    workSheet.Cell(1, 28).Value = "OfficerClub";
                    workSheet.Cell(1, 29).Value = "OfficerAssociation";
                    workSheet.Cell(1, 30).Value = "MedicalFund";
                    workSheet.Cell(1, 31).Value = "CPF";
                    workSheet.Cell(1, 32).Value = "GPF";
                    workSheet.Cell(1, 33).Value = "Advance";
                    workSheet.Cell(1, 34).Value = "IncomeTax";
                    workSheet.Cell(1, 35).Value = "Other";
                    workSheet.Cell(1, 36).Value = "SpecialDeduction";
                    workSheet.Cell(1, 37).Value = "Hospitalisation";
                    workSheet.Cell(1, 38).Value = "TMBill";
                    workSheet.Cell(1, 39).Value = "HouseRentReturn";
                    workSheet.Cell(1, 40).Value = "Dormitory";
                    workSheet.Cell(1, 41).Value = "FuelReturn";
                    workSheet.Cell(1, 42).Value = "HBLoan";
                    workSheet.Cell(1, 43).Value = "MCylLoan";
                    workSheet.Cell(1, 44).Value = "BCylLoan";
                    workSheet.Cell(1, 45).Value = "ComLoan";
                    workSheet.Cell(1, 46).Value = "PFLoan";
                    workSheet.Cell(1, 47).Value = "WPFLoan";
                    workSheet.Cell(1, 48).Value = "CarLoan";
                    workSheet.Cell(1, 49).Value = "CosLoan";
                    workSheet.Cell(1, 50).Value = "Total Deduction";
                    workSheet.Cell(1, 51).Value = "Net Pay";
                    workSheet.Cell(1, 52).Value = "CPF Company";
                    workSheet.Cell(1, 53).Value = "GPF Company";
                    workSheet.Cell(1, 54).Value = "Total Company Liabilities";


                    int row = 2;

                    foreach (var item in data)
                    {

                        item.GrossPay = (Convert.ToDecimal(item.BasicSalary)) +
                                    (Convert.ToDecimal(item.PersonalSalary)) +
                                    (Convert.ToDecimal(item.ArrearSalary)) +
                                    (Convert.ToDecimal(item.LikeBasic)) +
                                    (Convert.ToDecimal(item.OtherSalary)) +
                                    (Convert.ToDecimal(item.LunchAllow)) +
                                    (Convert.ToDecimal(item.TiffinAllow)) +
                                    (Convert.ToDecimal(item.SpecialBenefit)) +
                                    (Convert.ToDecimal(item.HouseRentAllow)) +
                                    (Convert.ToDecimal(item.FMAllow)) +
                                    (Convert.ToDecimal(item.WashAllow)) +
                                    (Convert.ToDecimal(item.EducationalAllow)) +
                                    (Convert.ToDecimal(item.FieldRiskAllow)) +
                                    (Convert.ToDecimal(item.ChargeAllow)) +
                                    (Convert.ToDecimal(item.DAidAllow)) +
                                    (Convert.ToDecimal(item.DeputationAllow)) +
                                    (Convert.ToDecimal(item.OtherAllow)) +
                                    (Convert.ToDecimal(item.Conveyance)) +
                                    (Convert.ToDecimal(item.CME));

                        // Calculate Total Deduction
                        item.TotalDeduction = (Convert.ToDecimal(item.RevenueStamp)) +
                                          (Convert.ToDecimal(item.WelfareFund)) +
                                          (Convert.ToDecimal(item.OfficerClub)) +
                                          (Convert.ToDecimal(item.OfficerAssociation)) +
                                          (Convert.ToDecimal(item.MedicalFund)) +
                                          (Convert.ToDecimal(item.ProvidentFund)) +
                                          (Convert.ToDecimal(item.Advance)) +
                                          (Convert.ToDecimal(item.IncomeTax)) +
                                          (Convert.ToDecimal(item.Other)) +
                                          (Convert.ToDecimal(item.SpecialDeduction)) +
                                          (Convert.ToDecimal(item.Hospitalisation)) +
                                          (Convert.ToDecimal(item.TMBill)) +
                                          (Convert.ToDecimal(item.HouseRentReturn)) +
                                          (Convert.ToDecimal(item.Dormitory)) +
                                          (Convert.ToDecimal(item.FuelReturn)) +
                                           (Convert.ToDecimal(item.HBLoan)) +
                                          (Convert.ToDecimal(item.MCylLoan)) +
                                          (Convert.ToDecimal(item.BCylLoan)) +
                                          (Convert.ToDecimal(item.ComLoan)) +
                                          (Convert.ToDecimal(item.PFLoan)) +
                                          (Convert.ToDecimal(item.WPFLoan)) +
                                          (Convert.ToDecimal(item.CosLoan)) +
                                          (Convert.ToDecimal(item.CarLoan)) +
                                          (Convert.ToDecimal(item.OtherLoan));

                        // Calculate Net Pay
                        item.NetPay = item.GrossPay - item.TotalDeduction;



                        //workSheet.Cell(row, 1).Value = item.MonthYear;
                        workSheet.Cell(row, 2).Value = item.JobCode;
                        workSheet.Cell(row, 3).Value = item.EmployeeName;
                        workSheet.Cell(row, 4).Value = item.DesignationName;
                        workSheet.Cell(row, 5).Value = item.DepartmentName;
                        workSheet.Cell(row, 6).Value = item.BasicSalary;
                        workSheet.Cell(row, 7).Value = item.PersonalSalary;
                        workSheet.Cell(row, 8).Value = item.ArrearSalary;
                        workSheet.Cell(row, 9).Value = item.LikeBasic;
                        workSheet.Cell(row, 10).Value = item.OtherSalary;
                        workSheet.Cell(row, 11).Value = item.LunchAllow;
                        workSheet.Cell(row, 12).Value = item.TiffinAllow;
                        workSheet.Cell(row, 13).Value = item.SpecialBenefit;
                        workSheet.Cell(row, 14).Value = item.HouseRentAllow;
                        workSheet.Cell(row, 15).Value = item.FMAllow;
                        workSheet.Cell(row, 16).Value = item.WashAllow;
                        workSheet.Cell(row, 17).Value = item.EducationalAllow;
                        workSheet.Cell(row, 18).Value = item.FieldRiskAllow;
                        workSheet.Cell(row, 19).Value = item.ChargeAllow;
                        workSheet.Cell(row, 20).Value = item.DAidAllow;
                        workSheet.Cell(row, 21).Value = item.DeputationAllow;
                        workSheet.Cell(row, 22).Value = item.OtherAllow;
                        workSheet.Cell(row, 23).Value = item.Conveyance;
                        workSheet.Cell(row, 24).Value = item.CME;
                        workSheet.Cell(row, 25).Value = item.GrossPay;

                        workSheet.Cell(row, 26).Value = item.RevenueStamp;
                        workSheet.Cell(row, 27).Value = item.WelfareFund;
                        workSheet.Cell(row, 28).Value = item.OfficerClub;
                        workSheet.Cell(row, 29).Value = item.OfficerAssociation;
                        workSheet.Cell(row, 30).Value = item.MedicalFund;
                        workSheet.Cell(row, 31).Value = item.CPF;
                        workSheet.Cell(row, 32).Value = item.GPF;
                        workSheet.Cell(row, 33).Value = item.Advance;
                        workSheet.Cell(row, 34).Value = item.IncomeTax;
                        workSheet.Cell(row, 35).Value = item.Other;
                        workSheet.Cell(row, 36).Value = item.SpecialDeduction;
                        workSheet.Cell(row, 37).Value = item.Hospitalisation;
                        workSheet.Cell(row, 38).Value = item.TMBill;
                        workSheet.Cell(row, 39).Value = item.HouseRentReturn;
                        workSheet.Cell(row, 40).Value = item.Dormitory;
                        workSheet.Cell(row, 41).Value = item.FuelReturn;
                        workSheet.Cell(row, 42).Value = item.HBLoan;
                        workSheet.Cell(row, 43).Value = item.MCylLoan;
                        workSheet.Cell(row, 44).Value = item.BCylLoan;
                        workSheet.Cell(row, 45).Value = item.ComLoan;
                        workSheet.Cell(row, 46).Value = item.PFLoan;
                        workSheet.Cell(row, 47).Value = item.WPFLoan;
                        workSheet.Cell(row, 48).Value = item.CarLoan;
                        workSheet.Cell(row, 49).Value = item.CosLoan;
                        workSheet.Cell(row, 50).Value = item.TotalDeduction;
                        workSheet.Cell(row, 51).Value = item.GrossPay - item.TotalDeduction;
                        workSheet.Cell(row, 52).Value = item.PfFromCompany;
                        workSheet.Cell(row, 53).Value = item.PensionFromCompany;
                        workSheet.Cell(row, 54).Value = item.TotalCompanyLiabilities;

                        row++;
                    }
                    var stream = new MemoryStream();
                    workBook.SaveAs(stream);
                    stream.Position = 0;

                    string excelName = "yearly_Salary_Sheet_";
                    if (EmployeeTypeId == 1)
                    {
                        excelName += "officer_";
                    }
                    else
                    {
                        excelName += "junior_staff_";
                    }
                    excelName += currentMonth + "_" + currentYear.ToString() + ".xlsx";
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
                }
                else
                {
                    DateTime currentDate = DateTime.Now;
                    string printDate = currentDate.ToString("MMMM dd, yyyy");
                    var parameters = new[]
                  {
                    new Microsoft.Reporting.NETCore.ReportParameter("printDate",printDate.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("fmonth",fmonthName.ToString()),
                     new Microsoft.Reporting.NETCore.ReportParameter("tmonth",tmonthName.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("fyear",fyear.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("tyear",tyear.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("netPay",netPayInWords.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("totalCount",totalCount.ToString()),
                  };

                    string renderFormat = "PDF";
                    string mimtype = "application/pdf";
                    using var report = new Microsoft.Reporting.NETCore.LocalReport();
                    report.EnableExternalImages = true;
                    string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptSalaryControlSheetOFYearly.rdlc";
                    report.DataSources.Add(new ReportDataSource("dsSalaryReportOF", data));
                    report.ReportPath = rptPath;
                    report.SetParameters(parameters);
                    var pdf = report.Render(renderFormat);
                    return File(pdf, mimtype);
                }
                  

            }
            else
            {
                int fmonthid = fyear * 100 + fmonth;
                int tmonthid = tyear * 100 + tmonth;
                //var source = await _salaryReportOfficerContract.GetSalaryReportJS(monthid, department);
                var data = await _salaryReportOfficerContract.GetYearlySalaryControlSheetJS(jobCode, fmonthid, tmonthid, department);


                // Initialize the grand total variables
                decimal grandGrossPay = 0m;
                decimal grandTotalDeduction = 0m;
                decimal grandNetPay = 0m;
                int totalCount = 0;

                // Calculate Gross Pay, Total Deduction, and Net Pay for each record
                foreach (var item in data)
                {
                    SalarySettingsJuniorStaff salarySettingsJS = await _salarySettingContract.GetSalarySettingsJuniorStaffByJobCode(item.JobCode);

                    // Ensure all values are of type decimal before addition
                    item.GrossPay = (Convert.ToDecimal(item.BasicSalary)) +
                                    (Convert.ToDecimal(item.PersonalSalary)) +
                                    (Convert.ToDecimal(item.ArrearSalary)) +
                                    (Convert.ToDecimal(item.OtherSalary)) +
                                    (Convert.ToDecimal(item.LunchAllow)) +
                                    (Convert.ToDecimal(item.TiffinAllow)) +
                                    (Convert.ToDecimal(item.HouseRentAllow)) +
                                    (Convert.ToDecimal(item.FamilyMedicalAllow)) +
                                    (Convert.ToDecimal(item.ShiftAllow)) +
                                    (Convert.ToDecimal(item.OtAllow)) +
                                    (Convert.ToDecimal(item.UtilityAllow)) +
                                    (Convert.ToDecimal(item.EducationAllowance)) +
                                    (Convert.ToDecimal(item.FuelAllow)) +
                                    (Convert.ToDecimal(item.OtherAllow)) +
                                    (Convert.ToDecimal(item.FieldAllow)) +
                                    (Convert.ToDecimal(item.ConvenienceAllow)) +
                                    (Convert.ToDecimal(item.SpecialBenefit));

                    // Calculate Total Deduction
                    item.TotalDeduction = (Convert.ToDecimal(item.RevenueStamp)) +
                                          (Convert.ToDecimal(item.WelfareFund)) +
                                          (Convert.ToDecimal(item.EmployeeClub)) +
                                          (Convert.ToDecimal(item.EmployeeUnion)) +
                                          (Convert.ToDecimal(item.ProvidentFund)) +
                                          (Convert.ToDecimal(item.Dormitory)) +
                                          (Convert.ToDecimal(item.HospitalDeduction)) +
                                          (Convert.ToDecimal(item.FuelReturn)) +
                                          (Convert.ToDecimal(item.SpecialDeduction)) +
                                          (Convert.ToDecimal(item.Others)) +
                                          (Convert.ToDecimal(item.Advance)) +
                                           (Convert.ToDecimal(item.UtilityReturn)) +
                                          (Convert.ToDecimal(item.HBLoan)) +
                                          (Convert.ToDecimal(item.MCylLoan)) +
                                          (Convert.ToDecimal(item.BCylLoan)) +
                                          (Convert.ToDecimal(item.ComputerLoan)) +
                                          (Convert.ToDecimal(item.PFLoan)) +
                                          (Convert.ToDecimal(item.WPFLoan)) +
                                          (Convert.ToDecimal(item.CosLoan));

                    if (salarySettingsJS.ProvidentFund > 0)
                    {
                        item.GPF = item.ProvidentFund;
                        item.CPF = 0;
                        item.PensionFromCompany = item.PensionCom;
                        item.TotalCompanyLiabilities = item.GrossPay + item.PensionCom;
                    }
                    else
                    {
                        item.CPF = item.ProvidentFund;
                        item.GPF = 0;
                        item.PfFromCompany = item.ProvidentFund;
                        item.TotalCompanyLiabilities = item.GrossPay + item.ProvidentFund;
                    }


                    // Calculate Net Pay
                    item.NetPay = item.GrossPay - item.TotalDeduction;

                    // Accumulate grand totals
                    grandGrossPay += item.GrossPay ?? 0m;
                    grandTotalDeduction += item.TotalDeduction ?? 0m;
                    grandNetPay += item.NetPay ?? 0m;
                    totalCount++;
                }
                string netPayInWords = NumberToWordsConverter.Convert(grandNetPay);
                string fmonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(fmonth);
                string tmonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(tmonth);

                DateTime now = DateTime.Now;
                int currentMonth = now.Month;
                int currentYear = now.Year;
                if (!string.IsNullOrEmpty(isExcel))
                {
                    var workBook = new XLWorkbook();
                    var workSheet = workBook.Worksheets.Add("Sheet1");

                    // Set headers
                   // workSheet.Cell(1, 1).Value = "Month Year";
                    workSheet.Cell(1, 2).Value = "JobCode";
                    workSheet.Cell(1, 3).Value = "EmployeeName";
                    workSheet.Cell(1, 4).Value = "Designation";
                    workSheet.Cell(1, 5).Value = "Department";
                    workSheet.Cell(1, 6).Value = "BasicSalary";
                    workSheet.Cell(1, 7).Value = "PersonalSalary";
                    workSheet.Cell(1, 8).Value = "ArrearSalary";
                    workSheet.Cell(1, 9).Value = "OtherSalary";
                    workSheet.Cell(1, 10).Value = "LunchAllow";
                    workSheet.Cell(1, 11).Value = "TiffinAllow";
                    workSheet.Cell(1, 12).Value = "HouseRentAllow";
                    workSheet.Cell(1, 13).Value = "FamilyMedicalAllow";
                    workSheet.Cell(1, 14).Value = "ShiftAllow";
                    workSheet.Cell(1, 15).Value = "OtAllow";
                    workSheet.Cell(1, 16).Value = "UtilityAllow";
                    workSheet.Cell(1, 17).Value = "EducationAllowance";
                    workSheet.Cell(1, 18).Value = "FuelAllow";
                    workSheet.Cell(1, 19).Value = "OtherAllow";
                    workSheet.Cell(1, 20).Value = "FieldAllow";
                    workSheet.Cell(1, 21).Value = "ConvenienceAllow";
                    workSheet.Cell(1, 22).Value = "SpecialBenefit";
                    workSheet.Cell(1, 23).Value = "Gross Pay";
                    workSheet.Cell(1, 24).Value = "RevenueStamp";
                    workSheet.Cell(1, 25).Value = "WelfareFund";
                    workSheet.Cell(1, 26).Value = "EmployeeClub";
                    workSheet.Cell(1, 27).Value = "EmployeeUnion";
                    workSheet.Cell(1, 28).Value = "CPF";
                    workSheet.Cell(1, 29).Value = "GPF";
                    workSheet.Cell(1, 30).Value = "Dormitory";
                    workSheet.Cell(1, 31).Value = "HospitalDeduction";
                    workSheet.Cell(1, 32).Value = "FuelReturn";
                    workSheet.Cell(1, 33).Value = "SpecialDeduction";
                    workSheet.Cell(1, 34).Value = "Others";
                    workSheet.Cell(1, 35).Value = "Advance";
                    workSheet.Cell(1, 36).Value = "HBLoan";
                    workSheet.Cell(1, 37).Value = "MCylLoan";
                    workSheet.Cell(1, 38).Value = "BCylLoan";
                    workSheet.Cell(1, 39).Value = "ComputerLoan";
                    workSheet.Cell(1, 40).Value = "PFLoan";
                    workSheet.Cell(1, 41).Value = "WPFLoan";
                    workSheet.Cell(1, 42).Value = "CosLoan";
                    workSheet.Cell(1, 43).Value = "Other Loan";
                    workSheet.Cell(1, 44).Value = "Total Deduction";
                    workSheet.Cell(1, 45).Value = "Net Pay";
                    workSheet.Cell(1, 46).Value = "CPF Company";
                    workSheet.Cell(1, 47).Value = "GPF Company";
                    workSheet.Cell(1, 48).Value = "Total Company Liabilities";
                    workSheet.Cell(1, 49).Value = "Utility Return";

                    int row = 2;

                    // Iterate through data and populate the worksheet
                    foreach (var item in data)
                    {
                        // Calculate Gross Pay
                        item.GrossPay = (Convert.ToDecimal(item.BasicSalary)) +
                                        (Convert.ToDecimal(item.PersonalSalary)) +
                                        (Convert.ToDecimal(item.ArrearSalary)) +
                                        (Convert.ToDecimal(item.OtherSalary)) +
                                        (Convert.ToDecimal(item.LunchAllow)) +
                                        (Convert.ToDecimal(item.TiffinAllow)) +
                                        (Convert.ToDecimal(item.HouseRentAllow)) +
                                        (Convert.ToDecimal(item.FamilyMedicalAllow)) +
                                        (Convert.ToDecimal(item.ShiftAllow)) +
                                        (Convert.ToDecimal(item.OtAllow)) +
                                        (Convert.ToDecimal(item.UtilityAllow)) +
                                        (Convert.ToDecimal(item.EducationAllowance)) +
                                        (Convert.ToDecimal(item.FuelAllow)) +
                                        (Convert.ToDecimal(item.OtherAllow)) +
                                        (Convert.ToDecimal(item.FieldAllow)) +
                                        (Convert.ToDecimal(item.ConvenienceAllow)) +
                                        (Convert.ToDecimal(item.SpecialBenefit));

                        // Calculate Total Deduction
                        item.TotalDeduction = (Convert.ToDecimal(item.RevenueStamp)) +
                                              (Convert.ToDecimal(item.WelfareFund)) +
                                              (Convert.ToDecimal(item.EmployeeClub)) +
                                              (Convert.ToDecimal(item.EmployeeUnion)) +
                                              (Convert.ToDecimal(item.ProvidentFund)) +
                                              (Convert.ToDecimal(item.Dormitory)) +
                                              (Convert.ToDecimal(item.HospitalDeduction)) +
                                              (Convert.ToDecimal(item.FuelReturn)) +
                                              (Convert.ToDecimal(item.SpecialDeduction)) +
                                              (Convert.ToDecimal(item.Others)) +
                                              (Convert.ToDecimal(item.Advance)) +
                                              (Convert.ToDecimal(item.UtilityReturn)) +
                                              (Convert.ToDecimal(item.HBLoan)) +
                                              (Convert.ToDecimal(item.MCylLoan)) +
                                              (Convert.ToDecimal(item.BCylLoan)) +
                                              (Convert.ToDecimal(item.ComputerLoan)) +
                                              (Convert.ToDecimal(item.PFLoan)) +
                                              (Convert.ToDecimal(item.WPFLoan)) +
                                              (Convert.ToDecimal(item.CosLoan)) +
                                              (Convert.ToDecimal(item.OtherLoan));

                        // Calculate Net Pay
                        item.NetPay = item.GrossPay - item.TotalDeduction;

                        // Populate data in cells
                       // workSheet.Cell(row, 1).Value = item.MonthYear;
                        workSheet.Cell(row, 2).Value = item.JobCode;
                        workSheet.Cell(row, 3).Value = item.EmployeeName;
                        workSheet.Cell(row, 4).Value = item.DesignationName;
                        workSheet.Cell(row, 5).Value = item.DepartmentName;
                        workSheet.Cell(row, 6).Value = item.BasicSalary;
                        workSheet.Cell(row, 7).Value = item.PersonalSalary;
                        workSheet.Cell(row, 8).Value = item.ArrearSalary;
                        workSheet.Cell(row, 9).Value = item.OtherSalary;
                        workSheet.Cell(row, 10).Value = item.LunchAllow;
                        workSheet.Cell(row, 11).Value = item.TiffinAllow;
                        workSheet.Cell(row, 12).Value = item.HouseRentAllow;
                        workSheet.Cell(row, 13).Value = item.FamilyMedicalAllow;
                        workSheet.Cell(row, 14).Value = item.ShiftAllow;
                        workSheet.Cell(row, 15).Value = item.OtAllow;
                        workSheet.Cell(row, 16).Value = item.UtilityAllow;
                        workSheet.Cell(row, 17).Value = item.EducationAllowance;
                        workSheet.Cell(row, 18).Value = item.FuelAllow;
                        workSheet.Cell(row, 19).Value = item.OtherAllow;
                        workSheet.Cell(row, 20).Value = item.FieldAllow;
                        workSheet.Cell(row, 21).Value = item.ConvenienceAllow;
                        workSheet.Cell(row, 22).Value = item.SpecialBenefit;
                        workSheet.Cell(row, 23).Value = item.GrossPay;
                        workSheet.Cell(row, 24).Value = item.RevenueStamp;
                        workSheet.Cell(row, 25).Value = item.WelfareFund;
                        workSheet.Cell(row, 26).Value = item.EmployeeClub;
                        workSheet.Cell(row, 27).Value = item.EmployeeUnion;
                        workSheet.Cell(row, 28).Value = item.CPF;
                        workSheet.Cell(row, 29).Value = item.GPF;
                        workSheet.Cell(row, 30).Value = item.Dormitory;
                        workSheet.Cell(row, 31).Value = item.HospitalDeduction;
                        workSheet.Cell(row, 32).Value = item.FuelReturn;
                        workSheet.Cell(row, 33).Value = item.SpecialDeduction;
                        workSheet.Cell(row, 34).Value = item.Others;
                        workSheet.Cell(row, 35).Value = item.Advance;
                        workSheet.Cell(row, 36).Value = item.HBLoan;
                        workSheet.Cell(row, 37).Value = item.MCylLoan;
                        workSheet.Cell(row, 38).Value = item.BCylLoan;
                        workSheet.Cell(row, 39).Value = item.ComputerLoan;
                        workSheet.Cell(row, 40).Value = item.PFLoan;
                        workSheet.Cell(row, 41).Value = item.WPFLoan;
                        workSheet.Cell(row, 42).Value = item.CosLoan;
                        workSheet.Cell(row, 43).Value = item.OtherLoan;
                        workSheet.Cell(row, 44).Value = item.TotalDeduction;
                        workSheet.Cell(row, 45).Value = item.NetPay;
                        workSheet.Cell(row, 46).Value = item.PfFromCompany;
                        workSheet.Cell(row, 47).Value = item.PensionFromCompany;
                        workSheet.Cell(row, 48).Value = item.TotalCompanyLiabilities;
                        workSheet.Cell(row, 49).Value = item.UtilityReturn;

                        row++;
                    }

                    // Save the workbook
                    using (var stream = new MemoryStream())
                    {
                        workBook.SaveAs(stream);
                        string fileName = $"{currentMonth}_{currentYear}_Salary_Sheet.xlsx";
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }
                }

                else
                {
                    DateTime currentDate = DateTime.Now;
                    string printDate = currentDate.ToString("MMMM dd, yyyy");
                    var parameters = new[]
                    {
                    new Microsoft.Reporting.NETCore.ReportParameter("printDate",printDate.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("fmonth",fmonthName.ToString()),
                     new Microsoft.Reporting.NETCore.ReportParameter("tmonth",tmonthName.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("fyear",fyear.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("tyear",fyear.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("netPay",netPayInWords.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("totalCount",totalCount.ToString()),
                      };

                    string renderFormat = "PDF";
                    string mimtype = "application/pdf";
                    using var report = new Microsoft.Reporting.NETCore.LocalReport();
                    report.EnableExternalImages = true;
                    string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptSalaryControlSheetJSYearly.rdlc";
                    report.DataSources.Add(new ReportDataSource("dsSalaryControlSheetJS", data));
                    report.ReportPath = rptPath;
                    report.SetParameters(parameters);
                    var pdf = report.Render(renderFormat);
                    return File(pdf, mimtype);
                }

               

            }
        }


        //**************** Salary Bank Forwarding Report Start ****************





        //**************** Salary Bank Forwarding Officer Report Start ****************

        [HttpGet]
        public async Task<IActionResult> SalaryBankForward()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            var employeeType = await _employeeTypeContract.GetEmployeeTypes();
            var empTypeList = employeeType.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.EmployeeTypeName,
            }).ToList();
            ViewBag.employeeType = empTypeList;

            var bank = await _bankContract.GetBanks();
            var banklist = bank.Select(b => new SelectListItem
            {
                Value=b.Id.ToString(),
                Text=b.BankName,
            }).ToList();
            ViewBag.banks = banklist;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SalaryBankForward(int employeeType, int month,int year, string bank, int reportCopy)
        {
            if (employeeType == 1)
            {
               // Call GetSalaryCalculate to get the salary data
                var salaryResponse = await GetSalaryCalculateOF(employeeType, month, year, bank);

                string bankName = "";
                string bankBranch = "";
                decimal grandNetPay = salaryResponse.GrandNetPay;

                if (salaryResponse.IndividualSalaries != null && salaryResponse.IndividualSalaries.Count > 0)
                {
                    var firstItem = salaryResponse.IndividualSalaries.First();
                    bankName = firstItem.BankName; 
                    bankBranch = firstItem.BankBranchName; 
                }

                string netPayInWords = NumberToWordsConverter.Convert(grandNetPay);
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);

                var parameters = new[]
                {
                    new Microsoft.Reporting.NETCore.ReportParameter("month",monthName.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("year",year.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("bankName",bankName.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("bankAddress",bankBranch.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("netPay",netPayInWords.ToString()),
                };
                string renderFormat = "PDF";
                string mimtype = "application/pdf";
                using var report = new Microsoft.Reporting.NETCore.LocalReport();
                report.EnableExternalImages = true;
                string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptSalaryBankForwarding.rdlc";
                report.DataSources.Add(new ReportDataSource("dsBankForward", salaryResponse.IndividualSalaries));
                report.ReportPath = rptPath;
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                return File(pdf, mimtype);

            }
            else
            {
                // Call GetSalaryCalculate to get the salary data
                var salaryResponse = await GetSalaryCalculateJS(employeeType, month, year, bank);

                string bankName = "";
                string bankBranch = "";
                decimal grandNetPay = salaryResponse.GrandNetPay;

                if (salaryResponse.IndividualSalaries != null && salaryResponse.IndividualSalaries.Count > 0)
                {
                    var firstItem = salaryResponse.IndividualSalaries.First();
                    bankName = firstItem.BankName;
                    bankBranch = firstItem.BankBranchName;
                }

                string netPayInWords = NumberToWordsConverter.Convert(grandNetPay);
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);


                var parameters = new[]
               {
                    new Microsoft.Reporting.NETCore.ReportParameter("month",monthName.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("year",year.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("bankName",bankName.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("bankAddress",bankBranch.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("netPay",netPayInWords.ToString()),
                };
                string renderFormat = "PDF";
                string mimtype = "application/pdf";
                using var report = new Microsoft.Reporting.NETCore.LocalReport();
                report.EnableExternalImages = true;
                string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptSalaryBankForwarding.rdlc";
                report.DataSources.Add(new ReportDataSource("dsBankForward", salaryResponse.IndividualSalaries));
                report.ReportPath = rptPath;
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                return File(pdf, mimtype);

            }

        }

        //**************** Salary Bank Forwarding Report End ****************



        //**************** Salary Provident Fund Deduction  Report Start ****************
        [HttpGet]
        public async Task<IActionResult> GetPFDeductionOF()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            var employeeType = await _employeeTypeContract.GetEmployeeTypes();
            var empTypeList = employeeType.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.EmployeeTypeName,
            }).ToList();

            ViewBag.employeeType = empTypeList;

            var departments = await _departmentContract.GetDepartments();
            var departmentList = departments.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.DepartmentName
            }).ToList();

            ViewBag.Departments = departmentList;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetPFDeductionOF(int employeeType, int month,int year,int? department)
        {
            if (employeeType == 1)
            {
                int monthid = year * 100 + month;
                var source = await _salaryReportOfficerContract.GetSalaryReportOfficer(monthid, department);
                string mimetype = "";
                int extension = 1;
                var path = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptPFDeductionOF.rdlc";
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);

                var empType = "";
                if (employeeType == 1)
                {
                    empType = "Officer";
                }
                else
                {
                    empType = "Junior Staff";
                }
                

                var parameters = new[]
                {
                    new Microsoft.Reporting.NETCore.ReportParameter("month",monthName.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("empType",empType.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("year",year.ToString()),
                };
                string renderFormat = "PDF";
                string mimtype = "application/pdf";
                using var report = new Microsoft.Reporting.NETCore.LocalReport();
                report.EnableExternalImages = true;
                string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptPFDeductionOF.rdlc";
                report.DataSources.Add(new ReportDataSource("dsPFDeductionOF", source));
                report.ReportPath = rptPath;
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                return File(pdf, mimtype);


            }
            else
            {
                int monthid = year * 100 + month;
                var source = await _salaryReportOfficerContract.GetSalaryReportJS(monthid, department);
                string mimetype = "";
                int extension = 1;
                var path = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptPFDeductionOF.rdlc";
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);

                var empType = "";
                if (employeeType == 1)
                {
                    empType = "Officer";
                }
                else
                {
                    empType = "Junior Staff";
                }

                var parameters = new[]
                {
                    new Microsoft.Reporting.NETCore.ReportParameter("month",monthName.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("empType",empType.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("year",year.ToString()),
                };
                string renderFormat = "PDF";
                string mimtype = "application/pdf";
                using var report = new Microsoft.Reporting.NETCore.LocalReport();
                report.EnableExternalImages = true;
                string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptPFDeductionOF.rdlc";
                report.DataSources.Add(new ReportDataSource("dsPFDeductionOF", source));
                report.ReportPath = rptPath;
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                return File(pdf, mimtype);

            }
           
        }

        //**************** Salary Provident Fund Deduction  Report End ****************



        //**************** Salary Journal Report Start ****************
        [HttpGet]
        public async Task<IActionResult> GetSalaryJournal()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            var employeeType = await _employeeTypeContract.GetEmployeeTypes();
            var empTypeList = employeeType.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.EmployeeTypeName,
            }).ToList();

            ViewBag.employeeType = empTypeList;

            var departments = await _departmentContract.GetDepartments();
            var departmentList = departments.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.DepartmentName
            }).ToList();

            ViewBag.Departments = departmentList;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetSalaryJournal(int employeeType, int monthId, int bank, int reportCopy)
        {
            var source = await _salaryReportOfficerContract.GetSalaryReportOfficer(202405, employeeType);
            string mimetype = "";
            int extension = 1;
            var path = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptSalaryJournal.rdlc";
            //Dictionary<string, string> parameters = new Dictionary<string, string>();
            var parameters = new[]
            {
                new Microsoft.Reporting.NETCore.ReportParameter(),
                new Microsoft.Reporting.NETCore.ReportParameter(),
                new Microsoft.Reporting.NETCore.ReportParameter(),
            };
            string renderFormat = "PDF";
            string mimtype = "application/pdf";
            using var report = new Microsoft.Reporting.NETCore.LocalReport();
            report.EnableExternalImages = true;
            string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptSalaryJournal.rdlc";
            report.DataSources.Add(new ReportDataSource("dsSalaryJournal", source));
            report.ReportPath = rptPath;
            report.SetParameters(parameters);
            var pdf = report.Render(renderFormat);
            return File(pdf, mimtype);

        }

        //**************** Salary Journal Report End ****************


        //**************** Bank wise Summary Payment Report Start ****************
        [HttpGet]
        public async  Task<IActionResult> GetBankWisePaymentSummary()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            var employeeType = await _employeeTypeContract.GetEmployeeTypes();
            var empTypeList = employeeType.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.EmployeeTypeName,
            }).ToList();

            ViewBag.employeeType = empTypeList;

            var banks = await _bankContract.GetBanks();
            var banksList = banks.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.BankName,
            }).ToList();

            ViewBag.Banks = banksList;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetBankWisePaymentSummary(int employeeType, int month, int year, int bank, int reportCopy)
        {
            if (employeeType == 1)
            {
                int monthId = year * 100 + month;
                var bankWiseNetPay = await GetBankWiseNetPayOF(employeeType, month, year, null);

                // Calculate the grand net pay
                decimal grandNetPay = bankWiseNetPay.Sum(b => b.NetPay);

                string netPayInWords = NumberToWordsConverter.Convert(grandNetPay);
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
               

                var parameters = new[]
                {
                    new Microsoft.Reporting.NETCore.ReportParameter("month",monthName.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("netPay",netPayInWords.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("year",year.ToString()),
                };
                string renderFormat = "PDF";
                string mimtype = "application/pdf";
                using var report = new Microsoft.Reporting.NETCore.LocalReport();
                report.EnableExternalImages = true;
                string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptBankPaymentSummary.rdlc";
                report.DataSources.Add(new ReportDataSource("dsBankPaymentSummary", bankWiseNetPay));
                report.ReportPath = rptPath;
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                return File(pdf, mimtype);


            }
            else
            {
                int monthId = year * 100 + month;
                var bankWiseNetPay = await GetBankWiseNetPayJS(employeeType, month, year, null);

                // Calculate the grand net pay
                decimal grandNetPay = bankWiseNetPay.Sum(b => b.NetPay);

                string netPayInWords = NumberToWordsConverter.Convert(grandNetPay);
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
                Dictionary<string, string> parameters1 = new Dictionary<string, string>
                {
                    { "month", monthName },
                    { "year", year.ToString() },
                    { "netPay", netPayInWords }
                };

                var parameters = new[]
                {
                    new Microsoft.Reporting.NETCore.ReportParameter("month",monthName.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("netPay",netPayInWords.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("year",year.ToString()),
                };
                string renderFormat = "PDF";
                string mimtype = "application/pdf";
                using var report = new Microsoft.Reporting.NETCore.LocalReport();
                report.EnableExternalImages = true;
                string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptBankPaymentSummary.rdlc";
                report.DataSources.Add(new ReportDataSource("dsBankPaymentSummary", bankWiseNetPay));
                report.ReportPath = rptPath;
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                return File(pdf, mimtype);

            }
            
        }



        //**************** Bank wise Summary Payment Report End ****************


        //**************** Pay slip Report Start ****************
        [HttpGet]
        public async Task<IActionResult> GetSalaryPaySlip()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            var employeeType = await _employeeTypeContract.GetEmployeeTypes();
            var empTypeList = employeeType.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.EmployeeTypeName,
            }).ToList();

            ViewBag.employeeType = empTypeList;

            var departments = await _departmentContract.GetDepartments();
            var departmentList = departments.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.DepartmentName
            }).ToList();

            ViewBag.Departments = departmentList;
            var jobCode = await _employeeContract.GetEmployeeCode();
            var CodeList = jobCode.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.JobCode
            }).ToList();

            ViewBag.EmployeeCode = CodeList;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetSalaryPaySlip(string jobCode, int employeeTypeId, int year, int month,int department)
        {

            string monthsError = "";
            string yearsError = "";
            string employeeTypeIdError = "";
            string jobCodeError = "";

            // Validation checks
            if (month == 0)
            {
                monthsError = "Select a Month";
            }

            if (year == 0)
            {
                yearsError = "Select a Year";
            }

            if (employeeTypeId == 0)
            {
                employeeTypeIdError = "Select Employee Type";
            }

            if (string.IsNullOrEmpty(jobCode))
            {
                jobCodeError = "Select an Employee Code";
            }

            // Check if any errors exist
            if (monthsError.Length > 0 || yearsError.Length > 0 || employeeTypeIdError.Length > 0 || jobCodeError.Length > 0)
            {
                var departments = await _departmentContract.GetDepartments();
                var departmentList = departments.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.DepartmentName
                }).ToList();
                ViewBag.Departments = departmentList;

                var jobCodes = await _employeeContract.GetEmployeeCode();
                var CodeList = jobCodes.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.JobCode
                }).ToList();
                ViewBag.EmployeeCode = CodeList;

                var employeeType = await _employeeTypeContract.GetEmployeeTypes();
                var empTypeList = employeeType.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.EmployeeTypeName,
                }).ToList();

                ViewBag.employeeType = empTypeList;

                // Pass error messages to the view
                ViewBag.monthsError = monthsError;
                ViewBag.yearsError = yearsError;
                ViewBag.employeeTypeIdError = employeeTypeIdError;
                ViewBag.jobCodeError = jobCodeError;

                return View();
            }


            var empType = "";
            if (employeeTypeId == 1)
            {
                empType = "Officer";

            }
            else
            {
                empType = "Junior Staff";
            }

            if (employeeTypeId == 1)
            {
                int monthId = year * 100 + month;
                var source = await _salaryReportOfficerContract.GetSalaryPaySlip(jobCode, employeeTypeId, monthId, 0);
                var path = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptSalaryPaySlip.rdlc";

                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);


                var parameters = new[]
                {
                    new Microsoft.Reporting.NETCore.ReportParameter("month",monthName.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("empType",empType.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("year",year.ToString()),
                };




                string renderFormat = "PDF";
                string mimtype = "application/pdf";
                using var report = new Microsoft.Reporting.NETCore.LocalReport();
                report.EnableExternalImages = true;
                string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptSalaryPaySlip.rdlc";
                report.DataSources.Add(new ReportDataSource("dsSalaryPaySlip", source));
                report.ReportPath = rptPath;
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                return File(pdf, mimtype);
            }
            else
            {
                int monthId = year * 100 + month;
                var source = await _salaryReportOfficerContract.GetSalaryPaySlipJS(jobCode, employeeTypeId, monthId, 0);

                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);


                var parameters = new[]
                {
                    new Microsoft.Reporting.NETCore.ReportParameter("month",monthName.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("empType",empType.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("year",year.ToString()),
                };




                string renderFormat = "PDF";
                string mimtype = "application/pdf";
                using var report = new Microsoft.Reporting.NETCore.LocalReport();
                report.EnableExternalImages = true;
                string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptSalaryPaySlipJS.rdlc";
                report.DataSources.Add(new ReportDataSource("dsSalaryControlSheetJS", source));
                report.ReportPath = rptPath;
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                return File(pdf, mimtype);
            }
                
            
        }

        //****************Pay Slip Report End ****************


        //**************** Yearly  Pay slip Report Start ****************
        [HttpGet]
        public async Task<IActionResult> GetYearlyPaySlip()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            var jobCode = await _employeeContract.GetEmployeeCode();
            var CodeList = jobCode.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.JobCode
            }).ToList();

            ViewBag.EmployeeCode = CodeList;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetYearlyPaySlip(string jobCode, int EmployeeTypeId, int fmonth,int fyear,int tmonth,int tyear )

        {
            var fromMonthId = fyear * 100 + fmonth;
            var toMonthId= tyear * 100 + tmonth;
            var empType = "";
            if (EmployeeTypeId == 1)
            {
                empType = "Officer";

            }
            else
            {
                empType = "Junior Staff";
            }
            if (EmployeeTypeId == 1)
            {
                string fmonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(fmonth);
                string tmonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(tmonth);
                var source = await _salaryReportOfficerContract.GetSalaryYearPaySlip(jobCode, EmployeeTypeId, fromMonthId, toMonthId, 0);
                var parameters = new[]
                     {
                    new Microsoft.Reporting.NETCore.ReportParameter("fmonth",fmonthName.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("fyear",fyear.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("tmonth",tmonthName.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("tyear",tyear.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("empType",empType.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("jobCode",jobCode.ToString()),

                };
                string renderFormat = "PDF";
                string mimtype = "application/pdf";
                using var report = new Microsoft.Reporting.NETCore.LocalReport();
                report.EnableExternalImages = true;
                string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptYearlyPaySlip.rdlc";
                report.DataSources.Add(new ReportDataSource("dsYearlyPaySlip", source));
                report.ReportPath = rptPath;
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                return File(pdf, mimtype);
            }
            else
            {
                string fmonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(fmonth);
                string tmonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(tmonth);
                var source = await _salaryReportOfficerContract.GetSalaryYearPaySlipJS(jobCode, EmployeeTypeId, fromMonthId, toMonthId, 0);
                var parameters = new[]
                     {
                    new Microsoft.Reporting.NETCore.ReportParameter("fmonth",fmonthName.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("fyear",fyear.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("tmonth",tmonthName.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("tyear",tyear.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("empType",empType.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("jobCode",jobCode.ToString()),

                };
                string renderFormat = "PDF";
                string mimtype = "application/pdf";
                using var report = new Microsoft.Reporting.NETCore.LocalReport();
                report.EnableExternalImages = true;
                string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptYearlyPaySlipJS.rdlc";
                report.DataSources.Add(new ReportDataSource("dsYearlyPaySlipJS", source));
                report.ReportPath = rptPath;
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                return File(pdf, mimtype);
            }
            


        }

        //**************** Yearly Pay Slip Report End ****************


        // *********************************Salary Calculate OF ****************************
        public async Task<SalaryReportResponse> GetSalaryCalculateOF(int employeeType, int month, int year, string? bank)
        {
            int monthid = year * 100 + month;

            //var source = await _salaryReportOfficerContract.GetSalaryReportOfficer(monthid, department);
            var data= await _salaryReportOfficerContract.GetSalaryBankForwardOF(monthid, bank);
            // Initialize the response model
            var response = new SalaryReportResponse();

            // Calculate Gross Pay, Total Deduction, and Net Pay for each record
            foreach (var item in data)
            {
                var grossPay = (Convert.ToDecimal(item.BasicSalary)) +
                               (Convert.ToDecimal(item.PersonalSalary)) +
                               (Convert.ToDecimal(item.ArrearSalary)) +
                               (Convert.ToDecimal(item.LikeBasic)) +
                               (Convert.ToDecimal(item.OtherSalary)) +

                               (Convert.ToDecimal(item.LunchAllow)) +
                               (Convert.ToDecimal(item.TiffinAllow)) +
                               (Convert.ToDecimal(item.WashAllow)) +
                               (Convert.ToDecimal(item.HouseRentAllow)) +
                               (Convert.ToDecimal(item.Conveyance)) +
                               (Convert.ToDecimal(item.FMAllow)) +

                               (Convert.ToDecimal(item.EducationalAllow)) +
                               (Convert.ToDecimal(item.FieldRiskAllow)) +
                               (Convert.ToDecimal(item.ChargeAllow)) +
                               (Convert.ToDecimal(item.DAidAllow)) +
                               (Convert.ToDecimal(item.DeputationAllow)) +
                               (Convert.ToDecimal(item.OtherAllow));

                var totalDeduction = (Convert.ToDecimal(item.RevenueStamp)) +
                                     (Convert.ToDecimal(item.ProvidentFund)) +
                                     (Convert.ToDecimal(item.WelfareFund)) +
                                     (Convert.ToDecimal(item.OfficerClub)) +
                                     (Convert.ToDecimal(item.OfficerAssociation)) +
                                     (Convert.ToDecimal(item.MedicalFund)) +

                                     (Convert.ToDecimal(item.TMBill)) +
                                     (Convert.ToDecimal(item.Dormitory)) +
                                     (Convert.ToDecimal(item.Hospitalisation)) +
                                     (Convert.ToDecimal(item.HouseRentReturn)) +
                                     (Convert.ToDecimal(item.SpecialDeduction)) +
                                      (Convert.ToDecimal(item.FuelReturn)) +
                                     (Convert.ToDecimal(item.Advance)) +
                                     (Convert.ToDecimal(item.IncomeTax)) +
                                     (Convert.ToDecimal(item.Other));

                var netPay = grossPay - totalDeduction;

                // Add individual salary details to the response
                response.IndividualSalaries.Add(new SalaryReportItem
                {
                    JobCode = item.JobCode,
                    BankName = item.BankName,
                    BankBranchName = item.BankBranchName,
                    EmployeeName = item.EmployeeName,
                    AccountNumber = item.AccountNumber,
                    GrossPay = grossPay,
                    TotalDeduction = totalDeduction,
                    NetPay = netPay
                });

                // Accumulate grand totals
                response.GrandGrossPay += grossPay;
                response.GrandTotalDeduction += totalDeduction;
                response.GrandNetPay += netPay;
                response.TotalCount++;
            }

            return response;
        }


        // *********************************Salary Calculate JS ****************************
        public async Task<SalaryReportResponse> GetSalaryCalculateJS(int employeeType, int month, int year, string? bank)
        {
            int monthid = year * 100 + month;

            //var source = await _salaryReportOfficerContract.GetSalaryReportJS(monthid, department);
            var data = await _salaryReportOfficerContract.GetSalaryBankForwardJS(monthid, bank);
            // Initialize the response model
            var response = new SalaryReportResponse();

            // Calculate Gross Pay, Total Deduction, and Net Pay for each record
            foreach (var item in data)
            {
                var grossPay = (Convert.ToDecimal(item.BasicSalary)) +
                                    (Convert.ToDecimal(item.PersonalSalary)) +
                                    (Convert.ToDecimal(item.ArrearSalary)) +
                                    (Convert.ToDecimal(item.OtherSalary)) +
                                    (Convert.ToDecimal(item.LunchAllow)) +
                                    (Convert.ToDecimal(item.TiffinAllow)) +
                                    (Convert.ToDecimal(item.HouseRentAllow)) +
                                    (Convert.ToDecimal(item.FamilyMedicalAllow)) +
                                    (Convert.ToDecimal(item.ShiftAllow)) +
                                    (Convert.ToDecimal(item.OtAllow)) +
                                    (Convert.ToDecimal(item.UtilityAllow)) +
                                    (Convert.ToDecimal(item.EducationAllowance)) +
                                    (Convert.ToDecimal(item.FuelAllow)) +
                                    (Convert.ToDecimal(item.OtherAllow)) +
                                    (Convert.ToDecimal(item.FieldAllow)) +
                                    (Convert.ToDecimal(item.ConvenienceAllow));

                var totalDeduction = (Convert.ToDecimal(item.RevenueStamp)) +
                                          (Convert.ToDecimal(item.WelfareFund)) +
                                          (Convert.ToDecimal(item.EmployeeClub)) +
                                          (Convert.ToDecimal(item.EmployeeUnion)) +
                                          (Convert.ToDecimal(item.ProvidentFund)) +
                                          (Convert.ToDecimal(item.Dormitory)) +
                                          (Convert.ToDecimal(item.HospitalDeduction)) +
                                          (Convert.ToDecimal(item.FuelReturn)) +
                                          (Convert.ToDecimal(item.SpecialDeduction)) +
                                          (Convert.ToDecimal(item.Others)) +
                                          (Convert.ToDecimal(item.Advance));

                var netPay = grossPay - totalDeduction;

                // Add individual salary details to the response
                response.IndividualSalaries.Add(new SalaryReportItem
                {
                    JobCode = item.JobCode,
                    BankName = item.BankName,
                    BankBranchName = item.BankBranchName,
                    EmployeeName = item.EmployeeName,
                    AccountNumber = item.AccountNumber,
                    GrossPay = grossPay,
                    TotalDeduction = totalDeduction,
                    NetPay = netPay
                });

                // Accumulate grand totals
                response.GrandGrossPay += grossPay;
                response.GrandTotalDeduction += totalDeduction;
                response.GrandNetPay += netPay;
                response.TotalCount++;
            }

            return response;
        }



        // *********************************Bank Wise Net pay  ****************************
        public async Task<List<BankPaymentSummary>> GetBankWiseNetPayOF(int employeeType, int month, int year, int? department)
        {
            int monthid = year * 100 + month;

            var source = await _salaryReportOfficerContract.GetSalaryReportOfficer(monthid, department);

            // Initialize a list to hold the bank-wise payment summaries
            var bankWiseNetPay = new List<BankPaymentSummary>();

            // Calculate Gross Pay, Total Deduction, and Net Pay for each record
            foreach (var item in source)
            {
                var grossPay = (Convert.ToDecimal(item.BasicSalary)) +
                               (Convert.ToDecimal(item.PersonalSalary)) +
                               (Convert.ToDecimal(item.ArrearSalary)) +
                               (Convert.ToDecimal(item.LikeBasic)) +
                               (Convert.ToDecimal(item.OtherSalary)) +
                               (Convert.ToDecimal(item.LunchAllow)) +
                               (Convert.ToDecimal(item.TiffinAllow)) +
                               (Convert.ToDecimal(item.WashAllow)) +
                               (Convert.ToDecimal(item.HouseRentAllow)) +
                               (Convert.ToDecimal(item.Conveyance)) +
                               (Convert.ToDecimal(item.FMAllow)) +
                               (Convert.ToDecimal(item.EducationalAllow)) +
                               (Convert.ToDecimal(item.FieldRiskAllow)) +
                               (Convert.ToDecimal(item.ChargeAllow)) +
                               (Convert.ToDecimal(item.DAidAllow)) +
                               (Convert.ToDecimal(item.DeputationAllow)) +
                               (Convert.ToDecimal(item.OtherAllow));

                var totalDeduction = (Convert.ToDecimal(item.RevenueStamp)) +
                                     (Convert.ToDecimal(item.ProvidentFund)) +
                                     (Convert.ToDecimal(item.WelfareFund)) +
                                     (Convert.ToDecimal(item.OfficerClub)) +
                                     (Convert.ToDecimal(item.OfficerAssociation)) +
                                     (Convert.ToDecimal(item.MedicalFund)) +
                                     (Convert.ToDecimal(item.TMBill)) +
                                     (Convert.ToDecimal(item.Dormitory)) +
                                     (Convert.ToDecimal(item.Hospitalisation)) +
                                     (Convert.ToDecimal(item.HouseRentReturn)) +
                                     (Convert.ToDecimal(item.SpecialDeduction)) +
                                     (Convert.ToDecimal(item.FuelReturn)) +
                                     (Convert.ToDecimal(item.Advance)) +
                                     (Convert.ToDecimal(item.IncomeTax)) +
                                     (Convert.ToDecimal(item.Other));

                var netPay = grossPay - totalDeduction;

                // Get bank details
                var bankName = item.BankName;
                var bankAddress = item.BankBranchName;
                var bankBranch = item.BankBranchName;

                // Accumulate the net pay for each bank
                var bankSummary = bankWiseNetPay.FirstOrDefault(b => b.BankName == bankName);
                if (bankSummary != null)
                {
                    bankSummary.NetPay += netPay;
                }
                else
                {
                    bankWiseNetPay.Add(new BankPaymentSummary
                    {
                        BankName = bankName,
                        BankAddress = bankAddress,
                        BankBranch = bankBranch,
                        NetPay = netPay
                    });
                }
            }

            return bankWiseNetPay;
        }

        public async Task<List<BankPaymentSummary>> GetBankWiseNetPayJS(int employeeType, int month, int year, int? department)
        {
            int monthid = year * 100 + month;

            var source = await _salaryReportOfficerContract.GetSalaryReportJS(monthid, department);

            // Initialize a list to hold the bank-wise payment summaries
            var bankWiseNetPay = new List<BankPaymentSummary>();

            // Calculate Gross Pay, Total Deduction, and Net Pay for each record
            foreach (var item in source)
            {
                var grossPay = (Convert.ToDecimal(item.BasicSalary)) +
                                    (Convert.ToDecimal(item.PersonalSalary)) +
                                    (Convert.ToDecimal(item.ArrearSalary)) +
                                    (Convert.ToDecimal(item.OtherSalary)) +
                                    (Convert.ToDecimal(item.LunchAllow)) +
                                    (Convert.ToDecimal(item.TiffinAllow)) +
                                    (Convert.ToDecimal(item.HouseRentAllow)) +
                                    (Convert.ToDecimal(item.FamilyMedicalAllow)) +
                                    (Convert.ToDecimal(item.ShiftAllow)) +
                                    (Convert.ToDecimal(item.OtAllow)) +
                                    (Convert.ToDecimal(item.UtilityAllow)) +
                                    (Convert.ToDecimal(item.EducationAllowance)) +
                                    (Convert.ToDecimal(item.FuelAllow)) +
                                    (Convert.ToDecimal(item.OtherAllow)) +
                                    (Convert.ToDecimal(item.FieldAllow)) +
                                    (Convert.ToDecimal(item.ConvenienceAllow));

                var totalDeduction = (Convert.ToDecimal(item.RevenueStamp)) +
                                          (Convert.ToDecimal(item.WelfareFund)) +
                                          (Convert.ToDecimal(item.EmployeeClub)) +
                                          (Convert.ToDecimal(item.EmployeeUnion)) +
                                          (Convert.ToDecimal(item.ProvidentFund)) +
                                          (Convert.ToDecimal(item.Dormitory)) +
                                          (Convert.ToDecimal(item.HospitalDeduction)) +
                                          (Convert.ToDecimal(item.FuelReturn)) +
                                          (Convert.ToDecimal(item.SpecialDeduction)) +
                                          (Convert.ToDecimal(item.Others)) +
                                          (Convert.ToDecimal(item.Advance));

                var netPay = grossPay - totalDeduction;

                // Get bank details
                var bankName = item.BankName;
                var bankBranch = item.BankBranchName;
                var bankAddress = "";

                // Accumulate the net pay for each bank
                var bankSummary = bankWiseNetPay.FirstOrDefault(b => b.BankName == bankName);
                if (bankSummary != null)
                {
                    bankSummary.NetPay += netPay;
                }
                else
                {
                    bankWiseNetPay.Add(new BankPaymentSummary
                    {
                        BankName = bankName,
                        BankAddress = bankAddress,
                        BankBranch = bankBranch,
                        NetPay = netPay
                    });
                }
            }

            return bankWiseNetPay;
        }
    }
}
