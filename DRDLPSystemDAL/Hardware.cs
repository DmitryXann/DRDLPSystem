//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DRDLPSystemDAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class Hardware
    {
        public Hardware()
        {
            this.HardwareID = "\"\"";
            this.Name = "\"\"";
            this.OtherInfo = "\"\"";
            this.AccessLog = new HashSet<AccessLog>();
        }
    
        public int Id { get; set; }
        public string HardwareID { get; set; }
        public string Name { get; set; }
        public string OtherInfo { get; set; }
        public HardwareTypeEnum Type { get; set; }
    
        public virtual PC PC { get; set; }
        public virtual ICollection<AccessLog> AccessLog { get; set; }
    }
}
