using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using AspnetCore.Models;
using AspnetCore.ObjectModel.Class;
using AspnetCore.ObjectModel.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json.Linq;

namespace AspnetCore.Controllers
{
    [Route("api/crm/v1")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private const string DEFAULT_VERSION = "8.2";
        private ConcurrentDictionary<string, HttpClient> _tokenCacheCollection = new ConcurrentDictionary<string, HttpClient>();
        private ConcurrentDictionary<string, string> _resourceVersionCollection = new ConcurrentDictionary<string, string>();
        private IThirdPartyConnector _thirdPartyConnector { get; set; }
        private ProjectConfiguration _projectConfiguration { get; set; }
        private IHttpContextAccessor _httpContextAccesstor { get; set; }
        private async Task<HttpClient> GetHttpClientAsync(string resource)
        {
            if (_tokenCacheCollection.TryGetValue(resource, out var client))
            {
                return client;
            }
            var httpClient = new HttpClient() { Timeout = new TimeSpan(0, 5, 0) };
            var authenticationResult = await _thirdPartyConnector.GetAuthenticationTokenAsync(resource);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);
            return _tokenCacheCollection.GetOrAdd(resource, httpClient);
        }

        private string GetWebApiPath(string resource, string version)
        {
            return $"{resource}/api/data/v{version}";
        }

        private async Task<string> GetVersionAsync(string resource)
        {
            var IsVersionPresent = _resourceVersionCollection.TryGetValue(resource, out var version);
            if (!IsVersionPresent)
            {
                version = await TryGetOrgVersion(resource);
            }
            return version;
        }

        private async Task<string> TryGetOrgVersion(string resource)
        {
            var client = await GetHttpClientAsync(resource);
            var request = new HttpRequestMessage(new HttpMethod(Request.Method), $"{GetWebApiPath(resource, DEFAULT_VERSION)}/RetrieveVersion");
            var versionResponse = await request.TryParseResponse<RetrieveVersion>(client, User.Identity.Name);
            if (versionResponse.IsSuccessStatusCode)
            {
                var versionInfo = versionResponse.DeserializedContent.Version.Substring(0, 3);
                return _resourceVersionCollection.GetOrAdd(resource, versionInfo);
            }
            return null;
        }
        private async Task<ApiResponse<JObject>> WhoAmI(string resource)
        {
            var client = await GetHttpClientAsync(resource);
            var version = await GetVersionAsync(resource);
            var request = new HttpRequestMessage(new HttpMethod(Request.Method), $"{GetWebApiPath(resource, version)}/WhoAmI");
            return await request.TryParseResponse<JObject>(client, User.Identity.Name);
        }


        public ApiController(IServiceProvider serviceProvider)
        {
            _thirdPartyConnector = (IThirdPartyConnector)serviceProvider.GetService(typeof(IThirdPartyConnector));
            _projectConfiguration = (ProjectConfiguration)serviceProvider.GetService(typeof(ProjectConfiguration));
            _httpContextAccesstor = (IHttpContextAccessor)serviceProvider.GetService(typeof(IHttpContextAccessor));
        }

        [Route("User")]
        [HttpGet]
        public async Task<ApiResponse<JObject>> GetLoggedUser(string resource)
        {
            var client = await GetHttpClientAsync(resource);
            var version = await GetVersionAsync(resource);
            var whoAmiResponse = await WhoAmI(resource);
            if (whoAmiResponse.IsSuccessStatusCode)
            {
                var userId = whoAmiResponse.DeserializedContent.GetValue("UserId");
                var request = new HttpRequestMessage(new HttpMethod(Request.Method), $"{GetWebApiPath(resource, version)}/systemusers({userId})?$select=firstname, lastname, fullname");
                return await request.TryParseResponse<JObject>(client, User.Identity.Name);
            }
            return null;
        }

        [Route("Odata")]
        [HttpGet]
        public async Task<ApiResponse<JObject>> ExecuteOdataAsync(string resource, string path)
        {
            var client = await GetHttpClientAsync(resource);
            var version = await GetVersionAsync(resource);
            var request = new HttpRequestMessage(new HttpMethod(Request.Method), $"{GetWebApiPath(resource, version)}/{path}");
            return await request.TryParseResponse<JObject>(client, User.Identity.Name);

        }
    }
}