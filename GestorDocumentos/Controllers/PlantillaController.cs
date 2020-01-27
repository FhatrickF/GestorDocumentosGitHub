using GestorDocumentos.Models.Plantillas;
using GestorDocumentosExceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GestorDocumentos.Controllers
{
    public class PlantillaController : Controller
    {
        // GET: Plantilla
        public ActionResult Index()
        {
            var login = User.Identity.IsAuthenticated;
            if (!login)
                return RedirectToAction("Login", "Account");

            return View();
        }
        public ActionResult BuscarNG()
        {
            var login = User.Identity.IsAuthenticated;
            if (!login)
                return RedirectToAction("Login", "Account");

            ViewBag.Referencia = false;

            return View();
        }
        [HttpPost]
        public string BuscarNG(NormasGenerales nor)
        {
            try
            {
                var login = User.Identity.IsAuthenticated;
                if (!login)
                    return "{\"MensajeError\":\"Usuario no logeado\"}";

                string q = string.Empty;
                string bNorma = string.Empty;
                string bDatos = string.Empty;
                string fecha = string.Empty;
                string coleccion = "&q=Coleccion:'DONG'";
                string fl = "Norma,Numero,Articulo,Inciso,Titulo,Fecha,IdDocumento,Organismo,Estado,Partes,Tribunal,Propiedad";

                if(!String.IsNullOrEmpty(nor.LY))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'LEY'" : "Norma:'LEY'";
                if (!String.IsNullOrEmpty(nor.RES))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'RESOLUCION'" : "Norma:'RESOLUCION'";
                if (!String.IsNullOrEmpty(nor.CI))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'CIRCULAR'" : "Norma:'CIRCULAR'";
                if (!String.IsNullOrEmpty(nor.DF))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'DECRETO CON FUERZA DE LEY'" : "Norma:'DECRETO CON FUERZA DE LEY'";

                if(!String.IsNullOrEmpty(q))
                    bNorma = " AND (" + q + ")";

                q = string.Empty;

                if (nor.FechaD != null)
                {
                    fecha = nor.FechaD.Replace("/", "-");
                    string[] f = fecha.Split('-');
                    fecha = f[2] + "-" + f[1] + "-" + f[0] + @"T00:00:00Z";
                    q += " AND Fecha:'" + fecha + "'";
                }

                if (nor.num != null)
                    q += " AND Numero:'" + nor.num + "'";

                if (!string.IsNullOrEmpty(nor.todas))
                    q += " AND Texto:'" + nor.todas + "'";


                string url = "select?fl=" + fl + coleccion + bNorma + bDatos + "&q=Estado:'98'&sort=Fecha asc &start=" + nor.Pagina;
                url = url.Replace("  ", " ");

                string response = Indexador.Solr.getResponse(url);

                var expConverter = new ExpandoObjectConverter();
                dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(response, expConverter);

                var doc = obj.response;

                return JsonConvert.SerializeObject(doc);
            }
            catch (BusinessException bx)
            {
                return "{\"MensajeError\":\"" + bx.Message + "\"}";
            }
            catch (Exception ex)
            {
                new TechnicalException("Error al realizar la busqueda del documento.", ex);
                return "{\"MensajeError\":\"No es posible cargar la página, contactarse con el administrador\"}";
            }

        }
        public ActionResult BuscarNP()
        {
            var login = User.Identity.IsAuthenticated;
            if (!login)
                return RedirectToAction("Login", "Account");

            ViewBag.Referencia = false;

            return View();
        }

        [HttpPost]
        public string BuscarNP(NormasPerticulares nor)
        {
            try
            {
                var login = User.Identity.IsAuthenticated;
                if (!login)
                    return "{\"MensajeError\":\"Usuario no logeado\"}";

                string q = string.Empty;
                string bNorma = string.Empty;
                string bDatos = string.Empty;
                string fecha = string.Empty;
                string coleccion = "&q=Coleccion:'DONP'";
                string fl = "Norma,Numero,Articulo,Inciso,Titulo,Fecha,IdDocumento,Organismo,Estado,Partes,Tribunal,Propiedad";

                if (!String.IsNullOrEmpty(nor.dc))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'DECRETO'" : "Norma:'DECRETO'";
                if (!String.IsNullOrEmpty(nor.res))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'RESOLUCION'" : "Norma:'RESOLUCION'";



                if (!String.IsNullOrEmpty(q))
                    bNorma = " AND (" + q + ")";

                q = string.Empty;

                if (nor.FechaD != null)
                {
                    fecha = nor.FechaD.Replace("/", "-");
                    string[] f = fecha.Split('-');
                    fecha = f[2] + "-" + f[1] + "-" + f[0] + @"T00:00:00Z";
                    q += " AND Fecha:'" + fecha + "'";
                }

                if (!string.IsNullOrEmpty(nor.n))
                    q += " AND Numero:'" + nor.n + "'";

                if (!string.IsNullOrEmpty(nor.Todas))
                    q += " AND Texto:'" + nor.Todas + "'";


                string url = "select?fl=" + fl + coleccion + bNorma + bDatos + "&q=Estado:'98'&sort=Fecha asc &start=" + nor.Pagina;
                url = url.Replace("  ", " ");

                string response = Indexador.Solr.getResponse(url);

                var expConverter = new ExpandoObjectConverter();
                dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(response, expConverter);

                var doc = obj.response;

                return JsonConvert.SerializeObject(doc);
            }
            catch (BusinessException bx)
            {
                return "{\"MensajeError\":\"" + bx.Message + "\"}";
            }
            catch (Exception ex)
            {
                new TechnicalException("Error al realizar la busqueda del documento.", ex);
                return "{\"MensajeError\":\"No es posible cargar la página, contactarse con el administrador\"}";
            }
        }

        public ActionResult BuscarPJ()
        {
            var login = User.Identity.IsAuthenticated;
            if (!login)
                return RedirectToAction("Login", "Account");

            ViewBag.Referencia = false;

            return View();
        }

        [HttpPost]
        public string BuscarPJ(PublicacionesJudiciales nor)
        {
            try
            {
                var login = User.Identity.IsAuthenticated;
                if (!login)
                    return "{\"MensajeError\":\"Usuario no logeado\"}";

                string q = string.Empty;
                string bNorma = string.Empty;
                string bDatos = string.Empty;
                string fecha = string.Empty;
                string coleccion = "&q=Coleccion:'DOPJ'";
                string fl = "Norma,Numero,Articulo,Inciso,Titulo,Fecha,IdDocumento,Organismo,Estado,Partes,Tribunal,Propiedad";

                if (!String.IsNullOrEmpty(nor.qu))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'QUIEBRAS'" : "Norma:'QUIEBRAS'";
                if (!String.IsNullOrEmpty(nor.cm))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'CAMBIO DE NOMBRE'" : "Norma:'CAMBIO DE NOMBRE'";
                if (!String.IsNullOrEmpty(nor.mp))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'MUERTE PRESUNTA'" : "Norma:'MUERTE PRESUNTA'";
                if (!String.IsNullOrEmpty(nor.n))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'NOTIFICACION'" : "Norma:'NOTIFICACION'";
                if (!String.IsNullOrEmpty(nor.ed))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'EXTRAVIOS DE DOCUMENTOS'" : "Norma:'EXTRAVIOS DE DOCUMENTOS'";
                if (!String.IsNullOrEmpty(nor.n))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'RECONSTITUCION DE DOMICILIO'" : "Norma:'RECONSTITUCION DE DOMICILIO'";



                if (!String.IsNullOrEmpty(q))
                    bNorma = " AND (" + q + ")";

                q = string.Empty;

                if (nor.FechaD != null)
                {
                    fecha = nor.FechaD.Replace("/", "-");
                    string[] f = fecha.Split('-');
                    fecha = f[2] + "-" + f[1] + "-" + f[0] + @"T00:00:00Z";
                    q += " AND Fecha:'" + fecha + "'";
                }

                if (!string.IsNullOrEmpty(nor.Todas))
                    q += " AND Texto:'" + nor.Todas + "'";


                string url = "select?fl=" + fl + coleccion + bNorma + bDatos + "&q=Estado:'98'&sort=Fecha asc &start=" + nor.pagina;
                url = url.Replace("  ", " ");

                string response = Indexador.Solr.getResponse(url);

                var expConverter = new ExpandoObjectConverter();
                dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(response, expConverter);

                var doc = obj.response;

                return JsonConvert.SerializeObject(doc);
            }
            catch (BusinessException bx)
            {
                return "{\"MensajeError\":\"" + bx.Message + "\"}";
            }
            catch (Exception ex)
            {
                new TechnicalException("Error al realizar la busqueda del documento.", ex);
                return "{\"MensajeError\":\"No es posible cargar la página, contactarse con el administrador\"}";
            }
        }

        public ActionResult BuscarA()
        {
            var login = User.Identity.IsAuthenticated;
            if (!login)
                return RedirectToAction("Login", "Account");

            ViewBag.Referencia = false;

            return View();
        }

        [HttpPost]
        public string BuscarA(Avisos nor)
        {
            try
            {
                var login = User.Identity.IsAuthenticated;
                if (!login)
                    return "{\"MensajeError\":\"Usuario no logeado\"}";

                string q = string.Empty;
                string bNorma = string.Empty;
                string bDatos = string.Empty;
                string fecha = string.Empty;
                string coleccion = "&q=Coleccion:'DOAV'";
                string fl = "Norma,Numero,Articulo,Inciso,Titulo,Fecha,IdDocumento,Organismo,Estado,Partes,Tribunal,Propiedad";

                if (!String.IsNullOrEmpty(nor.b))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'BALANCES'" : "Norma:'BALANCES'";
                if (!String.IsNullOrEmpty(nor.ca))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'CITACION A ACCIONISTAS'" : "Norma:'CITACION A ACCIONISTAS'";
                if (!String.IsNullOrEmpty(nor.cp))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'CONCURSOS PUBLICOS'" : "Norma:'CONCURSOS PUBLICOS'";
                if (!String.IsNullOrEmpty(nor.l))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'LICITACION'" : "Norma:'LICITACION'";
                if (!String.IsNullOrEmpty(nor.oa))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'OTROS AVISOS'" : "Norma:'OTROS AVISOS'";
                if (!String.IsNullOrEmpty(nor.p))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'PROPUESTA'" : "Norma:'PROPUESTA'";

                if (!String.IsNullOrEmpty(q))
                    bNorma = " AND (" + q + ")";

                q = string.Empty;

                if (nor.FechaD != null)
                {
                    fecha = nor.FechaD.Replace("/", "-");
                    string[] f = fecha.Split('-');
                    fecha = f[2] + "-" + f[1] + "-" + f[0] + @"T00:00:00Z";
                    q += " AND Fecha:'" + fecha + "'";
                }



                if (!string.IsNullOrEmpty(nor.Todas))
                    q += " AND Texto:'" + nor.Todas + "'";


                string url = "select?fl=" + fl + coleccion + bNorma + bDatos + "&q=Estado:'98'&sort=Fecha asc &start=" + nor.pagina;
                url = url.Replace("  ", " ");

                string response = Indexador.Solr.getResponse(url);

                var expConverter = new ExpandoObjectConverter();
                dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(response, expConverter);

                var doc = obj.response;

                return JsonConvert.SerializeObject(doc);
            }
            catch (BusinessException bx)
            {
                return "{\"MensajeError\":\"" + bx.Message + "\"}";
            }
            catch (Exception ex)
            {
                new TechnicalException("Error al realizar la busqueda del documento.", ex);
                return "{\"MensajeError\":\"No es posible cargar la página, contactarse con el administrador\"}";
            }
        }

        public ActionResult BuscarLA()
        {
            var login = User.Identity.IsAuthenticated;
            if (!login)
                return RedirectToAction("Login", "Account");

            ViewBag.Referencia = false;

            return View();
        }

        [HttpPost]
        public string BuscarLA(LegislacionActualizada nor)
        {
            try
            {
                var login = User.Identity.IsAuthenticated;
                if (!login)
                    return "{\"MensajeError\":\"Usuario no logeado\"}";

                string q = string.Empty;
                string bNorma = string.Empty;
                string bDatos = string.Empty;
                string fecha = string.Empty;
                string coleccion = "&q=Coleccion:'LA'";
                string fl = "Norma,Numero,Articulo,Inciso,Titulo,Fecha,IdDocumento,Organismo,Estado,Partes,Tribunal,Propiedad";

                if (!String.IsNullOrEmpty(nor.dl))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'DECRETO LEY'" : "Norma:'DECRETO LEY'";
                if (!String.IsNullOrEmpty(nor.dfl))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'DECRETO CON FUERZA DE LEY'" : "Norma:'DECRETO CON FUERZA DE LEY'";
                if (!String.IsNullOrEmpty(nor.dec))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'DECRETOS'" : "Norma:'DECRETOS'";
                if (!String.IsNullOrEmpty(nor.rg))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'REGLAMENTO'" : "Norma:'REGLAMENTO'";
                if (!String.IsNullOrEmpty(nor.res))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'RESOLUCION'" : "Norma:'RESOLUCION'";
                if (!String.IsNullOrEmpty(nor.ti))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'TRATADOS INTERNACIONALES'" : "Norma:'TRATADOS INTERNACIONALES'";
                if (!String.IsNullOrEmpty(nor.ly))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'LEY'" : "Norma:'LEY'";
                if (!String.IsNullOrEmpty(nor.aac))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'AUTO ACUERDO'" : "Norma:'AUTO ACUERDO'";
                if (!String.IsNullOrEmpty(nor.ac))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'ACUERDOS'" : "Norma:'ACUERDOS'";
                if (!String.IsNullOrEmpty(nor.ci))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'CIRCULAR'" : "Norma:'CIRCULAR'";



                if (!String.IsNullOrEmpty(q))
                    bNorma = " AND (" + q + ")";

                q = string.Empty;

                if (nor.FechaD != null)
                {
                    fecha = nor.FechaD.Replace("/", "-");
                    string[] f = fecha.Split('-');
                    fecha = f[2] + "-" + f[1] + "-" + f[0] + @"T00:00:00Z";
                    q += " AND Fecha:'" + fecha + "'";
                }

                if (!string.IsNullOrEmpty(nor.norma))
                    q += " AND Norma:'" + nor.norma + "'";
                if (!string.IsNullOrEmpty(nor.n))
                    q += " AND Numero:'" + nor.n + "'";
                if (!string.IsNullOrEmpty(nor.articulo))
                    q += " AND Articulo:'" + nor.articulo + "'";
                if (!string.IsNullOrEmpty(nor.todas))
                    q += " AND Texto:'" + nor.todas + "'";


                string url = "select?fl=" + fl + coleccion + bNorma + bDatos + "&q=Estado:'98'&sort=Fecha asc &start=" + nor.pagina;
                url = url.Replace("  ", " ");

                string response = Indexador.Solr.getResponse(url);

                var expConverter = new ExpandoObjectConverter();
                dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(response, expConverter);

                var doc = obj.response;

                return JsonConvert.SerializeObject(doc);
            }
            catch (BusinessException bx)
            {
                return "{\"MensajeError\":\"" + bx.Message + "\"}";
            }
            catch (Exception ex)
            {
                new TechnicalException("Error al realizar la busqueda del documento.", ex);
                return "{\"MensajeError\":\"No es posible cargar la página, contactarse con el administrador\"}";
            }
        }

        public ActionResult BuscarNM()
        {
            var login = User.Identity.IsAuthenticated;
            if (!login)
                return RedirectToAction("Login", "Account");

            ViewBag.Referencia = false;

            return View();
        }
    }
}