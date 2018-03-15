using Praxair.Web.Base.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Praxair.Web.Base.Model.Account
{
    public class Credentials : ICredentials
    {
        [Display(Name = "UserName")]
        [Required]
        public virtual string UserName { get; set; }

        [Display(Name = "Password")]
        [Required]
        [DataType(DataType.Password)]
        public virtual string Password { get; set; }

        [Display(Name = "RememberMe")]
        public bool RememberMe { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
