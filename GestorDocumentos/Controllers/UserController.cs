using GestorDocumentos.Models;
using GestorDocumentosBusiness;
using GestorDocumentosEntities;
using GestorDocumentosExceptions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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
                try
                {
                    List<usuarioTabEntity> listUser = new List<usuarioTabEntity>();
                    listUser = usuarioBO.getListUser();
                    //listUser.ForEach(x => x.Id = "1");
                    ViewBag.lista = listUser;
                    return View();
                }
                catch (BusinessException)
                {
                    return RedirectToAction("Index", "Home");
                }
               
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

        [HttpGet]
        public JsonResult Create(string Email, bool EmailConfirmed, string Rol, string UserName, string PasswordHash)
        {
            try
            {
                ApplicationDbContext context = new ApplicationDbContext();

                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                var users = new ApplicationUser
                {
                    UserName = Email,
                    Email = Email,
                    EmailConfirmed = EmailConfirmed,
                };

                var result = userManager.Create(users, PasswordHash);
                if(!result.Succeeded)
                    throw new BusinessException("No se pudo agregar el usuario.");
                
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                string rolentity = Sys_RolEntity.getRol(Rol);
                var resultrol = userManager.AddToRole(users.Id, rolentity);
                if (!resultrol.Succeeded)
                    throw new BusinessException("No se pudo actualizar el rol del usuario. por favor comunicarse con el administrador");

                usuarioBO.setRol(users.Id, Rol);

                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (BusinessException ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult getUser(string id)
        {
            try
            {
                usuarioEntity user = usuarioBO.getUserbyId(id);
                return Json(user, JsonRequestBehavior.AllowGet);
            }
            catch (BusinessException ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult eliminar(string id)
        {
            try
            {
                usuarioBO.deleteById(id);
                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (BusinessException ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult Update(string id, string Email, bool EmailConfirmed, string Rol, string UserName, string PasswordHash)
        {
            try
            {
                ApplicationDbContext context = new ApplicationDbContext();

                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

                var users = (ApplicationUser)userManager.FindById(id);

                //users.Id = id;
                users.UserName = Email;
                users.Email = Email;
                users.EmailConfirmed = EmailConfirmed;
                users.PasswordHash = userManager.PasswordHasher.HashPassword(PasswordHash);
                string rolentity = Sys_RolEntity.getRol(Rol);
                var resultrol = userManager.AddToRole(users.Id, rolentity);
                var result = userManager.Update(users);
                //string token = WebSecurity.GeneratePasswordResetToken(users.UserName);
                //string code = userManager.GeneratePasswordResetToken(users.Id);
                //result = userManager.ResetPassword(users.Id,userManager.GeneratePasswordResetToken(users.Id),PasswordHash);
                if (!result.Succeeded)
                    throw new BusinessException("No se pudo actualizar el usuario.");
                if (!resultrol.Succeeded)
                    throw new BusinessException("No se pudo actualizar el rol del usuario.");

                usuarioBO.setRol(users.Id, Rol);

                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch (BusinessException ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
    }
}