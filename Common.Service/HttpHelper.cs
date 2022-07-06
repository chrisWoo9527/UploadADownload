using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;

namespace Common.Service
{
    public class HttpClientHelp
    {
        // Post请求
        public static T? PostResponse<T>(string url, string postData, int iWaitTime = 3000)
            where T : class, new()
        {
            try
            {
                if (url.StartsWith("https"))
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                }

                HttpContent httpContent = new StringContent(postData);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpClient httpClient = new HttpClient();

                HttpResponseMessage response = httpClient.PostAsync(url, httpContent).Result;
                if (response != null && response.IsSuccessStatusCode)
                {
                    Task<string> t = response.Content.ReadAsStringAsync();
                    if (!t.Wait(iWaitTime))
                    {
                        return null;
                    }
                    return JsonConvert.DeserializeObject<T>(t.Result);
                }

                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static bool PostResponse(string url, string postData)
        {
            try
            {
                if (url.StartsWith("https"))
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                }

                HttpContent httpContent = new StringContent(postData);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                //httpContent.Headers.ContentType.CharSet = "UTF-8";
                HttpClient httpClient = new HttpClient();

                httpClient.PostAsync(url, httpContent);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        // Get请求
        public static T? GetResponse<T>(string url, int iWaitTime = 3000)
        //where T : class, new()
        {
            try
            {
                if (url.StartsWith("https"))
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                }

                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead).Result;
                if (response != null && response.IsSuccessStatusCode)
                {
                    Task<string> t = response.Content.ReadAsStringAsync();
                    if (!t.Wait(iWaitTime))
                    {
                        return default;
                    }

                    return JsonConvert.DeserializeObject<T>(t.Result);
                }

                return default; ;
            }
            catch (Exception ex)
            {
                return default;
            }
        }

        public static bool GetResponse(string url)
        {
            try
            {
                if (url.StartsWith("https"))
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                }

                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.GetAsync(url, HttpCompletionOption.ResponseContentRead);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}