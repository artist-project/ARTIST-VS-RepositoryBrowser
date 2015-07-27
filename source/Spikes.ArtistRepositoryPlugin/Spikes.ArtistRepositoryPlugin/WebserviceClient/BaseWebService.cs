using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Spikes.Spikes_ArtistRepositoryPlugin.WebserviceClient
{
    abstract public class BaseWebService
    {
        protected const string JsonContentType = "application/json";
        protected const string UrlEncodedContentType = "application/x-www-form-urlencoded";
        protected const string BinaryContentType = "application/octet-stream";

        protected async Task<T> DoWebRequest<T>(string url, string authorization)
            where T : class
        {
            return await _DoWebRequest<T>(url, "GET", authorization, false);
        }

        protected async Task<string> DoWebRequestRaw(string url, string authorization)
        {
            return await _DoWebRequest<string>(url, "GET", authorization, true);
        }

        protected async Task<T> DoDeleteWebRequest<T>(string url, string authorization)
            where T : class
        {
            return await _DoWebRequest<T>(url, "DELETE", authorization, false);
        }

        private async Task<T> _DoWebRequest<T>(string url, string webMethod, string authorization, bool raw) 
            where T: class
        {
            var request = WebRequest.Create(url);
            request.Method = webMethod;

            if (!string.IsNullOrEmpty(authorization))
            {
                request.Headers.Add("Authorization", authorization);
            }

            using (var response = await request.GetResponseAsync())
            {
                try
                {
                    var streamReader = new StreamReader(response.GetResponseStream());
                    var data = streamReader.ReadToEnd();

                    if (raw)
                    {
                        return data as T;
                    }

                    return JsonConvert.DeserializeObject<T>(data, new JsonSerializerSettings() {NullValueHandling = NullValueHandling.Ignore});
                }
                finally
                {
                    response.Close();
                }
            }
        }

        protected async Task<T> DoPostWebRequest<T>(string url, dynamic postData, string authorization)
            where T : class
        {
            return await _DoPostWebRequest<T>(url, JsonConvert.SerializeObject(postData), authorization, JsonContentType, "POST");
        }

        protected async Task<T> DoPutWebRequest<T>(string url, dynamic postData, string authorization)
            where T : class
        {
            return await _DoPostWebRequest<T>(url, JsonConvert.SerializeObject(postData), authorization, JsonContentType, "PUT");
        }

        protected async Task<T> DoPostWebRequest<T>(string url, string postData, string authorization, string contentType)
            where T : class
        {
            return await _DoPostWebRequest<T>(url, postData, authorization, contentType, "POST");
        }

        private  async Task<T> _DoPostWebRequest<T>(string url, string postData, string authorization, string contentType, string method)
            where T : class
        {
            if (postData == null)
                postData = "{}";

            var request = WebRequest.Create(url);
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.Method = method;
            request.ContentType = contentType;
            request.ContentLength = byteArray.Length;

            if (!string.IsNullOrEmpty(authorization))
            {
                request.Headers.Add("Authorization", authorization);
            }

            using (var dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
            }

            using (var response = await request.GetResponseAsync())
            {
                try
                {
                    var streamReader = new StreamReader(response.GetResponseStream());
                    var data = streamReader.ReadToEnd();

                    return JsonConvert.DeserializeObject<T>(data, new JsonSerializerSettings() {NullValueHandling = NullValueHandling.Ignore});
                }
                finally
                {
                    response.Close();
                }
            }
        }
    }
}
