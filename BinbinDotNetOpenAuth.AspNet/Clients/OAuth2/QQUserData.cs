using System;
using System.Runtime.Serialization;

namespace BinbinDotNetOpenAuth.AspNet.Clients
{
    [DataContract]
    [Serializable]
    public class QQUserData
    {
        [DataMember]
        public string nickname { get; set; }
    }
}