using Contracts;
using Dapper;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class MaritalRepository:IMaritalContract
    {
        private readonly BgfclContext _context;

        public MaritalRepository(BgfclContext context)
        {
            _context = context;
        }
        public async Task<List<Marital>> GetMaritals()
        {
            var query = "SELECT * FROM Maritals";
            using (var connection = _context.CreateConnection())
            {
                var maritals = await connection.QueryAsync<Marital>(query);
                return maritals.ToList();
            }
        }
    }
}
