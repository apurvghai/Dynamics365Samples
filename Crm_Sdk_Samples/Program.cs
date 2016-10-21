using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
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
            IOrganizationService orgServcs = toolingHelper.GetConnectToCrm(CrmAuthType.Office365);
            
            //Metadaata functions
            CrmMetadataHelper crmMetadataHelper = new CrmMetadataHelper();
            crmMetadataHelper.DoRetrieveAllEntities(orgServcs);

        }
    }
}
