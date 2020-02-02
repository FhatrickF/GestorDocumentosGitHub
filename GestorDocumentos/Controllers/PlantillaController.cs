using GestorDocumentos.Models.Plantillas;
using GestorDocumentosExceptions;
using mvc4.Models;
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
        public ActionResult Index(string id)
        {
            Session["estado"] = 0;
            Documento d = new Documento();
            try
            {
                var login = User.Identity.IsAuthenticated;
                if (!login)
                    return RedirectToAction("Login", "Account");

                System.Web.HttpContext.Current.Session["id-doc-referencia"] = null;

                if (id != null)
                {
                    System.Web.HttpContext.Current.Session["id-doc-referencia"] = id;

                    d = Indexador.Solr.getDocumentoById(id, false);

                    ViewBag.Referencia = true;
                }
                else
                {
                    ViewBag.Referencia = false;
                }
                return View(d);
            }
            catch (BusinessException bx)
            {
                return RedirectToAction("Index", "Home");
            }
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
                string fecha2 = string.Empty;
                string coleccion = "&q=Coleccion:'DONG'";
                string fl = "Norma,Numero,Articulo,Inciso,Titulo,Fecha,IdDocumento,Organismo,Estado,Partes,Tribunal,Propiedad";

                if(!String.IsNullOrEmpty(nor.LY))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'LEY'" : "Norma:'LEY'";
                if (!String.IsNullOrEmpty(nor.RES))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'RESOLUCION'" : "Norma:'RESOLUCION'";
                if (!String.IsNullOrEmpty(nor.CI))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'CIRCULAR'" : "Norma:'CIRCULAR'";
                if (!String.IsNullOrEmpty(nor.DF))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'DFL'" : "Norma:'DFL'";
                if (!String.IsNullOrEmpty(nor.PJ))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'NOMINA'" : "Norma:'NOMINA'";
                if (!String.IsNullOrEmpty(nor.AA))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'AAC'" : "Norma:'AAC'";
                if (!String.IsNullOrEmpty(nor.ACBC))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'ACUERDO&CERTIFICADO'" : "Norma:'ACUERDO&CERTIFICADO'";
                if (!String.IsNullOrEmpty(nor.DO))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'DCTO&ORDENANZA'" : "Norma:'DCTO&ORDENANZA'";
                if (!String.IsNullOrEmpty(nor.IG))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'INFORMACION&COREDE&INSTRUCTIVO&NORMA&ORDEN&SENTENCIA&RECTIFICACION&REGLAMENTO'" : "Norma:'INFORMACION&COREDE&INSTRUCTIVO&NORMA&ORDEN&SENTENCIA&RECTIFICACION&REGLAMENTO'";

                if (!String.IsNullOrEmpty(q))
                    bNorma = " AND (" + q + ")";

                q = string.Empty;

                if (nor.FechaD != null)
                {
                    fecha = nor.FechaD.Replace("/", "-");
                    string[] f = fecha.Split('-');
                    fecha = f[2] + "-" + f[1] + "-" + f[0] + @"T00:00:00Z";
                    fecha2 = nor.FechaH.Replace("/", "-");
                    f = fecha2.Split('-');
                    fecha2 = f[2] + "-" + f[1] + "-" + f[0] + @"T00:00:00Z";
                    q += " AND Fecha:'" + fecha + " A " + fecha2 + "'";
                }

                if (nor.num != null)
                    q += " AND Numero:'" + nor.num + "'";

                if (!string.IsNullOrEmpty(nor.todas))
                {
                    if (!string.IsNullOrEmpty(nor.ninguna))
                        q += " AND Texto:'" + nor.todas + " NOT " + nor.ninguna + "'";
                    else
                        q += " AND Texto:'" + nor.todas + "'";
                }

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
                string fecha2 = string.Empty;
                string coleccion = "&q=Coleccion:'DONP'";
                string fl = "Norma,Numero,Articulo,Inciso,Titulo,Fecha,IdDocumento,Organismo,Estado,Partes,Tribunal,Propiedad";

                if (!String.IsNullOrEmpty(nor.dc))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'DECRETO'" : "Norma:'DECRETO'";
                if (!String.IsNullOrEmpty(nor.soc))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'SOCIEDAD*'" : "Norma:'SOCIEDAD*'";
                if (!String.IsNullOrEmpty(nor.pre))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'PRENDA'" : "Norma:'PRENDA'";
                if (!String.IsNullOrEmpty(nor.da))
                {
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'SOLICITUD EXPLORACION' OR Norma:'SOLICITUD INSCRIPCION' OR Norma:'SOLICITUD REGULARIZACION'" : "Norma:'RESOLUCION' OR Norma:'SOLICITUD INSCRIPCION' OR Norma:'SOLICITUD REGULARIZACION'";
                }
                if (!String.IsNullOrEmpty(nor.dc))
                {
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'PROTOCOLIZACION' OR Norma:'INFORMACION SORTEO' OR Norma:'CERTIFICACION' OR Norma:'CERTIFICADO' OR Norma:'LISTA DCTO' OR Norma:'IMPACTO AMBIENTAL' OR Norma:'PLANTA RESIDUOS' OR Norma:'ACUERDO' OR Norma:'REGISTRO VARIEDAD' OR Norma:'LISTA NOMBRAMIENTO' OR Norma:'PARTIDO'" : "Norma:'PROTOCOLIZACION' OR Norma:'INFORMACION SORTEO' OR Norma:'CERTIFICACION' OR Norma:'CERTIFICADO' OR Norma:'LISTA DCTO' OR Norma:'IMPACTO AMBIENTAL' OR Norma:'PLANTA RESIDUOS' OR Norma:'ACUERDO' OR Norma:'REGISTRO VARIEDAD' OR Norma:'LISTA NOMBRAMIENTO' OR Norma:'PARTIDO'";
                }
                if (!String.IsNullOrEmpty(nor.res))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'RESOLUCION'" : "Norma:'RESOLUCION'";
                if (!String.IsNullOrEmpty(nor.eirl))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'Empresa Individual*'" : "Norma:'Empresa Individual*'";
                if (!String.IsNullOrEmpty(nor.mc))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'MARCA'" : "Norma:'MARCA'";
                if (!String.IsNullOrEmpty(nor.spo))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'SOLICITUD TELECOMUNICACIONE' OR Norma:'OTRAS SOLICITUDES' OR Norma:'SOLICITUD TRANSPORTES' OR Norma:'SOLICITUD CONCESION' OR Norma:'SOLICITUD CONCESION TELEVISIVA' OR Norma:'SOLICITUD ELECTRICIDAD'" : "Norma:'SOLICITUD TELECOMUNICACIONE' OR Norma:'OTRAS SOLICITUDES' OR Norma:'SOLICITUD TRANSPORTES' OR Norma:'SOLICITUD CONCESION' OR Norma:'SOLICITUD CONCESION TELEVISIVA' OR Norma:'SOLICITUD ELECTRICIDAD'";
                if (!String.IsNullOrEmpty(nor.ag))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'ASOCIACION GREMIAL'" : "Norma:'ASOCIACION GREMIAL'";
                if (!String.IsNullOrEmpty(nor.pmd))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'PATENTE' OR Norma:'MODELO' OR Norma:'DISEÑO'" : "Norma:'DECRETO' OR Norma:'MODELO' OR Norma:'DISEÑO'";
                if (!String.IsNullOrEmpty(nor.pm))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'ART 83' OR Norma:'PEDIMENTOS MINEROS' OR Norma:'MANIFESTACIONES MINERAS' OR Norma:'SOLICITUDES DE MENSURA' OR Norma:'SENTENCIA EXPLORACION' OR Norma:'SENTENCIA EXPLOTACION' OR Norma:'VIGENCIA MENSURA' OR Norma:'RENUNCIA CONCESION' OR Norma:'PRORROGA EXPLORACION' OR Norma:'NOMINA REMATE' OR Norma:'ACUERDO JUNTA' OR Norma:'CITACION JUNTA' OR Norma:'NOMINA PATENTE' OR Norma:'ACUERDO CONCESION'" : "Norma:'ART 83' OR Norma:'PEDIMENTOS MINEROS' OR Norma:'MANIFESTACIONES MINERAS' OR Norma:'SOLICITUDES DE MENSURA' OR Norma:'SENTENCIA EXPLORACION' OR Norma:'SENTENCIA EXPLOTACION' OR Norma:'VIGENCIA MENSURA' OR Norma:'RENUNCIA CONCESION' OR Norma:'PRORROGA EXPLORACION' OR Norma:'NOMINA REMATE' OR Norma:'ACUERDO JUNTA' OR Norma:'CITACION JUNTA' OR Norma:'NOMINA PATENTE' OR Norma:'ACUERDO CONCESION'";

                if (!String.IsNullOrEmpty(q))
                    bNorma = " AND (" + q + ")";

                q = string.Empty;

                if (nor.FechaD != null)
                {
                    fecha = nor.FechaD.Replace("/", "-");
                    string[] f = fecha.Split('-');
                    fecha = f[2] + "-" + f[1] + "-" + f[0] + @"T00:00:00Z";
                    fecha2 = nor.FechaH.Replace("/", "-");
                    f = fecha2.Split('-');
                    fecha2 = f[2] + "-" + f[1] + "-" + f[0] + @"T00:00:00Z";
                    q += " AND Fecha:'" + fecha + " A " + fecha2 + "'";
                }

                if (!string.IsNullOrEmpty(nor.n))
                    q += " AND Numero:'" + nor.n + "'";

                if (!string.IsNullOrEmpty(nor.Todas))
                {
                    if (!string.IsNullOrEmpty(nor.ninguna))
                        q += " AND Texto:'" + nor.Todas + " NOT " + nor.ninguna + "'";
                    else
                        q += " AND Texto:'" + nor.Todas + "'";
                }
                
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
                string fecha2 = string.Empty;
                string coleccion = "&q=Coleccion:'DOPJ'";
                string fl = "Norma,Numero,Articulo,Inciso,Titulo,Fecha,IdDocumento,Organismo,Estado,Partes,Tribunal,Propiedad";

                if (!String.IsNullOrEmpty(nor.qu))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'QUIEBRA'" : "Norma:'QUIEBRA'";
                if (!String.IsNullOrEmpty(nor.cm))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'CAMBIO NOMBRE'" : "Norma:'CAMBIO NOMBRE'";
                if (!String.IsNullOrEmpty(nor.mp))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'MUERTE PRESUNTA'" : "Norma:'MUERTE PRESUNTA'";
                if (!String.IsNullOrEmpty(nor.n))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'NOTIFICACION'" : "Norma:'NOTIFICACION'";
                if (!String.IsNullOrEmpty(nor.ed))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'EXTRAVIOS DE DOCUMENTOS'" : "Norma:'EXTRAVIOS DE DOCUMENTOS'";
                if (!String.IsNullOrEmpty(nor.rd))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'DOMINIO'" : "Norma:'DOMINIO'";



                if (!String.IsNullOrEmpty(q))
                    bNorma = " AND (" + q + ")";

                q = string.Empty;

                if (nor.FechaD != null)
                {
                    fecha = nor.FechaD.Replace("/", "-");
                    string[] f = fecha.Split('-');
                    fecha = f[2] + "-" + f[1] + "-" + f[0] + @"T00:00:00Z";
                    fecha2 = nor.FechaH.Replace("/", "-");
                    f = fecha2.Split('-');
                    fecha2 = f[2] + "-" + f[1] + "-" + f[0] + @"T00:00:00Z";
                    q += " AND Fecha:'" + fecha + " A " + fecha2 + "'";
                }

                if (!string.IsNullOrEmpty(nor.Todas))
                {
                    if (!string.IsNullOrEmpty(nor.ninguna))
                        q += " AND Texto:'" + nor.Todas + " NOT " + nor.ninguna + "'";
                    else
                        q += " AND Texto:'" + nor.Todas + "'";
                }

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
                string fecha2 = string.Empty;
                string coleccion = "&q=Coleccion:'DOAV'";
                string fl = "";

                if (!String.IsNullOrEmpty(nor.b))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'BALANCE'" : "Norma:'BALANCE'";
                if (!String.IsNullOrEmpty(nor.ca))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'CITACION' OR Norma:'REPARTO'" : "Norma:'CITACION' OR Norma:'REPARTO'";
                if (!String.IsNullOrEmpty(nor.cp))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'CONCURSO'" : "Norma:'CONCURSO'";
                if (!String.IsNullOrEmpty(nor.l))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'LICITACION'" : "Norma:'LICITACION'";
                if (!String.IsNullOrEmpty(nor.oa))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'SUBASTA' OR Norma:'REPARTO' OR Norma:'PAGO' OR Norma:'POSTULANTES' OR Norma:'INFORMACION' OR Norma:'SORTEO' OR Norma:'RIESGO' OR Norma:'EMISION' OR Norma:'ADJUDICACION'" : "Norma:'SUBASTA' OR Norma:'REPARTO' OR Norma:'PAGO' OR Norma:'POSTULANTES' OR Norma:'INFORMACION' OR Norma:'SORTEO' OR Norma:'RIESGO' OR Norma:'EMISION' OR Norma:'ADJUDICACION'";
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
                    fecha2 = nor.FechaH.Replace("/", "-");
                    f = fecha2.Split('-');
                    fecha2 = f[2] + "-" + f[1] + "-" + f[0] + @"T00:00:00Z";
                    q += " AND Fecha:'" + fecha + " A " + fecha2 + "'";
                }

                if (!string.IsNullOrEmpty(nor.Todas))
                {
                    if(!string.IsNullOrEmpty(nor.ninguna))
                        q += " AND Texto:'" + nor.Todas + " NOT " + nor.ninguna + "'";
                    else
                        q += " AND Texto:'" + nor.Todas + "'";
                }

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
                string fecha2 = string.Empty;
                string coleccion = "&q=Coleccion:'LA'";
                string fl = "Norma,Numero,Articulo,Inciso,Titulo,Fecha,IdDocumento,Organismo,Estado,Partes,Tribunal,Propiedad";

                if (!String.IsNullOrEmpty(nor.dl))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'DL'" : "Norma:'DL'";
                if (!String.IsNullOrEmpty(nor.dfl))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'DFL'" : "Norma:'DFL'";
                if (!String.IsNullOrEmpty(nor.dec))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'DECRETO'" : "Norma:'DECRETO'";
                if (!String.IsNullOrEmpty(nor.rg))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'REGLAMENTO*'" : "Norma:'REGLAMENTO*'";
                if (!String.IsNullOrEmpty(nor.res))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'RESOLUCION'" : "Norma:'RESOLUCION'";
                if (!String.IsNullOrEmpty(nor.ti))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'convencion*'" : "Norma:'convencion*'";
                if (!String.IsNullOrEmpty(nor.ly))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'LEY'" : "Norma:'LEY'";
                if (!String.IsNullOrEmpty(nor.aac))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'aac'" : "Norma:'aac'";
                if (!String.IsNullOrEmpty(nor.ac))
                    q += (!String.IsNullOrEmpty(q)) ? " OR Norma:'ACUERDO'" : "Norma:'ACUERDO'";
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
                    fecha2 = nor.FechaH.Replace("/", "-");
                    f = fecha2.Split('-');
                    fecha2 = f[2] + "-" + f[1] + "-" + f[0] + @"T00:00:00Z";
                    q += " AND Fecha:'" + fecha + " A " + fecha2 + "'";
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