using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Web;

namespace BinbinDotNetOpenAuth.AspNet.Clients
{
    internal static class UriHelper
    {
        public static Uri BuildUri(string baseUri, NameValueCollection queryParameters)
        {
            NameValueCollection q = HttpUtility.ParseQueryString(String.Empty);
            q.Add(queryParameters);
            var builder = new UriBuilder(baseUri)
                          {
                              Query = q.ToString()
                          };
            return builder.Uri;
        }

        internal static string OAuthPost(string endpoint, NameValueCollection collection)
        {
            NameValueCollection postData = HttpUtility.ParseQueryString(String.Empty);
            postData.Add(collection);

            var webRequest = (HttpWebRequest) WebRequest.Create(endpoint);

            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";

            using (Stream s = webRequest.GetRequestStream())
            {
                using (var sw = new StreamWriter(s))
                {
                    sw.Write(postData.ToString());
                }
            }

            string response;
            using (WebResponse webResponse = webRequest.GetResponse())
            {
                Stream responseStream = webResponse.GetResponseStream();
                if (responseStream == null)
                {
                    return null;
                }

                using (var reader = new StreamReader(responseStream))
                {
                    response = reader.ReadToEnd();
                }
            }
            return response;
        }

        internal static string OAuthGet(string endpoint, NameValueCollection valueCollection)
        {
            Uri uri = BuildUri(endpoint, valueCollection);

            var webRequest = (HttpWebRequest) WebRequest.Create(uri);

            string json;
            using (WebResponse webResponse = webRequest.GetResponse())
            {
                using (Stream stream = webResponse.GetResponseStream())
                {
                    if (stream == null)
                    {
                        return null;
                    }

                    using (var textReader = new StreamReader(stream))
                    {
                        json = textReader.ReadToEnd();
                    }
                }
            }
            return json;
        }

        internal static string OAuthGetWithHeader(string endpoint, NameValueCollection valueCollection, string accessToken)
        {
            Uri uri = BuildUri(endpoint, valueCollection);

            var webRequest = (HttpWebRequest) WebRequest.Create(uri);
            webRequest.Headers.Add("Authorization", "Bearer " + accessToken);
            string json;
            using (WebResponse webResponse = webRequest.GetResponse())
            {
                using (Stream stream = webResponse.GetResponseStream())
                {
                    if (stream == null)
                    {
                        return null;
                    }

                    using (var textReader = new StreamReader(stream))
                    {
                        json = textReader.ReadToEnd();
                    }
                }
            }
            return json;
        }
    }
}