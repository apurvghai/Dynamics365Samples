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
            CrmConnection crmConnect = new CrmConnection();
            string orgUrl = "https://apurvgh003.api.crm.dynamics.com/xrmservices/2011/organization.svc";
            CheckOrphanSolutionComponents.DoRetrieve(crmConnect.GetCrmConnection(orgUrl, "admin@apurvgh003.onmicrosoft.com", "Password1@@", null, false));
        }
    }
}
