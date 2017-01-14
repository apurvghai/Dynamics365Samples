/*
        Author : Apurv Ghai
        Description: The program has been distributed *as is* to help the community members and do not certify to be used for Production Use.
 */

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Dyn365Samples
{
    /// <summary>
    /// This class will demonstrate the examples for using Retrieve Multiple using CRM SDK
    /// </summary>
    internal class CrmRetrieveMultiple
    {
        /// <summary>
        /// This function will return the collecting of Entity records.
        /// </summary>
        /// <param name="svcsClient">Service Object</param>
        /// <param name="countOfRecords">Will return the count of records</param>
        /// <returns></returns>
        public EntityCollection DoRetrieveMultile(IOrganizationService svcsClient, out int countOfRecords)
        {
            string fetchxml = "<fetch distinct='false' mapping='logical' output-format='xml-platform' version='1.0'>" +
                   "<entity name='account'>" +
                     "<attribute name='name'/>" +
                      "<attribute name='accountnumber'/>" +
                     "<order descending='false' attribute='name'/>" +
                     "<filter type='and'>" +
                      " <condition attribute='name' value='Sample Account Name' operator='eq'/>" +
                      "<condition attribute='name' operator='not-null'/>" +
                      "</filter>" +
                   "</entity>" +
                 "</fetch>";

            EntityCollection entityCollection = svcsClient.RetrieveMultiple(new FetchExpression(fetchxml));
            countOfRecords = entityCollection.Entities.Count;
            return entityCollection;
        }
    }
}
