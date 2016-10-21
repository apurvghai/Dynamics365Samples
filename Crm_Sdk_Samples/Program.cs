using Microsoft.Xrm.Sdk;

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
