using AspNetCore.Reporting;
using ClosedXML.Excel;
using Contracts;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Reporting.NETCore;
using System.Drawing.Imaging;
using System.Globalization;
using System.Reflection.Emit;

namespace BgfclApp.Controllers.Report
{
    public class AmenitiesReportController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private ISalarySettingContract _salarySettingContract;
        private ISalaryReportOfficerContract _salaryReportOfficerContract;
        private IDepartmentContract _departmentContract;
        private IAmenitiesReportContract _amenitiesReportContract;
        private IBankContract _bankContract;
        private IEmployeeTypeContract _employeeTypeContract;
        private IAmenitiesContract _amenitiesContract;
        private IEmployeeContract _employeeContract;
        public AmenitiesReportController(IWebHostEnvironment webHostEnvironment,
                                        ISalarySettingContract salarySettingContract,
                                        ISalaryReportOfficerContract salaryReportOfficerContract,
                                        IDepartmentContract departmentContract,
                                        IAmenitiesReportContract amenitiesReportContract,
                                        IBankContract bankContract,
                                        IEmployeeTypeContract employeeTypeContract,
                                        IAmenitiesContract amenitiesContract,
                                        IEmployeeContract employeeContract)
        {

            _webHostEnvironment = webHostEnvironment;
            _salarySettingContract = salarySettingContract;
            _salaryReportOfficerContract = salaryReportOfficerContract;
            _departmentContract = departmentContract;
            _amenitiesReportContract = amenitiesReportContract;
            _bankContract = bankContract;
            _employeeTypeContract = employeeTypeContract;
            _amenitiesContract = amenitiesContract;
            _employeeContract = employeeContract;
        }
        public async Task <IActionResult> Index()
        {
            
            return View();
        }

        //**************** Amenities  Control Sheet Report Start ****************
        public async Task<IActionResult> GetAmenitiesControlSheet()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
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
        public async Task<IActionResult> GetAmenitiesControlSheet(List<string> jobCodes, int year,int month)
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
           

            if (monthsError.Length > 0 || yearsError.Length > 0 || employeeTypeIdError.Length > 0)
            {
                var departments = await _departmentContract.GetDepartments();
                var departmentList = departments.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.DepartmentName
                }).ToList();

                ViewBag.Departments = departmentList;
                ViewBag.monthsError = monthsError;
                ViewBag.yearsError = yearsError;
                return View();
            }

            var monthid = year * 100 + month;
            var source = await _amenitiesContract.GetAmenitiesControlSheet(jobCodes,monthid);



            // Calculate the grand total
            decimal grandTotal = source.Sum(item =>
             ((Convert.ToDecimal(item.WageGMSCB)) +
             (Convert.ToDecimal(item.HouseKeepUp)) +
             (Convert.ToDecimal(item.FuelSubsidy)) +
             (Convert.ToDecimal(item.WESubsidy)) +
             (Convert.ToDecimal(item.OtherPay))) -
             ((Convert.ToDecimal(item.RevenueStamp)) +
             (Convert.ToDecimal(item.OtherDeduction)))
            );
            // Convert the grand total to words
            //string totalInWords = NumberToWords(grandTotal);
            string totalInWords = NumberToWordsConverter.Convert(grandTotal);
            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
            DateTime currentDate = DateTime.Now;
            string printDate = currentDate.ToString("MMMM dd, yyyy");
            var parameters = new[]
            {
                new ReportParameter("printDate",printDate.ToString()),
                new ReportParameter("month",monthName.ToString()),
                new ReportParameter("year",year.ToString()),
                new ReportParameter("totalInWords",totalInWords.ToString()),
            };
            string renderFormat = "PDF";
            string mimtype = "application/pdf";
            using var report = new Microsoft.Reporting.NETCore.LocalReport();
            report.EnableExternalImages = true;
            string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptAmenitiesControlSheet.rdlc";
            report.DataSources.Add(new ReportDataSource("dsAmenitiesControlSheet", source));
            report.ReportPath = rptPath;
            report.SetParameters(parameters);
            var pdf = report.Render(renderFormat);
            return File(pdf, mimtype);

        }

        [HttpGet]
        public async Task<IActionResult> YearlyAmenitiesControlSheet()
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
        public async Task<IActionResult> YearlyAmenitiesControlSheet(List<string> jobCodes,int fmonth, int fyear, int tmonth, int tyear, string department,string isExcel)
        {
            

            var fmonthid = fyear * 100 + fmonth;
            var tmonthid= tyear * 100 + tmonth;
            var source = await _amenitiesContract.YearlyAmenitiesControlSheet(jobCodes,fmonthid, tmonthid, department);



            // Calculate the grand total
            decimal grandTotal = source.Sum(item =>
             ((Convert.ToDecimal(item.WageGMSCB)) +
             (Convert.ToDecimal(item.HouseKeepUp)) +
             (Convert.ToDecimal(item.FuelSubsidy)) +
             (Convert.ToDecimal(item.WESubsidy)) +
             (Convert.ToDecimal(item.OtherPay))) -
             ((Convert.ToDecimal(item.RevenueStamp)) +
             (Convert.ToDecimal(item.OtherDeduction)))
            );
            // Convert the grand total to words
            string totalInWords = NumberToWordsConverter.Convert(grandTotal);

            string mimetype = "";
            int extension = 1;
            string fmonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(fmonth);
            string tmonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(tmonth);

            DateTime now = DateTime.Now;
            int currentMonth = now.Month;
            int currentYear = now.Year;
            if (!String.IsNullOrEmpty(isExcel))
            {
                var workBook = new XLWorkbook();
                var workSheet = workBook.Worksheets.Add("Sheet1");

                workSheet.Cell(1, 1).Value = "Month Year";
                workSheet.Cell(1, 2).Value = "JobCode";
                workSheet.Cell(1, 3).Value = "EmployeeName";
                workSheet.Cell(1, 4).Value = "Designation";
                workSheet.Cell(1, 5).Value = "Department";
                workSheet.Cell(1, 6).Value = "AccountNumber";
                workSheet.Cell(1, 7).Value = "Bank Name";
                workSheet.Cell(1, 8).Value = "Bank Branch";

                workSheet.Cell(1, 9).Value = "Guard/Mali";
                workSheet.Cell(1, 10).Value = "House Up Keep";
                workSheet.Cell(1, 11).Value = "Fuel Subsidy";
                workSheet.Cell(1, 12).Value = "Water Subsidy";
                workSheet.Cell(1, 13).Value = "Other Pay";
                workSheet.Cell(1, 14).Value = "Gross Pay";
                workSheet.Cell(1, 15).Value = "Other Deduction";
                workSheet.Cell(1, 16).Value = "Revenue Stamp";
                workSheet.Cell(1, 17).Value = "Total Deduction";
                workSheet.Cell(1, 18).Value = "Net Pay";
                workSheet.Cell(1, 19).Value = "Journal Code";

                int row = 2;

                foreach (var item in source)
                {

                    decimal grossTotal =
                    ((Convert.ToDecimal(item.WageGMSCB)) +
                    (Convert.ToDecimal(item.HouseKeepUp)) +
                    (Convert.ToDecimal(item.FuelSubsidy)) +
                    (Convert.ToDecimal(item.WESubsidy))+
                    (Convert.ToDecimal(item.OtherPay)));


                    decimal totalDeduction =
                         ((Convert.ToDecimal(item.RevenueStamp)) +
                         (Convert.ToDecimal(item.OtherDeduction)));

                    decimal netPay = grossTotal - totalDeduction;
                    //workSheet.Cell(row, 1).Value = item.MonthYear;
                    workSheet.Cell(row, 2).Value = item.JobCode;
                    workSheet.Cell(row, 3).Value = item.EmployeeName;
                    workSheet.Cell(row, 4).Value = item.DesignationName;
                    workSheet.Cell(row, 5).Value = item.DepartmentName;
                    workSheet.Cell(row, 6).Value = item.AccountNumber;     
                    workSheet.Cell(row, 7).Value = item.BankName;
                    workSheet.Cell(row, 8).Value = item.BankBranchName;
                    workSheet.Cell(row, 9).Value = item.WageGMSCB;
                    workSheet.Cell(row, 10).Value = item.HouseKeepUp;
                    workSheet.Cell(row, 11).Value = item.FuelSubsidy;
                    workSheet.Cell(row, 12).Value = item.WESubsidy;
                    workSheet.Cell(row, 13).Value = item.OtherPay;
                    workSheet.Cell(row, 14).Value = grossTotal;
                    workSheet.Cell(row, 15).Value = item.OtherDeduction;
                    workSheet.Cell(row, 16).Value = item.RevenueStamp;
                    workSheet.Cell(row, 17).Value = totalDeduction;
                    workSheet.Cell(row, 18).Value = netPay;
                    workSheet.Cell(row, 19).Value = item.JournalCode;
                    row++;
                }
                using (var stream = new MemoryStream())
                {
                    workBook.SaveAs(stream);
                    string fileName = $"{currentMonth}_{currentYear}_Yearly_Amenities_Sheet.xlsx";
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
            else
            {
                DateTime currentDate = DateTime.Now;
                string printDate = currentDate.ToString("MMMM dd, yyyy");
                var parameters = new[]
                {
                    new ReportParameter("printDate",printDate.ToString()),
                    new ReportParameter("fmonth",fmonthName.ToString()),
                    new ReportParameter("tmonth",tmonthName.ToString()),
                    new ReportParameter("fyear",fyear.ToString()),
                    new ReportParameter("tyear",tyear.ToString()),
                    new ReportParameter("totalInWords",totalInWords.ToString()),
                };
                string renderFormat = "PDF";
                string mimtype = "application/pdf";
                using var report = new Microsoft.Reporting.NETCore.LocalReport();
                report.EnableExternalImages = true;
                string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptAmenitiesControlSheetYearly.rdlc";
                report.DataSources.Add(new ReportDataSource("dsAmenitiesControlSheet", source));
                report.ReportPath = rptPath;
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                return File(pdf, mimtype);
            }
        }

        //**************** Amenities  Control Sheet Report End ****************


        //**************** Amenities  Bank Forward Report Start ****************
        public async Task<IActionResult> GetAmenitiesBankForward()
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
                Value = b.Id.ToString(),
                Text = b.BankName,
            }).ToList();
            ViewBag.banks = banklist;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetAmenitiesBankForward(int year,int month, int bank)
        {

            string monthsError = "";
            string yearsError = "";
            string bankError = "";
            if (month == 0)
            {
                monthsError = "Select a Month";

            }
            if (year == 0)
            {
                yearsError = "Select a Year";
            }
            

            if (monthsError.Length > 0 || yearsError.Length > 0)
            {
                var banks = await _bankContract.GetBanks();
                var banklist = banks.Select(b => new SelectListItem
                {
                    Value = b.Id.ToString(),
                    Text = b.BankName,
                }).ToList();
                ViewBag.banks = banklist;

                ViewBag.monthsError = monthsError;
                ViewBag.yearsError = yearsError;
                ViewBag.bankError = bankError;
                return View();
            }


            var monthid = year * 100 + month    ;
            var amenitiesFinalAdjustments = await _amenitiesContract.GetAmenitiesByMonthIdAndBankId(monthid, bank);
            var bankName = amenitiesFinalAdjustments.FirstOrDefault()?.BankName;
            if (bank == 0)
            {
                bankName = "";
            }
            double grandNetPay = 0;

            // Calculate grand net pay
            foreach (var item in amenitiesFinalAdjustments)
            {
                double netPay = (item.WageGMSCB + item.HouseKeepUp + item.WESubsidy  + item.FuelSubsidy + item.OtherPay)  - (item.RevenueStamp + item.OtherDeduction);

                grandNetPay += netPay;
            }

            string netPayInWords = NumberToWordsConverter.Convert(Convert.ToDecimal(grandNetPay));
            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
            var path = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptAmeBankForward.rdlc";
            DateTime currentDate = DateTime.Now;
            string printDate = currentDate.ToString("MMMM dd, yyyy");

            var parameters = new[]
               {
                    new ReportParameter("printDate",printDate.ToString()),
                    new ReportParameter("bank",bankName?.ToString()),
                    new ReportParameter("month",monthName.ToString()),
                    new ReportParameter("year",year.ToString()),
                    new ReportParameter("grandNetPay",grandNetPay.ToString("N2")),
                    new ReportParameter("netPayInWords",netPayInWords.ToString()),
                };
            string renderFormat = "PDF";
            string mimtype = "application/pdf";
            using var report = new Microsoft.Reporting.NETCore.LocalReport();
            report.EnableExternalImages = true;
            string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptAmeBankForward.rdlc";
            report.DataSources.Add(new ReportDataSource("dsAmeBankForward", amenitiesFinalAdjustments));
            report.ReportPath = rptPath;
            report.SetParameters(parameters);
            var pdf = report.Render(renderFormat);
            return File(pdf, mimtype);

        }
        //**************** Amenities  Bank Forward Report End ****************


        //**************** Amenities  Journal Report Start ****************
        public IActionResult GetAmenitiesJournal()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetAmenitiesJournal(int year,int month, int employeeTypeId)
        {
            int monthid = year * 100 + month;
            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
            var empType = "";
            if (employeeTypeId == 1)
            {
                empType = "Officer";
            }
            else
            {
                empType = "Junior Staff";
            }
            var monthnum = "";
            if (monthName == "January")
            {
                monthnum = "7";
            }
            if (monthName == "February")
            {
                monthnum = "8";
            }
            if (monthName == "March")
            {
                monthnum = "9";
            }
            if (monthName == "April")
            {
                monthnum = "10";
            }
            if (monthName == "May")
            {
                monthnum = "11";
            }
            if (monthName == "June")
            {
                monthnum = "12";
            }
            if (monthName == "July")
            {
                monthnum = "1";
            }
            if (monthName == "August")
            {
                monthnum = "2";
            }
            if (monthName == "September")
            {
                monthnum = "3";
            }
            if (monthName == "October")
            {
                monthnum = "4";
            }
            if (monthName == "November")
            {
                monthnum = "5";
            }
            if (monthName == "December")
            {
                monthnum = "6";
            }
            List<AmenitiesJournal> AmenitiesJournals = new List<AmenitiesJournal>();
            var journalMasterData = await _amenitiesContract.GetAmenitiesJournalMaster(monthid, employeeTypeId);
            if (journalMasterData != null)
            {
                AmenitiesJournals.AddRange(await _amenitiesContract.GetAmenitiesJournalsByMasterId(journalMasterData.Id));
            }
            DateTime currentDate = DateTime.Now;
            string printDate = currentDate.ToString("MMMM dd, yyyy");
            var parameters = new[]
            {
                new ReportParameter("printDate",printDate.ToString()),
                new ReportParameter("monthnum",monthnum.ToString()),
                new ReportParameter("empType",empType.ToString()),
                new ReportParameter("month",monthName.ToString()),
                new ReportParameter("year",year.ToString()),

            };

            string renderFormat = "PDF";
            string mimtype = "application/pdf";
            using var report = new Microsoft.Reporting.NETCore.LocalReport();
            report.EnableExternalImages = true;
            string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptAmeJournal.rdlc";
            report.DataSources.Add(new ReportDataSource("dsAmeJournal", AmenitiesJournals));
            report.ReportPath = rptPath;
            report.SetParameters(parameters);
            var pdf = report.Render(renderFormat);
            return File(pdf, mimtype);

        }

        [HttpGet]
        public async Task<IActionResult> AmenitiesPaySlip()
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
        public async Task<IActionResult> AmenitiesPaySlip(string jobCode, int year, int month, int department)
        {
           
                int monthId = year * 100 + month;
                var source = await _amenitiesContract.GeAmenitiesPaySlip(jobCode, monthId, 0);

                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);

            DateTime currentDate = DateTime.Now;
            string printDate = currentDate.ToString("MMMM dd, yyyy");
            var parameters = new[]
                {
                    new Microsoft.Reporting.NETCore.ReportParameter("printDate",printDate.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("month",monthName.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("year",year.ToString()),
                };

                string renderFormat = "PDF";
                string mimtype = "application/pdf";
                using var report = new Microsoft.Reporting.NETCore.LocalReport();
                report.EnableExternalImages = true;
                string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptAmenitiesPaySlip.rdlc";
                report.DataSources.Add(new ReportDataSource("dsAmenitiesControlSheet", source));
                report.ReportPath = rptPath;
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                return File(pdf, mimtype);
   
        }

        [HttpGet]
        public async Task<IActionResult> AmenitiesYearlyPayslip()
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
            var departments = await _departmentContract.GetDepartments();
            var departmentList = departments.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.DepartmentName
            }).ToList();

            ViewBag.Departments = departmentList;
            return View();
        }

        public async Task<IActionResult> AmenitiesYearlyPayslip(string jobCode,int fmonth, int fyear, int tmonth, int tyear,string department)
        {
            var fromMonthId = fyear * 100 + fmonth;
            var toMonthId = tyear * 100 + tmonth;
           string fmonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(fmonth);
            string tmonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(tmonth);

           // string fmonthName = new DateTime(fyear, fmonth, 1).ToString("MMMM");
           // string tmonthName = new DateTime(tyear, tmonth, 1).ToString("MMMM");

            var source = await _amenitiesContract.YearlyAmenitiesPaySlip(jobCode, fromMonthId, toMonthId, department);
            DateTime currentDate = DateTime.Now;
            string printDate = currentDate.ToString("MMMM dd, yyyy");
            var parameters = new[]
                 {
                    new Microsoft.Reporting.NETCore.ReportParameter("printDate",printDate.ToString()),
                     new Microsoft.Reporting.NETCore.ReportParameter("fmonth",fmonthName.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("fyear",fyear.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("tmonth",tmonthName.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("tyear",tyear.ToString()),
                    //new Microsoft.Reporting.NETCore.ReportParameter("jobCode",jobCode.ToString()),

                };
            string renderFormat = "PDF";
            string mimtype = "application/pdf";
            using var report = new Microsoft.Reporting.NETCore.LocalReport();
            report.EnableExternalImages = true;
            string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptAmenitiesPaySlipYearly.rdlc";
            report.DataSources.Add(new ReportDataSource("dsAmenitiesControlSheet", source));
            report.ReportPath = rptPath;
            report.SetParameters(parameters);
            var pdf = report.Render(renderFormat);
            return File(pdf, mimtype);
        }

        //**************** Amenities  Journal Report End ****************


        public string NumberToWords(decimal number)
        {
            if (number == 0) return "Zero";

            string words = "";
            long integerPart = (long)Math.Floor(number);
            long fractionPart = (long)((number - integerPart) * 100);

            if (integerPart > 0)
            {
                words = ConvertWholeNumber(integerPart);
            }

            if (fractionPart > 0)
            {
                words += " and " + ConvertWholeNumber(fractionPart) + " paisa";
            }

            return words.Trim();
        }

        public string ConvertWholeNumber(long number)
        {
            string[] unitsMap = { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
            string[] tensMap = { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

            if (number < 20)
                return unitsMap[number];
            else if (number < 100)
                return tensMap[number / 10] + ((number % 10 > 0) ? "-" + unitsMap[number % 10] : "");
            else if (number < 1000)
                return unitsMap[number / 100] + " Hundred" + ((number % 100 > 0) ? " and " + ConvertWholeNumber(number % 100) : "");
            else if (number < 1000000)
                return ConvertWholeNumber(number / 1000) + " Thousand" + ((number % 1000 > 0) ? " " + ConvertWholeNumber(number % 1000) : "");
            else if (number < 1000000000)
                return ConvertWholeNumber(number / 1000000) + " Million" + ((number % 1000000 > 0) ? " " + ConvertWholeNumber(number % 1000000) : "");
            else
                return ConvertWholeNumber(number / 1000000000) + " Billion" + ((number % 1000000000 > 0) ? " " + ConvertWholeNumber(number % 1000000000) : "");
        }
    }
}
