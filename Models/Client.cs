using System.Collections.Generic;

namespace Nomad.Models
{
    public class Client
    {
        public string ID { get; set; }
        public string Datacenter { get; set; }
        public string Name { get; set; }
        public string NodeClass { get; set; }
        public bool Drain { get; set; }
        public string Status { get; set; }
        public string StatusDescription { get; set; }
        public long CreateIndex { get; set; }
        public long ModifyIndex { get; set; }
        public string SecretID { get; set; }
        public string HTTPAddr { get; set; }
        public bool TLSEnabled { get; set; }
        public Dictionary<string, dynamic> Attributes { get; set; }
        public Resources Resources { get; set; }
        public Reserved Reserved { get; set; }
        public Dictionary<string, dynamic> Links { get; set; }
        public Dictionary<string, dynamic> Meta { get; set; }
        public string ComputedClass { get; set; }
        public int StatusUpdatedAt { get; set; }

        // Custom Properties
        public int Up { get; set; }
        public int Down { get; set; }
        public int Draining { get; set; }
        public Stats Stats { get; set; }
        public List<Allocation> Allocations { get; set; }
    }

    public class Stats
    {
        public ResourceUsage ResourceUsage { get; set; }
        public Dictionary<string, TaskName> Tasks { get; set; }
        public long Timestamp { get; set; }
        public AllocDirStats AllocDirStats { get; set; }
        public List<ClientCPU> CPU { get; set; }
        public double CPUTicksConsumed { get; set; }
        public List<DiskStat> DiskStats { get; set; }
        public Memory Memory { get; set; }
        public int Uptime { get; set; }
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
        public double SystemMode { get; set; }
        public int ThrottledPeriods { get; set; }
        public int ThrottledTime { get; set; }
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

    public class Reserved
    {
        public int CPU { get; set; }
        public int MemoryMB { get; set; }
        public int DiskMB { get; set; }
        public int IOPS { get; set; }
        public List<Network> Networks { get; set; }
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

    public class ClientCPU
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
}
