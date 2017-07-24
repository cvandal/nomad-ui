using System.Collections.Generic;
using System.Linq;

namespace Nomad.Models
{
    public class Dashboard
    {
        public List<Job> Jobs { get; set; }
        public int RunningJobs => Jobs.Sum(j => j.Running);
        public int PendingJobs => Jobs.Sum(j => j.Pending);
        public int DeadJobs => Jobs.Sum(j => j.Dead);

        public List<Allocation> Allocations { get; set; }
        public int RunningAllocations => Allocations.Sum(a => a.Running);
        public int PendingAllocations => Allocations.Sum(a => a.Pending);
        public int DeadAllocations => Allocations.Sum(a => a.Dead);

        public List<Client> Clients { get; set; }
        public int UpClients => Clients.Sum(c => c.Up);
        public int DownClients => Clients.Sum(c => c.Down);
        public int DrainingClients => Clients.Sum(c => c.Draining);

        public List<Member> Servers { get; set; }
        public int UpServers => Servers.Sum(s => s.Up);
        public int DownServers => Servers.Sum(s => s.Down);

        public List<Event> Events { get; set; }
    }
}
