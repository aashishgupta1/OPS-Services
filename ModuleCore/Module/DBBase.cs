using System;

namespace ModuleCore.Module
{
    public class DBBase
    {
        public int ID { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}
