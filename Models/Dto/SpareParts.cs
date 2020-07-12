using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HondaEU.Models.Dto
{
    public class SpareParts
    {
        public string id { get; set; }
        public string name { get; set; }
        public string deskr { get; set; }
        public string image_id { get; set; }
        public List<hotspots> hotspots { get; set; }
        public override string ToString()
        {
            return name;
        }
    }
}
