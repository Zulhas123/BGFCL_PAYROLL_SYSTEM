using AspNetCore.Reporting;
using Contracts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Reporting.NETCore;
using System.Globalization;

namespace BgfclApp.Controllers.Report
{
    public class MedicalFundController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private ISalarySettingContract _salarySettingContract;
        private ISalaryReportOfficerContract _salaryReportOfficerContract;
        private IEmployeeTypeContract _employeeTypeContract;
        public MedicalFundController(IWebHostEnvironment webHostEnvironment, 
                                     ISalarySettingContract salarySettingContract,
                                     ISalaryReportOfficerContract salaryReportOfficerContract,
                                     IEmployeeTypeContract employeeTypeContract)
        {

            _webHostEnvironment = webHostEnvironment;
            _salarySettingContract = salarySettingContract;
            _salaryReportOfficerContract = salaryReportOfficerContract;
            _employeeTypeContract = employeeTypeContract;
        }
        public IActionResult Index()
        {
            return View();
        }
        //**************** Medical Fund Report Start ****************

        [HttpGet]
        public async Task<IActionResult> GetMedicalFund()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetMedicalFund(int month, int year)
        {

            string monthsError = "";
            string yearsError = "";
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
                ViewBag.monthsError = monthsError;
                ViewBag.yearsError = yearsError;
                return View();
            }

            int monthId = year * 100 + month;
            var source = await _salarySettingContract.GetMedicalFunds(monthId);
            source = source.Where(c => c.MedicalFund != 0).OrderBy(b => b.JobCode).ToList();

            // Calculate the grand total for MedicalFund
            decimal grandTotalMedicalFund = source.Sum(item => (Convert.ToDecimal(item.MedicalFund)));
            string totalInWords = NumberToWords(grandTotalMedicalFund);
            string mimetype = "";
            int extension = 1;
            var path = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptMedicalFund.rdlc";
            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
            DateTime currentDate = DateTime.Now;
            string printDate = currentDate.ToString("MMMM dd, yyyy");
            var parameters = new[]
            {
                new ReportParameter("printDate",printDate.ToString()),
                new ReportParameter("month",monthName.ToString()),
                new ReportParameter("year",year.ToString()),
                new ReportParameter("grandTotal",totalInWords.ToString()),
            };
            string renderFormat = "PDF";
            string mimtype = "application/pdf";
            using var report = new Microsoft.Reporting.NETCore.LocalReport();
            report.EnableExternalImages = true;
            string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptMedicalFund.rdlc";
            report.DataSources.Add(new ReportDataSource("dsMedicalFund", source));
            report.ReportPath = rptPath;
            report.SetParameters(parameters);
            var pdf = report.Render(renderFormat);
            return File(pdf, mimtype);

        }


        //**************** Medical Fund Report End ****************

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

        private string ConvertWholeNumber(long number)
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
