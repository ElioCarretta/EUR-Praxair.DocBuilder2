using Praxair.Web.Base.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Praxair.Web.Part.Login.IdentityServer.Models.AccountViewModels
{
    public class LoginViewModel : ICredentials
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
