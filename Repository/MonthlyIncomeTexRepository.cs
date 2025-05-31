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
    public class MonthlyIncomeTexRepository : IMonthlyIncomeTexContract
    {
        private readonly BgfclContext _context;

        public MonthlyIncomeTexRepository(BgfclContext context)
        {
            _context = context;
        }
        public async Task<List<MonthlyIncomeTex>> GetMonthlyIncomeTex(int monthId, string? department, string? designation)
        {
            IEnumerable<MonthlyIncomeTex> MonthlyIncome = null;
            var parameters = new DynamicParameters();
            parameters.Add("MonthID", monthId, DbType.Int32);


            // Start constructing the query
            var query = "SELECT * FROM SalaryReportOF WHERE MonthID = @MonthID";

            // Add the department filter only if departmentId is not null
            if (!string.IsNullOrEmpty(department))
            {
                query += " AND DepartmentName = @DepartmentName";
                parameters.Add("DepartmentName", department, DbType.String);
            }

            // Add the designation filter only if designation is not null or empty
            if (!string.IsNullOrEmpty(designation))
            {
                query += " AND DesignationName = @DesignationName";
                parameters.Add("DesignationName", designation, DbType.String);
            }

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    MonthlyIncome = await connection.QueryAsync<MonthlyIncomeTex>(query, parameters);
                }
                catch (Exception ex)
                {
                    // Handle the exception (logging, rethrowing, etc.)
                    throw;
                }
            }

            return MonthlyIncome?.ToList() ?? new List<MonthlyIncomeTex>();
        }

    }
}
