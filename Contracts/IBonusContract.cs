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
    public interface IBonusContract
    {
        public Task<int> CreateBonus(Bonus bonus);
        public Task<List<Bonus>> GetBonus();
        public Task<List<BonusControlSheet>> GetBonusSheet();
        public Task<List<Bonus>> GetLastBonusAmount();
        public Task<Bonus> GetLastBonusName();
        public Task<IEnumerable<BonusSheetViewModal>> GetPayableBonusList();
        public Task<Bonus> GetBonusById(int id);
        public Task<int> RemoveBonus(int id);
        public Task<int> UpdateBonus(Bonus bonus);
        public Task<List<Bonus>> GetBonusByMonthId(int monthId);
        public Task<int> BonusSetting(int bonusId, int employeeTypeId);
        public Task<int> BonusProcess(int bonusId, int employeeTypeId, int fieldIndex, int religion, double basic);
        public Task<List<BonusAdjustmentViewModel>> GetBonusProcessData(int bonusId, int employeeTypeId);
        public Task<List<BonusAdjustmentViewModel>> GetAllBonusProcessData();
        public Task<List<BonusAdjustmentViewModel>> GetBonusProcessDataWithFilter(int employeeTypeId, int? schoolId, int? roleId, string? department, string? designation);
        public Task<int> UpdateBonusSheet(BonusAdjustmentViewModel bonusAdjustmentViewModel);
        public Task<List<BonusAdjustmentViewModel>> CheckBonusJournalData(int bonusId, int employeeTypeId);
        public Task<int> CreateBonusJournalMaster(BonusJournalMaster bonusJournalMaster);
        public Task<BonusJournalMaster> GetBonusJournalMaster(int monthId, int employeeTypeId, int bonusId);
        public Task<int> CreateBonusJournal(BonusJournal bonusJournal);
        public Task<List<BonusJournal>> GetBonusJournalsByMasterId(int journalMasterId);
        public Task<int> DeleteBonus(int id);
        public Task<int> CreateBonusSheet(BonusControlSheet bonusControlSheet);
        public Task<int> CreateBonusSheetData(BonusControlSheet bonusControlSheet);
        public Task<int> DeleteBonusSheet(int id, int employeeTypeId);
        //report
        public Task<List<BonusControlSheetViewModel>> GetBonusControlSheet(int monthId, int bonusId, int employeeTypeId, int? departmentId);
        public Task<List<BonusControlSheetViewModel>> GetBonusPayslip(string jobCode, int monthId, int bonusId, int employeeTypeId, int? departmentId);
        public Task<List<BonusControlSheetViewModel>> GetBonusBankForword(int monthId, int bonusId, int employeeTypeId, int? departmentId, string bankName);
        public Task<List<BonusControlSheetViewModel>> GetBonusBankForwarding(int monthId, int bonusId, int employeeTypeId, int? BankId, int? departmentId);
        public  Task<string> GetLastBonus();
        public Task<decimal> GetLastBonusAmountAsync();
    }
}
