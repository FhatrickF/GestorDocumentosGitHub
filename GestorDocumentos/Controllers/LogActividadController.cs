using GestorDocumentosBusiness;
using GestorDocumentosExceptions;
using System.Web.Mvc;

namespace GestorDocumentos.Controllers
{
    public class LogActividadController : Controller
    {
        // GET: LogActividad
        public ActionResult Index()
        {
            ViewBag.list = LogBO.getListLog();
            return View();
        }

        //public JsonResult List()
        //{
        //    try
        //    {
        //        var list = LogBO.getListLog();
        //        return Json(list, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (BusinessException bx)
        //    {
        //        return Json(bx.Message, JsonRequestBehavior.DenyGet);
        //    }
        //}
    }
}