using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace XMLHTTPParser
{
	public class Object
	{
		public Channel channel;
	}

	public class Channel
	{
		public string title;
		public string description;
		public string link;
		public List<Item> item;
	}

	public class Item
	{
		public string title;
		public string link;
		[JsonProperty("nyaa:seeders")]
		public string seeders;
		[JsonProperty("nyaa:leechers")]
		public string leechers;
		[JsonProperty("nyaa:downloads")]
		public string downloads;
		[JsonProperty("nyaa:size")]
		public string size;
	}

	class Program
	{
		static void Main(string[] args)
		{
			string url = "https://nyaa.si/?page=rss&q=Boku+no+Hero+Academia+horriblesubs+720&c=0_0&f=0";

			WebClient client = new WebClient();
			string xmlText = client.DownloadString(url);

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xmlText);

			string json = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.None, true);
			Object newObject = JsonConvert.DeserializeObject<Object>(json);
			Channel NyaaChannel = newObject.channel;

			List<Item> items = NyaaChannel.item.OrderBy(x => x.seeders).ToList();

			foreach(Item item in NyaaChannel.item)
			{
				Console.WriteLine(item.title);
				Console.WriteLine("Seeders: " + item.seeders + ", Leechers: " + item.leechers + "\n");

			}

			Console.WriteLine("Finished");
			Console.ReadLine();
		}
	}
}
