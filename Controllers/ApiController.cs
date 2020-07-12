using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HondaEU.Models.Dto;
using HondaEU.Models;
using System.IO;
using System.Configuration;
using System.Net.Http;
using System.Net;

namespace HondaEU.Controllers
{
    public class ApiController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("/models")]
        public IActionResult GetModels()
        {
            List<ModelCar> list = ClassCrud.GetModelCars();
            return Json(list);
        }


        [Route("/locales")]
        public IActionResult GetLang()
        {
            List<lang> list = ClassCrud.GetLang();
            return Json(list);
        }

        [Route("/vehicle/wmi")]
        public IActionResult GetWmi()
        {
            List<string> list = ClassCrud.GetWmi();
            return Json(list);
        }
        [HttpPost]    //   6.1
        [Route("/vehicle/{vehicle_id:required}/sgroups")]
        public IActionResult GetSgroups(string vehicle_id, string group_id, string[] codes, string[] node_ids)
        {
            #region lang
            string lang = "EN";
            if (!String.IsNullOrEmpty(Request.Headers["lang"].ToString()))
            {
                lang = Request.Headers["lang"].ToString();
            }
            #endregion

            if (!String.IsNullOrEmpty(group_id))
            {
                List<Sgroups> list = ClassCrud.GetSgroups(vehicle_id, group_id, lang);
                return Json(list);
            }
            else if ((codes != null && codes.Length > 0) || (node_ids != null && node_ids.Length > 0))
            {
                List<node> list = ClassCrud.GetNodes(codes, node_ids);
                return Json(list);
            }

            return NotFound("Проверте параметры!");
        }

        [Route("/filters")]   //  [FromQuery] and [FromRoute]
        public IActionResult GetFilters(string model_id, [FromQuery(Name = "params[]")] string[] param)
        {
            List<Filters> list = ClassCrud.GetFilters(model_id, param);
            return Json(list);
        }
    }
}
