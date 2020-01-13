using GestorDocumentosBusiness;
using GestorDocumentosExceptions;
using System;
using System.Web.Mvc;

namespace GestorDocumentos.Controllers
{
    public class LogActividadController : Controller
    {
        // GET: LogActividad
        public ActionResult Index()
        {
            ViewBag.list = LogBO.getListLog("");
            ViewBag.Fecha = (DateTime.Now.ToString("dd/MM/yyyy")).Replace("-", "/");
            return View();
        }

        [HttpPost]
        public ActionResult GetLista(string Fecha)
        {
            string[] f = Fecha.Split('/');
            string _f = f[2] + "-" + f[1] + "-" + f[0];
            ViewBag.list = LogBO.getListLog(_f);
            ViewBag.Fecha = Fecha;
            return View("Index");
        }


    }
}