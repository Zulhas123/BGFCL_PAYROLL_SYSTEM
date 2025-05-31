using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    using System.ComponentModel.DataAnnotations;

    public class User
    {
        [Key]
        public int UserId { get; set; }
        public int? UuId { get; set; } = 0; 

        public int? GuestPkId { get; set; } = 0;

        public int? SchoolId { get; set; } = 0;

        [Required]
        [Display(Name = "Username")]
        [StringLength(100, ErrorMessage = "Username must be less than 100 characters.")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [StringLength(255, ErrorMessage = "Password must be less than 255 characters.")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

}
