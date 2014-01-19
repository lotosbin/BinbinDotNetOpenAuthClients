using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using System.Web;
using DotNetOpenAuth.AspNet.Clients;
using log4net;
using Newtonsoft.Json;

namespace BinbinDotNetOpenAuth.AspNet.Clients
{
    /// <summary>
    ///     A DotNetOpenAuth client for logging in to Google using OAuth2.
    ///     Reference: https://developers.google.com/accounts/docs/OAuth2
    /// </summary>
    public class SohuClient : OAuth2Client
    {
        private static readonly ILog log = LogManager.GetLogger(typeof (SohuClient));

        #region Constants and Fields

        /// <summary>
        ///     The authorization endpoint.
        /// </summary>
        private const string AuthorizationEndpoint = "https://api.sohu.com/oauth2/authorize";

        /// <summary>
        ///     The token endpoint.
        /// </summary>
        private const string TokenEndpoint = "https://api.sohu.com/oauth2/token";

        /// <summary>
        ///     The user info endpoint.
        /// </summary>
        private const string UserInfoEndpoint = "https://api.sohu.com/rest/pp/prv/1/user/get_info";

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
        public SohuClient(string clientId, string clientSecret)
            : this(clientId, clientSecret, new string[]
                                           {
                                           })
        {
        }

        /// <summary>
        ///     Creates a new Google OAuth2 client.
        /// </summary>
        /// <param name="clientId">The Google Client Id</param>
        /// <param name="clientSecret">The Google Client Secret</param>
        /// <param name="requestedScopes">One or more requested scopes, passed without the base URI.</param>
        public SohuClient(string clientId, string clientSecret, params string[] requestedScopes)
            : base("sohu")
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
            var uid = (string) HttpContext.Current.Session["uid"];
            var collection = new NameValueCollection
                             {
                                 {"access_token", accessToken},
                             };
            string json = UriHelper.OAuthGet(UserInfoEndpoint, collection);
            var result = JsonConvert.DeserializeObject<GetUserDataResult>(json);
            if (result.status != "10200")
            {
                log.Error("GetUserData:" + json);
            }
            var extraData = new Dictionary<string, string>
                            {
                                {"id", uid},
                                {"name", result.data.uniqname},
                            };
            return extraData;
        }

        protected override string QueryAccessToken(Uri returnUrl, string authorizationCode)
        {
            var valueCollection = new NameValueCollection
                                  {
                                      {"grant_type", "authorization_code"},
                                      {"code", authorizationCode},
                                      {"client_id", this._clientId},
                                      {"client_secret", this._clientSecret},
                                      {"redirect_uri", returnUrl.GetLeftPart(UriPartial.Path)},
                                  };
            string json = UriHelper.OAuthGet(TokenEndpoint, valueCollection);
            if (json == null)
            {
                return null;
            }
            NameValueCollection results = HttpUtility.ParseQueryString(json);
            string accessToken = results["access_token"];

            HttpContext.Current.Session["uid"] = results["open_id"];
            return accessToken;
        }

        [DataContract]
        [Serializable]
        public class GetUserDataResult
        {
            /// <summary>
            ///     ×´Ì¬Âë£¨×¢£º10200Îª³É¹¦£©
            /// </summary>
            [DataMember]
            public string status { get; set; }

            /// <summary>
            ///     ×´Ì¬ÃèÊö
            /// </summary>
            [DataMember]
            public string message { get; set; }

            [DataMember]
            public Data data { get; set; }

            [DataContract]
            [Serializable]
            public class Data
            {
                /// <summary>
                ///     êÇ³Æ
                /// </summary>
                [DataMember]
                public string uniqname { get; set; }
            }
        }
    }
}