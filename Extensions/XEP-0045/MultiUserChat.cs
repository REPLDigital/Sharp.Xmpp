using Sharp.Xmpp.Im;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sharp.Xmpp.Extensions
{
    internal class MultiUserChat : XmppExtension, IInputFilter<Presence>
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
        private ConcurrentDictionary<Jid, TaskCompletionSource<JoinRoomResult>> m_pendingRoomJoins = new ConcurrentDictionary<Jid, TaskCompletionSource<JoinRoomResult>>();
        private ConcurrentDictionary<Jid, TaskCompletionSource<bool>> m_pendingRoomLeaves = new ConcurrentDictionary<Jid, TaskCompletionSource<bool>>();

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

        public Task<JoinRoomResult> JoinRoom(Jid mucService, string roomName, string password = "")
        {
            const string ns = "http://jabber.org/protocol/muc";

            var tcs = new TaskCompletionSource<JoinRoomResult>();

            var roomJid = new Jid(mucService.Domain, roomName, IM.Jid.Node);
            m_pendingRoomJoins[roomJid] = tcs;

            var payload = Xml.Element("x", ns);
            if (!string.IsNullOrEmpty(password))
            {
                payload.Child(Xml.Element("password", ns).Text(password));
            }

            IM.SendPresence(new Presence(roomJid, IM.Jid, PresenceType.Available, null, null, payload));

            return tcs.Task;
        }

        public Task LeaveRoom(Jid mucService, string roomName, string status = "")
        {
            const string ns = "http://jabber.org/protocol/muc";

            var tcs = new TaskCompletionSource<bool>();

            var roomJid = new Jid(mucService.Domain, roomName, IM.Jid.Node);
            m_pendingRoomLeaves[roomJid] = tcs;

            XmlElement payload = null;
            if (!string.IsNullOrEmpty(status))
            {
                payload = Xml.Element("status", ns).Text(status);
            }

            IM.SendPresence(new Presence(roomJid, IM.Jid, PresenceType.Unavailable, null, null, payload));

            return tcs.Task;
        }

        public bool Input(Presence stanza)
        {
            //Handle error case
            if (stanza.Type == PresenceType.Error)
            {
                //Check if the error relates to a pending room join operation
                TaskCompletionSource<JoinRoomResult> pendingRoomJoin = null;
                if (m_pendingRoomJoins.TryGetValue(stanza.From, out pendingRoomJoin))
                {
                    pendingRoomJoin.SetException(Util.ExceptionFromError(stanza.Data["error"], "Failed to join room " + stanza.From.ToString()));
                    m_pendingRoomJoins.TryRemove(stanza.From, out pendingRoomJoin);

                    return true;
                }

                //Check if the error relates to a pending room leave operation
                TaskCompletionSource<bool> pendingRoomLeave = null;
                if (m_pendingRoomLeaves.TryGetValue(stanza.From, out pendingRoomLeave))
                {
                    pendingRoomLeave.SetException(Util.ExceptionFromError(stanza.Data["error"], "Failed to leave room " + stanza.From.ToString()));
                    m_pendingRoomLeaves.TryRemove(stanza.From, out pendingRoomLeave);

                    return true;
                }
            }

            //Handle success case
            var x = stanza.Data["x"];
            if (x != null && x.NamespaceURI == "http://jabber.org/protocol/muc#user")
            {
                var itemNode = x["item"];
                if (itemNode != null)
                {
                    //See if the result relates to a pending room join operation
                    TaskCompletionSource<JoinRoomResult> pendingRoomJoin = null;
                    if (m_pendingRoomJoins.TryGetValue(stanza.From, out pendingRoomJoin))
                    {
                        //Parse room affiliation and role
                        RoomAffiliation affiliation = RoomAffiliation.None;
                        if (itemNode.HasAttribute("affiliation"))
                        {
                            Enum.TryParse<RoomAffiliation>(itemNode.GetAttribute("affiliation"), out affiliation);
                        }

                        RoomRole role = RoomRole.None;
                        if (itemNode.HasAttribute("role"))
                        {
                            Enum.TryParse<RoomRole>(itemNode.GetAttribute("role"), out role);
                        }

                        var result = new JoinRoomResult()
                        {
                            Affiliation = affiliation,
                            Role = role
                        };

                        Exception createException = null;
                        //For rooms that don't exist, the server will create a new room and respond with a role of "owner"
                        if (affiliation == RoomAffiliation.Owner)
                        {
                            //Server should respond with status code 201 to indicate room created. Search for it...
                            //N.B. Prosody doesn't seem to include this node and just auto creates a default room
                            bool created = false;
                            foreach (XmlNode node in x.ChildNodes)
                            {
                                if (node.Name == "status")
                                {
                                    var code = node.Attributes["code"];
                                    if (code != null && code.InnerText == "201")
                                    {
                                        created = true;
                                        break;
                                    }
                                }
                            }

                            if (created)
                            {
                                //If the room was created, the server expects confirmation of room settings
                                //Send off a request to accept default instant room settings
                                Jid roomJid = new Jid(stanza.From.Domain, stanza.From.Node);
                                var response = IM.IqRequest(Core.IqType.Set, roomJid, IM.Jid, Xml.Element("query", "http://jabber.org/protocol/muc#owner").Child(Xml.Element("x", "jabber:x:data").Attr("type", "submit")));
                                if (response.Type == Core.IqType.Error)
                                {
                                    createException = Util.ExceptionFromError(response, "Failed to join room " + roomJid);
                                }
                            }
                        }

                        if (createException != null)
                        {
                            pendingRoomJoin.SetException(createException);
                        }
                        else
                        {
                            pendingRoomJoin.SetResult(result);
                        }

                        m_pendingRoomJoins.TryRemove(stanza.From, out pendingRoomJoin);

                        return true;
                    }

                    //See if the result relates to a pending room leave operation
                    TaskCompletionSource<bool> pendingRoomLeave = null;
                    if (m_pendingRoomLeaves.TryGetValue(stanza.From, out pendingRoomLeave))
                    {
                        pendingRoomLeave.SetResult(true);

                        m_pendingRoomLeaves.TryRemove(stanza.From, out pendingRoomLeave);

                        return true;
                    }
                }
            }

            return false;
        }
    }
}