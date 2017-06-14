using System.Collections.Generic;

namespace Nomad.Models
{
    public class Server
    {
        public List<Member> Members { get; set; }
        public Member Member { get; set; }
        public Config Config { get; set; }
        public Stats Stats { get; set; }
    }

    public class Member
    {
        public string Name { get; set; }
        public string Addr { get; set; }
        public string Port { get; set; }
        public Dictionary<string, dynamic> Tags { get; set; }
        public string Status { get; set; }
        public long ProtocolMin { get; set; }
        public long ProtocolMax { get; set; }
        public long ProtocolCur { get; set; }
        public long DelegateMin { get; set; }
        public long DelegateMax { get; set; }
        public long DelegateCur { get; set; }
        public Operator Operator { get; set; }

        // Custom Properties
        public long Up { get; set; }
        public long Down { get; set; }
    }

    public class AdvertiseAddrs
    {
        public string HTTP { get; set; }
        public string RPC { get; set; }
        public string Serf { get; set; }
    }

    public class Operator
    {
        public List<Server2> Servers { get; set; }
    }

    public class Server2
    {
        public string Address { get; set; }
        public string ID { get; set; }
        public string Leader { get; set; }
        public string Node { get; set; }
        public string Voter { get; set; }
    }
}
