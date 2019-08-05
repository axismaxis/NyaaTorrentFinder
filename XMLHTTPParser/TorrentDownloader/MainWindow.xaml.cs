using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Xml;
using System.IO;
using System.Reflection;

namespace TorrentDownloader
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			List<Item> items = WebsiteLoader.GetChannelFromNyaa();

			List<Torrent> torrentsToDisplay = new List<Torrent>();
			foreach(Item item in items)
			{
				torrentsToDisplay.Add(new Torrent() {
					Name = item.title,
					SeedersInt = Int16.Parse(item.seeders),
					Seeders = (item.seeders + "\t/ " + item.leechers),
					Link = item.link
				});
			}
			torrentsToDisplay = torrentsToDisplay.OrderByDescending(x => x.SeedersInt).ToList();
			listView.ItemsSource = torrentsToDisplay;

			Assembly asm = Assembly.GetExecutingAssembly();
			string path = System.IO.Path.GetDirectoryName(asm.Location);

			using (WebClient client = new WebClient())
			{
				for(int i = 0; i < 3; i++)
				{
					//client.DownloadFile(torrentsToDisplay[i].Link, "torrent" + i + ".torrent");
					//string pathToTorrent = Path.Combine(path, "torrent" + i + ".torrent");
					//Process.Start(pathToTorrent);
				}
			}
		}

		public class Torrent
		{
			public string Name { get; set; }
			public int SeedersInt { get; set; }
			public string Seeders { get; set; }

			public string Link { get; set; }
		}

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

		public class WebsiteLoader
		{
			public static void DownloadAndOpenTorrent(string link)
			{
				Assembly asm = Assembly.GetExecutingAssembly();
				string path = System.IO.Path.GetDirectoryName(asm.Location);

				using (WebClient client = new WebClient())
				{
					Guid newGuid = Guid.NewGuid();
					client.DownloadFile(link, newGuid + ".torrent");
					string pathToTorrent = Path.Combine(path, newGuid + ".torrent");
					Process.Start(pathToTorrent);
				}
			}

			public static List<Item> GetChannelFromNyaa(string searchField = "")
			{
				string url;
				if (String.IsNullOrEmpty(searchField))
				{
					url = "https://nyaa.si/?page=rss&q=Tate+no+Yuusha+no+Nariagari+horriblesubs+720&c=0_0&f=0";
				}
				else
				{
					url = "https://nyaa.si/?page=rss&q=";
					url += searchField.Replace(' ', '+');
					url += "&c=0_0&f=0";
				}
				 

				WebClient client = new WebClient();
				string xmlText = client.DownloadString(url);

				XmlDocument doc = new XmlDocument();
				doc.LoadXml(xmlText);

				string json = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.None, true);
				Object newObject = JsonConvert.DeserializeObject<Object>(json);

				return newObject.channel.item;
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{

		}

		private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
		{
			TextBlock tb = (TextBlock)sender;
			string linkToTorrent = tb.Text;

			WebsiteLoader.DownloadAndOpenTorrent(linkToTorrent);

			Console.WriteLine("We got a winner: " + tb.Text);
		}

		private void SearchField_KeyDown(object sender, KeyEventArgs e)
		{
			if(e.Key == Key.Enter)
			{
				List<Item> items = WebsiteLoader.GetChannelFromNyaa(SearchField.Text);

				List<Torrent> torrentsToDisplay = new List<Torrent>();
				foreach (Item item in items)
				{
					torrentsToDisplay.Add(new Torrent()
					{
						Name = item.title,
						SeedersInt = Int16.Parse(item.seeders),
						Seeders = (item.seeders + "\t/ " + item.leechers),
						Link = item.link
					});
				}
				torrentsToDisplay = torrentsToDisplay.OrderByDescending(x => x.SeedersInt).ToList();
				listView.ItemsSource = torrentsToDisplay;
			}
		}
	}
}
