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
    public class SalaryReportRepository : ISalaryReportRepository
    {
        private readonly BgfclContext _context;

        public SalaryReportRepository(BgfclContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SalarySettingsOfficer>> GetSalaryReportDataAsync(int month, int year, int empType, int? department,int designation)
        {
            //return await _context.SalarySettingsOfficer
            //    .Where(e => e.DesignationID == designation && e.EmpStatus == empType && e.DepartmentID == department && e.Month == month && e.Year == year)
            //    .ToListAsync();
            return await GetSalaryReportDataAsync(month, year, empType, department, designation);


        }

        
    }
}
