using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HondaEU.Models.Dto
{
    public class Detail
    {
        public string name { get; set; }
        public string number { get; set; }


        public string npl { get; set; }
        public string nplblk { get; set; }
        public string nplpartref { get; set; }



        public List<hotspots> hotspots { get; set; }
        public List<attributes> attributes { get; set; }
        public List<attributes> links { get; set; }

        public override string ToString()
        {
            return name;
        }
    }
}
