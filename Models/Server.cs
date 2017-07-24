using System.Collections.Generic;

namespace Nomad.Models
{
    public class Agent
    {
        public string ServerName { get; set; }
        public string ServerRegion { get; set; }
        public string ServerDC { get; set; }
        public List<Member> Members { get; set; }
        public Member Member { get; set; }
        public ServerConfig Config { get; set; }
        public ServerStats Stats { get; set; }
    }

    public class Member
    {
        public string Name { get; set; }
        public string Addr { get; set; }
        public int Port { get; set; }
        public Tags Tags { get; set; }
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
        public bool Leader { get; set; }
        public bool Voter { get; set; }
        public Operator Operator { get; set; }
    }

    public class Tags
    {
        public string Build { get; set; }
        public string Port { get; set; }
        public string Expect { get; set; }
        public string Role { get; set; }
        public string Region { get; set; }
        public string DC { get; set; }
        public string VSN { get; set; }
        public string MNV { get; set; }
    }

    public class Operator
    {
        public long Index { get; set; }
        public List<Server> Servers { get; set; }
    }

    public class Server
    {
        public string Address { get; set; }
        public string ID { get; set; }
        public bool Leader { get; set; }
        public string Node { get; set; }
        public bool Voter { get; set; }
    }

    public class ServerConfig
    {
        public Addresses Addresses { get; set; }
        public AdvertiseAddrs AdvertiseAddrs { get; set; }
        public Atlas Atlas { get; set; }
        public string BindAddr { get; set; }
        public Client Client { get; set; }
        public Consul Consul { get; set; }
        public string DataDir { get; set; }
        public string Datacenter { get; set; }
        public bool DevMode { get; set; }
        public bool DisableAnonymousSignature { get; set; }
        public bool DisableUpdateCheck { get; set; }
        public bool EnableDebug { get; set; }
        public bool EnableSyslog { get; set; }
        public List<string> Files { get; set; }
        //public HTTPAPIResponseHeaders HTTPAPIResponseHeaders { get; set; }
        public bool LeaveOnInt { get; set; }
        public bool LeaveOnTerm { get; set; }
        public string LogLevel { get; set; }
        public string NodeName { get; set; }
        public Ports Ports { get; set; }
        public string Region { get; set; }
        public string Revision { get; set; }
        public Server Server { get; set; }
        public string SyslogFacility { get; set; }
        public TLSConfig TLSConfig { get; set; }
        public Telemetry Telemetry { get; set; }
        public Vault Vault { get; set; }
        public string Version { get; set; }
        public string VersionPrerelease { get; set; }
    }

    public class Addresses
    {
        public string HTTP { get; set; }
        public string RPC { get; set; }
        public string Serf { get; set; }
    }

    public class AdvertiseAddrs
    {
        public string HTTP { get; set; }
        public string RPC { get; set; }
        public string Serf { get; set; }
    }

    public class Atlas
    {
        public string Endpoint { get; set; }
        public string Infrastructure { get; set; }
        public bool Join { get; set; }
    }

    public class Consul
    {
        public string Addr { get; set; }
        public string Auth { get; set; }
        public bool AutoAdvertise { get; set; }
        public string CAFile { get; set; }
        public string CertFile { get; set; }
        public bool ChecksUseAdvertise { get; set; }
        public bool ClientAutoJoin { get; set; }
        public string ClientServiceName { get; set; }
        public bool EnableSSL { get; set; }
        public string KeyFile { get; set; }
        public bool ServerAutoJoin { get; set; }
        public string ServerServiceName { get; set; }
        public long Timeout { get; set; }
        public string Token { get; set; }
        public bool VerifySSL { get; set; }
    }

    public class Ports
    {
        public int HTTP { get; set; }
        public int RPC { get; set; }
        public int Serf { get; set; }
    }

    public class TLSConfig
    {
        public string CAFile { get; set; }
        public string CertFile { get; set; }
        public bool EnableHTTP { get; set; }
        public bool EnableRPC { get; set; }
        public string KeyFile { get; set; }
        public bool VerifyServerHostname { get; set; }
    }

    public class Telemetry
    {
        public string CirconusAPIApp { get; set; }
        public string CirconusAPIToken { get; set; }
        public string CirconusAPIURL { get; set; }
        public string CirconusBrokerID { get; set; }
        public string CirconusBrokerSelectTag { get; set; }
        public string CirconusCheckDisplayName { get; set; }
        public string CirconusCheckForceMetricActivation { get; set; }
        public string CirconusCheckID { get; set; }
        public string CirconusCheckInstanceID { get; set; }
        public string CirconusCheckSearchTag { get; set; }
        public string CirconusCheckSubmissionURL { get; set; }
        public string CirconusCheckTags { get; set; }
        public string CirconusSubmissionInterval { get; set; }
        public string CollectionInterval { get; set; }
        public string DataDogAddr { get; set; }
        public bool DisableHostname { get; set; }
        public bool PublishAllocationMetrics { get; set; }
        public bool PublishNodeMetrics { get; set; }
        public string StatsdAddr { get; set; }
        public string StatsiteAddr { get; set; }
        public bool UseNodeName { get; set; }
    }

    public class Vault
    {
        public string Addr { get; set; }
        public bool AllowUnauthenticated { get; set; }
        public long ConnectionRetryIntv { get; set; }
        public object Enabled { get; set; }
        public string Role { get; set; }
        public string TLSCaFile { get; set; }
        public string TLSCaPath { get; set; }
        public string TLSCertFile { get; set; }
        public string TLSKeyFile { get; set; }
        public string TLSServerName { get; set; }
        public object TLSSkipVerify { get; set; }
        public string TaskTokenTTL { get; set; }
        public string Token { get; set; }
    }

    public class ServerStats
    {
        public Raft Raft { get; set; }
        public Serf Serf { get; set; }
        public Runtime Runtime { get; set; }
        public Nomad Nomad { get; set; }
    }

    public class Raft
    {
        public string Applied_Index { get; set; }
        public string Last_Snapshot_Index { get; set; }
        public string Protocol_Version_Min { get; set; }
        public string State { get; set; }
        public string Last_Log_Index { get; set; }
        public string Latest_Configuration_Index { get; set; }
        public string Latest_Configuration { get; set; }
        public string Num_Peers { get; set; }
        public string Term { get; set; }
        public string Snapshot_Version_Max { get; set; }
        public string Protocol_Version { get; set; }
        public string Snapshot_Version_Min { get; set; }
        public string Last_Contact { get; set; }
        public string Last_Log_Term { get; set; }
        public string Commit_Index { get; set; }
        public string Protocol_Version_Max { get; set; }
        public string Fsm_Pending { get; set; }
        public string Last_Snapshot_Term { get; set; }
    }

    public class Serf
    {
        public string Members { get; set; }
        public string Failed { get; set; }
        public string Left { get; set; }
        public string Event_Time { get; set; }
        public string Query_Time { get; set; }
        public string Intent_Queue { get; set; }
        public string Health_Score { get; set; }
        public string Member_Time { get; set; }
        public string Event_Queue { get; set; }
        public string Query_Queue { get; set; }
        public string Encrypted { get; set; }
    }

    public class Runtime
    {
        public string Cpu_Count { get; set; }
        public string Arch { get; set; }
        public string Version { get; set; }
        public string Max_Procs { get; set; }
        public string GoRoutines { get; set; }
    }

    public class Nomad
    {
        public string Leader_Addr { get; set; }
        public string Bootstrap { get; set; }
        public string Known_Regions { get; set; }
        public string Server { get; set; }
        public string Leader { get; set; }
    }
}
