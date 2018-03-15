using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Praxair.Web.Base.Localization.Json
{
    public class StringLocalizer : IStringLocalizer
    {
        private readonly Func<string, CultureInfo, List<LocalizerItem>> _localizationsProvider;
        private readonly string _baseName;
        private readonly CultureInfo _culture;
        private readonly ILogger<StringLocalizer> _logger;

        internal StringLocalizer(Func<string, CultureInfo, List<LocalizerItem>> localizationsProvider, string baseName, CultureInfo culture, ILogger<StringLocalizer> logger)
        {
            _localizationsProvider = localizationsProvider;
            _baseName = baseName;
            _culture = culture ?? CultureInfo.CurrentUICulture;
            _logger = logger;
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return
            _localizationsProvider(_baseName, _culture)
                .Where(x => includeParentCultures || !x.IsParent)
                .Select(x => new LocalizedString(x.Name, x.Value, false, _baseName))
                .ToList();
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return new StringLocalizer(_localizationsProvider, _baseName, culture, _logger);
        }

        LocalizedString IStringLocalizer.this[string name]
        {
            get
            {
                if (name == null)
                {
                    throw new ArgumentNullException(nameof(name));
                }

                var value = GetString(name);

                return new LocalizedString(name, value ?? name, resourceNotFound: value == null, searchedLocation: _baseName);
            }
        }

        LocalizedString IStringLocalizer.this[string name, params object[] arguments]
        {
            get
            {
                if (name == null)
                {
                    throw new ArgumentNullException(nameof(name));
                }

                var format = GetString(name);
                var value = string.Format(format ?? name, arguments);

                return new LocalizedString(name, value, resourceNotFound: format == null, searchedLocation: _baseName);
            }
        }

        /// <summary>
        /// Gets a resource string from the cache and returns <c>null</c> if a match isn't found.
        /// </summary>
        /// <param name="name">The name of the string resource.</param>
        /// <returns>The resource string, or <c>null</c> if none was found.</returns>
        private string GetString(string name)
        {

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return _localizationsProvider(_baseName, _culture)
                .FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))?.Value;
        }
    }
}
