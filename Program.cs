using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Scraper
{
    class Program
    {
        static void Main(string[] args)
        {
            getHtmlAsync();
            Console.ReadLine();
        }

        private static async void getHtmlAsync()
        {
            var url = "https://www.ebay.co.uk/sch/i.html?_nkw=ps4+pro&_in_kw=1&_ex_kw=&_sacat=0&_udlo=&_udhi=&_ftrt=901&_ftrv=1&_sabdlo=&_sabdhi=&_samilow=&_samihi=&_sargn=-1%26saslc%3D1&_salic=3&_sop=12&_dmd=1&_ipg=200";

            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(url);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var productsHtml = htmlDocument.DocumentNode.Descendants("ul")
                .Where(node => node.GetAttributeValue("id", "")
                .Equals("ListViewInner")).ToList();

            var productsListItems = productsHtml[0].Descendants("li")
                .Where(node => node.GetAttributeValue("id", "")
                .Contains("item")).ToList();

            foreach(var productListItem in productsListItems)
            {
                //id
                Console.WriteLine(productListItem.GetAttributeValue("listingid", ""));

                //name
                Console.WriteLine(productListItem.Descendants("h3")
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("lvtitle")).FirstOrDefault().InnerText.Trim('\r', '\n', '\t'));

                //price
                Console.WriteLine(
                    Regex.Match(
                    productListItem.Descendants("li")
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("lvprice prc")).FirstOrDefault().InnerText.Trim('\r', '\n', '\t')
                    ,@"\d+.\d+")
                    );

                //listingType

                Console.WriteLine(
                    productListItem.Descendants("li")
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("lvformat")).FirstOrDefault().InnerText.Trim('\r', '\n', '\t'));

                //link
                Console.WriteLine(productListItem.Descendants("a").FirstOrDefault().GetAttributeValue("href", "").Trim('\r', '\n', '\t'));

                Console.WriteLine("------------------------------------------------------------------");
            }

        }
    }
}
