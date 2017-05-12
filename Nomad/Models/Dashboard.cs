using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public int PendingJobs
        {
            get
            {
                return Jobs.Sum(j => j.Pending);
            }
        }

        public int RunningJobs
        {
            get
            {
                return Jobs.Sum(j => j.Running);
            }
        }

        public int DeadJobs
        {
            get
            {
                return Jobs.Sum(j => j.Dead);
            }
        }

        // Allocations
        public int PendingAllocations
        {
            get
            {
                return Allocations.Sum(a => a.Pending);
            }
        }

        public int RunningAllocations
        {
            get
            {
                return Allocations.Sum(a => a.Pending);
            }
        }

        public int DeadAllocations
        {
            get
            {
                return Allocations.Sum(a => a.Pending);
            }
        }

        // Nodes
        public int UpNodes
        {
            get
            {
                return Nodes.Sum(n => n.Up);
            }
        }

        public int DownNodes
        {
            get
            {
                return Nodes.Sum(n => n.Down);
            }
        }

        // Members
        public int UpMembers
        {
            get
            {
                return Agent.Members.Sum(a => a.Up);
            }
        }

        public int DownMembers
        {
            get
            {
                return Agent.Members.Sum(a => a.Down);
            }
        }
    }
}
