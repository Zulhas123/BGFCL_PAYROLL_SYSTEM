namespace BgfclApp.ViewModels
{
    public class ExelUploadMonthlySalarySettingStaffViewModel
    {
        public int Sl { get; set; }
        public string JobCode { get; set; }
        public string EmployeeName { get; set; }
        public int WorkDays { get; set; }
        public int NumberOfShift { get; set; }
        public double ArrearSalary { get; set; }
        public double OtSingle { get; set; }
        public double OtDouble { get; set; }
        public double OtherAllow { get; set; }
        public double AdvanceDeduction { get; set; }
        public double OtherDeduction { get; set; }
        public double SpecialDeduction { get; set; }
        public double HospitalDeduction { get; set; }
    }
}
