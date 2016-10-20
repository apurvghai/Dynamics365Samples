using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crm_Sdk_Samples
{
    internal static class RetrieveEntityMetadataWithAttributesIntoFile
    {
        //TO Under work : Does not work yet
        public static void DoRetrieveByEntity(string entitySchemaName, OrganizationServiceProxy svcProxy)
        {
            RetrieveEntityRequest retrieveEntityRequest = new RetrieveEntityRequest()
            {
                LogicalName = entitySchemaName,
                RetrieveAsIfPublished = true
            };

            RetrieveEntityResponse retrieveEntityResponse = (RetrieveEntityResponse)svcProxy.Execute(retrieveEntityRequest);
            string logicalName = retrieveEntityResponse.EntityMetadata.LogicalName;
        }
    }
}
