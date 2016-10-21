/*
        Author : Apurv Ghai
        Description: The program has been distributed as in to help the community members and do not certify to be use for Production Use.
 */

using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Configuration;

namespace Crm_Sdk_Samples
{
    /// <summary>
    /// These enums provide easy access to determine the CRM Type of Authentication you want your app to perform.
    /// </summary>
    enum CrmAuthType
    {
        ADWithUserName = 1,
        IntegratedAD = 2,
        Office365 = 3,
        IFD = 4,
        OAuth = 5,
        OAuthWithUserInterface = 6,
    }
    internal class CrmToolingHelper
    {
        private IOrganizationService crmOrgService;

        /// <summary>
        /// Returns the Crm Service Connecting Object. 
        /// </summary>
        /// <param name="crmAuthType">Provide the authentication type, it will pick up the connecting string from app.config</param>
        /// <returns></returns>
        internal IOrganizationService GetConnectToCrm(CrmAuthType crmAuthType)
        {
            string connectingString = string.Empty;

            switch (crmAuthType)
            {
                case CrmAuthType.ADWithUserName:
                    connectingString = ConfigurationManager.AppSettings["XrmToolingConnectionADNamed"].ToString();
                    break;

                case CrmAuthType.IntegratedAD:
                    connectingString = ConfigurationManager.AppSettings["XrmToolingConnectionADIntegrated"].ToString();
                    break;

                case CrmAuthType.Office365:
                    connectingString = ConfigurationManager.AppSettings["XrmToolingConnectionOffice365"].ToString();
                    break;

                case CrmAuthType.IFD:
                    connectingString = ConfigurationManager.AppSettings["XrmToolingConnectionIFD"].ToString();
                    break;

                case CrmAuthType.OAuth:
                    connectingString = ConfigurationManager.AppSettings["XrmToolingConnectionOAuth"].ToString();
                    break;

                case CrmAuthType.OAuthWithUserInterface:
                    connectingString = ConfigurationManager.AppSettings["XrmToolingConnectionOAuthUX"].ToString();
                    break;
            }

            CrmServiceClient crmSvcClient = new CrmServiceClient(connectingString);
            crmOrgService = crmSvcClient.OrganizationWebProxyClient != null ?
                (IOrganizationService)crmSvcClient.OrganizationWebProxyClient : crmSvcClient.OrganizationServiceProxy;

            return crmOrgService;
        }


        /// <summary>
        /// This function will provide you the user full name whose is currently logged in Using CrmServiceClient
        /// </summary>
        /// <param name="crmAuthType"></param>
        /// <returns></returns>
        internal string GetLoggedOnUser(CrmAuthType crmAuthType)
        {
            IOrganizationService serviceProxy = GetConnectToCrm(crmAuthType);
            Guid userid = ((WhoAmIResponse)serviceProxy.Execute(new WhoAmIRequest())).UserId;
            Entity systemUserEntity = serviceProxy.Retrieve("systemuser", userid, new ColumnSet(true));
            return systemUserEntity.GetAttributeValue<string>("fullname").ToString();
        }

    }
}
