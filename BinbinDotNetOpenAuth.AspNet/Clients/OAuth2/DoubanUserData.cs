using System;
using System.Runtime.Serialization;

namespace BinbinDotNetOpenAuth.AspNet.Clients
{
    [DataContract]
    [Serializable]
    public class DoubanUserData
    {
        [DataMember]
        public string id { get; set; }

        [DataMember]
        public string uid { get; set; }

        [DataMember]
        public string name { get; set; }
    }
}