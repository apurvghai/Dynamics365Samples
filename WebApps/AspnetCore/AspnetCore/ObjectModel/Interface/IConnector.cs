using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspnetCore.ObjectModel.Interface
{
    interface IConnector
    {
        /// <summary>
        /// Connect to Dynamics CRM Organization
        /// </summary>
        /// <param name="resourceUrl"></param>
        void Authenticate(string resourceUrl);
        /// <summary>
        /// Connect to Discovery Service
        /// </summary>
        void DiscoverOrgs();
        
        void RetrieveEntity(string entityName);
    }
}
