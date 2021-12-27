using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModuleCore
{
    [Serializable]
    public class StateInfo
    {

        //public int? rowid { get; set; }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        [JsonProperty]
        public string Information { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }


    }
}
