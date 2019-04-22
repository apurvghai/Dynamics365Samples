using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspnetCore.ObjectModel.Interface
{
    public interface IThirdPartyConnector
    {
        Task<AuthenticationResult> GetAuthenticationTokenAsync(string url);
    }
}
