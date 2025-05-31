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
    public interface IAmenitiesContract
    {
        public Task<int> CreateAmenities(Amenities amenities);
        public Task<int> UpdateAmenities(Amenities amenities);
        public Task<List<AmenitiesViewModel>> GetAmenities();
        public Task<Amenities> GetAmenitiesById(int amenitiesId);
        public  Task<Amenities> GetAmenitiesByJobCode(string jobCode);
        public Task<AmenitiesReportMaster> GetAmenitiesReportMasterByMonthId(int monthId);
        public Task<int> ProcessAmenities(int monthId, int status);
        public Task<List<AmenitiesFinalAdjustmentViewModel>> GetAmenitiesFinalAdjustmentsByMonthId(int monthId);
        public Task<List<AmenitiesFinalAdjustmentViewModel>> GetAmenitiesByMonthIdAndBankId(int monthId, int? bankId);
        public Task<int> UpdateAmenitiesFinalAdjustment(AmenitiesFinalAdjustmentViewModel amenitiesFinalAdjustmentViewModel);
        public Task<int> CreateAmenitiesJournalMaster(AmenitiesJournalMaster amenitiesJournalMaster);
        public Task<int> CreateAmenitiesJournal(AmenitiesJournal amenitiesJournal);

        // reports
        public Task<List<AmenitiesControlSheetViewModel>> GetAmenitiesControlSheet(List<string> jobCodes,int monthId);
        public Task<List<AmenitiesControlSheetViewModel>> YearlyAmenitiesControlSheet(List<string> jobCodes,int fromMonthId, int toMonthId, string? department);
        public Task<List<AmenitiesControlSheetViewModel>> GeAmenitiesPaySlip(string jobCode, int monthId, int? departmentId);
        public Task<List<AmenitiesControlSheetViewModel>> YearlyAmenitiesPaySlip(string jobCode, int fromMonthId, int toMonthId, string? department);
        public Task<string> GetLastMonthAmenitiesProcess();
        public  Task<decimal> GetLastMonthAmenitiesAmount();
        public Task<AmenitiesJournalMaster> GetAmenitiesJournalMaster(int monthId, int employeeTypeId);
        public  Task<List<AmenitiesJournal>> GetAmenitiesJournalsByMasterId(int journalMasterId);
        public  Task<List<JournalAdjustmentOfficer>> JournalAdjustment(int monthId);
        public  Task<int> UpdateJournalAdjustment(JournalAdjustmentOfficer amenitiesJournal);
        public  Task<JournalAdjustmentOfficer> InsertNewamenitiesJournal(JournalAdjustmentOfficer AmenitiesJournals);
        public  Task<int> DeleteAmenitiesJournal(int id);
    }
}
