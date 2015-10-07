using Sharp.Xmpp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Sharp.Xmpp.Extensions
{
    public static class Helper
    {
        public static XmlElement ToXmlElement(this XElement source)
        {
            if (source == null)
            {
                return null;
            }

            var xml = new XmlDocument();
            xml.LoadXml(source.ToString());
            return xml.DocumentElement;
        }

        public static void ThrowIfError(this Iq source)
        {
            if (source == null || source.Type != IqType.Error)
            {
                return;
            }

            var errorText = source.Data["error"].GetElementsByTagName("text")[0].InnerText; // todo: null checks
            throw new Exception(errorText);
        }
    }
}
