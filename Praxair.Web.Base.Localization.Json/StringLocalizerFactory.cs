using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Praxair.Web.Base.Interfaces;

namespace Praxair.Web.Base.Localization.Json
{
    /// <inheritdoc />
    /// <summary>
    /// An <see cref="T:Microsoft.Extensions.Localization.IStringLocalizerFactory" /> that creates instances of <see cref="T:Praxair.Web.Base.Localization.Json.StringLocalizer" />.
    /// </summary>
    /// <remarks>
    /// <see cref="T:Praxair.Web.Base.Localization.Json.StringLocalizerFactory" /> 
    /// Uses a <see cref="T:Praxair.Web.Base.Localization.Json.LocalizationOptions" /> configuration in order to indentify the json files with the localizations
    /// </remarks>
    public class StringLocalizerFactory : IExtendedStringLocalizerFactory
    {
        #region Ctor

        private const string localizedFilesPattern = @"(?<fileName>[\w\.-]+)[_](?<culture>[A-Za-z]{2}|[A-Za-z]{2}[-][A-Za-z]{2})\.json";

        private readonly IFileProvider _fileProvider;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly LocalizationOptions _localizationOptions;

        private readonly ConcurrentDictionary<string, LocalizedFileInfo> _fileLocalizationsCache;
        private readonly ConcurrentDictionary<string, List<string>> _fileLocalizersCache;

        /// <summary>
        /// Creates a new <see cref="StringLocalizerFactory"/>.
        /// </summary>
        /// <param name="fileProvider">The <see cref="IFileProvider"/>.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
        /// <param name="memoryCache">The <see cref="IMemoryCache"/>.</param>
        /// <param name="localizationOptions">The <see cref="IOptions{T}"/>.</param>
        public StringLocalizerFactory(
            IFileProvider fileProvider,
            ILoggerFactory loggerFactory,
            IMemoryCache memoryCache,
            IOptions<LocalizationOptions> localizationOptions)
        {
            _fileProvider = fileProvider;
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _localizationOptions = localizationOptions?.Value ?? throw new ArgumentNullException(nameof(localizationOptions));

            _fileLocalizationsCache = new ConcurrentDictionary<string, LocalizedFileInfo>();
            GetLocalizationFiles(_fileLocalizationsCache, _localizationOptions.RootFolder);

            _fileLocalizersCache = new ConcurrentDictionary<string, List<string>>();

            WatchChanges();
        }

        #endregion

        #region IStringLocalizerFactory members

        /// <summary>
        /// Creates a <see cref="T:Praxair.Web.Base.Localization.Json.StringLocalizer" /> using the <see cref="T:System.Reflection.Assembly" /> and
        /// <see cref="P:System.Type.FullName" /> of the specified <see cref="T:System.Type" />.
        /// </summary>
        /// <param name="referredType">The <see cref="T:System.Type" />.</param>
        /// <returns>The <see cref="T:Praxair.Web.Base.Localization.Json.StringLocalizer" />.</returns>
        public IStringLocalizer Create(Type referredType)
        {
            if (referredType == null)
            {
                throw new ArgumentNullException(nameof(referredType));
            }

            var typeInfo = referredType.GetTypeInfo();
            var assembly = typeInfo.Assembly;
            var assemblyName = new AssemblyName(assembly.FullName);

            var baseName = typeInfo.FullName;
            var location = assemblyName.Name;

            return Create(baseName, location);
        }

        /// <summary>
        /// Creates a <see cref="T:Praxair.Web.Base.Localization.Json.StringLocalizer" />.
        /// </summary>
        /// <param name="location">The location of the object. To be used when location and base name are the same</param>
        /// <returns></returns>
        public IStringLocalizer Create(string location)
        {
            return Create(location, location);
        }

        /// <summary>
        /// Creates a <see cref="T:Praxair.Web.Base.Localization.Json.StringLocalizer" />.
        /// </summary>
        /// <param name="baseName">The base name of the object the localizations are referred to.</param>
        /// <param name="location">The location of the object.</param>
        /// <returns>The <see cref="T:Praxair.Web.Base.Localization.Json.StringLocalizer" />.</returns>
        public IStringLocalizer Create(string baseName, string location)
        {
            if (baseName == null)
            {
                throw new ArgumentNullException(nameof(baseName));
            }

            if (location == null)
            {
                throw new ArgumentNullException(nameof(location));
            }
         
            return new StringLocalizer(GetLocalizationsProvider, baseName, null, _loggerFactory.CreateLogger<StringLocalizer>());
        }

        #endregion

        #region Private methods

        private void GetLocalizationFiles(ConcurrentDictionary<string, LocalizedFileInfo> dictionaryCache, string parentFolder)
        {
            foreach (var directoryContent in _fileProvider.GetDirectoryContents(parentFolder))
            {
                if (!directoryContent.IsDirectory)
                {
                    var match = Regex.Match(directoryContent.Name, localizedFilesPattern);
                    if (match.Success)
                    {
                        var fileInfo = new LocalizedFileInfo(
                            $"{parentFolder}/{match.Groups["fileName"].Value}",
                            match.Groups["culture"].Value, 
                            directoryContent);

                        var fileInfoKey = $"{fileInfo.FileName}|{fileInfo.Culture}";

                        LoadFile(fileInfoKey, fileInfo, dictionaryCache);
                    }
                }
                else if (_localizationOptions.RecurseSubfolders)
                {
                    GetLocalizationFiles(dictionaryCache, $"{parentFolder}/{directoryContent.Name}");
                }
            }
        }

        private ConcurrentDictionary<string, string> FlatObject(object data, string path = null, ConcurrentDictionary<string, string> dictionary = null)
        {
            if (dictionary == null)
                dictionary = new ConcurrentDictionary<string, string>();

            if (data is JContainer obj)
            {
                if (obj is JProperty)
                {
                    var prop = obj as JProperty;
                    path = path == null ? prop.Name : $"{path}.{prop.Name}";
                }
                foreach (var child in obj.Children())
                {
                    FlatObject(child, path, dictionary);
                }
            }
            else
            {
                dictionary.TryAdd(path, data.ToString());
            }

            return dictionary;
        }

        private List<LocalizerItem> GetLocalizationsProvider(string baseName, CultureInfo culture)
        {
            var result = new List<LocalizerItem>();
            var localizerKey = $"{baseName.ToUpperInvariant()}|{culture.Name}";

            lock (_memoryCache)
            {
                // Look for a cached localizer
                if (!_memoryCache.TryGetValue(localizerKey, out var cacheEntry))
                {
                    result.AddRange(GetLocalizerItemsAndAddToCache(localizerKey));
                }
                else
                {
                    result = cacheEntry as List<LocalizerItem>;
                }
            }

            return result;
        }

        private List<LocalizerItem> GetLocalizerItemsAndAddToCache(string localizerKey)
        {
            var result = new List<LocalizerItem>();

            var splittedLocalizerKey = localizerKey.Split('|');
            var baseName = $"{splittedLocalizerKey[0]}.";
            var culture = new CultureInfo(splittedLocalizerKey[1]);

            bool SearchedLocatizations(KeyValuePair<string, string> item) =>
                item.Key.ToUpperInvariant().StartsWith(baseName);

            var stageCulture = culture;
            do
            {
                var files =
                _fileLocalizationsCache.Values
                    .Where(fileInfo =>
                        fileInfo.Culture.Equals(stageCulture.Name, StringComparison.InvariantCultureIgnoreCase) &&
                        fileInfo.Items.Any(SearchedLocatizations)
                    )
                    .ToList();

                var partialResult = files
                    .SelectMany(file => file.Items)
                    .Where(SearchedLocatizations)
                    .Select(item => new LocalizerItem
                    {
                        Name = item.Key.Replace(baseName, ""),
                        Value = item.Value,
                        IsParent = ReferenceEquals(stageCulture, culture)
                    })
                    .ToList();

                if (!ReferenceEquals(stageCulture, culture))
                {
                    partialResult = partialResult
                        .GroupJoin(
                            result,
                            x => x.Name,
                            y => y.Name,
                            (x, y) => new LocalizerItem
                            {
                                Name = x.Name,
                                Value = x.Value,
                                IsParent = x.IsParent
                            })
                        .SelectMany(
                            x => x.Name.DefaultIfEmpty(),
                            (x, y) => new LocalizerItem
                            {
                                Name = x.Name,
                                Value = x.Value,
                                IsParent = x.IsParent
                            })
                            .ToList();
                }

                if (partialResult.Any())
                {
                    result.AddRange(partialResult);

                    files.ForEach(file =>
                    {
                        if (_fileLocalizersCache.TryGetValue(file.FileName, out var fileLocalizerCache))
                        {
                            if (!fileLocalizerCache.Any(x =>
                                x.Equals(localizerKey, StringComparison.InvariantCultureIgnoreCase)))
                            {
                                fileLocalizerCache.Add(localizerKey);
                            }
                        }
                        else
                        {
                            _fileLocalizersCache.TryAdd(file.FileName, new List<string> { localizerKey });
                        }
                    });
                }

                stageCulture = stageCulture.Parent;
            } while (!String.IsNullOrWhiteSpace(stageCulture?.Name));

            if (result.Any())
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(60));

                _memoryCache.Set(localizerKey, result, cacheEntryOptions);
            }

            return result;
        }

        private void LoadFile(string fileInfoKey, LocalizedFileInfo fileInfo, ConcurrentDictionary<string, LocalizedFileInfo> dictionaryCache)
        {
            if (dictionaryCache.ContainsKey(fileInfoKey))
            {
                dictionaryCache.Remove(fileInfoKey, out fileInfo);
            }

            using (var reader = new StreamReader(fileInfo.CreateReadStream()))
            {
                var contents = JObject.Parse(reader.ReadToEnd()).ToObject(typeof(object));
                fileInfo.Items = FlatObject(contents);
            }

            fileInfo.InitialModified = DateTimeOffset.Now;
            dictionaryCache.TryAdd(fileInfoKey, fileInfo);
        }

        private void OnFileChange(object state)
        {
            lock (_memoryCache)
            {
                var filesChanged = _fileLocalizationsCache
                    .Where(x => x.Value.LastModified > x.Value.InitialModified)
                    .ToList();

                foreach (var fileChanged in filesChanged)
                {
                    // Load the changed file
                    LoadFile(fileChanged.Key, fileChanged.Value, _fileLocalizationsCache);

                    // Refresh the cached localizers
                    if (_fileLocalizersCache.TryGetValue(fileChanged.Value.FileName, out var localizerKeys))
                    {
                        foreach (var localizerKey in localizerKeys.ToList())
                        {
                            if (_memoryCache.TryGetValue(localizerKey, out var _))
                            {
                                _memoryCache.Remove(localizerKey);
                            }

                            GetLocalizerItemsAndAddToCache(localizerKey);
                        }
                    }
                }

                WatchChanges();
            }
        }

        private void WatchChanges()
        {
            var token = _fileProvider.Watch("**/*.json");
            token.RegisterChangeCallback(OnFileChange, token);
        }

        #endregion
    }
}
