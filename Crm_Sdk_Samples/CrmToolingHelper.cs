using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crm_Sdk_Samples
{
    internal class CrmToolingHelper
    {
        public string GblUser { get; set; }
        public string GblPassword { get; set; }
        public string GblDomain { get; set; }
        public string GblOrgUrl { get; set; }
        public string AuthType { get; set; }

        /// <summary>
        /// This function accepts the global variables and returns the tooling service : Currently Supports only CRM Online. 
        /// </summary>
        /// <returns></returns>
        internal CrmServiceClient GetConnectToCrm()
        {
            CrmServiceClient crmSvcClient = new CrmServiceClient("AuthType=" + AuthType +
                ";Username=" + GblUser + "; Password=" + GblPassword + ";Url=" + GblOrgUrl + "");
            return crmSvcClient;
        }
    }
}
