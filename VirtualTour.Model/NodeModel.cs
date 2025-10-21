using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTour.Model
{
    public class NodeModel
    {
        public int Id { get; set; }
        public string Panorama { get; set; }
        public string Thumbnail { get; set; }
        public string Name { get; set; }
        public string Caption { get; set; }
        public double DefaultYaw { get; set; }
        public double DefaultPitch { get; set; }
        public bool IsStartNode { get; set; }
        public string WorkShop { get; set; }
        public string Floor { get; set; }
        public string AreaName { get; set; }
        public string DeptName { get; set; }
        public ICollection<LinkedNodes> Links { get; set; }
    }
    public class MarkerModel
    {
        public int Id { get; set; }
        public string MarkerType { get; set; }
        public string Data { get; set; }
    }
    public class LinkedNodes
    {
        public int TargetNode { get; set; }
        public double Yaw { get; set; }
        public double Pitch { get; set; }
        public int rotation { get; set; }
        public string Tooltip { get; set; }
        public int MarkerId { get; set; }
        public string? TargetName { get; set; }
    }
}
