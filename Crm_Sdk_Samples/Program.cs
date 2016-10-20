using Microsoft.Xrm.Sdk.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crm_Sdk_Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            CrmConnector crmConnector = new CrmConnector();
            string orgUrl = "https://<Org>.api.crm.dynamics.com/xrmservices/2011/organization.svc";
            crmConnector.GblUser = "UserName ";
            crmConnector.GblPassword = "Password";
            
            //TODO : Under Progress

            //using (OrganizationServiceProxy svcProxy = crmConnector.GetOrganizationProxy(orgUrl))
            //{
            //    RetrieveEntityMetadataWithAttributesIntoFile.DoRetrieveByEntity("account", svcProxy);
            //}
        }
    }
}
