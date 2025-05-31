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
    public class ActiveStatusRepository:IActiveStatusContract
    {
        private readonly BgfclContext _context;

        public ActiveStatusRepository(BgfclContext context)
        {
            _context = context;
        }
        public async Task<List<ActiveStatus>> GetActiveStatus()
        {
            var query = "SELECT * FROM ActiveStatus";
            using (var connection = _context.CreateConnection())
            {
                var activeStatus = await connection.QueryAsync<ActiveStatus>(query);
                return activeStatus.ToList();
            }
        }
    }
}
