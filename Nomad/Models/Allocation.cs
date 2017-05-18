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
        public Job Job { get; set; }
        public string TaskGroup { get; set; }
        public Resources Resources { get; set; }
        public SharedResources SharedResources { get; set; }
        public Dictionary<string, TaskName> TaskResources { get; set; }
        public Metrics Metrics { get; set; }
        public string DesiredStatus { get; set; }
        public string DesiredDescription { get; set; }
        public string ClientStatus { get; set; }
        public string ClientDescription { get; set; }
        public Dictionary<string, TaskName> TaskStates { get; set; }
        public string PreviousAllocation { get; set; }
        public long CreateIndex { get; set; }
        public long ModifyIndex { get; set; }
        public long AllocModifyIndex { get; set; }
        public long CreateTime { get; set; }
        public Stats Stats { get; set; }
        public List<Log> Logs { get; set; }

        // Custom Properties
        public long Pending { get; set; }
        public long Running { get; set; }
        public long Dead { get; set; }

        public DateTime CreateDateTime => CreateTime.FromUnixTime();
    }

    public class SharedResources
    {
        public long CPU { get; set; }
        public long MemoryMB { get; set; }
        public long DiskMB { get; set; }
        public long IOPS { get; set; }
        public object Networks { get; set; }
    }

    public class TaskName
    {
        public string State { get; set; }
        public bool Failed { get; set; }
        public string StartedAt { get; set; }
        public string FinishedAt { get; set; }
        public List<Event> Events { get; set; }
        public long CPU { get; set; }
        public long MemoryMB { get; set; }
        public long DiskMB { get; set; }
        public long IOPS { get; set; }
        public List<Network> Networks { get; set; }
    }

    public class Event
    {
        public string Type { get; set; }
        public long Time { get; set; }
        public bool FailsTask { get; set; }
        public string RestartReason { get; set; }
        public string SetupError { get; set; }
        public string DriverError { get; set; }
        public long ExitCode { get; set; }
        public long Signal { get; set; }
        public string Message { get; set; }
        public long KillTimeout { get; set; }
        public string KillError { get; set; }
        public string KillReason { get; set; }
        public long StartDelay { get; set; }
        public string DownloadError { get; set; }
        public string ValidationError { get; set; }
        public long DiskLimit { get; set; }
        public string FailedSibling { get; set; }
        public string VaultError { get; set; }
        public string TaskSignalReason { get; set; }
        public string TaskSignal { get; set; }
        public string DriverMessage { get; set; }

        // Custom Properties
        public string AllocationId { get; set; }
        public string AllocationName { get; set; }

        public DateTime DateTime => Time.FromUnixTime();
    }

    public class Metrics
    {
        public long NodesEvaluated { get; set; }
        public long NodesFiltered { get; set; }
        public Dictionary<string, dynamic> NodesAvailable { get; set; }
        public object ClassFiltered { get; set; }
        public object ConstralongFiltered { get; set; }
        public long NodesExhausted { get; set; }
        public object ClassExhausted { get; set; }
        public object DimensionExhausted { get; set; }
        public Dictionary<string, dynamic> Scores { get; set; }
        public long AllocationTime { get; set; }
        public long CoalescedFailures { get; set; }
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
