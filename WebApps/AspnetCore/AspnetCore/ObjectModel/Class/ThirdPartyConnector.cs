using AspnetCore.ObjectModel.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspnetCore.ObjectModel.Class
{
    public class ThirdPartyConnector : IThirdPartyConnector
    {
        private readonly string assertionType = "urn:ietf:params:oauth:grant-type:jwt-bearer";
        private IHttpContextAccessor _contextAccessor { get; set; }
        private ProjectConfiguration _projectConfig { get; set; }
        private static AuthenticationContext _context = null;
        private static AuthenticationResult _result = null;
        private static string _httpAccessToken = string.Empty;
        public ThirdPartyConnector(ProjectConfiguration projectConfig, IHttpContextAccessor contextAccessor)
        {
            _projectConfig = projectConfig;
            _contextAccessor = contextAccessor;
            if (_context == null)
                _context = new AuthenticationContext(_projectConfig.Authority + _projectConfig.TenantId);
        }

        public async Task<AuthenticationResult> GetAuthenticationTokenAsync(string url)
        {
            var httpContext = _contextAccessor.HttpContext;
            var currrentPrincipal = httpContext.User;
            string email = currrentPrincipal.FindFirst(ClaimTypes.Upn).Value;
            if (string.IsNullOrEmpty(_httpAccessToken))
                _httpAccessToken = await httpContext.GetTokenAsync("id_token");
            var userAssertion = new UserAssertion(_httpAccessToken, assertionType, email);
            var clientCredentials = new ClientCredential(_projectConfig.ClientId, _projectConfig.ClientSecret);
            if (_result == null || DateTime.UtcNow.AddMinutes(15) >= _result.ExpiresOn)
                _result = await _context.AcquireTokenAsync(url, clientCredentials, userAssertion);
            return _result;
        }
    }
}
