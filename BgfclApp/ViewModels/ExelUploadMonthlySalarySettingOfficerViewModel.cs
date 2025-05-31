namespace BgfclApp.ViewModels
{
    public class ExelUploadMonthlySalarySettingOfficerViewModel
    {
        public int Sl { get; set; }
        public string JobCode { get; set; }
        public string EmployeeName { get; set; }
        public int WorkDays { get; set; }
        public double ArrearSalary { get; set; }
        public double AdvanceSalary { get; set; }
        public double OtherAllow { get; set; }
        public double TMBill { get; set; }
        public double HospitalBill { get; set; }
        public double SpecialDeduction { get; set; }
        public double AdvanceDeduction { get; set; }
        public double OtherDeduction { get; set; }
    }
}
