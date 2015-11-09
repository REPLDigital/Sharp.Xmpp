using System;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace Sharp.Xmpp.Core
{
    /// <summary>
    /// Represents the base class for XML stanzas as are used by XMPP from which
    /// all implementations must derive.
    /// </summary>
    public abstract class Stanza
    {
        /// <summary>
        /// The XmlElement containing the actual data.
        /// </summary>
        protected XmlElement Element
        {
            get;
            set;
        }

        /// <summary>
        /// Specifies the JID of the intended recipient for the stanza.
        /// </summary>
        public Jid To
        {
            get
            {
                string v = Element.GetAttribute("to");
                return string.IsNullOrEmpty(v) ? null : new Jid(v);
            }

            set
            {
                if (value == null)
                    Element.RemoveAttribute("to");
                else
                    Element.SetAttribute("to", value.ToString());
            }
        }

        /// <summary>
        /// Specifies the JID of the sender. If this is null, the stanza was generated
        /// by the client's server.
        /// </summary>
        public Jid From
        {
            get
            {
                string v = Element.GetAttribute("from");
                return string.IsNullOrEmpty(v) ? null : new Jid(v);
            }

            set
            {
                if (value == null)
                    Element.RemoveAttribute("from");
                else
                    Element.SetAttribute("from", value.ToString());
            }
        }

        /// <summary>
        /// The ID of the stanza, which may be used for internal tracking of stanzas.
        /// </summary>
        public string Id
        {
            get
            {
                var v = Element.GetAttribute("id");
                return string.IsNullOrEmpty(v) ? null : v;
            }

            set
            {
                if (value == null)
                    Element.RemoveAttribute("id");
                else
                    Element.SetAttribute("id", value);
            }
        }

        /// <summary>
        /// The language of the XML character data if the stanza contains data that is
        /// intended to be presented to a human user.
        /// </summary>
        public CultureInfo Language
        {
            get
            {
                string v = Element.GetAttribute("xml:lang");
                return string.IsNullOrEmpty(v) ? null : new CultureInfo(v);
            }

            set
            {
                if (value == null)
                    Element.RemoveAttribute("xml:lang");
                else
                    Element.SetAttribute("xml:lang", value.Name);
            }
        }

        /// <summary>
        /// The data of the stanza.
        /// </summary>
        public XmlElement Data
        {
            get
            {
                return Element;
            }
        }

        /// <summary>
        /// Determines whether the stanza is empty, i.e. has no child nodes.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return Data.IsEmpty;
            }
        }

        /// <summary>
        /// Initializes a new instance of the Stanza class.
        /// </summary>
        /// <param name="namespace">The xml namespace of the stanza, if any.</param>
        /// <param name="to">The JID of the intended recipient for the stanza.</param>
        /// <param name="from">The JID of the sender.</param>
        /// <param name="id">The ID of the stanza.</param>
        /// <param name="language">The language of the XML character data of
        /// the stanza.</param>
        /// <param name="data">The content of the stanza.</param>
        public Stanza(string @namespace = null, Jid to = null,
            Jid from = null, string id = null, CultureInfo language = null,
            params XmlElement[] data)
        {
            string name = GetType().Name.ToLowerInvariant();
            Element = Xml.Element(name, @namespace);
            To = to;
            From = from;
            Id = id;
            Language = language;
            foreach (XmlElement e in data)
            {
                if (e != null)
                    Element.Child(e);
            }
        }

        /// <summary>
        /// Initializes a new instance of the Stanza class using the specified
        /// XmlElement.
        /// </summary>
        /// <param name="element">The XmlElement to create the stanza from.</param>
        /// <exception cref="ArgumentNullException">The element parameter is
        /// null.</exception>
        protected Stanza(XmlElement element)
        {
            element.ThrowIfNull("element");
            this.Element = element;
        }

        /// <summary>
        /// Converts the data XmlElement to an XElement.
        /// </summary>
        /// <returns>The data of the stanza as an XElement.</returns>
        public XElement DataXElememt()
        {
            return XElement.Parse(this.ToString());
        }

        /// <summary>
        /// Returns a textual representation of this instance of the Stanza class.
        /// </summary>
        /// <returns>A textual representation of this Stanza instance.</returns>
        public override string ToString()
        {
            return Element.ToXmlString();
        }
    }
}