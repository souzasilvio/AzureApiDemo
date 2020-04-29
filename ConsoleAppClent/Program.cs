using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
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
            //var apiUrl = "http://localhost:61605/api/values"
            var apiUrlAzure = "https://appdemowebapi.azurewebsites.net/api/values";
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
            string clientID = "a9e24fc4-5bf8-4f5c-9639-ef6fded5b3ce";
            string clientSecret = "kdA5/?jxo9Cq9IXrX4v]HpcjSRLMe:=3";
            var cred = new ClientCredential(clientID, clientSecret);

            //ClientId da API
            string resourceUri = "c603531b-1494-445c-8eb8-142dabcd2e5e";

            //OAuth2 authority Uri: https://login.microsoftonline.com + TenantId
            string authorityUri = "https://login.microsoftonline.com/94b1e746-0c85-4ddd-99b7-1e6ac22c732b";
                

            AuthenticationContext authContext = new AuthenticationContext(authorityUri);
            string token = authContext.AcquireTokenAsync(resourceUri, cred).GetAwaiter().GetResult().AccessToken;

            return token;
        }

        #endregion
    }
}
