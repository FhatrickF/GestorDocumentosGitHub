using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GestorDocumentos.Controllers
{
    public class ImagenesDoeController : Controller
    {
        // GET: ImagenesDoe
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Image(string imagen)
        {
            var dir = Server.MapPath(@"D:\INFO\DATA\_img");
            var path = Path.Combine(dir, imagen + ".jpg");
            return base.File(path, "image/jpeg");
        }
    }
}