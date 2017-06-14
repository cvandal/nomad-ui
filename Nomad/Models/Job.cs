using System.Collections.Generic;

namespace Nomad.Models
{
    public class Job
    {
        public string Region { get; set; }
        public string ID { get; set; }
        public string ParentID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public long Priority { get; set; }
        public bool AllAtOnce { get; set; }
        public List<string> Datacenters { get; set; }
        public List<Constraint> Constraints { get; set; }
        public List<TaskGroup> TaskGroups { get; set; }
        public Update Update { get; set; }
        public object Periodic { get; set; }
        public object ParameterizedJob { get; set; }
        public object Payload { get; set; }
        public Dictionary<string, dynamic> Meta { get; set; }
        public string VaultToken { get; set; }
        public string Status { get; set; }
        public string StatusDescription { get; set; }
        public JobSummary JobSummary { get; set; }
        public long CreateIndex { get; set; }
        public long ModifyIndex { get; set; }
        public long JobModifyIndex { get; set; }
        public List<Evaluation> Evaluations { get; set; }
        public List<Allocation> Allocations { get; set; }

        // Custom Properties
        public long Dead { get; set; }
        public long Pending { get; set; }
        public long Running { get; set; }
    }

    public class Constraint
    {
        public string LTarget { get; set; }
        public string RTarget { get; set; }
        public string Operand { get; set; }
    }

    public class TaskGroup
    {
        public string Name { get; set; }
        public long Count { get; set; }
        public List<Constraint> Constraints { get; set; }
        public RestartPolicy RestartPolicy { get; set; }
        public List<Task> Tasks { get; set; }
        public EphemeralDisk EphemeralDisk { get; set; }
        public Dictionary<string, dynamic> Meta { get; set; }
    }

    public class RestartPolicy
    {
        public long Attempts { get; set; }
        public long longerval { get; set; }
        public long Delay { get; set; }
        public string Mode { get; set; }
    }

    public class Task
    {
        public string Name { get; set; }
        public string Driver { get; set; }
        public string User { get; set; }
        public Config Config { get; set; }
        public Dictionary<string, dynamic> Env { get; set; }
        public List<Service> Services { get; set; }
        public object Vault { get; set; }
        public List<object> Templates { get; set; }
        public object Constralongs { get; set; }
        public Resources Resources { get; set; }
        public object DispatchPayload { get; set; }
        public List<Constraint> Constraints { get; set; }
        public Dictionary<string, dynamic> Meta { get; set; }
        public long KillTimeout { get; set; }
        public LogConfig LogConfig { get; set; }
        public List<object> Artifacts { get; set; }
        public bool Leader { get; set; }
    }

    public class Config
    {
        public string Image { get; set; }
        public string Network_Mode { get; set; }
        public List<string> Args { get; set; }
        public string Command { get; set; }
        public AdvertiseAddrs AdvertiseAddrs { get; set; }
        public string BindAddr { get; set; }
        public string DataDir { get; set; }
        public string Datacenter { get; set; }
        public List<string> Files { get; set; }
        public string LogLevel { get; set; }
        public string Region { get; set; }
        public string Version { get; set; }
    }

    public class Service
    {
        public string Name { get; set; }
        public string PortLabel { get; set; }
        public List<string> Tags { get; set; }
        public List<Check> Checks { get; set; }
    }

    public class Check
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Command { get; set; }
        public List<string> Args { get; set; }
        public string Path { get; set; }
        public string Protocol { get; set; }
        public string PortLabel { get; set; }
        public long longerval { get; set; }
        public long Timeout { get; set; }
        public string InitialStatus { get; set; }
    }

    public class Resources
    {
        public long CPU { get; set; }
        public long MemoryMB { get; set; }
        public long DiskMB { get; set; }
        public long IOPS { get; set; }
        public List<Network> Networks { get; set; }
    }

    public class Network
    {
        public string Device { get; set; }
        public string CIDR { get; set; }
        public string IP { get; set; }
        public long MBits { get; set; }
        public object ReservedPorts { get; set; }
        public List<DynamicPort> DynamicPorts { get; set; }
    }

    public class DynamicPort
    {
        public string Label { get; set; }
        public long Value { get; set; }
    }

    public class LogConfig
    {
        public long MaxFiles { get; set; }
        public long MaxFileSizeMB { get; set; }
    }

    public class EphemeralDisk
    {
        public bool Sticky { get; set; }
        public long SizeMB { get; set; }
        public bool Migrate { get; set; }
    }

    public class Update
    {
        public long Stagger { get; set; }
        public long MaxParallel { get; set; }
    }

    public class JobSummary
    {
        public string JobID { get; set; }
        public Dictionary<string, TaskGroupStatus> Summary { get; set; }
        public object Children { get; set; }
        public long CreateIndex { get; set; }
        public long ModifyIndex { get; set; }
    }

    public class TaskGroupStatus
    {
        public long Queued { get; set; }
        public long Complete { get; set; }
        public long Failed { get; set; }
        public long Running { get; set; }
        public long Starting { get; set; }
        public long Lost { get; set; }
    }
}
