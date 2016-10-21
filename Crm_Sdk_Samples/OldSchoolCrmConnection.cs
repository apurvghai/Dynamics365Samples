
/*
        Author : Apurv Ghai
        Description: The program has been distributed *as is* to help the community members and do not certify to be used for Production Use.
 */

//.NET DLLS
using System;
using System.ServiceModel.Description;

//CRM SDK DLLS
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Discovery;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Query;



namespace Crm_Sdk_Samples
{
    public class OldSchoolCrmConnection
    {
        public string GblUser { get; set; }
        public string GblPassword { get; set; }
        public string GblDomain { get; set; }

        //Using Global Variables : Credentials


        /// <summary>
        /// Using the service to discover the organizations
        /// </summary>
        /// <param name="discoveryUrl"></param>
        /// <returns></returns>
        public OrganizationDetailCollection DiscoverOrganizations(string discoveryUrl)
        {
            IDiscoveryService service = GetDiscoveryProxy(discoveryUrl);

            RetrieveOrganizationsRequest orgsRequest = new RetrieveOrganizationsRequest()
            {
                AccessType = EndpointAccessType.Default,
                Release = OrganizationRelease.Current
            };
            RetrieveOrganizationsResponse organizations = (RetrieveOrganizationsResponse)service.Execute(orgsRequest);
            return organizations.Details;
        }

        // <summary>
        /// This function will return the Type of proxy Specified
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TProxy"></typeparam>
        /// <param name="serviceManagement"></param>
        /// <param name="authCredentials"></param>
        /// <param name="Url"></param>
        /// <param name="IsDiscovery"></param>
        /// <returns></returns>
        private TProxy GetProxy<TService, TProxy>(IServiceManagement<TService> serviceManagement, AuthenticationCredentials authCredentials,
            String Url, Boolean IsDiscovery)
            where TService : class
            where TProxy : ServiceProxy<TService>
        {
            Type classType = typeof(TProxy);
            serviceManagement = ServiceConfigurationFactory.CreateManagement<TService>(new Uri(Url));
            AuthenticationProviderType OrgAuthType = serviceManagement.AuthenticationType;
            authCredentials = GetCredentials(OrgAuthType);
            AuthenticationCredentials tokenCredentials = serviceManagement.Authenticate(authCredentials);
            SecurityTokenResponse responseToken = tokenCredentials.SecurityTokenResponse;
            if (IsDiscovery)
            {
                return (TProxy)classType
                    .GetConstructor(new Type[] { typeof(IServiceManagement<TService>), typeof(SecurityTokenResponse) })
                    .Invoke(new object[] { serviceManagement, responseToken });
            }
            else
                return (TProxy)classType
                    .GetConstructor(new Type[] { typeof(IServiceManagement<TService>), typeof(ClientCredentials) })
                    .Invoke(new object[] { serviceManagement, authCredentials.ClientCredentials });
        }

        /// <summary>
        /// Returns the discovert service proxy
        /// </summary>
        /// <param name="discoverUrl"></param>
        /// <returns></returns>
        internal DiscoveryServiceProxy GetDiscoveryProxy(string discoverUrl)
        {
            //DiscoveryServiceProxy discoverProxy = null;
            IServiceManagement<IDiscoveryService> discoverManagement = null;
            AuthenticationCredentials authCredentials = null;
            return GetProxy<IDiscoveryService, DiscoveryServiceProxy>(discoverManagement, authCredentials, discoverUrl, true);
        }

        /// <summary>
        /// Returns the Organization Service Proxy
        /// </summary>
        /// <param name="organizationUrl"></param>
        /// <returns></returns>
        internal OrganizationServiceProxy GetOrganizationProxy(string organizationUrl)
        {
            //DiscoveryServiceProxy discoverProxy = null;
            IServiceManagement<IOrganizationService> orgManagement = null;
            AuthenticationCredentials authCredentials = null;
            return GetProxy<IOrganizationService, OrganizationServiceProxy>(orgManagement, authCredentials, organizationUrl, false);
        }

        //Note: Please use your own authentication mechanism. Refer to this link for more types: http://msdn.microsoft.com/en-us/library/gg309393.aspx
        /// <summary>
        /// This function retuns the authentication instance for different type of modes. 
        /// </summary>
        /// <param name="endpointType"></param>
        /// <returns></returns>
        private AuthenticationCredentials GetCredentials(AuthenticationProviderType endpointType)
        {
            AuthenticationCredentials authCredentials = new AuthenticationCredentials();
            switch (endpointType)
            {

                case AuthenticationProviderType.ActiveDirectory:
                    authCredentials.ClientCredentials.Windows.ClientCredential = new System.Net.NetworkCredential(GblUser, GblPassword, GblDomain);
                    break;

                default: // For Federated and OnlineFederated environments.                    
                    authCredentials.ClientCredentials.UserName.UserName = GblUser;
                    authCredentials.ClientCredentials.UserName.Password = GblPassword;
                    // For OnlineFederated single-sign on, you could just use current UserPrincipalName instead of passing user name and password.
                    // authCredentials.UserPrincipalName = UserPrincipal.Current.UserPrincipalName;  //Windows/Kerberos
                    break;

            }
            return authCredentials;
        }

        /// <summary>
        /// Based on the Organization URL specified. The string will return the logged user credentials
        /// </summary>
        /// <param name="organizationUrl"></param>
        /// <returns></returns>
        public string GetLoggedUser(string organizationUrl)
        {
            IOrganizationService serviceProxy = this.GetOrganizationProxy(organizationUrl);
            Guid userid = ((WhoAmIResponse)serviceProxy.Execute(new WhoAmIRequest())).UserId;
            Entity systemUserEntity = serviceProxy.Retrieve("systemuser", userid, new ColumnSet(true));
            return systemUserEntity.GetAttributeValue<String>("fullname").ToString();
        }
    }
}
