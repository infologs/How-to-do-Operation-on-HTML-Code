using HtmlAgilityPack;
using System;
using System.IO;
using System.Linq;
using System.Net;

namespace ScanWebsite
{
    class Program
    {
        public static string domainURL = "https://www.tutorialsteacher.com";
        static void Main(string[] args)
        {

            //DownloadPragimTech();

            DownloadTutorialTech();
            Console.ReadKey();
        }

        private static void DownloadTutorialTech()
        {
            var html = new HtmlDocument();
            
            html.LoadHtml(new WebClient().DownloadString(domainURL + "/core/"));

           

            HtmlNodeCollection htmlNodes = html.DocumentNode.SelectNodes("//ul[@id='leftmenu']");

            string leftPanel = htmlNodes[0].InnerHtml;

            html.LoadHtml(leftPanel);

            var anchors = html.DocumentNode.Descendants("a");

            int anchorCount = anchors.Count(), i = 0;

            string[] array = new string[anchorCount];


            foreach (var item in anchors)
            {
                if (item.Attributes["href"] != null && item.Attributes["href"].Value.ToLower().Contains("core"))
                {

                    string s = item.InnerText;

                    var articleDoc = new HtmlDocument();
                    articleDoc.LoadHtml(new WebClient().DownloadString(domainURL + item.Attributes["href"].Value));

                    HtmlNodeCollection article = articleDoc.DocumentNode.SelectNodes("//article");

                    string fullBody = article[0].InnerHtml;


                    var _checkforImage = new HtmlDocument();
                    _checkforImage.LoadHtml(fullBody);

                    HtmlNodeCollection images = _checkforImage.DocumentNode.SelectNodes("//img");

                    if (images != null && images.Count > 0)
                    {
                        foreach (var item1 in images)
                        {
                            string filename = item1.Attributes["src"].Value;

                            string[] data = filename.Split("../..");

                            DownloadImage(domainURL +  data[1], Guid.NewGuid().ToString() + ".png");
                            item1.Attributes["src"].Value = "../images/" + Guid.NewGuid().ToString() + ".png";
                        }

                    }

                    using (StreamWriter outputFile = new StreamWriter(Path.Combine(Path.GetFullPath("Articles"), s + ".html"), true))
                    {
                        outputFile.WriteLine(_checkforImage.DocumentNode.InnerHtml);
                    }

                    Console.WriteLine( s + " - Downloaded successfully.");

                }
            }
        }

        private static void DownloadPragimTech()
        {
            var html = new HtmlDocument();
            html.LoadHtml(new WebClient().DownloadString("https://www.pragimtech.com/courses/asp-net-core-mvc-tutorial-for-beginners/"));
            var root = html.DocumentNode;

            var anchors = root.Descendants("a");

            int anchorCount = anchors.Count();

            string s = "";
            string[] Titles = null;
            string[] urls = new string[anchorCount];

            Titles = new string[anchorCount];

            int i = 0;

            foreach (var item in anchors)
            {
                if (item.Attributes["href"].Value.ToLower().Contains("www.youtube.com") || item.Attributes["href"].Value.ToLower().Contains("youtu.be"))
                {
                    s = item.InnerText;
                    Titles[i] = s;
                }

                if (item.InnerText.ToLower() == "text")
                {
                    Console.WriteLine(s + " - " + item.Attributes["href"].Value + " - " + item.InnerText);
                    urls[i] = item.Attributes["href"].Value;
                    i++;

                    var _html = new HtmlDocument();
                    _html.LoadHtml(new WebClient().DownloadString(item.Attributes["href"].Value));
                    var _root = _html.DocumentNode;

                    HtmlNodeCollection node = _html.DocumentNode.SelectNodes("//div[@class='post-body entry-content']");

                    string fullBody = node[0].InnerHtml;


                    var _checkforImage = new HtmlDocument();
                    _checkforImage.LoadHtml(fullBody);

                    HtmlNodeCollection images = _checkforImage.DocumentNode.SelectNodes("//img");

                    if (images.Count > 0)
                    {
                        foreach (var item1 in images)
                        {
                            string filename = Path.GetFileName(item1.Attributes["src"].Value);
                            DownloadImage(item1.Attributes["src"].Value, Guid.NewGuid().ToString() + ".png");
                            item1.Attributes["src"].Value = "../images/" + Guid.NewGuid().ToString() + ".png";
                        }

                    }

                    HtmlNodeCollection scriptNodes = _checkforImage.DocumentNode.SelectNodes("//script");
                    foreach (HtmlNode script in scriptNodes)
                    {
                        script.Remove();
                    }


                    HtmlNodeCollection insNodes = _checkforImage.DocumentNode.SelectNodes("//ins");
                    foreach (HtmlNode insNode in insNodes)
                    {
                        insNode.Remove();
                    }

                    using (StreamWriter outputFile = new StreamWriter(Path.Combine(Path.GetFullPath("Articles"), s + ".html"), true))
                    {
                        outputFile.WriteLine(_checkforImage.DocumentNode.InnerHtml);
                    }

                    Console.WriteLine("File has successfully written.");

                }
            }
        }

        private static void DownloadImage(string filePath, string name)
        {
            using (WebClient webClient = new WebClient())
            {
                 string[] array = name.Split("?");
                
                 webClient.DownloadFile(filePath, "images\\" + array[0]);
                
                
            }

        }
    }
}
