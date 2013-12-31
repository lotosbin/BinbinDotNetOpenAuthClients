using System.Collections.Specialized;
using System.Web;

namespace BinbinDotNetOpenAuth.AspNet.Clients
{
    public static class HttpContextBaseExtension
    {
        public static void RewriteRequestWhenUseState(this HttpContextBase context)
        {
            string stateString = HttpUtility.UrlDecode(context.Request.QueryString["state"]);
            if (stateString != null && stateString.Contains("__provider__="))
            {
                NameValueCollection q = HttpUtility.ParseQueryString(stateString);
                q.Add(context.Request.QueryString);
                q.Remove("state");
                context.RewritePath(context.Request.Path + "?" + q);
            }
        }
    }
}