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
        // async static Task<Newtonsoft.Json.Linq.JObject> APICall(string APILink, string APIKey, string Data) {

        //     //In the next using statement you will initiate the Get Request, use the await keyword so it will execute the using statement in order.
        //     var client = new HttpClient();
        //     var useragent = "my-user-agent"; // THIS WAS ASKED BY THE API
        //     client.DefaultRequestHeaders.Add("User-Agent", useragent);
        //     var response = await client.GetAsync(APILink + "?" + Data + "&apiKey=" + APIKey);
        //     var responseContent = await response.Content.ReadAsStringAsync();

        //     var json = JObject.Parse(responseContent);
        //     //Console.WriteLine(json.ToString());
        //     return json;
        // }

        // static async Task Main(string[] args)
        // {
        //     Console.WriteLine("Choose options");
        //     Console.Write(@"
        //         1 - Top 10 News
        //         2 - iPhone News
        //     ");

        //     string U_input = Console.ReadLine();


        //     Newtonsoft.Json.Linq.JObject t;
        //     switch (U_input)
        //     { 
        //         case "1":
        //             t = await APICall("https://newsapi.org/v2/everything", "c8d5702f44ab45b7bb311d22fced51f8", "q=tech&pageSize=10&page=1");
        //             foreach (var item in t["articles"]) {
        //                 Console.WriteLine(item["title"] +" -- by "+ item["author"]);
        //                 Console.WriteLine("Follow here in this link - " + item["url"]);

        //                 Console.WriteLine("\n");

        //             }
        //             break;
        //         case "2":
        //             t = await APICall("https://newsapi.org/v2/everything", "c8d5702f44ab45b7bb311d22fced51f8", "q=iphone&pageSize=10&page=1");
        //             foreach (var item in t["articles"])
        //             {
        //                 Console.WriteLine(item["title"] + " -- by " + item["author"]);
        //                 Console.WriteLine("Follow here in this link - " + item["url"]);

        //                 Console.WriteLine("\n");

        //             }
        //             break;

        //     }

        // }
    
        static Dictionary<string, string> LoadUsers(string filepath)
        {
            Dictionary<string, string> users = new Dictionary<string, string>();

            if (!File.Exists(filepath))
            {
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.Write("");
                }
            }

            string[] lines = File.ReadAllLines(filepath);
            foreach (string line in lines)
            {
                string[] parts = line.Split('|');
                if (parts.Length == 2)
                {
                    string username = parts[0].Trim();
                    string password = parts[1].Trim();
                    users[username] = password;
                }
            }

            return users;
        }


        static async Task Main(string[] args){
            string url = "http://*:8080/";
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine("Listening for requests at " + url);

            while (true)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                Console.WriteLine("Received request: " + context.Request.HttpMethod + " " + context.Request.Url);

                if (context.Request.HttpMethod == "GET")
                {
                    // Handle GET request
                    await HandleGetRequest(context);
                }
                else if (context.Request.HttpMethod == "POST")
                {
                    // Handle POST request
                    await HandlePostRequest(context);
                }
                else
                {
                    // Handle other HTTP methods
                    context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                    context.Response.Close();
                }
            }
        }
        static async Task HandleGetRequest(HttpListenerContext context)
        {
            // Handle GET request here
            // Get the user and password parameters from the query string
            string user = context.Request.QueryString["user"];
            string pass = context.Request.QueryString["pass"];

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                // If either parameter is missing, return a bad request response
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.Close();
                return;
            }
            
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(await ReadUserData(user, pass));
            context.Response.ContentLength64 = buffer.Length;
            await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            context.Response.OutputStream.Close();
        }

        static async Task HandlePostRequest(HttpListenerContext context)
        {
            // Handle POST request here
            var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
            var body = await reader.ReadToEndAsync();

            var json = JObject.Parse(body);
            var user = json["user"]?.ToString();
            var pass = json["pass"]?.ToString();

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.Close();
                return;
            }

            var result = await ReadUserData(user, pass);

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(result);
            context.Response.ContentLength64 = buffer.Length;
            await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            context.Response.OutputStream.Close();
        }

        static async Task<String> ReadUserData(string User, string Pass){
            Dictionary<string, string> users = LoadUsers("Users.txt");
            Console.WriteLine("Loaded " + users.Count + " users from file.");

            // Example usage
            if (users.TryGetValue(User, out string password))
            {
                if (password == Pass)
                {
                    return("true");
                }
                else
                {
                    return("Invalid password");
                }
            }
            else
            {
                return("User "+User +" not found.");
            }
        }
    }
}