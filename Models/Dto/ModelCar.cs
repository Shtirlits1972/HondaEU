using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HondaEU.Models.Dto
{
    public class ModelCar
    {
        public  string model_id { get; set; }  //  
        public string name { get; set; }
        public string seo_url { get; set; }

        public override string ToString()
        {
            return name;
        }
    }
}
