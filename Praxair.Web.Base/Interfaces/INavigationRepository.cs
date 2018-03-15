using System.Collections.Generic;
using System.Threading.Tasks;
using Praxair.Web.Base.Model.Navigation;

namespace Praxair.Web.Base.Interfaces
{
    public interface INavigationRepository
    {
        IList<MenuItem> GetSiteMap();
        Task<IList<MenuItem>>  GetSiteMapAsync();
    }
}
