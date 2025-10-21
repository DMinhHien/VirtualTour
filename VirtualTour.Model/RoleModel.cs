using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTour.Model
{
    public class RoleModel
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }
    public class RoleCreateRequest
    {
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
        public List<int> PermissionIds { get; set; } = new List<int>();
    }
}
