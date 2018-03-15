namespace Praxair.Web.Base.Localization.Json
{
    /// <summary>
    /// Provides programmatic configuration for localization.
    /// </summary>
    public class LocalizationOptions
    {
        /// <summary>
        /// Root folder where to search localized files. 
        /// Default: i18n
        /// </summary>
        public string RootFolder { get; set; } = "i18n";

        /// <summary>
        /// Tells StringLocalizerFactory to search localized files in subfolders. 
        /// Default: true
        /// </summary>
        public bool RecurseSubfolders { get; set; } = true;
    };
}
