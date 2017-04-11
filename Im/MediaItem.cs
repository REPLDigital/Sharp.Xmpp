using System;
using System.Xml;

namespace Sharp.Xmpp.Im
{
	public class MediaItem
	{
		public string Id { get; private set; }
		public string Type { get; private set; }

		public MediaItem()
		{ 
		}

		public MediaItem(string id, string type)
		{
			Id = id;
			Type = type;
		}

		public static MediaItem GetMediaItem(XmlElement mediaItemNode)
		{
			MediaItem mediaItem = null;

			if (mediaItemNode != null)
			{
				var mediaIdAttribute = mediaItemNode.GetAttribute("id");
				var typeAttribute = mediaItemNode.GetAttribute("type");

				mediaItem = new MediaItem()
				{
					Id = mediaIdAttribute != null ? mediaIdAttribute : null,
					Type = typeAttribute != null ? typeAttribute : null
				};
			}

			return mediaItem;
		}
	}
}