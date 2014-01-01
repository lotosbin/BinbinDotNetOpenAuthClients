using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Web;
using DotNetOpenAuth.AspNet.Clients;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BinbinDotNetOpenAuth.AspNet.Clients
{
    /// <summary>
    ///     A DotNetOpenAuth client for logging in to Google using OAuth2.
    ///     Reference: https://developers.google.com/accounts/docs/OAuth2
    /// </summary>
    public class QQClient : OAuth2Client
    {
        #region Constants and Fields

        /// <summary>
        ///     The authorization endpoint.
        /// </summary>
        private const string AuthorizationEndpoint = "https://graph.qq.com/oauth2.0/authorize";

        /// <summary>
        ///     The token endpoint.
        /// </summary>
        private const string TokenEndpoint = "https://graph.qq.com/oauth2.0/token";

        /// <summary>
        ///     The user info endpoint.
        /// </summary>
        private const string UserInfoEndpoint = "https://graph.qq.com/user/get_user_info";

        /// <summary>
        ///     The _app id.
        /// </summary>
        private readonly string _clientId;

        /// <summary>
        ///     The _app secret.
        /// </summary>
        private readonly string _clientSecret;

        /// <summary>
        ///     The requested scopes.
        /// </summary>
        private readonly string[] _requestedScopes;

        #endregion

        /// <summary>
        ///     Creates a new Google OAuth2 Client, requesting the default "userinfo.profile" and "userinfo.email" scopes.
        /// </summary>
        /// <param name="clientId">The Google Client Id</param>
        /// <param name="clientSecret">The Google Client Secret</param>
        public QQClient(string clientId, string clientSecret) : this(clientId, clientSecret, new[]
        {
            "get_user_info"
        })
        {
        }

        /// <summary>
        ///     Creates a new Google OAuth2 client.
        /// </summary>
        /// <param name="clientId">The Google Client Id</param>
        /// <param name="clientSecret">The Google Client Secret</param>
        /// <param name="requestedScopes">One or more requested scopes, passed without the base URI.</param>
        public QQClient(string clientId, string clientSecret, params string[] requestedScopes) : base("qq")
        {
            if (string.IsNullOrWhiteSpace(clientId))
                throw new ArgumentNullException("clientId");

            if (string.IsNullOrWhiteSpace(clientSecret))
                throw new ArgumentNullException("clientSecret");

            if (requestedScopes == null)
                throw new ArgumentNullException("requestedScopes");

            if (requestedScopes.Length == 0)
                throw new ArgumentException("One or more scopes must be requested.", "requestedScopes");

            _clientId = clientId;
            _clientSecret = clientSecret;
            _requestedScopes = requestedScopes;
        }

        protected override Uri GetServiceLoginUrl(Uri returnUrl)
        {
            IEnumerable<string> scopes = _requestedScopes;
            string state = string.IsNullOrEmpty(returnUrl.Query) ? string.Empty : returnUrl.Query.Substring(1);

            return UriHelper.BuildUri(AuthorizationEndpoint, new NameValueCollection
            {
                {"response_type", "code"},
                {"client_id", _clientId},
                {"scope", string.Join(" ", scopes)},
                {"redirect_uri", returnUrl.GetLeftPart(UriPartial.Path)},
                {"state", state},
            });
        }

        protected override IDictionary<string, string> GetUserData(string accessToken)
        {
            var uid = (string) HttpContext.Current.Session["uid"];
            Uri uri = UriHelper.BuildUri(UserInfoEndpoint, new NameValueCollection
            {
                {"access_token", accessToken},
                {"oauth_consumer_key", _clientId},
                {"openid", uid}
            });

            var webRequest = (HttpWebRequest) WebRequest.Create(uri);

            using (WebResponse webResponse = webRequest.GetResponse())
            using (Stream stream = webResponse.GetResponseStream())
            {
                if (stream == null)
                    return null;

                using (var textReader = new StreamReader(stream))
                {
                    string json = textReader.ReadToEnd();
                    var user = JsonConvert.DeserializeObject<QQUserData>(json);
                    var extraData = new Dictionary<string, string>
                    {
                        {"id", uid},
                        {"name", user.nickname},
                    };
                    return extraData;
                }
            }
        }

        protected override string QueryAccessToken(Uri returnUrl, string authorizationCode)
        {
            NameValueCollection postData = HttpUtility.ParseQueryString(string.Empty);
            postData.Add(new NameValueCollection
            {
                {"grant_type", "authorization_code"},
                {"code", authorizationCode},
                {"client_id", _clientId},
                {"client_secret", _clientSecret},
                {"redirect_uri", returnUrl.GetLeftPart(UriPartial.Path)},
            });

            var webRequest = (HttpWebRequest) WebRequest.Create(TokenEndpoint);

            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";

            using (Stream s = webRequest.GetRequestStream())
            using (var sw = new StreamWriter(s))
                sw.Write(postData.ToString());

            string accessToken;
            using (WebResponse webResponse = webRequest.GetResponse())
            {
                Stream responseStream = webResponse.GetResponseStream();
                if (responseStream == null)
                    return null;

                using (var reader = new StreamReader(responseStream))
                {
                    string response = reader.ReadToEnd();
                    JObject json = JObject.Parse(response);
                    //var uid = json.Value<string>("uid");
                    //HttpContext.Current.Session["uid"] = uid;
                    accessToken = json.Value<string>("access_token");
                }
            }
            {
                HttpContext.Current.Session["uid"] = GetOpenId(accessToken);
            }
            return accessToken;
        }

        private object GetOpenId(string accessToken)
        {
            NameValueCollection postData = HttpUtility.ParseQueryString(string.Empty);
            postData.Add(new NameValueCollection
            {
                {"access_token", accessToken},
            });

            var webRequest = (HttpWebRequest) WebRequest.Create("https://graph.qq.com/oauth2.0/me");

            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";

            using (Stream s = webRequest.GetRequestStream())
            using (var sw = new StreamWriter(s))
                sw.Write(postData.ToString());

            string openid = null;
            using (WebResponse webResponse = webRequest.GetResponse())
            {
                Stream responseStream = webResponse.GetResponseStream();
                if (responseStream == null)
                    return null;

                using (var reader = new StreamReader(responseStream))
                {
                    string response = reader.ReadToEnd(); //callback( {"client_id":"YOUR_APPID","openid":"YOUR_OPENID"} ); 
                    if (response.StartsWith("callback("))
                    {
                        string jsontext = response.Substring(9, response.Length - (9 + 2)); // {"client_id":"YOUR_APPID","openid":"YOUR_OPENID"} 
                        JObject json = JObject.Parse(jsontext);
                        openid = json.Value<string>("openid");
                    }
                }
            }

            return openid;
        }
    }
}