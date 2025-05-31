using Contracts;
using Dapper;
using Entities;
using Entities.ViewModels;
using Entities.ViewModels.Reports;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class AmenitiesRepository:IAmenitiesContract
    {
        private readonly BgfclContext _context;

        public AmenitiesRepository(BgfclContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAmenities(Amenities amenities)
        {
            int result = 0;
            var query = "INSERT INTO amenities (JobCode,WageGMSCB,HouseKeepUp,FuelSubsidy,WESubsidy,OtherDeduction,OtherPay,PayModeId,BankId,BankBranchId,AccountNumber,CreatedBy,CreatedDate) VALUES (@JobCode,@WageGMSCB,@HouseKeepUp,@FuelSubsidy,@WESubsidy,@OtherDeduction,@OtherPay,@PayModeId,@BankId,@BankBranchId,@AccountNumber,@CreatedBy,@CreatedDate)";
            var parameters = new DynamicParameters();
            parameters.Add("JobCode", amenities.JobCode, DbType.String);
            parameters.Add("WageGMSCB", amenities.WageGMSCB, DbType.Double);
            parameters.Add("HouseKeepUp", amenities.HouseKeepUp, DbType.Double);
            parameters.Add("FuelSubsidy", amenities.FuelSubsidy, DbType.Double);
            parameters.Add("WESubsidy", amenities.WESubsidy, DbType.Double);
            parameters.Add("OtherDeduction", amenities.OtherDeduction, DbType.Double);
            parameters.Add("OtherPay", amenities.OtherPay, DbType.Double);
            parameters.Add("PayModeId", amenities.PayModeId, DbType.Int32);
            parameters.Add("BankId", amenities.BankId, DbType.Int32);
            parameters.Add("BankBranchId", amenities.BankBranchId, DbType.Int32);
            parameters.Add("AccountNumber", amenities.AccountNumber, DbType.String);
            parameters.Add("CreatedBy", amenities.CreatedBy, DbType.String);
            parameters.Add("CreatedDate", amenities.CreatedDate, DbType.DateTime);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    result = await connection.ExecuteAsync(query, parameters);
                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }
        public async Task<int> UpdateAmenities(Amenities amenities)
        {
            int result = 0;
            var query = "update amenities set WageGMSCB=@WageGMSCB,HouseKeepUp=@HouseKeepUp,FuelSubsidy=@FuelSubsidy,WESubsidy=@WESubsidy,OtherDeduction=@OtherDeduction,OtherPay=@OtherPay,PayModeId=@PayModeId,BankId=@BankId,BankBranchId=@BankBranchId,AccountNumber=@AccountNumber,UpdatedBy=@UpdatedBy,UpdatedDate=@UpdatedDate where Id=@Id";
            var parameters = new DynamicParameters();

            parameters.Add("WageGMSCB", amenities.WageGMSCB, DbType.Double);
            parameters.Add("HouseKeepUp", amenities.HouseKeepUp, DbType.Double);
            parameters.Add("FuelSubsidy", amenities.FuelSubsidy, DbType.Double);
            parameters.Add("WESubsidy", amenities.WESubsidy, DbType.Double);
            parameters.Add("OtherDeduction", amenities.OtherDeduction, DbType.Double);
            parameters.Add("OtherPay", amenities.OtherPay, DbType.Double);
            parameters.Add("PayModeId", amenities.PayModeId, DbType.Int32);
            parameters.Add("BankId", amenities.BankId, DbType.Int32);
            parameters.Add("BankBranchId", amenities.BankBranchId, DbType.Int32);
            parameters.Add("AccountNumber", amenities.AccountNumber, DbType.String);
            parameters.Add("UpdatedBy", amenities.UpdatedBy, DbType.String);
            parameters.Add("UpdatedDate", amenities.UpdatedDate, DbType.DateTime);
            parameters.Add("Id", amenities.Id, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    result = await connection.ExecuteAsync(query, parameters);
                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }
        public async Task<List<AmenitiesViewModel>> GetAmenities()
        {
            var query = $"select Amenities.Id,Amenities.JobCode,Employees.EmployeeName, Departments.DepartmentName, Designations.DesignationName " +
                $"from Amenities join employees on Amenities.Jobcode = employees.Jobcode " +
                $"join Departments on Departments.Id = employees.DepartmentId join Designations on Designations.Id = Employees.DesignationId";
            using (var connection = _context.CreateConnection())
            {
                var salarySettings = await connection.QueryAsync<AmenitiesViewModel>(query);
                return salarySettings.ToList();
            }
        }

        public async Task<Amenities> GetAmenitiesById(int amenitiesId)
        {
            var query = $"select * from amenities where id=@amenitiesId";
            using (var connection = _context.CreateConnection())
            {
                var amenities = await connection.QueryFirstOrDefaultAsync<Amenities>(query,new { amenitiesId });
                return amenities;
            }
        }
        public async Task<Amenities> GetAmenitiesByJobCode(string jobCode)
        {
            var query = $"select jobcode from amenities where JobCode=@jobCode";
            using (var connection = _context.CreateConnection())
            {
                var amenities = await connection.QueryFirstOrDefaultAsync<Amenities>(query, new { jobCode });
                return amenities;
            }
        }

        public async Task<AmenitiesReportMaster> GetAmenitiesReportMasterByMonthId(int monthId)
        {
            var query = $"select * from AmenitiesReportMaster where MonthId=@monthId";
            using (var connection = _context.CreateConnection())
            {
                var amenitiesMaster = await connection.QueryFirstOrDefaultAsync<AmenitiesReportMaster>(query, new { monthId });
                return amenitiesMaster;
            }
        }

        public async Task<int> ProcessAmenities(int monthId,int status)
        {
            int result = 0;
            var parameters = new DynamicParameters();
            parameters.Add("MonthId", monthId, DbType.Int32);
            parameters.Add("Status", status, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    result = await connection.ExecuteAsync("spAmenitiesProcess", parameters, commandType: CommandType.StoredProcedure);

                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }

        public async Task<List<AmenitiesFinalAdjustmentViewModel>> GetAmenitiesFinalAdjustmentsByMonthId(int monthId)
        {
            var query = $"select * from AmenitiesReport where MonthId=@monthId";
            using (var connection = _context.CreateConnection())
            {
                var amenitiesFinalAdjustments = await connection.QueryAsync<AmenitiesFinalAdjustmentViewModel>(query, new { monthId });
                return amenitiesFinalAdjustments.ToList();
            }
        }

        public async Task<List<AmenitiesFinalAdjustmentViewModel>> GetAmenitiesByMonthIdAndBankId(int monthId, int? bankId)
        {
            string query;

            if (bankId==0)
            {
                query = "SELECT * FROM AmenitiesReport WHERE MonthId = @monthId";
                
            }
            else
            {
                query = "SELECT * FROM AmenitiesReport WHERE MonthId = @monthId AND BankId = @bankId";
            }

            using (var connection = _context.CreateConnection())
            {
                var amenitiesFinalAdjustments = await connection.QueryAsync<AmenitiesFinalAdjustmentViewModel>(query, new { monthId, bankId });
                return amenitiesFinalAdjustments.ToList();
            }
        }
        public async Task<AmenitiesJournalMaster> GetAmenitiesJournalMaster(int monthId, int employeeTypeId)
        {
            var query = "SELECT * FROM AmenitiesJournalMaster where MonthId=@MonthId and EmployeeTypeId=@EmployeeTypeId";
            var parameters = new DynamicParameters();
            parameters.Add("MonthId", monthId, DbType.Int32);
            parameters.Add("EmployeeTypeId", employeeTypeId, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                var journalMaster = await connection.QuerySingleOrDefaultAsync<AmenitiesJournalMaster>(query, parameters);
                return journalMaster;
            }
        }
        public async Task<List<AmenitiesJournal>> GetAmenitiesJournalsByMasterId(int journalMasterId)
        {
            var query = "SELECT * FROM AmenitiesJournals where JournalMasterId=@JournalMasterId";
            var parameters = new DynamicParameters();
            parameters.Add("JournalMasterId", journalMasterId, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                var amenitiesJournals = await connection.QueryAsync<AmenitiesJournal>(query, parameters);
                return amenitiesJournals.ToList();
            }
        }

        public async Task<int> UpdateAmenitiesFinalAdjustment(AmenitiesFinalAdjustmentViewModel amenitiesFinalAdjustmentViewModel)
        {
            int result = 0;
            var query = "update AmenitiesReport set WageGMSCB=@WageGMSCB,HouseKeepUp=@HouseKeepUp,FuelSubsidy=@FuelSubsidy,WESubsidy=@WESubsidy,OtherDeduction=@OtherDeduction,OtherPay=@OtherPay,RevenueStamp=@RevenueStamp where AmenitiesReportId=@AmenitiesReportId";
            var parameters = new DynamicParameters();

            parameters.Add("WageGMSCB", amenitiesFinalAdjustmentViewModel.WageGMSCB, DbType.Double);
            parameters.Add("HouseKeepUp", amenitiesFinalAdjustmentViewModel.HouseKeepUp, DbType.Double);
            parameters.Add("FuelSubsidy", amenitiesFinalAdjustmentViewModel.FuelSubsidy, DbType.Double);
            parameters.Add("WESubsidy", amenitiesFinalAdjustmentViewModel.WESubsidy, DbType.Double);
            parameters.Add("OtherDeduction", amenitiesFinalAdjustmentViewModel.OtherDeduction, DbType.Double);
            parameters.Add("OtherPay", amenitiesFinalAdjustmentViewModel.OtherPay, DbType.Double);
            parameters.Add("RevenueStamp", amenitiesFinalAdjustmentViewModel.RevenueStamp, DbType.Double);
            parameters.Add("AmenitiesReportId", amenitiesFinalAdjustmentViewModel.AmenitiesReportId, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    result = await connection.ExecuteAsync(query, parameters);
                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }

        public async Task<int> CreateAmenitiesJournalMaster(AmenitiesJournalMaster amenitiesJournalMaster)
        {
            int result = 0;
            string query = "insert into AmenitiesJournalMaster (MonthId,EmployeeTypeId,CreatedBy,CreatedDate) values (@MonthId,@EmployeeTypeId,@CreatedBy,@CreatedDate) SELECT CAST(SCOPE_IDENTITY() AS INT);";
            var parameters = new DynamicParameters();
            parameters.Add("MonthId", amenitiesJournalMaster.MonthId, DbType.Int32);
            parameters.Add("EmployeeTypeId", amenitiesJournalMaster.EmployeeTypeId, DbType.Int32);
            parameters.Add("CreatedBy", amenitiesJournalMaster.CreatedBy, DbType.String);
            parameters.Add("CreatedDate", amenitiesJournalMaster.CreatedDate, DbType.DateTime);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    result = await connection.QuerySingleAsync<int>(query, parameters);
                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }

        public async Task<int> CreateAmenitiesJournal(AmenitiesJournal amenitiesJournal)
        {
            int result = 0;
            string query = "insert into AmenitiesJournals (AccountNumber,JournalCode,Details,Debit,Credit,JournalMasterId) values (@AccountNumber,@JournalCode,@Details,@Debit,@Credit,@JournalMasterId)";
            var parameters = new DynamicParameters();
            parameters.Add("AccountNumber", amenitiesJournal.AccountNumber, DbType.String);
            parameters.Add("JournalCode", amenitiesJournal.JournalCode, DbType.String);
            parameters.Add("Details", amenitiesJournal.Details, DbType.String);
            parameters.Add("Debit", amenitiesJournal.Debit, DbType.Decimal);
            parameters.Add("Credit", amenitiesJournal.Credit, DbType.Decimal);
            parameters.Add("JournalMasterId", amenitiesJournal.JournalMasterId, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    result = await connection.ExecuteAsync(query, parameters);
                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }

        #region reports
        public async Task<List<AmenitiesControlSheetViewModel>> GetAmenitiesControlSheet(List<string> jobCodes,int monthId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("MonthID", monthId, DbType.Int32);
            var query = $"select * from AmenitiesReport where MonthId=@monthId";
            // Add the job codes filter if specified
            if (jobCodes != null && jobCodes.Count > 0)
            {
                query += " AND JobCode IN @JobCodes";
                parameters.Add("JobCodes", jobCodes);
            }
            using (var connection = _context.CreateConnection())
            {
                var amenitiesControlSheets = await connection.QueryAsync<AmenitiesControlSheetViewModel>(query, parameters);
                return amenitiesControlSheets.ToList();
            }
        }

        //public async Task<List<AmenitiesControlSheetViewModel>> YearlyAmenitiesControlSheet(List<string> jobCodes, int fromMonthId, int toMonthId, string? department)
        //{
        //    var parameters = new DynamicParameters();
        //    parameters.Add("fMonthID", fromMonthId, DbType.Int32);
        //    parameters.Add("tMonthID", toMonthId, DbType.Int32);

        //    var query = @"SELECT 
        //            JobCode
        //            ,EmployeeName
        //            ,AccountNumber
        //            ,BankId
        //            ,BankName
        //            ,BankBranchName
        //            ,SUM(WageGMSCB) AS WageGMSCB
        //            ,SUM(HouseKeepUp) AS HouseKeepUp
        //            ,SUM(FuelSubsidy) AS FuelSubsidy
        //            ,SUM(WESubsidy) AS WESubsidy
        //            ,SUM(OtherDeduction) AS OtherDeduction
        //            ,SUM(OtherPay) AS OtherPay
        //            ,SUM(RevenueStamp) AS RevenueStamp
        //            ,JournalCode
        //         FROM AmenitiesReport
        //         WHERE MonthID BETWEEN @fMonthID AND @tMonthID";

        //    // Add the job codes filter if specified
        //    if (jobCodes != null && jobCodes.Count > 0)
        //    {
        //        query += " AND JobCode IN @JobCodes";
        //        parameters.Add("JobCodes", jobCodes);
        //    }

        //    // Add the department filter if specified
        //    if (!String.IsNullOrEmpty(department))
        //    {
        //        query += " AND DepartmentName = @DepartmentName";
        //        parameters.Add("DepartmentName", department, DbType.String);
        //    }

        //    // Grouping and aggregation
        //    query += @" GROUP BY JobCode, EmployeeName,
        //                    AccountNumber, BankId, BankName, BankBranchName, JournalCode";

        //    using (var connection = _context.CreateConnection())
        //    {
        //        try
        //        {
        //            var amenitiesControlSheets = await connection.QueryAsync<AmenitiesControlSheetViewModel>(query, parameters);
        //            return amenitiesControlSheets.ToList();
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.Error.WriteLine($"An error occurred: {ex.Message}");
        //            // Handle or log the exception as needed
        //            return new List<AmenitiesControlSheetViewModel>(); // Return an empty list on error
        //        }
        //    }
        //}

        public async Task<List<AmenitiesControlSheetViewModel>> YearlyAmenitiesControlSheet(List<string> jobCodes, int fromMonthId, int toMonthId, string? department)
        {
            var parameters = new DynamicParameters();
            parameters.Add("fMonthID", fromMonthId, DbType.Int32);
            parameters.Add("tMonthID", toMonthId, DbType.Int32);

            var query = @"
                    -- Summarize values for each JobCode across all months
                    WITH SummarizedAmenities AS (
                        SELECT 
                            JobCode,
                            EmployeeName,
                            SUM(WageGMSCB) AS WageGMSCB,
                            SUM(HouseKeepUp) AS HouseKeepUp,
                            SUM(FuelSubsidy) AS FuelSubsidy,
                            SUM(WESubsidy) AS WESubsidy,
                            SUM(OtherDeduction) AS OtherDeduction,
                            SUM(OtherPay) AS OtherPay,
                            SUM(RevenueStamp) AS RevenueStamp,
                            MAX(MonthID) AS MaxMonthID -- Get the latest month ID for selecting additional details
                        FROM AmenitiesReport
                        WHERE MonthID BETWEEN @fMonthID AND @tMonthID
                ";

                // Add job codes filter if specified
                if (jobCodes != null && jobCodes.Count > 0)
                {
                    query += " AND JobCode IN @JobCodes";
                    parameters.Add("JobCodes", jobCodes);
                }

                // Add department filter if specified
                if (!String.IsNullOrEmpty(department))
                {
                    query += " AND DepartmentName = @DepartmentName";
                    parameters.Add("DepartmentName", department, DbType.String);
                }

            // Group by JobCode and EmployeeName to avoid duplicates
            query += @"
            GROUP BY JobCode, EmployeeName
        ),
                -- Retrieve the latest details (AccountNumber, Bank details, Department, Designation) for each JobCode
                LatestDetails AS (
                    SELECT 
                        JobCode,
                        AccountNumber,
                        BankId,
                        BankName,
                        BankBranchName,
                        JournalCode,
                        DepartmentName,
                        DesignationName,
                        ROW_NUMBER() OVER (PARTITION BY JobCode ORDER BY MonthID DESC) AS rn
                    FROM AmenitiesReport
                    WHERE MonthID BETWEEN @fMonthID AND @tMonthID
                )
                    -- Select the summarized data and join it with the latest details
                    SELECT 
                        s.JobCode,
                        s.EmployeeName,
                        l.AccountNumber,
                        l.BankId,
                        l.BankName,
                        l.BankBranchName,
                        s.WageGMSCB,
                        s.HouseKeepUp,
                        s.FuelSubsidy,
                        s.WESubsidy,
                        s.OtherDeduction,
                        s.OtherPay,
                        s.RevenueStamp,
                        l.JournalCode,
                        l.DepartmentName,
                        l.DesignationName
                    FROM SummarizedAmenities s
                    JOIN LatestDetails l
                        ON s.JobCode = l.JobCode
                    WHERE l.rn = 1; -- Ensure we take only the latest record per JobCode
                ";

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var amenitiesControlSheets = await connection.QueryAsync<AmenitiesControlSheetViewModel>(query, parameters);
                    return amenitiesControlSheets.ToList();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"An error occurred: {ex.Message}");
                    // Handle or log the exception as needed
                    return new List<AmenitiesControlSheetViewModel>(); // Return an empty list on error
                }
            }
        }




        public async Task<List<AmenitiesControlSheetViewModel>> YearlyAmenitiesPaySlip(string jobCode, int fromMonthId, int toMonthId, string? department)
        {
            var parameters = new DynamicParameters();
            parameters.Add("fMonthID", fromMonthId, DbType.Int32);
            parameters.Add("tMonthID", toMonthId, DbType.Int32);
            parameters.Add("JobCode", jobCode, DbType.String);

            var query = @"
                    SELECT * 
                    FROM AmenitiesReport 
                    WHERE JobCode = @JobCode 
                      AND MonthID BETWEEN @fMonthID AND @tMonthID";

            if (!string.IsNullOrWhiteSpace(department))
            {
                query += " AND DepartmentName = @DepartmentName";
                parameters.Add("DepartmentName", department.Trim(), DbType.String);
            }

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var amenitiesControlSheets = await connection.QueryAsync<AmenitiesControlSheetViewModel>(query, parameters);
                    return amenitiesControlSheets.ToList();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"An error occurred: {ex.Message}");
                    // Consider using a logging framework here
                    return new List<AmenitiesControlSheetViewModel>(); // Return an empty list on error
                }
            }
        }



        public async Task<List<AmenitiesControlSheetViewModel>> GeAmenitiesPaySlip(string jobCode, int monthId, int? departmentId)
        {
            IEnumerable<AmenitiesControlSheetViewModel> AmwnitiespaySlip = null;
            var parameters = new DynamicParameters();
            parameters.Add("MonthID", monthId, DbType.Int32);
            parameters.Add("JobCode", jobCode, DbType.String);

            var query = $"select * from AmenitiesReport where MonthId=@MonthID AND JobCode=@JobCode";

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
                    AmwnitiespaySlip = await connection.QueryAsync<AmenitiesControlSheetViewModel>(query, parameters);
                }
                catch (Exception ex)
                {
                    // Log or handle exception appropriately
                    throw new Exception("Error executing SQL query", ex);
                }
            }

            return AmwnitiespaySlip?.ToList() ?? new List<AmenitiesControlSheetViewModel>(); // Return an empty list if null
        }

        #endregion reports

        public async Task<string> GetLastMonthAmenitiesProcess()
        {
            var query = "SELECT TOP 1 MonthID FROM AmenitiesReportMaster ORDER BY Id DESC;";

            using (var connection = _context.CreateConnection())
            {
                var lastMonthID = await connection.QueryFirstOrDefaultAsync<int?>(query);

                if (lastMonthID.HasValue)
                {
                    var year = lastMonthID.Value / 100;
                    var monthNumber = lastMonthID.Value % 100;
                    var date = new DateTime(year, monthNumber, 1);
                    var formattedDate = date.ToString("MMM") + " '" + date.ToString("yy");
                    return formattedDate;
                }
                return "No Data";
            }
        }
        public async Task<List<JournalAdjustmentOfficer>> JournalAdjustment(int monthId)
        {
            IEnumerable<JournalAdjustmentOfficer> finalAdjustments = null;
            var parameters = new DynamicParameters();
            parameters.Add("MonthID", monthId, DbType.Int32);
            var query = "SELECT j.Id, j.JournalMasterId, jm.MonthId, jm.EmployeeTypeId,j.JournalCode,j.AccountNumber, j.Details, j.Debit, j.Credit FROM AmenitiesJournals AS j JOIN AmenitiesJournalMaster AS jM ON jM.Id = j.JournalMasterId WHERE jm.MonthId = @MonthId";
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    finalAdjustments = await connection.QueryAsync<JournalAdjustmentOfficer>(query, new { monthId});
                }
                catch (Exception ex)
                {

                }
            }
            return finalAdjustments.ToList();
        }
        public async Task<int> UpdateJournalAdjustment(JournalAdjustmentOfficer amenitiesJournal)
        {
            string query = $"UPDATE AmenitiesJournals SET " +
                "AccountNumber = @AccountNumber, " +
                "JournalCode = @JournalCode, " +
                "Details = @Details, " +
                "Debit = @Debit, " +
                "Credit = @Credit " + // Remove the comma and add a WHERE clause
                "WHERE Id = @Id"; // Assume Id or another unique column

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("AccountNumber", amenitiesJournal.AccountNumber, DbType.String);
                    parameters.Add("JournalCode", amenitiesJournal.JournalCode, DbType.String);
                    parameters.Add("Details", amenitiesJournal.Details, DbType.String);
                    parameters.Add("Debit", amenitiesJournal.Debit, DbType.Double);
                    parameters.Add("Credit", amenitiesJournal.Credit, DbType.Double);
                    parameters.Add("Id", amenitiesJournal.Id, DbType.Int32); // Add the Id parameter

                    var result = await connection.ExecuteScalarAsync<int>(query, parameters);
                    return result;
                }
                catch (Exception ex)
                {

                    return 0;
                }
            }
        }
        public async Task<JournalAdjustmentOfficer> InsertNewamenitiesJournal(JournalAdjustmentOfficer AmenitiesJournals)
        {
            string insertQuery = @"
                                INSERT INTO AmenitiesJournals (AccountNumber, JournalCode, Details, Debit, Credit, JournalMasterId)
                                VALUES (@AccountNumber, @JournalCode, @Details, @Debit, @Credit, @JournalMasterId);
                                SELECT CAST(SCOPE_IDENTITY() AS INT);"; // Get the generated ID

            string selectQuery = @"
                                SELECT *
                                FROM SalaryJournals
                                WHERE Id = @Id";

            int newId = 0;

            var parameters = new DynamicParameters();
            parameters.Add("AccountNumber", AmenitiesJournals.AccountNumber, DbType.String);
            parameters.Add("JournalCode", AmenitiesJournals.JournalCode, DbType.String);
            parameters.Add("Details", AmenitiesJournals.Details, DbType.String);

            // Check if Debit and Credit are 0, and set them to null if they are
            parameters.Add("Debit", AmenitiesJournals.Debit != 0 ? AmenitiesJournals.Debit : (decimal?)null, DbType.Decimal);
            parameters.Add("Credit", AmenitiesJournals.Credit != 0 ? AmenitiesJournals.Credit : (decimal?)null, DbType.Decimal);

            parameters.Add("JournalMasterId", AmenitiesJournals.JournalMasterId, DbType.Int32);

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    // Execute the insert query and get the new ID
                    newId = await connection.ExecuteScalarAsync<int>(insertQuery, parameters);

                    // Fetch the newly inserted record using the new ID
                    var selectParameters = new DynamicParameters();
                    selectParameters.Add("Id", newId, DbType.Int32);

                    var newRecord = await connection.QuerySingleOrDefaultAsync<JournalAdjustmentOfficer>(selectQuery, selectParameters);

                    return newRecord;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occurred: {ex.Message}");
                    throw;
                }
            }
        }
        public async Task<int> DeleteAmenitiesJournal(int id)
        {
            string query = "DELETE FROM AmenitiesJournals WHERE Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("Id", id, DbType.Int32);

                    var result = await connection.ExecuteAsync(query, parameters);
                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occurred: {ex.Message}");
                    return 0;
                }
            }
        }


        public async Task<decimal> GetLastMonthAmenitiesAmount()
        {
            var query = @"
                SELECT 
                    SUM(
                        ([WageGMSCB] +
                        [HouseKeepUp] +
                        [FuelSubsidy] +
                        [WESubsidy] +
                        [OtherPay]) 
                        - 
                        ([RevenueStamp] + 
                        [OtherDeduction])
                    ) AS NetAmount
                FROM 
                    [AmenitiesReport]
                WHERE 
                    MonthId = (SELECT TOP 1 MonthId FROM AmenitiesReport ORDER BY AmenitiesReportId DESC);
                ";

            using (var connection = _context.CreateConnection())
            {
                var netamount = await connection.QueryFirstOrDefaultAsync<decimal?>(query);
                return netamount ?? 0m;
            }
        }

    }
}
