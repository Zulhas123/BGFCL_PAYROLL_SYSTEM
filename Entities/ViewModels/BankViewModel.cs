namespace Entities.ViewModels
{
    public class BankViewModel
    {
        public int Id { get; set; }
        public string BankName { get; set; }
        public string BankTagName { get; set; }
        public string BankTypeName { get; set; }
        public int? SchoolId { get; set; }
        public int? RoleId { get; set; }
        public int? GuestPkId { get; set; }
        public int? UserId { get; set; }
        public bool IsActive { get; set; }

    }
}
