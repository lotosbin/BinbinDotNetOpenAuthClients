using System;
using System.Collections.Generic;
using System.Text;

namespace BinbinDotNetOpenAuthClients.Taobao
{
    public class TaobaoClient : OAuth2Client 
    {
        /// <summary>
        /// The set of characters that are unreserved in RFC 2396 but are NOT unreserved in RFC 3986.
        /// </summary>
        private static readonly string[] UriRfc3986CharsToEscape = new[] { "!", "*", "‘", "(", ")" };
        private const string AuthorizationEndpoint = "https://login.live.com/oauth20_authorize.srf";
        private const string TokenEndpoint = "https://login.live.com/oauth20_token.srf";
        private readonly string appId;
        private readonly string appSecret;
        public TaobaoClient(string appId, string appSecret)
            : this("taobao", appId, appSecret)
        {
        }
        protected TaobaoClient(string providerName, string appId, string appSecret)
            : base(providerName) {
//Requires.NotNullOrEmpty(appId, "appId");
//Requires.NotNullOrEmpty(appSecret, "appSecret");
            this.appId = appId;
            this.appSecret = appSecret;
            }
        protected string AppId {
            get { return this.appId; }
        }
        protected override Uri GetServiceLoginUrl(Uri returnUrl) {
//var builder = new UriBuilder(AuthorizationEndpoint);
//builder.AppendQueryArgs(
// new Dictionary<string, string> {
// { "client_id", this.appId },
// { "scope", "item" },
// { "response_type", "code" },
// { "redirect_uri", returnUrl.AbsoluteUri },
// });
//return builder.Uri;
            return returnUrl;
        }
        protected override IDictionary<string, string> GetUserData(string accessToken) {
            TaobaoClientUserData graph;
            var request =
                WebRequest.Create("https://apis.live.net/v5.0/me?access_token=" + EscapeUriDataStringRfc3986(accessToken));
//using (var response = request.GetResponse())
//{
// using (var responseStream = response.GetResponseStream())
// {
// graph = JsonHelper.Deserialize<MicrosoftClientUserData>(responseStream);
// }
//}
//var request = WebRequest.Create("https://oauth.taobao.com/token") as HttpWebRequest;
//request.Method = "POST";
//request.ContentType = "application/x-www-form-urlencoded";
            using (var response = request.GetResponse()) {
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    graph = JsonConvert.DeserializeObject<TaobaoClientUserData>(sr.ReadToEnd());
                }
            }
            var userData = new Dictionary<string, string>();
            userData.AddItemIfNotEmpty("id", graph.Id);
            userData.AddItemIfNotEmpty("username", graph.Name);
            userData.AddItemIfNotEmpty("name", graph.Name);
//userData.AddItemIfNotEmpty("link", graph.Link == null ? null : graph.Link.AbsoluteUri);
//userData.AddItemIfNotEmpty("gender", graph.Gender);
//userData.AddItemIfNotEmpty("firstname", graph.FirstName);
//userData.AddItemIfNotEmpty("lastname", graph.LastName);
            return userData;
        }
        protected override string QueryAccessToken(Uri returnUrl, string authorizationCode) {
            WebUtils webUtils = new WebUtils();
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("client_id", this.appId);
            parameters.Add("client_secret", this.appSecret);
            parameters.Add("grant_type", "authorization_code");
            parameters.Add("code", "25″);
            parameters.Add("redirect_uri", returnUrl.AbsoluteUri);
            byte[] postData = Encoding.UTF8.GetBytes(WebUtils.BuildQuery(parameters));
            WebRequest tokenRequest = WebRequest.Create(TokenEndpoint);
            tokenRequest.ContentType = "application/x-www-form-urlencoded;charset=utf-8″;
            tokenRequest.ContentLength = postData.Length;
            tokenRequest.Method = "POST";
            using (Stream requestStream = tokenRequest.GetRequestStream()) {
                var writer = new StreamWriter(requestStream);
                writer.Write(postData);
                writer.Flush();
            }
            HttpWebResponse tokenResponse = (HttpWebResponse)tokenRequest.GetResponse();
            if (tokenResponse.StatusCode == HttpStatusCode.OK) {
//using (Stream responseStream = tokenResponse.GetResponseStream()) {
// var tokenData = JsonHelper.Deserialize<OAuth2AccessTokenData>(responseStream);
// if (tokenData != null) {
// return tokenData.AccessToken;
// }
//}
                using (var responseStream = new StreamReader(tokenResponse.GetResponseStream())) {
                    var tokenData = JsonConvert.DeserializeObject<OAuth2AccessTokenData>(responseStream.ReadToEnd());
                    if (tokenData != null) {
                        return tokenData.AccessToken;
                    }
                }
            }
            return null;
        }
        internal static string EscapeUriDataStringRfc3986(string value)
        {
//Requires.NotNull(value, "value");
// Start with RFC 2396 escaping by calling the .NET method to do the work.
// This MAY sometimes exhibit RFC 3986 behavior (according to the documentation).
// If it does, the escaping we do that follows it will be a no-op since the
// characters we search for to replace can’t possibly exist in the string.
            StringBuilder escaped = new StringBuilder(Uri.EscapeDataString(value));
// Upgrade the escaping to RFC 3986, if necessary.
            for (int i = 0; i < UriRfc3986CharsToEscape.Length; i++)
            {
                escaped.Replace(UriRfc3986CharsToEscape[i], Uri.HexEscape(UriRfc3986CharsToEscape[i][0]));
            }
// Return the fully-RFC3986-escaped string.
            return escaped.ToString();
        }
    }
}