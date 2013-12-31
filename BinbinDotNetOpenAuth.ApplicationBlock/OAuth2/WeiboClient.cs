using System;
using DotNetOpenAuth.OAuth2;

namespace BinbinDotNetOpenAuth.ApplicationBlock.OAuth2
{
    public class WeiboClient : WebServerClient
    {
        private static readonly AuthorizationServerDescription WeiboDescription = new AuthorizationServerDescription {TokenEndpoint = new Uri("https://api.weibo.com/oauth2/access_token"), AuthorizationEndpoint = new Uri("https://api.weibo.com/oauth2/authorize"), ProtocolVersion = ProtocolVersion.V20};

        public WeiboClient() : base(WeiboDescription)
        {
        }

        public WeiboClient(AuthorizationServerDescription authorizationServer, string clientIdentifier = null, string clientSecret = null) : base(authorizationServer, clientIdentifier, clientSecret)
        {
        }

        public WeiboClient(AuthorizationServerDescription authorizationServer, string clientIdentifier, ClientCredentialApplicator clientCredentialApplicator) : base(authorizationServer, clientIdentifier, clientCredentialApplicator)
        {
        }
    }
}