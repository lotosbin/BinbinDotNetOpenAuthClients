using System;
using System.Runtime.Serialization;

namespace BinbinDotNetOpenAuth.AspNet.Clients
{
    [DataContract]
    [Serializable]
    public class TaobaoUserData
    {
        [DataMember]
        public string user_id { get; set; }

        [DataMember]
        public string nick { get; set; }
    }
}