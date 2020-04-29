using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppClent
{
    class Program
    {
        static void Main(string[] args)
        {
            string token = GetToken();
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var apiUrlAzure =  $"{ConfigurationManager.AppSettings["BaseUrl"]}/api/values";
            using (var request = new HttpRequestMessage(HttpMethod.Get, apiUrlAzure))
            {
                var response = client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).GetAwaiter().GetResult();

                Console.WriteLine(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
            }

            Console.ReadLine();
        }


        #region Get an authentication access token
        private static string GetToken()
        {
            //Credencial criada para o App que vai chamar a API.
            var cred = new ClientCredential(ConfigurationManager.AppSettings["ClientId"], ConfigurationManager.AppSettings["ClientSecret"]);

            //ClientId da API
            string resourceUri = ConfigurationManager.AppSettings["Resource"];

            //OAuth2 authority Uri: https://login.microsoftonline.com + TenantId
            string authorityUri = "https://login.microsoftonline.com/94b1e746-0c85-4ddd-99b7-1e6ac22c732b";
                

            AuthenticationContext authContext = new AuthenticationContext(authorityUri);
            string token = authContext.AcquireTokenAsync(resourceUri, cred).GetAwaiter().GetResult().AccessToken;

            return token;
        }

        #endregion
    }
}
