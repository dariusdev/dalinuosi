using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using CryptoPunksCrowler;
using HtmlAgilityPack;

namespace Dalinuosi
{
    class Program
    {
        public static string url = "https://www.dalinuosi.lt/profilis-";
        public static string  CookieStr = "__cfduid=d13dffa9555d30bb1bdf8c0b377659d5a1609408754; csrfToken=06d73bc1b9863d24034f2cb87d081fec64d2e2c61e022ef9171c92f51f5bed884241a2a253620ea1d1411754955802391b57d48838f2548c55f8795993c6ff85; gtmClientId=1511514888.507624275; _pk_id.1.e83c=eea64689708cf87e.1609408758.; _ga=GA1.2.1511514888.507624275; __gads=ID=de5108a026871e6b-228b599078c50092:T=1609408758:RT=1609408758:S=ALNI_MZAAmIXG9bpPqrcPHPfr49wZJVOLw; _fbp=fb.1.1609408758596.370435527; _pubcid=19e15530-51fb-4e16-b9a9-68373e793aef; _gid=GA1.2.804192197.1609517164; DLNSESS=5e25f37ccb6efa3e7eea3203f07b83b0; Facebook=Q2FrZQ%3D%3D.MTA5ZTk4MmY5NzBkNzJhM2FmYTc4NGQ4YWY3N2UzZThmMTcyYzVjYTIzZDJkZDkxNjlkZDcwZGY0ZGVhNWNiZcbB1hvKUCU8pj%2BxmhjGLY6lsSI16MSwE7lTQvLCjYEjDroARFcBLXlqYKkDG7ARnUOvfjV4jAJeTcD%2BMxIE1ucd5pCFPF759uubBNmhLcBrCt0iVxKXGuaKO5CviK3K7lGYcaeHciS3MEZ6jpPKHH%2FIWLZBBArUad4ZeUoQcx4DvP%2FvwSvujO%2BrIqUkti8bb4nPGt6wkYRoG1wXBpnfp1LAvjhL9h9uk05gN2p2KBstgG0JgJ5X%2BvGdeXC04BC6OW0JMdonCOG1gnQYhW4Xe5cWrKZHy9FzvCbS9etS8dWDh38TzUsdy%2BimgudctDrIUA9kxgfSZudTmtJKIA9bx%2FQ%3D; _pk_ref.1.e83c=%5B%22%22%2C%22%22%2C1609636113%2C%22https%3A%2F%2Fwww.google.com%2F%22%5D; _pk_ses.1.e83c=1; cto_bidid=0b4TWl91YmdTR2pYWEx5NVclMkJpM1oxd2JwaEZJdEVmdG1ReTNIR1FYTDFONUwlMkJLSmslMkJNNDhLaTRLVkY3NXZMdmx4cUhNTEVweCUyQkgwbEpmSXpiclklMkJmb1B1cWtzdDhiTyUyQmc5NjRiUFYlMkYyRHZWZ3pjJTNE; cto_bundle=98oCsl9aNmc1N3Nlc0ZxUlFMWnpFekJ4WTF4VzhoNFklMkZhWGRPeHdnbnpmWEsyMEFDJTJCOTRmZEV1bmFvOXRvVkJBSmlzREswTkFpVFoycElLejk2VGNRUUZ2MEFlc2xMRmtGQW1iUnhsZEcxZDZIVkxnY2lvYkRkYVZBQyUyRkJmSk9NNHI2NllJVk11Z1J0UnZpU0RGRFd2eUh5bVElM0QlM0Q; _dc_gtm_UA-35443210-1=1";

        //https://www.dalinuosi.lt/profilis-4978

        static void Main(string[] args)
        {
            Console.WriteLine("Start!");

            var now = DateTime.Now.ToString("s", System.Globalization.CultureInfo.InvariantCulture);

            // iterate all set 70000  4978
            // 60869
            // 64789
            for (int i = 0; i <= 70000; i++)
            {
                Console.WriteLine($"{i} Start");

                DowloadItemsFromWeb(i, now);

                Console.WriteLine($"{i} Done");
            }


            Console.WriteLine("Done ALL!");
        }

        private static void DowloadItemsFromWeb(int i, string now)
        {
            string urlas = url + i;
            var html = GetHtmlString(urlas);

            if (String.IsNullOrEmpty(html))
            {
                Console.WriteLine($"{i} not found");
                return;
            }
            
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            HtmlNodeCollection commentNodes = doc.DocumentNode.SelectNodes("//div[@class='" + "comment" + "']");

            int commentsFound = 0;
                
            if (commentNodes != null)
            {
                List<Items> list = new List<Items>();
                commentsFound = commentNodes.Count();
                
                foreach (HtmlNode node in commentNodes)
                {
                    var value = node.SelectSingleNode(".//a");

                    // string name = value.InnerText;
                    string linkhtml = value.OuterHtml;

                    var item = list.FirstOrDefault(x => x.Key.Equals(linkhtml));

                    if (item != null)
                    {
                        item.Count = item.Count + 1;
                    }
                    else
                    {
                        list.Add(new Items(){Key = linkhtml, Count = 1});
                    }

                }
                
                
                SaveItems(list, urlas, now);
               
            }


            SaveUser(urlas, commentsFound, now);
            
            //Console.WriteLine($"Key: {i} done");

          //  Thread.Sleep(1000);
        }

   

        public static String GetHtmlString(string url)
        {
            HttpWebRequest myRequest = (HttpWebRequest) WebRequest.Create(url);
            myRequest.Method = "GET";
            WebResponse myResponse = new HttpWebResponse();


            CookieContainer cookiecontainer = new CookieContainer();
            string[] cookies = CookieStr.Split(';');
            foreach (string cookie in cookies)
                cookiecontainer.SetCookies(new Uri(url), cookie);
            myRequest.CookieContainer = cookiecontainer;
            
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    myRequest = (HttpWebRequest) WebRequest.Create(url);
                    myRequest.Method = "GET";
                    myRequest.CookieContainer = cookiecontainer;
                    myResponse = myRequest.GetResponse();
                    
                    
                    break;
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("(404) Not Found."))
                    {
                        return "";
                    }
                        
                    Console.WriteLine(e);
                    Console.WriteLine($"retry {i}");
                    Thread.Sleep(5000);
                }
            }
            
           
            StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
            string result = sr.ReadToEnd();
            
            sr.Close();
            myResponse.Close();

            if (result.Contains("Junkis su el. paštu"))
            {
                // return "";
                throw new Exception("disconnected");
            }

            return result;
        }
        
        private static void SaveItems(List<Items> list, string urlas, string now)
        {
            
            string fileName = now + "items.txt";
          
            string pattern = "{0},{1},{2}";
            
            foreach (var item in list)
            {
                var line = String.Format(pattern, item.Count, urlas, item.Key);

                using (StreamWriter sw = File.AppendText(fileName))
                {
                    sw.WriteLine(line);
                }
            }
        }
        
        private static void SaveUser(string url, int count, string now)
        {
            string fileName = now + "profiles.txt";
          
            string pattern = "{0},{1}";
           
            var line = String.Format(pattern, count, url);

            using (StreamWriter sw = File.AppendText(fileName))
            {
                sw.WriteLine(line);
            }
        }
    }
}