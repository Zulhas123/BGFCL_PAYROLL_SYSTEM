using Contracts;
using Dapper;
using Entities;
using Entities.ViewModels.Reports;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class ReportRepository: IReportContract
    {
        private readonly BgfclContext _context;

        public ReportRepository(BgfclContext context)
        {
            _context = context;
        }

        public async Task<List<IncomeTaxMonthlyViewModel>> GetMonthlyIncomeTax(int monthId)
        {
            IEnumerable<IncomeTaxMonthlyViewModel> monthlyIncomeTax = null;
            var parameters = new DynamicParameters();
            parameters.Add("MonthID", monthId, DbType.Int32);

            var query = "SELECT * FROM SalaryReportOF WHERE MonthID = @MonthID";

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    monthlyIncomeTax = await connection.QueryAsync<IncomeTaxMonthlyViewModel>(query, parameters);
                }
                catch (Exception ex)
                {
                    
                }
            }

            return monthlyIncomeTax?.ToList() ?? new List<IncomeTaxMonthlyViewModel>();
        }


    }
}
