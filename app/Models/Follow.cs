using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace app.Models
{
    public class Follow
    {
        public int Id { get; set; }
        public int FollowerId { get; set; }
        public int FollowingId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public User Follower { get; set; } = null!;
        public User Following { get; set; } = null!;
    }
}