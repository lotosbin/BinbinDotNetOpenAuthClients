using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.Serialization;
using DotNetOpenAuth.AspNet.Clients;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BinbinDotNetOpenAuth.AspNet.Clients
{
    /// <summary>
    /// </summary>
    public class BaiduClient : OAuth2Client
    {
        #region Constants and Fields

        /// <summary>
        ///     The authorization endpoint.
        /// </summary>
        private const string AuthorizationEndpoint = "https://openapi.baidu.com/oauth/2.0/authorize";

        /// <summary>
        ///     The token endpoint.
        /// </summary>
        private const string TokenEndpoint = "https://openapi.baidu.com/oauth/2.0/token";

        /// <summary>
        ///     The user info endpoint.
        /// </summary>
        private const string UserInfoEndpoint = "https://openapi.baidu.com/rest/2.0/passport/users/getLoggedInUser";

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
        public BaiduClient(string clientId, string clientSecret)
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
        public BaiduClient(string clientId, string clientSecret, params string[] requestedScopes)
            : base("baidu")
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
            var collection = new NameValueCollection
                             {
                                 {"access_token", accessToken},
                             };
            string json = UriHelper.OAuthPost(UserInfoEndpoint, collection);
            var user = JsonConvert.DeserializeObject<GetUserDataResult>(json);
            var extraData = new Dictionary<string, string>
                            {
                                {"id", user.uid},
                                {"name", user.uname},
                            };
            return extraData;
        }

        protected override string QueryAccessToken(Uri returnUrl, string authorizationCode)
        {
            var collection = new NameValueCollection
                             {
                                 {"grant_type", "authorization_code"},
                                 {"code", authorizationCode},
                                 {"client_id", this._clientId},
                                 {"client_secret", this._clientSecret},
                                 {"redirect_uri", returnUrl.GetLeftPart(UriPartial.Path)},
                             };
            string response = UriHelper.OAuthPost(TokenEndpoint, collection);
            if (response == null)
            {
                return null;
            }
            JObject json = JObject.Parse(response);
            return json.Value<string>("access_token");
        }

        [DataContract]
        [Serializable]
        public class GetUserDataResult
        {
            [DataMember]
            public string uid { get; set; }

            [DataMember]
            public string uname { get; set; }
        }
    }
}