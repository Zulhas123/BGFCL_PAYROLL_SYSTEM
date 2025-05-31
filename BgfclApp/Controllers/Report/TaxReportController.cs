using AspNetCore.Reporting;
using Contracts;
using Entities;
using Entities.ViewModels;
using Entities.ViewModels.Reports;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Reporting.NETCore;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;

namespace BgfclApp.Controllers.Report
{
    public class TaxReportController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private IEmployeeTypeContract _employeeTypeContract;
        private IDepartmentContract _departmentContract;
        private IDesignationContract _designationContract;
        private IMonthlyIncomeTexContract _monthlyIncomeTexContract;
        private IYearlyIncomeTaxContract _yearlyIncomeTaxContract;
        private IReportContract _reportContract;
        private ISalarySettingContract _salarySettingContract;
        private IEmployeeContract _employeeContract;
        private IAdvanceTaxContract _advanceTaxContract;
        public TaxReportController(IWebHostEnvironment webHostEnvironment,
                                  ISalarySettingContract salarySettingContract,
                                  ISalaryReportOfficerContract salaryReportOfficerContract,
                                  IEmployeeTypeContract employeeTypeContract,
                                  IDepartmentContract departmentContract,
                                  IDesignationContract designationContract,
                                  IMonthlyIncomeTexContract monthlyIncomeTexContract,
                                  IYearlyIncomeTaxContract yearlyIncomeTaxContract,
                                  IReportContract reportContract,
                                  IEmployeeContract employeeContract,
                                  IAdvanceTaxContract advanceTaxContract)
        {

            _webHostEnvironment = webHostEnvironment;
            _employeeTypeContract = employeeTypeContract;
            _departmentContract = departmentContract;
            _designationContract = designationContract;
            _monthlyIncomeTexContract = monthlyIncomeTexContract;
            _yearlyIncomeTaxContract = yearlyIncomeTaxContract;
            _reportContract = reportContract;
            _salarySettingContract = salarySettingContract;
            _employeeContract = employeeContract;
            _advanceTaxContract = advanceTaxContract;
        }
        public IActionResult Index()
        {
            return View();
        }


        //**************** Income Tax Monthly Report Start ****************

        [HttpGet]
        public async Task<IActionResult> GetIncomeTaxMonthly()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetIncomeTaxMonthly(int year, int month,int employeeTypeId)
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
            if (employeeTypeId == 0)
            {
                employeeTypeIdError = "Select Employee Type";
            }



            if (monthsError.Length > 0 || yearsError.Length > 0 )
            {
                ViewBag.monthsError = monthsError;
                ViewBag.yearsError = yearsError;
                return View();
            }

            var monthId = year * 100 + month;
            var source = await _reportContract.GetMonthlyIncomeTax( monthId);
            var sorteddata = source.OrderBy(b => b.JobCode).ToList();
            var filteredList = sorteddata.Where(s => s.IncomeTax != 0).ToList();

            // Calculate the grand total of IncomeTax
            var grandTotalIncomeTax = source.Sum(item => item.IncomeTax);
            string netPayInWords = NumberToWordsConverter.Convert((int)Math.Round((decimal)grandTotalIncomeTax));

            string mimetype = "";
            int extension = 1;
            var path = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptIncomeTaxMonthly.rdlc";
            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
            DateTime currentDate = DateTime.Now;
            string printDate = currentDate.ToString("MMMM dd, yyyy");

            var parameters = new[] 
            {
                new ReportParameter("printDate",printDate.ToString()),
                new ReportParameter("month",monthName.ToString()),
                new ReportParameter("year",year.ToString()),
                new ReportParameter("netPayInWords",netPayInWords.ToString()),
            };

            string renderFormat = "PDF";
            string mimtype = "application/pdf";
            using var report = new Microsoft.Reporting.NETCore.LocalReport();
            report.EnableExternalImages = true;
            string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptIncomeTaxMonthly.rdlc";
            report.DataSources.Add(new ReportDataSource("dsIncomeTaxMonthly", filteredList));
            report.ReportPath = rptPath;
            report.SetParameters(parameters);
            var pdf = report.Render(renderFormat);
            return File(pdf, mimtype);

        }


        //**************** Income Tax Monthly Report End ****************


        //**************** Income Tax yearly Report Start ****************

        [HttpGet]
        public async Task<IActionResult> GetIncomeTaxYearly()
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

            var designation = await _designationContract.GetDesignations();
            var designationList = designation.Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.DesignationName
            }).ToList();

            ViewBag.designations = designationList;
            return View();
        }


        
        [HttpPost]
        public async Task<IActionResult> GetIncomeTaxYearly(int employeeTypeId, int fyear, int fmonth, int tyear, int tmonth, string? department, string? designation)
        {

            string fmonthsError = "";
            string fyearsError = "";
            string tmonthsError = "";
            string tyearsError = "";
            string employeeTypeIdError = "";
            string jobCodeError = "";
            if (fmonth == 0)
            {
                fmonthsError = "Select a From Month";

            }
            if (fyear == 0)
            {
                fyearsError = "Select a From  Year";
            }
            if (tmonth == 0)
            {
                tmonthsError = "Select a To Month";

            }
            if (tyear == 0)
            {
                tyearsError = "Select a To Year";
            }
            
           

            if (fmonthsError.Length > 0 || fyearsError.Length > 0 || tmonthsError.Length > 0 || tyearsError.Length > 0 )
            {

                var departments = await _departmentContract.GetDepartments();
                var departmentList = departments.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.DepartmentName
                }).ToList();

                ViewBag.Departments = departmentList;

                var designations = await _designationContract.GetDesignations();
                var designationList = designations.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = d.DesignationName
                }).ToList();

                ViewBag.designations = designationList;


                ViewBag.fmonthsError = fmonthsError;
                ViewBag.fyearsError = fyearsError;
                ViewBag.tmonthsError = tmonthsError;
                ViewBag.tyearsError = tyearsError;
                ViewBag.jobCodeError = jobCodeError;
                
                return View();
            }


            var fromMonthId = fyear * 100 + fmonth;
            var toMonthId = tyear * 100 + tmonth;
            var source = await _yearlyIncomeTaxContract.GetYearlyIncomeTex(fromMonthId, toMonthId, department, designation);
            source = source.Where(c => c.IncomeTax != 0).OrderBy(b => b.JobCode).ToList();
            var grandTotalIncomeTax = source.Sum(item => item.IncomeTax);
            string netPayInWords = NumberToWordsConverter.Convert((int)Math.Round((decimal)grandTotalIncomeTax));

            string fmonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(fmonth);
            string tmonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(tmonth);

            var empType = employeeTypeId == 1 ? "Officer" : "Junior Staff";
            DateTime currentDate = DateTime.Now;
            string printDate = currentDate.ToString("MMMM dd, yyyy");


            var parameters = new[]
            {
                new ReportParameter("printDate",printDate.ToString()),
                new ReportParameter("tmonth",tmonthName.ToString()),
                new ReportParameter("tyear",tyear.ToString()),
                new ReportParameter("fmonth",fmonthName.ToString()),
                new ReportParameter("fyear",fyear.ToString()),
                new ReportParameter("netPayInWords",netPayInWords.ToString()),
            };

            string renderFormat = "PDF";
            string mimtype = "application/pdf";
            using var report = new Microsoft.Reporting.NETCore.LocalReport();
            report.EnableExternalImages = true;
            string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptYearlyIncomeTax.rdlc";
            report.DataSources.Add(new ReportDataSource("dsYearlyIncomeTax", source));
            report.ReportPath = rptPath;
            report.SetParameters(parameters);
            var pdf = report.Render(renderFormat);
            return File(pdf, mimtype);





        }
        //**************** Income Tax Yearly Report End ****************
        [HttpGet]
        public async Task<IActionResult> GetTaxCertificate()
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


        [HttpPost]
        public async Task<IActionResult> GetTaxCertificate(string jobCode, int year, int month, int department)
        {
            Dictionary<int, AdvanceTax> advanceTaxes = new Dictionary<int, AdvanceTax>();
            //List<AdvanceTax> advanceTaxes = new List<AdvanceTax>();
            List<int> fiscalYears = new List<int>();
            List<TaxRecordViewModel> taxRecords = new List<TaxRecordViewModel>();


            // Define the fiscal year range based on the selected year
            int fiscalYearStart = (year - 1) * 100 + 7; // July of the previous year
            int fiscalYearEnd = year * 100 + 6; // June of the current year

            for (int i = 7; i <= 12; i++)
            {
                fiscalYears.Add((year - 1) * 100 + i);
            }

            for (int i = 1; i <= 6; i++)
            {
                fiscalYears.Add((year * 100) + i);
            }

            foreach (var item in fiscalYears)
            {
                advanceTaxes.Add(item,await _advanceTaxContract.GetAdvanceTaxByMonthId(item));
            }


            var source = await _salarySettingContract.GetSalaryDataForTaxCertificate(jobCode, department, fiscalYearStart, fiscalYearEnd);
            if (source == null || source.Count == 0)
            {
                return NotFound("Tax certificate data not found.");
            }

            foreach (var item in fiscalYears)
            {
                SalaryReportOfficer salaryData = source.Where(s=>s.MonthId == item).SingleOrDefault();
                AdvanceTax advanceTax = null;
                foreach (var data in advanceTaxes)
                {
                    if(data.Key == item)
                    {
                        advanceTax = data.Value;
                    }
                }

                decimal? incomeTax = 0;
                string? chalanNo = "";
                DateTime? date = null;

                if (salaryData!=null)
                {
                    if (salaryData.IncomeTax!=null)
                    {
                        incomeTax = salaryData.IncomeTax;
                    }
                }
                if (advanceTax!=null)
                {
                    chalanNo = advanceTax.LetterNo;
                    if (!String.IsNullOrEmpty(advanceTax.Date))
                    {
                        date = Convert.ToDateTime(advanceTax.Date);
                    }
                }

                TaxRecordViewModel taxRecord = new TaxRecordViewModel();
                taxRecord.Month = TaxReportController.GetMonthNameByMonthId(item);
                taxRecord.IncomeTax = incomeTax;
                taxRecord.ChalanNo = chalanNo;
                taxRecord.Date = date;

                taxRecords.Add(taxRecord);

            }

            // Convert fiscalYearStart and fiscalYearEnd to DateTime
            DateTime fiscalStartDate = new DateTime(fiscalYearStart / 100, fiscalYearStart % 100, 1);
            DateTime fiscalEndDate = new DateTime(fiscalYearEnd / 100, fiscalYearEnd % 100, 1);
            Employee employee = await _employeeContract.GetEmployeeByjobCode(jobCode);
            EmployeeViewModel employeeViewModel = await _employeeContract.GetEmployeeForView(employee.Id);

            //var groupedData = source
            //    .Where(item => item.MonthID >= fiscalYearStart && item.MonthID <= fiscalYearEnd).ToList();
           
            //// If no data to display, return an empty PDF with a message or handle as needed
            //if (!groupedData.Any())
            //{
            //    // Create an empty PDF with a message if needed
            //    string emptyReportMessage = "No tax certificate data available for the selected criteria.";
               
            //    var emptyParameters = new[]
            //    {
                
            //    new ReportParameter("fiscalStart", "N/A"),
            //    new ReportParameter("fiscalEnd", "N/A"),
            //    new ReportParameter("year", year.ToString()),
            //    new ReportParameter("fiscalYearTotal", "0.00"),
            //    new ReportParameter("fiscalYearTotalInWords", "Zero"),
            //    new ReportParameter("TIN", "00"),
            //    new ReportParameter("JobCode", "00"),

            //    };
            //}

            // Extract TIN and JobCode from the first groupedData item for use in parameters
            string tin = employeeViewModel.TinNo;
            string firstJobCode = employeeViewModel.JobCode;
            string designation = employeeViewModel.DesignationName;
            string employeeName = employeeViewModel.EmployeeName;

            decimal fiscalYearTotal = (decimal)taxRecords.Sum(t=>t.IncomeTax);
            string fiscalYearTotalInWords = NumberToWordsConverter.Convert((int)Math.Round(fiscalYearTotal));
            DateTime currentDate = DateTime.Now;
            string printDate = currentDate.ToString("MMMM dd, yyyy");
            var parameters = new[]
            {
                new ReportParameter("printDate", printDate.ToString()),
                new ReportParameter("fiscalStart", fiscalStartDate.ToString("MMMM yyyy")),
                new ReportParameter("fiscalEnd", fiscalEndDate.ToString("MMMM yyyy")),
                new ReportParameter("year", year.ToString()),
                new ReportParameter("fiscalYearTotal", fiscalYearTotal.ToString("N2")), // Format with comma separator and two decimal places
                new ReportParameter("fiscalYearTotalInWords", fiscalYearTotalInWords),
                new ReportParameter("TIN", tin),
                new ReportParameter("JobCode", firstJobCode),
                new ReportParameter("employeeName", employeeName),
                new ReportParameter("designation", designation),
            };

            string renderFormat = "PDF";
            string mimtype = "application/pdf";
            using var report = new Microsoft.Reporting.NETCore.LocalReport();
            report.EnableExternalImages = true;
            string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptTaxCertificate.rdlc";
            report.DataSources.Add(new ReportDataSource("dsTaxCertificate", taxRecords));
            report.ReportPath = rptPath;
            report.SetParameters(parameters);
            var pdf = report.Render(renderFormat);
            return File(pdf, mimtype);
        }


        public static string GetMonthNameByMonthId(int monthId)
        {
            int month = monthId % 100;
            string monthName;

            switch (month)
            {
                case 1:
                    monthName = "January";
                    break;
                case 2:
                    monthName = "February";
                    break;
                case 3:
                    monthName = "March";
                    break;
                case 4:
                    monthName = "April";
                    break;
                case 5:
                    monthName = "May";
                    break;
                case 6:
                    monthName = "June";
                    break;
                case 7: 
                    monthName = "July"; 
                    break;
                case 8: 
                    monthName = "August"; 
                    break;
                case 9:
                    monthName = "September";
                    break;
                case 10:
                    monthName = "October";
                    break;
                case 11:
                    monthName = "November";
                    break;
                case 12:
                    monthName = "December";
                    break;
                default:
                    monthName = "";
                    break;
            }

            return monthName;
        }


    }
}
