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
    public interface ILoanContract
    {
        public Task<List<Loan>> GetLoans(string loanType);

        public Task<List<LoanHeadViewModel>> GetLoanHeadByEmpType(int employeeType, string fieldType);
        public Task<int> CreateMonthlyLoan(MonthlyLoan monthlyLoan);
        public Task<int> CreateMcLoan(McLoan mclLoan);
        public Task<List<McLoanViewModel>> GetMcLoans(int isActive);
        public Task<McLoan> GetMcLoanById(int id);
        public Task<int> UpdateMcLoan(McLoan mcLoan);
        public  Task<int> UpdateYearEndingData(YearEndingData yearEndingData);
        public Task<int> CreateComLoan(ComLoan comLoan);
        public Task<List<ComLoanViewModel>> GetComLoans(int isActive);
        public Task<ComLoan> GetComLoanById(int id);
        public Task<int> UpdateComLoan(ComLoan comLoan);
        public Task<int> CreateCarLoan(CarLoan carLoan);
        public Task<List<CarLoanViewModel>> GetCarLoans(int isActive);
        public Task<int> CreateCarLoanInstallment(CarLoanInstallment carLoanInstallment);
        public Task<List<CarLoanInstallment>> GetCarLoanInstallments(int loanId);
        public Task<List<CarInstallmentReport>> GetCarLoanInstallmentsReport(int loanId);
        public Task<int> CreateCarLoanDepreciationInstallment(DepreciationInstallment depreciationInstallment);
        public Task<List<DepreciationInstallment>> GetCarLoanDepreciationInstallments(int loanId);
        public Task<List<DepreciationInstallment>> GetCarLoanDepreciationInstallmentsByMonth(int loanId, int startMonthId, int endMonthId);
        public Task<List<CarDepreciateReport>> GetCarLoanDepreciationReport(int loanId);
        public Task<int> CreateMclInstallment(MclInstallment mclInstallment);
        public Task<List<MclInstallment>> GetMcInstallments(int loanId);
        public Task<List<MclInstallment>> GetMcInstallmentsByDate(int loanId, int fromMonthId, int toMonthId);
        public Task<MclYearEnding> GetMclYearEnding(int loanId, int yearEndingMonthId);
        public Task<int> UpdateMcLoanByProcess(McLoan mcLoan);
        public Task<int> CreateloanProcessHistory(LoanProcessHistory loanProcessHistory);
        public Task<List<LoanProcessHistory>> GetLoanProcessHistory(int monthId, int loanId);
        public Task<int> UpdateComLoanByProcess(ComLoan comLoan);
        public Task<int> CreateComLoanInstallment(ComInstallment comInstallment);
        public Task<CarLoan> GetCarLoanById(int id);
        public Task<int> UpdateCarLoanInstallment(CarLoanInstallment carLoanInstallment);
        public Task<int> UpdateCarLoanDepreciationInstallment(DepreciationInstallment depreciationInstallment);
        public Task<int> UpdateCarLoan(CarLoan carLoan);
        public Task<List<HbLoanViewModel>> GetHbLoans(int isActive);
        public Task<int> CreateHbLoan(HbLoan hbLoan);
        public Task<List<HbLoanViewModel>> GetHbLoansByLoanType(int isActive, int loanTypeId);
        public  Task<List<HbLoanViewModel>> GetHbLoansByEmployeeId(int employeeId);
        public Task<HbLoan> GetHbLoanById(int id);
        public Task<int> UpdateHbLoan(HbLoan hbLoan);
        public Task<int> CreateHbLoanInstallment(HblInstallment hblInstallment);
        public Task<int> UpdateHbLoanByProcess(HbLoan hbLoan);
        public Task<int> UpdateLoansInFinalAdjustment(string jobCode, int monthId, double instalmentAmount, int loanType);

        public Task<HblInstallment> GetInstallmentByHBLoanId_MonthId(int loanId, int monthId);
        public Task<MclInstallment> GetInstallmentByMCLoanId_MonthId(int loanId, int monthId);
        public Task<ComInstallment> GetInstallmentByComLoanId_MonthId(int loanId, int monthId);
        public Task<CarLoanInstallment> GetInstallmentByCarLoanId_MonthId(int loanId, int monthId);
        public Task<int> CreateHbLoanHistory(HbLoan hbLoan);
        public Task<int> UpdateHbLoanForReschedule(HbLoan hbLoan);

        // Report 
        public Task<List<ComLoanInfoVM>> GetComputerLoanInfo(string jobCode, int fMonthId, int tmonthId, int employeeType);
        public Task<List<MotorLoanInfoVM>> GetMotorLoanInfo(string jobCode, int fMonthId, int tmonthId, int employeeType);
        public Task<List<ComInstallment>> GetComInstallmentsByDate(int loanId, int fromMonthId, int toMonthId);
        public Task<List<HblInstallment>> GetHBLInstallmentsByDate(int loanId, int fromMonthId, int toMonthId);
        public  Task<List<ComInstallment>> GetPreviousComInstallmentsByDate(int loanId, int fromMonthId, int offsetMonthId);
        public Task<List<decimal>> GetPreviousComClosingInstallments(int loanId, int monthId);
        public  Task<ComYearEnding> GetComYearEnding(int loanId, int yearEndingMonthId);
        public Task<HblYearEnding> GethblYearEnding(int loanId, int yearEndingMonthId);
        // year ending
        public Task<List<YearEndingMCLVM>> GetYearEndingMCLLoanInfo(string jobCode, int fMonthId, int tmonthId, int employeeType);
        public Task<List<YearEndingCOMVM>> GetYearEndingCOMLoanInfo(string jobCode, int fMonthId, int tmonthId, int employeeType);
        public Task<List<YearEndingHBIVM>> GetYearEndingHBILoanInfo(List<string> jobCodes, int fMonthId, int tMonthId, int employeeType);
    }
}
