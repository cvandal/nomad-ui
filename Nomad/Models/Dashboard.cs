using System.Collections.Generic;
using System.Linq;

namespace Nomad.Models
{
    public class Dashboard
    {
        public List<Job> Jobs { get; set; }
        public List<Allocation> Allocations { get; set; }
        public List<Event> Events { get; set; }
        public List<Client> Clients { get; set; }
        public List<Member> Servers { get; set; }

        // Jobs
        public long PendingJobs => Jobs.Sum(j => j.Pending);
        public long RunningJobs => Jobs.Sum(j => j.Running);
        public long DeadJobs => Jobs.Sum(j => j.Dead);

        // Allocations
        public long PendingAllocations => Allocations.Sum(a => a.Pending);
        public long RunningAllocations => Allocations.Sum(a => a.Running);
        public long DeadAllocations => Allocations.Sum(a => a.Dead);

        // Clients
        public long UpClients => Clients.Sum(c => c.Up);
        public long DownClients => Clients.Sum(c => c.Down);
        public long DrainingClients => Clients.Sum(c => c.Draining);

        // Servers
        public long UpMembers => Servers.Sum(s => s.Up);
        public long DownMembers => Servers.Sum(s => s.Down);
    }
}
