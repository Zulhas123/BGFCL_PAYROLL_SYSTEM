using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IAmenitiesReportContract
    {
        public Task<List<Amenities1>> GetAmenitiesControlSheet(int monthid, int? departmentId);
        public Task<List<Amenities1>> GetAmenitiesBankForward(int monthId, string? bank);
    }
}
