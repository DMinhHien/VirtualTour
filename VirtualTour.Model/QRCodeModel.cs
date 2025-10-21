using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTour.Model
{

    public class QRCodeModel
    {
        public int IDQR { get; set; }
        public string? QRName { get; set; }
        public string? QRType { get; set; }
        public string? QRContent { get; set; }
        public string? SSIDName { get; set; }
        public string? SecurityType { get; set; }
        public string? WifiPassHash { get; set; }    
        
        public bool IsChange { get; set; }
        public string? ChangeOnDays { get; set; }
        public bool InActive { get; set; }
        public string? CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime LastUpdated { get; set; }
    }
    public class QRCodeCreateRequest
    {
        public int? IDQR { get; set; }
        public string? QRName { get; set; }
        public string? QRType { get; set; }
        public string? QRContent { get; set; }
        public string? SSIDName { get; set; }
        public string? WifiPassHash { get; set; }
        public string? SecurityType { get; set; }        
        public string? ChangeOnDays { get; set; }

    }
    public class QRCodeUpdateRequest
    {
        public int IDQR { get; set; }
        public string? QRName { get; set; }
        public string? QRType { get; set; }
        public string? QRContent { get; set; }
        public string? SSIDName { get; set; }
        public string? WifiPassHash { get; set; }
        public string? SecurityType { get; set; }
        public string? ChangeOnDays { get; set; }
       
    }
}
