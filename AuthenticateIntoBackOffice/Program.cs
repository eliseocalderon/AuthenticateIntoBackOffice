using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticateIntoBackOffice
{
    class Program
    {
        static void Main(string[] args)
        {
            var getCookie = new GetAuthCookie();
            Task.Run(() => getCookie.SendRequest()).Wait();
            Console.ReadLine();
        }

        public class GetAuthCookie
        {
            public async Task SendRequest()
            {

                Uri uri = new Uri("https://office.uplocal.com/api/sso/authenticate?sessionId={045bdd55-0112-4a16-9cea-fef002a369a2}");
                //Uri uri = new Uri("https://office.uplocal.com/api/sso/authenticate");

                CookieContainer cookies = new CookieContainer();
                HttpClientHandler handler = new HttpClientHandler();
                handler.CookieContainer = cookies;

                using (var client = new HttpClient(handler))
                {
                    ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                    var response = await client.GetAsync(uri);

                    Console.WriteLine($"This is the result status code {response.StatusCode}");
                    var resultText = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"This is the result = {resultText}");

                    if (response.IsSuccessStatusCode)
                    {
                        var aspAuthCookie = cookies.GetCookies(uri).Cast<Cookie>().Where(x => x.Name == ".ASPXAUTH").FirstOrDefault();
                        var cookies2 = new CookieContainer();
                        if (aspAuthCookie != null)
                        {
                            cookies2.Add(aspAuthCookie);
                        }
                        HttpClientHandler handler2 = new HttpClientHandler
                        {
                            CookieContainer = cookies2
                        };
                        var client2 = new HttpClient(handler2);
                        uri = new Uri("https://office.uplocal.com/api/report/GetReportsAndSavedGroups");
                        response = await client2.GetAsync(uri);
                        response.EnsureSuccessStatusCode();
                        var json = await response.Content.ReadAsStringAsync();

                        Console.WriteLine(json);
                    }
                    Console.ReadLine();
                }
            }
        }

    }
}
