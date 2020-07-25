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

        [Route("/vehicle/vin")]
        public IActionResult GetListCarTypeInfo(string vin, int page = 1, [FromQuery(Name = "per-page")] int qty = 10)
        {
            List<CarTypeInfo> list = ClassCrud.GetListCarTypeInfo(vin);
            List<header> headerList = ClassCrud.GetHeaders();

            List<CarTypeInfo> items = list.Skip((page - 1) * qty).Take(qty).ToList();

            int o = 0;

            var result = new
            {
                header = headerList,
                items,
                cnt_items = list.Count,
                page 
            };

            return Json(result);
        }

        [Route("/image/{image_id}")]
        public async Task<FileContentResult> GetImageAsync(string image_id)
        {
            string imgPath = Ut.GetImagePath();   //   17SA501_HMT0300.png
            string npl = image_id.Substring(0, image_id.IndexOf("_"));
            string imageName = image_id.Substring(image_id.IndexOf("_")+1, image_id.Length - (image_id.IndexOf("_") + 1)).Replace("-", ".");
            //     \honda_eu\Img\Pictures\18SA801\IMGE
            byte[] result = new byte[0];
            string fullPath = Ut.GetImagePath() + npl + "/IMGE/" + imageName;

            try
            {
                if(fullPath.Substring(0, 4) == "http")
                {
                    using (var handler = new HttpClientHandler())
                    {
                        using (var client = new HttpClient(handler))
                        {
                            result = await client.GetByteArrayAsync(fullPath);
                        }
                    }
                }
                else
                {
                    if(System.IO.File.Exists(fullPath))
                    {
                        result = await System.IO.File.ReadAllBytesAsync(fullPath);
                    }
                }
            }
            catch (Exception ex)
            {
                string Error = ex.Message;
                int o = 0;
            }

            return File(result, "image/png");
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

        [HttpPost]    //   7.1
        [Route("/vehicle/{vehicle_id:required}/sgroups")]
        public IActionResult GetSgroups(string vehicle_id, string group_id, string[] codes, string[] node_id)
        {
            #region lang
            string lang = "EN";
            if (!String.IsNullOrEmpty(Request.Headers["lang"].ToString()))
            {
                lang = Request.Headers["lang"].ToString();
            }
            #endregion

            if (!String.IsNullOrEmpty(group_id))   //   7.1
            {
                List<Sgroups> list = ClassCrud.GetSgroups(vehicle_id, group_id, lang);
                return Json(list);
            }
            else if ((codes != null && codes.Length > 0) || (node_id != null && node_id.Length > 0))  //  7.2
            {
                List<node> list = ClassCrud.GetNodes(codes, node_id);
                return Json(list);
            }

            return NotFound("Проверте параметры!");
        }

        [Route("/filters")]   //  [FromQuery] and [FromRoute]
        public IActionResult GetFilters(string model_id, [FromQuery(Name = "params[0]")] string xcardrsP, [FromQuery(Name = "params[1]")] string dmodyrP,
            [FromQuery(Name = "params[2]")] string xgradefulnamP, [FromQuery(Name = "params[3]")] string ctrsmtypP, [FromQuery(Name = "params[4]")] string cmftrepcP,
            [FromQuery(Name = "params[5]")] string careaP, [FromQuery(Name = "params[6]")] string nengnpfP)
        {
            List<Filters> list = ClassCrud.GetFilters(model_id, xcardrsP, dmodyrP, xgradefulnamP, ctrsmtypP, cmftrepcP, careaP, nengnpfP);
            return Json(list);
        }

        [Route("/filter-cars")]
        public IActionResult GetListCarTypeInfoFilterCars(string model_id, [FromQuery(Name = "params[0]")] string xcardrsP, 
            [FromQuery(Name = "params[1]")] string dmodyrP, 
            [FromQuery(Name = "params[2]")] string xgradefulnamP, 
            [FromQuery(Name = "params[3]")] string ctrsmtypP, 
            [FromQuery(Name = "params[4]")] string cmftrepcP, 
            [FromQuery(Name = "params[5]")] string careaP,
            [FromQuery(Name = "params[6]")] string nengnpfP,
            int page = 1, int page_size = 10)
        {
            List<header> headerList = ClassCrud.GetHeaders();
            List<CarTypeInfo> list = ClassCrud.GetListCarTypeInfoFilterCars(model_id, xcardrsP, dmodyrP, xgradefulnamP, ctrsmtypP, cmftrepcP, careaP, nengnpfP);

            list = list.Skip((page - 1) * page_size).Take(page_size).ToList();

            var result = new
            {
                header = headerList,
                items = list,
                cnt_items = list.Count,
                page = page
            };
            return Json(result);
        }


        [Route("/vehicle/{vehicle_id:required}/mgroups")]
        public IActionResult GetPartsGroups(string vehicle_id)
        {
            string lang = "EN";
            if (!String.IsNullOrEmpty(Request.Headers["lang"].ToString()))
            {
                lang = Request.Headers["lang"].ToString();
            }

            try
            {
                List<PartsGroup> list = ClassCrud.GetPartsGroup(vehicle_id, lang);
                return Json(list);
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [Route("/vehicle/{vehicle_id:required}")]
        public IActionResult GetVehiclePropArr(string vehicle_id)
        {
            try
            {
                VehiclePropArr result = ClassCrud.GetVehiclePropArr(vehicle_id);
                return Json(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Route("/vehicle/{vehicle_id:required}/sgroups/{node_id:required}")]   //  
        public IActionResult GetSpareParts(string vehicle_id, string node_id  )
        {
            string lang = "EN";
            if (!String.IsNullOrEmpty(Request.Headers["lang"].ToString()))
            {
                lang = Request.Headers["lang"].ToString();
            }

            DetailsInNode detailsInNode = ClassCrud.GetDetailsInNode(vehicle_id, node_id, lang);
            return Json(detailsInNode);
        }
    }
}
