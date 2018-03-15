using System.Collections.Generic;
using Praxair.Web.Base.Model.Corporation;

namespace Praxair.Web.Base.Model.Account
{
    public class UserInfo
    {
        public virtual string Id { get; set; }
        public virtual string UserName { get; set; }
        public virtual string Name { get; set; }
        public virtual string Surname { get; set; }
        public virtual string Email { get; set; }
        public virtual string IdWindows { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual Company Company { get; set; }
        public virtual List<string> Provinces { get; set; }
    }
}
