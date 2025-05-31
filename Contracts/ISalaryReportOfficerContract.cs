using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ISalaryReportOfficerContract
    {
        public Task<List<SalaryReportOfficer>> GetSalaryReportOfficer(int monthid, int? departmentId);
        public Task<List<SalaryReportJS>> GetSalaryReportJS(int monthid, int? departmentId);
        public Task<List<SalaryReportOfficer>> GetSalaryBankForwardOF(int monthId, string? bank);
        public Task<List<SalaryReportJS>> GetSalaryBankForwardJS(int monthId, string? bank);
        public Task<List<SalaryReportOfficer>> GetSalaryBankForwardJRStaff(int monthId, int? bank);
        public Task<List<SalaryReportOfficer>> GetSalaryPaySlip(string jobCode, int employeeType, int monthId, int? departmentId);
        public Task<List<SalaryReportOfficer>> GetSalaryYearPaySlip(string jobCode, int employeeType, int fmonthId, int tmonthId, int? departmentId);
        public Task<List<SalaryReportJS>> GetSalaryPaySlipJS(string jobCode, int employeeType, int monthId, int? departmentId);
        public Task<List<SalaryReportJS>> GetSalaryYearPaySlipJS(string jobCode, int employeeType, int fmonthId, int tmonthId, int? departmentId);
        public Task<List<SalaryReportOfficer>> GetYearlySalaryControlSheetOF(List<string> jobCode, int fromMonthId, int toMonthId, string? department);
        public Task<List<SalaryReportJS>> GetYearlySalaryControlSheetJS(List<string> jobCode, int fromMonthId, int toMonthId, string? department);
        public Task<List<SalaryReportOfficer>> GetSalaryControlSheetOf(List<string> jobCodes, int monthId, int? departmentId);
        public Task<List<ScSalaryReport>> GetScSalarySheetPermanent(List<string> jobCodes, int monthId);
        public Task<List<ProvidentFundData>> GetProvidentFund(int monthId);
        public Task<List<ScSalaryReportContructual>> GetScSalarySheetContructual(List<string> jobCodes, int monthId);
        public Task<List<DailyWorker>> GetScSalarySheetDailyWorker(List<string> jobCodes, int monthId);
        public Task<List<ScSalaryReportUnified>> GetAllSalarySheetUnified(List<string> jobCodes, int monthId);
        public Task<List<SalaryReportJS>> GetSalaryControlSheetJS(List<string> jobCodes, int monthId, int? departmentId);
        public Task<List<SalaryReportOfficer>> GetSalaryReportOfficerData();
        public Task<List<SalaryReportOfficer>> GetreportOfficerWithFilter(int? employeeTypeId, int? schoolId, int? roleId, int? departmentId, int? designationId);
        public Task<List<SalaryReportJS>> GetreportContractWithFilter(int? employeeTypeId, int? schoolId, int? roleId, int? departmentId, int? designationId);
        public Task<List<SalaryReportJS>> GetSalaryReportContractData();

    }
}
