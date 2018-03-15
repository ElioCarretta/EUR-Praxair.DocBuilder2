using System.Collections.Generic;
using System.Threading.Tasks;
using Praxair.Web.Base.Interfaces;
using Praxair.Web.Base.Model.Navigation;

namespace Praxair.Web.Base.Repositories
{
    public abstract class NavigationRepositoryBase : INavigationRepository
    {
        public abstract IList<MenuItem> GetSiteMap();

        public Task<IList<MenuItem>> GetSiteMapAsync()
        {
            return Task.Run(() =>
            {
                var siteMap = GetSiteMap();
                return siteMap;
            });
        }
    }
}
