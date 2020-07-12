using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HondaEU.Models.Dto
{
    public class Sgroups
    {
        public string node_id { get; set; }
        public string name { get; set; }
        public string image_id { get; set; }
        public string image_ext { get; set; }
        public List<attributes> attributes { get; set; }
        public override string ToString()
        {
            return name;
        }
    }
}
