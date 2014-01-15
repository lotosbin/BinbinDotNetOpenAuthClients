using System;
using System.Runtime.Serialization;

namespace BinbinDotNetOpenAuth.AspNet.Clients
{
    [DataContract]
    [Serializable]
    public class TaobaoResponseData
    {
        [DataMember]
        public TaobaoUserSellerGetResponseData user_seller_get_response { get; set; }
    }
}