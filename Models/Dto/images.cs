using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HondaEU.Models.Dto
{
    public class images
    {
        public string image_id { get; set; }
        public string ext { get; set; }
        public string path { get; set; }
        public override string ToString()
        {
            return $"{image_id} {ext}";
        }
    }
}
