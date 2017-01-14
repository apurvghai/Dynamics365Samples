using Microsoft.Xrm.Sdk;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Dyn365Samples
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


            Program app = new Program();
           // Task.WaitAll(Task.Run(async () => await app.CreateMyReordsAsync()));

        }


        async Task CreateMyReordsAsync()
        {
            WebAPIHelper apihelper = new WebAPIHelper();
            apihelper.BaseOrganizationApiUrl = "https://org.api.crm.dynamics.com";
            apihelper.ObtainOAuthToken();

            if (!(string.IsNullOrEmpty(apihelper.AccessToken)))
            {
                //Create Sample Contact
                JObject contact1 = new JObject();
                contact1.Add("firstname", "Peter");
                contact1.Add("lastname", "Cambel");

                string filter = "?$select=firstname&$filter=contains(firstname,'Peter')";

                //await apihelper.CreateEntityRecords("contacts", contact1);
                await apihelper.SearchExistingRecord("contacts", filter);
                Console.Read();
            }
            Console.Read();
        }
    }
}
