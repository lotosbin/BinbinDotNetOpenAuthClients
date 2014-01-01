using System;
using System.Runtime.Serialization;

namespace BinbinDotNetOpenAuth.AspNet.Clients
{
    [DataContract]
    [Serializable]
    public class WeiboUserData
    {
        [DataMember]
        public string id { get; set; }

        [DataMember]
        public string screen_name { get; set; }

        [DataMember]
        public string name { get; set; }
    }
}