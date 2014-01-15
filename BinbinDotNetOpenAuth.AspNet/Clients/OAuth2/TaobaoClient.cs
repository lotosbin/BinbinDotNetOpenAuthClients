using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Web;
using DotNetOpenAuth.AspNet.Clients;
using log4net;
using Newtonsoft.Json;
using Top.Api;
using Top.Api.Request;
using Top.Api.Response;

namespace BinbinDotNetOpenAuth.AspNet.Clients
{
    /// <summary>
    ///     A DotNetOpenAuth client for logging in to Google using OAuth2.
    ///     Reference: https://developers.google.com/accounts/docs/OAuth2
    /// </summary>
    public class TaobaoClient : OAuth2Client
    {
        private static readonly ILog log = LogManager.GetLogger(typeof (TaobaoClient));

        #region Constants and Fields

        /// <summary>
        ///     The authorization endpoint.
        /// </summary>
        private const string AuthorizationEndpoint = "https://oauth.taobao.com/authorize";

        /// <summary>
        ///     The token endpoint.
        /// </summary>
        private const string TokenEndpoint = "https://oauth.taobao.com/token";

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
        public TaobaoClient(string clientId, string clientSecret)
            : this(clientId, clientSecret, new[]
                                           {
                                               "email"
                                           })
        {
        }

        /// <summary>
        ///     Creates a new Google OAuth2 client.
        /// </summary>
        /// <param name="clientId">The Google Client Id</param>
        /// <param name="clientSecret">The Google Client Secret</param>
        /// <param name="requestedScopes">One or more requested scopes, passed without the base URI.</param>
        public TaobaoClient(string clientId, string clientSecret, params string[] requestedScopes)
            : base("taobao")
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
            log.Info("GetServiceLoginUrl");
            IEnumerable<string> scopes = this._requestedScopes;
            string state = string.IsNullOrEmpty(returnUrl.Query) ? string.Empty : returnUrl.Query.Substring(1);

            return UriHelper.BuildUri(AuthorizationEndpoint, new NameValueCollection
                                                             {
                                                                 {"response_type", "code"},
                                                                 {"client_id", this._clientId},
                                                                 {"scope", string.Join(" ", scopes)},
                                                                 {"redirect_uri", returnUrl.GetLeftPart(UriPartial.Path)},
                                                                 {"state", state},
                                                             });
        }

        protected override IDictionary<string, string> GetUserData(string accessToken)
        {
            log.Info("GetUserData");
            const string url = "http://gw.api.taobao.com/router/rest";
            //沙箱环境：http://gw.api.tbsandbox.com/router/rest
            ITopClient myclient = new DefaultTopClient(url, this._clientId, this._clientSecret); //实例化ITopClient类
            var req = new UserSellerGetRequest
                      {
                          Fields = "nick,user_id,type"
                      }; //实例化具体API对应的Request类
            UserSellerGetResponse rsp = myclient.Execute(req, accessToken); //执行API请求并将该类转换为response对象

            string json = rsp.Body;
            log.Info("response:" + rsp.Body);
            var data = JsonConvert.DeserializeObject<TaobaoResponseData>(json);
            var extraData = new Dictionary<string, string>
                            {
                                {"id", data.user_seller_get_response.user.uid},
                                {"name", data.user_seller_get_response.user.nickname},
                            };
            return extraData;
        }

        protected override string QueryAccessToken(Uri returnUrl, string authorizationCode)
        {
            log.Info("QueryAccessToken");
            var valueCollection = new NameValueCollection
                                  {
                                      {"grant_type", "authorization_code"},
                                      {"code", authorizationCode},
                                      {"client_id", this._clientId},
                                      {"client_secret", this._clientSecret},
                                      {"redirect_uri", returnUrl.GetLeftPart(UriPartial.Path)},
                                  };
            string json = OAuthGet(valueCollection);
            if (json == null)
            {
                return null;
            }
            log.Info("response:" + json);
            NameValueCollection results = HttpUtility.ParseQueryString(json);
            string accessToken = results["code"];
            return accessToken;
        }

        private static string OAuthGet(NameValueCollection valueCollection)
        {
            Uri uri = UriHelper.BuildUri(TokenEndpoint, valueCollection);

            var webRequest = (HttpWebRequest) WebRequest.Create(uri);

            string json;
            using (WebResponse webResponse = webRequest.GetResponse())
            {
                using (Stream stream = webResponse.GetResponseStream())
                {
                    if (stream == null)
                    {
                        return null;
                    }

                    using (var textReader = new StreamReader(stream))
                    {
                        json = textReader.ReadToEnd();
                    }
                }
            }
            return json;
        }
    }
}