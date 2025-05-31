using Contracts;
using Dapper;
using Entities;
using Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class AdvanceTaxRepository:IAdvanceTaxContract
    {
        private readonly BgfclContext _context;

        public AdvanceTaxRepository(BgfclContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAdvanceTax(AdvanceTax advanceTax)
        {
            int result = 0;
            var query = "INSERT INTO AdvanceTaxes (LetterNo,Amount,Date,MonthId,CreatedBy,CreatedDate) VALUES (@letterNo,@amount,@date,@monthId,@createdBy,@createdDate)";
            var parameters = new DynamicParameters();
            parameters.Add("letterNo", advanceTax.LetterNo, DbType.String);
            parameters.Add("amount", advanceTax.Amount, DbType.Double);
            parameters.Add("date", DateTime.ParseExact(advanceTax.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture), DbType.Date);
            parameters.Add("monthId", advanceTax.MonthId, DbType.Int32);
            parameters.Add("createdBy", advanceTax.CreatedBy, DbType.String);
            parameters.Add("createdDate", advanceTax.CreatedDate, DbType.Date);
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

        public async Task<List<AdvanceTax>> GetAdvanceTaxes()
        {
            var query = "select * from AdvanceTaxes";
            using (var connection = _context.CreateConnection())
            {
                var advanceTaxes = await connection.QueryAsync<AdvanceTax>(query);
                return advanceTaxes.ToList();
            }
        }
        public async Task<AdvanceTax> GetAdvanceTaxByMonthId(int monthId)
        {
            var query = "select * from AdvanceTaxes where MonthId=@monthId";
            using (var connection = _context.CreateConnection())
            {
                var advanceTax = await connection.QuerySingleOrDefaultAsync<AdvanceTax>(query, new { monthId });
                return advanceTax;
            }
        }
        public async Task<int> RemoveAdvanceTax(int id)
        {
            var query = "delete from AdvanceTaxes where id = @id";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteScalarAsync<int>(query, new { id });
                return result;
            }
        }
    }
}
