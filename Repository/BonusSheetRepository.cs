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
    public class BonusSheetRepository:IBonusSheetContract
    {
        private readonly BgfclContext _context;

        public BonusSheetRepository(BgfclContext context)
        {
            _context = context;
        }

        public async Task<List<BonusControlSheet>> GetBonusControlSheet(int monthId, int bonus ,int employeeType, int? departmentId)
        {
            IEnumerable<BonusControlSheet> BonusControlSheet = null;
            var parameters = new DynamicParameters();
            parameters.Add("MonthId", monthId, DbType.Int32);
            parameters.Add("Bonus", bonus, DbType.Int32);
            parameters.Add("EmployeeType", employeeType, DbType.Int32);

            // Start constructing the query
            var query = "SELECT * FROM BonusSheet WHERE MonthID = @MonthId AND BonusId = @Bonus AND EmployeeType =@EmployeeType";

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
                    BonusControlSheet = await connection.QueryAsync<BonusControlSheet>(query, parameters);
                }
                catch (Exception ex)
                {

                    throw;
                }
            }

            return BonusControlSheet?.ToList() ?? new List<BonusControlSheet>();
        }


        public async Task<List<BonusControlSheet>> GetBonusBankForward(int monthId, int bonus, int employeeType, string? bank)
        {
            IEnumerable<BonusControlSheet> BonusBankForward = null;
            var parameters = new DynamicParameters();
            parameters.Add("MonthId", monthId, DbType.Int32);
            parameters.Add("Bonus", bonus, DbType.Int32);
            parameters.Add("EmployeeType", employeeType, DbType.Int32);
            parameters.Add("BankName", bank, DbType.String);

            var query = "SELECT * FROM BonusSheet WHERE MonthID = @MonthId AND BonusId = @Bonus AND EmployeeType = @EmployeeType AND BankName = @BankName";

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    BonusBankForward = await connection.QueryAsync<BonusControlSheet>(query, parameters);
                }
                catch (Exception ex)
                {
                    // Handle exception
                    throw;
                }
            }

            return BonusBankForward?.ToList() ?? new List<BonusControlSheet>();
        }



    }
}
