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
        public long JobModifyIndex { get; set; }
        public string NodeID { get; set; }
        public long NodeModifyIndex { get; set; }
        public string Status { get; set; }
        public string StatusDescription { get; set; }
        public long Wait { get; set; }
        public string NextEval { get; set; }
        public string PreviousEval { get; set; }
        public string BlockedEval { get; set; }
        public object FailedTGAllocs { get; set; }
        //public ClassEligibility ClassEligibility { get; set; }
        public bool EscapedComputedClass { get; set; }
        public bool AnnotatePlan { get; set; }
        public Dictionary<string, dynamic> QueuedAllocations { get; set; }
        public long SnapshotIndex { get; set; }
        public long CreateIndex { get; set; }
        public long ModifyIndex { get; set; }

        // Custom Properties
        public List<Allocation> Allocations { get; set; }
    }
}
