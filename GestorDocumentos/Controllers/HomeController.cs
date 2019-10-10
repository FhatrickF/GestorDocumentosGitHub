using GestorDocumentosBusiness;
using GestorDocumentosEntities;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GestorDocumentos.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var loggin = User.Identity.IsAuthenticated;
            if (loggin)
            {
                usuarioEntity us = new usuarioEntity();
                us = usuarioBO.getUserbyName(User.Identity.Name);
                Session["rol"] = us.Rol.Trim();
                return View();
            }
            else
                return RedirectToAction("Login", "Account");
        }
    }
}