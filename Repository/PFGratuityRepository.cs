using Contracts;
using Dapper;
using Entities;
using Entities.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class PFGratuityRepository: IPFGratuityContract
    {
        private readonly BgfclContext _context;

        public PFGratuityRepository(BgfclContext context)
        {
            _context = context;
        }
        public async Task<List<ProvidentFundData>> GetProvidentFund()
        {
            var query = "select p.id, p.JobCode,e.employeeName,EmpOpeningContribution,EmpCurrentYearContribution,EmpEndingContribution ,CompanyOpeningContribution,CompanyCurrentYearContribution,CompanyEndingContribution,InterestAsOpening,InterestAsEnding,InterestAsYear from ProvidentFund p INNER JOIN  Employees e on p.EmployeeId=e.id";
            using (var connection = _context.CreateConnection())
            {
                var pf = await connection.QueryAsync<ProvidentFundData>(query);
                return pf.ToList();
            }
        }
        public async Task<ProvidentFundData> GetProvidentFundById(int providentId)
        {
            var query = "SELECT * FROM ProvidentFund WHERE id=@id";
            using (var connection = _context.CreateConnection())
            {
                var pf = await connection.QuerySingleOrDefaultAsync<ProvidentFundData>(query, new { id = providentId });
                return pf;
            }
        }
        public async Task<int> UpdateProvidentFund(ProvidentFundData provident)
        {
            var query = @"
                        UPDATE ProvidentFund
                        SET 
                            InterestRate = @interestRate,
                            EmpOpeningContribution = @empOpeningContribution,
                            EmpEndingContribution = @empEndingContribution,
                            EmpSubTotal = @empSubTotal,
                            CompanyOpeningContribution = @companyOpeningContribution,
                            CompanyEndingContribution = @companyEndingContribution,
                            CompanySubTotal = @companySubTotal,
                            InterestAsOpening = @interestAsOpening,
                            InterestAsEnding = @interestAsEnding,
                            InterestAsYear = @interestAsYear,
                            TotalContribution = @totalContribution,
                            GrandTotal = @grandTotal
                        WHERE id = @id";

            var parameters = new DynamicParameters();
            parameters.Add("interestRate", provident.InterestRate);
            parameters.Add("empOpeningContribution", provident.EmpOpeningContribution);
            parameters.Add("empCurrentYearContribution", provident.EmpCurrentYearContribution);
            parameters.Add("empEndingContribution", provident.EmpEndingContribution);
            parameters.Add("empSubTotal", provident.EmpSubTotal);
            parameters.Add("companyOpeningContribution", provident.CompanyOpeningContribution);
            parameters.Add("companyCurrentYearContribution", provident.CompanyCurrentYearContribution);
            parameters.Add("companyEndingContribution", provident.CompanyEndingContribution);
            parameters.Add("companySubTotal", provident.CompanySubTotal);
            parameters.Add("interestAsOpening", provident.InterestAsOpening);
            parameters.Add("interestAsEnding", provident.InterestAsEnding);
            parameters.Add("interestAsYear", provident.InterestAsYear);
            parameters.Add("totalContribution", provident.TotalContribution);
            parameters.Add("grandTotal", provident.GrandTotal);
            parameters.Add("id", provident.Id);

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameters);
                return result; // number of affected rows
            }
        }

    }
}
