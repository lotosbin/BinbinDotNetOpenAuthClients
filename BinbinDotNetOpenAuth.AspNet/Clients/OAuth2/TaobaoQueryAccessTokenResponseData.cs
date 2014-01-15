using System;
using System.Runtime.Serialization;

namespace BinbinDotNetOpenAuth.AspNet.Clients
{
    [DataContract]
    [Serializable]
    public class TaobaoQueryAccessTokenResponseData
    {
        [DataMember]
        public string access_token { get; set; }

        [DataMember]
        public string taobao_user_id { get; set; }

        [DataMember]
        public string taobao_user_nick { get; set; }
    }
}