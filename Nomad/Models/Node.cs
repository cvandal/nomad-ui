using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nomad.Models
{
    public class Node
    {
        public string ID { get; set; }
        public string SecretID { get; set; }
        public string Datacenter { get; set; }
        public string Name { get; set; }
        public string HTTPAddr { get; set; }
        public bool TLSEnabled { get; set; }
        public Dictionary<string, dynamic> Attributes { get; set; }
        public Resources Resources { get; set; }
        public Reserved Reserved { get; set; }
        public Dictionary<string, dynamic> Links { get; set; }
        public object Meta { get; set; }
        public string NodeClass { get; set; }
        public string ComputedClass { get; set; }
        public bool Drain { get; set; }
        public string Status { get; set; }
        public string StatusDescription { get; set; }
        public int StatusUpdatedAt { get; set; }
        public int CreateIndex { get; set; }
        public int ModifyIndex { get; set; }
        public Stats Stats { get; set; }
        public ResourceUsage ResourceUsage { get; set; }
        public long Timestamp { get; set; }
        public List<Allocation> Allocations { get; set; }

        // Custom Properties
        public int Up { get; set; }
        public int Down { get; set; }
    }

    public class Reserved
    {
        public int CPU { get; set; }
        public int MemoryMB { get; set; }
        public int DiskMB { get; set; }
        public int IOPS { get; set; }
        public List<Network> Networks { get; set; }
    }

    public class Stats
    {
        public AllocDirStats AllocDirStats { get; set; }
        public List<AllocationCPU> CPU { get; set; }
        public double CPUTicksConsumed { get; set; }
        public List<DiskStat> DiskStats { get; set; }
        public Memory Memory { get; set; }
        public long Timestamp { get; set; }
        public int Uptime { get; set; }
        public ResourceUsage ResourceUsage { get; set; }
        public Dictionary<string, dynamic> Nomad { get; set; }
        public Dictionary<string, dynamic> Raft { get; set; }
        public Dictionary<string, dynamic> Serf { get; set; }
        public Dictionary<string, dynamic> Runtime { get; set; }
    }

    public class AllocDirStats
    {
        public long Available { get; set; }
        public string Device { get; set; }
        public double InodesUsedPercent { get; set; }
        public string Mountpoint { get; set; }
        public long Size { get; set; }
        public long Used { get; set; }
        public double UsedPercent { get; set; }
    }

    public class AllocationCPU
    {
        public string CPU { get; set; }
        public double Idle { get; set; }
        public double System { get; set; }
        public double Total { get; set; }
        public double User { get; set; }
    }

    public class DiskStat
    {
        public long Available { get; set; }
        public string Device { get; set; }
        public double InodesUsedPercent { get; set; }
        public string Mountpoint { get; set; }
        public long Size { get; set; }
        public long Used { get; set; }
        public double UsedPercent { get; set; }
    }

    public class Memory
    {
        public long Available { get; set; }
        public long Free { get; set; }
        public long Total { get; set; }
        public long Used { get; set; }
    }

    public class ResourceUsage
    {
        public CpuStats CpuStats { get; set; }
        public MemoryStats MemoryStats { get; set; }
    }

    public class CpuStats
    {
        public List<string> Measured { get; set; }
        public double Percent { get; set; }
        public long SystemMode { get; set; }
        public long ThrottledPeriods { get; set; }
        public long ThrottledTime { get; set; }
        public double TotalTicks { get; set; }
        public double UserMode { get; set; }
    }

    public class MemoryStats
    {
        public int Cache { get; set; }
        public int KernelMaxUsage { get; set; }
        public int KernelUsage { get; set; }
        public int MaxUsage { get; set; }
        public List<string> Measured { get; set; }
        public int RSS { get; set; }
        public int Swap { get; set; }
    }
}
