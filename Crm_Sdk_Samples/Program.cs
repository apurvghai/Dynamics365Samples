using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Tooling.Connector;
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
            //Running the sample
            CrmToolingHelper toolingHelper = new CrmToolingHelper();
            toolingHelper.AuthType = "Office365";
            toolingHelper.GblOrgUrl = "https://ORLGURL.api.crm.dynamics.com/xrmservices/2011/organization.svc";
            toolingHelper.GblUser = "UserID ";
            toolingHelper.GblPassword = "************";


            //Metadaata functions
            RetrieveEntityMetadataWithAttributesIntoFile retrieveEntityMetadataWithAttributesIntoFile = new RetrieveEntityMetadataWithAttributesIntoFile();
            retrieveEntityMetadataWithAttributesIntoFile.DoRetrieveByEntity("account", toolingHelper);
            retrieveEntityMetadataWithAttributesIntoFile.DoRetrieveAllEntities(toolingHelper);

        }
    }
}
