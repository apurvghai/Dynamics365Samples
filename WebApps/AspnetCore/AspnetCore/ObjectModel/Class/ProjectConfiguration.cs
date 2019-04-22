using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspnetCore.ObjectModel.Class
{
    public class ProjectConfiguration
    {

        public string Authority { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string TenantId { get; set; }

        public ProjectConfiguration(IConfiguration configuration)
        {
            Authority = configuration.GetValue<string>("OpenIdConnect:Authority");
            ClientId = configuration.GetValue<string>("OpenIdConnect:ClientId");
            ClientSecret = configuration.GetValue<string>("OpenIdConnect:ClientSecret");
            TenantId = configuration.GetValue<string>("OpenIdConnect:TenantId");


            //var Authority = _config.GetValue<string>("OpenIdConnect:Authority");
            //var ClientId = _config.GetValue<string>("OpenIdConnect:ClientId");
            //var ClientSecret = _config.GetValue<string>("OpenIdConnect:ClientSecret");
            //;
            //var TenantId = _config.GetValue<string>("OpenIdConnect:TenantId");
        }


        //    "Instance": "https://login.microsoftonline.com/",
        //"Domain": "apurvghai.com",
        //"TenantId": "d7cb2ddc-ed2a-452a-8825-8346e2c7e281",
        //"ClientId": "a0ad02cb-79ab-4a1a-a088-d2c34ed4581e",
        //"CallbackPath": "/signin-oidc",
        //"SaveTokens": true,
        //"AutomaticChallenge":  true
    }
}
