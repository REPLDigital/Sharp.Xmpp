using Sharp.Xmpp.Core;
using System.Collections.Generic;
using System.Xml;

namespace Sharp.Xmpp.Extensions
{
    internal class DirectMucInvitations : XmppExtension
    {
        private const string xmlns = "jabber:x:conference";

        public override IEnumerable<string> Namespaces
        {
            get { return new string[] { xmlns }; }
        }

        public override Extension Xep
        {
            get { return Extension.DirectMucInvitations; }
        }

        public DirectMucInvitations(Sharp.Xmpp.Im.XmppIm im)
            : base(im)
        {
        }

        public void InviteUserToMuc(Jid mucService, string roomName, Jid userId, string reason = null, string password = null)
        {
            mucService.ThrowIfNull();
            roomName.ThrowIfNull();
            userId.ThrowIfNull();

            XmlElement inviteNode = Xml.Element("x", xmlns).Attr("jid", roomName + "@" + mucService.ToString());

            if (!string.IsNullOrEmpty(reason))
            {
                inviteNode.Attr("reason", reason);
            }

            if (!string.IsNullOrEmpty(password))
            {
                inviteNode.Attr("password", password);
            }

            IM.SendMessage(new Im.Message(new Message(userId, IM.Jid, inviteNode)));
        }
    }
}