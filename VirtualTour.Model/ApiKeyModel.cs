using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTour.Model
{
    public class ApiKeyModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string HashKey { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiredAt { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime? LastUsedAt { get; set; }
    }
}
