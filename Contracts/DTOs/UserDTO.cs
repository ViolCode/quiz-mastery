using Contracts.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [Display(Name = "Password")]
        public string PasswordHash { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        public Role Role { get; set; }
        public string RoleName => Role.ToString();
    }
}
