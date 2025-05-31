using Contracts;
using Entities;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class GenderRepository:IGenderContract
    {
        private readonly BgfclContext _context;

        public GenderRepository(BgfclContext context)
        {
            _context = context;
        }
        public async Task<List<Gender>> GetGenders()
        {
            var query = "SELECT * FROM Genders";
            using (var connection = _context.CreateConnection())
            {
                var genders = await connection.QueryAsync<Gender>(query);
                return genders.ToList();
            }
        }
    }
}
