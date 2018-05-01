/*
        Author : Apurv Ghai
        Description: The program has been distributed *as is* to help the community members and do not certify to be used for Production Use.
 */

using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;



/// <summary>
/// This helper provides facility to connect to Dynamics 365 using webapi and perform basic CRUD Operations.
/// </summary>
namespace Dyn365Samples
{
    public class WebAPIHelper
    {
        public string AccessToken { get; set; }
        public string BaseOrganizationApiUrl { get; set; }
        private const string Authority = "https://login.microsoftonline.com/<TenantID> or <domain.onmicrosoft.com>";
        private const string Resource = "https://YOURORG.crm.dynamics.com";
        static AuthenticationContext authContext = new AuthenticationContext(Authority, false);
        static AuthenticationResult authenticationResult;

        /// <summary>
        /// Use this method for Interactive authentication. Auth is handled by ADFS Login Prompt
        /// </summary>
        public async Task<string> GetTokenInteractiveAsync()
        {
            string clientId = "<< AppId>>";
            Uri redirectUrl = new Uri("<< Redirect URL >>");
            //Performance / Token Renewal 
            if (authenticationResult == null || DateTime.UtcNow.AddMinutes(15) >= authenticationResult.ExpiresOn)
                authenticationResult = await authContext.AcquireTokenAsync(Resource, clientId, redirectUrl, new PlatformParameters(PromptBehavior.Always));
            return authenticationResult.AccessToken;
        }

        /// <summary>
        /// Use this method if you are connecting your WebAPI/WebApp Azure App with client secret. If you are using this, 
        /// please make sure that Client or Application ID is registered as applicatio user in CRM
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetTokenNonInteractiveAsync()
        {
            string clientId = "<< AppId>>";
            ClientCredential clientCredentials = new ClientCredential(clientId, "<< Client secret >>");
            //Performance / Token Renewal 
            if (authenticationResult == null || DateTime.UtcNow.AddMinutes(15) >= authenticationResult.ExpiresOn)
                authenticationResult = await authContext.AcquireTokenAsync(Resource, clientCredentials);
            return authenticationResult.AccessToken;
        }


        /// <summary>
        /// This function can create any record type using Json Entity Objects
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="entityObject"></param>
        /// <returns></returns>
        public async Task CreateEntityRecords(string entityName, JObject entityObject)
        {
            using (var httpClient = GetClient(AccessToken, entityName))
            {
                HttpRequestMessage createHttpRequest = new HttpRequestMessage(HttpMethod.Post, BaseOrganizationApiUrl + "/api/data/v8.1/" + entityName);
                createHttpRequest.Content = new StringContent(entityObject.ToString(), Encoding.UTF8, "application/json");
                var response = await httpClient.SendAsync(createHttpRequest);
                response.EnsureSuccessStatusCode();
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    // Do something with response. Example get content:
                    Console.WriteLine("The record was created successfully.");
                    Console.ReadKey();
                }
            }
        }

        /// <summary>
        /// This function can create any record type by providing the Odata filters
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task SearchExistingRecord(string entityName, string filter)
        {
            using (var httpClient = GetClient(AccessToken, entityName))
            {
                string completedFilterCondition = BaseOrganizationApiUrl + "/api/data/v8.1/" + entityName + filter;
                var response = await httpClient.GetAsync(completedFilterCondition);
                response.EnsureSuccessStatusCode();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var objParsedContent = JsonConvert.DeserializeObject(content);
                    // Do something with response. Example get content:
                    Console.WriteLine(objParsedContent);
                    Console.WriteLine("Records Found");
                    Console.ReadKey();

                }
            }
        }


        public async Task DeleteExistingRecord(string entityName, JObject entityObject)
        {
            using (var httpClient = GetClient(AccessToken, entityName))
            {
                HttpRequestMessage createHttpRequest = new HttpRequestMessage(HttpMethod.Post, BaseOrganizationApiUrl + "/api/data/v8.1/" + entityName);
                createHttpRequest.Content = new StringContent(entityObject.ToString(), Encoding.UTF8, "application/json");
                var response = await httpClient.SendAsync(createHttpRequest);
                if (response.IsSuccessStatusCode)
                {
                    // Do something with response. Example get content:
                    Console.WriteLine("The record was created successfully.");
                    Console.ReadKey();
                }
            }
        }

        /// <summary>
        /// This function is responsible to update records. 
        /// </summary>
        /// <param name="entityName">Eg: /account(Guid-Here)</param>
        /// <param name="entityObject">JObject Entity values</param>
        /// <returns></returns>
        public async Task<object> UpdateExistingRecord(string entityName, JObject entityObject)
        {
            HttpClient httpClient;
            using (httpClient = GetClient(AccessToken, entityName))
            {
                HttpRequestMessage updateHttpMessage = null;
                updateHttpMessage = new HttpRequestMessage(new HttpMethod("PATCH"), BaseOrganizationApiUrl + "/api/data/v8.2/" + entityName);
                updateHttpMessage.Content = new StringContent(entityObject.ToString(), Encoding.UTF8, "application/json");
                var response = await httpClient.SendAsync(updateHttpMessage);
                if (response.IsSuccessStatusCode)
                {
                    var recordId = (response.Headers.Location.Segments[5].Split('('))[1].Remove(36);
                    return recordId;
                }
                return false;
            }
        }

        private HttpClient GetClient(string accessToken, string apiEntity)
        {

            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(360);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            client.BaseAddress = new Uri(BaseOrganizationApiUrl + "/api/data/v8.1/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            client.DefaultRequestHeaders.Add("OData-Version", "4.0");
            client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            return client;
        }
    }
}
