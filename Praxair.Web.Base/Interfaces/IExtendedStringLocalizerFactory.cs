using Microsoft.Extensions.Localization;

namespace Praxair.Web.Base.Interfaces
{
    public interface IExtendedStringLocalizerFactory : IStringLocalizerFactory
    {
        IStringLocalizer Create(string location);
    }
}
