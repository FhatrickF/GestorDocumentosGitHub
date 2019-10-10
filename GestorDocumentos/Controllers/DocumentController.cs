using GestorDocumentosBusiness;
using GestorDocumentosEntities;
using mvc4.Business;
using mvc4.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace GestorDocumentos.Controllers
{
    public class DocumentController : Controller
    {
        private string directorio = WebConfigurationManager.AppSettings["MVC-DATA"];
        private string directorio_ma = WebConfigurationManager.AppSettings["MVC-DATA-MA"];
        private static string URL_SOLR = WebConfigurationManager.AppSettings["webSolr"] + @"/solr/test-1/update?commitWithin=1000&overwrite=true&wt=json";

        // GET: Document
        public ActionResult Index()
        {
            var loggin = User.Identity.IsAuthenticated;
            if (!loggin)
                return RedirectToAction("Login", "Account");

            return View();
        }

        public ActionResult Nuevo()
        {
            NuevoDocumentViewModel modelo = new NuevoDocumentViewModel();
            return View(modelo);
        }

        [HttpPost]
        public ActionResult Nuevo(NuevoDocumentViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    sgd_documentoEntity nuevo = new sgd_documentoEntity();

                    nuevo.Descripcion = model.Descripcion;
                    nuevo.Titulo = model.Titulo;
                    nuevo.Texto = model.Texto;
                    nuevo.FechaCreacion = DateTime.Now;
                    nuevo.VersionFinal = true;
                    if (!DocumentoBO.setDocumento(nuevo))
                        throw new Exception("No se puede guerdar el documento");

                    string xml = FileBo.SerializeXML(nuevo);
                    FileBo.setXmlStringToFile(directorio + nuevo.IdDocumento + ".xml", xml);
                    SolrBO.SolrAdd(xml);

                    return Redirect("~/Document");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        public ActionResult Editar(int id)
        {
            NuevoDocumentViewModel model = new NuevoDocumentViewModel();
            DetalleDocumento detalleDocumento = new DetalleDocumento();

            sgd_documentoEntity documento = DocumentoBO.getDocumentoById(id);

            string xml = "";
            if (documento.EsBorrador)
            {
                xml = System.IO.File.ReadAllText(directorio + documento.IdDocumento + "_borrador.xml");
            }
            else
            {
                xml = System.IO.File.ReadAllText(directorio + documento.IdDocumento + ".xml");
            }

            documento = (sgd_documentoEntity)FileBo.DeserializeXML(documento.GetType(), xml);
            model.Texto = documento.Texto;
            model.Titulo = documento.Titulo;
            model.Descripcion = documento.Descripcion;
            model.IdDocumento = documento.IdDocumento;
            model.VersionFinal = documento.VersionFinal;
            model.Version = documento.Version;
            model.EsBorrador = documento.EsBorrador;
            model.FechaCreacion = documento.FechaCreacion;
            model.Texto = documento.Texto;

            List<VersionesDocumento> versiones = new List<VersionesDocumento>();
            if (documento.Version > 0)
            {
                DirectoryInfo di = new DirectoryInfo(directorio);
                foreach (var fi in di.GetFiles(documento.IdDocumento + "_version*.xml"))
                {
                    VersionesDocumento version = new VersionesDocumento();
                    version.nombre = fi.Name;
                    versiones.Add(version);
                }
            }

            detalleDocumento.Document = model;
            detalleDocumento.ListaVersiones = versiones;

            return View(detalleDocumento);
        }

        [HttpPost]
        public ActionResult Editar(DetalleDocumento model)
        {
            string ruta = "";
            bool esBorrador = false;
            bool esVersion = false;
            bool esDocumento = false;
            int version = 0;
            try
            {
                if (ModelState.IsValid)
                {
                    sgd_documentoEntity documento = DocumentoBO.getDocumentoById(Int32.Parse(model.Document.IdDocumento.ToString()));
                    sgd_documentoEntity doc_ = documento;

                    documento.Descripcion = model.Document.Descripcion;
                    documento.EsBorrador = model.Document.EsBorrador;
                    documento.Texto = model.Document.Texto;
                    documento.Titulo = model.Document.Titulo;
                    documento.VersionFinal = model.Document.VersionFinal;

                    if (model.Document.EsBorrador && !model.Document.VersionFinal)
                    {
                        documento.EsBorrador = model.Document.EsBorrador;
                        documento.VersionFinal = model.Document.VersionFinal;
                        esBorrador = true;
                    }
                    else if (model.Document.VersionFinal && model.Document.EsBorrador)
                    {
                        version = documento.Version + 1;
                        documento.Version = version;
                        documento.EsBorrador = false;
                        esVersion = true;
                        esDocumento = true;
                    }
                    else
                    {
                        esDocumento = true;
                    }

                    DocumentoBO.setSaveDocumento(documento);
                        //db.Entry(documento).State = System.Data.Entity.EntityState.Modified;
                        //db.SaveChanges();

                    if (esBorrador)
                    {
                        ruta = directorio + documento.IdDocumento + "_borrador.xml";
                        guardaArchivo(ruta, documento);
                    }
                    if (esVersion)
                    {
                        ruta = directorio + documento.IdDocumento + "_version-" + version + ".xml";
                        guardaArchivo(ruta, doc_);
                    }
                    if (esDocumento)
                    {
                        ruta = directorio + documento.IdDocumento + ".xml";
                        guardaArchivo(ruta, documento);
                    }
                    
                    return Redirect("~/Document");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult Eliminar(int id)
        {
            try
            {
                
                DocumentoBO.deleteById(id);
                return Redirect("~/Document");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        public ActionResult Ma_Nuevo()
        {
            MedioAmbiental ma = new MedioAmbiental();
            return View(ma);
        }

        [HttpPost]
        public ActionResult Ma_Nuevo(MedioAmbiental ma)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    sgd_documentoEntity nuevo = new sgd_documentoEntity();

                    nuevo.Descripcion = "Base de datos medio ambiental";
                    nuevo.EsBorrador = false;
                    nuevo.Titulo = ma.Titulo;
                    nuevo.Texto = ma.Texto;
                    nuevo.FechaCreacion = DateTime.Now;
                    nuevo.VersionFinal = true;
                    nuevo.Version = 1;
                    int idDoc = DocumentoBO.setDocumentoMA(nuevo);
                    if (idDoc == 0)
                        throw new Exception("No se puede guerdar el documento");

                    ma.IdDocumento = "N" + string.Format("{0:0000000000}", idDoc);
                    string xml = FileBo.SerializeXML(ma);
                    FileBo.setXmlStringToFile(directorio_ma + "N" + string.Format("{0:0000000000}", idDoc) + ".xml", xml);

                    Ma_SendSorl(ma, true);
                    return Redirect("~/Document/Ma_NuevoSuccess/");
                }
                return View(ma);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(ma);
            }

        }

        public ActionResult Ma_NuevoSuccess()
        {
            return View();
        }

        public ActionResult Ma_VerVersion(string id)
        {
            MedioAmbiental ma = new MedioAmbiental();

            try
            {
                if (id.IndexOf("_") > -1)
                {
                    string[] archivo = id.Split('_');
                    ViewBag.Version = archivo[1].TrimStart('0');
                }
                else
                {
                    ViewBag.Version = "original";
                }

                string xml = System.IO.File.ReadAllText(directorio_ma + id + ".xml");
                ma = (MedioAmbiental)FileBo.DeserializeXML(ma.GetType(), xml);
                return View(ma);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(ma);
            }
        }

        public ActionResult Ma_VerDocumento(string id)
        {
            MedioAmbiental ma = new MedioAmbiental();

            try
            {
                string response = SolrBO.SolrQueryById(id);

                var expConverter = new ExpandoObjectConverter();
                dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(response, expConverter);

                string idDocumento = "";
                foreach (var doc in obj.response.docs)
                {
                    idDocumento = doc.ma_iddocumento;
                }
                string xml = System.IO.File.ReadAllText(directorio_ma + idDocumento + ".xml");

                ma = (MedioAmbiental)FileBo.DeserializeXML(ma.GetType(), xml);

                return View(ma);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(ma);
            }
        }

        public ActionResult Ma_EditarDocumento(string id)
        {
            MedioAmbiental ma = new MedioAmbiental();

            try
            {
                string response = SolrBO.SolrQueryById(id);

                var expConverter = new ExpandoObjectConverter();
                dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(response, expConverter);

                string idDocumento = "";
                foreach (var doc in obj.response.docs)
                {
                    idDocumento = doc.ma_iddocumento;
                }

                bool esBorrador = false;
                if (System.IO.File.Exists(directorio_ma + idDocumento + "_borrador.xml"))
                    esBorrador = true;

                string xml = "";
                if (!esBorrador)
                    xml = System.IO.File.ReadAllText(directorio_ma + idDocumento + ".xml");
                else
                    xml = System.IO.File.ReadAllText(directorio_ma + idDocumento + "_borrador.xml");

                ViewBag.EsBorrador = esBorrador;
                ma = (MedioAmbiental)FileBo.DeserializeXML(ma.GetType(), xml);
                List<VersionesDocumento> versiones = new List<VersionesDocumento>();
                if (ma.Versiones != null && ma.Versiones.Count > 0)
                {

                    foreach (VersionesDocumento v in ma.Versiones)
                    {
                        VersionesDocumento version = new VersionesDocumento();
                        version.nombre = v.nombre;
                        version.id = v.id;
                        versiones.Add(version);
                    }
                }
                else
                {
                    VersionesDocumento version = new VersionesDocumento();
                    version.nombre = idDocumento;
                    version.id = "original";
                    versiones.Add(version);
                }
                ViewBag.Versiones = versiones;
                return View(ma);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(ma);
            }
        }

        [HttpPost]
        public ActionResult Ma_EditarDocumento(MedioAmbiental ma)
        {
            try
            {
                bool existeBorrador = System.IO.File.Exists(directorio_ma + ma.IdDocumento + "_borrador.xml");
                if (!ma.EsBorrador && !existeBorrador)
                    ma.EsBorrador = true;

                string xml = FileBo.SerializeXML(ma);
                if (ma.EsBorrador)
                {
                    ma.Versiones = null;
                    FileBo.setXmlStringToFile(directorio_ma + ma.IdDocumento + "_borrador.xml", xml);
                }
                else
                {
                    if (System.IO.File.Exists(directorio_ma + ma.IdDocumento + "_borrador.xml"))
                        System.IO.File.Delete(directorio_ma + ma.IdDocumento + "_borrador.xml");

                    string response = SolrBO.SolrQueryById(ma.id);
                    var expConverter = new ExpandoObjectConverter();
                    dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(response, expConverter);
                    string idDocumento = "";
                    foreach (var doc in obj.response.docs)
                    {
                        idDocumento = doc.ma_iddocumento;
                    }
                    string versionXmlOriginal = System.IO.File.ReadAllText(directorio_ma + idDocumento + ".xml");
                    MedioAmbiental VersionOriginalMa = (MedioAmbiental)FileBo.DeserializeXML(ma.GetType(), versionXmlOriginal);

                    int totalVersiones = 0;
                    VersionesDocumento v = new VersionesDocumento();
                    List<VersionesDocumento> versiones = new List<VersionesDocumento>();
                    if (VersionOriginalMa.Versiones == null)
                    {
                        totalVersiones = 1;
                        v.nombre = idDocumento + "_" + string.Format("{0:0000000000}", totalVersiones);
                        v.id = "1";
                    }
                    else
                    {
                        totalVersiones = VersionOriginalMa.Versiones.Count + 1;
                        v.nombre = idDocumento + "_" + string.Format("{0:0000000000}", totalVersiones);
                        v.id = Convert.ToString(totalVersiones);
                    }
                    VersionOriginalMa.Versiones.Add(v);

                    versionXmlOriginal = FileBo.SerializeXML(VersionOriginalMa);
                    FileBo.setXmlStringToFile(directorio_ma + ma.IdDocumento + "_" + string.Format("{0:0000000000}", totalVersiones) + ".xml", versionXmlOriginal);

                    ma.Versiones = VersionOriginalMa.Versiones;
                    string versionFinal = FileBo.SerializeXML(ma);
                    FileBo.setXmlStringToFile(directorio_ma + ma.IdDocumento + ".xml", versionFinal);

                    Ma_SendSorl(ma, false);
                }
            }
            catch (Exception ex)
            {
                //ModelState.AddModelError("", ex.Message);
                //return View(ma);
            }
            return Redirect("~/Document/Ma_EditarDocumento/" + ma.id);
        }

        [HttpPost]
        public ActionResult ma_buscar(string q, int p)
        {
            if (p == 0)
            {
                System.Web.HttpContext.Current.Session["sessBusqueda"] = q;
                p = 1;
            }
            else
            {
                q = System.Web.HttpContext.Current.Session["sessBusqueda"] as String;
            }

            System.Web.HttpContext.Current.Session["sessPagina"] = p.ToString();

            string Query = SolrBO.SolrSelect(q, (p - 1) * 5);
            var lista = JsonConvert.DeserializeObject<ExpandoObject>(Query);

            ViewBag.Query = lista;
            ViewBag.Pagina = p;
            ViewBag.Busqueda = DecodeHtmlText(q);
            return View();
        }

        [HttpPost]
        public ActionResult Buscar(string q, int p)
        {
            if (p == 0)
            {
                System.Web.HttpContext.Current.Session["sessBusqueda"] = q;
                p = 1;
            }
            else
            {
                q = System.Web.HttpContext.Current.Session["sessBusqueda"] as String;
            }

            System.Web.HttpContext.Current.Session["sessPagina"] = p.ToString();

            string Query = SolrBO.SolrSelect(q, (p - 1) * 5);
            var lista = JsonConvert.DeserializeObject<ExpandoObject>(Query);

            ViewBag.Query = lista;
            ViewBag.Pagina = p;
            ViewBag.Busqueda = DecodeHtmlText(q);
            return View();

        }

        [HttpGet]
        public ActionResult Buscar()
        {
            string q = System.Web.HttpContext.Current.Session["sessBusqueda"] as String;
            string p = System.Web.HttpContext.Current.Session["sessPagina"] as String;

            if (q == null)
            {
                return Redirect("~/Account/LogOff");
            }

            string Query = SolrBO.SolrSelect(q, (Convert.ToInt32(p) - 1) * 5);
            var lista = JsonConvert.DeserializeObject<ExpandoObject>(Query);

            ViewBag.Query = lista;
            ViewBag.Pagina = Convert.ToInt32(p);
            ViewBag.Busqueda = DecodeHtmlText(q);
            return View();

        }

        private bool guardaArchivo(string ruta, sgd_documentoEntity doc)
        {
            string xml = FileBo.SerializeXML(doc);
            FileBo.setXmlStringToFile(ruta, xml);
            return true;
        }

        private bool Ma_SendSorl(MedioAmbiental ma, bool nuevo)
        {
            ma.Texto = Regex.Replace(ma.Texto, "[<].*?>", " ");
            ma.Texto = Regex.Replace(ma.Texto, @"\s+", " ");

            ma.Texto = DecodeHtmlText(ma.Texto);

            string xml = "";

            xml = "<add><doc>";
            if (!nuevo)
            {
                xml += "<field name=\"id\">" + ma.id + "</field>";
            }
            xml += "<field name=\"ma_iddocumento\">" + ma.IdDocumento + "</field>";
            xml += "<field name=\"ma_categoria\">" + ma.Categoria + "</field>";
            xml += "<field name=\"ma_norma\">" + ma.Norma + "</field>";
            xml += "<field name=\"ma_numero\">" + ma.Numero + "</field>";
            xml += "<field name=\"ma_organismo\">" + ma.Organismo + "</field>";
            xml += "<field name=\"ma_seccion\">" + ma.Seccion + "</field>";
            xml += "<field name=\"ma_suborganismo\">" + ma.SubOrganismo + "</field>";
            xml += "<field name=\"ma_tema\">" + ma.Tema + "</field>";
            xml += "<field name=\"ma_texto\">" + ma.Texto + "</field>";
            xml += "<field name=\"ma_titulo\">" + ma.Titulo + "</field>";
            xml += "</doc></add>";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL_SOLR);
            byte[] bytes;
            bytes = System.Text.Encoding.UTF8.GetBytes(xml);
            request.ContentType = "text/xml; encoding='utf-8'";
            request.ContentLength = bytes.Length;
            request.Method = "POST";
            Stream requestStream = request.GetRequestStream();
            requestStream.Write(bytes, 0, bytes.Length);
            requestStream.Close();
            HttpWebResponse response;
            response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = response.GetResponseStream();
                string responseStr = new StreamReader(responseStream).ReadToEnd();
            }
            return true;
        }

        private string DecodeHtmlText(string texto)
        {
            StringWriter myWriter = new StringWriter();
            HttpUtility.HtmlDecode(texto, myWriter);
            return myWriter.ToString();
        }
    }
}