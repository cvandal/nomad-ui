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
        public int PendingJobs =>Jobs.Sum(j => j.Pending);

        public int RunningJobs => Jobs.Sum(j => j.Running);

        public int DeadJobs => Jobs.Sum(j => j.Dead);

        // Allocations
        public int PendingAllocations => Allocations.Sum(a => a.Pending);

        public int RunningAllocations => Allocations.Sum(a => a.Running);

        public int DeadAllocations => Allocations.Sum(a => a.Dead);

        // Clients
        public int UpClients => Clients.Sum(c => c.Up);

        public int DownClients => Clients.Sum(c => c.Down);

        // Servers
        public int UpMembers => Servers.Sum(s => s.Up);

        public int DownMembers => Servers.Sum(s => s.Down);
    }
}
