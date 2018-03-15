using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Praxair.Web.Base.Interfaces;
using Praxair.Web.Components.Model;

namespace Praxair.Web.Components.ViewComponents
{
    public class MenuBarViewComponent : ViewComponent
    {
        private readonly INavigationRepository _navigationRepository;
        private readonly IHostingEnvironment _environment;

        public MenuBarViewComponent(INavigationRepository navigationRepository, IHostingEnvironment environment)
        {
            _navigationRepository = navigationRepository;
            _environment = environment;
        }

        public async Task<IViewComponentResult> InvokeAsync(string brandLogo = null, string brandTitle = null, string brandUrl = null, string brand = null)
        {
            // Optional parameters are currently not supported with VC tag helper syntax
            // ViewComponentTagHelper: Allow optional parameters. https://github.com/aspnet/Razor/issues/1266

            var items = await _navigationRepository.GetSiteMapAsync();

            var model = new MenuBar
            {
                BrandLogo = brandLogo ?? $"/images/praxair-logo-{_environment.EnvironmentName.ToLower()}.png",
                BrandTitle = brandTitle ?? "Praxair | Making our planet more productive",
                BrandUrl = brandUrl ?? "www.praxair.com",
                Brand = brand ?? "Rivoira S.p.A.",
                Items = items
            };

            return View(model);
        }
    }
}
