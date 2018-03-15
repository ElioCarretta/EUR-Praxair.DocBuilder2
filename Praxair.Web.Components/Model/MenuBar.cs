using System;
using System.Collections.Generic;
using System.Text;
using Praxair.Web.Base.Model.Navigation;

namespace Praxair.Web.Components.Model
{
    public class MenuBar
    {
        public string BrandUrl { get; set; }
        public string BrandTitle { get; set; }
        public string BrandLogo { get; set; }
        public string Brand { get; set; }
        public IList<MenuItem> Items { get; set; }
    }
}
