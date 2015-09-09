using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Web;
using DotNetOpenAuth.AspNet;
using DotNetOpenAuth.AspNet.Clients;
using log4net;
using Newtonsoft.Json.Linq;

namespace BinbinDotNetOpenAuth.AspNet.Clients
{
    /// <summary>
    ///     <see cref="http://mp.weixin.qq.com/wiki/17/c0f37d5704f0b64713d5d2c37b468d75.html" />
    /// </summary>
    public class WeixinClient : OAuth2Client
    {
        /// <summary>
        ///     The authorization endpoint.
        /// </summary>
        private const string AuthorizationEndpoint = "https://open.weixin.qq.com/connect/oauth2/authorize";

        /// <summary>
        ///     The token endpoint.
        /// </summary>
        private const string TokenEndpoint = "https://api.weixin.qq.com/sns/oauth2/access_token";

        /// <summary>
        ///     The user info endpoint.
        /// </summary>
        private const string UserInfoEndpoint = "https://api.weixin.qq.com/sns/userinfo";

        private static readonly ILog log = LogManager.GetLogger(typeof (WeixinClient));

        /// <summary>
        ///     The _app id.
        /// </summary>
        protected readonly string _clientId;

        /// <summary>
        ///     The _app secret.
        /// </summary>
        protected readonly string _clientSecret;

        /// <summary>
        ///     The requested scopes.
        /// </summary>
        private readonly string[] _requestedScopes;

        /// <summary>
        ///     Creates a new Google OAuth2 Client, requesting the default "userinfo.profile" and "userinfo.email" scopes.
        /// </summary>
        /// <param name="clientId">The Google Client Id</param>
        /// <param name="clientSecret">The Google Client Secret</param>
        public WeixinClient(string clientId, string clientSecret)
            : this(clientId, clientSecret, "snsapi_base")
        {
        }

        /// <summary>
        ///     Creates a new Google OAuth2 client.
        /// </summary>
        /// <param name="clientId">The Google Client Id</param>
        /// <param name="clientSecret">The Google Client Secret</param>
        /// <param name="requestedScopes">One or more requested scopes, passed without the base URI.</param>
        public WeixinClient(string clientId, string clientSecret, params string[] requestedScopes)
            : base("weixin")
        {
            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new ArgumentNullException("clientId");
            }

            if (string.IsNullOrWhiteSpace(clientSecret))
            {
                throw new ArgumentNullException("clientSecret");
            }

            if (requestedScopes == null)
            {
                throw new ArgumentNullException("requestedScopes");
            }

            if (requestedScopes.Length == 0)
            {
                throw new ArgumentException("One or more scopes must be requested.", "requestedScopes");
            }

            _clientId = clientId;
            _clientSecret = clientSecret;
            _requestedScopes = requestedScopes;
        }

        protected override Uri GetServiceLoginUrl(Uri returnUrl)
        {
            IEnumerable<string> scopes = _requestedScopes;
            var state = string.IsNullOrEmpty(returnUrl.Query) ? string.Empty : returnUrl.Query.Substring(1);

            var collection = new NameValueCollection
                             {
                                 {"appid", _clientId},
                                 {"redirect_uri", returnUrl.GetLeftPart(UriPartial.Path)},
                                 {"response_type", "code"},
                                 {"scope", string.Join(" ", scopes)},
                                 {"state", state}
                             };
            var uri = UriHelper.BuildUri(AuthorizationEndpoint, collection, "wechat_redirect");
            log.Debug("GetServiceLoginUrl:" + uri);
            return uri;
        }

        protected override IDictionary<string, string> GetUserData(string accessToken)
        {
            var openid = (string) HttpContext.Current.Session["openid"];
            var collection = new NameValueCollection
                             {
                                 {"access_token", accessToken},
                                 {"openid", openid}
                             };
            //string json = UriHelper.OAuthGet(UserInfoEndpoint, collection);
            //var user = JsonConvert.DeserializeObject<WeiXinUserData>(json);
            var extraData = new Dictionary<string, string>
                            {
                                {"id", openid},
                                {"name", openid}
                                //{"nickname", user.nickname},
                                //{"sex", user.sex.ToString()},
                                //{"city", user.city},
                                //{"country", user.country},
                                //{"headimgurl", user.headimgurl},
                                //{"privilege", user.privilege.ToString()},
                                //{"unionid", user.unionid},
                            };
            return extraData;
        }

        protected override string QueryAccessToken(Uri redirectUrl, string authorizationCode)
        {
            log.Debug("QueryAccessToken:start");
            var collection = new NameValueCollection
                             {
                                 {"appid", _clientId},
                                 {"secret", _clientSecret},
                                 {"code", authorizationCode},
                                 {"grant_type", "authorization_code"}
                             };
            var response = UriHelper.OAuthGet(TokenEndpoint, collection);
            log.Debug("QueryAccessToken:" + response);
            if (response == null)
            {
                return null;
            }
            var json = JObject.Parse(response);
            if (!string.IsNullOrEmpty((string) json["errcode"]))
            {
                throw new Exception(response);
            }
            HttpContext.Current.Session["openid"] = json["openid"].Value<string>();
            return json["access_token"].Value<string>();
        }

        public override AuthenticationResult VerifyAuthentication(HttpContextBase context, Uri returnPageUrl)
        {
            log.Debug("VerifyAuthentication start");
            return base.VerifyAuthentication(context, returnPageUrl);
        }

        public Uri GetServiceLoginUrlTest(Uri returnUrl)
        {
            return GetServiceLoginUrl(returnUrl);
        }

        public string QueryAccessTokenTest(Uri returnUrl, string authorizationCode)
        {
            return QueryAccessToken(returnUrl, authorizationCode);
        }

        [DataContract]
        [Serializable]
        public class WeiXinUserData
        {
            [DataMember]
            public string openid { get; set; }

            [DataMember]
            public string nickname { get; set; }

            [DataMember]
            public int sex { get; set; }

            [DataMember]
            public string city { get; set; }

            [DataMember]
            public string country { get; set; }

            [DataMember]
            public string headimgurl { get; set; }

            [DataMember]
            public List<string> privilege { get; set; }

            [DataMember]
            public string unionid { get; set; }
        }
    }
}