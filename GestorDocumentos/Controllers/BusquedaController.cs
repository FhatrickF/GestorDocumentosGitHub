using GestorDocumentosExceptions;
using mvc4.Business;
using mvc4.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;


namespace GestorDocumentos.Controllers
{
    public class BusquedaController : Controller
    {
        private string directorio_ma = WebConfigurationManager.AppSettings["MVC-DATA-MA"];

        // GET: Busqueda
        public ActionResult Index(string id)
        {
            Documento d = new Documento();
            try
            {
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

        [HttpPost]
        public string Index(FormularioBusqueda form)
        {
            string ct = form.Ct;
            string lr = form.Lr;
            string lt = form.Lt;
            string lzf = form.Lzf;
            string liva = form.Liva;

            string circulares = form.Circular;
            string decretos = form.Decreto;
            string dfl = form.Dfl;
            string dl = form.Dl;
            string ds = form.Ds;
            string ley = form.Ley;
            string resolucion = form.Resolucion;
            string fecha = form.Fecha;
            string numero = form.Numero;
            string articulo = form.Articulo;
            string inciso = form.Inciso;
            string texto = form.Texto;
            string pagina = Convert.ToString(form.Pagina);
            string coleccion = form.Coleccion;

            bool ordenBy = false;

            string q = "";
            try
            {
                if (ct != null)
                {
                    q += "Norma:'CODIGO TRIBUTARIO'";
                    ordenBy = true;
                }
                if (lr != null)
                {
                    q += (q != "" && q != null) ? " OR Norma:'LEY DE LA RENTA'" : "Norma:'LEY DE LA RENTA'";
                    ordenBy = true;
                }
                if (lt != null)
                {
                    q += (q != "" && q != null) ? " OR Norma:'LEY DE TIMBRES Y ESTAMPILLAS'" : "Norma:'LEY DE TIMBRES Y ESTAMPILLAS'";
                    ordenBy = true;
                }
                if (lzf != null)
                {
                    q += (q != "" && q != null) ? " OR Norma:'LEY DE ZONA FRANCA'" : "Norma:'LEY DE ZONA FRANCA'";
                    ordenBy = true;
                }
                if (liva != null)
                {
                    q += (q != "" && q != null) ? " OR Norma:'LEY DEL IVA'" : "Norma:'LEY DEL IVA'";
                    ordenBy = true;
                }
                if (circulares != null)
                    q += (q != "" && q != null) ? " OR Norma:'CIRCULAR'" : "Norma:'CIRCULAR'";
                if (decretos != null)
                    q += (q != "" && q != null) ? " OR Norma:'DECRETO'" : "Norma:'DECRETO'";
                if (dfl != null)
                    q += (q != "" && q != null) ? " OR Norma:'DECRETO CON FUERZA DE LEY'" : "Norma:'DECRETO CON FUERZA DE LEY'";
                if (dl != null)
                    q += (q != "" && q != null) ? " OR Norma:'DECRETO LEY'" : "Norma:'DECRETO LEY'";
                if (ds != null)
                    q += (q != "" && q != null) ? " OR Norma:'DECRETO SUPREMO'" : "Norma:'DECRETO SUPREMO'";
                if (ley != null)
                    q += (q != "" && q != null) ? " OR Norma:'LEY'" : "Norma:'LEY'";
                if (resolucion != null)
                    q += (q != "" && q != null) ? " OR Norma:'RESOLUCION'" : "Norma:'RESOLUCION'";

                string bNorma = "";
                if (q != "")
                    bNorma = " AND (" + q + ")";

                q = "";
                if (fecha != null)
                {
                    fecha = fecha.Replace("/", "-");
                    string[] f = fecha.Split('-');
                    fecha = f[2] + "-" + f[1] + "-" + f[0] + @"T00:00:00Z";
                    q += " AND Fecha:'" + fecha + "'";
                }
                if (numero != null)
                    q += " AND Numero:'" + numero + "'";
                if (articulo != null)
                    q += " AND Articulo:'" + articulo + "'";
                if (inciso != null)
                    q += " AND Inciso:'" + inciso + "'";
                if (texto != null)
                    q += " AND Texto:'" + texto + "'";

                string bDatos = "";
                if (q != "")
                    bDatos = q;

                string fl = "Norma,Numero,Articulo,Inciso,Titulo,Fecha,IdDocumento,Organismo";

                string orden = "";
                if (ordenBy)
                    orden = "Orden";
                else
                    orden = "Fecha";

                string url = "select?fl=" + fl + "&q=Coleccion:'" + coleccion + "'" + bNorma + bDatos + "&sort=" + orden + " asc&start=" + pagina;
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
                new TechnicalException("Error al realizar la busqueda del documento.",ex);
                return "{\"MensajeError\":\"No es posible cargar la página, contactarse con el administrador\"}";
            }
        }
    }
}