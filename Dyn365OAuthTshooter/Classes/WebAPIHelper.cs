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

        //Provides a persistent client-to-CRM server communication channel.
        HttpClient httpClient = new HttpClient();

        /// <summary>
        /// This function will return the access token based on authority, ClientId and Dynamics 365 Url (Applies to Online and OnPremise)
        /// </summary>
        public async Task ObtainOAuthToken(string clientId, string redirectStringUrl, AuthenticationParameters authParams)
        {
            Uri redirectUrl = new Uri(redirectStringUrl);
            AuthenticationContext authContext = new AuthenticationContext(authParams.Authority);
            AuthenticationResult result = await     authContext.AcquireTokenAsync(authParams.Resource, clientId,
                redirectUrl, new PlatformParameters(PromptBehavior.Always));
            if (result != null)
                AccessToken = result.AccessToken;
        }

        /// <summary>
        /// This function returns the Authority Url (OAuth Endpoint) and Resource Url (Dynamics 365 Url)
        /// </summary>
        /// <returns></returns>
        public AuthenticationParameters DiscoverAuthority()
        {
            AuthenticationParameters authParamters = AuthenticationParameters.CreateFromResourceUrlAsync
                (new Uri(BaseOrganizationApiUrl + "/api/data/")).Result;
            return authParamters;
        }

        /// <summary>
        /// This function can create any record type using Json Entity Objects
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="entityObject"></param>
        /// <returns></returns>
        public async Task CreateEntityRecords(string entityName, JObject entityObject, string apiVersion)
        {
            httpClient = CreateDynHttpClient(AccessToken, entityName);
            HttpRequestMessage createHttpRequest = new HttpRequestMessage(HttpMethod.Post, BaseOrganizationApiUrl + "/api/data/" + apiVersion + "/" + entityName);
            createHttpRequest.Content = new StringContent(entityObject.ToString(), Encoding.UTF8, "application/json");
            var response = await httpClient.SendAsync(createHttpRequest);
            response.EnsureSuccessStatusCode();
            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                // Do something with response. Example get content:
                Console.WriteLine("The record was created successfully.");
                Console.ReadKey();
                //Dispose the Object :: Best Practice
                httpClient.Dispose();
            }
        }

        /// <summary>
        /// This function can create any record type by providing the Odata filters
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task SearchExistingRecord(string entityName, string filter, string apiVersion)
        {
            httpClient = CreateDynHttpClient(AccessToken, entityName);
            string completedFilterCondition = BaseOrganizationApiUrl + "/api/data/" + apiVersion + entityName + filter;
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
                //Dispose the Object :: Best Practice
                httpClient.Dispose();
            }
        }

        private HttpClient CreateDynHttpClient(string accessToken, string apiEntity)
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
