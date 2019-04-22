using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AspnetCore.Models;
using Newtonsoft.Json;

namespace AspnetCore.ObjectModel.Class
{
    public static class RequestProcessor
    {
        public static async Task<ApiResponse<T>> TryParseResponse<T>(this HttpRequestMessage request, HttpClient client, string loggedInUser)
        {
            try
            {
                var response = await client.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();
                var deserialzedContent = JsonConvert.DeserializeObject<T>(content);
                response.Headers.Add("X-ActivityId", Guid.NewGuid().ToString());
                response.Headers.Add("X-User", loggedInUser);
                return GetApiFromHttpResponse<T>(response, deserialzedContent);
            }
            catch (Exception e)
            {
                var badResponse = request.CreateResponse(HttpStatusCode.InternalServerError);
                badResponse.Content = GetFormattedContent(e.StackTrace);
                return GetApiFromHttpResponse<T>(badResponse, default);
            }
        }

        private static ApiResponse<T> GetApiFromHttpResponse<T>(HttpResponseMessage response, T deserialzedContent)
        {
            var apiResponse = new ApiResponse<T>();
            apiResponse.Content = response.Content;
            var headers = response.Headers;
            foreach (var header in headers)
            {
                var values = header.Value;
                foreach (var value in values)
                {
                    if (value.Count() > 0)
                    {
                        apiResponse.Headers.Add(header.Key, values);
                        break;
                    }
                }
            }
            apiResponse.DeserializedContent = deserialzedContent;
            apiResponse.HasFailed = response.IsSuccessStatusCode == true ? false : true;
            apiResponse.ReasonPhrase = response.ReasonPhrase;
            apiResponse.StatusCode = response.StatusCode;
            apiResponse.Version = response.Version;
            return apiResponse;
        }

        private static HttpContent GetFormattedContent(string content)
        {
            return new StringContent(content, Encoding.UTF8, "application/json");
        }
    }
}
