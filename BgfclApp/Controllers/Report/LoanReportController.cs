using AspNetCore.Reporting;
using ClosedXML.Excel;
using Contracts;
using Entities;
using Entities.ViewModels;
using Entities.ViewModels.Reports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Reporting.NETCore;
using System.Globalization;
using System.Reflection.Emit;

namespace BgfclApp.Controllers.Report
{
    public class LoanReportController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private ISalarySettingContract _salarySettingContract;
        private ISalaryReportOfficerContract _salaryReportOfficerContract;
        private IEmployeeContract _employeeContract;
        private ILoanContract _loanContract;
        public LoanReportController(IWebHostEnvironment webHostEnvironment,
                                    ISalarySettingContract salarySettingContract,
                                    ISalaryReportOfficerContract salaryReportOfficerContract,
                                    IEmployeeContract employeeContract,
                                    ILoanContract loanContract)
        {

            _webHostEnvironment = webHostEnvironment;
            _salarySettingContract = salarySettingContract;
            _salaryReportOfficerContract = salaryReportOfficerContract;
            _employeeContract = employeeContract;
            _loanContract = loanContract;
        }
        public IActionResult Index()
        {
            return View();
        }


        //**************** Loan Sheet Report Start ****************

        [HttpGet]
        public IActionResult GetLoanSheet()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetLoanSheet(int empType, int monthId, int bank, int reportCopy)
        {
            var source = await _salaryReportOfficerContract.GetSalaryReportOfficer(202405, empType);

           
            var parameters = new[]
                 {
                new ReportParameter(),
                new ReportParameter(),
                new ReportParameter(),
                new ReportParameter(),

                };
            string renderFormat = "PDF";
            string mimtype = "application/pdf";
            using var report = new Microsoft.Reporting.NETCore.LocalReport();
            report.EnableExternalImages = true;
            string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptLoanSheet.rdlc";
            report.DataSources.Add(new ReportDataSource("dsLoanSheet", source));
            report.ReportPath = rptPath;
            report.SetParameters(parameters);
            var pdf = report.Render(renderFormat);
            return File(pdf, mimtype); ;

        }

        //**************** Loan Sheet Report End ****************


        //**************** Yearly Ending MCL Report Start ****************

        [HttpGet]
        public async Task<IActionResult> GetMCLLoan()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            var jobCode = await _employeeContract.GetEmployeeCode();
            var CodeList = jobCode.Select(d => new SelectListItem
            {
                Value = d.JobCode.ToString(),
                Text = $"{d.JobCode} - {d.EmployeeName}"
            }).ToList();

            ViewBag.EmployeeCode = CodeList;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetMCLLoan(string jobCode, int fmonth, int fyear, int tmonth, int tyear,int employeeTypeId)
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
            if (employeeTypeId == 0)
            {
                employeeTypeIdError = "Select Employee Type";
            }
            if (String.IsNullOrEmpty(jobCode))
            {
                jobCodeError = "Select JobCode";
            }

            if (fmonthsError.Length > 0 || fyearsError.Length > 0 || tmonthsError.Length > 0 || tyearsError.Length > 0 || employeeTypeIdError.Length > 0)
            {
                var jobCode1 = await _employeeContract.GetEmployeeCode();
                var CodeList = jobCode1.Select(d => new SelectListItem
                {
                    Value = d.JobCode.ToString(),
                    Text = $"{d.JobCode} - {d.EmployeeName}"
                }).ToList();

                ViewBag.EmployeeCode = CodeList;

                ViewBag.fmonthsError = fmonthsError;
                ViewBag.fyearsError = fyearsError;
                ViewBag.tmonthsError = tmonthsError;
                ViewBag.tyearsError = tyearsError;
                ViewBag.jobCodeError = jobCodeError;
                ViewBag.employeeTypeIdError = employeeTypeIdError;
                return View();
            }

            var fmonthid = fyear * 100 + fmonth;
            var tmonthid = tyear * 100 + tmonth;
            var source = await _loanContract.GetYearEndingMCLLoanInfo(jobCode,fmonthid, tmonthid, employeeTypeId);
            var emp = "";
            if (employeeTypeId == 1)
            {
                emp = "Officer";
            }
            else
            {
                emp = "Junior Staff";
            }
            int extension = 1;
            string fmonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(fmonth);
            string tmonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(fmonth);

            
            var parameters = new[]
                 {
                new ReportParameter("fMonth",fmonthName.ToString()),
                new ReportParameter("fYear",fyear.ToString()),
                new ReportParameter("tMonth",tmonthName.ToString()),
                new ReportParameter("tYear",tyear.ToString()),
                new ReportParameter("empType",emp.ToString()),

                };
            string renderFormat = "PDF";
            string mimtype = "application/pdf";
            using var report = new Microsoft.Reporting.NETCore.LocalReport();
            report.EnableExternalImages = true;
            string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptMCl.rdlc";
            report.DataSources.Add(new ReportDataSource("dsMCL", source));
            report.ReportPath = rptPath;
            report.SetParameters(parameters);
            var pdf = report.Render(renderFormat);
            return File(pdf, mimtype);
        }

        //**************** Loan Sheet Report End ****************

        //**************** Yearly Ending COM Loan Report Start ****************

        [HttpGet]
        public async Task<IActionResult> GetComLoan()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            var jobCode = await _employeeContract.GetEmployeeCode();
            var CodeList = jobCode.Select(d => new SelectListItem
            {
                Value = d.JobCode.ToString(),
                Text = $"{d.JobCode} - {d.EmployeeName}"
            }).ToList();

            ViewBag.EmployeeCode = CodeList;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetComLoan(string jobCode, int fmonth, int fyear, int tmonth, int tyear, int employeeTypeId)
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
            if (employeeTypeId == 0)
            {
                employeeTypeIdError = "Select Employee Type";
            }
            if (String.IsNullOrEmpty(jobCode))
            {
                jobCodeError = "Select JobCode";
            }

            if (fmonthsError.Length > 0 || fyearsError.Length > 0 || tmonthsError.Length > 0 || tyearsError.Length > 0 || employeeTypeIdError.Length > 0)
            {
                var jobCode1 = await _employeeContract.GetEmployeeCode();
                var CodeList = jobCode1.Select(d => new SelectListItem
                {
                    Value = d.JobCode.ToString(),
                    Text = $"{d.JobCode} - {d.EmployeeName}"
                }).ToList();

                ViewBag.EmployeeCode = CodeList;

                ViewBag.fmonthsError = fmonthsError;
                ViewBag.fyearsError = fyearsError;
                ViewBag.tmonthsError = tmonthsError;
                ViewBag.tyearsError = tyearsError;
                ViewBag.jobCodeError = jobCodeError;
                ViewBag.employeeTypeIdError = employeeTypeIdError;
                return View();
            }

            var fmonthid = fyear * 100 + fmonth;
            var tmonthid = tyear * 100 + tmonth;
            var source = await _loanContract.GetYearEndingCOMLoanInfo(jobCode,fmonthid, tmonthid, employeeTypeId);
            var emp = "";
            if (employeeTypeId == 1)
            {
                emp = "Officer";
            }
            else
            {
                emp = "Junior Staff";
            }
            int extension = 1;
            string fmonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(fmonth);
            string tmonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(tmonth);

            
            var parameters = new[]
                 {
                new ReportParameter("fMonth",fmonthName.ToString()),
                new ReportParameter("fYear",fyear.ToString()),
                new ReportParameter("tMonth",tmonthName.ToString()),
                new ReportParameter("tYear",tyear.ToString()),
                new ReportParameter("empType",emp.ToString()),

                };
            string renderFormat = "PDF";
            string mimtype = "application/pdf";
            using var report = new Microsoft.Reporting.NETCore.LocalReport();
            report.EnableExternalImages = true;
            string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptComLoan.rdlc";
            report.DataSources.Add(new ReportDataSource("dsComLoan", source));
            report.ReportPath = rptPath;
            report.SetParameters(parameters);
            var pdf = report.Render(renderFormat);
            return File(pdf, mimtype);

        }

        //**************** Com Loan Report End ****************


        [HttpGet]
        public async Task<IActionResult> GetHBILoan()
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
                Text = $"{d.JobCode} - {d.EmployeeName}"
            }).ToList();

            ViewBag.EmployeeCode = CodeList;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetHBILoan( List<string> jobCodes,int fmonth, int fyear, int tmonth, int tyear, int employeeTypeId)
        {
            var fmonthid = fyear * 100 + fmonth;
            var tmonthid = tyear * 100 + tmonth;
            var source = await _loanContract.GetYearEndingHBILoanInfo(jobCodes,fmonthid, tmonthid, employeeTypeId);
            string fmonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(fmonth);
            string tmonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(fmonth);

            

            var parameters = new[]
                {
                new ReportParameter("fMonth",fmonthName.ToString()),
                new ReportParameter("fYear",fyear.ToString()),
                new ReportParameter("tMonth",tmonthName.ToString()),
                new ReportParameter("tYear",tyear.ToString()),
                };
            string renderFormat = "PDF";
            string mimtype = "application/pdf";
            using var report = new Microsoft.Reporting.NETCore.LocalReport();
            report.EnableExternalImages = true;
            string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptHBILoan.rdlc";
            report.DataSources.Add(new ReportDataSource("dsHBILoan", source));
            report.ReportPath = rptPath;
            report.SetParameters(parameters);
            var pdf = report.Render(renderFormat);
            return File(pdf, mimtype);

        }



        [HttpGet]
        public async Task<IActionResult> HBLLoanSchedule()
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
                Text = $"{d.JobCode} - {d.EmployeeName}"
            }).ToList();

            ViewBag.EmployeeCode = CodeList;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> HBLLoanSchedule(List<string> jobCodes,int fmonth, int fyear, int tmonth, int tyear, int employeeTypeId, DateTime? endDate = null)
        {
            var fmonthid = fyear * 100 + fmonth;
            var tmonthid = tyear * 100 + tmonth;
            var source = await _loanContract.GetYearEndingHBILoanInfo(jobCodes,fmonthid, tmonthid, employeeTypeId);

            foreach (var loan in source)
            {
                            
                if (endDate.HasValue)
                {
                    int totalDays = (endDate.Value - new DateTime(fyear, fmonth, 1)).Days;
                    if (totalDays > 0)
                    {
                        //loan.InterestChargeDuringYear = CalculateInterest(loan.OpeningPrincipalBalance, loan.InterestRate, totalDays);
                    }
                }

                        // Requirement 2: Automatically start interest deduction once principal is fully recovered

                //if (loan.RecoveryPrincipal >= loan.TotalBalancePrincipal)
                //{
                //    loan.RecoveryInterest = loan.InterestChargeDuringYear;
                //}

                //        // Requirement 3: Handle Re-scheduled and 2nd installment loans
                //if (loan.ReScheduleLoanTakenThisYear > 0)
                //{
                //    DateTime loanEntryDate = loan.LoanTakenDate;
                //    int daysForInterest = (new DateTime(tyear, tmonth, 1) - loanEntryDate).Days;

                //              // Calculate interest for the rescheduled or 2nd installment loan
                //    loan.InterestChargeDuringYear += CalculateInterest(loan.ReScheduleLoanTakenThisYear, loan.InterestRate, daysForInterest);
                //}
            }

            // Generate the report as per your existing logic
            string fmonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(fmonth);
            string tmonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(tmonth);

            var parameters = new[]
            {
                new ReportParameter("fMonth", fmonthName),
                new ReportParameter("fYear", fyear.ToString()),
                new ReportParameter("tMonth", tmonthName),
                new ReportParameter("tYear", tyear.ToString())
            };

            string renderFormat = "PDF";
            string mimtype = "application/pdf";
            using var report = new Microsoft.Reporting.NETCore.LocalReport();
            report.EnableExternalImages = true;
            string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptHBLoanSchedule.rdlc";
            report.DataSources.Add(new ReportDataSource("dsHBLoanSchedule", source));
            report.ReportPath = rptPath;
            report.SetParameters(parameters);
            var pdf = report.Render(renderFormat);
            return File(pdf, mimtype);
        }


        [HttpGet]
        public async Task<IActionResult> HBLoanIndividual()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            var jobCode = await _employeeContract.GetEmployeeCode();
            var CodeList = jobCode.Select(d => new SelectListItem
            {
                Value = d.JobCode.ToString(),
                Text = $"{d.JobCode} - {d.EmployeeName}"
            }).ToList();

            ViewBag.EmployeeCode = CodeList;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> HBLoanIndividual(string jobCodes, int fmonth, int fyear, int tmonth, int tyear,int yearclosingmonth,int yearclosingyear, int employeeTypeId, DateTime? endDate = null)
        {
            List<HblIndividual> hblData = new List<HblIndividual>();
            HbLoan loanData = new HbLoan();
            var type = "Individual";
            var fmonthid = fyear * 100 + fmonth;
            var tmonthid = tyear * 100 + tmonth;
            var closingMonthId= yearclosingyear * 100 + yearclosingmonth;

            string fmonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(fmonth);
            string tmonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(tmonth);

            EmployeeViewModel employee = new EmployeeViewModel();
            double recoveryprincipal = 0;
            double recoveryInterest = 0;

            var hblloans = await _loanContract.GetHbLoans(isActive: 1);

            if (hblloans.Count > 0)
            {
                var hblloan = hblloans.Where(c => c.JobCode == jobCodes).FirstOrDefault();
                if (hblloan != null)
                {
                    loanData = await _loanContract.GetHbLoanById(hblloan.Id);

                    var installments = await _loanContract.GetHBLInstallmentsByDate((int)loanData.Id, fmonthid, tmonthid);

                    foreach (var item in installments)
                    {
                        hblData.Add(new HblIndividual
                        {
                            MonthId = item.MonthId,
                            RecoveryPrincipal = (decimal)item.InstallmentAmount,
                            RecoveryInterests = (decimal)item.InterestAmount,

                        });
                    }

                    employee = await _employeeContract.GetEmployeeViewByJobCode(jobCodes);

                    recoveryprincipal = 0;
                    recoveryInterest = 0;
                    foreach (var item in installments)
                    {
                        recoveryprincipal += item.InstallmentAmount;
                        recoveryInterest += item.InterestAmount;
                    }

                }
                else
                {
                    var jobCode1 = await _employeeContract.GetEmployeeCode();
                    var CodeList = jobCode1.Select(d => new SelectListItem
                    {
                        Value = d.JobCode.ToString(),
                        Text = $"{d.JobCode} - {d.EmployeeName}"
                    }).ToList();
                    ViewBag.EmployeeCode = CodeList;
                    ViewBag.Message = "Data not found";
                    return View();
                }
                
            }
            var hblYearEndingprincipal = await _loanContract.GethblYearEnding((int)loanData.Id, closingMonthId);
            var parameters = new[]
                {
                        new ReportParameter("type",type.ToString()),
                        new ReportParameter("fMonth",fmonthName.ToString()),
                        new ReportParameter("fYear",fyear.ToString()),
                        new ReportParameter("tMonth",tmonthName.ToString()),
                        new ReportParameter("tYear",tyear.ToString()),
                        new ReportParameter("employeeName",employee.EmployeeName),
                        new ReportParameter("jobCode",employee.JobCode),
                        new ReportParameter("designation",employee.DesignationName),
                        new ReportParameter("department",employee.DepartmentName),
                        new ReportParameter("openingPrincipal",hblYearEndingprincipal.ClosingPrincipal.ToString("N2")),
                        new ReportParameter("openingInterest",hblYearEndingprincipal.ClosingInterest.ToString("N2")),
                        new ReportParameter("recoveryPrincipal",recoveryprincipal.ToString("N2")),
                        new ReportParameter("recoveryInterest",recoveryInterest.ToString("N2")),
                        new ReportParameter("recoveryTotal", (recoveryprincipal + recoveryInterest).ToString("N2")),
                        new ReportParameter("InstallmentNoPrincipal",loanData.InstallmentNo.ToString("N2")),
                        new ReportParameter("InstallmentNoInterest",loanData.InterestInstallmentNo.ToString()),
                        new ReportParameter("runningInstallment",loanData.PrincipalInstallmentAmount.ToString()),
                        new ReportParameter("closingPrincipal",(hblYearEndingprincipal.ClosingPrincipal-recoveryprincipal).ToString("N2")),
                        new ReportParameter("closingInterest",(hblYearEndingprincipal.ClosingInterest-recoveryInterest).ToString("N2")),
                        new ReportParameter("totalLoanLiability",((hblYearEndingprincipal.ClosingPrincipal - recoveryprincipal) + (hblYearEndingprincipal.ClosingInterest - recoveryInterest)).ToString("N2"))

            };
            string renderFormat = "PDF";
            string mimtype = "application/pdf";
            using var report = new Microsoft.Reporting.NETCore.LocalReport();
            report.EnableExternalImages = true;
            string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptIndividualHblLoan.rdlc";
            report.DataSources.Add(new ReportDataSource("dsHblIndividualLoan", hblData));
            report.ReportPath = rptPath;
            report.SetParameters(parameters);
            var pdf = report.Render(renderFormat);
            return File(pdf, mimtype);
        }

        private decimal CalculateInterest(decimal principal, decimal interestRate, int days)
        {
            // Calculate interest based on principal, interest rate, and number of days
            return (principal * interestRate / 100) * (days / 365.0m);
        }


        //**************** Com Loan Report End ****************


        public async Task<IActionResult> GetCarLoanSummary()
        {
            int fromMonthId = 202407;
            int toMonthId = 202412;

            var workBook = new XLWorkbook();
            var workSheet = workBook.Worksheets.Add("Sheet1");
            workSheet.Cell(1, 1).Value = "JobCode";
            workSheet.Cell(1, 2).Value = "EmployeeName";
            workSheet.Cell(1, 3).Value = "LoanTakenDate";
            workSheet.Cell(1, 4).Value = "TotalLoanAmount";
            workSheet.Cell(1, 5).Value = "InterestRate";
            workSheet.Cell(1, 6).Value = "MonthlyInstallmentAmount";
            workSheet.Cell(1, 7).Value = "ActualAmount";
            workSheet.Cell(1, 8).Value = "RecoveryActual";
            workSheet.Cell(1, 9).Value = "RemainingActual";
            workSheet.Cell(1, 10).Value = "Interest";
            workSheet.Cell(1, 11).Value = "DepreciationAmount";
            workSheet.Cell(1, 12).Value = "RecoveryDepreciation";
            workSheet.Cell(1, 13).Value = "RemainingDepreciation";
            workSheet.Cell(1, 14).Value = "InstallmentNo";
            workSheet.Cell(1, 15).Value = "RemainingInstallmentNo";

            // car loans
            var carLoans = await _loanContract.GetCarLoans(isActive: 1);

            int row = 2;
            foreach (var item in carLoans)
            {
                var employee = await _employeeContract.GetEmployeeByjobCode(item.JobCode);
                var _installments = await _loanContract.GetCarLoanInstallments(item.Id);
                var _depreciations = await _loanContract.GetCarLoanDepreciationInstallments(item.Id);

                var monthlyInstallmentAmount = _installments[0].TotalPayment;
                _installments = _installments.Where(i => i.IsPaid == true && (i.MonthId >= fromMonthId && i.MonthId <= toMonthId)).ToList();
                var totalActualAmountRecovery = _installments.Sum(i => i.PrincipalAmount);
                var totalInterestRecovery = _installments.Sum(i => i.InterestAmount);

                _depreciations = _depreciations.Where(d => d.IsPaid == true && (d.MonthId >= fromMonthId && d.MonthId <= toMonthId)).ToList();
                var totalDepreciationRecovery = _depreciations.Sum(d => d.DepreciationAmount);

                item.RemainingActualAmount = item.ActualAmount - totalActualAmountRecovery;
                item.RemainingDepreciationAmount = item.DepreciationAmount - totalDepreciationRecovery;

                workSheet.Cell(row, 1).Value = item.JobCode;
                workSheet.Cell(row, 2).Value = employee.EmployeeName;
                workSheet.Cell(row, 3).Value = Convert.ToDateTime(item.LoanTakenDate).ToString("dd-MMM-yyyy");
                workSheet.Cell(row, 4).Value = item.TotalLoanAmount;
                workSheet.Cell(row, 5).Value = item.InterestRate;
                workSheet.Cell(row, 6).Value = monthlyInstallmentAmount;
                workSheet.Cell(row, 7).Value = item.ActualAmount;
                workSheet.Cell(row, 8).Value = totalActualAmountRecovery;
                workSheet.Cell(row, 9).Value = item.RemainingActualAmount;
                workSheet.Cell(row, 10).Value = totalInterestRecovery;
                workSheet.Cell(row, 11).Value = item.DepreciationAmount;
                workSheet.Cell(row, 12).Value = totalDepreciationRecovery;
                workSheet.Cell(row, 13).Value = item.RemainingDepreciationAmount;
                workSheet.Cell(row, 14).Value = item.InstallmentNo;
                workSheet.Cell(row, 15).Value = item.RemainingInstallmentNo;
                row++;
            }

            var stream = new MemoryStream();
            workBook.SaveAs(stream);
            stream.Position = 0;
            string excelName = $"car_loan_summary_{fromMonthId}_to_{toMonthId}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

    }
}
