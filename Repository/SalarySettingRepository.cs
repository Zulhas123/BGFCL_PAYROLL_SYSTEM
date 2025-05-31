using Contracts;
using Dapper;
using Entities;
using Entities.ViewModels;
using Entities.ViewModels.Reports;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Data.Common;
using System.Reflection.Emit;

namespace Repositories
{
    public class SalarySettingRepository : ISalarySettingContract
    {
        private readonly BgfclContext _context;

        public SalarySettingRepository(BgfclContext context)
        {
            _context = context;
        }

        public async Task<List<SalaryPolicySetting>> GetSalaryPolicySettings()
        {
            var query = "select * from SalaryPolicySettings";
            using (var connection = _context.CreateConnection())
            {
                var salaryPolicySettings = await connection.QueryAsync<SalaryPolicySetting>(query);
                return salaryPolicySettings.ToList();
            }
        }

        public async Task<int> UpdateSalaryPolicySetting(SalaryPolicySetting salaryPolicySetting)
        {
            var query = "update SalaryPolicySettings set " +
                "FuelAllow=@fuelAllow," +
                "SpecialBenefit = @specialBenefit," +
                "LunchRate=@lunchRate," +
                "TiffinRate=@tiffinRate," +
                "OfficerClub=@officerClub," +
                "OfficerAssociation=@officerAssociation," +
                "ShiftAllowRate=@shiftAllowRate," +
                "OtSingle=@otSingle," +
                "OtDouble=@otDouble," +
                "EmployeeClub=@employeeClub," +
                "UtilityAllow=@utilityAllow," +
                "EmployeeUnion=@employeeUnion," +
                "WelfareFund=@welfareFund," +
                "ProvidentFundRate=@providentFundRate," +
                "RevenueStamp=@revenueStamp," +
                "PensionRate=@pensionRate," +
                "Medical=@medical," +
                "UpdatedBy=@updatedBy," +
                "UpdatedDate=@updatedDate where Id=@id";
            var parameters = new DynamicParameters();
            parameters.Add("fuelAllow", salaryPolicySetting.FuelAllow, DbType.Double);
            parameters.Add("specialBenefit", salaryPolicySetting.SpecialBenefit, DbType.Double);
            parameters.Add("lunchRate", salaryPolicySetting.LunchRate, DbType.Double);
            parameters.Add("tiffinRate", salaryPolicySetting.TiffinRate, DbType.Double);
            parameters.Add("officerClub", salaryPolicySetting.OfficerClub, DbType.Double);
            parameters.Add("officerAssociation", salaryPolicySetting.OfficerAssociation, DbType.Double);
            parameters.Add("shiftAllowRate", salaryPolicySetting.ShiftAllowRate, DbType.Double);
            parameters.Add("otSingle", salaryPolicySetting.OtSingle, DbType.Double);
            parameters.Add("otDouble", salaryPolicySetting.OtDouble, DbType.Double);
            parameters.Add("employeeClub", salaryPolicySetting.EmployeeClub, DbType.Double);
            parameters.Add("utilityAllow", salaryPolicySetting.UtilityAllow, DbType.Double);
            parameters.Add("employeeUnion", salaryPolicySetting.EmployeeUnion, DbType.Double);
            parameters.Add("welfareFund", salaryPolicySetting.WelfareFund, DbType.Double);
            parameters.Add("providentFundRate", salaryPolicySetting.ProvidentFundRate, DbType.Double);
            parameters.Add("revenueStamp", salaryPolicySetting.RevenueStamp, DbType.Double);
            parameters.Add("pensionRate", salaryPolicySetting.PensionRate, DbType.Double);
            parameters.Add("medical", salaryPolicySetting.Medical, DbType.Double);
            parameters.Add("updatedBy", salaryPolicySetting.UpdatedBy, DbType.String);
            parameters.Add("updatedDate", salaryPolicySetting.UpdatedDate, DbType.DateTime);
            parameters.Add("id", salaryPolicySetting.Id, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteScalarAsync<int>(query, parameters);
                return result;
            }
        }

        public async Task<List<SalarySettingViewModel>> GetSalarySettingsOfficer()
        {
            var query = $"select SalarySettingsOfficer.JobCode,SalarySettingsOfficer.OfficerPF as GpfRate,Employees.EmployeeName, Departments.DepartmentName, Designations.DesignationName " +
                $"from SalarySettingsOfficer join employees on SalarySettingsOfficer.Jobcode = employees.Jobcode " +
                $"join Departments on Departments.Id = employees.DepartmentId join Designations on Designations.Id = Employees.DesignationId";
            using (var connection = _context.CreateConnection())
            {
                var salarySettings = await connection.QueryAsync<SalarySettingViewModel>(query);
                return salarySettings.ToList();
            }
        }

        public async Task<int> CreateSalarySettingsOfficer(SalarySettingsOfficer salarySettingsOfficer)
        {
            int result = 0;
            var query = "INSERT INTO SalarySettingsOfficer (" +
                "JobCode," +
                "BasicSalary," +
                "PersonalPay," +
                "LikeBasic," +
                "OtherSalary," +
                "EducationAllow," +
                "WashAllow," +
                "HouseRentAllowRate," +
                "HouseRentAllow," +
                "ConveyanceAllow," +
                "FamilyMedicalAllow," +
                "OfficerPF," +
                "FieldRiskAllow," +
                "ChargeAllow," +
                "DAidAllow," +
                "DeputationAllow," +
                "HouseRentReturn," +
                "DormitoryDeduction," +
                "FuelReturn," +
                "IsMemberPF," +
                "IsMemberWF," +
                "IsMemberCOS," +
                "IsMemberMedical," +
                "IsMemberOffClub," +
                "IsMemberOffAsso," +
                "PayModeId," +
                "BankId," +
                "BankBranchId," +
                "AccountNumber," +
                "MonthlyTaxDeduction," +
                "CreatedBy," +
                "CreatedDate,CME) VALUES (" +
                "@jobCode," +
                "@basicSalary," +
                "@personalPay," +
                "@likeBasic," +
                "@otherSalary," +
                "@educationAllow," +
                "@washAllow," +
                "@houseRentAllowRate," +
                "@houseRentAllow," +
                "@conveyanceAllow," +
                "@familyMedicalAllow," +
                "@officerPF," +
                "@fieldRiskAllow," +
                "@chargeAllow," +
                "@dAidAllow," +
                "@deputationAllow," +
                "@houseRentReturn," +
                "@dormitoryDeduction," +
                "@fuelReturn," +
                "@isMemberPF," +
                "@isMemberWF," +
                "@isMemberCOS," +
                "@isMemberMedical," +
                "@isMemberOffClub," +
                "@isMemberOffAsso," +
                "@payModeId," +
                "@bankId," +
                "@bankBranchId," +
                "@accountNumber," +
                "@monthlyTaxDeduction," +
                "@createdBy," +
                "@createdDate,@CME)";
            var parameters = new DynamicParameters();
            parameters.Add("jobCode", salarySettingsOfficer.JobCode, DbType.String);
            parameters.Add("basicSalary", salarySettingsOfficer.BasicSalary, DbType.Double);
            parameters.Add("personalPay", salarySettingsOfficer.PersonalPay, DbType.Double);
            parameters.Add("likeBasic", salarySettingsOfficer.LikeBasic, DbType.Double);
            parameters.Add("otherSalary", salarySettingsOfficer.OtherSalary, DbType.Double);
            parameters.Add("educationAllow", salarySettingsOfficer.EducationAllow, DbType.Double);
            parameters.Add("washAllow", salarySettingsOfficer.WashAllow, DbType.Double);
            parameters.Add("houseRentAllowRate", salarySettingsOfficer.HouseRentAllowRate, DbType.Double);
            parameters.Add("houseRentAllow", salarySettingsOfficer.HouseRentAllow, DbType.Double);
            parameters.Add("conveyanceAllow", salarySettingsOfficer.ConveyanceAllow, DbType.Double);
            parameters.Add("familyMedicalAllow", salarySettingsOfficer.FamilyMedicalAllow, DbType.Double);
            parameters.Add("officerPF", salarySettingsOfficer.OfficerPF, DbType.Double);
            parameters.Add("fieldRiskAllow", salarySettingsOfficer.FieldRiskAllow, DbType.Double);
            parameters.Add("chargeAllow", salarySettingsOfficer.DAidAllow, DbType.Double);
            parameters.Add("dAidAllow", salarySettingsOfficer.DAidAllow, DbType.Double);
            parameters.Add("deputationAllow", salarySettingsOfficer.DeputationAllow, DbType.Double);
            parameters.Add("houseRentReturn", salarySettingsOfficer.HouseRentReturn, DbType.Double);
            parameters.Add("dormitoryDeduction", salarySettingsOfficer.DormitoryDeduction, DbType.Double);
            parameters.Add("fuelReturn", salarySettingsOfficer.FuelReturn, DbType.Double);
            parameters.Add("isMemberPF", salarySettingsOfficer.IsMemberPF, DbType.Boolean);
            parameters.Add("isMemberWF", salarySettingsOfficer.IsMemberWF, DbType.Boolean);
            parameters.Add("isMemberCOS", salarySettingsOfficer.IsMemberCOS, DbType.Boolean);
            parameters.Add("isMemberMedical", salarySettingsOfficer.IsMemberMedical, DbType.Boolean);
            parameters.Add("isMemberOffClub", salarySettingsOfficer.IsMemberOffClub, DbType.Boolean);
            parameters.Add("isMemberOffAsso", salarySettingsOfficer.IsMemberOffAsso, DbType.Boolean);
            parameters.Add("payModeId", salarySettingsOfficer.PayModeId, DbType.String);
            parameters.Add("bankId", salarySettingsOfficer.BankId, DbType.Int32);
            parameters.Add("bankBranchId", salarySettingsOfficer.BankBranchId, DbType.Int32);
            parameters.Add("accountNumber", salarySettingsOfficer.AccountNumber, DbType.String);
            parameters.Add("monthlyTaxDeduction", salarySettingsOfficer.MonthlyTaxDeduction, DbType.Decimal);
            parameters.Add("createdBy", salarySettingsOfficer.CreatedBy, DbType.String);
            parameters.Add("createdDate", salarySettingsOfficer.CreatedDate, DbType.Date);
            parameters.Add("CME", salarySettingsOfficer.CME, DbType.Double);

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    result = await connection.ExecuteAsync(query, parameters);
                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }

        public async Task<int> UpdateSalarySettingsOfficer(SalarySettingsOfficer salarySettingsOfficer)
        {
            int result = 0;
            var query = "update SalarySettingsOfficer set " +
                "BasicSalary=@BasicSalary," +
                "PersonalPay=@PersonalPay," +
                "LikeBasic=@LikeBasic," +
                "OtherSalary=@OtherSalary," +
                "EducationAllow=@EducationAllow," +
                "WashAllow=@WashAllow," +
                "HouseRentAllowRate=@HouseRentAllowRate," +
                "HouseRentAllow=@HouseRentAllow," +
                "ConveyanceAllow=@ConveyanceAllow," +
                "FamilyMedicalAllow=@FamilyMedicalAllow," +
                "OfficerPF=@OfficerPF," +
                "FieldRiskAllow=@FieldRiskAllow," +
                "ChargeAllow=@ChargeAllow," +
                "DAidAllow=@DAidAllow," +
                "DeputationAllow=@DeputationAllow," +
                "HouseRentReturn=@HouseRentReturn," +
                "DormitoryDeduction=@DormitoryDeduction," +
                "FuelReturn=@FuelReturn," +
                "IsMemberPF=@IsMemberPF," +
                "IsMemberWF=@IsMemberWF," +
                "IsMemberCOS=@IsMemberCOS," +
                "IsMemberMedical=@IsMemberMedical," +
                "IsMemberOffClub=@IsMemberOffClub," +
                "IsMemberOffAsso=@IsMemberOffAsso," +
                "PayModeId=@PayModeId," +
                "BankId=@BankId," +
                "BankBranchId=@BankBranchId," +
                "AccountNumber=@AccountNumber," +
                "MonthlyTaxDeduction=@MonthlyTaxDeduction," +
                "CME=@CME," +
                "UpdatedBy=@UpdatedBy," +
                "UpdatedDate=@UpdatedDate where JobCode=@JobCode";
            var parameters = new DynamicParameters();

            parameters.Add("BasicSalary", salarySettingsOfficer.BasicSalary, DbType.Double);
            parameters.Add("PersonalPay", salarySettingsOfficer.PersonalPay, DbType.Double);
            parameters.Add("LikeBasic", salarySettingsOfficer.LikeBasic, DbType.Double);
            parameters.Add("OtherSalary", salarySettingsOfficer.OtherSalary, DbType.Double);
            parameters.Add("EducationAllow", salarySettingsOfficer.EducationAllow, DbType.Double);
            parameters.Add("WashAllow", salarySettingsOfficer.WashAllow, DbType.Double);
            parameters.Add("HouseRentAllowRate", salarySettingsOfficer.HouseRentAllowRate, DbType.Double);
            parameters.Add("HouseRentAllow", salarySettingsOfficer.HouseRentAllow, DbType.Double);
            parameters.Add("ConveyanceAllow", salarySettingsOfficer.ConveyanceAllow, DbType.Double);
            parameters.Add("FamilyMedicalAllow", salarySettingsOfficer.FamilyMedicalAllow, DbType.Double);
            parameters.Add("OfficerPF", salarySettingsOfficer.OfficerPF, DbType.Double);
            parameters.Add("FieldRiskAllow", salarySettingsOfficer.FieldRiskAllow, DbType.Double);
            parameters.Add("ChargeAllow", salarySettingsOfficer.ChargeAllow, DbType.Double);
            parameters.Add("DAidAllow", salarySettingsOfficer.DAidAllow, DbType.Double);
            parameters.Add("DeputationAllow", salarySettingsOfficer.DeputationAllow, DbType.Double);
            parameters.Add("HouseRentReturn", salarySettingsOfficer.HouseRentReturn, DbType.Double);
            parameters.Add("DormitoryDeduction", salarySettingsOfficer.DormitoryDeduction, DbType.Double);
            parameters.Add("FuelReturn", salarySettingsOfficer.FuelReturn, DbType.Double);
            parameters.Add("IsMemberPF", salarySettingsOfficer.IsMemberPF, DbType.Boolean);
            parameters.Add("IsMemberWF", salarySettingsOfficer.IsMemberWF, DbType.Boolean);
            parameters.Add("IsMemberCOS", salarySettingsOfficer.IsMemberCOS, DbType.Boolean);
            parameters.Add("IsMemberMedical", salarySettingsOfficer.IsMemberMedical, DbType.Boolean);
            parameters.Add("IsMemberOffClub", salarySettingsOfficer.IsMemberOffClub, DbType.Boolean);
            parameters.Add("IsMemberOffAsso", salarySettingsOfficer.IsMemberOffAsso, DbType.Boolean);
            parameters.Add("PayModeId", salarySettingsOfficer.PayModeId, DbType.String);
            parameters.Add("BankId", salarySettingsOfficer.BankId, DbType.Int32);
            parameters.Add("BankBranchId", salarySettingsOfficer.BankBranchId, DbType.Int32);
            parameters.Add("AccountNumber", salarySettingsOfficer.AccountNumber, DbType.String);
            parameters.Add("MonthlyTaxDeduction", salarySettingsOfficer.MonthlyTaxDeduction, DbType.Double);
            parameters.Add("CME", salarySettingsOfficer.CME, DbType.Double);
            parameters.Add("UpdatedBy", salarySettingsOfficer.UpdatedBy, DbType.String);
            parameters.Add("UpdatedDate", salarySettingsOfficer.UpdatedDate, DbType.Date);
            parameters.Add("JobCode", salarySettingsOfficer.JobCode, DbType.String);

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    result = await connection.ExecuteAsync(query, parameters);
                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }

        public async Task<int> UpdateTaxInfo(List<SalarySettingsOfficer> salarySettings)
        {
            int result = 0;
            var query = "update SalarySettingsOfficer set MonthlyTaxDeduction=0";

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    result = await connection.ExecuteAsync(query);
                }
                catch (Exception ex)
                {

                }
                if (result > 0)
                {
                    if (salarySettings.Count > 0)
                    {
                        foreach (var item in salarySettings)
                        {
                            query = "update SalarySettingsOfficer set MonthlyTaxDeduction=@MonthlyTaxDeduction where JobCode=@JobCode";
                            var parameters = new DynamicParameters();
                            parameters.Add("MonthlyTaxDeduction", item.MonthlyTaxDeduction, DbType.Decimal);
                            parameters.Add("JobCode", item.JobCode, DbType.String);
                            await connection.ExecuteAsync(query, parameters);
                        }
                    }
                }
            }
            return result;
        }

        public async Task<int> UpdateBasicSalary(SalarySettingsOfficer salarySetting, int employeeType)
        {
            int result = 0;
            string query = "";
            if (employeeType == 1)
            {
                query = "update SalarySettingsOfficer set BasicSalary=@BasicSalary where JobCode=@JobCode";
            }
            else
            {
                query = "update SalarySettingsJuniorStaff set BasicSalary=@BasicSalary where JobCode=@JobCode";
            }

            var parameters = new DynamicParameters();
            parameters.Add("BasicSalary", salarySetting.BasicSalary, DbType.Decimal);
            parameters.Add("JobCode", salarySetting.JobCode, DbType.String);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    result = await connection.ExecuteAsync(query, parameters);
                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }


        public async Task<List<SalarySettingViewModel>> GetSalarySettingsJuniorStaff()
        {
            var query = $"select SalarySettingsJuniorStaff.JobCode,SalarySettingsJuniorStaff.ProvidentFund as GpfRate,Employees.EmployeeName, Departments.DepartmentName, Designations.DesignationName " +
                $"from SalarySettingsJuniorStaff join employees on SalarySettingsJuniorStaff.Jobcode = employees.Jobcode " +
                $"join Departments on Departments.Id = employees.DepartmentId join Designations on Designations.Id = Employees.DesignationId";
            using (var connection = _context.CreateConnection())
            {
                var salarySettings = await connection.QueryAsync<SalarySettingViewModel>(query);
                return salarySettings.ToList();
            }
        }
        public async Task<List<SalarySettingViewModel>> GetSalarySettingsWorker()
        {
            var query = $"select SalarySettingsJuniorStaff.JobCode,SalarySettingsJuniorStaff.ProvidentFund as GpfRate,Employees.EmployeeName, Departments.DepartmentName, Designations.DesignationName from SalarySettingsJuniorStaff join employees on SalarySettingsJuniorStaff.Jobcode = employees.Jobcode join Departments on Departments.Id = employees.DepartmentId join Designations on Designations.Id = Employees.DesignationId  where Employees.EmployeeTypeId=3";
            using (var connection = _context.CreateConnection())
            {
                var salarySettings = await connection.QueryAsync<SalarySettingViewModel>(query);
                return salarySettings.ToList();
            }
        }

        public async Task<SalarySettingsJuniorStaff> GetSalarySettingsJuniorStaffByJobCode(string jobCode)
        {
            var query = $"select * from SalarySettingsJuniorStaff where jobcode=@jobCode";
            using (var connection = _context.CreateConnection())
            {
                var salarySettings = await connection.QuerySingleOrDefaultAsync<SalarySettingsJuniorStaff>(query, new { jobCode });
                return salarySettings;
            }
        }
        public async Task<int> CreateSalarySettingsJuniorStaff(SalarySettingsJuniorStaff settingsJuniorStaff)
        {
            int result = 0;
            var query = "INSERT INTO SalarySettingsJuniorStaff (" +
                "JobCode," +
                "BasicSalary," +
                "PersonalPay," +
                "ConvenienceAllow," +
                "OtherSalary," +
                "HouseRentAllowRate," +
                "HouseRentAllow," +
                "FamilyMedicalAllow," +
                "FuelReturn," +
                "EducationAllow," +
                "FieldAllow," +
                "DormitoryDeduction," +
                "ProvidentFund," +
                "UtilityReturn," +
                "IsMemberPF," +
                "IsMemberWF," +
                "IsMemberCOS," +
                "IsMemberEmpClub," +
                "IsMemberEmpUnion," +
                "PayModeId," +
                "BankId," +
                "BankBranchId," +
                "AccountNumber," +
                "ClassTeacherAllow," +
                "ArrearAllow," +
                "SpecialBenefits," +
                "RevenueStamp," +
                "Per_attendence," +
                "Festival_bonus," +
                "Is_Daily_Worker," +
                "CreatedBy," +
                "CreatedDate) VALUES (" +
                "@jobCode," +
                "@basicSalary," +
                "@personalPay," +
                "@convenienceAllow," +
                "@otherSalary," +
                "@houseRentAllowRate," +
                "@houseRentAllow," +
                "@familyMedicalAllow," +
                "@fuelReturn," +
                "@educationAllow," +
                "@fieldAllow," +
                "@dormitoryDeduction," +
                "@providentFund," +
                "@utilityReturn," +
                "@isMemberPF," +
                "@isMemberWF," +
                "@isMemberCOS," +
                "@isMemberEmpClub," +
                "@isMemberEmpUnion," +
                "@payModeId," +
                "@bankId," +
                "@bankBranchId," +
                "@accountNumber," +
                "@classTeacherAllow," +
                "@arrearAllow," +
                "@specialBenefits," +
                "@revenueStamp," +
                "@per_attendence," +
                "@festival_bonus," +
                "@is_Daily_Worker," +
                "@createdBy," +
                "@createdDate)";
            var parameters = new DynamicParameters();
            parameters.Add("jobCode", settingsJuniorStaff.JobCode, DbType.String);
            parameters.Add("basicSalary",
               double.IsNaN(settingsJuniorStaff.BasicSalary) || double.IsInfinity(settingsJuniorStaff.BasicSalary)
               ? 0.0 : settingsJuniorStaff.BasicSalary,
               DbType.Double);

            parameters.Add("personalPay",
                double.IsNaN(settingsJuniorStaff.PersonalPay) || double.IsInfinity(settingsJuniorStaff.PersonalPay)
                ? 0.0 : settingsJuniorStaff.PersonalPay,
                DbType.Double);

            parameters.Add("convenienceAllow",
            double.IsNaN(settingsJuniorStaff.ConvenienceAllow) || double.IsInfinity(settingsJuniorStaff.ConvenienceAllow)? 0.0: settingsJuniorStaff.ConvenienceAllow,DbType.Double);

            parameters.Add("otherSalary",
             double.IsNaN(settingsJuniorStaff.OtherSalary) || double.IsInfinity(settingsJuniorStaff.OtherSalary)
             ? 0.0 : settingsJuniorStaff.OtherSalary,
             DbType.Double);

            parameters.Add("houseRentAllowRate",
                double.IsNaN(settingsJuniorStaff.HouseRentAllowRate) || double.IsInfinity(settingsJuniorStaff.HouseRentAllowRate)
                ? 0.0 : settingsJuniorStaff.HouseRentAllowRate,
                DbType.Double);

            parameters.Add("houseRentAllow",
                double.IsNaN(settingsJuniorStaff.HouseRentAllow) || double.IsInfinity(settingsJuniorStaff.HouseRentAllow)
                ? 0.0 : settingsJuniorStaff.HouseRentAllow,
                DbType.Double);

            parameters.Add("familyMedicalAllow",
                double.IsNaN(settingsJuniorStaff.FamilyMedicalAllow) || double.IsInfinity(settingsJuniorStaff.FamilyMedicalAllow)
                ? 0.0 : settingsJuniorStaff.FamilyMedicalAllow,
                DbType.Double);

            parameters.Add("fuelReturn",
                double.IsNaN(settingsJuniorStaff.FuelReturn) || double.IsInfinity(settingsJuniorStaff.FuelReturn)
                ? 0.0 : settingsJuniorStaff.FuelReturn,
                DbType.Double);

            parameters.Add("educationAllow",
                double.IsNaN(settingsJuniorStaff.EducationAllow) || double.IsInfinity(settingsJuniorStaff.EducationAllow)
                ? 0.0 : settingsJuniorStaff.EducationAllow,
                DbType.Double);

            parameters.Add("fieldAllow",
                double.IsNaN(settingsJuniorStaff.FieldAllow) || double.IsInfinity(settingsJuniorStaff.FieldAllow)
                ? 0.0 : settingsJuniorStaff.FieldAllow,
                DbType.Double);

            parameters.Add("dormitoryDeduction",
                double.IsNaN(settingsJuniorStaff.DormitoryDeduction) || double.IsInfinity(settingsJuniorStaff.DormitoryDeduction)
                ? 0.0 : settingsJuniorStaff.DormitoryDeduction,
                DbType.Double);

            parameters.Add("providentFund",
                double.IsNaN(settingsJuniorStaff.ProvidentFund) || double.IsInfinity(settingsJuniorStaff.ProvidentFund)
                ? 0.0 : settingsJuniorStaff.ProvidentFund,
                DbType.Double);

            parameters.Add("utilityReturn",
                double.IsNaN(settingsJuniorStaff.UtilityReturn) || double.IsInfinity(settingsJuniorStaff.UtilityReturn)
                ? 0.0 : settingsJuniorStaff.UtilityReturn,
                DbType.Double);

            parameters.Add("isMemberPF", settingsJuniorStaff.IsMemberPF, DbType.Boolean);
            parameters.Add("isMemberWF", settingsJuniorStaff.IsMemberWF, DbType.Boolean);

            parameters.Add("isMemberCOS", settingsJuniorStaff.IsMemberCOS, DbType.Boolean);
            parameters.Add("isMemberEmpClub", settingsJuniorStaff.IsMemberEmpClub, DbType.Boolean);
            parameters.Add("isMemberEmpUnion", settingsJuniorStaff.IsMemberEmpUnion, DbType.Boolean);
            parameters.Add("payModeId", settingsJuniorStaff.PayModeId, DbType.String);
            parameters.Add("bankId", settingsJuniorStaff.BankId, DbType.Int32);

            parameters.Add("bankBranchId", settingsJuniorStaff.BankBranchId, DbType.Int32);
            parameters.Add("accountNumber", settingsJuniorStaff.AccountNumber, DbType.String);
            parameters.Add("ClassTeacherAllow",
            double.IsNaN(settingsJuniorStaff.ClassTeacherAllow) || double.IsInfinity(settingsJuniorStaff.ClassTeacherAllow)
            ? 0.0 : settingsJuniorStaff.ClassTeacherAllow,
            DbType.Double);

            parameters.Add("ArrearAllow",
                double.IsNaN(settingsJuniorStaff.ArrearAllow) || double.IsInfinity(settingsJuniorStaff.ArrearAllow)
                ? 0.0 : settingsJuniorStaff.ArrearAllow,
                DbType.Double);

            parameters.Add("SpecialBenefits",
                double.IsNaN(settingsJuniorStaff.SpecialBenefits) || double.IsInfinity(settingsJuniorStaff.SpecialBenefits)
                ? 0.0 : settingsJuniorStaff.SpecialBenefits,
                DbType.Double); 
            parameters.Add("RevenueStamp", settingsJuniorStaff.RevenueStamp, DbType.Double);
            parameters.Add("Per_attendence", settingsJuniorStaff.per_attendence, DbType.Double);
            parameters.Add("Festival_bonus", settingsJuniorStaff.Festival_bonus, DbType.Double);
            parameters.Add("Is_Daily_Worker", settingsJuniorStaff.Is_Daily_Worker, DbType.Boolean);
            parameters.Add("createdBy", settingsJuniorStaff.CreatedBy, DbType.String);
            parameters.Add("createdDate", settingsJuniorStaff.CreatedDate, DbType.Date);

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    result = await connection.ExecuteAsync(query, parameters);
                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }

        public async Task<int> UpdateSalarySettingsJuniorStaff(SalarySettingsJuniorStaff settingsJuniorStaff)
        {
            int result = 0;
            var query = "update SalarySettingsJuniorStaff set " +
                "BasicSalary=@BasicSalary," +
                "PersonalPay=@PersonalPay," +
                "ConvenienceAllow=@ConvenienceAllow," +
                "OtherSalary=@OtherSalary," +
                "HouseRentAllowRate=@HouseRentAllowRate," +
                "HouseRentAllow=@HouseRentAllow," +
                "FamilyMedicalAllow=@FamilyMedicalAllow," +
                "FuelReturn=@FuelReturn," +
                "EducationAllow=@EducationAllow," +
                "FieldAllow=@FieldAllow," +
                "DormitoryDeduction=@DormitoryDeduction," +
                "ProvidentFund=@ProvidentFund," +
                "UtilityReturn=@UtilityReturn," +
                "IsMemberPF=@IsMemberPF," +
                "IsMemberWF=@IsMemberWF," +
                "IsMemberCOS=@IsMemberCOS," +
                "IsMemberEmpClub=@IsMemberEmpClub," +
                "IsMemberEmpUnion=@IsMemberEmpUnion," +
                "PayModeId=@PayModeId," +
                "BankId=@BankId," +
                "BankBranchId=@BankBranchId," +
                "AccountNumber=@AccountNumber," +
                "ArrearAllow=@ArrearAllow," +
                "ClassTeacherAllow=@ClassTeacherAllow," +
                "SpecialBenefits=@SpecialBenefits," +
                "RevenueStamp=@RevenueStamp," +
                "Per_attendence=@per_attendence," +
                "Is_Daily_Worker=@is_Daily_Worker," +
                "Festival_bonus=@festival_bonus," +
                "Eidul_Fitar=@eidul_fitar," +
                "Eidul_Ajha=@eidul_ajha," +
                "Baishakhi_bonus=@baishakhi," +
                "UpdatedBy=@UpdatedBy," +
                "UpdatedDate=@UpdatedDate where JobCode=@JobCode";

            var parameters = new DynamicParameters();

            parameters.Add("basicSalary",
                double.IsNaN(settingsJuniorStaff.BasicSalary) || double.IsInfinity(settingsJuniorStaff.BasicSalary)
                ? 0.0 : settingsJuniorStaff.BasicSalary,
                DbType.Double);

            parameters.Add("personalPay",
                double.IsNaN(settingsJuniorStaff.PersonalPay) || double.IsInfinity(settingsJuniorStaff.PersonalPay)
                ? 0.0 : settingsJuniorStaff.PersonalPay,
                DbType.Double);

            parameters.Add("convenienceAllow",
            double.IsNaN(settingsJuniorStaff.ConvenienceAllow) || double.IsInfinity(settingsJuniorStaff.ConvenienceAllow) ? 0.0 : settingsJuniorStaff.ConvenienceAllow, DbType.Double);

            parameters.Add("otherSalary",
             double.IsNaN(settingsJuniorStaff.OtherSalary) || double.IsInfinity(settingsJuniorStaff.OtherSalary)
             ? 0.0 : settingsJuniorStaff.OtherSalary,
             DbType.Double);

            parameters.Add("houseRentAllowRate",
                double.IsNaN(settingsJuniorStaff.HouseRentAllowRate) || double.IsInfinity(settingsJuniorStaff.HouseRentAllowRate)
                ? 0.0 : settingsJuniorStaff.HouseRentAllowRate,
                DbType.Double);

            parameters.Add("houseRentAllow",
                double.IsNaN(settingsJuniorStaff.HouseRentAllow) || double.IsInfinity(settingsJuniorStaff.HouseRentAllow)
                ? 0.0 : settingsJuniorStaff.HouseRentAllow,
                DbType.Double);

            parameters.Add("familyMedicalAllow",
                double.IsNaN(settingsJuniorStaff.FamilyMedicalAllow) || double.IsInfinity(settingsJuniorStaff.FamilyMedicalAllow)
                ? 0.0 : settingsJuniorStaff.FamilyMedicalAllow,
                DbType.Double);

            parameters.Add("fuelReturn",
                double.IsNaN(settingsJuniorStaff.FuelReturn) || double.IsInfinity(settingsJuniorStaff.FuelReturn)
                ? 0.0 : settingsJuniorStaff.FuelReturn,
                DbType.Double);

            parameters.Add("educationAllow",
                double.IsNaN(settingsJuniorStaff.EducationAllow) || double.IsInfinity(settingsJuniorStaff.EducationAllow)
                ? 0.0 : settingsJuniorStaff.EducationAllow,
                DbType.Double);

            parameters.Add("fieldAllow",
                double.IsNaN(settingsJuniorStaff.FieldAllow) || double.IsInfinity(settingsJuniorStaff.FieldAllow)
                ? 0.0 : settingsJuniorStaff.FieldAllow,
                DbType.Double);

            parameters.Add("dormitoryDeduction",
                double.IsNaN(settingsJuniorStaff.DormitoryDeduction) || double.IsInfinity(settingsJuniorStaff.DormitoryDeduction)
                ? 0.0 : settingsJuniorStaff.DormitoryDeduction,
                DbType.Double);

            parameters.Add("providentFund",
                double.IsNaN(settingsJuniorStaff.ProvidentFund) || double.IsInfinity(settingsJuniorStaff.ProvidentFund)
                ? 0.0 : settingsJuniorStaff.ProvidentFund,
                DbType.Double);

            parameters.Add("utilityReturn",
                double.IsNaN(settingsJuniorStaff.UtilityReturn) || double.IsInfinity(settingsJuniorStaff.UtilityReturn)
                ? 0.0 : settingsJuniorStaff.UtilityReturn,
                DbType.Double);
            parameters.Add("IsMemberPF", settingsJuniorStaff.IsMemberPF, DbType.Boolean);
            parameters.Add("IsMemberWF", settingsJuniorStaff.IsMemberWF, DbType.Boolean);

            parameters.Add("IsMemberCOS", settingsJuniorStaff.IsMemberCOS, DbType.Boolean);
            parameters.Add("IsMemberEmpClub", settingsJuniorStaff.IsMemberEmpClub, DbType.Boolean);
            parameters.Add("IsMemberEmpUnion", settingsJuniorStaff.IsMemberEmpUnion, DbType.Boolean);
            parameters.Add("PayModeId", settingsJuniorStaff.PayModeId, DbType.String);
            parameters.Add("BankId", settingsJuniorStaff.BankId, DbType.Int32);

            parameters.Add("BankBranchId", settingsJuniorStaff.BankBranchId, DbType.Int32);
            parameters.Add("AccountNumber", settingsJuniorStaff.AccountNumber, DbType.String);
            parameters.Add("ClassTeacherAllow",
           double.IsNaN(settingsJuniorStaff.ClassTeacherAllow) || double.IsInfinity(settingsJuniorStaff.ClassTeacherAllow)
           ? 0.0 : settingsJuniorStaff.ClassTeacherAllow,
           DbType.Double);

            parameters.Add("ArrearAllow",
                double.IsNaN(settingsJuniorStaff.ArrearAllow) || double.IsInfinity(settingsJuniorStaff.ArrearAllow)
                ? 0.0 : settingsJuniorStaff.ArrearAllow,
                DbType.Double);

            parameters.Add("SpecialBenefits",
                double.IsNaN(settingsJuniorStaff.SpecialBenefits) || double.IsInfinity(settingsJuniorStaff.SpecialBenefits)
                ? 0.0 : settingsJuniorStaff.SpecialBenefits,
                DbType.Double);
            parameters.Add("RevenueStamp", settingsJuniorStaff.RevenueStamp, DbType.Double);
            parameters.Add("Per_attendence", settingsJuniorStaff.per_attendence, DbType.Double);
            parameters.Add("Festival_bonus", settingsJuniorStaff.Festival_bonus, DbType.Double);
            parameters.Add("Is_Daily_Worker", settingsJuniorStaff.Is_Daily_Worker, DbType.Boolean);
            decimal eidulFitar = decimal.TryParse(settingsJuniorStaff.eidul_fitar?.ToString(), out var efVal)
            ? Math.Round(efVal, 2)
            : 0;
            parameters.Add("eidul_fitar", eidulFitar, DbType.Decimal);

            decimal eidulAjha = decimal.TryParse(settingsJuniorStaff.eidul_ajha?.ToString(), out var eaVal)
                ? Math.Round(eaVal, 2)
                : 0;
            parameters.Add("eidul_ajha", eidulAjha, DbType.Decimal);

            decimal baishakhi = decimal.TryParse(settingsJuniorStaff.baishakhi?.ToString(), out var bVal)
                ? Math.Round(bVal, 2)
                : 0;
            parameters.Add("baishakhi", baishakhi, DbType.Decimal);

            parameters.Add("UpdatedBy", settingsJuniorStaff.UpdatedBy, DbType.String);
            parameters.Add("UpdatedDate", settingsJuniorStaff.UpdatedDate, DbType.Date);
            parameters.Add("JobCode", settingsJuniorStaff.JobCode, DbType.String);

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    result = await connection.ExecuteAsync(query, parameters);
                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }

        public async Task<SalarySettingsOfficer> GetSalarySettingsOfficerByJobCode(string jobCode)
        {
            var query = "SELECT * FROM SalarySettingsOfficer where JobCode = @jobCode";
            using (var connection = _context.CreateConnection())
            {
                var salarySetting = await connection.QuerySingleOrDefaultAsync<SalarySettingsOfficer>(query, new { jobCode });
                return salarySetting == null ? new SalarySettingsOfficer() : salarySetting;
            }
        }

        public async Task<int> CreateMonthlySalarySettingOfficer(MonthlySalarySettingOfficer monthlySalarySettingOfficer)
        {
            int result = 0;
            var parameters = new DynamicParameters();
            parameters.Add("EmployeeID", monthlySalarySettingOfficer.JobCode, DbType.String);
            parameters.Add("MonthID", monthlySalarySettingOfficer.MonthId, DbType.Int32);
            parameters.Add("WorkDays", monthlySalarySettingOfficer.WorkDays, DbType.Int32);
            parameters.Add("ArrearSalary", monthlySalarySettingOfficer.ArrearSalary, DbType.Double);
            parameters.Add("AdvanceSalary", monthlySalarySettingOfficer.AdvanceSalary, DbType.Double);
            parameters.Add("OtherAllow", monthlySalarySettingOfficer.OtherAllow, DbType.Double);
            parameters.Add("TMBill", monthlySalarySettingOfficer.TMBill, DbType.Double);
            parameters.Add("HospitalBill", monthlySalarySettingOfficer.HospitalBill, DbType.Double);
            parameters.Add("SpecialDeduction", monthlySalarySettingOfficer.SpecialDeduction, DbType.Double);
            parameters.Add("AdvanceDeduction", monthlySalarySettingOfficer.AdvanceDeduction, DbType.Double);
            parameters.Add("OtherDeduction", monthlySalarySettingOfficer.OtherDeduction, DbType.Double);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    result = await connection.ExecuteScalarAsync<int>("spInsertMonthlySalarySettingOF", parameters, commandType: CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }

        public async Task<int> CreateMonthlySalaryJuniorStaff(MonthlySalarySettingJuniorStaff monthlySalarySettingJuniorStaff)
        {
            int result = 0;
            var parameters = new DynamicParameters();
            parameters.Add("EmployeeID", monthlySalarySettingJuniorStaff.JobCode, DbType.String);
            parameters.Add("MonthID", monthlySalarySettingJuniorStaff.MonthId, DbType.Int32);
            parameters.Add("WorkDays", monthlySalarySettingJuniorStaff.WorkDays, DbType.Int32);
            parameters.Add("NumberOfShift", monthlySalarySettingJuniorStaff.NumberOfShift, DbType.Int32);
            parameters.Add("ArrearSalary", monthlySalarySettingJuniorStaff.ArrearSalary, DbType.Double);
            parameters.Add("OtSingle", monthlySalarySettingJuniorStaff.OtSingle, DbType.Double);
            parameters.Add("OtDouble", monthlySalarySettingJuniorStaff.OtDouble, DbType.Double);
            parameters.Add("OtherAllow", monthlySalarySettingJuniorStaff.OtherAllow, DbType.Double);
            parameters.Add("AdvanceDeduction", monthlySalarySettingJuniorStaff.AdvanceDeduction, DbType.Double);
            parameters.Add("OtherDeduction", monthlySalarySettingJuniorStaff.OtherDeduction, DbType.Double);
            parameters.Add("SpecialDeduction", monthlySalarySettingJuniorStaff.SpecialDeduction, DbType.Double);
            parameters.Add("HospitalDeduction", monthlySalarySettingJuniorStaff.HospitalDeduction, DbType.Double);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    result = await connection.ExecuteScalarAsync<int>("spInsertMonthlySalarySettingJS", parameters, commandType: CommandType.StoredProcedure);
                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }

        public async Task<IEnumerable<PayMode>> GetPayModes()
        {
            var query = "SELECT * FROM PayModes";
            using (var connection = _context.CreateConnection())
            {
                var payModes = await connection.QueryAsync<PayMode>(query);
                return payModes.ToList();
            }
        }
        public async Task<SalaryProcessViewModel> GetSalaryProcessById(int processId)
        {
            var query = "SELECT SalaryProcessingID, MonthID, EmployeeTypeID AS EmployeeType, Status FROM SalaryReportMaster WHERE SalaryProcessingID = @id";

            using (var connection = _context.CreateConnection())
            {
                var process = await connection.QuerySingleOrDefaultAsync<SalaryProcessViewModel>(query, new { id = processId });
                return process;
            }
        }
        public async Task<IEnumerable<SalaryProcessMaster>> GetSalaryProcessMaster()
        {
            var query = @"Select * from SalaryReportMaster;";

            using (var connection = _context.CreateConnection())
            {
                var salaryProcesses = await connection.QueryAsync<SalaryProcessMaster>(query);
                return salaryProcesses.ToList();
            }
        }
        public async Task<int> UpdateSalaryProcessMaster(SalaryProcessMaster process)
        {
            var query = "UPDATE SalaryReportMaster SET Status = 0 WHERE SalaryProcessingID = @id";

            var parameters = new DynamicParameters();
            parameters.Add("id", process.SalaryProcessingID, DbType.Int32);

            using (var connection = _context.CreateConnection())
            {
                var result = await connection.ExecuteAsync(query, parameters);
                return result;
            }
        }

        public async Task<IEnumerable<SalaryProcessViewModel>> GetSalaryProcess()
        {
            var query = @"
                        SELECT 
                        SalaryProcessingID,
                        DATENAME(MONTH, DATEFROMPARTS(MonthID / 100, MonthID % 100, 1)) AS Month,
                        MonthID / 100 AS Year,
                        CASE 
                            WHEN EmployeeTypes.Id = 1 THEN 'Permanent'
                            WHEN EmployeeTypes.Id = 2 THEN 'Contract'
                            ELSE 'Daily Worker'
                        END AS EmployeeType,
                        CASE 
                            WHEN Status = 1 THEN 'Salary already processsed'
                            ELSE 'Finally Submitted' 
                        END AS Status
                    FROM 
                        SalaryReportMaster
                    INNER JOIN 
                        EmployeeTypes ON SalaryReportMaster.EmployeeTypeID = EmployeeTypes.Id
                        ORDER BY 
                        SalaryProcessingID DESC; 
                        ";

            using (var connection = _context.CreateConnection())
            {
                var salaryProcesses = await connection.QueryAsync<SalaryProcessViewModel>(query);
                return salaryProcesses.ToList();
            }
        }

        public async Task<int> SalaryProcess(int monthId, int employeeTypeId)
        {
            int result = 0;
            var parameters = new DynamicParameters();
            parameters.Add("MonthID", monthId, DbType.Int32);
            parameters.Add("EmployeeTypeID", employeeTypeId, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    if (employeeTypeId == 1)
                    {
                        result = await connection.ExecuteAsync("spSalaryProcessOfficer", parameters, commandType: CommandType.StoredProcedure);
                    }
                    else if (employeeTypeId == 2)
                    {
                        result = await connection.ExecuteAsync("spSalaryProcessJuniorStaff", parameters, commandType: CommandType.StoredProcedure);
                    }
                    else
                    {
                        result = await connection.ExecuteAsync("spSalaryProcessDailyWorker", parameters, commandType: CommandType.StoredProcedure);
                    }

                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }

        public async Task<int> SalaryProcesswithJobcode(int monthId, int employeeTypeId, List<string> jobCodes = null)
        {
            int result = 0;
            var parameters = new DynamicParameters();
            parameters.Add("MonthID", monthId, DbType.Int32);
            parameters.Add("EmployeeTypeID", employeeTypeId, DbType.Int32);

            if (jobCodes != null && jobCodes.Any())
            {
                string jobCodesString = string.Join(",", jobCodes);
                parameters.Add("JobCodes", jobCodesString, DbType.String);
            }

            try
            {
                using (var connection = _context.CreateConnection())
                {
                    string storedProcedure = employeeTypeId == 1 ? "spSalaryProcessOfficer" : "spSalaryProcessJuniorStaff";
                    result = await connection.ExecuteAsync(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;  
            }

            return result;
        }




        public async Task<List<FinalAdjustmentOfficer>> FinalAdjustmentOfficer(int monthId)
        {
            IEnumerable<FinalAdjustmentOfficer> finalAdjustments = null;
            var parameters = new DynamicParameters();
            parameters.Add("MonthID", monthId, DbType.Int32);
            var query = "SELECT * FROM SalaryReportOF where MonthID=@monthId ORDER BY JobCode ASC;";
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    finalAdjustments = await connection.QueryAsync<FinalAdjustmentOfficer>(query, new { monthId });
                }
                catch (Exception ex)
                {

                }
            }
            return finalAdjustments.ToList();
        }

        public async Task<List<JournalAdjustmentOfficer>> JournalAdjustmentOfficer(int monthId, int employeeTypeId)
        {
            IEnumerable<JournalAdjustmentOfficer> finalAdjustments = null;
            var parameters = new DynamicParameters();
            parameters.Add("MonthID", monthId, DbType.Int32);
            parameters.Add("EmployeeTypeId", employeeTypeId, DbType.Int32);
            var query = "SELECT j.Id, j.JournalMasterId, jm.MonthId, jm.EmployeeTypeId,j.JournalCode,j.AccountNumber, j.Details, j.Debit, j.Credit FROM SalaryJournals AS j JOIN SalaryJournalMaster AS jM ON jM.Id = j.JournalMasterId WHERE jm.MonthId = @MonthId AND jm.EmployeeTypeId = @EmployeeTypeId";
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    finalAdjustments = await connection.QueryAsync<JournalAdjustmentOfficer>(query, new { monthId, employeeTypeId });
                }
                catch (Exception ex)
                {

                }
            }
            return finalAdjustments.ToList();
        }

        public async Task<List<SmsService>> GetSmsService(int monthId, int employeeTypeId)
        {
            IEnumerable<SmsService> SmsOF = null;
            var parameters = new DynamicParameters();
            parameters.Add("MonthID", monthId, DbType.Int32);

            string query = string.Empty;

            if (employeeTypeId == 1)
            {
                query = "select SalaryReportOF.JobCode, SalaryReportOF.EmployeeName, DesignationName,DepartmentName, Employees.MobileNumber   from SalaryReportOF join Employees on Employees.JobCode = SalaryReportOF.JobCode where monthid=@monthid";
            }
            else if (employeeTypeId == 2)
            {
                query = "select SalaryReportJS.JobCode, SalaryReportJS.EmployeeName, DesignationName,DepartmentName, Employees.MobileNumber  from SalaryReportJS join Employees on Employees.JobCode = SalaryReportJS.JobCode where monthid=@monthid";
            }

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    SmsOF = await connection.QueryAsync<SmsService>(query, parameters);
                }
                catch (Exception ex)
                {
                   
                    
                    throw;
                }
            }

            return SmsOF.ToList();
        }



        public async Task<List<FinalAdjustmentJuniorStaff>> FinalAdjustmentJuniorStaff(int monthId)
        {
            IEnumerable<FinalAdjustmentJuniorStaff> finalAdjustments = null;
            var parameters = new DynamicParameters();
            parameters.Add("MonthID", monthId, DbType.Int32);
            var query = "SELECT * FROM SalaryReportJS where MonthID=@monthId ORDER BY JobCode ASC;";
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    finalAdjustments = await connection.QueryAsync<FinalAdjustmentJuniorStaff>(query, new { monthId });
                }
                catch (Exception ex)
                {

                }
            }
            return finalAdjustments.ToList();
        }


        public async Task<List<FinalAdjustmentOfficer>> CheckNetPayOfficer(int monthId)
        {
            IEnumerable<FinalAdjustmentOfficer> Netpay = null;
            var parameters = new DynamicParameters();
            parameters.Add("@MonthID", monthId, DbType.Int32);

            var query = @"
       SELECT 
                        JobCode,
                        EmployeeName,
                        DesignationName,
                        DepartmentName,
                        (
                            (ISNULL(BasicSalary, 0) +
                            ISNULL(PersonalSalary, 0) +
                            ISNULL(ArrearSalary, 0) +
                            ISNULL(LikeBasic, 0) +
                            ISNULL(OtherSalary, 0) +
                            ISNULL(SpecialBenefit, 0) +
                            ISNULL(LunchAllow, 0) +
                            ISNULL(TiffinAllow, 0) +
                            ISNULL(WashAllow, 0) +
                            ISNULL(HouseRentAllow, 0) +
                            ISNULL(Conveyance, 0) +
                            ISNULL(FMAllow, 0) +
                            ISNULL(EducationalAllow, 0) +
                            ISNULL(FieldRiskAllow, 0) +
                            ISNULL(ChargeAllow, 0) +
                            ISNULL(DAidAllow, 0) +
                            ISNULL(DeputationAllow, 0) +
                            ISNULL(OtherAllow, 0) +
                            ISNULL(CME, 0))
                            -
                            (ISNULL(RevenueStamp, 0) +
                            ISNULL(WelfareFund, 0) +
                            ISNULL(OfficerClub, 0) +
                            ISNULL(OfficerAssociation, 0) +
                            ISNULL(MedicalFund, 0) +
                            ISNULL(ProvidentFund, 0) +
                            ISNULL(Advance, 0) +
                            ISNULL(IncomeTax, 0) +
                            ISNULL(Other, 0) +
                            ISNULL(SpecialDeduction, 0) +
                            ISNULL(Hospitalisation, 0) +
                            ISNULL(TMBill, 0) +
                            ISNULL(HouseRentReturn, 0) +
                            ISNULL(Dormitory, 0) +
                            ISNULL(FuelReturn, 0) +
                            ISNULL(HBLoan, 0) +
                            ISNULL(MCylLoan, 0) +
                            ISNULL(BCylLoan, 0) +
                            ISNULL(ComLoan, 0) +
                            ISNULL(CarLoan, 0) +
                            ISNULL(PFLoan, 0) +
                            ISNULL(WPFLoan, 0) +
                            ISNULL(CosLoan, 0) +
                            ISNULL(OtherLoan, 0))
                        ) AS NetPay
                    FROM 
                        [bgfcl].[dbo].[SalaryReportOF] 
                    WHERE 
                        (
                            (ISNULL(BasicSalary, 0) +
                            ISNULL(PersonalSalary, 0) +
                            ISNULL(ArrearSalary, 0) +
                            ISNULL(LikeBasic, 0) +
                            ISNULL(OtherSalary, 0) +
                            ISNULL(SpecialBenefit, 0) +
                            ISNULL(LunchAllow, 0) +
                            ISNULL(TiffinAllow, 0) +
                            ISNULL(WashAllow, 0) +
                            ISNULL(HouseRentAllow, 0) +
                            ISNULL(Conveyance, 0) +
                            ISNULL(FMAllow, 0) +
                            ISNULL(EducationalAllow, 0) +
                            ISNULL(FieldRiskAllow, 0) +
                            ISNULL(ChargeAllow, 0) +
                            ISNULL(DAidAllow, 0) +
                            ISNULL(DeputationAllow, 0) +
                            ISNULL(OtherAllow, 0) +
                            ISNULL(CME, 0))
                            -
                            (ISNULL(RevenueStamp, 0) +
                            ISNULL(WelfareFund, 0) +
                            ISNULL(OfficerClub, 0) +
                            ISNULL(OfficerAssociation, 0) +
                            ISNULL(MedicalFund, 0) +
                            ISNULL(ProvidentFund, 0) +
                            ISNULL(Advance, 0) +
                            ISNULL(IncomeTax, 0) +
                            ISNULL(Other, 0) +
                            ISNULL(SpecialDeduction, 0) +
                            ISNULL(Hospitalisation, 0) +
                            ISNULL(TMBill, 0) +
                            ISNULL(HouseRentReturn, 0) +
                            ISNULL(Dormitory, 0) +
                            ISNULL(FuelReturn, 0) +
                            ISNULL(HBLoan, 0) +
                            ISNULL(MCylLoan, 0) +
                            ISNULL(BCylLoan, 0) +
                            ISNULL(ComLoan, 0) +
                            ISNULL(CarLoan, 0) +
                            ISNULL(PFLoan, 0) +
                            ISNULL(WPFLoan, 0) +
                            ISNULL(CosLoan, 0) +
                            ISNULL(OtherLoan, 0))
                        ) < 0 and  MonthID = @MonthID";


            using (var connection = _context.CreateConnection())
            {
                try
                {
                    Netpay = await connection.QueryAsync<FinalAdjustmentOfficer>(query, parameters);
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                }
            }

            return Netpay?.ToList() ?? new List<FinalAdjustmentOfficer>();
        }


        public async Task<List<FinalAdjustmentJuniorStaff>> CheckNetPayJuniorStaff(int monthId)
        {
            IEnumerable<FinalAdjustmentJuniorStaff> Netpay = null;
            var parameters = new DynamicParameters();
            parameters.Add("@MonthID", monthId, DbType.Int32);

            var query = @"
                        SELECT 
                            [JobCode], 
                            [EmployeeName],
                            ([BasicSalary] +
                             [PersonalSalary] +
                             [ArrearSalary] +
                             [OtherSalary] +
                             [LunchAllow] +
                             [TiffinAllow] +
                             [HouseRentAllow] +
                             [FamilyMedicalAllow] +
                             [ShiftAllow] +
                             [OtAllow] +
                             [UtilityAllow] +
                             [EducationAllowance] +
                             [FuelAllow] +
                             [OtherAllow] +
                             [FieldAllow] +
                             [ConvenienceAllow] +
                             [SpecialBenefit]) 
                             - 
                            ([RevenueStamp] +
                             [WelfareFund] +
                             [EmployeeClub] +
                             [EmployeeUnion] +
                             [ProvidentFund] +
                             [Dormitory] +
                             [HospitalDeduction] +
                             [FuelReturn] +
                             [SpecialDeduction] +
                             [Others] +
                             [Advance] +
                             [HBLoan] +
                             [MCylLoan] +
                             [BCylLoan] +
                             [ComputerLoan] +
                             [PFLoan] +
                             [WPFLoan] +
                             [CosLoan] +
                             [OtherLoan]) AS NetPay
                        FROM 
                            [bgfcl].[dbo].[SalaryReportJS]
                        WHERE 
                            (
                            ([BasicSalary] +
                             [PersonalSalary] +
                             [ArrearSalary] +
                             [OtherSalary] +
                             [LunchAllow] +
                             [TiffinAllow] +
                             [HouseRentAllow] +
                             [FamilyMedicalAllow] +
                             [ShiftAllow] +
                             [OtAllow] +
                             [UtilityAllow] +
                             [EducationAllowance] +
                             [FuelAllow] +
                             [OtherAllow] +
                             [FieldAllow] +
                             [ConvenienceAllow] +
                             [SpecialBenefit]) 
                             - 
                            ([RevenueStamp] +
                             [WelfareFund] +
                             [EmployeeClub] +
                             [EmployeeUnion] +
                             [ProvidentFund] +
                             [Dormitory] +
                             [HospitalDeduction] +
                             [FuelReturn] +
                             [SpecialDeduction] +
                             [Others] +
                             [Advance] +
                             [HBLoan] +
                             [MCylLoan] +
                             [BCylLoan] +
                             [ComputerLoan] +
                             [PFLoan] +
                             [WPFLoan] +
                             [CosLoan] +
                             [OtherLoan])
                            ) < 0 and  MonthID = @MonthID";



            using (var connection = _context.CreateConnection())
            {
                try
                {
                    Netpay = await connection.QueryAsync<FinalAdjustmentJuniorStaff>(query, parameters);
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                }
            }

            return Netpay?.ToList() ?? new List<FinalAdjustmentJuniorStaff>();
        }


        public async Task<List<SalaryReportOfficer>> GetGroupedSalaryReportData(int monthId, int department, int designation)
        {
            IEnumerable<SalaryReportOfficer> salaryReportData = null;
            var query = @"
            SELECT 
            MonthID,
            JobCode,
            OfficerName,
            BankACNo,
            Designation,
            Department,
            BankName,
            BankAddress,
            PersonalPay,
            BasicSalary,
            LikeBasic,
            ArrearSalary,
            OtherSalary,
            WorkDays,
            Other,
            LunchAllow,
            TiffineAllow,
            SBenefit,
            HRentAllow,
            FMAllow,
            Conveyance,
            FimAllow,
            WashAllow,
            Education,
            ChargeAllow,
            DeputAllow,
            OtherAllow,
            WFund,
            RevStamp,
            CPF,
            OffClub,
            OffAsso,
            MedFund,
            MobileBill,
            Hospitalization,
            RestHReturn,
            Dormitory,
            SpecialD,
            FuelReturn,
            TotalLoan,
            Advance,
            IncomeTax,
            (BasicSalary + LikeBasic + OtherSalary + ArrearSalary + PersonalPay + TiffineAllow + LunchAllow + SBenefit + HRentAllow + Conveyance + FMAllow + WashAllow + Education + ChargeAllow + DeputAllow + OtherAllow) AS GrossPay,
            (RevStamp + WFund + CPF + OffClub + MedFund + MobileBill + Hospitalization + RestHReturn + Dormitory + SpecialD + FuelReturn + TotalLoan + Advance + IncomeTax) AS TotalDeduction,
            ((BasicSalary + Other + PersonalPay + TiffineAllow + FMAllow + WFund + CPF) - (WFund + CPF + OffClub + MedFund)) AS NetPay,
            (GrossPay - TotalDeduction) AS NetPable
            FROM 
                SalaryReportOF
            WHERE 
                MonthID = @MonthID
                AND Department = @Department
                AND Designation = @Designation
            ORDER BY 
                Department, Designation, JobCode;
            ";

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    salaryReportData = await connection.QueryAsync<SalaryReportOfficer>(
                        query,
                        new { MonthID = monthId, Department = department, Designation = designation }
                    );
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"An error occurred: {ex.Message}");
                }
            }

            return salaryReportData?.ToList() ?? new List<SalaryReportOfficer>();
        }
        
        public async Task<int> UpdateFinalAdjustmentOfficer(FinalAdjustmentOfficer finalAdjustmentOfficer)
        {
            string query = $"update SalaryReportOF set  " +
                "BasicSalary=@BasicSalary," +
                "PersonalSalary=@PersonalSalary," +
                "ArrearSalary=@ArrearSalary," +
                "WorkDays=@WorkDays," +
                "LikeBasic=@LikeBasic," +
                "OtherSalary=@OtherSalary," +
                "LunchAllow=@LunchAllow," +
                "TiffinAllow=@TiffinAllow," +
                "WashAllow=@WashAllow," +
                "HouseRentAllow=@HouseRentAllow," +
                "Conveyance=@Conveyance," +
                "FMAllow=@FMAllow," +
                "EducationalAllow=@EducationalAllow," +
                "FieldRiskAllow=@FieldRiskAllow," +
                "ChargeAllow=@ChargeAllow," +
                "DAidAllow=@DAidAllow," +
                "DeputationAllow=@DeputationAllow," +
                "OtherAllow=@OtherAllow," +
                "RevenueStamp=@RevenueStamp," +
                "ProvidentFund=@ProvidentFund," +
                "PensionOfficer=@PensionOfficer," +
                "WelfareFund=@WelfareFund," +
                "OfficerClub=@OfficerClub," +
                "OfficerAssociation=@OfficerAssociation," +
                "MedicalFund=@MedicalFund," +
                "TMBill=@TMBill," +
                "Dormitory=@Dormitory," +
                "Hospitalisation=@Hospitalisation," +
                "HouseRentReturn=@HouseRentReturn," +
                "SpecialDeduction=@SpecialDeduction," +
                "FuelReturn=@FuelReturn," +
                "HBLoan=@HBLoan," +
                "MCylLoan=@MCylLoan," +
                "BCylLoan=@BCylLoan," +
                "ComLoan=@ComLoan," +
                "CarLoan=@CarLoan," +
                "PFLoan=@PFLoan," +
                "WPFLoan=@WPFLoan," +
                "CosLoan=@CosLoan," +
                "OtherLoan=@OtherLoan," +
                "Advance=@Advance," +
                "Other=@Other," +
                "IncomeTax=@IncomeTax," +
                "SpecialBenefit=@SpecialBenefit, " +
                "CME=@CME " +
                "where Id=@Id";

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("BasicSalary", finalAdjustmentOfficer.BasicSalary, DbType.Double);
                    parameters.Add("PersonalSalary", finalAdjustmentOfficer.PersonalSalary, DbType.Double);
                    parameters.Add("ArrearSalary", finalAdjustmentOfficer.ArrearSalary, DbType.Double);
                    parameters.Add("WorkDays", finalAdjustmentOfficer.WorkDays, DbType.Double);
                    parameters.Add("LikeBasic", finalAdjustmentOfficer.LikeBasic, DbType.Double);
                    parameters.Add("OtherSalary", finalAdjustmentOfficer.OtherSalary, DbType.Double);
                    parameters.Add("LunchAllow", finalAdjustmentOfficer.LunchAllow, DbType.Double);
                    parameters.Add("TiffinAllow", finalAdjustmentOfficer.TiffinAllow, DbType.Double);
                    parameters.Add("WashAllow", finalAdjustmentOfficer.WashAllow, DbType.Double);
                    parameters.Add("HouseRentAllow", finalAdjustmentOfficer.HouseRentAllow, DbType.Double);
                    parameters.Add("Conveyance", finalAdjustmentOfficer.Conveyance, DbType.Double);
                    parameters.Add("FMAllow", finalAdjustmentOfficer.FMAllow, DbType.Double);
                    parameters.Add("EducationalAllow", finalAdjustmentOfficer.EducationalAllow, DbType.Double);
                    parameters.Add("FieldRiskAllow", finalAdjustmentOfficer.FieldRiskAllow, DbType.Double);
                    parameters.Add("ChargeAllow", finalAdjustmentOfficer.ChargeAllow, DbType.Double);
                    parameters.Add("DAidAllow", finalAdjustmentOfficer.DAidAllow, DbType.Double);
                    parameters.Add("DeputationAllow", finalAdjustmentOfficer.DeputationAllow, DbType.Double);
                    parameters.Add("OtherAllow", finalAdjustmentOfficer.OtherAllow, DbType.Double);
                    parameters.Add("RevenueStamp", finalAdjustmentOfficer.RevenueStamp, DbType.Double);
                    parameters.Add("ProvidentFund", finalAdjustmentOfficer.ProvidentFund, DbType.Double);
                    parameters.Add("PensionOfficer", finalAdjustmentOfficer.PensionOfficer, DbType.Double);
                    parameters.Add("WelfareFund", finalAdjustmentOfficer.WelfareFund, DbType.Double);
                    parameters.Add("OfficerClub", finalAdjustmentOfficer.OfficerClub, DbType.Double);
                    parameters.Add("OfficerAssociation", finalAdjustmentOfficer.OfficerAssociation, DbType.Double);
                    parameters.Add("MedicalFund", finalAdjustmentOfficer.MedicalFund, DbType.Double);
                    parameters.Add("TMBill", finalAdjustmentOfficer.TMBill, DbType.Double);
                    parameters.Add("Dormitory", finalAdjustmentOfficer.Dormitory, DbType.Double);
                    parameters.Add("Hospitalisation", finalAdjustmentOfficer.Hospitalisation, DbType.Double);
                    parameters.Add("HouseRentReturn", finalAdjustmentOfficer.HouseRentReturn, DbType.Double);
                    parameters.Add("SpecialDeduction", finalAdjustmentOfficer.SpecialDeduction, DbType.Double);
                    parameters.Add("FuelReturn", finalAdjustmentOfficer.FuelReturn, DbType.Double);
                    parameters.Add("HBLoan", finalAdjustmentOfficer.HBLoan, DbType.Double);
                    parameters.Add("MCylLoan", finalAdjustmentOfficer.MCylLoan, DbType.Double);
                    parameters.Add("BCylLoan", finalAdjustmentOfficer.BCylLoan, DbType.Double);
                    parameters.Add("ComLoan", finalAdjustmentOfficer.ComLoan, DbType.Double);
                    parameters.Add("CarLoan", finalAdjustmentOfficer.CarLoan, DbType.Double);
                    parameters.Add("PFLoan", finalAdjustmentOfficer.PFLoan, DbType.Double);
                    parameters.Add("WPFLoan", finalAdjustmentOfficer.WPFLoan, DbType.Double);
                    parameters.Add("CosLoan", finalAdjustmentOfficer.CosLoan, DbType.Double);
                    parameters.Add("OtherLoan", finalAdjustmentOfficer.OtherLoan, DbType.Double);
                    parameters.Add("Advance", finalAdjustmentOfficer.Advance, DbType.Double);
                    parameters.Add("Other", finalAdjustmentOfficer.Other, DbType.Double);
                    parameters.Add("IncomeTax", finalAdjustmentOfficer.IncomeTax, DbType.Double);
                    parameters.Add("SpecialBenefit", finalAdjustmentOfficer.SpecialBenefit, DbType.Double);
                    parameters.Add("CME", finalAdjustmentOfficer.CME, DbType.Double);
                    parameters.Add("Id", finalAdjustmentOfficer.Id, DbType.Int32);
                    var result = await connection.ExecuteScalarAsync<int>(query, parameters);
                    return result;
                }
                catch (Exception ex)
                {
                    return 0;
                }

            }
        }

        public async Task<int> UpdateFinalAdjustmentJuniorStaff(FinalAdjustmentJuniorStaff finalAdjustmentJuniorStaff)
        {
            string query = $"update SalaryReportJS set  " +
                "BasicSalary=@BasicSalary," +
                "PersonalSalary=@PersonalSalary," +
                "ConvenienceAllow=@ConvenienceAllow," +
                "ArrearSalary=@ArrearSalary," +
                "WorkDays=@WorkDays," +
                "NumberOfShift=@NumberOfShift," +
                "OtherSalary=@OtherSalary," +
                "SpecialBenefit=@SpecialBenefit," +
                "LunchAllow=@LunchAllow," +
                "TiffinAllow=@TiffinAllow," +
                "ShiftAllow=@ShiftAllow," +
                "HouseRentAllow=@HouseRentAllow," +
                "FamilyMedicalAllow=@FamilyMedicalAllow," +
                "EducationAllowance=@EducationAllowance," +
                "FieldAllow=@FieldAllow," +
                "OtSingle=@OtSingle," +
                "OtDouble=@OtDouble," +
                "OtAllow=@OtAllow," +
                "FuelAllow=@FuelAllow," +
                "UtilityAllow=@UtilityAllow," +
                "OtherAllow=@OtherAllow," +
                "RevenueStamp=@RevenueStamp," +
                "ProvidentFund=@ProvidentFund," +
                "UtilityReturn=@UtilityReturn," +
                "WelfareFund=@WelfareFund," +
                "EmployeeClub=@EmployeeClub," +
                "EmployeeUnion=@EmployeeUnion," +
                "Dormitory=@Dormitory," +
                "HospitalDeduction=@HospitalDeduction," +
                "SpecialDeduction=@SpecialDeduction," +
                "FuelReturn=@FuelReturn," +
                "HBLoan=@HBLoan," +
                "MCylLoan=@MCylLoan," +
                "BCylLoan=@BCylLoan," +
                "ComputerLoan=@ComputerLoan," +
                "PFLoan=@PFLoan," +
                "WPFLoan=@WPFLoan," +
                "CosLoan=@CosLoan," +
                "OtherLoan=@OtherLoan," +
                "Advance=@Advance," +
                "Others=@Others," +
                "PensionCom=@PensionCom " +
                "where Id=@Id";

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("BasicSalary", finalAdjustmentJuniorStaff.BasicSalary, DbType.Double);
                    parameters.Add("PersonalSalary", finalAdjustmentJuniorStaff.PersonalSalary, DbType.Double);
                    parameters.Add("ConvenienceAllow", finalAdjustmentJuniorStaff.ConvenienceAllow, DbType.Double);
                    parameters.Add("ArrearSalary", finalAdjustmentJuniorStaff.ArrearSalary, DbType.Double);
                    parameters.Add("WorkDays", finalAdjustmentJuniorStaff.WorkDays, DbType.Double);
                    parameters.Add("NumberOfShift", finalAdjustmentJuniorStaff.NumberOfShift, DbType.Double);
                    parameters.Add("OtherSalary", finalAdjustmentJuniorStaff.OtherSalary, DbType.Double);
                    parameters.Add("SpecialBenefit", finalAdjustmentJuniorStaff.SpecialBenefit, DbType.Double);
                    parameters.Add("LunchAllow", finalAdjustmentJuniorStaff.LunchAllow, DbType.Double);
                    parameters.Add("TiffinAllow", finalAdjustmentJuniorStaff.TiffinAllow, DbType.Double);
                    parameters.Add("ShiftAllow", finalAdjustmentJuniorStaff.ShiftAllow, DbType.Double);
                    parameters.Add("HouseRentAllow", finalAdjustmentJuniorStaff.HouseRentAllow, DbType.Double);
                    parameters.Add("FamilyMedicalAllow", finalAdjustmentJuniorStaff.FamilyMedicalAllow, DbType.Double);
                    parameters.Add("EducationAllowance", finalAdjustmentJuniorStaff.EducationAllowance, DbType.Double);
                    parameters.Add("FieldAllow", finalAdjustmentJuniorStaff.FieldAllow, DbType.Double);
                    parameters.Add("OtSingle", finalAdjustmentJuniorStaff.OtSingle, DbType.Double);
                    parameters.Add("OtDouble", finalAdjustmentJuniorStaff.OtDouble, DbType.Double);
                    parameters.Add("OtAllow", finalAdjustmentJuniorStaff.OtAllow, DbType.Double);
                    parameters.Add("FuelAllow", finalAdjustmentJuniorStaff.FuelAllow, DbType.Double);
                    parameters.Add("UtilityAllow", finalAdjustmentJuniorStaff.UtilityAllow, DbType.Double);
                    parameters.Add("OtherAllow", finalAdjustmentJuniorStaff.OtherAllow, DbType.Double);
                    parameters.Add("RevenueStamp", finalAdjustmentJuniorStaff.RevenueStamp, DbType.Double);
                    parameters.Add("ProvidentFund", finalAdjustmentJuniorStaff.ProvidentFund, DbType.Double);
                    parameters.Add("UtilityReturn", finalAdjustmentJuniorStaff.UtilityReturn, DbType.Double);
                    parameters.Add("WelfareFund", finalAdjustmentJuniorStaff.WelfareFund, DbType.Double);
                    parameters.Add("EmployeeClub", finalAdjustmentJuniorStaff.EmployeeClub, DbType.Double);
                    parameters.Add("EmployeeUnion", finalAdjustmentJuniorStaff.EmployeeUnion, DbType.Double);
                    parameters.Add("Dormitory", finalAdjustmentJuniorStaff.Dormitory, DbType.Double);
                    parameters.Add("HospitalDeduction", finalAdjustmentJuniorStaff.HospitalDeduction, DbType.Double);
                    parameters.Add("SpecialDeduction", finalAdjustmentJuniorStaff.SpecialDeduction, DbType.Double);
                    parameters.Add("FuelReturn", finalAdjustmentJuniorStaff.FuelReturn, DbType.Double);
                    parameters.Add("HBLoan", finalAdjustmentJuniorStaff.HBLoan, DbType.Double);
                    parameters.Add("MCylLoan", finalAdjustmentJuniorStaff.MCylLoan, DbType.Double);
                    parameters.Add("BCylLoan", finalAdjustmentJuniorStaff.BCylLoan, DbType.Double);
                    parameters.Add("ComputerLoan", finalAdjustmentJuniorStaff.ComputerLoan, DbType.Double);
                    parameters.Add("PFLoan", finalAdjustmentJuniorStaff.PFLoan, DbType.Double);
                    parameters.Add("WPFLoan", finalAdjustmentJuniorStaff.WPFLoan, DbType.Double);
                    parameters.Add("CosLoan", finalAdjustmentJuniorStaff.CosLoan, DbType.Double);
                    parameters.Add("OtherLoan", finalAdjustmentJuniorStaff.OtherLoan, DbType.Double);
                    parameters.Add("Advance", finalAdjustmentJuniorStaff.Advance, DbType.Double);
                    parameters.Add("Others", finalAdjustmentJuniorStaff.Others, DbType.Double);
                    parameters.Add("PensionCom", finalAdjustmentJuniorStaff.PensionCom, DbType.Double);
                    parameters.Add("Id", finalAdjustmentJuniorStaff.Id, DbType.Int32);
                    var result = await connection.ExecuteScalarAsync<int>(query, parameters);
                    return result;
                }
                catch (Exception ex)
                {
                    return 0;
                }

            }
        }

        public async Task<SalaryJournalMaster> GetSalaryJournalMaster(int monthId, int employeeTypeId)
        {
            var query = "SELECT * FROM SalaryJournalMaster where MonthId=@MonthId and EmployeeTypeId=@EmployeeTypeId";
            var parameters = new DynamicParameters();
            parameters.Add("MonthId", monthId, DbType.Int32);
            parameters.Add("EmployeeTypeId", employeeTypeId, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                var journalMaster = await connection.QuerySingleOrDefaultAsync<SalaryJournalMaster>(query, parameters);
                return journalMaster;
            }
        }
        public async Task<int> CreateSalaryJournalMaster(SalaryJournalMaster salaryJournalMaster)
        {
            int result = 0;
            string query = "insert into SalaryJournalMaster (MonthId,EmployeeTypeId,CreatedBy,CreatedDate) values (@MonthId,@EmployeeTypeId,@CreatedBy,@CreatedDate) SELECT CAST(SCOPE_IDENTITY() AS INT);";
            var parameters = new DynamicParameters();
            parameters.Add("MonthId", salaryJournalMaster.MonthId, DbType.Int32);
            parameters.Add("EmployeeTypeId", salaryJournalMaster.EmployeeTypeId, DbType.Int32);
            parameters.Add("CreatedBy", salaryJournalMaster.CreatedBy, DbType.String);
            parameters.Add("CreatedDate", salaryJournalMaster.CreatedDate, DbType.DateTime);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    result = await connection.QuerySingleAsync<int>(query, parameters);
                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }

        public async Task<int> CreateSalaryJournal(SalaryJournal salaryJournal)
        {
            int result = 0;
            string query = "insert into SalaryJournals (AccountNumber,JournalCode,Details,Debit,Credit,JournalMasterId) values (@AccountNumber,@JournalCode,@Details,@Debit,@Credit,@JournalMasterId)";
            var parameters = new DynamicParameters();
            parameters.Add("AccountNumber", salaryJournal.AccountNumber, DbType.String);
            parameters.Add("JournalCode", salaryJournal.JournalCode, DbType.String);
            parameters.Add("Details", salaryJournal.Details, DbType.String);
            parameters.Add("Debit", salaryJournal.Debit, DbType.Decimal);
            parameters.Add("Credit", salaryJournal.Credit, DbType.Decimal);
            parameters.Add("JournalMasterId", salaryJournal.JournalMasterId, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    result = await connection.ExecuteAsync(query, parameters);
                }
                catch (Exception ex)
                {

                }
            }
            return result;
        }

        public async Task<JournalAdjustmentOfficer> InsertNewSalaryJournal(JournalAdjustmentOfficer salaryJournal)
        {
            string insertQuery = @"
                                INSERT INTO SalaryJournals (AccountNumber, JournalCode, Details, Debit, Credit, JournalMasterId)
                                VALUES (@AccountNumber, @JournalCode, @Details, @Debit, @Credit, @JournalMasterId);
                                SELECT CAST(SCOPE_IDENTITY() AS INT);"; // Get the generated ID

                    string selectQuery = @"
                                SELECT *
                                FROM SalaryJournals
                                WHERE Id = @Id";

            int newId = 0;

            var parameters = new DynamicParameters();
            parameters.Add("AccountNumber", salaryJournal.AccountNumber, DbType.String);
            parameters.Add("JournalCode", salaryJournal.JournalCode, DbType.String);
            parameters.Add("Details", salaryJournal.Details, DbType.String);

            // Check if Debit and Credit are 0, and set them to null if they are
            parameters.Add("Debit", salaryJournal.Debit != 0 ? salaryJournal.Debit : (decimal?)null, DbType.Decimal);
            parameters.Add("Credit", salaryJournal.Credit != 0 ? salaryJournal.Credit : (decimal?)null, DbType.Decimal);

            parameters.Add("JournalMasterId", salaryJournal.JournalMasterId, DbType.Int32);

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    // Execute the insert query and get the new ID
                    newId = await connection.ExecuteScalarAsync<int>(insertQuery, parameters);

                    // Fetch the newly inserted record using the new ID
                    var selectParameters = new DynamicParameters();
                    selectParameters.Add("Id", newId, DbType.Int32);

                    var newRecord = await connection.QuerySingleOrDefaultAsync<JournalAdjustmentOfficer>(selectQuery, selectParameters);

                    return newRecord;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occurred: {ex.Message}");
                    throw;
                }
            }
        }





        public async Task<List<SalaryJournal>> GetSalaryJournalsByMasterId(int journalMasterId)
        {
            var query = "SELECT * FROM SalaryJournals where JournalMasterId=@JournalMasterId";
            var parameters = new DynamicParameters();
            parameters.Add("JournalMasterId", journalMasterId, DbType.Int32);
            using (var connection = _context.CreateConnection())
            {
                var salaryJournals = await connection.QueryAsync<SalaryJournal>(query, parameters);
                return salaryJournals.ToList();
            }
        }
        public async Task<int> UpdateJournalAdjustmentOfficer(JournalAdjustmentOfficer salaryJournal)
        {
            string query = $"UPDATE SalaryJournals SET " +
                "AccountNumber = @AccountNumber, " +
                "JournalCode = @JournalCode, " +
                "Details = @Details, " +
                "Debit = @Debit, " +
                "Credit = @Credit " + // Remove the comma and add a WHERE clause
                "WHERE Id = @Id"; // Assume Id or another unique column

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("AccountNumber", salaryJournal.AccountNumber, DbType.String);
                    parameters.Add("JournalCode", salaryJournal.JournalCode, DbType.String);
                    parameters.Add("Details", salaryJournal.Details, DbType.String);
                    parameters.Add("Debit", salaryJournal.Debit, DbType.Double);
                    parameters.Add("Credit", salaryJournal.Credit, DbType.Double);
                    parameters.Add("Id", salaryJournal.Id, DbType.Int32); // Add the Id parameter

                    var result = await connection.ExecuteScalarAsync<int>(query, parameters);
                    return result;
                }
                catch (Exception ex)
                {
                    
                    return 0;
                }
            }
        }

        public async Task<int> DeleteSalaryJournalOff(int id)
        {
            string query = "DELETE FROM SalaryJournals WHERE Id = @Id";

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("Id", id, DbType.Int32); 

                    var result = await connection.ExecuteAsync(query, parameters);
                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occurred: {ex.Message}");
                    return 0;
                }
            }
        }


        public async Task<IEnumerable<OverTimeViewModal>> OvertimeData(List<string>? jobcodes, int currentMonthId, string? departmentname)
        {
            //var query = @"
            //            SELECT 
            //                JournalNumber,
            //                COUNT(DISTINCT JobCode) AS NumberOfEmployees,
            //                SUM(CASE WHEN MonthId = @CurrentMonthId THEN OtSingle + OtDouble ELSE 0 END) AS CurrentMonthOtHours,
            //                SUM(CASE WHEN MonthId = @CurrentMonthId THEN OtAllow ELSE 0 END) AS CurrentMonthOtAllow,
            //                SUM(OtSingle + OtDouble) AS TotalOtHoursFromJuly,
            //                SUM(OtAllow) AS TotalOtAllowFromJuly
            //            FROM SalaryReportJS
            //            WHERE MonthId >= 7 AND MonthId <= @CurrentMonthId";

            //var parameters = new DynamicParameters();
            //parameters.Add("CurrentMonthId", currentMonthId, DbType.Int32);

            // Conditionally add filters for jobcodes and departmentname
            //if (jobcodes != null && jobcodes.Any())
            //{
            //    query += " AND JobCode IN @JobCodes";
            //    parameters.Add("JobCodes", jobcodes);
            //}

            //if (!string.IsNullOrEmpty(departmentname))
            //{
            //    query += " AND DepartmentName = @DepartmentName";
            //    parameters.Add("DepartmentName", departmentname);
            //}

            //query += " GROUP BY JournalNumber";

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("monthId", currentMonthId);

            using (var connection = _context.CreateConnection())
            {
                var overtimeData = await connection.QueryAsync<OverTimeViewModal>("uspOverTime", parameters,commandType:CommandType.StoredProcedure);
                return overtimeData.ToList();
            }
        }


        #region report
        public async Task<List<MedicalFundViewModel>> GetMedicalFunds(int monthId)
        {
            List<MedicalFundViewModel> medicalFunds = new List<MedicalFundViewModel>();
            string query = "select JobCode,EmployeeName,DesignationName,DepartmentName,MedicalFund  from SalaryReportOF where monthid=@monthid";
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    medicalFunds.AddRange(await connection.QueryAsync<MedicalFundViewModel>(query, new { monthid = monthId }));
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"An error occurred: {ex.Message}");
                }
            }

            return medicalFunds;
        }

        public async Task<List<LoanSheetViewModel>> GetLoanSheetData(int monthId, int employeeTypeId)
        {
            string query = "";
            List<LoanSheetViewModel> loanSheets = new List<LoanSheetViewModel>();
            if (employeeTypeId == 1)
            {
                query = "select JobCode,EmployeeName,DesignationName,DepartmentName,HBLoan,MCylLoan as MCycleLoan,BCylLoan as BCycleLoan,ComLoan,PFLoan,WPFLoan,CosLoan,OtherLoan,CarLoan  from SalaryReportOF where monthid=@monthid";
            }
            else
            {
                query = "select JobCode,EmployeeName,DesignationName,DepartmentName,HBLoan,MCylLoan as MCycleLoan,BCylLoan as BCycleLoan,ComputerLoan as ComLoan,PFLoan,WPFLoan,CosLoan,OtherLoan  from SalaryReportJS where monthid=@monthid";
            }

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    loanSheets.AddRange(await connection.QueryAsync<LoanSheetViewModel>(query, new { monthid = monthId }));
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"An error occurred: {ex.Message}");
                }
            }

            return loanSheets;
        }

        public async Task<List<PFSheetViewModel>> GetPFSheetData(List<string> jobCode, int monthId, int employeeTypeId)
        {
            string query = "";
            List<PFSheetViewModel> pfSheet = new List<PFSheetViewModel>();

            var parameters = new DynamicParameters();
            parameters.Add("MonthID", monthId, DbType.Int32);

            // Build query based on employeeTypeId
            if (employeeTypeId == 1)
            {
                query = @"SELECT SalaryReportOF.JobCode, 
                         SalaryReportOF.EmployeeName, 
                         DesignationName, 
                         BasicSalary,
                         ProvidentFund AS OwnContribution, 
                         ProvidentFund AS CompanyContribution 
                  FROM SalaryReportOF 
                  JOIN Employees ON Employees.JobCode = SalaryReportOF.JobCode 
                  WHERE SalaryReportOF.MonthID = @MonthID 
                    AND Employees.EmployeeTypeId = 1 ORDER BY CONVERT(INT, Employees.EmpSl) ASC;";
            }
            if (employeeTypeId == 2)
            {
                query = @"SELECT SalaryReportJS.JobCode, 
                         SalaryReportJS.EmployeeName, 
                         DesignationName,
                         BasicSalary,
                         ProvidentFund AS OwnContribution, 
                         ProvidentFund AS CompanyContribution 
                  FROM SalaryReportJS 
                  JOIN Employees ON Employees.JobCode = SalaryReportJS.JobCode 
                  WHERE SalaryReportJS.MonthID = @MonthID 
                    AND Employees.EmployeeTypeId = 2";
            }


            // Add JobCode filter if provided
            if (jobCode != null && jobCode.Count > 0)
            {
                if (employeeTypeId == 1 || employeeTypeId == 3)
                {
                    query += " AND SalaryReportOF.JobCode IN @JobCodes";
                }
                else if (employeeTypeId == 2 || employeeTypeId == 4)
                {
                    query += " AND SalaryReportJS.JobCode IN @JobCodes";
                }
                parameters.Add("JobCodes", jobCode);
            }

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    pfSheet.AddRange(await connection.QueryAsync<PFSheetViewModel>(query, parameters));
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"An error occurred: {ex.Message}");
                }
            }

            return pfSheet;
        }



        public async Task<List<PFSheetViewModel>> GetYearlyPFSheetData(List<string> jobCodes, int employeeTypeId, int fromMonthId, int toMonthId, string? department)
        {
            string query = string.Empty;
            List<PFSheetViewModel> pfSheet = new List<PFSheetViewModel>();
            var parameters = new DynamicParameters();

            parameters.Add("fMonthID", fromMonthId, DbType.Int32);
            parameters.Add("tMonthID", toMonthId, DbType.Int32);

            // Base query
            query = @"
                    SELECT 
                        sr.JobCode,
                        sr.EmployeeName, 
                        DesignationName, 
                        SUM(ProvidentFund) AS OwnContribution, 
                        SUM(ProvidentFund) AS CompanyContribution 
                    FROM {0} sr 
                    JOIN Employees ON Employees.JobCode = sr.JobCode 
                    WHERE Employees.EmployeeTypeId = @EmployeeTypeId 
                        AND sr.MonthID BETWEEN @fMonthID AND @tMonthID";

            // Determine the correct table for SalaryReport based on employeeTypeId
            string tableName = employeeTypeId == 1 || employeeTypeId == 3 ? "SalaryReportOF" : "SalaryReportJS";
            query = string.Format(query, tableName);

            // Add parameters
            parameters.Add("EmployeeTypeId", employeeTypeId, DbType.Int32);

            // Add department filter if provided
            if (!string.IsNullOrEmpty(department))
            {
                query += " AND DepartmentName = @DepartmentName";
                parameters.Add("DepartmentName", department, DbType.String);
            }

            // Add job codes filter if provided
            if (jobCodes != null && jobCodes.Count > 0)
            {
                query += " AND sr.JobCode IN @JobCodes";
                parameters.Add("JobCodes", jobCodes);
            }

            // Group by required fields
            query += @" GROUP BY sr.JobCode, sr.EmployeeName, DesignationName";

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    pfSheet.AddRange(await connection.QueryAsync<PFSheetViewModel>(query, parameters));
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"An error occurred: {ex.Message}");
                    // You can log this exception or handle it further
                }
            }

            return pfSheet;
        }


        public async Task<List<WelfareFundViewModel>> WelfarefundData(int monthId, int employeeTypeId)
        {
            string query = "";
            List<WelfareFundViewModel> pfSheet = new List<WelfareFundViewModel>();
            //if (employeeTypeId == 1)
            //{
            //    query = "select SalaryReportOF.JobCode, SalaryReportOF.EmployeeName, DesignationName,DepartmentName, WelfareFund   from SalaryReportOF join Employees on Employees.JobCode = SalaryReportOF.JobCode where monthid=@monthid and Employees.EmployeeTypeId=1";
            //}
            //if (employeeTypeId == 3)
            //{
            //    query = "select SalaryReportOF.JobCode, SalaryReportOF.EmployeeName, DesignationName,DepartmentName, WelfareFund  from SalaryReportOF join Employees on Employees.JobCode = SalaryReportOF.JobCode where monthid=@monthid and Employees.EmployeeTypeId=3";
            //}
            //if (employeeTypeId == 2)
            //{
            //    query = "select SalaryReportJS.JobCode, SalaryReportJS.EmployeeName, DesignationName,DepartmentName, WelfareFund  from SalaryReportJS join Employees on Employees.JobCode = SalaryReportJS.JobCode where monthid=@monthid and Employees.EmployeeTypeId=2";
            //}
            //if (employeeTypeId == 4)
            //{
            //    query = "select SalaryReportJS.JobCode, SalaryReportJS.EmployeeName, DesignationName,DepartmentName, WelfareFund  from SalaryReportJS join Employees on Employees.JobCode = SalaryReportJS.JobCode where monthid=@monthid and Employees.EmployeeTypeId=4";
            //}


            if (employeeTypeId == 1)
            {
                query = "select SalaryReportOF.JobCode, SalaryReportOF.EmployeeName, DesignationName,DepartmentName, WelfareFund   from SalaryReportOF join Employees on Employees.JobCode = SalaryReportOF.JobCode where monthid=@monthid";
            }
            if (employeeTypeId == 2)
            {
                query = "select SalaryReportJS.JobCode, SalaryReportJS.EmployeeName, DesignationName,DepartmentName, WelfareFund  from SalaryReportJS join Employees on Employees.JobCode = SalaryReportJS.JobCode where monthid=@monthid";
            }

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    pfSheet.AddRange(await connection.QueryAsync<WelfareFundViewModel>(query, new { monthid = monthId }));
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"An error occurred: {ex.Message}");
                }
            }

            return pfSheet;
        }


        public async Task<List<EmployeeUnionORClub>> EmployeeUnionORClubData(int monthId, int reportType)
        {
            //string query = "";
            List<EmployeeUnionORClub> data = new List<EmployeeUnionORClub>();
            var query = "select SalaryReportJS.JobCode, SalaryReportJS.EmployeeName, DesignationName,DepartmentName, EmployeeUnion,EmployeeClub  from SalaryReportJS join Employees on Employees.JobCode = SalaryReportJS.JobCode where monthid=@monthid";

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    data.AddRange(await connection.QueryAsync<EmployeeUnionORClub>(query, new { monthid = monthId }));
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"An error occurred: {ex.Message}");
                }
            }

            return data;
        }



        public async Task<List<OfficerAssoOrClub>> OfficerAssoORClubData(int monthId, int reportType)
        {
            //string query = "";
            List<OfficerAssoOrClub> data = new List<OfficerAssoOrClub>();
            var query = "select SalaryReportOF.JobCode, SalaryReportOF.EmployeeName, DesignationName,DepartmentName, OfficerAssociation,OfficerClub from SalaryReportOF join Employees on Employees.JobCode = SalaryReportOF.JobCode where monthid=@monthid"; ;

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    data.AddRange(await connection.QueryAsync<OfficerAssoOrClub>(query, new { monthid = monthId }));
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"An error occurred: {ex.Message}");
                }
            }

            return data;
        }


        public async Task<List<PensionSheetViewModel>> GetPensionSheetData(int monthId, int employeeTypeId)
        {
            string query = "";
            List<PensionSheetViewModel> pensionSheet = new List<PensionSheetViewModel>();
            if (employeeTypeId == 3)
            {
                query = "select SalaryReportOF.JobCode,SalaryReportOF.BasicSalary, SalaryReportOF.EmployeeName, DesignationName, PensionOfficer as CompanyContribution from SalaryReportOF join Employees on Employees.JobCode = SalaryReportOF.JobCode where monthid=@monthid and Employees.EmployeeTypeId=3";
            }
            if (employeeTypeId == 4)
            {
                query = "select SalaryReportJS.JobCode,SalaryReportJS.BasicSalary, SalaryReportJS.EmployeeName, DesignationName, PensionCom as CompanyContribution from SalaryReportJS join Employees on Employees.JobCode = SalaryReportJS.JobCode where monthid=@monthid and Employees.EmployeeTypeId=4";
            }

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    pensionSheet.AddRange(await connection.QueryAsync<PensionSheetViewModel>(query, new { monthid = monthId }));
                }
                catch (Exception ex)
                {

                }
            }

            return pensionSheet;
        }




        public async Task<List<PensionSheetViewModel>> GetYearlyPensionSheetData(int employeeTypeId, List<string> jobCode, int fromMonthId, int toMonthId, string? department)
        {
            string query = string.Empty;
            List<PensionSheetViewModel> pensionSheet = new List<PensionSheetViewModel>();
            var parameters = new DynamicParameters();

            parameters.Add("fMonthID", fromMonthId, DbType.Int32);
            parameters.Add("tMonthID", toMonthId, DbType.Int32);

            // Check if jobCode is not null and contains items
            if (jobCode != null && jobCode.Count > 0)
            {
                query += " AND SalaryReportOF.JobCode IN @JobCodes";
                parameters.Add("JobCodes", jobCode);
            }

            if (employeeTypeId == 3)
            {
                query = @"
                        SELECT 
                            SalaryReportOF.JobCode,
                            SalaryReportOF.EmployeeName,
                            DesignationName, 
                            SUM(SalaryReportOF.BasicSalary) AS BasicSalary,
                            SUM(SalaryReportOF.PensionOfficer) AS CompanyContribution 
                        FROM SalaryReportOF 
                        JOIN Employees ON Employees.JobCode = SalaryReportOF.JobCode 
                        WHERE 
                            SalaryReportOF.MonthID BETWEEN @fMonthID AND @tMonthID 
                            AND Employees.EmployeeTypeId = 3";

                // Append jobCode condition if needed
                if (jobCode != null && jobCode.Count > 0)
                {
                    query += " AND SalaryReportOF.JobCode IN @JobCodes";
                }

                query += @"
                        GROUP BY 
                            SalaryReportOF.JobCode, 
                            SalaryReportOF.EmployeeName,
                            DesignationName;";
            }
            else if (employeeTypeId == 4)
            {
                query = @"
                        SELECT 
                            SalaryReportJS.JobCode, 
                            SalaryReportJS.EmployeeName, 
                            DesignationName, 
                            SUM(SalaryReportJS.BasicSalary) AS BasicSalary,
                            SUM(SalaryReportJS.PensionCom) AS CompanyContribution 
                        FROM SalaryReportJS 
                        JOIN Employees ON Employees.JobCode = SalaryReportJS.JobCode 
                        WHERE 
                            SalaryReportJS.MonthID BETWEEN @fMonthID AND @tMonthID 
                            AND Employees.EmployeeTypeId = 4";

                // Append jobCode condition if needed
                if (jobCode != null && jobCode.Count > 0)
                {
                    query += " AND SalaryReportJS.JobCode IN @JobCodes";
                }

                query += @"
                        GROUP BY 
                            SalaryReportJS.JobCode, 
                            SalaryReportJS.EmployeeName, 
                            DesignationName;";
                            }

            // Append department filter if provided
            if (!string.IsNullOrEmpty(department))
            {
                query += " AND Employees.DepartmentName = @DepartmentName";
                parameters.Add("DepartmentName", department, DbType.String);
            }

            using (var connection = _context.CreateConnection())
            {
                try
                {
                    pensionSheet.AddRange(await connection.QueryAsync<PensionSheetViewModel>(query, parameters));
                }
                catch (Exception ex)
                {
                    // Log the exception or handle it as needed
                    Console.WriteLine(ex.Message);
                }
            }

            return pensionSheet;
        }



        public async Task<List<PFDeductionViewModel>> GetPFDeductionData(List<string> jobCode, int monthId, int employeeTypeId)
        {
            string query = "";
            List<PFDeductionViewModel> pfDeductions = new List<PFDeductionViewModel>();
            var parameters = new DynamicParameters();

            parameters.Add("monthid", monthId, DbType.Int32);

            // Construct the query based on the employeeTypeId
            if (employeeTypeId == 1)
            {
                query = "SELECT JobCode, BasicSalary, EmployeeName, DesignationName, ProvidentFund AS OwnContribution FROM SalaryReportOF WHERE monthid = @monthid";
            }
            else if (employeeTypeId == 2)
            {
                query = "SELECT JobCode, BasicSalary, EmployeeName, DesignationName, ProvidentFund AS OwnContribution FROM SalaryReportJS WHERE monthid = @monthid";
            }

            // Add JobCode filter if provided
            if (jobCode != null && jobCode.Count > 0)
            {
                query += " AND JobCode IN @JobCodes";
                parameters.Add("JobCodes", jobCode);
            }

            // Execute the query using the dynamic parameters
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    pfDeductions = (await connection.QueryAsync<PFDeductionViewModel>(query, parameters)).ToList();
                }
                catch (Exception ex)
                {
                    // Handle the exception if necessary (e.g., log the error)
                    Console.WriteLine(ex.Message);
                }
            }

            return pfDeductions;
        }

        public async Task<string> GetLastMonthSalaryProcess()
        {
            var query = "SELECT TOP 1 MonthID FROM SalaryReportMaster ORDER BY SalaryProcessingID DESC;";

            using (var connection = _context.CreateConnection())
            {
                var lastMonthID = await connection.QueryFirstOrDefaultAsync<int?>(query);

                if (lastMonthID.HasValue)
                {
                    var year = lastMonthID.Value / 100;
                    var monthNumber = lastMonthID.Value % 100;
                    var date = new DateTime(year, monthNumber, 1);
                    var formattedDate = date.ToString("MMM") + " '" + date.ToString("yy");
                    return formattedDate;
                }
                return "No Data";
            }
        }

        public async Task<string> GetSalaryOfNetpayAsync()
        {
            var query = @"
        SELECT 
            SUM(
                ([BasicSalary] +
                [PersonalSalary] +
                [ArrearSalary] +
                [LikeBasic] +
                [OtherSalary] +
                [LunchAllow] +
                [TiffinAllow] +
                [SpecialBenefit] +
                [HouseRentAllow] +
                [FMAllow] +
                [WashAllow] +
                [EducationalAllow] +
                [FieldRiskAllow] +
                [ChargeAllow] +
                [DAidAllow] +
                [DeputationAllow] +
                [OtherAllow] +
                [Conveyance] +
                [CME])
                -
                ([RevenueStamp] +
                [WelfareFund] +
                [OfficerClub] +
                [OfficerAssociation] +
                [MedicalFund] +
                [ProvidentFund] +
                [Advance] +
                [IncomeTax] +
                [Other] +
                [SpecialDeduction] +
                [Hospitalisation] +
                [TMBill] +
                [HouseRentReturn] +
                [Dormitory] +
                [FuelReturn] +
                [HBLoan] +
                [MCylLoan] +
                [BCylLoan] +
                [ComLoan] +
                [PFLoan] +
                [WPFLoan] +
                [CosLoan] +
                [CarLoan] +
                [OtherLoan])
            ) AS TotalNetPay
        FROM
            SalaryReportOF
        WHERE 
            MonthID = (SELECT TOP 1 MonthID FROM SalaryReportMaster ORDER BY SalaryProcessingID DESC);
    ";

            using (var connection = _context.CreateConnection())
            {
                var netpay = await connection.QueryFirstOrDefaultAsync<decimal?>(query);

                // Format the net pay with commas and return it as a string.
                return netpay.HasValue ? netpay.Value.ToString("N0") : "0";
            }
        }

        public async Task<string> GetSalaryNetpayPermanent()
        {
            var query = @"
                        SELECT SUM(NetPay)
                FROM
                    SalaryReportOF
                WHERE 
                MonthID = (SELECT TOP 1 MonthID FROM SalaryReportMaster ORDER BY SalaryProcessingID DESC);
                ";

            using (var connection = _context.CreateConnection())
            {
                var netpay = await connection.QueryFirstOrDefaultAsync<decimal?>(query);
                return netpay.HasValue ? netpay.Value.ToString("N0") : "0";
            }
        }
        public async Task<Payrolldata> GetPayrollData(int monthId)
        {
            var queryOF = @"
                SELECT 
                    SUM(GrossPay) AS GrossPay, 
                    SUM(TotalDeduction) AS Deduction, 
                    SUM(NetPay) AS NetPay
                FROM SalaryReportOF
                WHERE MonthID = @monthId;
            ";

            var queryJS = @"
                SELECT 
                    SUM(GrossPay) AS GrossPay, 
                    SUM(TotalDeduction) AS Deduction, 
                    SUM(NetPay) AS NetPay
                FROM SalaryReportJS
                WHERE MonthID = @monthId;
            ";

            using (var connection = _context.CreateConnection())
            {
                var dataOF = await connection.QueryFirstOrDefaultAsync<Payrolldata>(queryOF, new { monthId });
                var dataJS = await connection.QueryFirstOrDefaultAsync<Payrolldata>(queryJS, new { monthId });

                var result = new Payrolldata
                {
                    GrossPay = (dataOF?.GrossPay ?? 0) + (dataJS?.GrossPay ?? 0),
                    Deduction = (dataOF?.Deduction ?? 0) + (dataJS?.Deduction ?? 0),
                    NetPay = (dataOF?.NetPay ?? 0) + (dataJS?.NetPay ?? 0)
                };

                return result;
            }
        }


        public async Task<string> GetSalaryGrossPermanent()
        {
            var query = @"
                        SELECT SUM(GrossPay)
                FROM
                    SalaryReportOF
                WHERE 
                MonthID = (SELECT TOP 1 MonthID FROM SalaryReportMaster ORDER BY SalaryProcessingID DESC);
                ";

            using (var connection = _context.CreateConnection())
            {
                var netpay = await connection.QueryFirstOrDefaultAsync<decimal?>(query);
                return netpay.HasValue ? netpay.Value.ToString("N0") : "0";
            }
        }
        public async Task<string> GetSalaryNetpayContract()
        {
            var query = @"
                        SELECT SUM(NetPay)
                FROM
                    SalaryReportJS
                WHERE 
                MonthID = (SELECT TOP 1 MonthID FROM SalaryReportMaster ORDER BY SalaryProcessingID DESC);
                ";

            using (var connection = _context.CreateConnection())
            {
                var netpay = await connection.QueryFirstOrDefaultAsync<decimal?>(query);
                return netpay.HasValue ? netpay.Value.ToString("N0") : "0";
            }
        }
        public async Task<string> GetSalaryGrossContract()
        {
            var query = @"
                        SELECT SUM(GrossPay)
                FROM
                    SalaryReportJS
                WHERE 
                MonthID = (SELECT TOP 1 MonthID FROM SalaryReportMaster ORDER BY SalaryProcessingID DESC);
                ";

            using (var connection = _context.CreateConnection())
            {
                var netpay = await connection.QueryFirstOrDefaultAsync<decimal?>(query);
                return netpay.HasValue ? netpay.Value.ToString("N0") : "0";
            }
        }
        public async Task<string> GetRevenueStampPermanent()
        {
            var query = @"
                        SELECT SUM(RevenueStamp)
                FROM
                    SalaryReportOF
                WHERE 
                MonthID = (SELECT TOP 1 MonthID FROM SalaryReportMaster ORDER BY SalaryProcessingID DESC);
                ";

            using (var connection = _context.CreateConnection())
            {
                var netpay = await connection.QueryFirstOrDefaultAsync<decimal?>(query);
                return netpay.HasValue ? netpay.Value.ToString("N0") : "0";
            }
        }
        public async Task<string> GetRevenueStampContruct()
        {
            var query = @"
                        SELECT SUM(RevenueStamp)
                FROM
                    SalaryReportJS
                WHERE 
                MonthID = (SELECT TOP 1 MonthID FROM SalaryReportMaster ORDER BY SalaryProcessingID DESC);
                ";

            using (var connection = _context.CreateConnection())
            {
                var netpay = await connection.QueryFirstOrDefaultAsync<decimal?>(query);
                return netpay.HasValue ? netpay.Value.ToString("N0") : "0";
            }
        }
        public async Task<string> GetPFPermanent()
        {
            var query = @"
                        SELECT SUM(ProvidentFund)
                FROM
                    SalaryReportOF
                WHERE 
                MonthID = (SELECT TOP 1 MonthID FROM SalaryReportMaster ORDER BY SalaryProcessingID DESC);
                ";

            using (var connection = _context.CreateConnection())
            {
                var netpay = await connection.QueryFirstOrDefaultAsync<decimal?>(query);
                return netpay.HasValue ? netpay.Value.ToString("N0") : "0";
            }
        }
        public async Task<string> GetPFContruct()
        {
            var query = @"
                        SELECT SUM(ProvidentFund)
                FROM
                    SalaryReportJS
                WHERE 
                MonthID = (SELECT TOP 1 MonthID FROM SalaryReportMaster ORDER BY SalaryProcessingID DESC);
                ";

            using (var connection = _context.CreateConnection())
            {
                var netpay = await connection.QueryFirstOrDefaultAsync<decimal?>(query);
                return netpay.HasValue ? netpay.Value.ToString("N0") : "0";
            }
        }
        public async Task<string> GetTotalDeductionPermanent()
        {
            var query = @"
                        SELECT SUM(TotalDeduction)
                FROM
                    SalaryReportOF
                WHERE 
                MonthID = (SELECT TOP 1 MonthID FROM SalaryReportMaster ORDER BY SalaryProcessingID DESC);
                ";

            using (var connection = _context.CreateConnection())
            {
                var netpay = await connection.QueryFirstOrDefaultAsync<decimal?>(query);
                return netpay.HasValue ? netpay.Value.ToString("N0") : "0";
            }
        }
        public async Task<string> GetTotalDeductionContruct()
        {
            var query = @"
                        SELECT SUM(TotalDeduction)
                FROM
                    SalaryReportJS
                WHERE 
                MonthID = (SELECT TOP 1 MonthID FROM SalaryReportMaster ORDER BY SalaryProcessingID DESC);
                ";

            using (var connection = _context.CreateConnection())
            {
                var netpay = await connection.QueryFirstOrDefaultAsync<decimal?>(query);
                return netpay.HasValue ? netpay.Value.ToString("N0") : "0";
            }
        }
        
        public async Task<string> GetSalaryNetpayJS()
        {
            var query = @"
        SELECT 
            SUM(
                (ISNULL([BasicSalary], 0) +
                 ISNULL([PersonalSalary], 0) +
                 ISNULL([ArrearSalary], 0) +
                 ISNULL([OtherSalary], 0) +
                 ISNULL([LunchAllow], 0) +
                 ISNULL([TiffinAllow], 0) +
                 ISNULL([ShiftAllow], 0) +
                 ISNULL([HouseRentAllow], 0) +
                 ISNULL([FamilyMedicalAllow], 0) +
                 ISNULL([EducationAllowance], 0) +
                 ISNULL([FieldAllow], 0) +
                 ISNULL([OtAllow], 0) +
                 ISNULL([UtilityAllow], 0) +
                 ISNULL([FuelAllow], 0) +
                 ISNULL([OtherAllow], 0) +
                 ISNULL([SpecialBenefit], 0))
                 -
                (ISNULL([RevenueStamp], 0) +
                 ISNULL([WelfareFund], 0) +
                 ISNULL([EmployeeClub], 0) +
                 ISNULL([EmployeeUnion], 0) +
                 ISNULL([ProvidentFund], 0) +
                 ISNULL([Dormitory], 0) +
                 ISNULL([HospitalDeduction], 0) +
                 ISNULL([FuelReturn], 0) +
                 ISNULL([SpecialDeduction], 0) +
                 ISNULL([Others], 0) +
                 ISNULL([Advance], 0) +
                 ISNULL([HBLoan], 0) +
                 ISNULL([MCylLoan], 0) +
                 ISNULL([BCylLoan], 0) +
                 ISNULL([ComputerLoan], 0) +
                 ISNULL([PFLoan], 0) +
                 ISNULL([WPFLoan], 0) +
                 ISNULL([CosLoan], 0))
            ) AS TotalNetPay
                FROM 
                    [bgfcl].[dbo].[SalaryReportJS]
                WHERE 
                    MonthID = (SELECT TOP 1 MonthID FROM SalaryReportMaster ORDER BY SalaryProcessingID DESC);
            ";

            using (var connection = _context.CreateConnection())
            {
                var netpay = await connection.QueryFirstOrDefaultAsync<decimal?>(query);
                return netpay.HasValue ? netpay.Value.ToString("N0") : "0";
            }
        }



        public async Task<List<PFDeductionViewModel>> YearlyGPFDeductionData(List<string> jobCodes, int fromMonthId, int toMonthId, string? department, int employeeTypeId)
        {
            List<PFDeductionViewModel> pfDeductions = new List<PFDeductionViewModel>();
            var parameters = new DynamicParameters();

            parameters.Add("fMonthID", fromMonthId, DbType.Int32);
            parameters.Add("tMonthID", toMonthId, DbType.Int32);

            string query = string.Empty;

            // Construct the query based on employeeTypeId
            if (employeeTypeId == 1)
            {
                query = @"
                    SELECT 
                        JobCode,
                        EmployeeName, 
                        DesignationName, 
                        Sum(ProvidentFund) AS OwnContribution  
                    FROM SalaryReportOF 
                    WHERE MonthID BETWEEN @fMonthID AND @tMonthID";
            }
            else if (employeeTypeId == 2)
            {
                query = @"
                    SELECT 
                        JobCode, 
                        EmployeeName, 
                        DesignationName, 
                        sum(ProvidentFund) AS OwnContribution  
                    FROM SalaryReportJS 
                    WHERE MonthID BETWEEN @fMonthID AND @tMonthID";
            }
            if (jobCodes != null && jobCodes.Count > 0)
            {
                query += " AND JobCode IN @JobCodes";
                parameters.Add("JobCodes", jobCodes);
            }
            // Append department condition if provided
            if (!string.IsNullOrEmpty(department))
            {
                query += " AND DepartmentName = @DepartmentName";
                parameters.Add("DepartmentName", department, DbType.String);
            }
            // Grouping and aggregation
            query += @" GROUP BY JobCode, EmployeeName,DesignationName";
            using (var connection = _context.CreateConnection())
            {
                try
                {
                    pfDeductions.AddRange(await connection.QueryAsync<PFDeductionViewModel>(query, parameters));
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"An error occurred: {ex.Message}");
                    // Handle exception as needed, e.g., logging to a file or rethrowing
                }
            }

            return pfDeductions;
        }

        public async Task<List<SalaryReportOfficer>> GetSalaryDataForTaxCertificate(string jobCode, int? departmentId, int fromMonthId, int toMonthId)
        {
            IEnumerable<SalaryReportOfficer> TaxCertificate = null;
            var parameters = new DynamicParameters();
            //parameters.Add("MonthID", monthid, DbType.Int32);
            parameters.Add("JobCode", jobCode, DbType.String);
            parameters.Add("fromMonthId", fromMonthId, DbType.Int32);
            parameters.Add("toMonthId", toMonthId, DbType.Int32);

            // Start constructing the query
            var query = "SELECT * FROM SalaryReportOF WHERE  JobCode =@JobCode and (MonthId between @fromMonthId and @toMonthId) ";

            if (departmentId.HasValue && departmentId.Value != 0)
            {
                query += " AND DepartmentId = @DepartmentId";
                parameters.Add("DepartmentId", departmentId.Value, DbType.Int32);
            }


            using (var connection = _context.CreateConnection())
            {
                try
                {
                    TaxCertificate = await connection.QueryAsync<SalaryReportOfficer>(query, parameters);
                }
                catch (Exception ex)
                {

                }
            }
            return TaxCertificate?.ToList() ?? new List<SalaryReportOfficer>();
        }



        #endregion
        public async Task<int> SalarySettinsConfig(List<SalarySettingConfigDetails> salarySettings)
        {
            int result = 0;

            if (salarySettings == null || !salarySettings.Any())
                return result;

            string deleteQuery = @"DELETE FROM dbo.SalarySettingConfigDetails 
                           WHERE EmployeeTypeId = @EmployeeTypeId;";

            string insertQuery = @"INSERT INTO dbo.SalarySettingConfigDetails 
                           (Field, InputType, Value, EmployeeTypeId, CreatedDate)
                           VALUES 
                           (@Field, @InputType, @Value, @EmployeeTypeId, @CreatedDate);";

            using (var connection = _context.CreateConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // delete old records
                        var employeeTypeId = salarySettings.First().EmployeeTypeId;
                        await connection.ExecuteAsync(deleteQuery, new { EmployeeTypeId = employeeTypeId }, transaction);

                        // insert all new records
                        foreach (var setting in salarySettings)
                        {
                            var parameters = new DynamicParameters();
                            parameters.Add("@Field", setting.Field, DbType.String);
                            parameters.Add("@InputType", setting.InputType, DbType.String);
                            parameters.Add("@Value", setting.Value, DbType.String);
                            parameters.Add("@EmployeeTypeId", setting.EmployeeTypeId, DbType.Int32);
                            parameters.Add("@CreatedDate", DateTime.Now, DbType.DateTime);

                            result += await connection.ExecuteAsync(insertQuery, parameters, transaction);
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Error in SalarySettinsConfig: " + ex.Message);
                    }
                }
            }

            return result; // this will return how many records were inserted
        }

        public async Task<int> InvestmentSchedule(InvestmentSchedule investment)
        {
            int result = 0;



            string Query = @"INSERT INTO dbo.InvestmentSchedule 
                           (Field, InputType, Value, EmployeeTypeId, CreatedDate)
                           VALUES 
                           (@Field, @InputType, @Value, @EmployeeTypeId, @CreatedDate);";

            using (var connection = _context.CreateConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Error in SalarySettinsConfig: " + ex.Message);
                    }
                }
            }

            return result; // this will return how many records were inserted
        }



    }
}
