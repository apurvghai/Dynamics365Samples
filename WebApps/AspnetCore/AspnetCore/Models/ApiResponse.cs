using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspnetCore.Models
{
    public class ApiResponse<T> : HttpResponseMessage
    {
        public T DeserializedContent { get; set; }

        public bool HasFailed { get; set; }
    }
}
