using AspnetCore.ObjectModel.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspnetCore.ObjectModel.Class
{
    public class Connector : IConnector
    {
        
        public void Authenticate(string resourceUrl)
        {
            throw new NotImplementedException();
        }

        public void DiscoverOrgs()
        {
            throw new NotImplementedException();
        }

        public void RetrieveEntity(string entityName)
        {
            throw new NotImplementedException();
        }
    }
}
