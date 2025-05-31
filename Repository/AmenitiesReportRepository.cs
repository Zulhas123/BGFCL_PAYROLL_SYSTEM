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
    public class AmenitiesReportRepository: IAmenitiesReportContract
    {
        private readonly BgfclContext _context;

        public AmenitiesReportRepository(BgfclContext context)
        {
            _context = context;
        }

        public async Task<List<Amenities1>> GetAmenitiesControlSheet(int monthId, int? departmentId)
        {
            IEnumerable<Amenities1> finalAdjustments = null;
            var parameters = new DynamicParameters();
            parameters.Add("MonthID", monthId, DbType.Int32);

            // Start constructing the query
            var query = "SELECT * FROM AmenitiesReport WHERE MonthID = @MonthID";

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
                    finalAdjustments = await connection.QueryAsync<Amenities1>(query, parameters);
                }
                catch (Exception ex)
                {

                }
            }
            return finalAdjustments?.ToList() ?? new List<Amenities1>(); // Return an empty list if null
        }

        public async Task<List<Amenities1>> GetAmenitiesBankForward(int monthId, string? bank)
        {
            IEnumerable<Amenities1> finalAdjustments = null;
            var parameters = new DynamicParameters();
            parameters.Add("MonthID", monthId, DbType.Int32);
            parameters.Add("BankName", bank, DbType.String);

            // Start constructing the query
            var query = "SELECT * FROM AmenitiesReport WHERE MonthID = @MonthID And BankName = @BankName";

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    finalAdjustments = await connection.QueryAsync<Amenities1>(query, parameters);
                }
                catch (Exception ex)
                {

                }
            }
            return finalAdjustments?.ToList() ?? new List<Amenities1>(); // Return an empty list if null
        }
    }
}
