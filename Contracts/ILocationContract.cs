using Entities.ViewModels;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ILocationContract
    {
        public Task<int> CreateLocation(Location location);

        public Task<List<Location>> GetLocations();

        public Task<Location> GetLocationById(int locationId);

        public Task<int> UpdateLocation(Location location);

        public Task<int> RemoveLocation(int id);
    }
}
