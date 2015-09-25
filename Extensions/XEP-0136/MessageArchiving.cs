using Sharp.Xmpp.Core;
using Sharp.Xmpp.Im;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public Task GetArchive(int max, DateTimeOffset? start = null, DateTimeOffset? end = null, Jid with = null)
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

            var maxNode = Xml.Element("max").Text(max.ToString());
            var setNode = Xml.Element("set", "http://jabber.org/protocol/rsm").Child(maxNode);

            request.Child(setNode);

            var response = IM.IqRequest(IqType.Get, null, null, request);

            return Task.FromResult(true);
        }
    }
}