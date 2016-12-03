/*
        Author : Apurv Ghai
        Description: The program has been distributed *as is* to help the community members and do not certify to be used for Production Use.
 */

using Crm_Sdk_Samples.WebAPI_Samples;
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
namespace Crm_Sdk_Samples
{
    public class WebApiOperationHelper
    {
        public string AccessToken { get; set; }

        public string BaseOrganizationApiUrl { get; set; }

        //Provides a persistent client-to-CRM server communication channel.
        HttpClient httpClient = new HttpClient();

        public void ObtainOAuthToken()
        {
            string clientId = "<< App ID Guid >>";
            Uri redirectUrl = new Uri("<< Redirect Url >>");
            AuthenticationParameters authParams = DiscoverAuthority();
            AuthenticationContext authContext = new AuthenticationContext(authParams.Authority, false);
            AuthenticationResult result = authContext.AcquireToken(authParams.Resource, clientId, redirectUrl, PromptBehavior.Always);
            if (result != null)
                AccessToken = result.AccessToken;
        }

        private AuthenticationParameters DiscoverAuthority()
        {
            AuthenticationParameters authParamters = AuthenticationParameters.CreateFromResourceUrlAsync
                (new Uri(BaseOrganizationApiUrl + "/api/data/v8.1")).Result;
            return authParamters;
        }

        public async Task CreateEntityRecords(string entityName, JObject entityObject)
        {
            httpClient = CreateDynHttpClient(AccessToken, entityName);
            HttpRequestMessage createHttpRequest = new HttpRequestMessage(HttpMethod.Post, BaseOrganizationApiUrl + "/api/data/v8.1/" + entityName);
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


        public async Task SearchExistingRecord(string entityName, string filter)
        {
            httpClient = CreateDynHttpClient(AccessToken, entityName);
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
