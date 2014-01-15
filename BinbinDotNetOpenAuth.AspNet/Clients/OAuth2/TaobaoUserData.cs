using System;
using System.Runtime.Serialization;

namespace BinbinDotNetOpenAuth.AspNet.Clients
{
    [DataContract]
    [Serializable]
    public class TaobaoUserData
    {
        [DataMember]
        public string uid { get; set; }

        [DataMember]
        public string nickname { get; set; }
    }
}