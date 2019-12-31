using GestorDocumentos.Models;
using GestorDocumentosBusiness;
using GestorDocumentosEntities;
using GestorDocumentosExceptions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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
                try
                {
                    ApplicationDbContext context = new ApplicationDbContext();

                    var userStore = new UserStore<ApplicationUser>(context);
                    var userManager = new UserManager<ApplicationUser>(userStore);

                    var uss = userManager.FindByName(User.Identity.Name);

                    Session["rol"] = uss.Roles.FirstOrDefault().RoleId;
                    return View();
                }
                catch (Exception ex)
                {
                    new TechnicalException("Error al recibir usuario logeado.", ex);
                    return RedirectToAction("Login", "Account");
                }
            }
            else
                return RedirectToAction("Login", "Account");
        }
    }
}