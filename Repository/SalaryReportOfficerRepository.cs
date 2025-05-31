using Contracts;
using Dapper;
using Entities;
using Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Repositories
{
    public class SalaryReportOfficerRepository:ISalaryReportOfficerContract
    {
        private readonly BgfclContext _context;

        public SalaryReportOfficerRepository(BgfclContext context)
        {
            _context = context;
        }

        public async Task<List<SalaryReportOfficer>> GetSalaryReportOfficer(int monthId, int? departmentId)
        {
            IEnumerable<SalaryReportOfficer> SalaryReportOfficer = null;
            var parameters = new DynamicParameters();
            parameters.Add("MonthID", monthId, DbType.Int32);

            // Start constructing the query
            var query = "SELECT * FROM SalaryReportOF WHERE MonthID = @MonthID";

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    SalaryReportOfficer = await connection.QueryAsync<SalaryReportOfficer>(query, parameters);
                }
                catch (Exception ex)
                {

                }
            }
            return SalaryReportOfficer?.ToList() ?? new List<SalaryReportOfficer>(); // Return an empty list if null
        }


        public async Task<List<SalaryReportOfficer>> GetSalaryControlSheetOf(List<string> jobCodes, int monthId, int? departmentId)
        {
            IEnumerable<SalaryReportOfficer> salaryReportOfficer = null;
            var parameters = new DynamicParameters();
            parameters.Add("MonthID", monthId, DbType.Int32);

            // Start constructing the query
            var query = "SELECT * FROM SalaryReportOF WHERE MonthID = @MonthID";

            // Add the job codes filter if specified
            if (jobCodes != null && jobCodes.Count > 0)
            {
                query += " AND JobCode IN @JobCodes";
                parameters.Add("JobCodes", jobCodes);
            }

            // Add the department filter if departmentId is not null
            if (departmentId.HasValue && departmentId.Value != 0)
            {
                query += " AND DepartmentId = @DepartmentId";
                parameters.Add("DepartmentId", departmentId.Value, DbType.Int32);
            }

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    salaryReportOfficer = await connection.QueryAsync<SalaryReportOfficer>(query, parameters);
                }
                catch (Exception ex)
                {
                    // Handle exception
                }
            }

            return salaryReportOfficer?.ToList() ?? new List<SalaryReportOfficer>(); // Return an empty list if null
        }
        public async Task<List<ScSalaryReport>> GetScSalarySheetPermanent(List<string> jobCodes, int monthId)
        {
            IEnumerable<ScSalaryReport> salaryReportPermanent = null;
            var parameters = new DynamicParameters();
            parameters.Add("MonthID", monthId, DbType.Int32);

            // Start constructing the query
            var query = "SELECT s.JobCode, s.EmployeeName, s.DesignationName, s.BankName, s.AccountNumber, " +
                        "MIN(e.JoiningDate) AS JoiningDate, g.GradeName AS PayScale, " +
                        "s.BasicSalary, s.OtherSalary, s.HouseRentAllow, s.FMAllow, s.Conveyance, " +
                        "s.PersonalSalary AS ElectricityAllow, s.LikeBasic AS GasAllow, " +
                        "s.DAidAllow AS CtAllow, '0.00' AS AcAllow, s.DeputationAllow AS ArrearAllow, " +
                        "s.ProvidentFund AS PF, s.RevenueStamp, s.Dormitory AS OtherDeduction, " +
                        "s.SpecialBenefit, 0 AS TotalDeduction, 0 AS GrossPay, 0 AS NetPay, 0 AS InstituteLib, " +
                        "e.EmpSl " +
                        "FROM SalaryReportOF s " +
                        "JOIN Employees e ON e.JobCode = s.JobCode " +
                        "JOIN Basics b ON b.BasicAmount = s.BasicSalary " +
                        "JOIN Grades g ON g.Id = b.GradeId " +
                        "WHERE e.ActiveStatus = 1 AND e.EmployeeTypeId = 1 AND s.MonthID = @MonthID";

            // Add the job codes filter if specified
            if (jobCodes != null && jobCodes.Count > 0)
            {
                query += " AND s.JobCode IN @JobCodes";  // Fixed placement
                parameters.Add("JobCodes", jobCodes);
            }

            // Add GROUP BY correctly
            query += " GROUP BY s.JobCode, s.EmployeeName, s.DesignationName, s.BankName, s.AccountNumber, " +
                     "s.BasicSalary, s.OtherSalary, s.HouseRentAllow, s.FMAllow, s.Conveyance, " +
                     "s.PersonalSalary, s.LikeBasic, s.DAidAllow, s.DeputationAllow, " +
                     "s.ProvidentFund, s.RevenueStamp, s.Dormitory, s.SpecialBenefit,g.GradeName, e.EmpSl";

            // ORDER BY should come after GROUP BY
            query += " ORDER BY CONVERT(INT, e.EmpSl) ASC;";

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    salaryReportPermanent = await connection.QueryAsync<ScSalaryReport>(query, parameters);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching salary report: {ex.Message}"); // Log the error
                }
            }

            return salaryReportPermanent?.ToList() ?? new List<ScSalaryReport>(); // Return an empty list if null
        }

        public async Task<List<ProvidentFundData>> GetProvidentFund(int monthId)
        {
            IEnumerable<ProvidentFundData> PfData = null;
            var parameters = new DynamicParameters();
            parameters.Add("MonthID", monthId, DbType.Int32);

            // Fixed: added space before WHERE
            var query = "Select  * ,e.EmployeeName,d.DesignationName from ProvidentFund As p Inner join Employees As  e on p.employeeId=e.id Inner join Designations As d on d.Id=e.DesignationId WHERE MonthID = @MonthID";

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    PfData = await connection.QueryAsync<ProvidentFundData>(query, parameters);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching provident fund data: {ex.Message}");
                }
            }

            return PfData?.ToList() ?? new List<ProvidentFundData>();
        }

        public async Task<List<ScSalaryReportContructual>> GetScSalarySheetContructual(List<string> jobCodes, int monthId)
        {
            IEnumerable<ScSalaryReportContructual> salaryReportContructual= null;
            var parameters = new DynamicParameters();
            parameters.Add("MonthID", monthId, DbType.Int32);

            // Start constructing the query
            var query = "select s.JobCode,s.EmployeeName,s.DesignationName,s.BankName,s.AccountNumber,e.JoiningDate,'0.00' as PayScale,s.BasicSalary,s.HouseRentAllow,s.FamilyMedicalAllow,s.ConvenienceAllow,s.PersonalSalary as ElectricityAllow,s.UtilityReturn as GasAllow,s.otherAllow AS CtAllow,'0.00' as AcAllow,s.ArrearSalary as ArrearAllow, s.ProvidentFund as PF,s.RevenueStamp,s.Dormitory as OtherDeduction,s.SpecialBenefit,0 as TotalDeduction, 0 as GrossPay,0 as NetPay, 0 as InstituteLib from SalaryReportJS s join Employees e on e.JobCode=s.JobCode Inner JOIN Grades g on g.EmployeeTypeId=e.EmployeeTypeId where ActiveStatus=1 and e.EmployeeTypeId=2 AND s.MonthID = @MonthID";

            // Add the job codes filter if specified
            if (jobCodes != null && jobCodes.Count > 0)
            {
                query += " AND JobCode IN @JobCodes";
                parameters.Add("JobCodes", jobCodes);
            }
            query += " ORDER BY CONVERT(INT, e.EmpSl) ASC;";


            using (var connection = _context.CreateConnection())
            {
                try
                {
                    salaryReportContructual = await connection.QueryAsync<ScSalaryReportContructual>(query, parameters);
                }
                catch (Exception ex)
                {
                    // Handle exception
                }
            }

            return salaryReportContructual?.ToList() ?? new List<ScSalaryReportContructual>(); // Return an empty list if null
        }

        public async Task<List<DailyWorker>> GetScSalarySheetDailyWorker(List<string> jobCodes, int monthId)
        {
            IEnumerable<DailyWorker> salaryReportDailyWorker = null;
            var parameters = new DynamicParameters();
            parameters.Add("MonthID", monthId, DbType.Int32);

            // Start constructing the query
            var query = "select s.JobCode,s.EmployeeName,s.DesignationName,s.BankName,s.AccountNumber,s.RevenueStamp,s.TotalDayCount as Attendance,s.Per_attendence as PerAttendance,GrossPay,NetPay, 0 as InstituteLib from SalaryReportJS s join Employees e on e.JobCode=s.JobCode  where ActiveStatus=1 and e.EmployeeTypeId=3 AND s.MonthID = @MonthID";

            // Add the job codes filter if specified
            if (jobCodes != null && jobCodes.Count > 0)
            {
                query += " AND JobCode IN @JobCodes";
                parameters.Add("JobCodes", jobCodes);
            }
            query += " ORDER BY CONVERT(INT, e.EmpSl) ASC;";


            using (var connection = _context.CreateConnection())
            {
                try
                {
                    salaryReportDailyWorker = await connection.QueryAsync<DailyWorker>(query, parameters);
                }
                catch (Exception ex)
                {
                    // Handle exception
                }
            }

            return salaryReportDailyWorker?.ToList() ?? new List<DailyWorker>(); // Return an empty list if null
        }

        public async Task<List<ScSalaryReportUnified>> GetAllSalarySheetUnified(List<string> jobCodes, int monthId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("MonthID", monthId, DbType.Int32);
            if (jobCodes != null && jobCodes.Count > 0)
            {
                parameters.Add("JobCodes", jobCodes);
            }

            var permanentQuery = @"
                                SELECT s.JobCode, s.EmployeeName, s.DesignationName, s.BankName, s.AccountNumber, 
                                       MIN(e.JoiningDate) AS JoiningDate, MAX(g.GradeName) AS PayScale, 
                                       s.BasicSalary, s.OtherSalary, s.HouseRentAllow, s.FMAllow, s.Conveyance, 
                                       s.PersonalSalary AS ElectricityAllow, s.LikeBasic AS GasAllow, 
                                       s.DAidAllow AS CtAllow, '0.00' AS AcAllow, s.DeputationAllow AS ArrearAllow, 
                                       s.ProvidentFund AS PF, s.RevenueStamp, s.Dormitory AS OtherDeduction, 
                                       s.SpecialBenefit, 0 AS TotalDeduction, 0 AS GrossPay, 0 AS NetPay, 0 AS InstituteLib, 
                                       e.EmpSl
                                FROM SalaryReportOF s
                                JOIN Employees e ON e.JobCode = s.JobCode
                                JOIN Grades g ON g.EmployeeTypeId = e.EmployeeTypeId
                                WHERE e.ActiveStatus = 1 AND e.EmployeeTypeId = 1 AND s.MonthID = @MonthID";

            if (jobCodes != null && jobCodes.Count > 0)
            {
                permanentQuery += " AND s.JobCode IN @JobCodes";
            }

            permanentQuery += @"
                                GROUP BY s.JobCode, s.EmployeeName, s.DesignationName, s.BankName, s.AccountNumber, 
                                         s.BasicSalary, s.OtherSalary, s.HouseRentAllow, s.FMAllow, s.Conveyance, 
                                         s.PersonalSalary, s.LikeBasic, s.DAidAllow, s.DeputationAllow, 
                                         s.ProvidentFund, s.RevenueStamp, s.Dormitory, s.SpecialBenefit, e.EmpSl
                                ORDER BY CONVERT(INT, e.EmpSl) ASC;
                            ";

            var contractualQuery = @"
                                    SELECT s.JobCode, s.EmployeeName, s.DesignationName, s.BankName, s.AccountNumber, 
                                           e.JoiningDate, '0.00' AS PayScale, 
                                           s.BasicSalary, 0.00 AS OtherSalary, s.HouseRentAllow, s.FamilyMedicalAllow AS FMAllow, 
                                           s.ConvenienceAllow AS Conveyance, s.PersonalSalary AS ElectricityAllow, 
                                           s.UtilityReturn AS GasAllow, s.OtherAllow AS CtAllow, '0.00' AS AcAllow, 
                                           s.ArrearSalary AS ArrearAllow, s.ProvidentFund AS PF, s.RevenueStamp, 
                                           s.Dormitory AS OtherDeduction, s.SpecialBenefit, 
                                           0 AS TotalDeduction, 0 AS GrossPay, 0 AS NetPay, 0 AS InstituteLib, 
                                           e.EmpSl
                                    FROM SalaryReportJS s
                                    JOIN Employees e ON e.JobCode = s.JobCode
                                    INNER JOIN Grades g ON g.EmployeeTypeId = e.EmployeeTypeId
                                    WHERE e.ActiveStatus = 1 AND e.EmployeeTypeId = 2 AND s.MonthID = @MonthID";

            if (jobCodes != null && jobCodes.Count > 0)
            {
                contractualQuery += " AND s.JobCode IN @JobCodes";
            }

            contractualQuery += " ORDER BY CONVERT(INT, e.EmpSl) ASC;";

            List<ScSalaryReportUnified> unifiedList = new();

            using var connection = _context.CreateConnection();
            try
            {
                var permanentData = await connection.QueryAsync<ScSalaryReportUnified>(permanentQuery, parameters);
                var contractualData = await connection.QueryAsync<ScSalaryReportUnified>(contractualQuery, parameters);

                unifiedList.AddRange(permanentData);
                unifiedList.AddRange(contractualData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching salary report: {ex.Message}");
            }

            return unifiedList;
        }

        public async Task<List<SalaryReportJS>> GetSalaryControlSheetJS(List<string> jobCodes, int monthId, int? departmentId)
        {
            IEnumerable<SalaryReportJS> salaryReportOfficer = null;
            var parameters = new DynamicParameters();
            parameters.Add("MonthID", monthId, DbType.Int32);

            // Start constructing the query
            var query = "SELECT * FROM SalaryReportJS WHERE MonthID = @MonthID";

            // Add the job codes filter if specified
            if (jobCodes != null && jobCodes.Count > 0)
            {
                query += " AND JobCode IN @JobCodes";
                parameters.Add("JobCodes", jobCodes);
            }

            // Add the department filter if departmentId is not null
            if (departmentId.HasValue && departmentId.Value != 0)
            {
                query += " AND DepartmentId = @DepartmentId";
                parameters.Add("DepartmentId", departmentId.Value, DbType.Int32);
            }

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    salaryReportOfficer = await connection.QueryAsync<SalaryReportJS>(query, parameters);
                }
                catch (Exception ex)
                {
                    // Handle exception
                }
            }

            return salaryReportOfficer?.ToList() ?? new List<SalaryReportJS>(); // Return an empty list if null
        }


        // Salary Control sheet JS*****************************************
        public async Task<List<SalaryReportJS>> GetSalaryReportJS(int monthId, int? departmentId)
        {
            IEnumerable<SalaryReportJS> finalAdjustments = null;
            var parameters = new DynamicParameters();
            parameters.Add("MonthID", monthId, DbType.Int32);

            // Start constructing the query
            var query = "SELECT * FROM SalaryReportJS WHERE MonthID = @MonthID";

            // Add the department filter only if departmentId is not null
            if (departmentId.HasValue && departmentId.Value != 0)
            {
                query += " AND DepartmentId = @DepartmentId";
                parameters.Add("DepartmentId", departmentId.Value, DbType.Int32);
            }

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    finalAdjustments = await connection.QueryAsync<SalaryReportJS>(query, parameters);
                }
                catch (Exception ex)
                {
                    // Handle exception (e.g., log the error)
                    // throw; // Optional: re-throw the exception to handle it further up the call stack
                }
            }
            return finalAdjustments?.ToList() ?? new List<SalaryReportJS>(); // Return an empty list if null
        }

        public async Task<List<SalaryReportOfficer>> GetSalaryBankForwardOF(int monthId, string? bank)
        {
            IEnumerable<SalaryReportOfficer> SalaryBankForwardOF = null;
            var parameters = new DynamicParameters();
            parameters.Add("MonthID", monthId, DbType.Int32);

            // Start constructing the query
            var query = "SELECT * FROM SalaryReportOF WHERE MonthID = @MonthID";

            // Add the department filter only if departmentId is not null
            if (!string.IsNullOrEmpty(bank))
            {
                query += " AND BankName = @BankName";
                parameters.Add("BankName", bank, DbType.String);
            }
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    SalaryBankForwardOF = await connection.QueryAsync<SalaryReportOfficer>(query, parameters);
                }
                catch (Exception ex)
                {

                }
            }
            return SalaryBankForwardOF?.ToList() ?? new List<SalaryReportOfficer>(); 
        }

        public async Task<List<SalaryReportJS>> GetSalaryBankForwardJS(int monthId, string? bank)
        {
            IEnumerable<SalaryReportJS> SalaryBankForwardJS = null;
            var parameters = new DynamicParameters();
            parameters.Add("MonthID", monthId, DbType.Int32);

            // Start constructing the query
            var query = "SELECT * FROM SalaryReportJS WHERE MonthID = @MonthID";

            if (!string.IsNullOrEmpty(bank))
            {
                query += " AND BankName = @BankName"; 
                parameters.Add("BankName", bank, DbType.String);
            }

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    SalaryBankForwardJS = await connection.QueryAsync<SalaryReportJS>(query, parameters);
                }
                catch (Exception ex)
                {
                    // Handle the exception (logging, rethrowing, etc.)
                    throw new Exception("Error executing query", ex);
                }
            }

            return SalaryBankForwardJS?.ToList() ?? new List<SalaryReportJS>();
        }

        public Task<List<SalaryReportOfficer>> GetSalaryBankForwardJRStaff(int monthId, int? bank)
        {
            throw new NotImplementedException();
        }


        public async Task<List<SalaryReportOfficer>> GetSalaryPaySlip(string jobCode, int employeeType, int monthId, int? departmentId)
        {
            IEnumerable<SalaryReportOfficer> SalarypaySlip = null;
            var parameters = new DynamicParameters();
            parameters.Add("MonthID", monthId, DbType.Int32);
            parameters.Add("JobCode", jobCode, DbType.String); 

            string query = "";
            if (employeeType == 1)
            {
                query = "SELECT * FROM SalaryReportOF WHERE MonthID = @MonthID AND JobCode = @JobCode";
            }
            else
            {
                query = "SELECT * FROM SalaryReportJS WHERE MonthID = @MonthID AND JobCode = @JobCode";
            }

            // Add the department filter only if departmentId is not null
            if (departmentId.HasValue && departmentId.Value != 0)
            {
                query += " AND DepartmentId = @DepartmentId";
                parameters.Add("DepartmentId", departmentId.Value, DbType.Int32);
            }

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    SalarypaySlip = await connection.QueryAsync<SalaryReportOfficer>(query, parameters);
                }
                catch (Exception ex)
                {
                    // Log or handle exception appropriately
                    throw new Exception("Error executing SQL query", ex);
                }
            }

            return SalarypaySlip?.ToList() ?? new List<SalaryReportOfficer>(); // Return an empty list if null
        }

        public async Task<List<SalaryReportJS>> GetSalaryPaySlipJS(string jobCode, int employeeType, int monthId, int? departmentId)
        {
            IEnumerable<SalaryReportJS> SalarypaySlip = null;
            var parameters = new DynamicParameters();
            parameters.Add("MonthID", monthId, DbType.Int32);
            parameters.Add("JobCode", jobCode, DbType.String);

            string query = "";
            if (employeeType == 1)
            {
                query = "SELECT * FROM SalaryReportOF WHERE MonthID = @MonthID AND JobCode = @JobCode";
            }
            else
            {
                query = "SELECT * FROM SalaryReportJS WHERE MonthID = @MonthID AND JobCode = @JobCode";
            }

            // Add the department filter only if departmentId is not null
            if (departmentId.HasValue && departmentId.Value != 0)
            {
                query += " AND DepartmentId = @DepartmentId";
                parameters.Add("DepartmentId", departmentId.Value, DbType.Int32);
            }

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    SalarypaySlip = await connection.QueryAsync<SalaryReportJS>(query, parameters);
                }
                catch (Exception ex)
                {
                    // Log or handle exception appropriately
                    throw new Exception("Error executing SQL query", ex);
                }
            }

            return SalarypaySlip?.ToList() ?? new List<SalaryReportJS>(); // Return an empty list if null
        }



        public async Task<List<SalaryReportOfficer>> GetSalaryYearPaySlip(string jobCode, int employeeType, int fmonthId, int tmonthId, int? departmentId)
        {
            IEnumerable<SalaryReportOfficer> SalarypaySlip = null;
            var parameters = new DynamicParameters();
            parameters.Add("fMonthId", fmonthId, DbType.Int32);
            parameters.Add("tMonthId", tmonthId, DbType.Int32);
            parameters.Add("JobCode", jobCode, DbType.String);
            employeeType = 1;
            string query = "";
           query = @"SELECT 
            MonthID,
            JobCode, 
            EmployeeName, 
            DesignationName,
            DepartmentName,
            AccountNumber,
            BankName,
            BankBranchName,
            WorkDays,
            JournalNumber,
            TIN,
            BasicSalary,
            LikeBasic,
            PersonalSalary,
            ArrearSalary,
            OtherSalary,
            SpecialBenefit,
            LunchAllow,
            TiffinAllow,
            WashAllow,
            Conveyance,
            HouseRentAllow,
            FMAllow,
            EducationalAllow,
            FieldRiskAllow,
            ChargeAllow,
            DAidAllow,
            DeputationAllow,
            OtherAllow,
            RevenueStamp,
            ProvidentFund,
            WelfareFund,
            OfficerClub,
            OfficerAssociation,
            Dormitory,
            PensionOfficer,
            MedicalFund,
            TMBill,
            Hospitalisation,
            HouseRentReturn,
            SpecialDeduction,
            FuelReturn,
            HBLoan,
            MCylLoan,
            BCylLoan,
            PFLoan,
            WPFLoan,
            CosLoan,
            OtherLoan,
            Advance,
            IncomeTax,
            Other
            FROM
                SalaryReportOF
            WHERE 
                JobCode = @JobCode 
                AND MonthId BETWEEN @fMonthId AND @tMonthId
                Order By MonthID";
            
            

            // Add the department filter only if departmentId is not null
            if (departmentId.HasValue && departmentId.Value != 0)
            {
                query += " AND DepartmentId = @DepartmentId";
                parameters.Add("DepartmentId", departmentId.Value, DbType.Int32);
            }

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    SalarypaySlip = await connection.QueryAsync<SalaryReportOfficer>(query, parameters);
                }
                catch (Exception ex)
                {
                    // Log or handle exception appropriately
                    throw new Exception("Error executing SQL query", ex);
                }
            }

            return SalarypaySlip?.ToList() ?? new List<SalaryReportOfficer>(); // Return an empty list if null
        }

        public async Task<List<SalaryReportJS>> GetSalaryYearPaySlipJS(string jobCode, int employeeType, int fmonthId, int tmonthId, int? departmentId)
        {
            IEnumerable<SalaryReportJS> SalarypaySlipJS = null;
            var parameters = new DynamicParameters();
            parameters.Add("fMonthId", fmonthId, DbType.Int32);
            parameters.Add("tMonthId", tmonthId, DbType.Int32);
            parameters.Add("JobCode", jobCode, DbType.String);
            employeeType = 1;
            string query = "";
            
                query = @"SELECT 
            MonthID,
            JobCode, 
            EmployeeName, 
            DesignationName,
            DepartmentName,
            AccountNumber,
            BankName,
            BankBranchName,
            WorkDays,
            JournalNumber,
            TIN,
            PensionCom,
            BasicSalary,
            PersonalSalary, 
            ConvenienceAllow,
            ArrearSalary, 
            OtherSalary, 
            SpecialBenefit, 
            LunchAllow, 
            TiffinAllow, 
            ShiftAllow, 
            HouseRentAllow, 
            FamilyMedicalAllow, 
            EducationAllowance,
            FieldAllow,
            OtSingle,
            OtDouble,
            OtAllow, 
            FuelAllow, 
            UtilityAllow, 
            OtherAllow, 
            RevenueStamp, 
            ProvidentFund, 
            WelfareFund,
            EmployeeClub,
            EmployeeUnion,
            Dormitory, 
            HospitalDeduction, 
            SpecialDeduction, 
            FuelReturn, 
            HBLoan, 
            MCylLoan, 
            BCylLoan,
            ComputerLoan, 
            PFLoan, 
            WPFLoan, 
            CosLoan, 
            OtherLoan,
            Advance, 
            Others
            FROM 
                SalaryReportJS
            WHERE 
                JobCode = @JobCode 
                AND MonthId BETWEEN @fMonthId AND @tMonthId
                  Order By MonthID";
            

            // Add the department filter only if departmentId is not null
            if (departmentId.HasValue && departmentId.Value != 0)
            {
                query += " AND DepartmentId = @DepartmentId";
                parameters.Add("DepartmentId", departmentId.Value, DbType.Int32);
            }

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    SalarypaySlipJS = await connection.QueryAsync<SalaryReportJS>(query, parameters);
                }
                catch (Exception ex)
                {
                    // Log or handle exception appropriately
                    throw new Exception("Error executing SQL query", ex);
                }
            }

            return SalarypaySlipJS?.ToList() ?? new List<SalaryReportJS>(); // Return an empty list if null
        }


        public async Task<List<SalaryReportOfficer>> GetYearlySalaryControlSheetOF(List<string> jobCode, int fromMonthId, int toMonthId, string? department)
        {
            IEnumerable<SalaryReportOfficer> yearlyControlSheet = null;
            var parameters = new DynamicParameters();

            // Adding the parameters
            parameters.Add("fMonthID", fromMonthId, DbType.Int32);
            parameters.Add("tMonthID", toMonthId, DbType.Int32);

            // Base query
            var query = @"SELECT JobCode, EmployeeName, DesignationName, DepartmentName, BankName, BankBranchName,
                        SUM(BasicSalary) AS BasicSalary,
                        SUM(PersonalSalary) AS PersonalSalary,
                        SUM(ArrearSalary) AS ArrearSalary,
                        SUM(WorkDays) AS WorkDays,
                        SUM(LikeBasic) AS LikeBasic,
                        SUM(OtherSalary) AS OtherSalary,
                        SUM(SpecialBenefit) AS SpecialBenefit,
                        SUM(LunchAllow) AS LunchAllow,
                        SUM(TiffinAllow) AS TiffinAllow,
                        SUM(WashAllow) AS WashAllow,
                        SUM(HouseRentAllow) AS HouseRentAllow,
                        SUM(Conveyance) AS Conveyance,
                        SUM(FMAllow) AS FMAllow,
                        SUM(EducationalAllow) AS EducationalAllow,
                        SUM(FieldRiskAllow) AS FieldRiskAllow,
                        SUM(ChargeAllow) AS ChargeAllow,
                        SUM(DAidAllow) AS DAidAllow,
                        SUM(DeputationAllow) AS DeputationAllow,
                        SUM(OtherAllow) AS OtherAllow,
                        SUM(RevenueStamp) AS RevenueStamp,
                        SUM(ProvidentFund) AS ProvidentFund,
                        SUM(PensionOfficer) AS PensionOfficer,
                        SUM(WelfareFund) AS WelfareFund,
                        SUM(OfficerClub) AS OfficerClub,
                        SUM(OfficerAssociation) AS OfficerAssociation,
                        SUM(MedicalFund) AS MedicalFund,
                        SUM(TMBill) AS TMBill,
                        SUM(Dormitory) AS Dormitory,
                        SUM(Hospitalisation) AS Hospitalisation,
                        SUM(HouseRentReturn) AS HouseRentReturn,
                        SUM(SpecialDeduction) AS SpecialDeduction,
                        SUM(FuelReturn) AS FuelReturn,
                        SUM(HBLoan) AS HBLoan,
                        SUM(MCylLoan) AS MCylLoan,
                        SUM(BCylLoan) AS BCylLoan,
                        SUM(ComLoan) AS ComLoan,
                        SUM(CarLoan) AS CarLoan,
                        SUM(PFLoan) AS PFLoan,
                        SUM(WPFLoan) AS WPFLoan,
                        SUM(CosLoan) AS CosLoan,
                        SUM(OtherLoan) AS OtherLoan,
                        SUM(Advance) AS Advance,
                        SUM(Other) AS Other,
                        SUM(IncomeTax) AS IncomeTax,
                        SUM(CME) AS CME
                    FROM SalaryReportOF
                    WHERE MonthID BETWEEN @fMonthID AND @tMonthID";

            // Adding conditions for JobCodes
            if (jobCode != null && jobCode.Count > 0)
            {
                query += " AND JobCode IN @JobCodes";
                parameters.Add("JobCodes", jobCode);
            }

            // Adding condition for department
            if (!string.IsNullOrEmpty(department))
            {
                query += " AND DepartmentName = @DepartmentName";
                parameters.Add("DepartmentName", department, DbType.String);
            }

            // Grouping
            query += @" GROUP BY JobCode, EmployeeName, DesignationName, DepartmentName, BankName, BankBranchName";

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    yearlyControlSheet = await connection.QueryAsync<SalaryReportOfficer>(query, parameters);
                }
                catch (Exception ex)
                {
                    // You can add logging here if needed
                    throw new Exception("An error occurred while retrieving the salary control sheet data", ex);
                }
            }

            return yearlyControlSheet?.ToList() ?? new List<SalaryReportOfficer>();
        }



        public async Task<List<SalaryReportJS>> GetYearlySalaryControlSheetJS(List<string> jobCode, int fromMonthId, int toMonthId, string? department)
        {
            IEnumerable<SalaryReportJS> YearlyControlSheet = null;
            var parameters = new DynamicParameters();
            parameters.Add("fMonthID", fromMonthId, DbType.Int32);
            parameters.Add("tMonthID", toMonthId, DbType.Int32);

            // Start constructing the query
            var query = @"
                SELECT  
                    JobCode, 
                    EmployeeName, 
                    DesignationName, 
                    DepartmentName, 
                    BankName, 
                    BankBranchName,
                    SUM(BasicSalary) AS BasicSalary,
                    SUM(PersonalSalary) AS PersonalSalary,
                    SUM(ConvenienceAllow) AS ConvenienceAllow,
                    SUM(ArrearSalary) AS ArrearSalary,
                    SUM(WorkDays) AS WorkDays,
                    SUM(NumberOfShift) AS NumberOfShift,
                    SUM(OtherSalary) AS OtherSalary,
                    SUM(SpecialBenefit) AS SpecialBenefit,
                    SUM(LunchAllow) AS LunchAllow,
                    SUM(TiffinAllow) AS TiffinAllow,
                    SUM(ShiftAllow) AS ShiftAllow,
                    SUM(HouseRentAllow) AS HouseRentAllow,
                    SUM(FamilyMedicalAllow) AS FamilyMedicalAllow,
                    SUM(EducationAllowance) AS EducationAllowance,
                    SUM(FieldAllow) AS FieldAllow,
                    SUM(OtSingle) AS OtSingle,
                    SUM(OtDouble) AS OtDouble,
                    SUM(OtAllow) AS OtAllow,
                    SUM(FuelAllow) AS FuelAllow,
                    SUM(UtilityAllow) AS UtilityAllow,
                    SUM(OtherAllow) AS OtherAllow,
                    SUM(RevenueStamp) AS RevenueStamp,
                    SUM(ProvidentFund) AS ProvidentFund,
                    SUM(WelfareFund) AS WelfareFund,
                    SUM(EmployeeClub) AS EmployeeClub,
                    SUM(EmployeeUnion) AS EmployeeUnion,
                    SUM(Dormitory) AS Dormitory,
                    SUM(HospitalDeduction) AS HospitalDeduction,
                    SUM(SpecialDeduction) AS SpecialDeduction,
                    SUM(FuelReturn) AS FuelReturn,
                    SUM(HBLoan) AS HBLoan,
                    SUM(MCylLoan) AS MCylLoan,
                    SUM(BCylLoan) AS BCylLoan,
                    SUM(ComputerLoan) AS ComputerLoan,
                    SUM(PFLoan) AS PFLoan,
                    SUM(WPFLoan) AS WPFLoan,
                    SUM(CosLoan) AS CosLoan,
                    SUM(OtherLoan) AS OtherLoan,
                    SUM(Advance) AS Advance,
                    SUM(Others) AS Others,
                    SUM(PensionCom) AS PensionCom,
                    SUM(UtilityReturn) AS UtilityReturn
                FROM SalaryReportJS
                WHERE MonthID BETWEEN @fMonthID AND @tMonthID";

            // Add job code filter if jobCode list is not empty
            if (jobCode != null && jobCode.Count > 0)
            {
                query += " AND JobCode IN @JobCodes";
                parameters.Add("JobCodes", jobCode);
            }

            // Add department filter if department is not null or empty
            if (!string.IsNullOrEmpty(department))
            {
                query += " AND DepartmentName = @DepartmentName";
                parameters.Add("DepartmentName", department, DbType.String);
            }

            // Grouping by required fields
            query += @"
                    GROUP BY 
                        JobCode, 
                        EmployeeName, 
                        DesignationName, 
                        DepartmentName, 
                        BankName, 
                        BankBranchName";

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    YearlyControlSheet = await connection.QueryAsync<SalaryReportJS>(query, parameters);
                }
                catch (Exception ex)
                {
                    // Log the exception (or use a logger)
                    Console.WriteLine(ex.Message);
                    throw new Exception("An error occurred while fetching the salary report data.", ex);
                }
            }

            return YearlyControlSheet?.ToList() ?? new List<SalaryReportJS>();
        }
        public async Task<List<SalaryReportOfficer>> GetSalaryReportOfficerData()
        {
            IEnumerable<SalaryReportOfficer> salaryReportOfficer = new List<SalaryReportOfficer>();
            var parameters = new DynamicParameters();
            var query = "SELECT * FROM SalaryReportOF";

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    salaryReportOfficer = await connection.QueryAsync<SalaryReportOfficer>(query, parameters);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Database Error: {ex.Message}"); // Log error for debugging
                    return new List<SalaryReportOfficer>(); // Return empty list on error
                }
            }
            return salaryReportOfficer.ToList();
        }

        public async Task<List<SalaryReportOfficer>> GetreportOfficerWithFilter(int? employeeTypeId, int? schoolId, int? roleId, int? departmentId, int? designationId)
        {
            string query = @"
                              SELECT s.Id, s.JobCode, s.EmployeeName, 
                              s.DesignationName, s.DepartmentName, s.BasicSalary,s.GrossPay,s.TotalDeduction,s.NetPay 
                              FROM SalaryReportOF as s  
                              WHERE s.EmployeeType = @EmployeeTypeId";

            if (schoolId.HasValue)
                query += " AND s.SchoolId = @SchoolId";
            if (roleId.HasValue)
                query += " AND s.RoleId = @RoleId";
            if (departmentId.HasValue)
                query += " AND s.DepartmentId = @DepartmentId";
            if (designationId.HasValue)
                query += " AND s.DesignationId = @DesignationId";

            query += " ORDER BY TRY_CAST(s.id AS INT) ASC;";

            using (var connection = _context.CreateConnection())
            {
                var data = await connection.QueryAsync<SalaryReportOfficer>(query, new
                {
                    EmployeeTypeId = employeeTypeId,
                    SchoolId = schoolId,
                    RoleId = roleId,
                    DepartmentId = departmentId,
                    DesignationId = designationId
                });

                return data.ToList();
            }
        }
        public async Task<List<SalaryReportJS>> GetSalaryReportContractData()
        {
            IEnumerable<SalaryReportJS> SalaryReportOfficer = null;
            var parameters = new DynamicParameters();

            // Start constructing the query
            var query = "SELECT * FROM SalaryReportJS";

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    SalaryReportOfficer = await connection.QueryAsync<SalaryReportJS>(query, parameters);
                }
                catch (Exception ex)
                {

                }
            }
            return SalaryReportOfficer?.ToList() ?? new List<SalaryReportJS>(); // Return an empty list if null
        }
        public async Task<List<SalaryReportJS>> GetreportContractWithFilter(int? employeeTypeId, int? schoolId, int? roleId, int? departmentId, int? designationId)
        {
            string query = @"
                              SELECT s.Id, s.JobCode, s.EmployeeName, 
                              s.DesignationName, s.DepartmentName, s.BasicSalary,s.GrossPay,s.TotalDeduction,s.NetPay 
                              FROM SalaryReportOF as s  
                              WHERE s.EmployeeType = @EmployeeTypeId";

            if (schoolId.HasValue)
                query += " AND s.SchoolId = @SchoolId";
            if (roleId.HasValue)
                query += " AND s.RoleId = @RoleId";
            if (departmentId.HasValue)
                query += " AND s.DepartmentId = @DepartmentId";
            if (designationId.HasValue)
                query += " AND s.DesignationId = @DesignationId";

            query += " ORDER BY TRY_CAST(s.id AS INT) ASC;";

            using (var connection = _context.CreateConnection())
            {
                var data = await connection.QueryAsync<SalaryReportJS>(query, new
                {
                    EmployeeTypeId = employeeTypeId,
                    SchoolId = schoolId,
                    RoleId = roleId,
                    DepartmentId = departmentId,
                    DesignationId = designationId
                });

                return data.ToList();
            }
        }
    }
}
