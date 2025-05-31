using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.OleDb;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using BgfclApp.ViewModels;
using Entities;
using Microsoft.IdentityModel.Tokens;
using Contracts;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Globalization;
using System.IO;
using ExcelDataReader;
using System.Text;

namespace BgfclApp.Controllers
{
    public class SalarySettingsController : Controller
    {
        private Microsoft.AspNetCore.Hosting.IWebHostEnvironment _environment;
        private IConfiguration _configuration;
        ISalarySettingContract _salarySettingContract;

        public SalarySettingsController(Microsoft.AspNetCore.Hosting.IWebHostEnvironment environment, IConfiguration configuration, ISalarySettingContract salarySettingContract)
        {
            _environment = environment;
            _configuration = configuration;
            _salarySettingContract = salarySettingContract;
        }
        public IActionResult SalaryPolicySettings()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            return View();
        }

        public IActionResult BasicSalarySettingsOfficer()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            return View();
        }
        public IActionResult BasicSalarySettingsJuniorStaff()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            return View();
        }
        public IActionResult BasicSalarySettingsDailyWorker()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            return View();
        }
        public IActionResult UpdateBasicSalary()
        {
            List<ExcelUploadBasicSalaryViewModel> excelUploads = new List<ExcelUploadBasicSalaryViewModel>();
            return View(excelUploads);
        }

        // need to write the update code
        [HttpPost]
        public async Task<IActionResult> UpdateBasicSalary(IFormFile postedFile,int employeeTypeId)
        {
            List<ExcelUploadBasicSalaryViewModel> excelUploads = new List<ExcelUploadBasicSalaryViewModel>();

            if (postedFile != null)
            {
                //Create a Folder.
                string path = Path.Combine(this._environment.WebRootPath, "Uploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                //Save the uploaded Excel file.
                string fileName = Path.GetFileName(postedFile.FileName);
                string filePath = Path.Combine(path, fileName);
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                //Read the connection string for the Excel file.
                string conString = this._configuration.GetConnectionString("ExcelConString");
                DataTable dt = new DataTable();
                conString = string.Format(conString, filePath);

                using (OleDbConnection connExcel = new OleDbConnection(conString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;

                            //Get the name of First Sheet.
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            connExcel.Close();

                            //Read Data from First Sheet.
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            connExcel.Close();
                        }
                    }
                }
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                if (dt.Rows.Count > 0)
                {
                    int resultCount = 0;
                    List<SalarySettingsOfficer> salarySettings = new List<SalarySettingsOfficer>();

                    for (int i=0;i<dt.Rows.Count;i++)
                    {
                        ExcelUploadBasicSalaryViewModel excelUploadBasic = new ExcelUploadBasicSalaryViewModel();
                        excelUploadBasic.Sl = i+1;
                        excelUploadBasic.JobCode = dt.Rows[i][0].ToString();
                        excelUploadBasic.EmployeeName = dt.Rows[i][1].ToString();
                        excelUploadBasic.NewBasic = Convert.ToDouble(dt.Rows[i][2]);
                        excelUploads.Add(excelUploadBasic);

                        SalarySettingsOfficer salarySettingsOfficer = new SalarySettingsOfficer();
                        salarySettingsOfficer.JobCode = dt.Rows[i][0].ToString();
                        salarySettingsOfficer.BasicSalary = Convert.ToDouble(dt.Rows[i][2]);
                        salarySettings.Add(salarySettingsOfficer);

                    }

                    foreach (var item in salarySettings)
                    {
                        resultCount += await _salarySettingContract.UpdateBasicSalary(item,employeeTypeId);
                    }
                    if (resultCount == salarySettings.Count)
                    {
                        ViewBag.BasicSalaryUpdateResult = "Data uploaded successfully";
                    }
                    else
                    {
                        ViewBag.BasicSalaryUpdateResult = "Something went wrong";
                    }
                }
            }

            return View(excelUploads);


        }

        public IActionResult UpdateTax()
        {
            List<ExcelUploadTaxViewModel> excelUploads = new List<ExcelUploadTaxViewModel>();
            return View(excelUploads);
        }

        // need to write the update code
        [HttpPost]
        public async Task<IActionResult> UpdateTax(IFormFile postedFile)
        {
            List<ExcelUploadTaxViewModel> excelUploads = new List<ExcelUploadTaxViewModel>();

            if (postedFile != null)
            {
                //Create a Folder.
                string path = Path.Combine(this._environment.WebRootPath, "Uploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                //Save the uploaded Excel file.
                string fileName = Path.GetFileName(postedFile.FileName);
                string filePath = Path.Combine(path, fileName);
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                //Read the connection string for the Excel file.
                string conString = this._configuration.GetConnectionString("ExcelConString");
                DataTable dt = new DataTable();
                conString = string.Format(conString, filePath);

                using (OleDbConnection connExcel = new OleDbConnection(conString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;

                            //Get the name of First Sheet.
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            connExcel.Close();

                            //Read Data from First Sheet.
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            connExcel.Close();
                        }
                    }
                }
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                if (dt.Rows.Count > 0)
                {
                    List<SalarySettingsOfficer> salarySettings = new List<SalarySettingsOfficer>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        ExcelUploadTaxViewModel excelUploadTax = new ExcelUploadTaxViewModel();
                        excelUploadTax.Sl = i + 1;
                        excelUploadTax.JobCode = dt.Rows[i][0].ToString();
                        excelUploadTax.EmployeeName = dt.Rows[i][1].ToString();
                        excelUploadTax.TIN = dt.Rows[i][2].ToString();
                        excelUploadTax.TAX = Convert.ToDouble(dt.Rows[i][3]);
                        excelUploads.Add(excelUploadTax);

                        SalarySettingsOfficer salarySettingsOfficer = new SalarySettingsOfficer();
                        salarySettingsOfficer.JobCode = dt.Rows[i][0].ToString();
                        salarySettingsOfficer.MonthlyTaxDeduction = Convert.ToDouble(dt.Rows[i][3]);
                        salarySettings.Add(salarySettingsOfficer);
                    }

                    var taxUpdateResult = await _salarySettingContract.UpdateTaxInfo(salarySettings);
                    if (taxUpdateResult>0)
                    {
                        ViewBag.TaxUpdateResult = "Data uploaded successfully";
                    }
                }
            }

            return View(excelUploads);

        }

        public IActionResult AdvanceTax()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            return View();
        }
        public IActionResult MonthlySalarySettingOfficer()
        {
            List<ExelUploadMonthlySalarySettingOfficerViewModel> monthlySalarySettingOfficers = new List<ExelUploadMonthlySalarySettingOfficerViewModel>();
            return View(monthlySalarySettingOfficers);
        }

        [HttpPost]
        public IActionResult MonthlySalarySettingOfficer(IFormFile postedFile)
        {
            List<ExelUploadMonthlySalarySettingOfficerViewModel> excelUploads = new List<ExelUploadMonthlySalarySettingOfficerViewModel>();

            if (postedFile != null)
            {
                //Create a Folder.
                string path = Path.Combine(this._environment.WebRootPath, "Uploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                //Save the uploaded Excel file.
                string fileName = Path.GetFileName(postedFile.FileName);
                string filePath = Path.Combine(path, fileName);


                using (var stream = System.IO.File.Create(filePath))
                {
                    postedFile.CopyTo(stream);
                }

                DataTableCollection dataTables = SalarySettingsController.ReadExcelFiles(filePath);

                foreach (DataTable dataTable in dataTables)
                {
                    if (dataTable.TableName.ToLower() == "payrol")
                    {
                        if (dataTable.Rows.Count > 0)
                        {
                            for (int i = 0; i < dataTable.Rows.Count; i++)
                            {
                                ExelUploadMonthlySalarySettingOfficerViewModel excelUpload = new ExelUploadMonthlySalarySettingOfficerViewModel();
                                excelUpload.Sl = i + 1;
                                excelUpload.JobCode = dataTable.Rows[i][0].ToString();
                                excelUpload.EmployeeName = dataTable.Rows[i][1].ToString();
                                excelUpload.ArrearSalary = dataTable.Rows[i][2] is DBNull ? 0 : Convert.ToDouble(dataTable.Rows[i][2]);
                                excelUpload.WorkDays = dataTable.Rows[i][3] is DBNull ? 0 : Convert.ToInt32(dataTable.Rows[i][3]);
                                excelUpload.AdvanceSalary = dataTable.Rows[i][4] is DBNull ? 0 : Convert.ToDouble(dataTable.Rows[i][4]);
                                excelUpload.OtherDeduction = dataTable.Rows[i][5] is DBNull ? 0 : Convert.ToDouble(dataTable.Rows[i][5]);
                                excelUpload.SpecialDeduction = dataTable.Rows[i][6] is DBNull ? 0 : Convert.ToDouble(dataTable.Rows[i][6]);
                                excelUpload.AdvanceDeduction = dataTable.Rows[i][7] is DBNull ? 0 : Convert.ToDouble(dataTable.Rows[i][7]);
                                excelUpload.HospitalBill = dataTable.Rows[i][8] is DBNull ? 0 : Convert.ToDouble(dataTable.Rows[i][8]);
                                excelUpload.TMBill = dataTable.Rows[i][9] is DBNull ? 0 : Convert.ToDouble(dataTable.Rows[i][9]);
                                excelUpload.OtherAllow = dataTable.Rows[i][10] is DBNull ? 0 : Convert.ToDouble(dataTable.Rows[i][10]);

                                excelUploads.Add(excelUpload);
                            }
                        }
                    }
                }

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            return View(excelUploads);
        }

        public IActionResult MonthlySalarySettingStaff()
        {
            List<ExelUploadMonthlySalarySettingStaffViewModel> monthlySalarySettingStaff = new List<ExelUploadMonthlySalarySettingStaffViewModel>();
            return View(monthlySalarySettingStaff);
        }
        [HttpPost]
        public IActionResult MonthlySalarySettingStaff(IFormFile postedFile)
        {
            List<ExelUploadMonthlySalarySettingStaffViewModel> monthlySalarySettingStaff = new List<ExelUploadMonthlySalarySettingStaffViewModel>();

            if (postedFile != null)
            {
                //Create a Folder.
                string path = Path.Combine(this._environment.WebRootPath, "Uploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                //Save the uploaded Excel file.
                string fileName = Path.GetFileName(postedFile.FileName);
                string filePath = Path.Combine(path, fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    postedFile.CopyTo(stream);
                }

                DataTableCollection dataTables = SalarySettingsController.ReadExcelFiles(filePath);

                foreach (DataTable dataTable in dataTables)
                {
                    if (dataTable.TableName.ToLower() == "payrol")
                    {
                        if (dataTable.Rows.Count > 0)
                        {
                            for (int i = 0; i < dataTable.Rows.Count; i++)
                            {
                                ExelUploadMonthlySalarySettingStaffViewModel excelUpload = new ExelUploadMonthlySalarySettingStaffViewModel();
                                excelUpload.Sl = i + 1;
                                excelUpload.JobCode = dataTable.Rows[i][0].ToString();
                                excelUpload.EmployeeName = dataTable.Rows[i][1].ToString();
                                excelUpload.WorkDays = dataTable.Rows[i][2] is DBNull ? 0 : Convert.ToInt32(dataTable.Rows[i][2]);
                                excelUpload.OtSingle = dataTable.Rows[i][3] is DBNull ? 0 : Convert.ToDouble(dataTable.Rows[i][3]);
                                excelUpload.OtDouble = dataTable.Rows[i][4] is DBNull ? 0 : Convert.ToDouble(dataTable.Rows[i][4]);
                                excelUpload.NumberOfShift = dataTable.Rows[i][5] is DBNull ? 0 : Convert.ToInt32(dataTable.Rows[i][5]);
                                excelUpload.OtherAllow = dataTable.Rows[i][6] is DBNull ? 0 : Convert.ToDouble(dataTable.Rows[i][6]);
                                excelUpload.ArrearSalary = dataTable.Rows[i][7] is DBNull ? 0 : Convert.ToDouble(dataTable.Rows[i][7]);
                                excelUpload.AdvanceDeduction = dataTable.Rows[i][8] is DBNull ? 0 : Convert.ToDouble(dataTable.Rows[i][8]);
                                excelUpload.OtherDeduction = dataTable.Rows[i][9] is DBNull ? 0 : Convert.ToDouble(dataTable.Rows[i][9]);
                                excelUpload.SpecialDeduction = dataTable.Rows[i][10] is DBNull ? 0 : Convert.ToDouble(dataTable.Rows[i][10]);
                                excelUpload.HospitalDeduction = dataTable.Rows[i][11] is DBNull ? 0 : Convert.ToDouble(dataTable.Rows[i][11]);

                                monthlySalarySettingStaff.Add(excelUpload);
                            }
                        }
                    }
                }

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }




            return View(monthlySalarySettingStaff);
        }

        public IActionResult MonthlyLoan()
        {
            List<ExcelUploadMonthlyLoanViewModel> monthlyLoanViewModels = new List<ExcelUploadMonthlyLoanViewModel>();
            return View(monthlyLoanViewModels);
        }

        [HttpPost]
        public IActionResult MonthlyLoan(IFormFile postedFile)
        {
            List<ExcelUploadMonthlyLoanViewModel> monthlyLoanViewModels = new List<ExcelUploadMonthlyLoanViewModel>();
            List<string> loanNames = new List<string>()
            {
                "pfloan",
                "cosloan",
                "wpfloan"
            };

            if (postedFile != null)
            {
                //Create a Folder.
                string path = Path.Combine(this._environment.WebRootPath, "Uploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                //Save the uploaded Excel file.
                string fileName = Path.GetFileName(postedFile.FileName);
                string filePath = Path.Combine(path, fileName);


                using (var stream = System.IO.File.Create(filePath))
                {
                    postedFile.CopyTo(stream);
                }

                DataTableCollection dataTables =  SalarySettingsController.ReadExcelFiles(filePath);

                foreach (DataTable dataTable in dataTables)
                {
                    
                    if (loanNames.Contains(dataTable.TableName.ToLower()))
                    {
                        if (dataTable.Rows.Count > 0)
                        {
                            for (int j = 0; j < dataTable.Rows.Count; j++)
                            {
                                if (!String.IsNullOrEmpty(dataTable.Rows[j][0].ToString()))
                                {
                                    ExcelUploadMonthlyLoanViewModel monthlyLoanViewModel = new ExcelUploadMonthlyLoanViewModel();
                                    monthlyLoanViewModel.Sl = j + 1;
                                    monthlyLoanViewModel.JobCode = dataTable.Rows[j][0].ToString();
                                    monthlyLoanViewModel.EmployeeName = dataTable.Rows[j][1].ToString();
                                    monthlyLoanViewModel.Amount = dataTable.Rows[j][2] is DBNull ? 0 : Convert.ToDouble(dataTable.Rows[j][2]);
                                    monthlyLoanViewModel.LoanType = dataTable.TableName;
                                    monthlyLoanViewModels.Add(monthlyLoanViewModel);
                                }

                            }
                        }
                    }

                }

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

            }




            return View(monthlyLoanViewModels);
        }


        public IActionResult SalaryProcess()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            return View();
        }
        public IActionResult FinalAdjustmentOfficer()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            return View();
        }
        public IActionResult FinalAdjustmentJuniorStaff()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            return View();
        }

        public IActionResult JournalPosting()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            return View();
        }

        public IActionResult JournalAdjustment()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            return View();
        }

        public async Task<IActionResult> FinalAdjustmentOfficerExcelExport(int monthId,int month, int year)
        {
            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
            string excelName = "final_adjustment_officer_";
            excelName += monthName + "_" + year.ToString() + ".xlsx";

            try
            {
                var finalAdjustments = await _salarySettingContract.FinalAdjustmentOfficer(monthId);

                

                var workBook = new XLWorkbook();
                var workSheet = workBook.Worksheets.Add("Sheet1");

                workSheet.Cell(1, 1).Value = "JobCode";
                workSheet.Cell(1, 2).Value = "EmployeeName";
                workSheet.Cell(1, 3).Value = "DesignationName";
                workSheet.Cell(1, 4).Value = "DepartmentName";
                workSheet.Cell(1, 5).Value = "AccountNumber";
                workSheet.Cell(1, 6).Value = "BankName";
                workSheet.Cell(1, 7).Value = "BankBranchName";
                workSheet.Cell(1, 8).Value = "BasicSalary";
                workSheet.Cell(1, 9).Value = "PersonalSalary";
                workSheet.Cell(1, 10).Value = "ArrearSalary";
                workSheet.Cell(1, 11).Value = "WorkDays";
                workSheet.Cell(1, 12).Value = "LikeBasic";
                workSheet.Cell(1, 13).Value = "OtherSalary";
                workSheet.Cell(1, 14).Value = "specialBenefit";
                workSheet.Cell(1, 15).Value = "LunchAllow";
                workSheet.Cell(1, 16).Value = "TiffinAllow";
                workSheet.Cell(1, 17).Value = "WashAllow";
                workSheet.Cell(1, 18).Value = "HouseRentAllow";
                workSheet.Cell(1, 19).Value = "Conveyance";
                workSheet.Cell(1, 20).Value = "FamilyMedicalAllow";
                workSheet.Cell(1, 21).Value = "EducationalAllow";
                workSheet.Cell(1, 22).Value = "FieldRiskAllow";
                workSheet.Cell(1, 23).Value = "ChargeAllow";
                workSheet.Cell(1, 24).Value = "DAidAllow";
                workSheet.Cell(1, 25).Value = "DeputationAllow";
                workSheet.Cell(1, 26).Value = "OtherAllow";
                workSheet.Cell(1, 27).Value = "CME";
                workSheet.Cell(1, 28).Value = "RevenueStamp";
                workSheet.Cell(1, 29).Value = "ProvidentFund";
                workSheet.Cell(1, 30).Value = "PensionOfficer";
                workSheet.Cell(1, 31).Value = "WelfareFund";
                workSheet.Cell(1, 32).Value = "OfficerClub";
                workSheet.Cell(1, 33).Value = "OfficerAssociation";
                workSheet.Cell(1, 34).Value = "MedicalFund";
                workSheet.Cell(1, 35).Value = "TMBill";
                workSheet.Cell(1, 36).Value = "Dormitory";
                workSheet.Cell(1, 37).Value = "Hospitalisation";
                workSheet.Cell(1, 38).Value = "HouseRentReturn";
                workSheet.Cell(1, 39).Value = "SpecialDeduction";
                workSheet.Cell(1, 40).Value = "FuelReturn";
                workSheet.Cell(1, 41).Value = "HBLoan";
                workSheet.Cell(1, 42).Value = "MCLoan";
                workSheet.Cell(1, 43).Value = "BCylLoan";
                workSheet.Cell(1, 44).Value = "ComLoan";
                workSheet.Cell(1, 45).Value = "CarLoan";
                workSheet.Cell(1, 46).Value = "PFLoan";
                workSheet.Cell(1, 47).Value = "WPFLoan";
                workSheet.Cell(1, 48).Value = "COSLoan";
                workSheet.Cell(1, 49).Value = "OtherLoan";
                workSheet.Cell(1, 50).Value = "Advance";
                workSheet.Cell(1, 51).Value = "Other";
                workSheet.Cell(1, 52).Value = "IncomeTax";

                int row = 2;

                foreach (var item in finalAdjustments)
                {
                    workSheet.Cell(row, 1).Value = item.JobCode;
                    workSheet.Cell(row, 2).Value = item.EmployeeName;
                    workSheet.Cell(row, 3).Value = item.DesignationName;
                    workSheet.Cell(row, 4).Value = item.DepartmentName;
                    workSheet.Cell(row, 5).Value = item.AccountNumber;
                    workSheet.Cell(row, 6).Value = item.BankName;
                    workSheet.Cell(row, 7).Value = item.BankBranchName;
                    workSheet.Cell(row, 8).Value = item.BasicSalary;
                    workSheet.Cell(row, 9).Value = item.PersonalSalary;
                    workSheet.Cell(row, 10).Value = item.ArrearSalary;
                    workSheet.Cell(row, 11).Value = item.WorkDays;
                    workSheet.Cell(row, 12).Value = item.LikeBasic;
                    workSheet.Cell(row, 13).Value = item.OtherSalary;
                    workSheet.Cell(row, 14).Value = item.SpecialBenefit;
                    workSheet.Cell(row, 15).Value = item.LunchAllow;
                    workSheet.Cell(row, 16).Value = item.TiffinAllow;
                    workSheet.Cell(row, 17).Value = item.WashAllow;
                    workSheet.Cell(row, 18).Value = item.HouseRentAllow;
                    workSheet.Cell(row, 19).Value = item.Conveyance;
                    workSheet.Cell(row, 20).Value = item.FMAllow;
                    workSheet.Cell(row, 21).Value = item.EducationalAllow;
                    workSheet.Cell(row, 22).Value = item.FieldRiskAllow;
                    workSheet.Cell(row, 23).Value = item.ChargeAllow;
                    workSheet.Cell(row, 24).Value = item.DAidAllow;
                    workSheet.Cell(row, 25).Value = item.DeputationAllow;
                    workSheet.Cell(row, 26).Value = item.OtherAllow;
                    workSheet.Cell(row, 27).Value = item.CME;
                    workSheet.Cell(row, 28).Value = item.RevenueStamp;
                    workSheet.Cell(row, 29).Value = item.ProvidentFund;
                    workSheet.Cell(row, 30).Value = item.PensionOfficer;
                    workSheet.Cell(row, 31).Value = item.WelfareFund;
                    workSheet.Cell(row, 32).Value = item.OfficerClub;
                    workSheet.Cell(row, 33).Value = item.OfficerAssociation;
                    workSheet.Cell(row, 34).Value = item.MedicalFund;
                    workSheet.Cell(row, 35).Value = item.TMBill;
                    workSheet.Cell(row, 36).Value = item.Dormitory;
                    workSheet.Cell(row, 37).Value = item.Hospitalisation;
                    workSheet.Cell(row, 38).Value = item.HouseRentReturn;
                    workSheet.Cell(row, 39).Value = item.SpecialDeduction;
                    workSheet.Cell(row, 40).Value = item.FuelReturn;
                    workSheet.Cell(row, 41).Value = item.HBLoan;
                    workSheet.Cell(row, 42).Value = item.MCylLoan;
                    workSheet.Cell(row, 43).Value = item.BCylLoan;
                    workSheet.Cell(row, 44).Value = item.ComLoan;
                    workSheet.Cell(row, 45).Value = item.CarLoan;
                    workSheet.Cell(row, 46).Value = item.PFLoan;
                    workSheet.Cell(row, 47).Value = item.WPFLoan;
                    workSheet.Cell(row, 48).Value = item.CosLoan;
                    workSheet.Cell(row, 49).Value = item.OtherLoan;
                    workSheet.Cell(row, 50).Value = item.Advance;
                    workSheet.Cell(row, 51).Value = item.Other;
                    workSheet.Cell(row, 52).Value = item.IncomeTax;
                    row++;
                }

                var stream = new MemoryStream();
                workBook.SaveAs(stream);
                stream.Position = 0;
                //return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName, true);
            }
            catch (Exception ex)
            {
                var workBook = new XLWorkbook();
                var workSheet = workBook.Worksheets.Add("Sheet1");

                var stream = new MemoryStream();
                workBook.SaveAs(stream);
                stream.Position = 0;
                //return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName, true);
            }
        }


        public async Task<IActionResult> FinalAdjustmentJuniorStaffExcelExport(int monthId, int month, int year)
        {
            string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
            string excelName = "final_adjustment_junior_staff_";
            excelName += monthName + "_" + year.ToString() + ".xlsx";

            try
            {
                var finalAdjustments = await _salarySettingContract.FinalAdjustmentJuniorStaff(monthId);



                var workBook = new XLWorkbook();
                var workSheet = workBook.Worksheets.Add("Sheet1");

                workSheet.Cell(1, 1).Value = "JobCode";
                workSheet.Cell(1, 2).Value = "EmployeeName";
                workSheet.Cell(1, 3).Value = "DesignationName";
                workSheet.Cell(1, 4).Value = "DepartmentName";
                workSheet.Cell(1, 5).Value = "AccountNumber";
                workSheet.Cell(1, 6).Value = "BankName";
                workSheet.Cell(1, 7).Value = "BankBranchName";
                workSheet.Cell(1, 8).Value = "BasicSalary";
                workSheet.Cell(1, 9).Value = "PersonalSalary";
                workSheet.Cell(1, 10).Value = "ConvenienceAllow";
                workSheet.Cell(1, 11).Value = "ArrearSalary";
                workSheet.Cell(1, 12).Value = "WorkDays";
                workSheet.Cell(1, 13).Value = "NumberOfShift";
                workSheet.Cell(1, 14).Value = "OtherSalary";
                workSheet.Cell(1, 15).Value = "SpecialBenefit";
                workSheet.Cell(1, 16).Value = "LunchAllow";
                workSheet.Cell(1, 17).Value = "TiffinAllow";
                workSheet.Cell(1, 18).Value = "ShiftAllow";
                workSheet.Cell(1, 19).Value = "HouseRentAllow";
                workSheet.Cell(1, 20).Value = "FamilyMedicalAllow";
                workSheet.Cell(1, 21).Value = "EducationAllowance";
                workSheet.Cell(1, 22).Value = "FieldAllow";
                workSheet.Cell(1, 23).Value = "OtSingle";
                workSheet.Cell(1, 24).Value = "OtDouble";
                workSheet.Cell(1, 25).Value = "OtAllow";
                workSheet.Cell(1, 26).Value = "FuelAllow";
                workSheet.Cell(1, 27).Value = "UtilityAllow";
                workSheet.Cell(1, 28).Value = "OtherAllow";
                workSheet.Cell(1, 29).Value = "RevenueStamp";
                workSheet.Cell(1, 30).Value = "ProvidentFund";
                workSheet.Cell(1, 31).Value = "WelfareFund";
                workSheet.Cell(1, 32).Value = "EmployeeClub";
                workSheet.Cell(1, 33).Value = "EmployeeUnion";
                workSheet.Cell(1, 34).Value = "Dormitory";
                workSheet.Cell(1, 35).Value = "HospitalDeduction";
                workSheet.Cell(1, 36).Value = "SpecialDeduction";
                workSheet.Cell(1, 37).Value = "FuelReturn";
                workSheet.Cell(1, 38).Value = "HbLoan";
                workSheet.Cell(1, 39).Value = "MCLoan";
                workSheet.Cell(1, 40).Value = "BCylLoan";
                workSheet.Cell(1, 41).Value = "ComputerLoan";
                workSheet.Cell(1, 42).Value = "PFLoan";
                workSheet.Cell(1, 43).Value = "WPFLoan";
                workSheet.Cell(1, 44).Value = "COSLoan";
                workSheet.Cell(1, 45).Value = "OtherLoan";
                workSheet.Cell(1, 46).Value = "Advance";
                workSheet.Cell(1, 47).Value = "Others";
                workSheet.Cell(1, 48).Value = "PensionCom";

                int row = 2;

                foreach (var item in finalAdjustments)
                {
                    workSheet.Cell(row, 1).Value = item.JobCode;
                    workSheet.Cell(row, 2).Value = item.EmployeeName;
                    workSheet.Cell(row, 3).Value = item.DesignationName;
                    workSheet.Cell(row, 4).Value = item.DepartmentName;
                    workSheet.Cell(row, 5).Value = item.AccountNumber;
                    workSheet.Cell(row, 6).Value = item.BankName;
                    workSheet.Cell(row, 7).Value = item.BankBranchName;
                    workSheet.Cell(row, 8).Value = item.BasicSalary;
                    workSheet.Cell(row, 9).Value = item.PersonalSalary;
                    workSheet.Cell(row, 10).Value = item.ConvenienceAllow;
                    workSheet.Cell(row, 11).Value = item.ArrearSalary;
                    workSheet.Cell(row, 12).Value = item.WorkDays;
                    workSheet.Cell(row, 13).Value = item.NumberOfShift;
                    workSheet.Cell(row, 14).Value = item.OtherSalary;
                    workSheet.Cell(row, 15).Value = item.SpecialBenefit;
                    workSheet.Cell(row, 16).Value = item.LunchAllow;
                    workSheet.Cell(row, 17).Value = item.TiffinAllow;
                    workSheet.Cell(row, 18).Value = item.ShiftAllow;
                    workSheet.Cell(row, 19).Value = item.HouseRentAllow;
                    workSheet.Cell(row, 20).Value = item.FamilyMedicalAllow;
                    workSheet.Cell(row, 21).Value = item.EducationAllowance;
                    workSheet.Cell(row, 22).Value = item.FieldAllow;
                    workSheet.Cell(row, 23).Value = item.OtSingle;
                    workSheet.Cell(row, 24).Value = item.OtDouble;
                    workSheet.Cell(row, 25).Value = item.OtAllow;
                    workSheet.Cell(row, 26).Value = item.FuelAllow;
                    workSheet.Cell(row, 27).Value = item.UtilityAllow;
                    workSheet.Cell(row, 28).Value = item.OtherAllow;
                    workSheet.Cell(row, 29).Value = item.RevenueStamp;
                    workSheet.Cell(row, 30).Value = item.ProvidentFund;
                    workSheet.Cell(row, 31).Value = item.WelfareFund;
                    workSheet.Cell(row, 32).Value = item.EmployeeClub;
                    workSheet.Cell(row, 33).Value = item.EmployeeUnion;
                    workSheet.Cell(row, 34).Value = item.Dormitory;
                    workSheet.Cell(row, 35).Value = item.HospitalDeduction;
                    workSheet.Cell(row, 36).Value = item.SpecialDeduction;
                    workSheet.Cell(row, 37).Value = item.FuelReturn;
                    workSheet.Cell(row, 38).Value = item.HBLoan;
                    workSheet.Cell(row, 39).Value = item.MCylLoan;
                    workSheet.Cell(row, 40).Value = item.BCylLoan;
                    workSheet.Cell(row, 41).Value = item.ComputerLoan;
                    workSheet.Cell(row, 42).Value = item.PFLoan;
                    workSheet.Cell(row, 43).Value = item.WPFLoan;
                    workSheet.Cell(row, 44).Value = item.CosLoan;
                    workSheet.Cell(row, 45).Value = item.OtherLoan;
                    workSheet.Cell(row, 46).Value = item.Advance;
                    workSheet.Cell(row, 47).Value = item.Others;
                    workSheet.Cell(row, 48).Value = item.PensionCom;
                    row++;
                }

                var stream = new MemoryStream();
                workBook.SaveAs(stream);
                stream.Position = 0;
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
            catch (Exception ex)
            {
                var workBook = new XLWorkbook();
                var workSheet = workBook.Worksheets.Add("Sheet1");

                var stream = new MemoryStream();
                workBook.SaveAs(stream);
                stream.Position = 0;
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }

        public IActionResult SMSService()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            return View();
        }


        public static DataTableCollection ReadExcelFiles(string filePath)
        {
            using (var stream =  System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var encoding = Encoding.GetEncoding("UTF-8");
                using (var reader = ExcelReaderFactory.CreateReader(stream,
                  new ExcelReaderConfiguration() { FallbackEncoding = encoding }))
                {
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration
                    {
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration { UseHeaderRow = true }
                    });

                    if (result.Tables.Count > 0)
                    {
                        return result.Tables;
                    }
                }
            }
            return null;
        }
        public IActionResult SalarySettinsConfig()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            return View();
        }

        public IActionResult InvestmentSchedule()
        {
            var userName = Request.Cookies["bgfcl_user_name"];
            if (userName == null)
            {
                return RedirectToAction("Login", "Dashboard");
            }
            return View();
        }
    }
}
