namespace Praxair.Web.Base.Localization.Json
{
    public class LocalizerItem
    {
        /// <summary>
        /// The translation identifier
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The translation
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// Tells if the traslation is from a parent culture
        /// </summary>
        public bool IsParent { get; set; }
    }
}
