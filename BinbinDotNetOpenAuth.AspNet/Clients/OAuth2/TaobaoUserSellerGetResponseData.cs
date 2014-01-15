using System;
using System.Runtime.Serialization;

namespace BinbinDotNetOpenAuth.AspNet.Clients
{
    [DataContract]
    [Serializable]
    public class TaobaoUserSellerGetResponseData
    {
        [DataMember]
        public TaobaoUserData user { get; set; }
    }
}