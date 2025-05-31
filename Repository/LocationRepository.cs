using Contracts;
using Dapper;
using Entities.ViewModels;
using Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class LocationRepository:ILocationContract
    {
        private readonly BgfclContext _context;

        public LocationRepository(BgfclContext context)
        {
            _context = context;
        }

        public async Task<int> CreateLocation(Location location)
        {
            int result = 0;
            var query = "INSERT INTO Locations (LocationName,IsActive,CreatedBy,CreatedDate) VALUES (@locationName,@isActive,@createdBy,@createdDate)";
            var parameters = new DynamicParameters();
            parameters.Add("locationName", location.LocationName, DbType.String);
          
            parameters.Add("isActive", location.IsActive, DbType.Boolean);
            parameters.Add("createdBy", location.CreatedBy, DbType.String);
            parameters.Add("createdDate", location.CreatedDate, DbType.DateTime);
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

        public async Task<List<Location>> GetLocations()
        {
            var query = "select * from Locations";
            using (var connection = _context.CreateConnection())
            {
                var banks = await connection.QueryAsync<Location>(query);
                return banks.ToList();
            }
        }

        public async Task<Location> GetLocationById(int locationId)
        {
            var query = "SELECT * FROM Locations where id=@id";
            using (var connection = _context.CreateConnection())
            {
                var bank = await connection.QuerySingleOrDefaultAsync<Location>(query, new { id = locationId });
                return bank;
            }
        }

        public async Task<int> UpdateLocation(Location location)
        {
            var query = "update Locations set IsActive=@isActive, LocationName = @locationName,updatedby = @updatedby, updateddate = @updateddate where id = @id";
            var parameters = new DynamicParameters();
            parameters.Add("locationName", location.LocationName, DbType.String);
            parameters.Add("isActive", location.IsActive, DbType.Boolean);
            parameters.Add("updatedby", location.UpdatedBy, DbType.String);
            parameters.Add("updateddate", location.UpdatedDate, DbType.DateTime);
            parameters.Add("id", location.Id, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteScalarAsync<int>(query, parameters);
                return result;
            }
        }

        public async Task<int> RemoveLocation(int id)
        {
            var query = "update Locations set isactive = 0 where id = @id";
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteScalarAsync<int>(query, new { id });
                return result;
            }
        }
    }
}
