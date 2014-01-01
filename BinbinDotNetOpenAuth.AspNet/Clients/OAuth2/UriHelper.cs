using System;
using System.Collections.Specialized;
using System.Web;

namespace BinbinDotNetOpenAuth.AspNet.Clients
{
    internal static class UriHelper
    {
        public static Uri BuildUri(string baseUri, NameValueCollection queryParameters)
        {
            NameValueCollection q = HttpUtility.ParseQueryString(String.Empty);
            q.Add(queryParameters);
            var builder = new UriBuilder(baseUri) {Query = q.ToString()};
            return builder.Uri;
        }
    }
}