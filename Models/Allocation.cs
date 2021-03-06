using System;
using System.Collections.Generic;
using Nomad.Extensions;

namespace Nomad.Models
{
    public class Allocation
    {
        public string ID { get; set; }
        public string EvalID { get; set; }
        public string Name { get; set; }
        public string NodeID { get; set; }
        public string JobID { get; set; }
        public string TaskGroup { get; set; }
        public string DesiredStatus { get; set; }
        public string DesiredDescription { get; set; }
        public string ClientStatus { get; set; }
        public string ClientDescription { get; set; }
        public Dictionary<string, TaskName> TaskStates { get; set; }
        public long CreateIndex { get; set; }
        public long ModifyIndex { get; set; }
        public long CreateTime { get; set; }
        public Job Job { get; set; }
        public Resources Resources { get; set; }
        public SharedResources SharedResources { get; set; }
        public Dictionary<string, TaskName> TaskResources { get; set; }
        public Metrics Metrics { get; set; }
        public string PreviousAllocation { get; set; }
        public int AllocModifyIndex { get; set; }

        // Custom Properties
        public int Running { get; set; }
        public int Pending { get; set; }
        public int Dead { get; set; }
        public DateTime CreateDateTime => CreateTime.FromUnixTime();
        public Stats Stats { get; set; }
        public List<Log> Logs { get; set; }
    }

    public class TaskName
    {
        public string State { get; set; }
        public bool Failed { get; set; }
        public List<Event> Events { get; set; }
        public object Pids { get; set; }
        public ResourceUsage ResourceUsage { get; set; }
    }

    public class Event
    {
        public string Type { get; set; }
        public long Time { get; set; }
        public bool FailsTask { get; set; }
        public string RestartReason { get; set; }
        public string SetupError { get; set; }
        public string DriverError { get; set; }
        public int ExitCode { get; set; }
        public long Signal { get; set; }
        public string Message { get; set; }
        public long KillTimeout { get; set; }
        public string KillError { get; set; }
        public string KillReason { get; set; }
        public long StartDelay { get; set; }
        public string DownloadError { get; set; }
        public string ValidationError { get; set; }
        public int DiskLimit { get; set; }
        public string FailedSibling { get; set; }
        public string VaultError { get; set; }
        public string TaskSignalReason { get; set; }
        public string TaskSignal { get; set; }
        public string DriverMessage { get; set; }

        // Custom Properties
        public string AllocationID { get; set; }
        public string AllocationName { get; set; }
        public DateTime DateTime => Time.FromUnixTime();
    }

    public class SharedResources
    {
        public int CPU { get; set; }
        public int MemoryMB { get; set; }
        public int DiskMB { get; set; }
        public int IOPS { get; set; }
        public object Networks { get; set; }
    }

    public class Metrics
    {
        public int NodesEvaluated { get; set; }
        public int NodesFiltered { get; set; }
        public object ClassFiltered { get; set; }
        public int NodesExhausted { get; set; }
        public object ClassExhausted { get; set; }
        public object DimensionExhausted { get; set; }
        public int AllocationTime { get; set; }
        public int CoalescedFailures { get; set; }
    }

    public class Log
    {
        public string FileMode { get; set; }
        public bool IsDir { get; set; }
        public string ModTime { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
    }
}
