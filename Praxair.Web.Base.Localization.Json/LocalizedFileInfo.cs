using System;
using System.Collections.Concurrent;
using System.IO;
using Microsoft.Extensions.FileProviders;

namespace Praxair.Web.Base.Localization.Json
{
    /// <summary>
    /// It represents and stores the information of a Localized file
    /// </summary>
    internal class LocalizedFileInfo
    {
        internal LocalizedFileInfo(string fileName, string culture, IFileInfo fileInfo)
        {
            FileName = fileName;
            Culture = culture;
            PhysicalPath = fileInfo.Exists ? fileInfo.PhysicalPath : null;
            InitialModified = DateTimeOffset.Now;
        }

        /// <summary>
        /// The suffix of the file (before _)
        /// </summary>
        public string FileName { get; } 
 
        /// <summary>
        /// The culture of the file
        /// </summary>
        public string Culture { get; }

        /// <summary>
        /// The physical path of the file. Null if file doesn't exist
        /// </summary>
        public string PhysicalPath { get; }

        /// <summary>       
        /// The modification datetime offset on object init
        /// </summary>
        public DateTimeOffset? InitialModified { get; set; }

        /// <summary>       
        /// The last modification datetime offset
        /// </summary>
        public DateTimeOffset? LastModified => PhysicalPath != null ? new DateTimeOffset?(File.GetLastWriteTime(PhysicalPath)) : null;

        /// <summary>       
        /// The stream of the file
        /// </summary>
        public Stream CreateReadStream()
        {
            if (PhysicalPath != null)
            {
                return File.OpenRead(PhysicalPath);
            }

            return null;
        }

        /// <summary>
        /// The localized strings of the file (flattened)
        /// </summary>
        public ConcurrentDictionary<string, string> Items { get; set; }
    }
}
