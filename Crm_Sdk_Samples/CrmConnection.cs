using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crm_Sdk_Samples
{
    public class CrmConnection
    {
        public OrganizationServiceProxy GetCrmConnection(string CrmOrganizationUrl, string crmUserName, string crmPassword, string crmDomain, bool isWindowAuth)
        {
            IServiceManagement<IOrganizationService> OrganizationServiceManagement = ServiceConfigurationFactory.CreateManagement<IOrganizationService>(new Uri(CrmOrganizationUrl));
            AuthenticationProviderType OrgAuthType = OrganizationServiceManagement.AuthenticationType;
            AuthenticationCredentials authCredentials = new AuthenticationCredentials();
            if (isWindowAuth)
            {
                authCredentials.ClientCredentials.Windows.ClientCredential.UserName = crmUserName;
                authCredentials.ClientCredentials.Windows.ClientCredential.Password = crmPassword;
                authCredentials.ClientCredentials.Windows.ClientCredential.Domain = crmDomain;
            }
            else
            {
                authCredentials.ClientCredentials.UserName.UserName = crmUserName;
                authCredentials.ClientCredentials.UserName.Password = crmPassword;
            }
            AuthenticationCredentials tokenCredentials = OrganizationServiceManagement.Authenticate(authCredentials);
            OrganizationServiceProxy organizationProxy;
            SecurityTokenResponse responseToken = tokenCredentials.SecurityTokenResponse;

            using (organizationProxy = new OrganizationServiceProxy(OrganizationServiceManagement, authCredentials.ClientCredentials)) { }
            return organizationProxy;
        }
    }
}
