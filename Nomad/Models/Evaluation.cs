using System.Collections.Generic;

namespace Nomad.Models
{
    public class Evaluation
    {
        public string ID { get; set; }
        public int Priority { get; set; }
        public string Type { get; set; }
        public string TriggeredBy { get; set; }
        public string JobID { get; set; }
        public int JobModifyIndex { get; set; }
        public string NodeID { get; set; }
        public int NodeModifyIndex { get; set; }
        public string Status { get; set; }
        public string StatusDescription { get; set; }
        public int Wait { get; set; }
        public string NextEval { get; set; }
        public string PreviousEval { get; set; }
        public string BlockedEval { get; set; }
        public object FailedTGAllocs { get; set; }
        public bool EscapedComputedClass { get; set; }
        public bool AnnotatePlan { get; set; }
        public Dictionary<string, dynamic> QueuedAllocations { get; set; }
        public int SnapshotIndex { get; set; }
        public int CreateIndex { get; set; }
        public int ModifyIndex { get; set; }
        public List<Allocation> Allocations { get; set; }
    }
}
