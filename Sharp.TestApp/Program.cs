using System;
using Sharp.Xmpp;
using Sharp.Xmpp.Client;
using Sharp.Xmpp.Im;

namespace Sharp.TestApp
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			var host = "xmpp.tinderstone-labs.com";
			var user = "154e8169-0de3-43d4-93b2-fac3a7fbbf8e";
			var pass = "45e0e44b-5c0f-47bc-9480-6072bb5ad0b2";

			XmppClient m_xmppClient = new Xmpp.Client.XmppClient(host, user, pass);
			Jid to = new Jid("jid");
			var mediaItem = new MediaItem("1b82497d-b0f6-4693-a961-bc614c928602", "0");

			m_xmppClient.Connect();

			m_xmppClient.SendMessage(to, "body", "s", "t", Xmpp.Im.MessageType.Chat, null, mediaItem);
		}
	}
}
