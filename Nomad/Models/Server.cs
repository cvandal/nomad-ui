using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nomad.Models
{
    public class Server
    {
        public string ServerName { get; set; }
        public string ServerRegion { get; set; }
        public string ServerDC { get; set; }
        public List<Member> Members { get; set; }
        public Member Member { get; set; }
        public Operator Operator { get; set; }
        public Config Config { get; set; }
        public Stats Stats { get; set; }
        public string Name { get; set; }
        public string Addr { get; set; }
        public int Port { get; set; }
        public string Status { get; set; }

        // Custom Properties
        public bool Leader { get; set; }
    }

    public class Member
    {
        public string Name { get; set; }
        public string Addr { get; set; }
        public int Port { get; set; }
        public Dictionary<string, dynamic> Tags { get; set; }
        public string Status { get; set; }
        public int ProtocolMin { get; set; }
        public int ProtocolMax { get; set; }
        public int ProtocolCur { get; set; }
        public int DelegateMin { get; set; }
        public int DelegateMax { get; set; }
        public int DelegateCur { get; set; }
        

        // Custom Properties
        public int Up { get; set; }
        public int Down { get; set; }
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
        public bool Leader { get; set; }
        public string Node { get; set; }
        public bool Voter { get; set; }
    }
}
