using AspNetCore.Reporting;
using BgfclApp.DataSet;
using Contracts;
using Entities;
using Entities.ViewModels;
using Entities.ViewModels.Reports;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Reporting.NETCore;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Bibliography;
using System.ComponentModel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace BgfclApp.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private ISalarySettingContract _salarySettingContract;
        private IBankContract _bankContract;
        private IBranchContract _branchContract;
        private IBankTagContract _bankTagContract;
        private ISalaryReportOfficerContract _salaryReportOfficerContract;
        private IEmployeeTypeContract _employeeTypeContract;
        private IDepartmentContract _departmentContract;
        private IEmployeeContract _employeeContract;
        private ILoanContract _loanContract;
        public ReportsController(IWebHostEnvironment webHostEnvironment,
                                ISalarySettingContract salarySettingContract,
                                IBankContract bankContract,
                                IBankTagContract bankTagContract,
                                IEmployeeTypeContract employeeTypeContract,
                                 ISalaryReportOfficerContract salaryReportOfficerContract,
                                IDepartmentContract departmentContract,
                                IPFSheetReportContract pFSheetReportContract,
                                IEmployeeContract employeeContract,
                                ILoanContract loanContract,
                                IBranchContract branchContract


                                )
        {
            _webHostEnvironment = webHostEnvironment;
            _salarySettingContract = salarySettingContract;
            _bankContract = bankContract;
            _bankTagContract = bankTagContract;
            _employeeTypeContract = employeeTypeContract;
            _salaryReportOfficerContract = salaryReportOfficerContract;
            _departmentContract = departmentContract;
            _employeeContract = employeeContract;
            _loanContract = loanContract;
            _branchContract = branchContract;
        }


        #region salary
        public async Task<IActionResult> BankWiseSalaryBankForwarding()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> BankWiseSalaryBankForwarding(int month, int year, int employeeTypeId, int bankTagId, string isExcel, int reportType)
        {


            string monthsError = "";
            string yearsError = "";
            string employeeTypeIdError = "";
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
            if (bankTagId == 0)
            {
                bankError = "Select a Bank";
            }
            if (monthsError.Length > 0 || yearsError.Length > 0 || employeeTypeIdError.Length > 0 || bankError.Length > 0)
            {
                ViewBag.monthsError = monthsError;
                ViewBag.yearsError = yearsError;
                ViewBag.employeeTypeIdError = employeeTypeIdError;
                ViewBag.bankError = bankError;
                return View();
            }

            string monthName1 = new DateTime(year, month, 1).ToString("MMMM");
            string empTypeName1 = employeeTypeId == 1 ? "Permanent" :
                                 employeeTypeId == 2 ? "Contructual" : "Unknown";

            var processDataList = await _salarySettingContract.GetSalaryProcess();

            var isProcessed = processDataList.Any(p =>
                p.Month.Equals(monthName1, StringComparison.OrdinalIgnoreCase) &&
                p.Year == year //&&
                               //p.EmployeeType.Equals(empTypeName1, StringComparison.OrdinalIgnoreCase) &&
                               // p.Status.Equals("Salary already processsed", StringComparison.OrdinalIgnoreCase)
            );

            if (!isProcessed)
            {
                ViewBag.ProcessMessage = $"Salary not processed for {empTypeName1} Employee for {monthName1} {year} ";
                return View();
            }

            List<SalaryBankForwardViewModel> salaryBanks = new List<SalaryBankForwardViewModel>();
            int monthid = year * 100 + month;
            string netPayInWords = "";
            decimal grandNetPay = 0;
            List<BankViewModel> filteredBanks = new List<BankViewModel>();
            var bankTag = await _bankTagContract.GetBankTagById(bankTagId);
            var banks = await _bankContract.GetBanks();
            foreach (var bank in banks)
            {
                if (bank.BankTagName == bankTag.BankTagName)
                {
                    filteredBanks.Add(bank);
                }
            }

            if (employeeTypeId == 1)
            {
                var data = await _salarySettingContract.FinalAdjustmentOfficer(monthid);
                data = data.OrderBy(sb => sb.JobCode).ToList();
                foreach (var item in data)
                {
                    var checkBank = filteredBanks.Where(b => b.BankName == item.BankName).SingleOrDefault();
                    if (checkBank != null)
                    {
                        var grossPay = (Convert.ToDecimal(item.BasicSalary)) +
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
                                    (Convert.ToDecimal(item.CME));

                        var totalDeduction = (Convert.ToDecimal(item.RevenueStamp)) +
                                          (Convert.ToDecimal(item.WelfareFund)) +
                                          (Convert.ToDecimal(item.OfficerClub)) +
                                          (Convert.ToDecimal(item.OfficerAssociation)) +
                                          (Convert.ToDecimal(item.MedicalFund)) +
                                          (Convert.ToDecimal(item.PensionOfficer)) +
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
                                          (Convert.ToDecimal(item.CarLoan)) +
                                          (Convert.ToDecimal(item.CosLoan));

                        var netPay = grossPay - totalDeduction;
                        grandNetPay += netPay;

                        salaryBanks.Add(new SalaryBankForwardViewModel
                        {
                            JobCode = item.JobCode,
                            EmployeeName = item.EmployeeName,
                            BankName = item.BankName,
                            BranchName = item.BankBranchName,
                            AccountNumber = item.AccountNumber,
                            RoutingNo = item.RoutingNumber,
                            NetPay = Convert.ToDouble(netPay)
                        });
                    }
                }
            }
            if (employeeTypeId == 2)
            {
                var data = await _salarySettingContract.FinalAdjustmentJuniorStaff(monthid);
                data = data.OrderBy(sb => sb.JobCode).ToList();
                foreach (var item in data)
                {
                    var checkBank = filteredBanks.Where(b => b.BankName == item.BankName).SingleOrDefault();
                    if (checkBank != null)
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
                                    (Convert.ToDecimal(item.ConvenienceAllow)) +
                                    (Convert.ToDecimal(item.SpecialBenefit));

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
                                          (Convert.ToDecimal(item.Advance)) +
                                          (Convert.ToDecimal(item.HBLoan)) +
                                          (Convert.ToDecimal(item.MCylLoan)) +
                                          (Convert.ToDecimal(item.BCylLoan)) +
                                          (Convert.ToDecimal(item.ComputerLoan)) +
                                          (Convert.ToDecimal(item.PFLoan)) +
                                          (Convert.ToDecimal(item.WPFLoan)) +
                                          (Convert.ToDecimal(item.CosLoan));

                        var netPay = grossPay - totalDeduction;
                        grandNetPay += netPay;

                        salaryBanks.Add(new SalaryBankForwardViewModel
                        {
                            JobCode = item.JobCode,
                            EmployeeName = item.EmployeeName,
                            BankName = item.BankName,
                            BranchName = item.BankBranchName,
                            AccountNumber = item.AccountNumber,
                            RoutingNo = item.RoutingNumber,
                            NetPay = Convert.ToDouble(netPay)
                        });
                    }
                }
            }
            if (employeeTypeId == 3)
            {
                {
                    var data = await _salarySettingContract.FinalAdjustmentOfficer(monthid);
                    //data = data.OrderBy(sb => sb.JobCode).ToList();
                    foreach (var item in data)
                    {
                        var checkBank = filteredBanks.Where(b => b.BankName == item.BankName).SingleOrDefault();
                        if (checkBank != null)
                        {
                            var grossPay = (Convert.ToDecimal(item.BasicSalary)) +
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
                                        (Convert.ToDecimal(item.CME));

                            var totalDeduction = (Convert.ToDecimal(item.RevenueStamp)) +
                                              (Convert.ToDecimal(item.WelfareFund)) +
                                              (Convert.ToDecimal(item.OfficerClub)) +
                                              (Convert.ToDecimal(item.OfficerAssociation)) +
                                              (Convert.ToDecimal(item.MedicalFund)) +
                                              (Convert.ToDecimal(item.PensionOfficer)) +
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
                                              (Convert.ToDecimal(item.CarLoan)) +
                                              (Convert.ToDecimal(item.CosLoan));

                            var netPay = grossPay - totalDeduction;
                            grandNetPay += netPay;

                            salaryBanks.Add(new SalaryBankForwardViewModel
                            {
                                JobCode = item.JobCode,
                                EmployeeName = item.EmployeeName,
                                BankName = item.BankName,
                                BranchName = item.BankBranchName,
                                AccountNumber = item.AccountNumber,
                                RoutingNo = item.RoutingNumber,
                                NetPay = Convert.ToDouble(netPay)
                            });
                        }
                    }

                }

                {
                    var data = await _salarySettingContract.FinalAdjustmentJuniorStaff(monthid);
                    //data = data.OrderBy(sb => sb.JobCode).ToList();
                    foreach (var item in data)
                    {
                        var checkBank = filteredBanks.Where(b => b.BankName == item.BankName).SingleOrDefault();
                        if (checkBank != null)
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
                                        (Convert.ToDecimal(item.ConvenienceAllow)) +
                                        (Convert.ToDecimal(item.SpecialBenefit));

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
                                              (Convert.ToDecimal(item.Advance)) +
                                              (Convert.ToDecimal(item.HBLoan)) +
                                              (Convert.ToDecimal(item.MCylLoan)) +
                                              (Convert.ToDecimal(item.BCylLoan)) +
                                              (Convert.ToDecimal(item.ComputerLoan)) +
                                              (Convert.ToDecimal(item.PFLoan)) +
                                              (Convert.ToDecimal(item.WPFLoan)) +
                                              (Convert.ToDecimal(item.CosLoan));

                            var netPay = grossPay - totalDeduction;
                            grandNetPay += netPay;

                            salaryBanks.Add(new SalaryBankForwardViewModel
                            {
                                JobCode = item.JobCode,
                                EmployeeName = item.EmployeeName,
                                BankName = item.BankName,
                                BranchName = item.BankBranchName,
                                AccountNumber = item.AccountNumber,
                                RoutingNo = item.RoutingNumber,
                                NetPay = Convert.ToDouble(netPay)
                            });
                        }
                    }
                }

            }


            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);

            if (!String.IsNullOrEmpty(isExcel))
            {
                var workBook = new XLWorkbook();
                var workSheet = workBook.Worksheets.Add("Sheet1");

                workSheet.Cell(1, 1).Value = "JobCode";
                workSheet.Cell(1, 2).Value = "EmployeeName";
                workSheet.Cell(1, 3).Value = "AccountNumber";
                workSheet.Cell(1, 4).Value = "RoutingNumber";
                workSheet.Cell(1, 5).Value = "NetPay";

                int row = 2;

                foreach (var item in salaryBanks)
                {
                    workSheet.Cell(row, 1).Value = item.JobCode;
                    workSheet.Cell(row, 2).Value = item.EmployeeName;
                    workSheet.Cell(row, 3).Value = item.AccountNumber;
                    workSheet.Cell(row, 4).Value = item.RoutingNo;
                    workSheet.Cell(row, 5).Value = item.NetPay;
                    row++;
                }
                var stream = new MemoryStream();
                workBook.SaveAs(stream);
                stream.Position = 0;

                string excelName = "bank_wise_forwarding_";
                if (employeeTypeId == 1)
                {
                    excelName += "officer_";
                }
                else
                {
                    excelName += "junior_staff_";
                }
                excelName += monthName + "_" + year.ToString() + ".xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
            else
            {

                salaryBanks = salaryBanks
               .OrderBy(sb =>
               {
                   // Prioritize FO before JS
                   var prefix = sb.JobCode?.Substring(0, 2); // Get the first 2 characters as the prefix
                   if (prefix == "FO")
                   {
                       return 0; // FO will come first
                   }
                   else if (prefix == "JS")
                   {
                       return 1; // JS will come after FO
                   }

                   // If neither FO nor JS, treat it as a generic group
                   return 2;
               })
               .ThenBy(sb =>
               {
                   // Then, order by the numeric part of JobCode
                   var numericPart = sb.JobCode?.Substring(2);
                   int jobCodeValue;
                   return int.TryParse(numericPart, out jobCodeValue) ? jobCodeValue : int.MaxValue;
               })
               .ToList();

                var reType = "";
                if (reportType == 1)
                {
                    reType = "ORIGINAL";
                }
                else
                {
                    reType = "DUPLICATE";
                }

                netPayInWords = NumberToWordsConverter.Convert(grandNetPay);
                DateTime currentDate = DateTime.Now;
                string printDate = currentDate.ToString("MMMM dd, yyyy");
                var parameters = new[]
                {

                new Microsoft.Reporting.NETCore.ReportParameter("printDate",printDate.ToString()),
                new ReportParameter("reType",reType.ToString()),
                new ReportParameter("month",monthName.ToString()),
                new ReportParameter("year",year.ToString()),
                new ReportParameter("netPay",netPayInWords.ToString()),
            };
                string renderFormat = "PDF";
                string mimtype = "application/pdf";
                using var report = new Microsoft.Reporting.NETCore.LocalReport();
                report.EnableExternalImages = true;
                string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptSalaryBankForwarding.rdlc";
                report.DataSources.Add(new ReportDataSource("dsBankForward", salaryBanks));
                report.ReportPath = rptPath;
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                return File(pdf, mimtype);
            }


        }

        public async Task<IActionResult> SalaryBankForwarding()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SalaryBankForwarding(List<string> jobCodes, int month, int year, int employeeTypeId, int reportType, string isExcel)
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

            string monthName1 = new DateTime(year, month, 1).ToString("MMMM");
            string empTypeName1 = employeeTypeId == 1 ? "Salary Sheet for Permanent Employee" :
                                 employeeTypeId == 2 ? "Salary Sheet for Contract Employee" : " Wages Sheet for Daily Worker";

            var processDataList = await _salarySettingContract.GetSalaryProcess();

            var isProcessed = processDataList.Any(p =>
                p.Month.Equals(monthName1, StringComparison.OrdinalIgnoreCase) &&
                p.Year == year //&&
                               //p.EmployeeType.Equals(empTypeName1, StringComparison.OrdinalIgnoreCase) &&
                               // p.Status.Equals("Salary already processsed", StringComparison.OrdinalIgnoreCase)
            );

            if (!isProcessed)
            {
                ViewBag.ProcessMessage = $"Salary not processed for {empTypeName1} Employee for {monthName1} {year} ";
                return View();
            }


            if (monthsError.Length > 0 || yearsError.Length > 0 || employeeTypeIdError.Length > 0)
            {
                ViewBag.monthsError = monthsError;
                ViewBag.yearsError = yearsError;
                ViewBag.employeeTypeIdError = employeeTypeIdError;
                return View();
            }
            else
            {
                List<DailyWorker> dailyWorker = new List<DailyWorker>();
                List<SalaryBankForwardViewModel> salaryBanks = new List<SalaryBankForwardViewModel>();
                int monthid = year * 100 + month;
                string netPayInWords = "";
                decimal grandNetPay = 0;

                if (employeeTypeId == 1)
                {
                    var data = await _salaryReportOfficerContract.GetScSalarySheetPermanent(jobCodes, monthid);
                    foreach (var item in data)
                    {
                        var grossPay = (Convert.ToDecimal(item.BasicSalary)) +
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

                        var totalDeduction = (Convert.ToDecimal(item.PF)) +
                                    (Convert.ToDecimal(item.RevenueStamp)) +
                                    (Convert.ToDecimal(item.OtherDeduction));

                        var netPay = grossPay - totalDeduction;
                        grandNetPay += netPay;

                        salaryBanks.Add(new SalaryBankForwardViewModel
                        {
                            JobCode = item.JobCode,
                            EmployeeName = item.EmployeeName,
                            BankName = item.BankName,
                            AccountNumber = item.AccountNumber,
                            DesignationName = item.DesignationName,
                            NetPay = Convert.ToDouble(netPay)
                        });
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
                            "EMPLOYEE CODE", "EMP. NAME", "DESIGNATION", "BANK ACCOUNT NO.","NET PAY"
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
                        foreach (var item in salaryBanks)
                        {
                            worksheet.Cells[row, 1].Value = item.JobCode;
                            worksheet.Cells[row, 2].Value = item.EmployeeName;
                            worksheet.Cells[row, 3].Value = item.DesignationName;
                            worksheet.Cells[row, 4].Value = item.AccountNumber;
                            worksheet.Cells[row, 5].Value = item.NetPay;
                            row++;
                        }

                        // Auto-fit columns for better visibility
                        worksheet.Cells.AutoFitColumns();

                        // Save the package to a MemoryStream
                        using var stream = new MemoryStream();
                        package.SaveAs(stream);
                        var content = stream.ToArray();

                        // Return the Excel file as a download
                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BankSheet.xlsx");
                    }
                }
                if (employeeTypeId == 2)
                {
                    var data = await _salaryReportOfficerContract.GetScSalarySheetContructual(jobCodes, monthid);
                    var allBranch = await _branchContract.GetBranches();
                    foreach (var item in data)
                    {
                        var grossPay = (Convert.ToDecimal(item.BasicSalary)) +
                                    (Convert.ToDecimal(item.HouseRentAllow)) +
                                    (Convert.ToDecimal(item.FamilyMedicalAllow)) +
                                    (Convert.ToDecimal(item.ConvenienceAllow)) +
                                    (Convert.ToDecimal(item.ElectricityAllow)) +
                                    (Convert.ToDecimal(item.GasAllow)) +
                                    (Convert.ToDecimal(item.CtAllow)) +
                                    (Convert.ToDecimal(item.AcAllow)) +
                                    (Convert.ToDecimal(item.ArrearAllow)) +
                                    (Convert.ToDecimal(item.SpecialBenefit));

                        var totalDeduction = (Convert.ToDecimal(item.PF)) +
                                    (Convert.ToDecimal(item.RevenueStamp)) +
                                    (Convert.ToDecimal(item.OtherDeduction));
                        var netPay = grossPay - totalDeduction;
                        grandNetPay += netPay;
                        salaryBanks.Add(new SalaryBankForwardViewModel
                        {
                            JobCode = item.JobCode,
                            EmployeeName = item.EmployeeName,
                            BankName = item.BankName,
                            AccountNumber = item.AccountNumber,
                            DesignationName = item.DesignationName,
                            NetPay = Convert.ToDouble(netPay)
                        });
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
                            "EMPLOYEE CODE", "EMP. NAME", "DESIGNATION", "BANK ACCOUNT NO.","NET PAY"
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
                        foreach (var items in salaryBanks)
                        {
                            worksheet.Cells[row, 1].Value = items.JobCode;
                            worksheet.Cells[row, 2].Value = items.EmployeeName;
                            worksheet.Cells[row, 3].Value = items.DesignationName;
                            worksheet.Cells[row, 4].Value = items.AccountNumber;
                            worksheet.Cells[row, 5].Value = items.NetPay;
                            row++;
                        }

                        // Auto-fit columns for better visibility
                        worksheet.Cells.AutoFitColumns();

                        // Save the package to a MemoryStream
                        using var stream = new MemoryStream();
                        package.SaveAs(stream);
                        var content = stream.ToArray();

                        // Return the Excel file as a download
                        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BankSheet.xlsx");
                    }
                }
                if (employeeTypeId == 3)
                {
                    dailyWorker = await _salaryReportOfficerContract.GetScSalarySheetDailyWorker(jobCodes, monthid);
                    grandNetPay = dailyWorker.Sum(x => x.NetPay ?? 0);


                }
                var reType = "";
                if (reportType == 1)
                {
                    reType = "Bank Copy";
                }
                else
                {
                    reType = "Office Copy";
                }
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
                netPayInWords = NumberToWordsConverter.Convert(grandNetPay) + " Only";
                DateTime currentDate = DateTime.Now;
                string printDate = currentDate.ToString("M/d/yy, h:mm tt");
                var parameters = new[]
                {
                new Microsoft.Reporting.NETCore.ReportParameter("printDate",printDate.ToString()),
                new ReportParameter("reType",reType.ToString()),
                new ReportParameter("printDate",printDate.ToString()),
                new ReportParameter("month",monthName.ToString()),
                new ReportParameter("year",year.ToString()),
                new ReportParameter("netPay",netPayInWords.ToString()),
                new ReportParameter("employeeType",empTypeName1.ToString())
                };
                string renderFormat = "PDF";
                string mimtype = "application/pdf";
                using var report = new Microsoft.Reporting.NETCore.LocalReport();
                report.EnableExternalImages = true;
                string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptSalaryBankForwarding.rdlc";
                if (employeeTypeId == 3)
                {
                    report.DataSources.Add(new ReportDataSource("dsBankForward", dailyWorker));
                }
                else
                {
                    report.DataSources.Add(new ReportDataSource("dsBankForward", salaryBanks));
                }

                report.ReportPath = rptPath;
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                return File(pdf, mimtype);
            }

        }

        public async Task<IActionResult> BankWisePaymentSummary()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> BankWisePaymentSummary(int month, int year, int employeeTypeId, int bankTagId, int reportType)
        {
            string monthsError = "";
            string yearsError = "";
            string employeeTypeIdError = "";
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
            if (bankTagId == 0)
            {
                bankError = "Select a Bank";
            }
            if (monthsError.Length > 0 || yearsError.Length > 0 || employeeTypeIdError.Length > 0 || bankError.Length > 0)
            {
                ViewBag.monthsError = monthsError;
                ViewBag.yearsError = yearsError;
                ViewBag.employeeTypeIdError = employeeTypeIdError;
                ViewBag.bankError = bankError;
                return View();
            }


            List<BankWisePaymentSummaryViewModel> paymentSummary = new List<BankWisePaymentSummaryViewModel>();
            int monthid = year * 100 + month;
            string netPayInWords = "";
            decimal grandNetPay = 0;
            List<BankViewModel> filteredBanks = new List<BankViewModel>();
            var bankTag = await _bankTagContract.GetBankTagById(bankTagId);
            var banks = await _bankContract.GetBanks();
            foreach (var bank in banks)
            {
                if (bank.BankTagName == bankTag.BankTagName)
                {
                    filteredBanks.Add(bank);
                }
            }

            var reType = "";
            if (reportType == 1)
            {
                reType = "Original";
            }
            else
            {
                reType = "Duplicate";
            }

            if (employeeTypeId == 1)
            {
                //foreach (var filteredBank in filteredBanks)
                //{
                //    var branches = 
                //}
                var data = await _salarySettingContract.FinalAdjustmentOfficer(monthid);

                foreach (var item in data)
                {
                    var checkBank = filteredBanks.Where(b => b.BankName == item.BankName).SingleOrDefault();
                    if (checkBank != null)
                    {
                        var grossPay = (Convert.ToDecimal(item.BasicSalary)) +
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
                                    (Convert.ToDecimal(item.CME));

                        var totalDeduction = (Convert.ToDecimal(item.RevenueStamp)) +
                                          (Convert.ToDecimal(item.WelfareFund)) +
                                          (Convert.ToDecimal(item.OfficerClub)) +
                                          (Convert.ToDecimal(item.OfficerAssociation)) +
                                          (Convert.ToDecimal(item.MedicalFund)) +
                                          (Convert.ToDecimal(item.PensionOfficer)) +
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
                                          (Convert.ToDecimal(item.CarLoan)) +
                                          (Convert.ToDecimal(item.CosLoan));

                        var netPay = grossPay - totalDeduction;
                        grandNetPay += netPay;
                        paymentSummary.Add(new BankWisePaymentSummaryViewModel
                        {
                            BankName = item.BankName,
                            BranchName = item.BankBranchName,
                            NetPay = Convert.ToDouble(netPay)
                        });
                    }
                }
            }
            if (employeeTypeId == 2)
            {
                var data = await _salarySettingContract.FinalAdjustmentJuniorStaff(monthid);
                foreach (var item in data)
                {
                    var checkBank = filteredBanks.Where(b => b.BankName == item.BankName).SingleOrDefault();
                    if (checkBank != null)
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
                                    (Convert.ToDecimal(item.ConvenienceAllow)) +
                                    (Convert.ToDecimal(item.SpecialBenefit));

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
                                          (Convert.ToDecimal(item.Advance)) +
                                          (Convert.ToDecimal(item.HBLoan)) +
                                          (Convert.ToDecimal(item.MCylLoan)) +
                                          (Convert.ToDecimal(item.BCylLoan)) +
                                          (Convert.ToDecimal(item.ComputerLoan)) +
                                          (Convert.ToDecimal(item.PFLoan)) +
                                          (Convert.ToDecimal(item.WPFLoan)) +
                                          (Convert.ToDecimal(item.CosLoan));

                        var netPay = grossPay - totalDeduction;
                        grandNetPay += netPay;

                        paymentSummary.Add(new BankWisePaymentSummaryViewModel
                        {
                            BankName = item.BankName,
                            BranchName = item.BankBranchName,
                            NetPay = Convert.ToDouble(netPay)
                        });
                    }
                }
            }


            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
            netPayInWords = NumberToWordsConverter.Convert(grandNetPay);
            DateTime currentDate = DateTime.Now;
            string printDate = currentDate.ToString("MMMM dd, yyyy");
            var parameters = new[]
            {
                new ReportParameter("printDate",printDate.ToString()),
                 new ReportParameter("reType",reType.ToString()),
                new ReportParameter("month",monthName.ToString()),
                new ReportParameter("year",year.ToString()),

            };
            string renderFormat = "PDF";
            string mimtype = "application/pdf";
            using var report = new Microsoft.Reporting.NETCore.LocalReport();
            report.EnableExternalImages = true;
            string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptBankPaymentSummary.rdlc";
            report.DataSources.Add(new ReportDataSource("dsBankPaymentSummary", paymentSummary));
            report.ReportPath = rptPath;
            report.SetParameters(parameters);
            var pdf = report.Render(renderFormat);
            return File(pdf, mimtype);

        }
        [HttpGet]
        public async Task<IActionResult> SalaryJournal()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SalaryJournal(int month, int year, int employeeTypeId)
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

            if (monthsError.Length > 0 || yearsError.Length > 0 || employeeTypeIdError.Length > 0)
            {
                ViewBag.monthsError = monthsError;
                ViewBag.yearsError = yearsError;
                ViewBag.employeeTypeIdError = employeeTypeIdError;
                return View();
            }

            int monthid = year * 100 + month;
            var dataSource = "";
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
            List<SalaryJournal> salaryJournals = new List<SalaryJournal>();
            var journalMasterData = await _salarySettingContract.GetSalaryJournalMaster(monthid, employeeTypeId);
            if (journalMasterData != null)
            {
                salaryJournals.AddRange(await _salarySettingContract.GetSalaryJournalsByMasterId(journalMasterData.Id));
            }
            salaryJournals = salaryJournals
                            .OrderByDescending(j => j.Details == "Payroll Suspense A/C")
                            .ThenBy(j => j.Details != "Payroll Suspense A/C")
                            .ToList();


            var parameters = new[]
            {
                new ReportParameter("empType",empType.ToString()),
                new ReportParameter("month",monthName.ToString()),
                new ReportParameter("year",year.ToString()),

            };
            string renderFormat = "PDF";
            string mimtype = "application/pdf";
            using var report = new Microsoft.Reporting.NETCore.LocalReport();
            report.EnableExternalImages = true;
            string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptSalaryJournal.rdlc";
            report.DataSources.Add(new ReportDataSource("dsSalaryJournal", salaryJournals));
            report.ReportPath = rptPath;
            report.SetParameters(parameters);
            var pdf = report.Render(renderFormat);
            return File(pdf, mimtype);
        }
        #endregion

        #region general
        [HttpGet]
        public async Task<IActionResult> LoanSheet()
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
        public async Task<IActionResult> LoanSheet(List<string> jobCode, int month, int year, int employeeTypeId)
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
                ViewBag.employeeTypeIdError = employeeTypeIdError;
                return View();
            }

            var empType = "";
            if (employeeTypeId == 1)
            {
                empType = "Officer";
            }
            if (employeeTypeId == 2)
            {
                empType = "Junior Staff";
            }
            var monthid = year * 100 + month;
            var source = await _salarySettingContract.GetLoanSheetData(monthid, employeeTypeId);

            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
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
            string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptLoanSheet.rdlc";
            report.DataSources.Add(new ReportDataSource("dsLoanSheet", source));
            report.ReportPath = rptPath;
            report.SetParameters(parameters);
            var pdf = report.Render(renderFormat);
            return File(pdf, mimtype);

        }
        [HttpGet]
        public async Task<IActionResult> EmployeeUnionORClub()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> EmployeeUnionORClub(int month, int year, int reportType)
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

            if (reportType == 1)
            {
                var monthid = year * 100 + month;
                var source = await _salarySettingContract.EmployeeUnionORClubData(monthid, reportType);


                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
                DateTime currentDate = DateTime.Now;
                string printDate = currentDate.ToString("MMMM dd, yyyy");
                var parameters = new[]
                {
                new ReportParameter("printDate",printDate.ToString()),
                new ReportParameter("month",monthName.ToString()),
                new ReportParameter("year",year.ToString()),

                 };
                string renderFormat = "PDF";
                string mimtype = "application/pdf";
                using var report = new Microsoft.Reporting.NETCore.LocalReport();
                report.EnableExternalImages = true;
                string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptEmployeeUnion.rdlc";
                report.DataSources.Add(new ReportDataSource("dsEmployeeUnionClub", source));
                report.ReportPath = rptPath;
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                return File(pdf, mimtype);
            }
            else
            {
                var monthid = year * 100 + month;
                var source = await _salarySettingContract.EmployeeUnionORClubData(monthid, reportType);



                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
                DateTime currentDate = DateTime.Now;
                string printDate = currentDate.ToString("MMMM dd, yyyy");
                var parameters = new[]
                {
                new ReportParameter("printDate",printDate.ToString()),
                new ReportParameter("month",monthName.ToString()),
                new ReportParameter("year",year.ToString()),

                };
                string renderFormat = "PDF";
                string mimtype = "application/pdf";
                using var report = new Microsoft.Reporting.NETCore.LocalReport();
                report.EnableExternalImages = true;
                string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptEmployeeClub.rdlc";
                report.DataSources.Add(new ReportDataSource("dsEmployeeUnionClub", source));
                report.ReportPath = rptPath;
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                return File(pdf, mimtype);
            }

        }


        public async Task<IActionResult> OfficerAssoORClub()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> OfficerAssoORClub(int month, int year, int reportType)
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

            if (reportType == 1)
            {
                var monthid = year * 100 + month;
                var source = await _salarySettingContract.OfficerAssoORClubData(monthid, reportType);
                var filteredList = source
                .Where(s => s.OfficerAssociation != 0).ToList();


                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
                DateTime currentDate = DateTime.Now;
                string printDate = currentDate.ToString("MMMM dd, yyyy");
                var parameters = new[]
                {
                new ReportParameter("printDate",printDate.ToString()),
                new ReportParameter("month",monthName.ToString()),
                new ReportParameter("year",year.ToString()),

                 };
                string renderFormat = "PDF";
                string mimtype = "application/pdf";
                using var report = new Microsoft.Reporting.NETCore.LocalReport();
                report.EnableExternalImages = true;
                string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptOfficerAssociation.rdlc";
                report.DataSources.Add(new ReportDataSource("dsOfficerAssoClub", filteredList));
                report.ReportPath = rptPath;
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                return File(pdf, mimtype);
            }
            else
            {
                var monthid = year * 100 + month;
                var source = await _salarySettingContract.OfficerAssoORClubData(monthid, reportType);
                var filteredList = source
               .Where(s => s.OfficerClub != 0).ToList();


                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
                DateTime currentDate = DateTime.Now;
                string printDate = currentDate.ToString("MMMM dd, yyyy");
                var parameters = new[]
                {
                new ReportParameter("printDate",printDate.ToString()),
                new ReportParameter("month",monthName.ToString()),
                new ReportParameter("year",year.ToString()),

                };
                string renderFormat = "PDF";
                string mimtype = "application/pdf";
                using var report = new Microsoft.Reporting.NETCore.LocalReport();
                report.EnableExternalImages = true;
                string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptOfficerClub.rdlc";
                report.DataSources.Add(new ReportDataSource("dsOfficerAssoClub", filteredList));
                report.ReportPath = rptPath;
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                return File(pdf, mimtype);
            }

        }


        [HttpGet]
        public async Task<IActionResult> WelfareFund()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> WelfareFund(int month, int year, int employeeTypeId)
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
            var empType = "";
            if (employeeTypeId == 1)
            {
                empType = "Officer";
            }
            if (employeeTypeId == 2)
            {
                empType = "Junior Staff";
            }
            if (employeeTypeId == 3)
            {
                empType = "Pension Offficer";
            }
            if (employeeTypeId == 4)
            {
                empType = "Pension Junior Staff";
            }

            if (monthsError.Length > 0 || yearsError.Length > 0 || employeeTypeIdError.Length > 0)
            {
                ViewBag.monthsError = monthsError;
                ViewBag.yearsError = yearsError;
                ViewBag.employeeTypeIdError = employeeTypeIdError;
                return View();
            }

            var monthid = year * 100 + month;
            var source = await _salarySettingContract.WelfarefundData(monthid, employeeTypeId);
            source = source.Where(c => c.WelfareFund != 0).OrderBy(b => b.JobCode).ToList();

            string mimetype = "";
            int extension = 1;
            var path = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptWelfareFund.rdlc";
            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
            DateTime currentDate = DateTime.Now;
            string printDate = currentDate.ToString("MMMM dd, yyyy");
            var parameters = new[]
            {
                new ReportParameter("printDate",printDate.ToString()),
                new ReportParameter("month",monthName.ToString()),
                 new ReportParameter("empType",empType.ToString()),
                new ReportParameter("year",year.ToString()),

            };
            string renderFormat = "PDF";
            string mimtype = "application/pdf";
            using var report = new Microsoft.Reporting.NETCore.LocalReport();
            report.EnableExternalImages = true;
            string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptWelfareFund.rdlc";
            report.DataSources.Add(new ReportDataSource("dsWelfareFund", source));
            report.ReportPath = rptPath;
            report.SetParameters(parameters);
            var pdf = report.Render(renderFormat);
            return File(pdf, mimtype);

        }




        public async Task<IActionResult> PFSheet()
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
        public async Task<IActionResult> PFSheet(List<string> jobCode, int month, int year, int employeeTypeId, string isExcel)
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
            var empType = "";
            if (employeeTypeId == 1)
            {
                empType = "Permanent";
            }
            if (employeeTypeId == 2)
            {
                empType = "Contract";
            }
            if (employeeTypeId == 3)
            {
                empType = "Pension Offficer";
            }
            if (employeeTypeId == 4)
            {
                empType = "Pension Junior Staff";
            }

            if (monthsError.Length > 0 || yearsError.Length > 0 || employeeTypeIdError.Length > 0)
            {
                ViewBag.monthsError = monthsError;
                ViewBag.yearsError = yearsError;
                ViewBag.employeeTypeIdError = employeeTypeIdError;
                return View();
            }

            string monthName1 = new DateTime(year, month, 1).ToString("MMMM");
            string empTypeName1 = employeeTypeId == 1 ? "Permanent" :
                                 employeeTypeId == 2 ? "Contract" : "Unknown";

            var processDataList = await _salarySettingContract.GetSalaryProcess();

            var isProcessed = processDataList.Any(p =>
                p.Month.Equals(monthName1, StringComparison.OrdinalIgnoreCase) &&
                p.Year == year &&
                p.EmployeeType.Equals(empTypeName1, StringComparison.OrdinalIgnoreCase)
            //p.Status.Equals("Salary already processsed", StringComparison.OrdinalIgnoreCase)
            );

            if (!isProcessed)
            {
                ViewBag.ProcessMessage = $"Salary not processed for {empTypeName1} Employee for {monthName1} {year} ";
                return View();
            }
            var monthid = year * 100 + month;
            var source = await _salarySettingContract.GetPFSheetData(jobCode, monthid, employeeTypeId);
            decimal grandTotal = (decimal)source.Sum(item => item.OwnContribution) * 2;
            string netPayInWords = NumberToWordsConverter.Convert(grandTotal);
            //var sortedListdata = source.OrderBy(b => b.JobCode).ToList();
            foreach (var item in source)
            {
                item.NetPay = item.OwnContribution + item.CompanyContribution;
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
                            "EMPLOYEE CODE", "EMP. NAME", "DESIGNATION", "BASIC PAY.","OWN CONT.","INSTITUTE CONT.","NET PAY"
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
                    worksheet.Cells[row, 4].Value = items.BasicSalary;
                    worksheet.Cells[row, 5].Value = items.OwnContribution;
                    worksheet.Cells[row, 6].Value = items.CompanyContribution;
                    worksheet.Cells[row, 7].Value = items.NetPay;
                    row++;
                }

                // Auto-fit columns for better visibility
                worksheet.Cells.AutoFitColumns();

                // Save the package to a MemoryStream
                using var stream = new MemoryStream();
                package.SaveAs(stream);
                var content = stream.ToArray();

                // Return the Excel file as a download
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PFSheet.xlsx");
            }

            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
            DateTime currentDate = DateTime.Now;
            string printDate = currentDate.ToString("MMMM dd, yyyy");


            DateTime currentDateTime = DateTime.Now;
            string printDateTime = currentDate.ToString("M/d/yy, h:mm tt");

            var parameters = new[]
            {
                new ReportParameter("printDateTime",printDateTime.ToString()),
                 new ReportParameter("printDate",printDate.ToString()),
                new ReportParameter("netPayInWords",netPayInWords.ToString()),
                new ReportParameter("month",monthName.ToString()),
                new ReportParameter("empType",empType.ToString()),
                new ReportParameter("year",year.ToString()),

            };
            string renderFormat = "PDF";
            string mimtype = "application/pdf";
            using var report = new Microsoft.Reporting.NETCore.LocalReport();
            report.EnableExternalImages = true;
            string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptPFSheet.rdlc";
            report.DataSources.Add(new ReportDataSource("dsPFSheet", source));
            report.ReportPath = rptPath;
            report.SetParameters(parameters);
            var pdf = report.Render(renderFormat);
            return File(pdf, mimtype);

        }
        [HttpGet]
        public async Task<IActionResult> PFForwarding()
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
        public async Task<IActionResult> PFForwarding(List<string> jobCode, int month, int year, int employeeTypeId, string Accounts)
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
            var empType = "";
            if (employeeTypeId == 1)
            {
                empType = "Permanent";
            }
            if (employeeTypeId == 2)
            {
                empType = "Contructual";
            }

            if (monthsError.Length > 0 || yearsError.Length > 0 || employeeTypeIdError.Length > 0)
            {
                ViewBag.monthsError = monthsError;
                ViewBag.yearsError = yearsError;
                ViewBag.employeeTypeIdError = employeeTypeIdError;
                return View();
            }
            string monthName1 = new DateTime(year, month, 1).ToString("MMMM");
            string empTypeName1 = employeeTypeId == 1 ? "Permanent" :
                                 employeeTypeId == 2 ? "Contructual" : "Unknown";

            var processDataList = await _salarySettingContract.GetSalaryProcess();

            var isProcessed = processDataList.Any(p =>
                p.Month.Equals(monthName1, StringComparison.OrdinalIgnoreCase) &&
                p.Year == year &&
                p.EmployeeType.Equals(empTypeName1, StringComparison.OrdinalIgnoreCase)
            // p.Status.Equals("Salary already processsed", StringComparison.OrdinalIgnoreCase)
            );

            if (!isProcessed)
            {
                ViewBag.ProcessMessage = $"Salary not processed for {empTypeName1} Employee for {monthName1} {year} ";
                return View();
            }
            var monthid = year * 100 + month;
            var source = await _salarySettingContract.GetPFSheetData(jobCode, monthid, employeeTypeId);
            //decimal grandTotal = (decimal)source.Sum(item => item.OwnContribution) * 2;
            //string netPayInWords = NumberToWordsConverter.Convert(grandTotal);

            decimal grandTotal = (decimal)source.Sum(item => item.OwnContribution) * 2;

            // Format with commas and two decimal places
            string formattedGrandTotal = grandTotal.ToString("#,##0.00");

            // If you also want the number in words:
            string netPayInWords = NumberToWordsConverter.Convert(grandTotal);

            var sortedListdata = source.OrderBy(b => b.JobCode).ToList();
            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
            DateTime currentDate = DateTime.Now;
            string printDate = currentDate.ToString("MMMM dd, yyyy");
            string monthYear = $"{monthName} - {year}";


            DateTime currentDateTime = DateTime.Now;
            string printDateTime = currentDate.ToString("M/d/yy, h:mm tt");
            var parameters = new[]
            {
                new ReportParameter("Accounts",Accounts.ToString()),
                new ReportParameter("printDate",printDate.ToString()),
                new ReportParameter("printDateTime",printDateTime.ToString()),
                new ReportParameter("grandTotal",formattedGrandTotal.ToString()),
                new ReportParameter("netPayInWords",netPayInWords.ToString()),
                new ReportParameter("month",monthName.ToString()),
                new ReportParameter("empType",empType.ToString()),
                new ReportParameter("monthYear",monthYear.ToString()),
                new ReportParameter("year",year.ToString()),

            };
            string renderFormat = "PDF";
            string mimtype = "application/pdf";
            using var report = new Microsoft.Reporting.NETCore.LocalReport();
            report.EnableExternalImages = true;
            string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptPFForwarding.rdlc";
            report.DataSources.Add(new ReportDataSource("dsPFSheet", sortedListdata));
            report.ReportPath = rptPath;
            report.SetParameters(parameters);
            var pdf = report.Render(renderFormat);
            return File(pdf, mimtype);

        }

        [HttpGet]
        public async Task<IActionResult> YearlyPFSheet()
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
        public async Task<IActionResult> YearlyPFSheet(List<string> jobCode, int EmployeeTypeId, int fmonth, int fyear, int tmonth, int tyear, string department)
        {


            var fmonthid = fyear * 100 + fmonth;
            var tmonthid = tyear * 100 + tmonth;
            var source = await _salarySettingContract.GetYearlyPFSheetData(jobCode, EmployeeTypeId, fmonthid, tmonthid, department);

            string fmonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(fmonth);
            string tmonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(tmonth);
            var empType = "";
            if (EmployeeTypeId == 1)
            {
                empType = "Officer";
            }
            if (EmployeeTypeId == 2)
            {
                empType = "Junior Staff";
            }
            if (EmployeeTypeId == 3)
            {
                empType = "Pension Officer";
            }
            if (EmployeeTypeId == 4)
            {
                empType = "Pension Junior Staff";
            }
            DateTime currentDate = DateTime.Now;
            string printDate = currentDate.ToString("MMMM dd, yyyy");
            var parameters = new[]
            {
                new ReportParameter("printDate",printDate.ToString()),
                new ReportParameter("fmonth",fmonthName.ToString()),
                new ReportParameter("fyear",fyear.ToString()),
                new ReportParameter("tmonth",tmonthName.ToString()),
                new ReportParameter("tyear",tyear.ToString()),
                new ReportParameter("empType",empType.ToString()),

            };
            string renderFormat = "PDF";
            string mimtype = "application/pdf";
            using var report = new Microsoft.Reporting.NETCore.LocalReport();
            report.EnableExternalImages = true;
            string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptPFSheetYearly.rdlc";
            report.DataSources.Add(new ReportDataSource("dsPFSheet", source));
            report.ReportPath = rptPath;
            report.SetParameters(parameters);
            var pdf = report.Render(renderFormat);
            return File(pdf, mimtype);

        }

        public async Task<IActionResult> PensionSheet()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PensionSheet(int month, int year, int employeeTypeId)
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

            if (monthsError.Length > 0 || yearsError.Length > 0 || employeeTypeIdError.Length > 0)
            {
                ViewBag.monthsError = monthsError;
                ViewBag.yearsError = yearsError;
                ViewBag.employeeTypeIdError = employeeTypeIdError;
                return View();
            }

            var empType = "";
            if (employeeTypeId == 3)
            {
                empType = "Pension Officer";
            }
            if (employeeTypeId == 4)
            {
                empType = "Pension Junior Staff";
            }

            var monthid = year * 100 + month;
            var source = await _salarySettingContract.GetPensionSheetData(monthid, employeeTypeId);
            var sortedSource = source.OrderBy(s => s.JobCode).ToList();

            foreach (var item in source)
            {
                var salaryPolicySettings = await _salarySettingContract.GetSalaryPolicySettings();
                var employeeType = employeeTypeId == 3 ? 1 : 2;
                item.PensionRate = salaryPolicySettings.Where(p => p.EmployeeTypeId == employeeType).SingleOrDefault().PensionRate;
            }

            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
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
            string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptPensionSheet.rdlc";
            report.DataSources.Add(new ReportDataSource("dsPensionSheet", sortedSource));
            report.ReportPath = rptPath;
            report.SetParameters(parameters);
            var pdf = report.Render(renderFormat);
            return File(pdf, mimtype);

        }


        public async Task<IActionResult> YearlyPensionSheet()
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
        public async Task<IActionResult> YearlyPensionSheet(List<string> jobCode, int EmployeeTypeId, int fmonth, int fyear, int tmonth, int tyear, string department)
        {

            int fmonthid = fyear * 100 + fmonth;
            int tmonthid = tyear * 100 + tmonth;

            var source = await _salarySettingContract.GetYearlyPensionSheetData(EmployeeTypeId, jobCode, fmonthid, tmonthid, department);

            foreach (var item in source)
            {
                var salaryPolicySettings = await _salarySettingContract.GetSalaryPolicySettings();
                var employeeType = EmployeeTypeId == 3 ? 1 : 2;
                item.PensionRate = salaryPolicySettings.Where(p => p.EmployeeTypeId == employeeType).SingleOrDefault().PensionRate;
            }

            string fmonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(fmonth);
            string tmonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(tmonth);
            var empType = "";
            if (EmployeeTypeId == 3)
            {
                empType = "Pension Officer";
            }
            else
            {
                empType = "Pension Junior Staff";
            }
            DateTime currentDate = DateTime.Now;
            string printDate = currentDate.ToString("MMMM dd, yyyy");
            var parameters = new[]
            {
                new ReportParameter("printDate",printDate.ToString()),
                new ReportParameter("fmonth",fmonthName.ToString()),
                new ReportParameter("fyear",fyear.ToString()),
                new ReportParameter("tmonth",tmonthName.ToString()),
                new ReportParameter("tyear",tyear.ToString()),
                 new ReportParameter("empType",empType.ToString()),

            };
            string renderFormat = "PDF";
            string mimtype = "application/pdf";
            using var report = new Microsoft.Reporting.NETCore.LocalReport();
            report.EnableExternalImages = true;
            string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptPensionSheetYearly.rdlc";
            report.DataSources.Add(new ReportDataSource("dsPensionSheet", source));
            report.ReportPath = rptPath;
            report.SetParameters(parameters);
            var pdf = report.Render(renderFormat);
            return File(pdf, mimtype);

        }
        public async Task<IActionResult> PFDeduction()
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
        public async Task<IActionResult> PFDeduction(List<string> jobCode, int month, int year, int employeeTypeId)
        {
            string monthsError = "";
            string yearsError = "";
            string employeeTypeIdError = "";
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

            if (monthsError.Length > 0 || yearsError.Length > 0 || employeeTypeIdError.Length > 0)
            {
                ViewBag.monthsError = monthsError;
                ViewBag.yearsError = yearsError;
                ViewBag.employeeTypeIdError = employeeTypeIdError;
                ViewBag.bankError = bankError;
                return View();
            }


            var monthid = year * 100 + month;
            var source = await _salarySettingContract.GetPFDeductionData(jobCode, monthid, employeeTypeId);

            source = source
            .OrderBy(sb =>
            {
                var numericPart = sb.JobCode?.Substring(2);
                int jobCodeValue;
                return int.TryParse(numericPart, out jobCodeValue) ? jobCodeValue : int.MaxValue;
            }).ToList();

            if (employeeTypeId == 1)
            {
                foreach (var item in source)
                {
                    var salaryPolicySettings = await _salarySettingContract.GetSalarySettingsOfficer();
                    var matchedSetting = salaryPolicySettings.FirstOrDefault(c => c.JobCode == item.JobCode);

                    if (matchedSetting != null)
                    {
                        item.GpfRate = matchedSetting.GpfRate;
                    }
                    else
                    {
                        item.GpfRate = 0;
                    }
                }
            }
            else
            {
                foreach (var item in source)
                {
                    var salaryPolicySettings = await _salarySettingContract.GetSalarySettingsJuniorStaff();
                    var matchedSetting = salaryPolicySettings.FirstOrDefault(c => c.JobCode == item.JobCode);

                    if (matchedSetting != null)
                    {
                        item.GpfRate = matchedSetting.GpfRate;
                    }
                    else
                    {
                        item.GpfRate = 0;
                    }
                }
            }

            decimal totalBasicSalary = source.Sum(item => item.BasicSalary);
            decimal totalOwnContribution = (decimal)source.Sum(item => item.OwnContribution);
            string totalBasicInWords = NumberToWordsConverter.Convert(totalBasicSalary);
            string totalContriInWords = NumberToWordsConverter.Convert(totalOwnContribution);

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
            DateTime currentDate = DateTime.Now;
            string printDate = currentDate.ToString("MMMM dd, yyyy");
            var parameters = new[]
            {
                    new Microsoft.Reporting.NETCore.ReportParameter("totalBasicInWords",totalBasicInWords.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("totalContriInWords",totalContriInWords.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("printDate",printDate.ToString()),
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

        [HttpGet]
        public async Task<IActionResult> YearlyGPFDeduction()
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
        public async Task<IActionResult> YearlyGPFDeduction(List<string> jobCode, int fmonth, int fyear, int tmonth, int tyear, int employeeTypeId, string department)
        {


            var fmonthid = fyear * 100 + fmonth;
            var tmonthid = tyear * 100 + tmonth;
            var source = await _salarySettingContract.YearlyGPFDeductionData(jobCode, fmonthid, tmonthid, department, employeeTypeId);

            var filteredAndOrderedData = source
                .Where(data => data.OwnContribution != null && data.OwnContribution > 0)
                .OrderBy(data => data.JobCode)
                .ToList();


            //source = source.OrderBy(s =>
            //{
            //    // Split the MonthYear into parts
            //    var parts = s.MonthYear.Split('-');
            //    var monthName = parts[0];
            //    var year = int.Parse(parts[1]);

            //    // Convert month name to month number
            //    var month = DateTime.ParseExact(monthName, "MMMM", CultureInfo.InvariantCulture).Month;

            //    // Create a sortable value (e.g., using DateTime)
            //    return new DateTime(year, month, 1);
            //}).ToList();
            string fmonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(fmonth);
            string tmonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(tmonth);

            var empType = "";
            if (employeeTypeId == 1)
            {
                empType = "Officer";
            }
            else
            {
                empType = "Junior Staff";
            }

            DateTime currentDate = DateTime.Now;
            string printDate = currentDate.ToString("MMMM dd, yyyy");
            var parameters = new[]
            {
                    new Microsoft.Reporting.NETCore.ReportParameter("printDate",printDate.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("fmonth",fmonthName.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("empType",empType.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("fyear",fyear.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("tmonth",tmonthName.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("tyear",tyear.ToString()),
                };
            string renderFormat = "PDF";
            string mimtype = "application/pdf";
            using var report = new Microsoft.Reporting.NETCore.LocalReport();
            report.EnableExternalImages = true;
            string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptGPFDeductionYearly.rdlc";
            report.DataSources.Add(new ReportDataSource("dsPFDeductionOF", filteredAndOrderedData));
            report.ReportPath = rptPath;
            report.SetParameters(parameters);
            var pdf = report.Render(renderFormat);
            return File(pdf, mimtype);
        }


        public IActionResult GetAmortizationSchedule()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            return View();
        }

        [HttpPost]
        public IActionResult GetAmortizationSchedule(int reportType, decimal loanAmount, decimal interestRate, int installmentNumber, int installNoOfInterest, DateTime startDate)
        {
            if (reportType == 1)
            {
                var source = CalculateLoanReport(loanAmount, interestRate, installmentNumber, installNoOfInterest, startDate);

                //var path = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptAmortizationSchedule.rdlc";
                DateTime currentDate = DateTime.Now;
                string printDate = currentDate.ToString("MMMM dd, yyyy");
                var parameters = new[]
                {
                     new ReportParameter("printDate",printDate.ToString()),

                };

                string renderFormat = "PDF";
                string mimtype = "application/pdf";
                using var report = new Microsoft.Reporting.NETCore.LocalReport();
                report.EnableExternalImages = true;
                string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptAmortizationSchedule.rdlc";
                report.DataSources.Add(new ReportDataSource("dsAmortizationSchedule", source));
                report.ReportPath = rptPath;
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                return File(pdf, mimtype);


                // Dictionary<string, string> parameters = new Dictionary<string, string>();


                //AspNetCore.Reporting.LocalReport localReport = new AspNetCore.Reporting.LocalReport(path);
                //localReport.AddDataSource("dsAmortizationSchedule", report);
                //var result = localReport.Execute(RenderType.Pdf, extension, parameters, mimetype);
                //return File(result.MainStream, "application/pdf");
            }
            else
            {
                var source = CalculateInterestInstalment(loanAmount, interestRate, installmentNumber, installNoOfInterest, startDate);

                DateTime currentDate = DateTime.Now;
                string printDate = currentDate.ToString("MMMM dd, yyyy");
                var parameters = new[]
                {
                     new ReportParameter("printDate",printDate.ToString()),

                };

                string renderFormat = "PDF";
                string mimtype = "application/pdf";
                using var report = new Microsoft.Reporting.NETCore.LocalReport();
                report.EnableExternalImages = true;
                string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptAmortizationInterest.rdlc";
                report.DataSources.Add(new ReportDataSource("dsAmortizationInterest", source));
                report.ReportPath = rptPath;
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                return File(pdf, mimtype);




                //Dictionary<string, string> parameters = new Dictionary<string, string>();

                //AspNetCore.Reporting.LocalReport localReport = new AspNetCore.Reporting.LocalReport(path);
                //localReport.AddDataSource("dsAmortizationInterest", report);

                //var result = localReport.Execute(RenderType.Pdf, extension, parameters, mimetype);

                //return File(result.MainStream, "application/pdf");
            }

        }


        #endregion


        // Computer Individual Loan Report
        [HttpGet]
        public async Task<IActionResult> GetComputerLoan()
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
                Value = d.JobCode.ToString(),
                Text = $"{d.JobCode} - {d.EmployeeName}"
            }).ToList();

            ViewBag.EmployeeCode = CodeList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetComputerLoan(string jobCode, int fmonth, int fyear, int tmonth, int tyear, int yearclosingmonth, int yearclosingyear, int employeeTypeId, int reportType)
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
            if (employeeTypeId == 0 && reportType == 1)
            {
                employeeTypeIdError = "Select Employee Type";
            }
            if (String.IsNullOrEmpty(jobCode) && reportType == 1)
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


            var offsetMonthId = DateTime.Now.Year * 100 + DateTime.Now.Day;
            if (reportType == 1)
            {
                List<ComLoanInfoVM> individualComputerLoanReportViewModels = new List<ComLoanInfoVM>();
                ComLoan loanData = new ComLoan();
                var type = "Individual";
                var fmonthid = fyear * 100 + fmonth;
                var tmonthid = tyear * 100 + tmonth;
                string fmonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(fmonth);
                string tmonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(tmonth);
                EmployeeViewModel employee = new EmployeeViewModel();
                double? recoveryprincipal = 0;
                double? recoveryInterest = 0;
                double? prevRecoveryPrincipal = 0;
                var comloans = await _loanContract.GetComLoans(isActive: 1);
                if (comloans.Count > 0)
                {
                    var comlon = comloans.Where(c => c.JobCode == jobCode).FirstOrDefault();
                    if (comlon != null)
                    {
                        loanData = await _loanContract.GetComLoanById(comlon.Id);

                        var installments = await _loanContract.GetComInstallmentsByDate((int)loanData.Id, fmonthid, tmonthid);
                        var previousInstallments = await _loanContract.GetPreviousComInstallmentsByDate((int)loanData.Id, fmonthid, offsetMonthId);


                        foreach (var item in installments)
                        {
                            individualComputerLoanReportViewModels.Add(new ComLoanInfoVM
                            {
                                MonthId = item.MonthId,
                                RecoveryAmounts = (decimal)item.InstallmentAmount,
                                RecoveryInterests = (decimal)item.InterestAmount,

                            });
                        }

                        employee = await _employeeContract.GetEmployeeViewByJobCode(jobCode);



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
                var closingmonthid = yearclosingyear * 100 + yearclosingmonth;
                var comYearEndingprincipal = await _loanContract.GetComYearEnding((int)loanData.Id, closingmonthid);
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
                        new ReportParameter("loanTakenAmount", ((decimal)loanData.TotalLoanAmount).ToString("F2")),
                        new ReportParameter("totalInstallmentNumber",loanData.InstallmentNo.ToString()),
                        new ReportParameter("runningInstallment",((decimal)loanData.InstallmentAmount).ToString("F2")),
                        //new ReportParameter("openingBalance",(loanData.TotalLoanAmount-prevRecoveryPrincipal).ToString()),
                        new ReportParameter("openingBalance",((decimal)comYearEndingprincipal.ClosingPrincipal).ToString("F2")),
                        new ReportParameter("recoveryPrincipal",((decimal)recoveryprincipal).ToString("F2")),
                        new ReportParameter("recoveryInterest",((decimal)recoveryInterest).ToString("F2")),
                        //new ReportParameter("closingBalance",((loanData.TotalLoanAmount-prevRecoveryPrincipal)-recoveryprincipal).ToString()),
                        new ReportParameter("closingBalance",((decimal)(comYearEndingprincipal.ClosingPrincipal-recoveryprincipal)).ToString("F2")),

                };
                string renderFormat = "PDF";
                string mimtype = "application/pdf";
                using var report = new Microsoft.Reporting.NETCore.LocalReport();
                report.EnableExternalImages = true;
                string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptIndividualComLoan.rdlc";
                report.DataSources.Add(new ReportDataSource("dsIndividualComLoan", individualComputerLoanReportViewModels));
                report.ReportPath = rptPath;
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                return File(pdf, mimtype);




            }
            else
            {
                //Summary Com Loan
                List<ComLoanInfoVM> summarycomLoans = new List<ComLoanInfoVM>();
                var fmonthid = fyear * 100 + fmonth;
                var tmonthid = tyear * 100 + tmonth;
                var type = "Summary";

                string fmonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(fmonth);
                string tmonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(fmonth);

                var comloans = await _loanContract.GetComLoans(isActive: 1);
                var comloan = comloans.Where(c => c.JobCode == jobCode).FirstOrDefault();
                if (comloan != null)
                {

                }
                if (employeeTypeId == 1)
                {
                    comloans = comloans.Where(m => m.JobCode.Contains("F")).ToList();
                }
                else
                {
                    comloans = comloans.Where(m => m.JobCode.Contains("J")).ToList();
                }

                foreach (var item in comloans)
                {
                    var installments = await _loanContract.GetComInstallmentsByDate((int)item.Id, fmonthid, tmonthid);
                    var previousInstallments = await _loanContract.GetPreviousComInstallmentsByDate((int)item.Id, fmonthid, offsetMonthId);
                    double? prevRecoveryPrincipal = 0;


                    var comYearEnding = await _loanContract.GetComYearEnding((int)item.Id, 202406);
                    double? recoveryprincipal = 0;
                    double? recoveryInterest = 0;
                    foreach (var installment in installments)
                    {
                        recoveryprincipal += installment.InstallmentAmount;
                        recoveryInterest += installment.InterestAmount;
                    }
                    // double? prevRecoveryPrincipal = 0;
                    //foreach (var previnstallment in previousInstallments)
                    //{
                    //    prevRecoveryPrincipal += previnstallment.InstallmentAmount;
                    //}

                    summarycomLoans.Add(new ComLoanInfoVM
                    {
                        EmployeeName = item.EmployeeName,
                        JobCode = item.JobCode,
                        LoanTakenDate = Convert.ToDateTime(item.LoanTakenDate).ToString("dd-MMM-yyyy"),
                        OpeningBalance = comYearEnding.ClosingPrincipal,
                        RecoveryAmounts = (decimal)recoveryprincipal,
                        RecoveryInterests = (decimal)recoveryInterest,
                        Total = (decimal)(recoveryprincipal + recoveryInterest),
                        ClosingBalance = (double)(comYearEnding.ClosingPrincipal - recoveryprincipal),

                    });
                }
                DateTime currentDate = DateTime.Now;
                string printDate = currentDate.ToString("MMMM dd, yyyy");
                var parameters = new[]
                {
                        new ReportParameter("printDate",printDate.ToString()),
                        new ReportParameter("fMonth",fmonthName.ToString()),
                        new ReportParameter("fYear",fyear.ToString()),
                        new ReportParameter("tMonth",tmonthName.ToString()),
                        new ReportParameter("tYear",tyear.ToString()),
                        new ReportParameter("type",type.ToString()),

                    };
                string renderFormat = "PDF";
                string mimtype = "application/pdf";
                using var report = new Microsoft.Reporting.NETCore.LocalReport();
                report.EnableExternalImages = true;
                string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptSummaryComLoan.rdlc";
                report.DataSources.Add(new ReportDataSource("dsSummaryComLoan", summarycomLoans));
                report.ReportPath = rptPath;
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                return File(pdf, mimtype);
            }
        }

        // Motorcycle Individual Loan Report
        [HttpGet]
        public async Task<IActionResult> GetMotorLoan()
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
                Text = $"{d.JobCode}"
            }).ToList();

            ViewBag.EmployeeCode = CodeList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetMotorLoan(string jobCode, int fmonth, int fyear, int tmonth, int tyear, int employeeTypeId, int reportType)
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
            if (employeeTypeId == 0 && reportType == 1)
            {
                employeeTypeIdError = "Select Employee Type";
            }
            if (String.IsNullOrEmpty(jobCode) && reportType == 1)
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


            var offsetMonthId = DateTime.Now.Year * 100 + DateTime.Now.Day;
            // indivisual mcl report.
            if (reportType == 1)
            {
                List<IndividualMotorCycleLoanReportViewModel> individualMotorCycleLoanReportViewModels = new List<IndividualMotorCycleLoanReportViewModel>();

                var jobCode1 = await _employeeContract.GetEmployeeCode();
                var CodeList = jobCode1.Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text = $"{d.JobCode}"
                }).ToList();

                ViewBag.EmployeeCode = CodeList;


                McLoan loanData = null;
                var fmonthid = fyear * 100 + fmonth;
                var tmonthid = tyear * 100 + tmonth;
                var mclLoans = await _loanContract.GetMcLoans(isActive: 1);
                if (mclLoans.Count > 0)
                {
                    var mclLoan = mclLoans.Where(ml => ml.JobCode == jobCode).SingleOrDefault();
                    if (mclLoan != null)
                    {
                        loanData = await _loanContract.GetMcLoanById(mclLoan.Id);
                    }
                    else
                    {
                        ViewBag.Message = "Data not found";
                        return View();
                    }
                }
                else
                {
                    //var jobCode1 = await _employeeContract.GetEmployeeCode();
                    //var CodeList = jobCode1.Select(d => new SelectListItem
                    //{
                    //    Value = d.JobCode.ToString(),
                    //    Text = $"{d.JobCode} - {d.EmployeeName}"
                    //}).ToList();
                    //ViewBag.EmployeeCode = CodeList;
                    ViewBag.Message = "Data not found";
                    return View();
                }

                var installments = await _loanContract.GetMcInstallmentsByDate((int)loanData.Id, fmonthid, tmonthid);
                var mclYearEnding = await _loanContract.GetMclYearEnding((int)loanData.Id, 202406);
                foreach (var item in installments)
                {
                    individualMotorCycleLoanReportViewModels.Add(
                        new IndividualMotorCycleLoanReportViewModel
                        {
                            MonthId = item.MonthId,
                            RecoveryAmount = item.InstallmentAmount
                        });
                }
                var employee = await _employeeContract.GetEmployeeViewByJobCode(jobCode);

                string fmonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(fmonth);
                string tmonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(fmonth);

                double? recovery = 0;
                foreach (var item in installments)
                {
                    recovery += item.InstallmentAmount;
                }

                //double? previousRecovery = 0;
                //foreach (var item in previousInstallments)
                //{
                //    previousRecovery += item.InstallmentAmount;
                //}
                DateTime currentDate = DateTime.Now;
                string printDate = currentDate.ToString("MMMM dd, yyyy");
                var parameters = new[]
                {
                    new ReportParameter("printDate",printDate.ToString()),
                    new ReportParameter("fMonth",fmonthName.ToString()),
                    new ReportParameter("fYear",fyear.ToString()),
                    new ReportParameter("tMonth",tmonthName.ToString()),
                    new ReportParameter("tYear",tyear.ToString()),
                    new ReportParameter("employeeName",employee.EmployeeName),
                    new ReportParameter("jobCode",employee.JobCode),
                    new ReportParameter("designation",employee.DesignationName),
                    new ReportParameter("department",employee.DepartmentName),
                    new ReportParameter("loanTakenAmount",((decimal)loanData.TotalLoanAmount).ToString("F2")),
                    new ReportParameter("totalInstallmentNumber",loanData.InstallmentNo.ToString()),
                    new ReportParameter("runningInstallment",((decimal)loanData.InstallmentAmount).ToString("F2")),
                    new ReportParameter("openingBalance",((decimal)mclYearEnding.ClosingPrincipal).ToString("F2")),
                    new ReportParameter("recovery",((decimal)recovery).ToString("F2")),
                    new ReportParameter("closingBalance",((decimal)(mclYearEnding.ClosingPrincipal-recovery)).ToString("F2")),

                };
                string renderFormat = "PDF";
                string mimtype = "application/pdf";
                using var report = new Microsoft.Reporting.NETCore.LocalReport();
                report.EnableExternalImages = true;
                string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptIndividualMotorLoan.rdlc";
                report.DataSources.Add(new ReportDataSource("dsIndividualMotorLoan", individualMotorCycleLoanReportViewModels));
                report.ReportPath = rptPath;
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                return File(pdf, mimtype);
            }
            else
            {
                //Summary Motor Loan
                List<SummaryMotorCycleLoanViewModel> summaryMotorCycleLoanViewModels = new List<SummaryMotorCycleLoanViewModel>();
                var fmonthid = fyear * 100 + fmonth;
                var tmonthid = tyear * 100 + tmonth;
                int extension = 1;
                string fmonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(fmonth);
                string tmonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(fmonth);

                var mclLoans = await _loanContract.GetMcLoans(isActive: 1);
                if (employeeTypeId == 1)
                {
                    mclLoans = mclLoans.Where(m => m.JobCode.Contains("F")).ToList();
                }
                else
                {
                    mclLoans = mclLoans.Where(m => m.JobCode.Contains("J")).ToList();
                }

                foreach (var item in mclLoans)
                {
                    var installments = await _loanContract.GetMcInstallmentsByDate((int)item.Id, fmonthid, tmonthid);
                    var mclYearEnding = await _loanContract.GetMclYearEnding((int)item.Id, 202406);

                    double? recovery = 0;
                    foreach (var installment in installments)
                    {
                        recovery += item.InstallmentAmount;
                    }

                    //double? previousRecovery = 0;
                    //foreach (var installment in previousInstallments)
                    //{
                    //    previousRecovery += item.InstallmentAmount;
                    //}

                    SummaryMotorCycleLoanViewModel summaryMotorCycleLoanViewModel = new SummaryMotorCycleLoanViewModel();
                    summaryMotorCycleLoanViewModel.EmployeeName = item.EmployeeName;
                    summaryMotorCycleLoanViewModel.JobCode = item.JobCode;
                    summaryMotorCycleLoanViewModel.LoanTakenDate = Convert.ToDateTime(item.LoanTakenDate).ToString("dd-MMM-yyyy");
                    summaryMotorCycleLoanViewModel.OpeningBalance = mclYearEnding.ClosingPrincipal;
                    summaryMotorCycleLoanViewModel.RecoveryAmount = recovery;
                    summaryMotorCycleLoanViewModel.ClosingBalance = mclYearEnding.ClosingPrincipal - recovery;
                    summaryMotorCycleLoanViewModels.Add(summaryMotorCycleLoanViewModel);
                }
                DateTime currentDate = DateTime.Now;
                string printDate = currentDate.ToString("MMMM dd, yyyy");
                var parameters = new[]
                {
                new ReportParameter("printDate",printDate.ToString()),
                new ReportParameter("fMonth",fmonthName.ToString()),
                new ReportParameter("fYear",fyear.ToString()),
                new ReportParameter("tMonth",tmonthName.ToString()),
                new ReportParameter("tYear",tyear.ToString()),

                };
                string renderFormat = "PDF";
                string mimtype = "application/pdf";
                using var report = new Microsoft.Reporting.NETCore.LocalReport();
                report.EnableExternalImages = true;
                string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptSummaryMotorLoan.rdlc";
                report.DataSources.Add(new ReportDataSource("dsSummaryMotorLoan", summaryMotorCycleLoanViewModels));
                report.ReportPath = rptPath;
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                return File(pdf, mimtype);
            }



        }

        [HttpGet]
        public async Task<IActionResult> OverTime()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            var jobCode = await _employeeContract.GetEmployeeCode();
            var CodeList = jobCode
                 .Where(d => d.JobCode.StartsWith("JS"))
                 .Select(d => new SelectListItem
                 {
                     Value = d.Id.ToString(),
                     Text = d.JobCode
                 })
                 .ToList();


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

        public async Task<IActionResult> OverTime(List<string>? jobcodes, int month, int year, string department, string isExcel)
        {
            var MonthId = year * 100 + month;
            var source = await _salarySettingContract.OvertimeData(jobcodes, MonthId, department);

            if (source != null)
            {
                foreach (var item in source)
                {
                    if (item.JournalNumber == "8152")
                    {
                        item.DepartmentName = "DEVELOPMENT";
                    }
                    else
                    {
                        Entities.Department existingDepartment = await _departmentContract.GetDepartmentByJournalCode(item.JournalNumber);

                        item.DepartmentName = existingDepartment == null ? item.JournalNumber : existingDepartment.DepartmentName;
                    }

                }
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
                    "Department Name", "Number of Emp.", "Current Month OT Hours", "Current Month Taka",
                    "Total OT Hours (From July)", "Total Taka (From July)", "Budget", "Exp. Limit", "Actual Limit"
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
                foreach (var item in source)
                {
                    worksheet.Cells[row, 1].Value = item.DepartmentName;
                    worksheet.Cells[row, 2].Value = item.NumberOfEmployees;
                    worksheet.Cells[row, 3].Value = item.CurrentMonthOtHours;
                    worksheet.Cells[row, 4].Value = item.CurrentMonthOtAllow;
                    worksheet.Cells[row, 5].Value = item.TotalOtHoursFromJuly;
                    worksheet.Cells[row, 6].Value = item.TotalOtAllowFromJuly;
                    worksheet.Cells[row, 7].Value = 0;  // Placeholder for Budget
                    worksheet.Cells[row, 8].Value = 0;  // Placeholder for Exp. Limit
                    worksheet.Cells[row, 9].Value = 0;  // Placeholder for Actual Limit
                    row++;
                }

                // Auto-fit columns for better visibility
                worksheet.Cells.AutoFitColumns();

                // Save the package to a MemoryStream
                using var stream = new MemoryStream();
                package.SaveAs(stream);
                var content = stream.ToArray();

                // Return the Excel file as a download
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "OvertimeReport.xlsx");
            }
            else
            {
                DateTime currentDate = DateTime.Now;
                string printDate = currentDate.ToString("MMMM dd, yyyy");
                string monthName = new DateTime(year, month, 1).ToString("MMMM");
                var fiscalyear = GetFiscalYear(month, year);

                var parameters = new[]
                {
                    new Microsoft.Reporting.NETCore.ReportParameter("printDate", printDate),
                    new Microsoft.Reporting.NETCore.ReportParameter("monthName", monthName),
                    new Microsoft.Reporting.NETCore.ReportParameter("year", year.ToString()),
                    new Microsoft.Reporting.NETCore.ReportParameter("fiscalyear", fiscalyear.ToString()),
                };

                string renderFormat = "PDF";
                string mimeType = "application/pdf";
                using var report = new Microsoft.Reporting.NETCore.LocalReport();
                report.EnableExternalImages = true;
                string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptOverTime.rdlc";
                report.DataSources.Add(new ReportDataSource("dsOverTime", source));
                report.ReportPath = rptPath;
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                return File(pdf, mimeType);
            }
        }
        public string GetFiscalYear(int month, int year)
        {
            // Assuming fiscal year starts in July
            int fiscalStartMonth = 7;

            if (month >= fiscalStartMonth)
            {
                // If the month is July or later, fiscal year spans current year to next year
                return $"{year}-{year + 1}";
            }
            else
            {
                // If the month is before July, fiscal year spans previous year to current year
                return $"{year - 1}-{year}";
            }
        }

        public async Task<IActionResult> CarInstallments(int loanId)
        {
            if (loanId > 0)
            {


                CarLoan carLoan = await _loanContract.GetCarLoanById(loanId);
                List<CarLoanInstallment> carLoanInstallments = await _loanContract.GetCarLoanInstallments(loanId);
                carLoanInstallments = carLoanInstallments.OrderByDescending(ci => ci.RemainingBalance).ToList();

                EmployeeViewModel employee = await _employeeContract.GetEmployeeViewByJobCode(carLoan.JobCode);


                double paidAmount = 0;
                double unPaidAmount = 0;

                foreach (var loan in carLoanInstallments)
                {
                    if (loan.IsPaid)
                    {
                        paidAmount += loan.PrincipalAmount;
                    }
                    else
                    {
                        unPaidAmount += loan.PrincipalAmount;
                    }
                }


                var parameters = new[]
                {
                        new ReportParameter("jobcode",employee.JobCode),
                        new ReportParameter("employeeName",employee.EmployeeName),
                        new ReportParameter("actualAmount",carLoanInstallments.Sum(cl=>cl.PrincipalAmount).ToString("N2")),
                        new ReportParameter("paidAmount", paidAmount.ToString("N2")),
                        new ReportParameter("unpaidAmount", unPaidAmount.ToString("N2"))

                };
                string renderFormat = "PDF";
                string mimtype = "application/pdf";
                using var report = new Microsoft.Reporting.NETCore.LocalReport();
                report.EnableExternalImages = true;
                string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptCarInstallment.rdlc";
                report.DataSources.Add(new ReportDataSource("dsCarInstallment", carLoanInstallments));
                report.ReportPath = rptPath;
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                return File(pdf, mimtype);
            }
            return BadRequest("Invalid loan ID.");
        }

        public async Task<IActionResult> CarDepreciationInstallments(int loanId)
        {
            if (loanId > 0)
            {

                CarLoan carLoan = await _loanContract.GetCarLoanById(loanId);
                List<DepreciationInstallment> depreciationInstallments = await _loanContract.GetCarLoanDepreciationInstallments(loanId);
                depreciationInstallments = depreciationInstallments.OrderBy(d => d.MonthId).ToList();

                EmployeeViewModel employee = await _employeeContract.GetEmployeeViewByJobCode(carLoan.JobCode);


                double paidAmount = 0;
                double unPaidAmount = 0;

                foreach (var loan in depreciationInstallments)
                {
                    if (loan.IsPaid)
                    {
                        paidAmount += loan.DepreciationAmount;
                    }
                    else
                    {
                        unPaidAmount += loan.DepreciationAmount;
                    }
                }


                var parameters = new[]
                {
                        new ReportParameter("jobcode",employee.JobCode),
                        new ReportParameter("employeeName",employee.EmployeeName),
                        new ReportParameter("depreciationAmount",depreciationInstallments.Sum(di=>di.DepreciationAmount).ToString("N2")),
                        new ReportParameter("paidAmount", paidAmount.ToString("N2")),
                        new ReportParameter("unpaidAmount", unPaidAmount.ToString("N2")),

                    };
                string renderFormat = "PDF";
                string mimtype = "application/pdf";
                using var report = new Microsoft.Reporting.NETCore.LocalReport();
                report.EnableExternalImages = true;
                string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptCarDepriciate.rdlc";
                report.DataSources.Add(new ReportDataSource("dsCarDepriciate", depreciationInstallments));
                report.ReportPath = rptPath;
                report.SetParameters(parameters);
                var pdf = report.Render(renderFormat);
                return File(pdf, mimtype);
            }
            return BadRequest("Invalid loan ID.");
        }

        private List<AmortizationSchedule> CalculateLoanReport(decimal loanAmount, decimal interestRate, int installmentNumber, int installNoOfInterest, DateTime startDate)
        {
            List<AmortizationSchedule> reportEntries = new List<AmortizationSchedule>();

            decimal totalInterest = 0;
            decimal monthlyInterestRate = interestRate / 12 / 100;
            decimal monthlyInstallment = loanAmount / installmentNumber;
            decimal remainingPrincipal = loanAmount;

            for (int i = 0; i < installmentNumber; i++)
            {
                decimal interest = remainingPrincipal * monthlyInterestRate;
                decimal principal = (i == 0) ? loanAmount : remainingPrincipal;
                remainingPrincipal -= monthlyInstallment;
                totalInterest += interest;
                DateTime installmentDate = startDate.AddMonths(i);
                string monthYear = installmentDate.ToString("MMMM-yyyy");

                AmortizationSchedule entry = new AmortizationSchedule
                {
                    NoOfInterest = i + 1,
                    MonthYear = monthYear,
                    Principal = Math.Round(principal, 2),
                    Installment = Math.Round(monthlyInstallment, 2),
                    Interest = Math.Round(interest, 2),
                    NewPrincipal = Math.Round(remainingPrincipal, 2)
                };

                reportEntries.Add(entry);
            }

            return reportEntries;
        }

        private List<AmortizationSchedule> CalculateInterestInstalment(decimal loanAmount, decimal interestRate, int installmentNumber, int installNoOfInterest, DateTime startDate)
        {
            List<AmortizationSchedule> reportEntries = new List<AmortizationSchedule>();

            decimal totalInterest = 0;
            decimal monthlyInterestRate = interestRate / 12 / 100;
            decimal remainingPrincipal = loanAmount;
            DateTime lastCapitalInstallmentDate = startDate;

            // Calculate total interest over the loan period
            for (int i = 0; i < installmentNumber; i++)
            {
                decimal interest = remainingPrincipal * monthlyInterestRate;
                totalInterest += interest;
                remainingPrincipal -= loanAmount / installmentNumber;
                lastCapitalInstallmentDate = startDate.AddMonths(i);
            }

            // Calculate interest installment after capital installments are complete
            decimal interestInstallment = totalInterest / installNoOfInterest;
            DateTime firstInterestInstallmentDate = lastCapitalInstallmentDate.AddMonths(1);

            for (int i = 0; i < installNoOfInterest; i++)
            {
                DateTime installmentDate = firstInterestInstallmentDate.AddMonths(i);
                string monthYear = installmentDate.ToString("MMMM-yyyy");

                AmortizationSchedule entry = new AmortizationSchedule
                {
                    NoOfInterest = i + 1,
                    MonthYear = monthYear,
                    InterestInstallment = Math.Round(interestInstallment, 2),
                };

                reportEntries.Add(entry);
            }

            return reportEntries;
        }

        public async Task<IActionResult> GenereateScheduleProvidentFound()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GenereateScheduleProvidentFound(int month, int year, string isExcel)
        {


            List<ProvidentFundData> pfData = new List<ProvidentFundData>();
            int monthid = year * 100 + 12;
            string netPayInWords = "";
            decimal grandNetPay = 0;

            var data = await _salaryReportOfficerContract.GetProvidentFund(monthid);
            if (!string.IsNullOrEmpty(isExcel))
            {
                // Set EPPlus license context
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                // Create a new Excel package
                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Overtime Report");
                // Define headers
                string[] headers = {
                            "EMPLOYEE CODE", "EMP. NAME", "DESIGNATION", "BANK ACCOUNT NO.","NET PAY"
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
                foreach (var item in pfData)
                {
                    worksheet.Cells[row, 1].Value = item.JobCode;
                    worksheet.Cells[row, 2].Value = item.EmployeeName;
                    worksheet.Cells[row, 3].Value = item.DesignationName;
                    row++;
                }

                // Auto-fit columns for better visibility
                worksheet.Cells.AutoFitColumns();

                // Save the package to a MemoryStream
                using var stream = new MemoryStream();
                package.SaveAs(stream);
                var content = stream.ToArray();

                // Return the Excel file as a download
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BankSheet.xlsx");
            }
            netPayInWords = NumberToWordsConverter.Convert(grandNetPay) + " Only";
            DateTime currentDate = DateTime.Now;
            string printDate = currentDate.ToString("M/d/yy, h:mm tt");
            var parameters = new[]
            {
                    new Microsoft.Reporting.NETCore.ReportParameter("printDate",printDate.ToString()),
                    new ReportParameter("printDate",printDate.ToString()),
                    new ReportParameter("year",year.ToString()),

                };
            string renderFormat = "PDF";
            string mimtype = "application/pdf";
            using var report = new Microsoft.Reporting.NETCore.LocalReport();
            report.EnableExternalImages = true;
            string rptPath = $"{_webHostEnvironment.WebRootPath}\\Reports\\RptSchedulePF.rdlc";
            report.DataSources.Add(new ReportDataSource("dsProvidentFund", data));
            report.ReportPath = rptPath;
            report.SetParameters(parameters);
            var pdf = report.Render(renderFormat);
            return File(pdf, mimtype);

        }

    }
}
