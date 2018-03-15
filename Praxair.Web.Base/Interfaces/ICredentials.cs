using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Praxair.Web.Base.Interfaces
{
    public interface ICredentials
    {
        [Display(Name = "UserName")]
        [Required]
        string UserName { get; set; }

        [Display(Name = "Password")]
        [Required]
        [DataType(DataType.Password)]
        string Password { get; set; }

        [Display(Name = "RememberMe")]
        bool RememberMe { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        string Email { get; set; }
    }
}
