using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using DotNetOpenAuth.AspNet.Clients;
using log4net;
using Newtonsoft.Json;

namespace BinbinDotNetOpenAuth.AspNet.Clients
{
    /// <summary>
    ///     A DotNetOpenAuth client for logging in to Google using OAuth2.
    ///     Reference: https://developers.google.com/accounts/docs/OAuth2
    /// </summary>
    public class SohuWeiboClient : OAuth2Client
    {
        private static readonly ILog log = LogManager.GetLogger(typeof (SohuWeiboClient));

        #region Constants and Fields

        /// <summary>
        ///     The authorization endpoint.
        /// </summary>
        private const string AuthorizationEndpoint = "https://api.t.sohu.com/oauth2/authorize";

        /// <summary>
        ///     The token endpoint.
        /// </summary>
        private const string TokenEndpoint = "https://api.t.sohu.com/oauth2/access_token";

        /// <summary>
        ///     The user info endpoint.
        /// </summary>
        private const string UserInfoEndpoint = "https://api.t.sohu.com/users/show.json";

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
        public SohuWeiboClient(string clientId, string clientSecret)
            : this(clientId, clientSecret, new[]
                                           {
                                               "basic"
                                           })
        {
        }

        /// <summary>
        ///     Creates a new Google OAuth2 client.
        /// </summary>
        /// <param name="clientId">The Google Client Id</param>
        /// <param name="clientSecret">The Google Client Secret</param>
        /// <param name="requestedScopes">One or more requested scopes, passed without the base URI.</param>
        public SohuWeiboClient(string clientId, string clientSecret, params string[] requestedScopes)
            : base("sohuweibo")
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

            //if (requestedScopes.Length == 0)
            //{
            //    throw new ArgumentException("One or more scopes must be requested.", "requestedScopes");
            //}

            this._clientId = clientId;
            this._clientSecret = clientSecret;
            this._requestedScopes = requestedScopes;
        }

        protected override Uri GetServiceLoginUrl(Uri returnUrl)
        {
            log.Info("GetServiceLoginUrl");
            IEnumerable<string> scopes = this._requestedScopes;
            string state = string.IsNullOrEmpty(returnUrl.Query) ? string.Empty : returnUrl.Query.Substring(1);

            var collection = new NameValueCollection
                             {
                                 {"response_type", "code"},
                                 {"client_id", this._clientId},
                                 {"scope", string.Join(" ", scopes)},
                                 {"redirect_uri", returnUrl.GetLeftPart(UriPartial.Path)},
                                 {"state", state},
                             };
            return UriHelper.BuildUri(AuthorizationEndpoint, collection);
        }

        protected override IDictionary<string, string> GetUserData(string accessToken)
        {
            log.Info("GetUserData");
            //var uid = (string) HttpContext.Current.Session["uid"];
            var collection = new NameValueCollection
                             {
                                 {"access_token", accessToken},
                             };
            string json = UriHelper.OAuthGet(UserInfoEndpoint, collection);
            log.Info("response:" + json);
            var result = JsonConvert.DeserializeObject<GetUserDataResult>(json);
            var extraData = new Dictionary<string, string>
                            {
                                {"id", result.id},
                                {"name", result.screen_name},
                            };
            return extraData;
        }

        protected override string QueryAccessToken(Uri returnUrl, string authorizationCode)
        {
            log.Info("QueryAccessToken(authcode:" + authorizationCode + ")");
            var valueCollection = new NameValueCollection
                                  {
                                      {"grant_type", "authorization_code"},
                                      {"code", authorizationCode},
                                      {"client_id", this._clientId},
                                      {"client_secret", this._clientSecret},
                                      {"redirect_uri", returnUrl.GetLeftPart(UriPartial.Path)},
                                  };
            string json = UriHelper.OAuthPost(TokenEndpoint, valueCollection);
            log.Info("response:" + json);
            if (json == null)
            {
                return null;
            }
            var data = JsonConvert.DeserializeObject<QueryAccessTokenResponseData>(json);
            string accessToken = data.access_token;

            //HttpContext.Current.Session["uid"] = data.open_id;
            return accessToken;
        }

        [DataContract]
        [Serializable]
        public class GetUserDataResult
        {
            /// <summary>
            /// </summary>
            [DataMember]
            public string id { get; set; }

            /// <summary>
            ///     状态描述
            /// </summary>
            [DataMember]
            public string screen_name { get; set; }
        }

        [DataContract]
        [Serializable]
        public class QueryAccessTokenResponseData
        {
            [DataMember]
            public string access_token { get; set; }
        }
    }
}