using Sharp.Xmpp.Core;
using Sharp.Xmpp.Im;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sharp.Xmpp.Extensions
{
    internal class MessageArchiving : XmppExtension
    {
        private const string xmlns = "urn:xmpp:archive";

        public override IEnumerable<string> Namespaces
        {
            get
            {
                return new string[] { xmlns };
            }
        }

        public override Extension Xep
        {
            get { return Extension.MessageArchiving; }
        }

        public MessageArchiving(XmppIm im)
            : base(im)
        {
        }

        private static IList<ArchivedChatId> GetChatIdsFromStanza(XmlElement xml)
        {
            List<ArchivedChatId> chats = new List<ArchivedChatId>();
            var chatNodes = xml.GetElementsByTagName("chat");

            foreach (XmlNode node in chatNodes)
            {
                string with = null;
                try
                {
                    with = node.Attributes["with"].InnerText;
                }
                catch
                {
                }

                DateTimeOffset start = default(DateTimeOffset);
                try
                {
                    string startText = node.Attributes["start"].InnerText;
                    start = DateTimeProfiles.FromXmppString(startText);
                }
                catch
                {
                }

                chats.Add(new ArchivedChatId(with, start));
            }

            return chats;
        }

        public XmppPage<ArchivedChatId> GetArchivedChatIds(XmppPageRequest setRequest, DateTimeOffset? start = null, DateTimeOffset? end = null, Jid with = null)
        {
            var request = Xml.Element("list", xmlns);

            if (with != null)
            {
                request.Attr("with", with.ToString());
            }

            if (start != null)
            {
                request.Attr("start", start.Value.ToXmppDateTimeString());
            }

            if (end != null)
            {
                request.Attr("end", end.Value.ToXmppDateTimeString());
            }

            var setNode = setRequest.ToXmlElement();
            request.Child(setNode);

            var response = IM.IqRequest(IqType.Get, null, null, request);

            if (response.Type == IqType.Error)
            {
                throw Util.ExceptionFromError(response, "Failed to get archived chat ids");
            }

            return new XmppPage<ArchivedChatId>(response.Data["list"], GetChatIdsFromStanza);
        }
    }
}