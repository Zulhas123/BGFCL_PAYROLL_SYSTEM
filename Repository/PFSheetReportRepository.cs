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
    public class PFSheetReportRepository:IPFSheetReportContract
    {
        private readonly BgfclContext _context;
        public PFSheetReportRepository(BgfclContext context)
        {
            _context = context;
        }

        public async Task<List<PFSheetReport>> GetPFSheet(string jobCode, int monthid, string? department)
        {
            IEnumerable<PFSheetReport> PFSheetOF = null;
            var parameters = new DynamicParameters();
            parameters.Add("MonthID", monthid, DbType.Int32);
            parameters.Add("JobCode", jobCode, DbType.String);

            // Start constructing the query
            var query = "SELECT * FROM SalaryReportOF WHERE MonthID = @MonthID AND JobCode = @JobCode";

            // Add the department filter only if department is not null or empty
            if (!string.IsNullOrEmpty(department))
            {
                query += " AND Department = @Department";
                parameters.Add("Department", department, DbType.String);
            }

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    PFSheetOF = await connection.QueryAsync<PFSheetReport>(query, parameters);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);

                }
            }
            return PFSheetOF?.ToList() ?? new List<PFSheetReport>();
        }

        public async Task<List<PFSheetReport>> GetPFSheetOF(List<string> jobCodes, int monthId, string? department)
        {
            IEnumerable<PFSheetReport> PFSheetOF = null;
            var parameters = new DynamicParameters();
            parameters.Add("MonthID", monthId, DbType.Int32);

            // Construct query with multiple job codes
            var query = "SELECT * FROM SalaryReportOF WHERE MonthID = @MonthID";

            if (jobCodes != null && jobCodes.Count > 0)
            {
                var jobCodeParams = string.Join(",", jobCodes.Select((code, index) => $"@JobCode{index}"));
                query += $" AND JobCode IN ({jobCodeParams})";

                for (int i = 0; i < jobCodes.Count; i++)
                {
                    parameters.Add($"JobCode{i}", jobCodes[i], DbType.String);
                }
            }

            if (!string.IsNullOrEmpty(department))
            {
                query += " AND Department = @Department";
                parameters.Add("Department", department, DbType.String);
            }

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    PFSheetOF = await connection.QueryAsync<PFSheetReport>(query, parameters);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return PFSheetOF?.ToList() ?? new List<PFSheetReport>();
        }


        public static List<PFSheetReport> SampleData = new List<PFSheetReport>
        {
            new PFSheetReport { JobCode = "001", EmployeeName = "John Doe", DesignationName = "Manager", DepartmentName = "HR", BasicSalary = 50000, ProvidentFund = 5000, OwnCont = 2500, CompanyCont = 2500 },
            new PFSheetReport { JobCode = "002", EmployeeName = "Jane Smith", DesignationName = "Developer", DepartmentName = "IT", BasicSalary = 60000, ProvidentFund = 6000, OwnCont = 3000, CompanyCont = 3000 },
            new PFSheetReport { JobCode = "001", EmployeeName = "John Doe", DesignationName = "Manager", DepartmentName = "HR", BasicSalary = 52000, ProvidentFund = 5200, OwnCont = 2600, CompanyCont = 2600 },
            new PFSheetReport { JobCode = "003", EmployeeName = "Mike Johnson", DesignationName = "Tester", DepartmentName = "QA", BasicSalary = 40000, ProvidentFund = 4000, OwnCont = 2000, CompanyCont = 2000 },
            new PFSheetReport { JobCode = "002", EmployeeName = "Jane Smith", DesignationName = "Developer", DepartmentName = "IT", BasicSalary = 62000, ProvidentFund = 6200, OwnCont = 3100, CompanyCont = 3100 }
        };
    }
}
