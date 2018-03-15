using Microsoft.AspNetCore.Mvc;
using Praxair.Web.Components.Model;

namespace Praxair.Web.Components.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        //private readonly INavigationRepository _navigationRepository;
        //private readonly IHostingEnvironment _environment;

        //public FooterViewComponent(INavigationRepository navigationRepository, IHostingEnvironment environment)
        //{
        //    _navigationRepository = navigationRepository;
        //    _environment = environment;
        //}

        public IViewComponentResult Invoke(string brand = null)
        {
            // Optional parameters are currently not supported with VC tag helper syntax
            // ViewComponentTagHelper: Allow optional parameters. https://github.com/aspnet/Razor/issues/1266

            var model = new Footer
            {
                Brand = brand ?? "Rivoira S.p.A."
            };

            return View(model);
        }
    }
}
