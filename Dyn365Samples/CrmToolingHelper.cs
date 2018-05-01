/*
        Author : Apurv Ghai
        Description: The program has been distributed *as is* to help the community members and do not certify to be used for Production Use.
 */

using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Configuration;

namespace Dyn365Samples
{
    /*
         CrmServiceClient has logging option, please review App.config for more options.  
    */
    internal class CrmToolingHelper
    {
        /// <summary>
        /// Returns the Crm Service Client. 
        /// </summary>
        /// <returns>returns CrmServiceClient</returns>
        internal CrmServiceClient GetServiceClient()
        {
            //Retrieve the connection string from App.Config
            string connectingString = ConfigurationManager.ConnectionStrings["CrmConnectiongString"].ConnectionString;
            CrmServiceClient serviceClient = new CrmServiceClient(connectingString);
            return serviceClient;
        }


        /// <summary>
        /// Sample function to perform WhoAmI and retrieve full name of the User
        /// </summary>
        /// <returns></returns>
        internal string GetLoggedOnUser()
        {
            using (var client = GetServiceClient())
            {
                Guid userid = ((WhoAmIResponse)client.Execute(new WhoAmIRequest())).UserId;
                Entity systemUserEntity = client.Retrieve("systemuser", userid, new ColumnSet(true));
                return systemUserEntity.GetAttributeValue<string>("fullname").ToString();
            }
        }
    }
}
