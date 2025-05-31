using Contracts;
using Dapper;
using Entities;
using Entities.ViewModels;
using Entities.ViewModels.Reports;
using System.Data;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Repositories
{
    public class BonusRepository : IBonusContract
    {
        private readonly BgfclContext _context;

        public BonusRepository(BgfclContext context)
        {
            _context = context;
        }

        public async Task<int> CreateBonus(Bonus bonus)
        {
            int result = 0;
            var query = "INSERT INTO Bonus (UserId,SchoolId,RoleId,GuestPkId,BonusTitle,PayableMonth,StatusOF,StatusJS,CreatedBy,CreatedDate,IsActive,IsFestival,IsIncentive,IsHonorarium,IsScholarship)" +
                " VALUES (@userId,@schoolId,@roleId,@guestPkId,@bonusTitle,@payableMonth,@statusOF,@statusJS,@createdBy,@createdDate,@isActive,@isFestival,@isIncentive,@isHonorarium,@isScholarship)";
            var parameters = new DynamicParameters();
            parameters.Add("schoolId", bonus.SchoolId, DbType.Int32);
            parameters.Add("guestPkId", bonus.GuestPkId, DbType.Int32);
            parameters.Add("roleId", bonus.RoleId, DbType.Int32);
            parameters.Add("userId", bonus.UserId, DbType.Int32);
            parameters.Add("bonusTitle", bonus.BonusTitle, DbType.String);
            parameters.Add("payableMonth", bonus.PayableMonth, DbType.Int32);
            parameters.Add("statusOF", bonus.StatusOF, DbType.Int32);
            parameters.Add("statusJS", bonus.StatusJS, DbType.Int32);
            parameters.Add("createdBy", bonus.CreatedBy, DbType.String);
            parameters.Add("createdDate", bonus.CreatedDate, DbType.DateTime);
            parameters.Add("isActive", bonus.IsActive, DbType.Boolean);
            parameters.Add("isFestival", bonus.IsFestival, DbType.Boolean);
            parameters.Add("isIncentive", bonus.IsIncentive, DbType.Boolean);
            parameters.Add("isHonorarium", bonus.IsHonorarium, DbType.Boolean);
            parameters.Add("isScholarship", bonus.IsScholarship, DbType.Boolean);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    result = await connection.ExecuteScalarAsync<int>(query, parameters);
                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }

        public async Task<List<Bonus>> GetBonus()
        {
            var query = "select * from bonus where isactive in(1)";
            using (var connection = _context.CreateConnection())
            {
                var bonus = await connection.QueryAsync<Bonus>(query);
                return bonus.ToList();
            }
        }
        public async Task<List<BonusControlSheet>> GetBonusSheet()
        {
            var query = "select * from BonusSheetId where isactive in(1)";
            using (var connection = _context.CreateConnection())
            {
                var bonus = await connection.QueryAsync<BonusControlSheet>(query);
                return bonus.ToList();
            }
        }

        public async Task<List<Bonus>> GetLastBonusAmount()
        {
            var query = @"
                        SELECT
                            BonusId,
                            SUM(ISNULL(FestivalBonus, 0) +
                                ISNULL(IncentiveBonus, 0) +
                                ISNULL(HonorariumBonus, 0) +
                                ISNULL(ScholarshipBonus, 0)) AS TotalBonusAmount
                        FROM
                            BonusSheet
                        WHERE
                            BonusId = (
                                SELECT MAX(BonusId) FROM BonusSheet
                            )
                        GROUP BY
                            BonusId;
                    ";

            using (var connection = _context.CreateConnection())
            {
                var bonus = await connection.QueryAsync<Bonus>(query);
                return bonus.ToList();
            }
        }
        public async Task<Bonus> GetLastBonusName()
        {
            var query = @"
                        SELECT
                            BonusTitle
                        FROM
                            Bonus
                        WHERE
                            Id = (
                                SELECT MAX(Id) FROM Bonus
                            );
                    ";

            using (var connection = _context.CreateConnection())
            {
                var bonus = await connection.QuerySingleOrDefaultAsync<Bonus>(query);
                return bonus;
            }
        }
        public async Task<IEnumerable<BonusSheetViewModal>> GetPayableBonusList()
        {
            var query = @"
                        SELECT 
                        Id,
                        DATENAME(MONTH, DATEFROMPARTS(PayableMonth / 100, PayableMonth % 100, 1)) AS Month,
                        PayableMonth / 100 AS Year,
                        BonusTitle,
                        CASE 
                            WHEN StatusOF = 1 OR StatusJS = 1 THEN 'Processed'
                            ELSE 'Processed' 
                        END AS Status
                    FROM 
                        Bonus
                        ORDER BY 
                        Id DESC; 
                        ";

            using (var connection = _context.CreateConnection())
            {
                var bonusProcesses = await connection.QueryAsync<BonusSheetViewModal>(query);
                return bonusProcesses.ToList();
            }
        }


        public async Task<int> UpdateBonus(Bonus bonus)
        {
            var query = "update Bonus set UserId=@userId,SchoolId=@schoolId,RoleId=@roleId,GuestPkId=@guestPkId, BonusTitle = @bonusTitle,PayableMonth=@payableMonth,IsActive=@isActive,updatedby = @updatedby, updateddate = @updateddate where id = @id\r\n";
            var parameters = new DynamicParameters();
            parameters.Add("schoolId", bonus.SchoolId, DbType.Int32);
            parameters.Add("guestPkId", bonus.GuestPkId, DbType.Int32);
            parameters.Add("roleId", bonus.RoleId, DbType.Int32);
            parameters.Add("userId", bonus.UserId, DbType.Int32);
            parameters.Add("bonusTitle", bonus.BonusTitle, DbType.String);
            parameters.Add("payableMonth", bonus.PayableMonth, DbType.Int32);
            parameters.Add("updatedby", bonus.UpdatedBy, DbType.String);
            parameters.Add("updateddate", bonus.UpdatedDate, DbType.DateTime);
            parameters.Add("isActive", bonus.IsActive, DbType.Boolean);
            parameters.Add("id", bonus.Id, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteScalarAsync<int>(query, parameters);
                return result;
            }
        }

        public async Task<int> RemoveBonus(int id)
        {
            var query = "delete bonus  where id = @id";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteScalarAsync<int>(query, new { id });
                return result;
            }
        }

        public async Task<Bonus> GetBonusById(int id)
        {
            var query = "select * from bonus where id = @id";
            using (var connection = _context.CreateConnection())
            {
                var bonus = await connection.QueryFirstOrDefaultAsync<Bonus>(query, new { id });
                return bonus;
            }
        }

        public async Task<List<Bonus>> GetBonusByMonthId(int monthId)
        {
            List<Bonus> bonus = new List<Bonus>();
            try
            {
                var query = "select * from bonus where PayableMonth = @monthId";
                using (var connection = _context.CreateConnection())
                {
                    var bonusData = await connection.QueryAsync<Bonus>(query, new { monthId });
                    bonus.AddRange(bonusData.ToList());
                }
            }
            catch (Exception ex)
            {

            }
            return bonus;
        }

        public async Task<int> BonusSetting(int bonusId, int employeeTypeId)
        {
            int result = 0;
            var parameters = new DynamicParameters();
            parameters.Add("BonusId", bonusId, DbType.Int32);
            parameters.Add("EmpType", employeeTypeId, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    result = await connection.ExecuteAsync("spBonusSetting", parameters, commandType: CommandType.StoredProcedure);

                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }

        public async Task<int> BonusProcess(int bonusId, int employeeTypeId, int fieldIndex, int religion, double basic)
        {
            int result = 0;
            var parameters = new DynamicParameters();
            parameters.Add("EmpType", employeeTypeId, DbType.Int32);
            parameters.Add("BonusId", bonusId, DbType.Int32);
            parameters.Add("FieldIndex", fieldIndex, DbType.Int32);
            parameters.Add("BasicPercent", basic, DbType.Double);
            parameters.Add("Religion", religion, DbType.Int32);

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    result = await connection.ExecuteAsync("spBonusProcess", parameters, commandType: CommandType.StoredProcedure);

                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }

        public async Task<List<BonusAdjustmentViewModel>> GetBonusProcessData(int bonusId, int employeeTypeId)
        {

            string query = "SELECT * FROM BonusSheet WHERE bonusid = @bonusId AND EmpType IN @empTypes";
            if (employeeTypeId == 1)
            {
                query = "select * from BonusSheet where bonusid=@bonusId and EmpType in (1,3)";
            }
            else
            {
                query = "select * from BonusSheet where bonusid=@bonusId and EmpType in (2,4)";
            }

            using (var connection = _context.CreateConnection())
            {
                var bonus = await connection.QueryAsync<BonusAdjustmentViewModel>(query, new { bonusId = bonusId, employeeTypeId = employeeTypeId });
                return bonus.ToList();
            }
        }
        public async Task<List<BonusAdjustmentViewModel>> GetAllBonusProcessData()
        {
            // Default query for a specific employee type
            string query = "SELECT * FROM BonusSheet";

            using (var connection = _context.CreateConnection())
            {
                // Execute the query and retrieve the bonus data
                var bonus = await connection.QueryAsync<BonusAdjustmentViewModel>(query);
                return bonus.ToList();

            }
        }
        public async Task<List<BonusAdjustmentViewModel>> GetBonusProcessDataWithFilter(int employeeTypeId, int? schoolId, int? roleId, string? department, string? designation)
        {
            string query = @"
                    SELECT BonusSheetId, JobCode, EmployeeName, HonorariumBonus, Deduction, RevStamp, 
                           DesignationName, DepartmentName, FestivalBonus, IncentiveBonus
                    FROM BonusSheet 
                    WHERE EmpType = @EmployeeTypeId";

            if (schoolId.HasValue)
                query += " AND SchoolId = @SchoolId";
            if (roleId.HasValue)
                query += " AND RoleId = @RoleId";
            if (!string.IsNullOrEmpty(department))
                query += " AND DepartmentName = @Department";
            if (!string.IsNullOrEmpty(designation))
                query += " AND DesignationName = @Designation";

            query += " ORDER BY TRY_CAST(JobCode AS INT) ASC;";

            using (var connection = _context.CreateConnection())
            {
                var bonusData = await connection.QueryAsync<BonusAdjustmentViewModel>(query, new
                {
                    EmployeeTypeId = employeeTypeId,
                    SchoolId = schoolId,
                    RoleId = roleId,
                    Department = department,
                    Designation = designation
                });

                return bonusData.ToList();
            }
        }


        public async Task<List<BonusAdjustmentViewModel>> CheckBonusJournalData(int bonusId, int employeeTypeId)
        {
            string query = "select * from BonusJournals b inner join BonusJournalMaster m on b.JournalMasterId=m.id where m.EmployeeTypeId=@employeeTypeId  and BonusId=@bonusId";



            using (var connection = _context.CreateConnection())
            {
                var bonus = await connection.QueryAsync<BonusAdjustmentViewModel>(query, new { bonusId = bonusId, employeeTypeId = employeeTypeId });
                return bonus.ToList();
            }
        }
        public async Task<int> UpdateBonusSheet(BonusAdjustmentViewModel bonusAdjustmentViewModel)
        {
            var query = "update BonusSheet set FestivalBonus=@FestivalBonus, IncentiveBonus=@IncentiveBonus, HonorariumBonus=@HonorariumBonus, ScholarshipBonus=@ScholarshipBonus, Deduction=@Deduction, RevStamp=@RevStamp  where BonusSheetId = @BonusSheetId";
            var parameters = new DynamicParameters();
            parameters.Add("FestivalBonus", bonusAdjustmentViewModel.FestivalBonus, DbType.Double);
            parameters.Add("IncentiveBonus", bonusAdjustmentViewModel.IncentiveBonus, DbType.Double);
            parameters.Add("HonorariumBonus", bonusAdjustmentViewModel.HonorariumBonus, DbType.Double);
            parameters.Add("ScholarshipBonus", bonusAdjustmentViewModel.ScholarshipBonus, DbType.Double);
            parameters.Add("Deduction", bonusAdjustmentViewModel.Deduction, DbType.Double);
            parameters.Add("RevStamp", bonusAdjustmentViewModel.RevStamp, DbType.Double);
            parameters.Add("BonusSheetId", bonusAdjustmentViewModel.BonusSheetId, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameters);
                return result;
            }
        }

        public async Task<BonusJournalMaster> GetBonusJournalMaster(int monthId, int employeeTypeId, int bonusId)
        {
            var query = "SELECT * FROM BonusJournalMaster where MonthId=@MonthId and EmployeeTypeId=@EmployeeTypeId and BonusId=@BonusId";
            var parameters = new DynamicParameters();
            parameters.Add("MonthId", monthId, DbType.Int32);
            parameters.Add("EmployeeTypeId", employeeTypeId, DbType.Int32);
            parameters.Add("BonusId", bonusId, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                var journalMaster = await connection.QuerySingleOrDefaultAsync<BonusJournalMaster>(query, parameters);
                return journalMaster;
            }
        }
        public async Task<int> CreateBonusJournalMaster(BonusJournalMaster bonusJournalMaster)
        {
            int result = 0;
            string query = "insert into BonusJournalMaster (MonthId,EmployeeTypeId,BonusId,CreatedBy,CreatedDate) values (@MonthId,@EmployeeTypeId,@BonusId,@CreatedBy,@CreatedDate);SELECT CAST(SCOPE_IDENTITY() AS INT)";
            var parameters = new DynamicParameters();
            parameters.Add("MonthId", bonusJournalMaster.MonthId, DbType.Int32);
            parameters.Add("EmployeeTypeId", bonusJournalMaster.EmployeeTypeId, DbType.Int32);
            parameters.Add("BonusId", bonusJournalMaster.BonusId, DbType.Int32);
            parameters.Add("CreatedBy", bonusJournalMaster.CreatedBy, DbType.String);
            parameters.Add("CreatedDate", bonusJournalMaster.CreatedDate, DbType.DateTime);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    result = connection.QuerySingle<int>(query, parameters);
                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }


        public async Task<int> CreateBonusJournal(BonusJournal bonusJournal)
        {
            int result = 0;
            string query = "insert into BonusJournals (AccountNumber,JournalCode,Details,Debit,Credit,JournalMasterId) values (@AccountNumber,@JournalCode,@Details,@Debit,@Credit,@JournalMasterId)";
            var parameters = new DynamicParameters();
            parameters.Add("AccountNumber", bonusJournal.AccountNumber, DbType.String);
            parameters.Add("JournalCode", bonusJournal.JournalCode, DbType.String);
            parameters.Add("Details", bonusJournal.Details, DbType.String);
            parameters.Add("Debit", bonusJournal.Debit, DbType.Decimal);
            parameters.Add("Credit", bonusJournal.Credit, DbType.Decimal);
            parameters.Add("JournalMasterId", bonusJournal.JournalMasterId, DbType.Int32);
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

        public async Task<List<BonusJournal>> GetBonusJournalsByMasterId(int journalMasterId)
        {
            var query = "SELECT * FROM BonusJournals where JournalMasterId=@JournalMasterId";
            var parameters = new DynamicParameters();
            parameters.Add("JournalMasterId", journalMasterId, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                var bonusJournals = await connection.QueryAsync<BonusJournal>(query, parameters);
                return bonusJournals.ToList();
            }
        }

        public async Task<string> GetLastBonus()
        {
            var query = "SELECT PayableMonth FROM Bonus WHERE PayableMonth = (SELECT TOP 1 PayableMonth FROM Bonus ORDER BY id DESC);";

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
        public async Task<decimal> GetLastBonusAmountAsync()
        {
            var query = @"
                SELECT 
                    SUM(FestivalBonus + IncentiveBonus + HonorariumBonus + ScholarshipBonus) AS TotalBonus 
                FROM 
                    Bonus 
                INNER JOIN  
                    BonusSheet ON Bonus.id = BonusSheet.BonusId 
                WHERE     
                    Bonus.PayableMonth = (SELECT TOP 1 PayableMonth FROM Bonus ORDER BY id DESC);
                ";

            using (var connection = _context.CreateConnection())
            {
                var bonusAmount = await connection.QueryFirstOrDefaultAsync<decimal?>(query);

                return bonusAmount ?? 0m;
            }
        }

        public async Task<int> DeleteBonus(int id)
        {
            var query = "delete from bonussheet where bonussheetid=@id";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, new { id });
                return result;
            }
        }
        public async Task<int> DeleteBonusSheet(int bonusId, int employeeTypeId)
        {
            var query = "delete from bonussheet where bonusid=@bonusId and EmpType=@employeeTypeId";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, new { bonusId, employeeTypeId });
                return result;
            }
        }

        public async Task<int> CreateBonusSheet(BonusControlSheet bonusControlSheet)
        {
            int result = 0;

            // First, calculate NetBonus manually
            var netBonus = (bonusControlSheet.FestivalBonus ?? 0)
                         + (bonusControlSheet.IncentiveBonus ?? 0)
                         + (bonusControlSheet.HonorariumBonus ?? 0)
                         + (bonusControlSheet.Scholarship ?? 0)
                         - ((bonusControlSheet.OtherDeduction ?? 0) + (bonusControlSheet.RevenueStamp ?? 0));

            string query = @"
                    INSERT INTO BonusSheet 
                        (SchoolId, GuestPkId, RoleId, UserId, BonusId, JobCode, EmployeeName, DesignationName, DepartmentName, 
                         AccountNumber, BankName, BankBranchName, FestivalBonus, IncentiveBonus, HonorariumBonus, ScholarshipBonus, 
                         Deduction, RevStamp, NetBonus, JournalCode, TIN, EmpType)
                    VALUES
                        (@schoolId, @guestPkId, @roleId, @userId, @BonusId, @JobCode, @EmployeeName, @DesignationName, @DepartmentName, 
                         @AccountNumber, @BankName, @BankBranchName, @FestivalBonus, @IncentiveBonus, @HonorariumBonus, @ScholarshipBonus, 
                         @Deduction, @RevStamp, @NetBonus, @JournalCode, @TIN, @EmpType);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var parameters = new DynamicParameters();
            parameters.Add("schoolId", bonusControlSheet.SchoolId, DbType.Int32);
            parameters.Add("guestPkId", bonusControlSheet.GuestPkId, DbType.Int32);
            parameters.Add("roleId", bonusControlSheet.RoleId, DbType.Int32);
            parameters.Add("userId", bonusControlSheet.UserId, DbType.Int32);
            parameters.Add("BonusId", bonusControlSheet.BonusId, DbType.Int32);
            parameters.Add("JobCode", bonusControlSheet.JobCode, DbType.String);
            parameters.Add("EmployeeName", bonusControlSheet.EmployeeName, DbType.String);
            parameters.Add("DesignationName", bonusControlSheet.DesignationName, DbType.String);
            parameters.Add("DepartmentName", bonusControlSheet.DepartmentName, DbType.String);
            parameters.Add("AccountNumber", bonusControlSheet.AccountNumber, DbType.String);
            parameters.Add("BankName", bonusControlSheet.BankName, DbType.String);
            parameters.Add("BankBranchName", bonusControlSheet.BankBranchName, DbType.String);
            parameters.Add("FestivalBonus", bonusControlSheet.FestivalBonus, DbType.Decimal);
            parameters.Add("IncentiveBonus", bonusControlSheet.IncentiveBonus, DbType.Decimal);
            parameters.Add("HonorariumBonus", bonusControlSheet.HonorariumBonus, DbType.Decimal);
            parameters.Add("ScholarshipBonus", bonusControlSheet.Scholarship, DbType.Decimal);
            parameters.Add("Deduction", bonusControlSheet.OtherDeduction, DbType.Decimal);
            parameters.Add("RevStamp", bonusControlSheet.RevenueStamp, DbType.Decimal);
            parameters.Add("NetBonus", netBonus, DbType.Decimal); // calculated NetBonus
            parameters.Add("JournalCode", bonusControlSheet.JournalCode, DbType.String);
            parameters.Add("TIN", bonusControlSheet.TIN, DbType.String);
            parameters.Add("EmpType", bonusControlSheet.EmployeeTypeId, DbType.Int32);

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    result = connection.QuerySingle<int>(query, parameters);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error creating bonus sheet", ex);
                }
            }
            return result;
        }

        public async Task<int> CreateBonusSheetData(BonusControlSheet dto)
        {
            int rowsInserted = 0;
            using var connection = _context.CreateConnection();
            connection.Open();
            using var tx = connection.BeginTransaction();


            try
            {
                // Step 1: Fetch employees
                const string fetchAllSql = @"
                    SELECT 
                        @MonthId AS MonthId,
                        e.Id AS EmployeeId,
                        e.EmployeeName,
                        e.EmployeeTypeId,
                        e.JobCode,
                        d.DepartmentName,
                        ds.DesignationName,
                        r.Title AS RoleTitle,
                        r.Id AS RoleId,
                        sc.Title AS SchoolTitle,
                        sc.Id AS SchoolId,
                        s.AccountNumber,
                        s.RevenueStamp,
                        ba.BankName,
                        b.BranchName,
                        s.BasicSalary
                    FROM Employees e
                    INNER JOIN Departments d ON e.DepartmentId = d.Id
                    INNER JOIN Designations ds ON e.DesignationId = ds.Id
                    INNER JOIN Roles r ON e.RoleId = r.Id
                    INNER JOIN Schools sc ON e.SchoolId = sc.Id
                    LEFT JOIN (
                        SELECT JobCode, AccountNumber, BankId, RevenueStamp, BasicSalary, 1 AS EmployeeTypeId
                        FROM SalarySettingsOfficer
                        UNION ALL
                        SELECT JobCode, AccountNumber, BankId, RevenueStamp, BasicSalary, 2 AS EmployeeTypeId
                        FROM SalarySettingsJuniorStaff
                        UNION ALL
                        SELECT JobCode, AccountNumber, BankId, RevenueStamp, BasicSalary, 3 AS EmployeeTypeId
                        FROM SalarySettingsJuniorStaff
                    ) AS s ON s.JobCode = e.JobCode AND s.EmployeeTypeId = e.EmployeeTypeId
                    LEFT JOIN Banks ba ON ba.Id = s.BankId
                    LEFT JOIN Branches b ON b.BankId = s.BankId
                    WHERE e.EmployeeTypeId = @EmpType;";

                var emps = (await connection.QueryAsync<EmpInfo>(fetchAllSql, new
                {
                    EmpType = dto.EmployeeTypeId,
                    MonthId = dto.MonthId
                }, tx)).ToList();

                if (!emps.Any())
                    throw new Exception("No employees found for the given EmployeeTypeId.");

                // Step 2: Delete based on fetched SchoolId and RoleId
                const string deleteSql = @"
                                        DELETE FROM BonusSheet
                                        WHERE BonusId = @BonusId AND SchoolId = @SchoolId AND RoleId = @RoleId AND MonthId = @MonthId AND EmpType=@EmployeeTypeId;";

                var uniqueCombos = emps
                    .Select(e => new { e.SchoolId, e.RoleId })
                    .Distinct();

                foreach (var combo in uniqueCombos)
                {
                    await connection.ExecuteAsync(deleteSql, new
                    {
                        BonusId = dto.BonusId,
                        SchoolId = combo.SchoolId,
                        RoleId = combo.RoleId,
                        MonthId = dto.MonthId,
                        EmployeeTypeId = dto.EmployeeTypeId
                    }, tx);
                }

                // Step 3: Insert bonus records
                const string insertSql = @"
                                INSERT INTO BonusSheet (
                                    MonthId, SchoolId, GuestPkId, RoleId, UserId, BonusId, JobCode, EmployeeName,
                                    DesignationName, DepartmentName, AccountNumber, BankName, BankBranchName,
                                    RevStamp, FestivalBonus, NetBonus, EmpType,BasicSalary
                                )
                                VALUES (
                                    @MonthId, @SchoolId, @GuestPkId, @RoleId, @UserId, @BonusId, @JobCode, @EmployeeName,
                                    @DesignationName, @DepartmentName, @AccountNumber, @BankName, @BankBranchName,
                                    @RevenueStamp, @FestivalBonus, @NetBonus, @EmpType,@BasicSalary
                                );";

                const string contractemployeeBonus = @"Select JobCode, Eidul_Fitar, Eidul_Ajha, Baishakhi_bonus
                                                            from SalarySettingsJuniorStaff
                                                            where Is_Daily_Worker = 0";

                var contractEmployeeBonusByJobCode = connection.Query<(string JobCode, decimal Eidul_Fitar, decimal Eidul_Ajha, decimal Baishakhi_bonus)>(contractemployeeBonus, transaction: tx)
                    .ToDictionary(
                        x => x.JobCode,
                        x => new
                        {
                            x.Eidul_Fitar,
                            x.Eidul_Ajha,
                            x.Baishakhi_bonus
                        });

                // BEFORE the foreach
                const string dailyworkerBonus = @"SELECT JobCode, Festival_bonus FROM SalarySettingsJuniorStaff WHERE Is_Daily_Worker = 1";
                Dictionary<string, decimal> dailyWorkerBonusByJobCode =
                    connection.Query<(string JobCode, decimal FestivalBonus)>(dailyworkerBonus, transaction: tx)
                              .ToDictionary(x => x.JobCode, x => x.FestivalBonus);
                
                

                // NOW do the foreach
                foreach (var e in emps)
                {
                    decimal basic = e.BasicSalary ?? 0;
                    decimal fest = 0;

                    if (e.EmployeeTypeId == 3) // Daily Worker
                    {
                        fest = dailyWorkerBonusByJobCode.TryGetValue(e.JobCode, out var dbBonus) ? dbBonus : 0; 
                    }
                    else if (e.EmployeeTypeId == 1 && dto.BonusId != 4) // Permanent
                    {
                        fest = (dto.FestivalBonus.GetValueOrDefault() / 100m) * basic;
                    }
                    else if (e.EmployeeTypeId == 1 && dto.BonusId == 4) // Permanent
                    {
                        fest = (dto.FestivalBonus.GetValueOrDefault());
                    }
                    else // Contract employee
                    {
                        if (contractEmployeeBonusByJobCode.TryGetValue(e.JobCode, out var bonus))
                        {
                            switch (dto.BonusId)
                            {
                                case 1:
                                    fest = bonus.Eidul_Fitar;
                                    break;
                                case 2:
                                    fest = bonus.Eidul_Ajha;
                                    break;
                                case 3:
                                    fest = bonus.Baishakhi_bonus;
                                    break;
                                default:
                                    fest = 0;
                                    break;
                            }
                        }
                    }

                    var p = new
                    {
                        e.MonthId,
                        e.SchoolId,
                        dto.GuestPkId,
                        e.RoleId,
                        dto.UserId,
                        dto.BonusId,
                        e.JobCode,
                        e.EmployeeName,
                        e.DesignationName,
                        e.DepartmentName,
                        e.AccountNumber,
                        e.BankName,
                        BankBranchName = e.BranchName,
                        RevenueStamp = e.RevenueStamp,
                        BasicSalary = e.BasicSalary,
                        FestivalBonus = fest,
                        NetBonus = fest - e.RevenueStamp,
                        EmpType = dto.EmployeeTypeId
                    };

                    rowsInserted += await connection.ExecuteAsync(insertSql, p, tx);
                }
                tx.Commit();
                return rowsInserted;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }



        // Supporting class
        private class EmpInfo
        {
            public int EmployeeId { get; set; }
            public int EmployeeTypeId { get; set; }
            public int MonthId { get; set; }
            public int SchoolId { get; set; }
            public int RoleId { get; set; }
            public string JobCode { get; set; }
            public string EmployeeName { get; set; }
            public decimal RevenueStamp { get; set; }
            public string DesignationName { get; set; }
            public string DepartmentName { get; set; }
            public decimal? BasicSalary { get; set; }
            public string AccountNumber { get; set; }
            public string BankName { get; set; }
            public string BranchName { get; set; }
        }





        #region report
        public async Task<List<BonusControlSheetViewModel>> GetBonusControlSheet(int monthId, int bonusId, int employeeTypeId, int? departmentId)
        {
            IEnumerable<BonusControlSheetViewModel> bonusControlSheet = null;
            var parameters = new DynamicParameters();
            parameters.Add("BonusId", bonusId, DbType.Int32);
            parameters.Add("EmployeeTypeId", employeeTypeId, DbType.Int32);

            // Start constructing the query
            var query = "SELECT * FROM BonusSheet WHERE BonusId = @BonusId AND EmpType =@EmployeeTypeId";

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
                    bonusControlSheet = await connection.QueryAsync<BonusControlSheetViewModel>(query, parameters);
                }
                catch (Exception ex)
                {

                    throw;
                }
            }

            return bonusControlSheet?.ToList() ?? new List<BonusControlSheetViewModel>();
        }


        public async Task<List<BonusControlSheetViewModel>> GetBonusBankForword(int monthId, int bonusId, int employeeTypeId, int? departmentId, string bankName)
        {
            IEnumerable<BonusControlSheetViewModel> bonusControlSheet = null;
            var parameters = new DynamicParameters();
            parameters.Add("BonusId", bonusId, DbType.Int32);
            parameters.Add("EmployeeTypeId", employeeTypeId, DbType.Int32);

            // Start constructing the query
            var query = "SELECT * FROM BonusSheet WHERE BonusId = @BonusId AND EmpType =@EmployeeTypeId";

            // Add the department filter only if departmentId is not null
            if (departmentId.HasValue && departmentId.Value != 0)
            {
                query += " AND DepartmentId = @DepartmentId";
                parameters.Add("DepartmentId", departmentId.Value, DbType.Int32);
            }
            if (!String.IsNullOrEmpty(bankName))
            {
                query += " AND BankName = @BankName";
                parameters.Add("BankName", bankName, DbType.String);
            }

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    bonusControlSheet = await connection.QueryAsync<BonusControlSheetViewModel>(query, parameters);
                }
                catch (Exception ex)
                {

                    throw;
                }
            }

            return bonusControlSheet?.ToList() ?? new List<BonusControlSheetViewModel>();
        }

        public async Task<List<BonusControlSheetViewModel>> GetBonusBankForwarding(int monthId, int bonusId, int employeeTypeId, int? BankId, int? departmentId)
        {
            IEnumerable<BonusControlSheetViewModel> bonusControlSheet = null;
            var parameters = new DynamicParameters();
            parameters.Add("BonusId", bonusId, DbType.Int32);
            parameters.Add("EmployeeTypeId", employeeTypeId, DbType.Int32);
            parameters.Add("MonthId", monthId, DbType.Int32);  // Add the missing MonthId parameter

            // Construct the query
            var query = "SELECT * FROM BonusSheet INNER JOIN Bonus ON Bonus.id = BonusSheet.BonusId " +
                        "WHERE  BonusId = @BonusId AND EmpType = @EmployeeTypeId";

            // Add the department filter only if departmentId is provided
            if (departmentId.HasValue && departmentId.Value != 0)
            {
                query += " AND DepartmentId = @DepartmentId";
                parameters.Add("DepartmentId", departmentId.Value, DbType.Int32);
            }

            // Add the bank filter only if BankId is provided
            if (BankId.HasValue && BankId.Value != 0)
            {
                query += " AND BankId = @BankId";
                parameters.Add("BankId", BankId.Value, DbType.Int32);
            }

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    bonusControlSheet = await connection.QueryAsync<BonusControlSheetViewModel>(query, parameters);
                }
                catch (Exception ex)
                {
                    // Log the exception
                    Console.WriteLine($"Error fetching bonus control sheet: {ex.Message}");
                    throw;
                }
            }

            return bonusControlSheet?.ToList() ?? new List<BonusControlSheetViewModel>();
        }


        public async Task<List<BonusControlSheetViewModel>> GetBonusPayslip(string jobCode, int monthId, int bonusId, int employeeTypeId, int? departmentId)
        {
            IEnumerable<BonusControlSheetViewModel> bonuspayslip = null;
            var parameters = new DynamicParameters();
            parameters.Add("BonusId", bonusId, DbType.Int32);
            parameters.Add("EmployeeTypeId", employeeTypeId, DbType.Int32);
            parameters.Add("JobCode", jobCode, DbType.String);
            parameters.Add("MonthId", monthId, DbType.Int32);

            var query = "select Bonus.PayableMonth as MonthId,JobCode,EmployeeName,DesignationName,DepartmentName,AccountNumber,BankName,BankBranchName," +
                "FestivalBonus,IncentiveBonus,HonorariumBonus,ScholarshipBonus,Deduction,RevStamp,JournalCode,TIN,EmpType from BonusSheet Inner Join Bonus on BonusSheet.BonusId= Bonus.Id WHERE JobCode = @JobCode AND BonusId = @BonusId AND EmpType =@EmployeeTypeId AND Bonus.PayableMonth=@MonthId";
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
                    bonuspayslip = await connection.QueryAsync<BonusControlSheetViewModel>(query, parameters);
                }
                catch (Exception ex)
                {

                    throw;
                }
            }

            return bonuspayslip?.ToList() ?? new List<BonusControlSheetViewModel>();
        }



        #endregion
    }
}
