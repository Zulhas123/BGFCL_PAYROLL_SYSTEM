using Contracts;
using Dapper;
using Entities;
using Entities.ViewModels;
using Entities.ViewModels.Reports;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Azure.Core.HttpHeader;

namespace Repositories
{
    public class LoanRepository:ILoanContract
    {
        private readonly BgfclContext _context;

        public LoanRepository(BgfclContext context)
        {
            _context = context;
        }
        public async Task<List<Loan>> GetLoans(string loanType)
        {
            var query = "SELECT * FROM Loans where LoanType = @loanType";
            using (var connection = _context.CreateConnection())
            {
                var employeeTypes = await connection.QueryAsync<Loan>(query, new {loanType});
                return employeeTypes.ToList();
            }
        }

        public async Task<List<LoanHeadViewModel>> GetLoanHeadByEmpType(int employeeType, string fieldType)
        {
            IEnumerable<LoanHeadViewModel> loanHeadViews = null;
            var parameters = new DynamicParameters();
            parameters.Add("EmployeeType", employeeType, DbType.Int32);
            parameters.Add("FieldType", fieldType, DbType.String);

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    loanHeadViews = await connection.QueryAsync<LoanHeadViewModel>("spGetAccountHeadByEmpType", parameters, commandType: CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {

                }
            }
            return loanHeadViews==null ? new List<LoanHeadViewModel>().ToList() : loanHeadViews.ToList();
        }
        public async Task<int> CreateMonthlyLoan(MonthlyLoan monthlyLoan)
        {
            int result = 0;
            var parameters = new DynamicParameters();
            parameters.Add("JobCode", monthlyLoan.JobCode, DbType.String);
            parameters.Add("MonthID", monthlyLoan.MonthId, DbType.Int32);
            parameters.Add("LoanName", monthlyLoan.LoanName, DbType.String);
            parameters.Add("Amount", monthlyLoan.Amount, DbType.Double);
            parameters.Add("EmployeeType", monthlyLoan.EmployeeType, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    result = await connection.ExecuteScalarAsync<int>("spInsertMonthlyLoan", parameters, commandType: CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }

        public async Task<int> CreateloanProcessHistory(LoanProcessHistory loanProcessHistory)
        {
            int result = 0;
            var parameters = new DynamicParameters();
            string query = $"insert into LoanProcessHistory (LoanId," +
                $"MonthId) values(@LoanId," +
                $"@MonthId)";

            parameters.Add("LoanId", loanProcessHistory.LoanId, DbType.Int32);
            parameters.Add("MonthId", loanProcessHistory.MonthId, DbType.Int32);

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
        public async Task<List<LoanProcessHistory>> GetLoanProcessHistory(int monthId, int loanId)
        {
            var query = "SELECT * from LoanProcessHistory where monthId = @monthId and loanId = @loanId";
            var parameters = new DynamicParameters();
            parameters.Add("monthId", monthId, DbType.Int32);
            parameters.Add("loanId", loanId, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                var histories = await connection.QueryAsync<LoanProcessHistory>(query, parameters);
                return histories.ToList();
            }
        }


        public async Task<int> CreateMcLoan(McLoan mcLoan)
        {
            int result = 0;
            var parameters = new DynamicParameters();
            string query = $"insert into MclLoans (JobCode," +
                $"TotalLoanAmount," +
                $"RemainingLoanAmount," +
                $"LoanTakenDate," +
                $"InstallmentNo," +
                $"RemainingInstallmentNo," +
                $"LoanTypeId," +
                $"InstallmentAmount," +
                $"IsPaused," +
                $"IsActive," +
                $"CreatedBy," +
                $"CreatedDate) values(@JobCode," +
                $"@TotalLoanAmount," +
                $"@RemainingLoanAmount," +
                $"@LoanTakenDate," +
                $"@InstallmentNo," +
                $"@RemainingInstallmentNo," +
                $"@LoanTypeId," +
                $"@InstallmentAmount," +
                $"@IsPaused," +
                $"@IsActive," +
                $"@CreatedBy," +
                $"@CreatedDate)";

            parameters.Add("JobCode", mcLoan.JobCode, DbType.String);
            parameters.Add("TotalLoanAmount", mcLoan.TotalLoanAmount, DbType.Double);
            parameters.Add("RemainingLoanAmount", mcLoan.RemainingLoanAmount, DbType.Double);
            parameters.Add("LoanTakenDate", mcLoan.LoanTakenDate, DbType.DateTime);
            parameters.Add("InstallmentNo", mcLoan.InstallmentNo, DbType.Int32);
            parameters.Add("RemainingInstallmentNo", mcLoan.RemainingInstallmentNo, DbType.Int32);
            parameters.Add("LoanTypeId", mcLoan.LoanTypeId, DbType.Int32);
            parameters.Add("InstallmentAmount", mcLoan.InstallmentAmount, DbType.Double);
            parameters.Add("IsPaused", mcLoan.IsPaused, DbType.Boolean);
            parameters.Add("IsActive", mcLoan.IsActive, DbType.Boolean);
            parameters.Add("CreatedBy", mcLoan.CreatedBy, DbType.String);
            parameters.Add("CreatedDate", mcLoan.CreatedDate, DbType.DateTime);

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
        public async Task<int> CreateMclInstallment(MclInstallment mclInstallment)
        {
            int result = 0;
            var parameters = new DynamicParameters();
            string query = $"insert into MclInstallments (JobCode," +
                $"InstallmentAmount," +
                $"MonthId," +
                $"MclId) values(@JobCode," +
                $"@InstallmentAmount," +
                $"@MonthId," +
                $"@MclId)";

            parameters.Add("JobCode", mclInstallment.JobCode, DbType.String);
            parameters.Add("InstallmentAmount", mclInstallment.InstallmentAmount, DbType.Double);
            parameters.Add("MonthId", mclInstallment.MonthId, DbType.Int32);
            parameters.Add("MclId", mclInstallment.MclId, DbType.Int32);

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

        public async Task<List<MclInstallment>> GetMcInstallments(int loanId)
        {
            var query = "select * from MclInstallments where MclId=@MclId order by Id";
            using (var connection = _context.CreateConnection())
            {
                var mclInstallments = await connection.QueryAsync<MclInstallment>(query, new { MclId = loanId });
                return mclInstallments.ToList();
            }
        }
        public async Task<List<MclInstallment>> GetMcInstallmentsByDate(int loanId,int fromMonthId, int toMonthId)
        {
            var query = "select * from MclInstallments where MclId=@MclId and (MonthId between @FromMonthId and @ToMonthId) order by Id";
            using (var connection = _context.CreateConnection())
            {
                var mclInstallments = await connection.QueryAsync<MclInstallment>(query, new { MclId = loanId, FromMonthId = fromMonthId, ToMonthId =toMonthId});
                return mclInstallments.ToList();
            }
        }

        public async Task<MclYearEnding> GetMclYearEnding(int loanId, int yearEndingMonthId)
        {
            var query = "select * from MCLYearEndings where LoanId=@LoanId and MonthId=@MonthId";
            using (var connection = _context.CreateConnection())
            {
                var mclYearEnding = await connection.QuerySingleOrDefaultAsync<MclYearEnding>(query, new { LoanId = loanId, MonthId = yearEndingMonthId });
                return mclYearEnding;
            }
        }

        public async Task<List<McLoanViewModel>> GetMcLoans(int isActive)
        {
            var query = "SELECT MclLoans.Id,MclLoans.JobCode,Employees.EmployeeName,MclLoans.TotalLoanAmount, MclLoans.RemainingLoanAmount,MclLoans.LoanTakenDate,MclLoans.InstallmentAmount,MclLoans.InstallmentNo,MclLoans.RemainingInstallmentNo FROM MclLoans join Employees on Employees.JobCode = MclLoans.JobCode where MclLoans.IsActive=@IsActive";
            var parameters = new DynamicParameters();
            parameters.Add("IsActive", isActive, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                var mcLoans = await connection.QueryAsync<McLoanViewModel>(query, parameters);
                return mcLoans.ToList();
            }
        }

        public async Task<McLoan> GetMcLoanById(int id)
        {
            var query = "SELECT * from MclLoans where Id=@id";
            var parameters = new DynamicParameters();
            parameters.Add("id", id, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                var mcLoan = await connection.QuerySingleOrDefaultAsync<McLoan>(query, parameters);
                return mcLoan;
            }
        }

        public async Task<int> UpdateMcLoan(McLoan mcLoan)
        {
            int result = 0;
            var parameters = new DynamicParameters();
            string query = $"update MclLoans set InstallmentAmount=@InstallmentAmount, IsPaused=@IsPaused, IsActive=@IsActive,UpdatedBy=@UpdatedBy,UpdatedDate=@UpdatedDate  where Id=@Id";
               
            parameters.Add("InstallmentAmount", mcLoan.InstallmentAmount, DbType.Double);
            parameters.Add("IsPaused", mcLoan.IsPaused, DbType.Boolean);
            parameters.Add("IsActive", mcLoan.IsActive, DbType.Boolean);
            parameters.Add("UpdatedBy", mcLoan.UpdatedBy, DbType.String);
            parameters.Add("UpdatedDate", mcLoan.UpdatedDate, DbType.DateTime);
            parameters.Add("Id", mcLoan.Id, DbType.Int32);

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

        public async Task<int> UpdateYearEndingData(YearEndingData yearEndingData)
        {
            int result = 0;
            var parameters = new DynamicParameters();
            string query = "";

            // Determine the correct query based on the type
            if (yearEndingData.Type == 1)
            {
                query = "UPDATE MCLYearEndings SET ClosingPrincipal = @ClosingPrincipal WHERE JobCode = @JobCode";
            }
            else if (yearEndingData.Type == 2)
            {
                query = "UPDATE comYearEndings SET ClosingPrincipal = @ClosingPrincipal WHERE JobCode = @JobCode";
            }
            else if (yearEndingData.Type == 3)
            {
                query = "UPDATE HBLYearEndings SET ClosingPrincipal = @ClosingPrincipal, ClosingInterest = @ClosingInterest WHERE JobCode = @JobCode";
                parameters.Add("ClosingInterest", yearEndingData.Interest, DbType.Decimal); // Add ClosingInterest for Type 3
            }

            // Add common parameters
            parameters.Add("ClosingPrincipal", yearEndingData.Amount, DbType.Decimal);
            parameters.Add("JobCode", yearEndingData.JobCode, DbType.String);

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    // Execute the query
                    result = await connection.ExecuteAsync(query, parameters);
                }
                catch (Exception ex)
                {
                    throw;  // Optionally log the exception or add custom handling
                }
            }

            return result;
        }




        public async Task<int> UpdateMcLoanByProcess(McLoan mcLoan)
        {
            int result = 0;
            var parameters = new DynamicParameters();
            string query = $"update MclLoans set RemainingLoanAmount=@RemainingLoanAmount, RemainingInstallmentNo=@RemainingInstallmentNo, LastProcessingDate=@LastProcessingDate,IsActive=@IsActive,DeactivatedDate=@DeactivatedDate,UpdatedBy=@UpdatedBy,UpdatedDate=@UpdatedDate  where Id=@Id";

            parameters.Add("RemainingLoanAmount", mcLoan.RemainingLoanAmount, DbType.Double);
            parameters.Add("RemainingInstallmentNo", mcLoan.RemainingInstallmentNo, DbType.Double);
            parameters.Add("LastProcessingDate", mcLoan.LastProcessingDate, DbType.DateTime);
            parameters.Add("IsActive", mcLoan.IsActive, DbType.Boolean);
            parameters.Add("DeactivatedDate", mcLoan.DeactivatedDate, DbType.DateTime);
            parameters.Add("UpdatedBy", mcLoan.UpdatedBy, DbType.String);
            parameters.Add("UpdatedDate", mcLoan.UpdatedDate, DbType.DateTime);
            parameters.Add("Id", mcLoan.Id, DbType.Int32);

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

        public async Task<int> UpdateLoansInFinalAdjustment(string jobCode, int monthId, double instalmentAmount, int loanType)
        {
            int result = 0;
            string query = "";
            

            if (jobCode.ToLower().Contains("fo"))
            {
                if (loanType == 1 || loanType==9) // hbl or flat
                {
                    query = $"update SalaryReportOF set HBLoan=@Amount where MonthID=@MonthID and JobCode=@JobCode";
                }
                if (loanType==2)// mcl
                {
                        query = $"update SalaryReportOF set MCylLoan=@Amount where MonthID=@MonthID and JobCode=@JobCode";
                }
                if (loanType == 4)// com
                {
                    query = $"update SalaryReportOF set ComLoan=@Amount where MonthID=@MonthID and JobCode=@JobCode";
                }
                if (loanType == 10)// car
                {
                    query = $"update SalaryReportOF set CarLoan=@Amount where MonthID=@MonthID and JobCode=@JobCode";
                }
            }
            else
            {
                if (loanType == 1 || loanType == 9) // hbl or flat
                {
                    query = $"update SalaryReportJS set HBLoan=@Amount where MonthID=@MonthID and JobCode=@JobCode";
                }
                if (loanType == 2)// mcl
                {
                    query = $"update SalaryReportJS set MCylLoan=@Amount where MonthID=@MonthID and JobCode=@JobCode";
                }
                if (loanType == 4)// com
                {
                    query = $"update SalaryReportJS set ComputerLoan=@Amount where MonthID=@MonthID and JobCode=@JobCode";
                }
            }
            var parameters = new DynamicParameters();

            parameters.Add("Amount", instalmentAmount, DbType.Double);
            parameters.Add("MonthID", monthId, DbType.Int32);
            parameters.Add("JobCode", jobCode, DbType.String);

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
        public async Task<int> CreateComLoan(ComLoan comLoan)
        {
            int result = 0;
            var parameters = new DynamicParameters();
            string query = $"insert into ComLoans (JobCode," +
                $"TotalLoanAmount," +
                $"RemainingLoanAmount," +
                $"LoanTakenDate," +
                $"InterestRate," +
                $"InstallmentNo," +
                $"RemainingInstallmentNo," +
                $"LastProcessingDate," +
                $"LoanTypeId," +
                $"InstallmentAmount," +
                $"IsPaused," +
                $"IsActive," +
                $"CreatedBy," +
                $"CreatedDate) values(@JobCode," +
                $"@TotalLoanAmount," +
                $"@RemainingLoanAmount," +
                $"@LoanTakenDate," +
                $"@InterestRate," +
                $"@InstallmentNo," +
                $"@RemainingInstallmentNo," +
                $"@LastProcessingDate," +
                $"@LoanTypeId," +
                $"@InstallmentAmount," +
                $"@IsPaused," +
                $"@IsActive," +
                $"@CreatedBy," +
                $"@CreatedDate)";

            parameters.Add("JobCode", comLoan.JobCode, DbType.String);
            parameters.Add("TotalLoanAmount", comLoan.TotalLoanAmount, DbType.Double);
            parameters.Add("RemainingLoanAmount", comLoan.RemainingLoanAmount, DbType.Double);
            parameters.Add("LoanTakenDate", comLoan.LoanTakenDate, DbType.DateTime);
            parameters.Add("InterestRate", comLoan.InterestRate, DbType.Double);
            parameters.Add("InstallmentNo", comLoan.InstallmentNo, DbType.Int32);
            parameters.Add("RemainingInstallmentNo", comLoan.RemainingInstallmentNo, DbType.Int32);
            parameters.Add("LastProcessingDate", comLoan.LastProcessingDate, DbType.DateTime);
            parameters.Add("LoanTypeId", comLoan.LoanTypeId, DbType.Int32);
            parameters.Add("InstallmentAmount", comLoan.InstallmentAmount, DbType.Double);
            parameters.Add("IsPaused", comLoan.IsPaused, DbType.Boolean);
            parameters.Add("IsActive", comLoan.IsActive, DbType.Boolean);
            parameters.Add("CreatedBy", comLoan.CreatedBy, DbType.String);
            parameters.Add("CreatedDate", comLoan.CreatedDate, DbType.DateTime);

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

        public async Task<List<ComLoanViewModel>> GetComLoans(int isActive)
        {
            var query = "SELECT ComLoans.Id,ComLoans.JobCode,Employees.EmployeeName,ComLoans.TotalLoanAmount, ComLoans.RemainingLoanAmount,ComLoans.LoanTakenDate,ComLoans.InstallmentAmount,ComLoans.InstallmentNo,ComLoans.RemainingInstallmentNo,ComLoans.InterestRate FROM ComLoans join Employees on Employees.JobCode = ComLoans.JobCode where ComLoans.IsActive=@IsActive";
            var parameters = new DynamicParameters();
            parameters.Add("IsActive", isActive, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                var comLoans = await connection.QueryAsync<ComLoanViewModel>(query, new { IsActive = isActive });
                return comLoans.ToList();
            }
        }

        public async Task<ComLoan> GetComLoanById(int id)
        {
            var query = "SELECT * from ComLoans where Id=@id";
            var parameters = new DynamicParameters();
            parameters.Add("id", id, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                var comLoan = await connection.QuerySingleOrDefaultAsync<ComLoan>(query, parameters);
                return comLoan;
            }
        }


        public async Task<List<ComInstallment>> GetComInstallmentsByDate(int loanId, int fromMonthId, int toMonthId)
        {
            var query = "select * from ComInstallments where ComLoanId=@ComLoanId and (MonthId between @FromMonthId and @ToMonthId) order by Id";
            using (var connection = _context.CreateConnection())
            {
                var comInstallments = await connection.QueryAsync<ComInstallment>(query, new { ComLoanId = loanId, FromMonthId = fromMonthId, ToMonthId = toMonthId });
                return comInstallments.ToList();
            }
        }

        public async Task<List<HblInstallment>> GetHBLInstallmentsByDate(int loanId, int fromMonthId, int toMonthId)
        {
            var query = "select * from HblInstallments where HbLoanId=@HbLoanId and (MonthId between @FromMonthId and @ToMonthId) order by Id";
            using (var connection = _context.CreateConnection())
            {
                var hblInstallments = await connection.QueryAsync<HblInstallment>(query, new { HbLoanId = loanId, FromMonthId = fromMonthId, ToMonthId = toMonthId });
                return hblInstallments.ToList();
            }
        }

        public async Task<List<ComInstallment>> GetPreviousComInstallmentsByDate(int loanId, int fromMonthId, int offsetMonthId)
        {
            var query = "select * from ComInstallments where ComLoanId=@ComLoanId and (MonthId not between @FromMonthId and @OffsetMonthId) order by Id";
            using (var connection = _context.CreateConnection())
            {
                var comInstallments = await connection.QueryAsync<ComInstallment>(query, new { ComLoanId = loanId, FromMonthId = fromMonthId, OffsetMonthId = offsetMonthId });
                return comInstallments.ToList();
            }
        }
        public async Task<List<decimal>> GetPreviousComClosingInstallments(int loanId, int monthId)
        {
            var query = "Select ClosingPrincipal from ComYearEndings where LoanId=@ComLoanId And MonthId=@MonthId";
            using (var connection = _context.CreateConnection())
            {
                var closingPrincipals = await connection.QueryAsync<decimal>(query, new { ComLoanId = loanId, MonthId = monthId});
                return closingPrincipals.ToList();
            }
        }
        public async Task<ComYearEnding> GetComYearEnding(int loanId, int yearEndingMonthId)
        {
            var query = "select * from ComYearEndings where LoanId=@LoanId and MonthId=@MonthId";
            using (var connection = _context.CreateConnection())
            {
                var comYearEnding = await connection.QuerySingleOrDefaultAsync<ComYearEnding>(query, new { LoanId = loanId, MonthId = yearEndingMonthId });
                return comYearEnding;
            }
        }
        public async Task<HblYearEnding> GethblYearEnding(int loanId, int yearEndingMonthId)
        {
            var query = "select * from HBLYearEndings where LoanId=@LoanId and MonthId=@MonthId";
            using (var connection = _context.CreateConnection())
            {
                var hblYearEnding = await connection.QuerySingleOrDefaultAsync<HblYearEnding>(query, new { LoanId = loanId, MonthId = yearEndingMonthId });
                return hblYearEnding;
            }
        }
        public async Task<int> UpdateComLoan(ComLoan comLoan)
        {
            int result = 0;
            var parameters = new DynamicParameters();
            string query = $"update ComLoans set InstallmentAmount=@InstallmentAmount, IsPaused=@IsPaused, IsActive=@IsActive,UpdatedBy=@UpdatedBy,UpdatedDate=@UpdatedDate  where Id=@Id";

            parameters.Add("InstallmentAmount", comLoan.InstallmentAmount, DbType.Double);
            parameters.Add("IsPaused", comLoan.IsPaused, DbType.Boolean);
            parameters.Add("IsActive", comLoan.IsActive, DbType.Boolean);
            parameters.Add("UpdatedBy", comLoan.UpdatedBy, DbType.String);
            parameters.Add("UpdatedDate", comLoan.UpdatedDate, DbType.DateTime);
            parameters.Add("Id", comLoan.Id, DbType.Int32);

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
        public async Task<int> UpdateComLoanByProcess(ComLoan comLoan)
        {
            int result = 0;
            var parameters = new DynamicParameters();
            string query = $"update ComLoans set RemainingLoanAmount=@RemainingLoanAmount, RemainingInstallmentNo=@RemainingInstallmentNo, LastProcessingDate=@LastProcessingDate,IsActive=@IsActive,DeactivatedDate=@DeactivatedDate,UpdatedBy=@UpdatedBy,UpdatedDate=@UpdatedDate  where Id=@Id";

            parameters.Add("RemainingLoanAmount", comLoan.RemainingLoanAmount, DbType.Double);
            parameters.Add("RemainingInstallmentNo", comLoan.RemainingInstallmentNo, DbType.Double);
            parameters.Add("LastProcessingDate", comLoan.LastProcessingDate, DbType.DateTime);
            parameters.Add("IsActive", comLoan.IsActive, DbType.Boolean);
            parameters.Add("DeactivatedDate", comLoan.DeactivatedDate, DbType.DateTime);
            parameters.Add("UpdatedBy", comLoan.UpdatedBy, DbType.String);
            parameters.Add("UpdatedDate", comLoan.UpdatedDate, DbType.DateTime);
            parameters.Add("Id", comLoan.Id, DbType.Int32);

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
        public async Task<int> CreateComLoanInstallment(ComInstallment comInstallment)
        {
            int result = 0;
            var parameters = new DynamicParameters();
            string query = $"insert into ComInstallments (JobCode," +
                $"InstallmentAmount," +
                $"InterestAmount," +
                $"MonthId," +
                $"ComLoanId) values(@JobCode," +
                $"@InstallmentAmount," +
                $"@InterestAmount," +
                $"@MonthId," +
                $"@ComLoanId)";

            parameters.Add("JobCode", comInstallment.JobCode, DbType.String);
            parameters.Add("InstallmentAmount", comInstallment.InstallmentAmount, DbType.Double);
            parameters.Add("InterestAmount", comInstallment.InterestAmount, DbType.Double);
            parameters.Add("MonthId", comInstallment.MonthId, DbType.Int32);
            parameters.Add("ComLoanId", comInstallment.ComLoanId, DbType.Int32);

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


        public async Task<List<CarLoanViewModel>> GetCarLoans(int isActive)
        {
            var query = "SELECT CarLoans.Id,CarLoans.JobCode,Employees.EmployeeName,CarLoans.TotalLoanAmount, CarLoans.DepreciationAmount, CarLoans.RemainingDepreciationAmount, CarLoans.ActualAmount, CarLoans.RemainingActualAmount, CarLoans.InterestRate,  CarLoans.LoanTakenDate, CarLoans.InstallmentNo, CarLoans.RemainingInstallmentNo, CarLoans.InterestRate  FROM CarLoans join Employees on Employees.JobCode = CarLoans.JobCode where CarLoans.IsActive=@IsActive";
            var parameters = new DynamicParameters();
            parameters.Add("IsActive", isActive, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                var carLoans = await connection.QueryAsync<CarLoanViewModel>(query, new { IsActive = isActive });
                return carLoans.ToList();
            }
        }

        public async Task<CarLoan> GetCarLoanById(int id)
        {
            var query = "SELECT * from CarLoans where Id=@id";
            var parameters = new DynamicParameters();
            parameters.Add("id", id, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                var carLoan = await connection.QuerySingleOrDefaultAsync<CarLoan>(query, parameters);
                return carLoan;
            }
        }

        public async Task<int> CreateCarLoan(CarLoan carLoan)
        {
            int result = 0;
            var parameters = new DynamicParameters();
            string query = $"insert into CarLoans (JobCode," +
                $"TotalLoanAmount," +
                $"DepreciationAmount," +
                $"ActualAmount," +
                $"InterestRate," +
                $"RemainingDepreciationAmount," +
                $"RemainingActualAmount," +
                $"InstallmentNo," +
                $"RemainingInstallmentNo," +
                $"LoanTakenDate," +
                $"IsActive," +
                $"LoanTypeId," +
                $"LastProcessingDate," +
                $"CreatedBy," +
                $"CreatedDate) values(@JobCode," +
                $"@TotalLoanAmount," +
                $"@DepreciationAmount," +
                $"@ActualAmount," +
                $"@InterestRate," +
                $"@RemainingDepreciationAmount," +
                $"@RemainingActualAmount," +
                $"@InstallmentNo," +
                $"@RemainingInstallmentNo," +
                $"@LoanTakenDate," +
                $"@IsActive," +
                $"@LoanTypeId," +
                $"@LastProcessingDate," +
                $"@CreatedBy," +
                $"@CreatedDate); SELECT CAST(SCOPE_IDENTITY() AS INT);";

            parameters.Add("JobCode", carLoan.JobCode, DbType.String);
            parameters.Add("TotalLoanAmount", carLoan.TotalLoanAmount, DbType.Double);
            parameters.Add("DepreciationAmount", carLoan.DepreciationAmount, DbType.Double);
            parameters.Add("ActualAmount", carLoan.ActualAmount, DbType.Double);
            parameters.Add("InterestRate", carLoan.InterestRate, DbType.Double);
            parameters.Add("RemainingDepreciationAmount", carLoan.RemainingDepreciationAmount, DbType.Double);
            parameters.Add("RemainingActualAmount", carLoan.RemainingActualAmount, DbType.Double);
            parameters.Add("InstallmentNo", carLoan.InstallmentNo, DbType.Double);
            parameters.Add("RemainingInstallmentNo", carLoan.RemainingInstallmentNo, DbType.Double);
            parameters.Add("LoanTakenDate", carLoan.LoanTakenDate, DbType.DateTime);
            parameters.Add("IsActive", carLoan.IsActive, DbType.Boolean);
            parameters.Add("LoanTypeId", carLoan.LoanTypeId, DbType.Int32);
            parameters.Add("LastProcessingDate", carLoan.LastProcessingDate, DbType.DateTime);
            parameters.Add("CreatedBy", carLoan.CreatedBy, DbType.String);
            parameters.Add("CreatedDate", carLoan.CreatedDate, DbType.DateTime);

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

        public async Task<int> UpdateCarLoan(CarLoan carLoan)
        {
            int result = 0;
            var parameters = new DynamicParameters();
            string query = $"update CarLoans set RemainingActualAmount = @RemainingActualAmount," +
                $"RemainingDepreciationAmount = @RemainingDepreciationAmount," +
                $"RemainingInstallmentNo = @RemainingInstallmentNo," +
                $"IsActive = @IsActive," +
                $"LastProcessingDate=@LastProcessingDate," +
                $"DeactivatedDate = @DeactivatedDate," +
                $"UpdatedBy=@UpdatedBy," +
                $"UpdatedDate=@UpdatedDate where Id = @Id";

            
            parameters.Add("RemainingActualAmount", carLoan.RemainingActualAmount, DbType.Double);
            parameters.Add("RemainingDepreciationAmount", carLoan.RemainingDepreciationAmount, DbType.Double);
            parameters.Add("RemainingInstallmentNo", carLoan.RemainingInstallmentNo, DbType.Double);
            parameters.Add("IsActive", carLoan.IsActive, DbType.Boolean);
            parameters.Add("LastProcessingDate", carLoan.LastProcessingDate, DbType.DateTime);
            parameters.Add("DeactivatedDate", carLoan.DeactivatedDate, DbType.DateTime);
            parameters.Add("UpdatedBy", carLoan.UpdatedBy, DbType.String);
            parameters.Add("UpdatedDate", carLoan.UpdatedDate, DbType.DateTime);
            parameters.Add("Id", carLoan.Id, DbType.Int32);


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

        public async Task<int> CreateCarLoanInstallment(CarLoanInstallment carLoanInstallment)
        {
            int result = 0;
            var parameters = new DynamicParameters();
            string query = $"insert into CarLoanInstallments (CarLoanId," +
                $"MonthId," +
                $"PrincipalAmount," +
                $"InterestAmount," +
                $"TotalPayment," +
                $"RemainingBalance," +
                $"DepreciationAmount," +
                $"IsPaid) values(@CarLoanId," +
                $"@MonthId," +
                $"@PrincipalAmount," +
                $"@InterestAmount," +
                $"@TotalPayment," +
                $"@RemainingBalance," +
                $"@DepreciationAmount," +
                $"@IsPaid)";

            parameters.Add("CarLoanId", carLoanInstallment.CarLoanId, DbType.Int32);
            parameters.Add("MonthId", carLoanInstallment.MonthId, DbType.Int32);
            parameters.Add("PrincipalAmount", carLoanInstallment.PrincipalAmount, DbType.Double);
            parameters.Add("InterestAmount", carLoanInstallment.InterestAmount, DbType.Double);
            parameters.Add("TotalPayment", carLoanInstallment.TotalPayment, DbType.Double);
            parameters.Add("RemainingBalance", carLoanInstallment.RemainingBalance, DbType.Double);
            parameters.Add("DepreciationAmount", carLoanInstallment.DepreciationAmount, DbType.Double);
            parameters.Add("IsPaid", carLoanInstallment.IsPaid, DbType.Boolean);

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

        public async Task<int> UpdateCarLoanInstallment(CarLoanInstallment carLoanInstallment)
        {
            int result = 0;
            var parameters = new DynamicParameters();
            string query = $"update CarLoanInstallments set " +
                $"DepreciationAmount=@DepreciationAmount," +
                $"IsPaid=@IsPaid where Id = @Id";

            parameters.Add("DepreciationAmount", carLoanInstallment.DepreciationAmount, DbType.Double);
            parameters.Add("IsPaid", carLoanInstallment.IsPaid, DbType.Boolean);
            parameters.Add("Id", carLoanInstallment.Id, DbType.Int32);

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

        public async Task<int> CreateCarLoanDepreciationInstallment(DepreciationInstallment depreciationInstallment)
        {
            int result = 0;
            var parameters = new DynamicParameters();
            string query = $"insert into CarLoanDepreciationInstallments (CarLoanId," +
                $"MonthId," +
                $"DepreciationAmount," +
                $"IsPaid) values(@CarLoanId," +
                $"@MonthId," +
                $"@DepreciationAmount," +
                $"@IsPaid)";

            parameters.Add("CarLoanId", depreciationInstallment.CarLoanId, DbType.Int32);
            parameters.Add("MonthId", depreciationInstallment.MonthId, DbType.Int32);
            parameters.Add("DepreciationAmount", depreciationInstallment.DepreciationAmount, DbType.Double);
            parameters.Add("IsPaid", depreciationInstallment.IsPaid, DbType.Boolean);

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

        public async Task<int> UpdateCarLoanDepreciationInstallment(DepreciationInstallment depreciationInstallment)
        {
            int result = 0;
            var parameters = new DynamicParameters();
            string query = $"update CarLoanDepreciationInstallments set " +
                $"IsPaid=@IsPaid where Id = @Id";

            parameters.Add("IsPaid", depreciationInstallment.IsPaid, DbType.Boolean);
            parameters.Add("Id", depreciationInstallment.Id, DbType.Int32);

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
        public async Task<List<CarLoanInstallment>> GetCarLoanInstallments(int loanId)
        {
            var query = "select * from CarLoanInstallments where CarLoanId=@CarLoanId order by MonthId asc";
            using (var connection = _context.CreateConnection())
            {
                var carLoanInstallments = await connection.QueryAsync<CarLoanInstallment>(query, new { CarLoanId= loanId });
                return carLoanInstallments.ToList();
            }
        }
        public async Task<List<CarInstallmentReport>> GetCarLoanInstallmentsReport(int loanId)
        {
            var query = @"
            SELECT 
                Employees.JobCode,
                Employees.EmployeeName,
                ActualAmount,
                ci.MonthId,
                ci.PrincipalAmount,
                ci.InterestAmount,
                ci.TotalPayment,
                ci.RemainingBalance,
                ci.DepreciationAmount,
                ci.IsPaid
                FROM 
                    CarLoanInstallments ci
                JOIN 
                    CarLoans c ON c.Id = ci.CarLoanId
                JOIN 
                    Employees ON Employees.JobCode = c.JobCode
                WHERE 
                    ci.CarLoanId = @CarLoanId
                ORDER BY 
                    ci.MonthId asc";

            using (var connection = _context.CreateConnection())
            {
                var carLoanInstallments = await connection.QueryAsync<CarInstallmentReport>(query, new { CarLoanId = loanId });

                return carLoanInstallments.ToList();
            }
        }




        public async Task<List<DepreciationInstallment>> GetCarLoanDepreciationInstallments(int loanId)
        {
            var query = "select * from CarLoanDepreciationInstallments where CarLoanId=@CarLoanId order by MonthId asc";
            var parameters = new DynamicParameters();
            parameters.Add("CarLoanId", loanId, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                var depreciationInstallments = await connection.QueryAsync<DepreciationInstallment>(query, new { CarLoanId = loanId });
                return depreciationInstallments.ToList();
            }
        }

        public async Task<List<DepreciationInstallment>> GetCarLoanDepreciationInstallmentsByMonth(int loanId, int startMonthId,int endMonthId)
        {
            var query = "select * from CarLoanDepreciationInstallments where IsPaid=0 and CarLoanId=@CarLoanId and (monthid between @StartMonthId and @EndMonthId) order by MonthId asc";
            var parameters = new DynamicParameters();
            parameters.Add("CarLoanId", loanId, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                var depreciationInstallments = await connection.QueryAsync<DepreciationInstallment>(query, new { CarLoanId = loanId, StartMonthId=startMonthId, EndMonthId =endMonthId});
                return depreciationInstallments.ToList();
            }
        }

        public async Task<List<CarDepreciateReport>> GetCarLoanDepreciationReport(int loanId)
        {
            var query = "select Employees.JobCode,EmployeeName,cd.MonthId,cd.DepreciationAmount,cd.IsPaid,c.DepreciationAmount AS ActualAmount from CarLoanDepreciationInstallments cd join CarLoans c on c.Id=cd.CarLoanId join Employees on Employees.JobCode = c.JobCode  where CarLoanId=@CarLoanId order by cd.MonthId asc";
            var parameters = new DynamicParameters();
            parameters.Add("CarLoanId", loanId, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                var depreciationInstallments = await connection.QueryAsync<CarDepreciateReport>(query, new { CarLoanId = loanId });
                return depreciationInstallments.ToList();
            }
        }

        public async Task<HbLoan> GetHbLoanById(int id)
        {
            try
            {
                var query = "select * from HbLoans where Id=@Id";
                using (var connection = _context.CreateConnection())
                {
                    var hbLoan = await connection.QuerySingleOrDefaultAsync<HbLoan>(query, new { Id = id });
                    return hbLoan;
                }
            }
            catch (Exception ex) 
            {
                return null;
            }

        }

        public async Task<List<HbLoanViewModel>> GetHbLoans(int isActive)
        {
            try
            {
                var query = "SELECT HbLoans.Id,HbLoans.JobCode,Employees.EmployeeName,HbLoans.TotalLoanAmount, HbLoans.RemainingLoanAmount,HbLoans.LoanTakenDate,HbLoans.PrincipalInstallmentAmount,HbLoans.InstallmentNo,HbLoans.RemainingInstallmentNo,HbLoans.InterestRate, HbLoans.TotalInterest, HbLoans.RemainingInterest, HbLoans.InterestInstallmentNo, HbLoans.RemainingInterestInstallmentNo  FROM HbLoans join Employees on Employees.JobCode = HbLoans.JobCode where HbLoans.IsActive=@IsActive and HbLoans.LoanTypeId in(1,9)";
                using (var connection = _context.CreateConnection())
                {
                    var hbLoans = await connection.QueryAsync<HbLoanViewModel>(query, new { IsActive = isActive });
                    return hbLoans.ToList();
                }
            }
            catch(Exception ex)
            {
                return new List<HbLoanViewModel>();
            }
           
        }

        public async Task<List<HbLoanViewModel>> GetHbLoansByLoanType(int isActive,int loanTypeId)
        {
            var query = "SELECT HbLoans.Id,HbLoans.JobCode,Employees.EmployeeName,HbLoans.TotalLoanAmount, HbLoans.RemainingLoanAmount,HbLoans.LoanTakenDate,HbLoans.PrincipalInstallmentAmount,HbLoans.InstallmentNo,HbLoans.RemainingInstallmentNo,HbLoans.InterestRate, HbLoans.TotalInterest, HbLoans.RemainingInterest, HbLoans.InterestInstallmentNo, HbLoans.RemainingInterestInstallmentNo,HbLoans.InterestInstallmentAmount  FROM HbLoans join Employees on Employees.JobCode = HbLoans.JobCode where HbLoans.IsActive=@IsActive and HbLoans.LoanTypeId=@LoanTypeId";
            using (var connection = _context.CreateConnection())
            {
                var hbLoans = await connection.QueryAsync<HbLoanViewModel>(query, new { IsActive = isActive, LoanTypeId= loanTypeId });
                return hbLoans.ToList();
            }
        }

        public async Task<List<HbLoanViewModel>> GetHbLoansByEmployeeId(int employeeId)
        {
            var query = @"
        SELECT HbLoans.Id, HbLoans.JobCode, Employees.EmployeeName, HbLoans.TotalLoanAmount, 
               HbLoans.RemainingLoanAmount, HbLoans.LoanTakenDate, HbLoans.PrincipalInstallmentAmount, 
               HbLoans.InstallmentNo, HbLoans.RemainingInstallmentNo, HbLoans.InterestRate, 
               HbLoans.TotalInterest, HbLoans.RemainingInterest, HbLoans.InterestInstallmentNo, 
               HbLoans.RemainingInterestInstallmentNo, HbLoans.InterestInstallmentAmount  
                FROM HbLoans 
                JOIN Employees ON Employees.JobCode = HbLoans.JobCode 
                WHERE HbLoans.IsActive = 1 
                  AND HbLoans.LoanTypeId = 1 
                  AND Employees.Id = @employeeId";

            using (var connection = _context.CreateConnection())
            {
                var hbLoans = await connection.QueryAsync<HbLoanViewModel>(query, new { employeeId });
                return hbLoans.ToList();
            }
        }



        public async Task<int> CreateHbLoan(HbLoan hbLoan)
        {
            int result = 0;
            var parameters = new DynamicParameters();
            string query = $"insert into HbLoans (JobCode," +
                $"TotalLoanAmount," +
                $"RemainingLoanAmount," +
                $"LoanTakenDate," +
                $"InterestRate," +
                $"InstallmentNo," +
                $"RemainingInstallmentNo," +
                $"LastProcessingDate," +
                $"LoanTypeId," +
                $"PrincipalInstallmentAmount," +
                $"IsPaused," +
                $"IsActive," +
                $"TotalInterest," +
                $"RemainingInterest," +
                $"InterestInstallmentNo," +
                $"RemainingInterestInstallmentNo," +
                $"InterestInstallmentAmount," +
                $"DateAfterPeriod," +
                $"IsRescheduled," +
                $"CreatedBy," +
                $"CreatedDate) values(@JobCode," +
                $"@TotalLoanAmount," +
                $"@RemainingLoanAmount," +
                $"@LoanTakenDate," +
                $"@InterestRate," +
                $"@InstallmentNo," +
                $"@RemainingInstallmentNo," +
                $"@LastProcessingDate," +
                $"@LoanTypeId," +
                $"@PrincipalInstallmentAmount," +
                $"@IsPaused," +
                $"@IsActive," +
                $"@TotalInterest," +
                $"@RemainingInterest," +
                $"@InterestInstallmentNo," +
                $"@RemainingInterestInstallmentNo," +
                $"@InterestInstallmentAmount," +
                $"@DateAfterPeriod," +
                $"@IsRescheduled," +
                $"@CreatedBy," +
                $"@CreatedDate)";

            parameters.Add("JobCode", hbLoan.JobCode, DbType.String);
            parameters.Add("TotalLoanAmount", hbLoan.TotalLoanAmount, DbType.Double);
            parameters.Add("RemainingLoanAmount", hbLoan.RemainingLoanAmount, DbType.Double);
            parameters.Add("LoanTakenDate", hbLoan.LoanTakenDate, DbType.DateTime);
            parameters.Add("InterestRate", hbLoan.InterestRate, DbType.Double);
            parameters.Add("InstallmentNo", hbLoan.InstallmentNo, DbType.Int32);
            parameters.Add("RemainingInstallmentNo", hbLoan.RemainingInstallmentNo, DbType.Int32);
            parameters.Add("LastProcessingDate", hbLoan.LastProcessingDate, DbType.DateTime);
            parameters.Add("LoanTypeId", hbLoan.LoanTypeId, DbType.Int32);
            parameters.Add("PrincipalInstallmentAmount", hbLoan.PrincipalInstallmentAmount, DbType.Double);
            parameters.Add("IsPaused", hbLoan.IsPaused, DbType.Boolean);
            parameters.Add("IsActive", hbLoan.IsActive, DbType.Boolean);
            parameters.Add("TotalInterest", hbLoan.TotalInterest, DbType.Double);
            parameters.Add("RemainingInterest", hbLoan.RemainingInterest, DbType.Double);
            parameters.Add("InterestInstallmentNo", hbLoan.InterestInstallmentNo, DbType.Int32);
            parameters.Add("RemainingInterestInstallmentNo", hbLoan.RemainingInterestInstallmentNo, DbType.Int32);
            parameters.Add("InterestInstallmentAmount", hbLoan.InterestInstallmentAmount, DbType.Double);
            parameters.Add("DateAfterPeriod", hbLoan.DateAfterPeriod, DbType.DateTime);
            parameters.Add("IsRescheduled", hbLoan.IsRescheduled, DbType.Boolean);
            parameters.Add("CreatedBy", hbLoan.CreatedBy, DbType.String);
            parameters.Add("CreatedDate", hbLoan.CreatedDate, DbType.DateTime);

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

        public async Task<int> UpdateHbLoan(HbLoan hbLoan)
        {
            int result = 0;
            var parameters = new DynamicParameters();
            string query = $"update HbLoans set " +
                $"PrincipalInstallmentAmount=@PrincipalInstallmentAmount," +
                $"InterestInstallmentAmount=@InterestInstallmentAmount," +
                $"InterestInstallmentNo=@InterestInstallmentNo," +
                $"RemainingInterestInstallmentNo=@RemainingInterestInstallmentNo," +
                $"IsActive=@IsActive," +
                $"IsPaused=@IsPaused," +
                $"UpdatedBy=@UpdatedBy," +
                $"UpdatedDate=@UpdatedDate where Id = @Id";

            parameters.Add("PrincipalInstallmentAmount", hbLoan.PrincipalInstallmentAmount, DbType.Double);
            parameters.Add("InterestInstallmentAmount", hbLoan.InterestInstallmentAmount, DbType.Double);
            parameters.Add("InterestInstallmentNo", hbLoan.InterestInstallmentNo, DbType.Double);
            parameters.Add("RemainingInterestInstallmentNo", hbLoan.RemainingInterestInstallmentNo, DbType.Double);
            parameters.Add("IsActive", hbLoan.IsActive, DbType.Boolean);
            parameters.Add("IsPaused", hbLoan.IsPaused, DbType.Boolean);
            parameters.Add("UpdatedBy", hbLoan.UpdatedBy, DbType.String);
            parameters.Add("UpdatedDate", hbLoan.UpdatedDate, DbType.Date);
            parameters.Add("Id", hbLoan.Id, DbType.Int32);

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

        public async Task<int> CreateHbLoanInstallment(HblInstallment hblInstallment)
        {
            int result = 0;
            var parameters = new DynamicParameters();
            string query = $"insert into HblInstallments (JobCode," +
                $"InstallmentAmount," +
                $"InterestAmount," +
                $"MonthId," +
                $"InstallmentType," +
                $"HbLoanId) values(@JobCode," +
                $"@InstallmentAmount," +
                $"@InterestAmount," +
                $"@MonthId," +
                $"@InstallmentType," +
                $"@HbLoanId)";

            parameters.Add("JobCode", hblInstallment.JobCode, DbType.String);
            parameters.Add("InstallmentAmount", hblInstallment.InstallmentAmount, DbType.Double);
            parameters.Add("InterestAmount", hblInstallment.InterestAmount, DbType.Double);
            parameters.Add("MonthId", hblInstallment.MonthId, DbType.Int32);
            parameters.Add("InstallmentType", hblInstallment.InstallmentType, DbType.String);
            parameters.Add("HbLoanId", hblInstallment.HbLoanId, DbType.Int32);

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

        public async Task<int> UpdateHbLoanByProcess(HbLoan hbLoan)
        {
            int result = 0;
            var parameters = new DynamicParameters();
            string query = $"update HbLoans set RemainingLoanAmount=@RemainingLoanAmount, " +
                $"RemainingInstallmentNo=@RemainingInstallmentNo," +
                $"TotalInterest=@TotalInterest," +
                $"RemainingInterest=@RemainingInterest," +
                $"RemainingInterestInstallmentNo=@RemainingInterestInstallmentNo, " +
                $"LastProcessingDate=@LastProcessingDate," +
                $"IsActive=@IsActive," +
                $"DeactivatedDate=@DeactivatedDate," +
                $"UpdatedBy=@UpdatedBy," +
                $"UpdatedDate=@UpdatedDate  where Id=@Id";

            parameters.Add("RemainingLoanAmount", hbLoan.RemainingLoanAmount, DbType.Double);
            parameters.Add("RemainingInstallmentNo", hbLoan.RemainingInstallmentNo, DbType.Int32);
            parameters.Add("TotalInterest", hbLoan.TotalInterest, DbType.Double);
            parameters.Add("RemainingInterest", hbLoan.RemainingInterest, DbType.Double);
            parameters.Add("RemainingInterestInstallmentNo", hbLoan.RemainingInterestInstallmentNo, DbType.Int32);
            parameters.Add("LastProcessingDate", hbLoan.LastProcessingDate, DbType.DateTime);
            parameters.Add("IsActive", hbLoan.IsActive, DbType.Boolean);
            parameters.Add("DeactivatedDate", hbLoan.DeactivatedDate, DbType.DateTime);
            parameters.Add("UpdatedBy", hbLoan.UpdatedBy, DbType.String);
            parameters.Add("UpdatedDate", hbLoan.UpdatedDate, DbType.DateTime);
            parameters.Add("Id", hbLoan.Id, DbType.Int32);

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


        public async Task<HblInstallment> GetInstallmentByHBLoanId_MonthId(int loanId, int monthId)
        {
            var query = "select * from HblInstallments where HbLoanId = @loanId and MonthId = @monthId";

            using (var connection = _context.CreateConnection())
            {
                var installment = await connection.QuerySingleOrDefaultAsync<HblInstallment>(query, new { loanId, monthId });
                return installment;
            }
        }

        public async Task<MclInstallment> GetInstallmentByMCLoanId_MonthId(int loanId, int monthId)
        {
            var query = "select * from MclInstallments where MclId = @loanId and MonthId = @monthId";

            using (var connection = _context.CreateConnection())
            {
                var installment = await connection.QuerySingleOrDefaultAsync<MclInstallment>(query, new { loanId, monthId });
                return installment;
            }
        }

        public async Task<ComInstallment> GetInstallmentByComLoanId_MonthId(int loanId, int monthId)
        {
            var query = "select * from ComInstallments where ComLoanId = @loanId and MonthId = @monthId";

            using (var connection = _context.CreateConnection())
            {
                var installment = await connection.QuerySingleOrDefaultAsync<ComInstallment>(query, new { loanId, monthId });
                return installment;
            }
        }

        public async Task<CarLoanInstallment> GetInstallmentByCarLoanId_MonthId(int loanId, int monthId)
        {
            var query = "select * from CarLoanInstallments where CarLoanId = @loanId and MonthId = @monthId and IsPaid = 1";

            using (var connection = _context.CreateConnection())
            {
                var installment = await connection.QuerySingleOrDefaultAsync<CarLoanInstallment>(query, new { loanId, monthId });
                return installment;
            }
        }



        public async Task<int> CreateHbLoanHistory(HbLoan hbLoan)
        {
            int result = 0;
            var parameters = new DynamicParameters();
            string query = $"insert into HbLoanHistories (JobCode," +
                $"TotalLoanAmount," +
                $"RemainingLoanAmount," +
                $"LoanTakenDate," +
                $"InterestRate," +
                $"InstallmentNo," +
                $"RemainingInstallmentNo," +
                $"LastProcessingDate," +
                $"LoanTypeId," +
                $"PrincipalInstallmentAmount," +
                $"TotalInterest," +
                $"RemainingInterest," +
                $"InterestInstallmentNo," +
                $"RemainingInterestInstallmentNo," +
                $"InterestInstallmentAmount," +
                $"DateAfterPeriod," +
                $"HBLoanId," +
                $"CreatedBy," +
                $"CreatedDate) values(@JobCode," +
                $"@TotalLoanAmount," +
                $"@RemainingLoanAmount," +
                $"@LoanTakenDate," +
                $"@InterestRate," +
                $"@InstallmentNo," +
                $"@RemainingInstallmentNo," +
                $"@LastProcessingDate," +
                $"@LoanTypeId," +
                $"@PrincipalInstallmentAmount," +
                $"@TotalInterest," +
                $"@RemainingInterest," +
                $"@InterestInstallmentNo," +
                $"@RemainingInterestInstallmentNo," +
                $"@InterestInstallmentAmount," +
                $"@DateAfterPeriod," +
                $"@HBLoanId," +
                $"@CreatedBy," +
                $"@CreatedDate)";

            parameters.Add("JobCode", hbLoan.JobCode, DbType.String);
            parameters.Add("TotalLoanAmount", hbLoan.TotalLoanAmount, DbType.Double);
            parameters.Add("RemainingLoanAmount", hbLoan.RemainingLoanAmount, DbType.Double);
            parameters.Add("LoanTakenDate", hbLoan.LoanTakenDate, DbType.DateTime);
            parameters.Add("InterestRate", hbLoan.InterestRate, DbType.Double);
            parameters.Add("InstallmentNo", hbLoan.InstallmentNo, DbType.Int32);
            parameters.Add("RemainingInstallmentNo", hbLoan.RemainingInstallmentNo, DbType.Int32);
            parameters.Add("LastProcessingDate", hbLoan.LastProcessingDate, DbType.DateTime);
            parameters.Add("LoanTypeId", hbLoan.LoanTypeId, DbType.Int32);
            parameters.Add("PrincipalInstallmentAmount", hbLoan.PrincipalInstallmentAmount, DbType.Double);
            parameters.Add("TotalInterest", hbLoan.TotalInterest, DbType.Double);
            parameters.Add("RemainingInterest", hbLoan.RemainingInterest, DbType.Double);
            parameters.Add("InterestInstallmentNo", hbLoan.InterestInstallmentNo, DbType.Int32);
            parameters.Add("RemainingInterestInstallmentNo", hbLoan.RemainingInterestInstallmentNo, DbType.Int32);
            parameters.Add("InterestInstallmentAmount", hbLoan.InterestInstallmentAmount, DbType.Double);
            parameters.Add("DateAfterPeriod", hbLoan.DateAfterPeriod, DbType.DateTime);
            parameters.Add("HBLoanId", hbLoan.Id, DbType.Int32);
            parameters.Add("CreatedBy", hbLoan.CreatedBy, DbType.String);
            parameters.Add("CreatedDate", hbLoan.CreatedDate, DbType.DateTime);

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

        public async Task<int> UpdateHbLoanForReschedule(HbLoan hbLoan)
        {
            int result = 0;
            var parameters = new DynamicParameters();
            string query = $"update HbLoans set " +
                $"LoanTakenDate=@LoanTakenDate," +
                $"LastProcessingDate=@LastProcessingDate," +
                $"TotalLoanAmount=@TotalLoanAmount," +
                $"RemainingLoanAmount=@RemainingLoanAmount," +
                $"InterestRate=@InterestRate," +
                $"TotalInterest=@TotalInterest," +
                $"RemainingInterest=@RemainingInterest," +
                $"InstallmentNo=@InstallmentNo," +
                $"RemainingInstallmentNo=@RemainingInstallmentNo," +
                $"InterestInstallmentNo=@InterestInstallmentNo," +
                $"RemainingInterestInstallmentNo=@RemainingInterestInstallmentNo," +
                $"PrincipalInstallmentAmount=@PrincipalInstallmentAmount," +
                $"IsPaused=@IsPaused," +
                $"IsRescheduled=@IsRescheduled," +
                $"DateAfterPeriod=@DateAfterPeriod," +
                $"UpdatedBy=@UpdatedBy," +
                $"UpdatedDate=@UpdatedDate " +
                $" where Id = @Id";

            parameters.Add("LoanTakenDate", hbLoan.LoanTakenDate, DbType.DateTime);
            parameters.Add("LastProcessingDate", hbLoan.LastProcessingDate, DbType.DateTime);
            parameters.Add("TotalLoanAmount", hbLoan.TotalLoanAmount, DbType.Double);
            parameters.Add("RemainingLoanAmount", hbLoan.RemainingLoanAmount, DbType.Double);
            parameters.Add("InterestRate", hbLoan.InterestRate, DbType.Double);
            parameters.Add("TotalInterest", hbLoan.TotalInterest, DbType.Double);
            parameters.Add("RemainingInterest", hbLoan.RemainingInterest, DbType.Double);
            parameters.Add("InstallmentNo", hbLoan.InstallmentNo, DbType.Int32);
            parameters.Add("RemainingInstallmentNo", hbLoan.RemainingInstallmentNo, DbType.Int32);
            parameters.Add("InterestInstallmentNo", hbLoan.InterestInstallmentNo, DbType.Int32);
            parameters.Add("RemainingInterestInstallmentNo", hbLoan.RemainingInterestInstallmentNo, DbType.Int32);
            parameters.Add("PrincipalInstallmentAmount", hbLoan.PrincipalInstallmentAmount, DbType.Double);
            parameters.Add("IsPaused", hbLoan.IsPaused, DbType.Boolean);
            parameters.Add("IsRescheduled", hbLoan.IsRescheduled, DbType.Boolean);
            parameters.Add("DateAfterPeriod", hbLoan.DateAfterPeriod, DbType.DateTime);
            parameters.Add("UpdatedBy", hbLoan.UpdatedBy, DbType.String);
            parameters.Add("UpdatedDate", hbLoan.UpdatedDate, DbType.DateTime);
            parameters.Add("Id", hbLoan.Id, DbType.Int32);

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

        //Report
        public Task<List<ComLoanInfoVM>> GetComputerLoanInfo(string jobCode, int fMonthId, int tmonthId, int employeeType)
        {
            throw new NotImplementedException();
        }

        public  Task<List<MotorLoanInfoVM>> GetMotorLoanInfo(string jobCode, int fMonthId, int tmonthId, int employeeType)
        {
            throw new NotImplementedException();
        }

        public async Task<List<YearEndingMCLVM>> GetYearEndingMCLLoanInfo(string jobCode, int fMonthId, int tmonthId, int employeeType)
        {
            var query = @"
            SELECT 
                MclLoans.JobCode, 
                Employees.EmployeeName, 
                Designations.DesignationName, 
                SUM(MclInstallments.InstallmentAmount) as Total 
            FROM 
                MclLoans 
            JOIN 
                MclInstallments ON MclLoans.Id = MclInstallments.MclId
            JOIN 
                Employees ON Employees.JobCode = MclLoans.JobCode
            JOIN 
                Designations ON Employees.DesignationId = Designations.Id
            WHERE 
                MclInstallments.MonthId BETWEEN @fMonthId AND @tmonthId";

            var parameters = new DynamicParameters();
            parameters.Add("fMonthId", fMonthId, DbType.Int32);
            parameters.Add("tmonthId", tmonthId, DbType.Int32);

            // Add jobCode filter conditionally
            if (!string.IsNullOrEmpty(jobCode))
            {
                query += " AND MclLoans.JobCode = @jobCode";
                parameters.Add("jobCode", jobCode, DbType.String);
            }

            query += " GROUP BY MclLoans.JobCode, Employees.EmployeeName, Designations.DesignationName";

            using (var connection = _context.CreateConnection())
            {
                var yearEndingMCL = await connection.QueryAsync<YearEndingMCLVM>(query, parameters);
                return yearEndingMCL.ToList();
            }
        }


        public async Task<List<YearEndingCOMVM>> GetYearEndingCOMLoanInfo(string jobCode, int fMonthId, int tmonthId, int employeeType)
        {
            var query = @"
                SELECT 
                    ComLoans.JobCode, 
                    Employees.EmployeeName,
                    Designations.DesignationName, 
                    ComInstallments.InstallmentAmount AS Installment, 
                    ComInstallments.InterestAmount AS Interest
                FROM 
                    ComLoans 
                JOIN 
                    ComInstallments ON ComLoans.Id = ComInstallments.ComLoanId
                JOIN 
                    Employees ON Employees.JobCode = ComLoans.JobCode
               JOIN 
                Designations ON Employees.DesignationId = Designations.Id
                WHERE 
                    ComInstallments.MonthId BETWEEN @fMonthId AND @tmonthId";

            var parameters = new DynamicParameters();
            parameters.Add("fMonthId", fMonthId, DbType.Int32);
            parameters.Add("tmonthId", tmonthId, DbType.Int32);

            // Add jobCode filter conditionally
            if (!string.IsNullOrEmpty(jobCode))
            {
                query += " AND ComLoans.JobCode = @jobCode";
                parameters.Add("jobCode", jobCode, DbType.String);
            }

            using (var connection = _context.CreateConnection())
            {
                var yearEndingCOM = await connection.QueryAsync<YearEndingCOMVM>(query, parameters);
                return yearEndingCOM.ToList();
            }
        }


        public async Task<List<YearEndingHBIVM>> GetYearEndingHBILoanInfo(List<string> jobCodes, int fMonthId, int tMonthId, int employeeType)
        {
            var query = @"
        SELECT 
            HbLoans.JobCode,
            HbLoans.LoanTakenDate,
            SUM(CASE WHEN HblInstallments.MonthId < @fMonthId THEN HbLoans.PrincipalInstallmentAmount ELSE 0 END) AS OpeningPrincipal,
            SUM(CASE WHEN HblInstallments.MonthId < @fMonthId THEN HbLoans.InterestInstallmentAmount ELSE 0 END) AS OpeningInterest,
            SUM(CASE WHEN HblInstallments.MonthId BETWEEN @fMonthId AND @tMonthId THEN HbLoans.PrincipalInstallmentAmount ELSE 0 END) AS AdditionPrincipal,
            SUM(CASE WHEN HblInstallments.MonthId BETWEEN @fMonthId AND @tMonthId THEN HbLoans.InterestInstallmentAmount ELSE 0 END) AS AdditionInterest,
            SUM(CASE WHEN HblInstallments.MonthId <= @tMonthId THEN HbLoans.PrincipalInstallmentAmount ELSE 0 END) AS ClosingPrincipal,
            SUM(CASE WHEN HblInstallments.MonthId <= @tMonthId THEN HbLoans.InterestInstallmentAmount ELSE 0 END) AS ClosingInterest,
            SUM(HbLoans.PrincipalInstallmentAmount + HbLoans.InterestInstallmentAmount) AS TotalLoanAmount
                FROM 
                    [bgfcl].[dbo].[HbLoans] 
                INNER JOIN 
                    [bgfcl].[dbo].[HblInstallments] ON HblInstallments.JobCode = HbLoans.JobCode
                WHERE 
                    HbLoans.LoanTakenDate IS NOT NULL"; 

            //SUM(CASE WHEN HblInstallments.MonthId BETWEEN @fMonthId AND @tMonthId THEN HblInstallments.RecoveryPrincipal ELSE 0 END) AS RecoveryPrincipal,
            //SUM(CASE WHEN HblInstallments.MonthId BETWEEN @fMonthId AND @tMonthId THEN HblInstallments.RecoveryInterest ELSE 0 END) AS RecoveryInterest,
            // Handle dynamic JobCode filtering if provided
            if (jobCodes != null && jobCodes.Any())
            {
                query += " AND HbLoans.JobCode IN @jobCodes";
            }

            query += @"
                    GROUP BY 
                        HbLoans.JobCode, HbLoans.LoanTakenDate
                    ORDER BY 
                        HbLoans.JobCode, HbLoans.LoanTakenDate";

            var parameters = new DynamicParameters();
            parameters.Add("fMonthId", fMonthId, DbType.Int32);
            parameters.Add("tMonthId", tMonthId, DbType.Int32);
            if (jobCodes != null && jobCodes.Any())
            {
                parameters.Add("jobCodes", jobCodes);
            }

            using (var connection = _context.CreateConnection())
            {
                var loanData = await connection.QueryAsync<YearEndingHBIVM>(query, parameters);
                var loanList = loanData.ToList();

                // Filter the results based on the employeeType and JobCode prefix
                var filteredLoanList = loanList.Where(item =>
                    (employeeType == 1 && item.JobCode.StartsWith("F")) ||   // EmployeeType 1 for 'F' job codes
                    (employeeType == 2 && item.JobCode.StartsWith("J"))      // EmployeeType 2 for 'J' job codes
                ).ToList();

                return filteredLoanList;
            }
        }




    }
}
