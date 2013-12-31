using System;
using System.Text.RegularExpressions;

namespace BinbinDotNetOpenAuthClients.Taobao
{
    internal static class UriHelper
    {
        /// <summary>
        ///     The attach query string parameter.
        /// </summary>
        /// <param name="url">
        ///     The url.
        /// </param>
        /// <param name="parameterName">
        ///     The parameter name. This value should not be provided by an end user; the caller should
        ///     ensure that this value comes only from a literal string.
        /// </param>
        /// <param name="parameterValue">
        ///     The parameter value.
        /// </param>
        /// <returns>An absolute URI.</returns>
        public static Uri AttachQueryStringParameter(this Uri url, string parameterName, string parameterValue)
        {
            var builder = new UriBuilder(url);
            string query = builder.Query;
            if (query.Length > 1)
            {
// remove the ‘?’ character in front of the query string
                query = query.Substring(1);
            }
            string parameterPrefix = parameterName + "=";
            string encodedParameterValue = Uri.EscapeDataString(parameterValue);
            string newQuery = Regex.Replace(query, parameterPrefix + @"[^\&]*", parameterPrefix + encodedParameterValue);
            if (newQuery == query)
            {
                if (newQuery.Length > 0)
                {
                    newQuery += "&";
                }
                newQuery = newQuery + parameterPrefix + encodedParameterValue;
            }
            builder.Query = newQuery;
            return builder.Uri;
        }

//internal static void AppendQueryArgs(this UriBuilder builder, IEnumerable<KeyValuePair<string, string>> args)
//{
// Requires.NotNull(builder, "builder");
// if (args != null && args.Count() > 0)
// {
// StringBuilder sb = new StringBuilder(50 + (args.Count() * 10));
// if (!string.IsNullOrEmpty(builder.Query))
// {
// sb.Append(builder.Query.Substring(1));
// sb.Append(‘&’);
// }
// sb.Append(CreateQueryString(args));
// builder.Query = sb.ToString();
// }
//}
        /// <summary>
        /// Converts an app-relative url, e.g. ~/Content/Return.cshtml, to a full-blown url, e.g. http://mysite.com/Content/Return.cshtml
        /// </summary>
        /// <param name="returnUrl">
        /// The return URL. 
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>An absolute URI.</returns>
//public static Uri ConvertToAbsoluteUri(string returnUrl, HttpContextBase context)
//{
// if (Uri.IsWellFormedUriString(returnUrl, UriKind.Absolute))
// {
// return new Uri(returnUrl, UriKind.Absolute);
// }
// if (!VirtualPathUtility.IsAbsolute(returnUrl))
// {
// returnUrl = VirtualPathUtility.ToAbsolute(returnUrl);
// }
// Uri publicUrl = context.Request..GetPublicFacingUrl();
// return new Uri(publicUrl, returnUrl);
//}
    }
}