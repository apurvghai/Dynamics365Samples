/*
        Author : Apurv Ghai
        Description: The program has been distributed *as is* to help the community members and do not certify to be used for Production Use.
 */

using Crm_Sdk_Samples.WebAPI_Samples;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class WebAPIOperationsHelper
    {
        //Provides a persistent client-to-CRM server communication channel.
        private HttpClient httpClient;

        public async Task ObtainOAuthToken(string clientId, string baseOrganizationUrl, string clientSecret)
        {
            AuthenticationParameters authParams = DiscoverAuthority(baseOrganizationUrl);
            AuthenticationContext authContext = new AuthenticationContext(authParams.Authority, false);
            ClientCredential clientCredential = new ClientCredential(clientId, clientSecret);
            AuthenticationResult result = await authContext.AcquireTokenAsync(authParams.Resource, clientCredential);
        }

        private AuthenticationParameters DiscoverAuthority(string baseOrganizationUrl)
        {
            AuthenticationParameters authParamters = AuthenticationParameters.CreateFromResourceUrlAsync(new Uri(baseOrganizationUrl + "/api/data/")).Result;
            return authParamters;
       }

        public async Task CreateEntityRecords(EntityType entityType)
        {
            entityType.entityAttribute = new JObject();
            entityType.entityAttribute.Add("firstname", "SDK First Name");
            entityType.entityAttribute.Add("lastname", "SDK Last Name");

            httpClient = await CreateDynHttpClient("dummy token", "api url", entityType.EntityLogicalName);


            HttpRequestMessage createHttpRequest = new HttpRequestMessage(HttpMethod.Post, entityType.EntityLogicalName);
            createHttpRequest.Content = new StringContent(entityType.entityAttribute.ToString(), Encoding.UTF8, "application/json");

            HttpResponseMessage createHttpResponse = await httpClient.SendAsync(createHttpRequest);

            if (createHttpResponse.StatusCode == HttpStatusCode.NoContent)  //204
            {
                //Console.WriteLine("Contact '{0} {1}' created.", contact1.GetValue("firstname"), contact1.GetValue("lastname"));
                //contact1Uri = createResponse1.Headers.GetValues("OData-EntityId").FirstOrDefault();
                //entityUris.Add(contact1Uri);
                //Console.WriteLine("Contact URI: {0}", contact1Uri);
            }
            else
            {
                //Console.WriteLine("Failed to create contact for reason: {0}", createResponse1.ReasonPhrase);
                throw new CrmHttpResponseException(createHttpResponse.Content);
            }

        }

        public async Task<HttpClient> CreateDynHttpClient(string accessToken, string apiUrl, string entityLogicalName)
        {
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = new TimeSpan(0, 0, 15);  // 2 minutes
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                client.BaseAddress = new Uri(apiUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                client.DefaultRequestHeaders.Add("OData-Version", "4.0");
                client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                HttpResponseMessage response = await client.GetAsync("/api/data/v8.0/" + entityLogicalName);
                //HttpResponseMessage response = await client.GetAsync("/api/data/v8.0/accounts?$select=name&$top=3");
                //Console.WriteLine(response);
                return client;
            }
        }
    }
}
