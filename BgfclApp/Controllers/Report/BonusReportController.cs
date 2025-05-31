using AspNetCore.Reporting;
using Contracts;
using DocumentFormat.OpenXml.Bibliography;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Reporting.NETCore;
using System.Globalization;

namespace BgfclApp.Controllers.Report
{
    public class BonusReportController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private IBonusSheetContract _bonusSheetContract;
        private IBonusContract _bonusContract;
        private IBankContract _bankContract;
        private IDepartmentContract _departmentContract;
        private IEmployeeTypeContract _employeeTypeContract;
        private ISalaryReportOfficerContract _salaryReportOfficerContract;
        private IEmployeeContract _employeeContract;
        public BonusReportController(IWebHostEnvironment webHostEnvironment, ISalarySettingContract salarySettingContract,
                                     ISalaryReportOfficerContract salaryReportOfficerContract,
                                     IBonusSheetContract bonussheetContract,
                                     IEmployeeTypeContract employeeTypeContract,
                                     IDepartmentContract departmentContract,
                                     IBonusContract bonusContract,
                                     IBankContract bankContract,
                                     IEmployeeContract employeeContract
                                     )
        {

            _webHostEnvironment = webHostEnvironment;
            _bonusSheetContract = bonussheetContract;
            _salaryReportOfficerContract = salaryReportOfficerContract;
            _employeeTypeContract = employeeTypeContract;
            _departmentContract = departmentContract;
            _bonusContract = bonusContract;
            _bankContract = bankContract;
            _employeeContract = employeeContract;
        }
        public IActionResult Index()
        {
            return View();
        }

        //**************** Bonus Control Sheet Report Start ****************
        [HttpGet]
        public async Task<IActionResult> GetBonusSheet()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetBonusSheet(int year, int month, int bonusId, int employeeTypeId, int reportType )
        {

            string monthsError = "";
            string yearsError = "";
            string employeeTypeIdError = "";
            string bonusError = "";
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
            if (bonusId == 0)
            {
                bonusError = "Select bonus field";
            }

            if (monthsError.Length > 0 || yearsError.Length > 0 || employeeTypeIdError.Length > 0 || bonusError.Length > 0)
            {
                ViewBag.monthsError = monthsError;
                ViewBag.yearsError = yearsError;
                ViewBag.employeeTypeIdError = employeeTypeIdError;
                ViewBag.bonusError = bonusError;
                return View();
            }

            int monthid = year * 100 + month;
            var source = await _bonusContract.GetBonusControlSheet(monthid, bonusId, employeeTypeId, 0);
            var bonusTypes = await _bonusContract.GetBonus();
            var bonusType = "";
            if (bonusId == 1)
            {
                bonusType = "Festival Bonus (Eidul Fitar)";
            }
            else if (bonusId == 2)
            {
                bonusType = "Festival Bonus (Eidul Azha)";
            }
            else if (bonusId == 3)
            {
                bonusType = "Baishakhi Allowance";
            }
            else if (bonusId == 4)
            {
                bonusType = "Dress Allowance";
            }
            else
            {
                bonusType = "All";
            }


            // Calculate the grand total
            double grandTotal = source.Sum(item =>
             (item.FestivalBonus + item.IncentiveBonus + item.HonorariumBonus + item.ScholarshipBonus) - (item.RevStamp + item.Deduction)
            );

            // Convert the grand total to words
            //string totalInWords = NumberToWords(Convert.ToDecimal(grandTotal));
            string totalInWords = NumberToWordsConverter.Convert((decimal)grandTotal);



            var EmpType = "";
            if (employeeTypeId == 1)
            {
                EmpType = "Permanent";
            }
            if (employeeTypeId == 2)
            {
                EmpType = "Contract";
            }
            if (employeeTypeId == 3)
            {
                EmpType = "Daily Worker";
            }
            if (reportType == 1)
            {
               
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
                DateTime currentDate = DateTime.Now;
                string printDate = currentDate.ToString("M/d/yy, h:mm tt");
                var parameters = new[]
                {
                    new ReportParameter("EmpType",EmpType.ToString()),
                    new ReportParameter("printDate",printDate.ToString()),
                    new ReportParameter("month",monthName.ToString()),
                    new ReportParameter("year",year.ToString()),
                    new ReportParameter("bonusType",bonusType.ToString()),
                    new ReportParameter("totalInWords",totalInWords.ToString()),
                };
                if (employeeTypeId == 1)
                {
                    string renderFormat = "PDF";
                    string mimtype = "application/pdf";
                    using var report = new Microsoft.Reporting.NETCore.LocalReport();
                    report.EnableExternalImages = true;
                    string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\Rpt_Bonus_Sheet_Office.rdlc";
                    report.DataSources.Add(new ReportDataSource("dsBonusControlSheet", source));
                    report.ReportPath = rptPath;
                    report.SetParameters(parameters);
                    var pdf = report.Render(renderFormat);
                    return File(pdf, mimtype);

                 
                }
                else
                {
                    string renderFormat = "PDF";
                    string mimtype = "application/pdf";
                    using var report = new Microsoft.Reporting.NETCore.LocalReport();
                    report.EnableExternalImages = true;
                    string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\Rpt_Bonus_Sheet_Office _Contract.rdlc";
                    report.DataSources.Add(new ReportDataSource("dsBonusControlSheet", source));
                    report.ReportPath = rptPath;
                    report.SetParameters(parameters);
                    var pdf = report.Render(renderFormat);
                    return File(pdf, mimtype);
                }
                
            }
            else
            {
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
                DateTime currentDate = DateTime.Now;
                string printDate = currentDate.ToString("M/d/yy, h:mm tt");
                var parameters = new[]
                {
                    new ReportParameter("EmpType",EmpType.ToString()),
                    new ReportParameter("printDate",printDate.ToString()),
                    new ReportParameter("month",monthName.ToString()),
                    new ReportParameter("year",year.ToString()),
                    new ReportParameter("bonusType",bonusType.ToString()),
                    new ReportParameter("totalInWords",totalInWords.ToString()),
                };
                string renderFormat = "PDF";
                string mimtype = "application/pdf";
                using var report = new Microsoft.Reporting.NETCore.LocalReport();
                report.EnableExternalImages = true;
                string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\Rpt_Bonus_Sheet_Bank_Copy.rdlc";
                report.DataSources.Add(new ReportDataSource("dsBonusControlSheet", source));
                report.ReportPath = rptPath;
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                return File(pdf, mimtype);
            }
            

        }
        public async Task<IActionResult> GetBonusControlSheet()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetBonusControlSheet(int year, int month, int bonusId, int employeeTypeId, int department = 0)
        {

            string monthsError = "";
            string yearsError = "";
            string employeeTypeIdError = "";
            string bonusError = "";
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
            if (bonusId == 0)
            {
                bonusError = "Select bonus field";
            }

            if (monthsError.Length > 0 || yearsError.Length > 0 || employeeTypeIdError.Length > 0 || bonusError.Length > 0)
            {
                ViewBag.monthsError = monthsError;
                ViewBag.yearsError = yearsError;
                ViewBag.employeeTypeIdError = employeeTypeIdError;
                ViewBag.bonusError = bonusError;
                return View();
            }

            int monthid = year * 100 + month;
            var source = await _bonusContract.GetBonusControlSheet(monthid, bonusId, employeeTypeId, department);
            var bonusTypes = await _bonusContract.GetBonus();
            var bonusType = "";
            if (bonusId == 1)
            {
                bonusType = "Festival Bonus";
            }
            else if (bonusId == 2)
            {
                bonusType = "Incentive Bonus";
            }
            else if (bonusId == 3)
            {
                bonusType = "Honorarium Bonus";
            }
            else if (bonusId == 4)
            {
                bonusType = "Scholarship Bonus";
            }
            else
            {
                bonusType = "All";
            }


            // Calculate the grand total
            double grandTotal = source.Sum(item =>
             (item.FestivalBonus + item.IncentiveBonus + item.HonorariumBonus + item.ScholarshipBonus) - (item.RevStamp + item.Deduction)
            );

            // Convert the grand total to words
            //string totalInWords = NumberToWords(Convert.ToDecimal(grandTotal));
            string totalInWords = NumberToWordsConverter.Convert((decimal)grandTotal);

            source = source.OrderBy(sb =>
            {
                var numericPart = sb.JobCode?.Substring(2); 
                int jobCodeValue;
                return int.TryParse(numericPart, out jobCodeValue) ? jobCodeValue : int.MaxValue;
            }).ToList();
            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
            DateTime currentDate = DateTime.Now;
            string printDate = currentDate.ToString("M/d/yy, h:mm tt");
            var parameters = new[]
            {
                    new ReportParameter("printDate",printDate.ToString()),
                    new ReportParameter("month",monthName.ToString()),
                    new ReportParameter("year",year.ToString()),
                    new ReportParameter("bonusType",bonusType.ToString()),
                    new ReportParameter("totalInWords",totalInWords.ToString()),
                };
            string renderFormat = "PDF";
            string mimtype = "application/pdf";
            using var report = new Microsoft.Reporting.NETCore.LocalReport();
            report.EnableExternalImages = true;
            string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptBonusControlSheet.rdlc";
            report.DataSources.Add(new ReportDataSource("dsBonusControlSheet", source));
            report.ReportPath = rptPath;
            report.SetParameters(parameters);
            var pdf = report.Render(renderFormat);
            return File(pdf, mimtype);

        }



        //**************** Bonus Control Sheet Report End ****************


        //**************** Bonus Journal Report Start ****************
        public IActionResult GetBonusJournal()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetBonusJournal(int year, int month, int bonusId, int employeeTypeId)
        {

            string monthsError = "";
            string yearsError = "";
            string employeeTypeIdError = "";
            string bonusError = "";
            string bankError = "";
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
            if (bonusId == 0)
            {
                bonusError = "Select bonus field";
            }


            if (monthsError.Length > 0 || yearsError.Length > 0 || employeeTypeIdError.Length > 0 || bonusError.Length > 0)
            {
                ViewBag.monthsError = monthsError;
                ViewBag.yearsError = yearsError;
                ViewBag.employeeTypeIdError = employeeTypeIdError;
                ViewBag.bonusError = bonusError;
                return View();
            }


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
            List<BonusJournal> bonusJournals = new List<BonusJournal>();
            var journalMasterData = await _bonusContract.GetBonusJournalMaster(monthid, employeeTypeId, bonusId);
            if (journalMasterData != null)
            {
                bonusJournals.AddRange(await _bonusContract.GetBonusJournalsByMasterId(journalMasterData.Id));
            }
            DateTime currentDate = DateTime.Now;
            string printDate = currentDate.ToString("MMMM dd, yyyy");

            var parameters = new[]
            {
                new ReportParameter("printDate",printDate.ToString()),
                new ReportParameter("empType",empType.ToString()),
                new ReportParameter("month",monthName.ToString()),
                new ReportParameter("year",year.ToString()),

            };
            string renderFormat = "PDF";
            string mimtype = "application/pdf";
            using var report = new Microsoft.Reporting.NETCore.LocalReport();
            report.EnableExternalImages = true;
            string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptBonusJournal.rdlc";
            report.DataSources.Add(new ReportDataSource("dsBonusJournal", bonusJournals));
            report.ReportPath = rptPath;
            report.SetParameters(parameters);
            var pdf = report.Render(renderFormat);
            return File(pdf, mimtype);

        }
        //**************** Bonus Journal Report End ****************


        //**************** Bonus Pay Slip Report Start ****************
        public async Task<IActionResult> GetBonusPaySlip()
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
        public async Task<IActionResult> GetBonusPaySlip(string jobCode, int month, int year, int bonusId, int EmployeeTypeId, int? department)
        {
            string monthsError = "";
            string yearsError = "";
            string employeeTypeIdError = "";
            string bonusError = "";

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

            if (bonusId == 0)
            {
                bonusError = "Select bonus field";
            }

            if (monthsError.Length > 0 ||
                yearsError.Length > 0 ||
                employeeTypeIdError.Length > 0 ||
                bonusError.Length > 0)
            {
                var departments = await _departmentContract.GetDepartments();
                var departmentList = departments.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.DepartmentName
                }).ToList();

                ViewBag.Departments = departmentList;

                var jobCodes = await _employeeContract.GetEmployeeCode();
                var codeList = jobCodes.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.JobCode
                }).ToList();

                ViewBag.EmployeeCode = codeList;

                ViewBag.monthsError = monthsError;
                ViewBag.yearsError = yearsError;
                ViewBag.employeeTypeIdError = employeeTypeIdError;
                ViewBag.bonusError = bonusError;

                return View(); 
            }

            var monthid = year * 100 + month;
            var source = await _bonusContract.GetBonusPayslip(jobCode, monthid, bonusId, EmployeeTypeId, department);

            var bonusType = bonusId switch
            {
                1 => "Festival Bonus",
                2 => "Incentive Bonus",
                3 => "Honorarium Bonus",
                4 => "Scholarship Bonus",
                _ => "All"
            };

            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
            DateTime currentDate = DateTime.Now;
            string printDate = currentDate.ToString("MMMM dd, yyyy");
            var parameters = new[]
                {
                new ReportParameter("printDate", printDate.ToString()),
                new ReportParameter("bonusType", bonusType),
                new ReportParameter("month", monthName),
                new ReportParameter("year", year.ToString())
                };

            string renderFormat = "PDF";
            string mimtype = "application/pdf";
            using var report = new Microsoft.Reporting.NETCore.LocalReport();
            report.EnableExternalImages = true;
            string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptBonusPaySlip.rdlc";
            report.DataSources.Add(new ReportDataSource("dsBonusPaySlip", source));
            report.ReportPath = rptPath;
            report.SetParameters(parameters);
            var pdf = report.Render(renderFormat);

            return File(pdf, mimtype);
        }

        //**************** Bonus Pay Slip Report End ****************




        //**************** Bonus Bank Forwarding  Report Start ****************
        [HttpGet]
        public async Task<IActionResult> BonusForwarding()
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
        public async Task<IActionResult> BonusForwarding(int employeeTypeId, int month, int year, int? bank, int bonusId, string Accounts)
        {

            string monthsError = "";
            string yearsError = "";
            string employeeTypeIdError = "";
            string bonusError = "";
            string bankError = "";

           

            if (employeeTypeId == 0)
            {
                employeeTypeIdError = "Select Employee Type";
            }

            if (bonusId == 0)
            {
                bonusError = "Select bonus field";
            }


            if (
                employeeTypeIdError.Length > 0 ||
                bonusError.Length > 0)
            {

                ViewBag.employeeTypeIdError = employeeTypeIdError;
                ViewBag.bonusError = bonusError;


                return View();
            }

            var monthId = year * 100 + month;
            var salaryResponse = await _bonusContract.GetBonusBankForwarding(monthId, bonusId, employeeTypeId, bank, 0);

            decimal grandNetPay = 0;

            // Calculate grand net pay
            string empType = "";
            if (employeeTypeId == 1)
            {
                empType = "Permanent";
            }
           else if(employeeTypeId == 2)
            {
                empType = "Contract";
            }
            else
            {
                empType = "Daily Worker";
            }

            double grandTotal = salaryResponse.Sum(item => (item.FestivalBonus + item.IncentiveBonus + item.HonorariumBonus + item.ScholarshipBonus) - (item.RevStamp + item.Deduction));
            string formattedGrandTotal = grandTotal.ToString("N2");
            string netPayInWords = NumberToWordsConverter.Convert(Convert.ToDecimal(grandTotal));
            salaryResponse = salaryResponse.OrderBy(sb =>
            {
                var numericPart = sb.JobCode?.Substring(2);
                int jobCodeValue;
                return int.TryParse(numericPart, out jobCodeValue) ? jobCodeValue : int.MaxValue;
            }).ToList();

            DateTime currentDate = DateTime.Now;
            string printDate = currentDate.ToString("MMMM dd, yyyy");
            string printDateTime = currentDate.ToString("M/d/yy, h:mm tt");
            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
            string monthYear = $"{monthName} - {year}";
            var parameters = new[]
             {
                    new ReportParameter("empType",empType.ToString()),
                     new ReportParameter("Accounts",Accounts.ToString()),
                    new ReportParameter("printDate",printDate.ToString()),
                    new ReportParameter("printDateTime",printDateTime.ToString()),
                    new ReportParameter("monthYear",monthYear.ToString()),
                    new ReportParameter("grandNetPay",formattedGrandTotal.ToString()),
                    new ReportParameter("netPayInWords",netPayInWords.ToString()),
                };
            string renderFormat = "PDF";
            string mimtype = "application/pdf";
            using var report = new Microsoft.Reporting.NETCore.LocalReport();
            report.EnableExternalImages = true;
            string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\Rpt_Bonus_Forwarding.rdlc";
            report.DataSources.Add(new ReportDataSource("dsBonusBankForward", salaryResponse));
            report.ReportPath = rptPath;
            report.SetParameters(parameters);
            var pdf = report.Render(renderFormat);
            return File(pdf, mimtype);
        }
        [HttpGet]
        public async Task<IActionResult> BonusBankForward()
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
        public async Task<IActionResult> BonusBankForward(int employeeTypeId, int month, int year, int? bank, int bonusId)
        {

            string monthsError = "";
            string yearsError = "";
            string employeeTypeIdError = "";
            string bonusError = "";
            string bankError = "";

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

            if (bonusId == 0)
            {
                bonusError = "Select bonus field";
            }

            if (bank==0)
            {
                bankError = "Select a Bank name";
            }

            if (monthsError.Length > 0 ||
                yearsError.Length > 0 ||
                employeeTypeIdError.Length > 0 ||
                bonusError.Length > 0 ||
                bankError.Length > 0)
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
                ViewBag.employeeTypeIdError = employeeTypeIdError;
                ViewBag.bonusError = bonusError;
                ViewBag.bankError = bankError;

                return View();
            }

            var monthId = year * 100 + month;
            var salaryResponse = await _bonusContract.GetBonusBankForwarding(monthId, bonusId, employeeTypeId, bank, 0);
            //var salaryResponse = await _bonusContract.GetBonusBankForword(monthId, bonusId, employeeTypeId, 0,bank);

            var bonusType = "";
            if (bonusId == 1)
            {
                bonusType = "Festival Bonus";
            }
            else if (bonusId == 2)
            {
                bonusType = "Incentive Bonus";
            }
            else if (bonusId == 3)
            {
                bonusType = "Honorarium Bonus";
            }
            else if (bonusId == 4)
            {
                bonusType = "Scholarship Bonus";
            }
            else
            {
                bonusType = "All";
            }

            string bankName = "";
            string bankBranch = "";
            decimal grandNetPay = 0;

            // Calculate grand net pay

            double grandTotal = salaryResponse.Sum(item => (item.FestivalBonus + item.IncentiveBonus + item.HonorariumBonus + item.ScholarshipBonus) - (item.RevStamp + item.Deduction));
            string formattedGrandTotal = grandTotal.ToString("N2");
            string netPayInWords = NumberToWordsConverter.Convert(Convert.ToDecimal(grandTotal));
            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
            salaryResponse = salaryResponse.OrderBy(sb =>
            {
                var numericPart = sb.JobCode?.Substring(2);
                int jobCodeValue;
                return int.TryParse(numericPart, out jobCodeValue) ? jobCodeValue : int.MaxValue;
            }).ToList();

            DateTime currentDate = DateTime.Now;
            string printDate = currentDate.ToString("MMMM dd, yyyy");
            var parameters = new[]
             {
                    new ReportParameter("printDate",printDate.ToString()),
                    new ReportParameter("bank",bank.ToString()),
                    new ReportParameter("bonusType",bonusType.ToString()),
                    new ReportParameter("month",monthName.ToString()),
                    new ReportParameter("year",year.ToString()),
                    new ReportParameter("grandNetPay",formattedGrandTotal.ToString()),
                    new ReportParameter("netPayInWords",netPayInWords.ToString()),
                };
            string renderFormat = "PDF";
            string mimtype = "application/pdf";
            using var report = new Microsoft.Reporting.NETCore.LocalReport();
            report.EnableExternalImages = true;
            string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptBonusBankForward.rdlc";
            report.DataSources.Add(new ReportDataSource("dsBonusBankForward", salaryResponse));
            report.ReportPath = rptPath;
            report.SetParameters(parameters);
            var pdf = report.Render(renderFormat);
            return File(pdf, mimtype);
        }

        public string NumberToWords(decimal number)
        {
            if (number == 0) return "Zero";

            string words = "";
            long integerPart = (long)Math.Floor(number);
            long fractionPart = (long)((number - integerPart) * 100);

            if (integerPart > 0)
            {
                words = ConvertWholeNumber(integerPart) + " Taka";
            }

            if (fractionPart > 0)
            {
                words += " and " + ConvertWholeNumber(fractionPart) + " Poisa";
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
