using Contracts;
using Dapper;
using Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class YearlyIncomeTaxRepository:IYearlyIncomeTaxContract
    {
        private readonly BgfclContext _context;

        public YearlyIncomeTaxRepository(BgfclContext context)
        {
            _context = context;
        }

        public async Task<List<YearlyIncomeTax>> GetYearlyIncomeTex(int fromMonthId, int toMonthId, string? department, string? designation)
        {
            IEnumerable<YearlyIncomeTax> yearlyIncomeTax = null;
            var parameters = new DynamicParameters();
            parameters.Add("fMonthID", fromMonthId, DbType.Int32);
            parameters.Add("tMonthID", toMonthId, DbType.Int32);

            // Start constructing the query
            var query = @"
                        SELECT JobCode, EmployeeName, DepartmentName, DesignationName, TIN, SUM(IncomeTax) AS IncomeTax
                        FROM SalaryReportOF
                        WHERE MonthID BETWEEN @fMonthID AND @tMonthID";

            // Add department filter if specified
            if (!string.IsNullOrEmpty(department))
            {
                query += " AND DepartmentName = @DepartmentName";
                parameters.Add("DepartmentName", department, DbType.String);
            }

            // Add designation filter if specified
            if (!string.IsNullOrEmpty(designation))
            {
                query += " AND DesignationName = @DesignationName";
                parameters.Add("DesignationName", designation, DbType.String);
            }

            // Add the GROUP BY clause
            query += " GROUP BY JobCode, EmployeeName, DepartmentName, DesignationName, TIN";

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    yearlyIncomeTax = await connection.QueryAsync<YearlyIncomeTax>(query, parameters);
                }
                catch (Exception ex)
                {
                    // Log the exception if needed (e.g., log to a file or monitoring system)
                    // logger.LogError(ex, "Error occurred while fetching yearly income tax data.");

                    // Rethrow the exception to propagate it up the call stack
                    throw;
                }
            }

            return yearlyIncomeTax?.ToList() ?? new List<YearlyIncomeTax>();
        }

    }
}
