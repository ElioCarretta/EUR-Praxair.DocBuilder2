using System.Collections.Generic;

namespace Praxair.Web.Base.Model.Navigation
{
    public class MenuItem
    {
        public virtual int Id { get; set; }
        public virtual string Title { get; set; }
        public virtual string Server { get; set; }
        public virtual string Path { get; set; }
        public virtual bool Hidden { get; set; }        

        /// <summary>
        /// If true, the search will stop at this element if the URL matches, otherwise it will search the child elements too and, in case it will find a child element that matches the URL, than it will return the child element.
        /// </summary>
        public bool StopSearchAtCurrentElement { get; set; }

        public virtual List<MenuItem> Menu { get; set; }
    }
}
