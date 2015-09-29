using Sharp.Xmpp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sharp.Xmpp.Extensions
{
    /// <summary>
    /// Represents a single page of results in a result set, as specified by XEP-0059.
    /// </summary>
    public class XmppPage<T> : IEnumerable<T>
    {
        /// <summary>
        /// The items in this page
        /// </summary>
        public IList<T> Items { get; private set; }

        /// <summary>
        /// The id of the first item in this page
        /// </summary>
        public string First { get; private set; }

        /// <summary>
        /// The id of the last item in this page
        /// </summary>
        public string Last { get; private set; }

        /// <summary>
        /// The total number of items in the result set.
        /// </summary>
        public int TotalCount { get; private set; }

        /// <summary>
        /// The total number of items in this page
        /// </summary>
        public int PageCount { get { return Items.Count; } }

        /// <summary>
        /// Enumerator for items in this page
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        /// <summary>
        /// Enumerator for items in this page
        /// </summary>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        /// <summary>
        /// Create an XmppPage from the an xml node containing a set
        /// </summary>
        /// <param name="xml">The xml node containing a set</param>
        /// <param name="itemSelector">Function to select items from the xml node</param>
        internal XmppPage(XmlElement xml, Func<XmlElement, IList<T>> itemSelector)
        {
            xml.ThrowIfNull("xml");

            itemSelector.ThrowIfNull("itemSelector");

            Items = itemSelector(xml);

            var set = xml["set"];
            if (set != null)
            {
                try
                {
                    First = set["first"].InnerText;
                }
                catch
                {
                }

                try
                {
                    Last = set["last"].InnerText;
                }
                catch
                {
                }

                try
                {
                    TotalCount = int.Parse(set["count"].InnerText);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Generate a request to fetch the page that follows this page
        /// </summary>
        /// <returns></returns>
        public XmppPageRequest NextPageRequest()
        {
            return new XmppPageRequest(PageCount)
            {
                After = Last
            };
        }

        /// <summary>
        /// Generate a request to fetch the page that precedes this page
        /// </summary>
        public XmppPageRequest PreviousPageRequest()
        {
            return new XmppPageRequest(PageCount)
            {
                Before = First
            };
        }
    }
}