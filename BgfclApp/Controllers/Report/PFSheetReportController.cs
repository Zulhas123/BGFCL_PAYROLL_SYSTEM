using AspNetCore.Reporting;
using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BgfclApp.Controllers.Report
{
    public class PFSheetReportController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private ISalarySettingContract _salarySettingContract;
        private IEmployeeTypeContract _employeeTypeContract;
        private IDepartmentContract _departmentContract;
        private ISalaryReportOfficerContract _salaryReportOfficerContract;
        private IPFSheetReportContract _pFSheetReportContract;
        private IEmployeeContract _employeeContract;
        public PFSheetReportController(IWebHostEnvironment webHostEnvironment,
                                       ISalarySettingContract salarySettingContract,
                                       ISalaryReportOfficerContract salaryReportOfficerContract,
                                       IEmployeeTypeContract employeeTypeContract,
                                       IDepartmentContract departmentContract,
                                       IPFSheetReportContract pFSheetReportContract
            ,
                                       IEmployeeContract employeeContract

                                      )
        {

            _webHostEnvironment = webHostEnvironment;
            _salarySettingContract = salarySettingContract;
            _salaryReportOfficerContract = salaryReportOfficerContract;
            _employeeTypeContract = employeeTypeContract;
            _departmentContract = departmentContract;
            _pFSheetReportContract = pFSheetReportContract;
            _employeeContract = employeeContract;
        }
        public IActionResult Index()
        {
            return View();
        }
        //**************** PF Sheet Report Start ****************

        [HttpGet]
        public async Task<IActionResult> GetPFSheet()
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
        public async Task<IActionResult> GetPFSheet(List<string> jobCode, int month, int year, string employeeType, string department)
        {
            var monthId = year * 100 + month;
            var source = await _pFSheetReportContract.GetPFSheetOF(jobCode, monthId, department);

            string mimetype = "";
            int extension = 1;
            var path = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptPFSheet.rdlc";
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            LocalReport localReport = new LocalReport(path);
            localReport.AddDataSource("dsPFSheet", source);

            var result = localReport.Execute(RenderType.Pdf, extension, parameters, mimetype);

            return File(result.MainStream, "application/pdf");
        }


        //**************** PF Sheet Report End ****************

    }
}
