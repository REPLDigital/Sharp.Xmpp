using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sharp.Xmpp.Extensions
{
    public class AdHocCommand
    {
        public Jid Id { get; set; }
        public string Name { get; set; }
        public string Node { get; set; }

        public AdHocCommand()
        {
        }

        public AdHocCommand(XmlElement element)
        {
            Id = new Jid(element.Attributes["jid"].Value);
            Name = element.Attributes["name"].Value;
            Node = element.Attributes["node"].Value;
        }
    }
}
