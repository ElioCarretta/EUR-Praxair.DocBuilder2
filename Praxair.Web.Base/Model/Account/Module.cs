using System.Collections.Generic;

namespace Praxair.Web.Base.Model.Account
{
    public class Module
    {
        public virtual string Id { get; set; }
        public virtual string Description { get; set; }
        public List<Role> Roles { get; set; }
    }
}
