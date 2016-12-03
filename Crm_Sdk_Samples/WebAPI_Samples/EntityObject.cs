using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crm_Sdk_Samples.WebAPI_Samples
{
    public class EntityType
    {
        public string EntityLogicalName { get; set; }

        public JObject entityAttribute { get; set; }

        public IList<JObject> entityAttributeCollections { get; set; }

    }
}
