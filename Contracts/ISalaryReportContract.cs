using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ISalaryReportRepository
    {
        Task<IEnumerable<SalarySettingsOfficer>> GetSalaryReportDataAsync(int month, int year, int empType, int? department,int designation);
    }
}   
