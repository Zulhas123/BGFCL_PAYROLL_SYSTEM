using Entities;
using Entities.ViewModels;
using Entities.ViewModels.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ISalarySettingContract
    {
        public Task<List<SalaryPolicySetting>> GetSalaryPolicySettings();
        public Task<int> UpdateTaxInfo(List<SalarySettingsOfficer> salarySettings);
        public Task<int> UpdateBasicSalary(SalarySettingsOfficer salarySetting, int employeeType);
        public Task<int> UpdateSalaryPolicySetting(SalaryPolicySetting salaryPolicySetting);
        public Task<List<SalarySettingViewModel>> GetSalarySettingsOfficer();
        public Task<int> CreateSalarySettingsOfficer(SalarySettingsOfficer salarySettingsOfficer);
        public Task<List<SalarySettingViewModel>> GetSalarySettingsJuniorStaff();
        public Task<List<SalarySettingViewModel>> GetSalarySettingsWorker();
        public Task<int> CreateSalarySettingsJuniorStaff(SalarySettingsJuniorStaff settingsJuniorStaff);

        public Task<SalarySettingsOfficer> GetSalarySettingsOfficerByJobCode(string jobCode);

        public Task<int> CreateMonthlySalarySettingOfficer(MonthlySalarySettingOfficer monthlySalarySettingOfficer);
        public Task<int> CreateMonthlySalaryJuniorStaff(MonthlySalarySettingJuniorStaff monthlySalarySettingJuniorStaff);
        public Task<IEnumerable<PayMode>> GetPayModes();
        public Task<int> SalaryProcess(int monthId, int employeeTypeId);
        public Task<IEnumerable<SalaryProcessViewModel>> GetSalaryProcess();
        public Task<SalaryProcessViewModel> GetSalaryProcessById(int processId);
        public Task<IEnumerable<SalaryProcessMaster>> GetSalaryProcessMaster();
        public Task<int> UpdateSalaryProcessMaster(SalaryProcessMaster process);
        public Task<int> SalaryProcesswithJobcode(int monthId, int employeeTypeId, List<string> jobCodes = null);
        public Task<List<FinalAdjustmentOfficer>> FinalAdjustmentOfficer(int monthId);
        public Task<List<JournalAdjustmentOfficer>> JournalAdjustmentOfficer(int monthId, int employeeTypeId);
        public Task<List<FinalAdjustmentJuniorStaff>> FinalAdjustmentJuniorStaff(int monthId);
        public Task<List<FinalAdjustmentOfficer>> CheckNetPayOfficer(int monthId);
        public Task<List<FinalAdjustmentJuniorStaff>> CheckNetPayJuniorStaff(int monthId);
        public  Task<List<SalaryReportOfficer>> GetGroupedSalaryReportData(int monthId, int department, int designation);
        public Task<int> UpdateFinalAdjustmentOfficer(FinalAdjustmentOfficer finalAdjustmentOfficer);
        public Task<int> UpdateFinalAdjustmentJuniorStaff(FinalAdjustmentJuniorStaff finalAdjustmentJuniorStaff);
        public Task<SalarySettingsJuniorStaff> GetSalarySettingsJuniorStaffByJobCode(string jobCode);
        public Task<int> UpdateSalarySettingsOfficer(SalarySettingsOfficer salarySettingsOfficer);
        public Task<int> UpdateSalarySettingsJuniorStaff(SalarySettingsJuniorStaff settingsJuniorStaff);
        public Task<SalaryJournalMaster> GetSalaryJournalMaster(int monthId, int employeeTypeId);
        public Task<int> CreateSalaryJournalMaster(SalaryJournalMaster salaryJournalMaster);
        public Task<int> CreateSalaryJournal(SalaryJournal salaryJournal);
        public Task<JournalAdjustmentOfficer> InsertNewSalaryJournal(JournalAdjustmentOfficer salaryJournal);
        public Task<List<SalaryJournal>> GetSalaryJournalsByMasterId(int journalMasterId);
        public Task<int> UpdateJournalAdjustmentOfficer(JournalAdjustmentOfficer salaryJournal);
        public Task<int> DeleteSalaryJournalOff(int id);

        // report
        public Task<List<MedicalFundViewModel>> GetMedicalFunds(int monthId);
        public Task<List<LoanSheetViewModel>> GetLoanSheetData(int monthId, int employeeTypeId);
        public Task<List<PFSheetViewModel>> GetPFSheetData(List<string> jobCode,int monthId, int employeeTypeId);
        public Task<List<PensionSheetViewModel>> GetPensionSheetData(int monthId, int employeeTypeId);
        public Task<List<PFDeductionViewModel>> GetPFDeductionData(List<string> jobCode, int monthId, int employeeTypeId);
        public Task<List<PensionSheetViewModel>> GetYearlyPensionSheetData(int employeeTypeId, List<string> jobCode, int fromMonthId, int toMonthId, string? department);
        public  Task<List<PFSheetViewModel>> GetYearlyPFSheetData(List<string> jobCodes,int employeeTypeId, int fromMonthId, int toMonthId, string? department);
        public Task<List<PFDeductionViewModel>> YearlyGPFDeductionData(List<string> jobCodes,int fromMonthId, int toMonthId, string? department, int employeeTypeId);
        public  Task<List<WelfareFundViewModel>> WelfarefundData(int monthId, int employeeTypeId);
        public Task<List<EmployeeUnionORClub>> EmployeeUnionORClubData(int monthId, int reportType);
        public Task<List<OfficerAssoOrClub>> OfficerAssoORClubData(int monthId, int reportType);
        public  Task<string> GetLastMonthSalaryProcess();
        public  Task<string> GetSalaryOfNetpayAsync();
        public Task<string> GetSalaryNetpayPermanent();
        public Task<Payrolldata> GetPayrollData(int monthId);
        public Task<string> GetSalaryNetpayContract();
        public Task<string> GetSalaryGrossPermanent();
        public Task<string> GetSalaryGrossContract();
        public Task<string> GetRevenueStampPermanent();
        public Task<string> GetRevenueStampContruct();
        public Task<string> GetPFPermanent();
        public Task<string> GetPFContruct();
        public Task<string> GetTotalDeductionPermanent();
        public Task<string> GetTotalDeductionContruct();
        public  Task<string> GetSalaryNetpayJS();
        public Task<List<SmsService>> GetSmsService(int monthId, int employeeTypeId);
        public Task<List<SalaryReportOfficer>> GetSalaryDataForTaxCertificate(string jobCode, int? departmentId, int fromMonthId, int toMonthId);
        public Task<IEnumerable<OverTimeViewModal>> OvertimeData(List<string>? jobcodes, int currentMonthId, string? departmentname);
        //public Task<int> SalarySettinsConfig(SalarySettingConfigDetails salarysettins);
        public Task<int> SalarySettinsConfig(List<SalarySettingConfigDetails> salarySettings);
    }
}
