namespace BgfclApp.ViewModels
{
    public class VmDepartmentSummary
    {
        public string Department { get; set; }
        public decimal TotalBasicSalary { get; set; }
        public decimal TotalOther { get; set; }
        public double TotalTiffinAllow { get; set; }
        public double TotalFMLAllow { get; set; }
        public double TotalWFund { get; set; }
        public double TotalCPF { get; set; }
        public double TotalGrossPay { get; set; }
        public double TotalDeduction { get; set; }
        public double TotalNetPay { get; set; }
    }
}
