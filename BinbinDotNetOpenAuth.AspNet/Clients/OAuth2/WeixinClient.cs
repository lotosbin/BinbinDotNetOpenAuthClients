using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using DotNetOpenAuth.AspNet.Clients;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;

namespace BinbinDotNetOpenAuth.AspNet.Clients
{
    /// <summary>
    ///     A DotNetOpenAuth client for logging in to Google using OAuth2.
    ///     Reference: https://developers.google.com/accounts/docs/OAuth2
    /// </summary>
    public class WeixinClient : OAuth2Client
    {
        #region Constants and Fields

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

        #endregion

        /// <summary>
        ///     Creates a new Google OAuth2 Client, requesting the default "userinfo.profile" and "userinfo.email" scopes.
        /// </summary>
        /// <param name="clientId">The Google Client Id</param>
        /// <param name="clientSecret">The Google Client Secret</param>
        public WeixinClient(string clientId, string clientSecret)
            : this(clientId, clientSecret, new[]
                                           {
                                               "snsapi_base"
                                           })
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

            this._clientId = clientId;
            this._clientSecret = clientSecret;
            this._requestedScopes = requestedScopes;
        }

        protected override Uri GetServiceLoginUrl(Uri returnUrl)
        {
            IEnumerable<string> scopes = this._requestedScopes;
            string state = string.IsNullOrEmpty(returnUrl.Query) ? string.Empty : returnUrl.Query.Substring(1);

            var collection = new NameValueCollection
                             {
                                 {"appid", this._clientId},                             
                                 //{"redirect_uri", returnUrl.GetLeftPart(UriPartial.Path)},
                                 {"redirect_uri", "http://alpha.guangchi.net/account/ExternalLoginCallback" },
                                 { "response_type", "code"},
                                 {"scope", string.Join(" ", scopes)},                            
                                 {"state", state},
                             };
            return UriHelper.BuildUri(AuthorizationEndpoint, collection, "wechat_redirect");
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
                                {"name",openid}
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

        protected override string QueryAccessToken(Uri redirectUrl,string authorizationCode)
        {
            var collection = new NameValueCollection
                             {
                                 {"appid", this._clientId},
                                 {"secret", this._clientSecret},
                                 {"code", authorizationCode},
                                 {"grant_type", "authorization_code"}

                             };
            string response = UriHelper.OAuthPost(TokenEndpoint, collection);
            if (response == null)
            {
                return null;
            }
            JObject json = JObject.Parse(response);
            HttpContext.Current.Session["openid"] = json["openid"].Value<string>();
            return json["access_token"].Value<string>();
        }

        public Uri GetServiceLoginUrlTest(Uri returnUrl)
        {
            return this.GetServiceLoginUrl(returnUrl);
        }

        public string QueryAccessTokenTest(Uri returnUrl, string authorizationCode)
        {
            return this.QueryAccessToken(returnUrl, authorizationCode);
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