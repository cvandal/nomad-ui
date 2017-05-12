using System.Collections.Generic;
using System.Linq;

namespace Nomad.Models
{
    public class Dashboard
    {
        public List<Job> Jobs { get; set; }
        public List<Allocation> Allocations { get; set; }
        public List<Event> Events { get; set; }
        public List<Node> Nodes { get; set; }
        public Agent Agent { get; set; }

        // Jobs
        public int PendingJobs =>Jobs.Sum(j => j.Pending);

        public int RunningJobs => Jobs.Sum(j => j.Running);

        public int DeadJobs => Jobs.Sum(j => j.Dead);

        // Allocations
        public int PendingAllocations => Allocations.Sum(a => a.Pending);

        public int RunningAllocations => Allocations.Sum(a => a.Running);

        public int DeadAllocations => Allocations.Sum(a => a.Dead);

        // Nodes
        public int UpNodes => Nodes.Sum(n => n.Up);

        public int DownNodes => Nodes.Sum(n => n.Down);

        // Members
        public int UpMembers => Agent.Members.Sum(a => a.Up);

        public int DownMembers => Agent.Members.Sum(a => a.Down);
    }
}
