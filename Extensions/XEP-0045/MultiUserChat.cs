using Sharp.Xmpp.Im;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharp.Xmpp.Extensions
{
    internal class MultiUserChat : XmppExtension
    {
        public override IEnumerable<string> Namespaces
        {
            get
            {
                return new[]
                {
                    "http://jabber.org/protocol/muc",
                    "http://jabber.org/protocol/muc#user",
                    "http://jabber.org/protocol/muc#admin",
                    "http://jabber.org/protocol/muc#owner",
                };
            }
        }

        public override Extension Xep
        {
            get { return Extension.MultiUserChat; }
        }

        private ServiceDiscovery disco;

        public MultiUserChat(XmppIm im)
            : base(im)
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            disco = IM.GetExtension<ServiceDiscovery>();
        }

        public IList<Jid> GetMucServices()
        {
            List<Jid> result = new List<Jid>();
            var items = disco.GetItems(IM.Jid.Domain);

            foreach (var item in items)
            {
                var extensions = disco.GetExtensions(item.Jid);
                if (extensions.Contains(Extension.MultiUserChat))
                {
                    result.Add(item.Jid);
                }
            }

            return result;
        }

        public IList<XmppItem> GetRooms(Jid mucService)
        {
            return disco.GetItems(mucService).ToList();
        }
    }
}