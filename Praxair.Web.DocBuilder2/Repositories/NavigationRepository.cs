using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Praxair.Web.Base.Model.Navigation;
using Microsoft.Extensions.Localization;
using Praxair.Web.Base.Interfaces;
using Praxair.Web.Base.Repositories;

namespace Praxair.Web.Demo.Repositories
{
    public class NavigationRepository : NavigationRepositoryBase
    {
        private readonly IStringLocalizer _localizer;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IUrlHelperFactory _urlHelperFactory;

        public NavigationRepository(IExtendedStringLocalizerFactory localizerFactory, IActionContextAccessor actionContextAccessor, IUrlHelperFactory urlHelperFactory)
        {
            _localizer = localizerFactory.Create("DOCUMENTS");
            _actionContextAccessor = actionContextAccessor;
            _urlHelperFactory = urlHelperFactory;
        }

        public override IList<MenuItem> GetSiteMap()
        {
            var request = _actionContextAccessor.ActionContext.HttpContext.Request;
            var host = request.Host.ToUriComponent();
            var pathBase = request.PathBase.ToUriComponent();
            var root = $"{request.Scheme}://{host}{pathBase}";
            var url = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            var index = 0;
            var menu =  new List<MenuItem>
                {
                    new MenuItem
                    {
                        Id = ++index,
                        Title = _localizer["PAGES.HOME"],
                        Server = root,
                        Path = String.Format("http://www.rivoiragroup.it/{0}", _localizer["LANG"]),
                        Menu = new List<MenuItem>
                        {
                            new MenuItem
                            {
                                Id = ++index,
                                Title = _localizer["PAGES.ROOT"],
                                Server = root,
                                Path = url.Link("Index", null),
                                Menu = new List<MenuItem>
                                {
                                    new MenuItem
                                    {
                                        Id = ++index,
                                        Title = _localizer["PAGES.SAFETYAREA.SAFETYDS"],
                                        Server = root,
                                        Path = url.Link("SafetyDataSheets", null),
                                        Menu = new List<MenuItem>()
                                    },
                                    new MenuItem
                                    {
                                        Id = ++index,
                                        Title = _localizer["PAGES.SAFETYAREA.PRECAUTIONSOFUSE"],
                                        Server = root,
                                        Path = url.Link("PrecautionsOfUse", null),
                                        Menu = new List<MenuItem>()
                                    },
                                    new MenuItem
                                    {
                                        Id = ++index,
                                        Title = _localizer["PAGES.INDICATIONSAREA.PRIVACY"],
                                        Server = root,
                                        Path = url.Link("InfoPrivacy", null),
                                        Menu = new List<MenuItem>()
                                    }
                                }
                            }
                        }
                    }
                };

            return menu;
        }
    }
}
