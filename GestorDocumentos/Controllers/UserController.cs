using GestorDocumentosBusiness;
using GestorDocumentosEntities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GestorDocumentos.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            var loggin = User.Identity.IsAuthenticated;
            if (loggin)
            {
                List<usuarioTabEntity> listUser = new List<usuarioTabEntity>();
                listUser = usuarioBO.getListUser();
                //listUser.ForEach(x => x.Id = "1");
                ViewBag.lista = listUser;
                return View();
            }
            else
                return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public JsonResult getList()
        {
            List<usuarioTabEntity> listUser = new List<usuarioTabEntity>();
            listUser = usuarioBO.getListUser();

            return Json(listUser, JsonRequestBehavior.AllowGet);
        }
    }
}