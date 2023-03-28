using System;
using System.Diagnostics;
using Newtonsoft;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;

namespace Program { 
    class Program
    {
        async static Task<Newtonsoft.Json.Linq.JObject> APICall(string APILink, string APIKey, string Data) {

            //In the next using statement you will initiate the Get Request, use the await keyword so it will execute the using statement in order.
            var client = new HttpClient();
            var useragent = "my-user-agent"; // THIS WAS ASKED BY THE API
            client.DefaultRequestHeaders.Add("User-Agent", useragent);
            var response = await client.GetAsync(APILink + "?" + Data + "&apiKey=" + APIKey);
            var responseContent = await response.Content.ReadAsStringAsync();

            var json = JObject.Parse(responseContent);
            //Console.WriteLine(json.ToString());
            return json;
        }

        static async Task Main(string[] args)
        {
            Console.WriteLine("Choose options");
            Console.Write(@"
                1 - Top 10 News
                2 - iPhone News
            ");

            string U_input = Console.ReadLine();


            Newtonsoft.Json.Linq.JObject t;
            switch (U_input)
            { 
                case "1":
                    t = await APICall("https://newsapi.org/v2/everything", "c8d5702f44ab45b7bb311d22fced51f8", "q=tech&pageSize=10&page=1");
                    foreach (var item in t["articles"]) {
                        Console.WriteLine(item["title"] +" -- by "+ item["author"]);
                        Console.WriteLine("Follow here in this link - " + item["url"]);

                        Console.WriteLine("\n");

                    }
                    break;
                case "2":
                    t = await APICall("https://newsapi.org/v2/everything", "c8d5702f44ab45b7bb311d22fced51f8", "q=iphone&pageSize=10&page=1");
                    foreach (var item in t["articles"])
                    {
                        Console.WriteLine(item["title"] + " -- by " + item["author"]);
                        Console.WriteLine("Follow here in this link - " + item["url"]);

                        Console.WriteLine("\n");

                    }
                    break;

            }

        }
    }
}