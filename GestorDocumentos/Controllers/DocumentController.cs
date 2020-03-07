using GestorDocumentosBusiness;
using GestorDocumentosEntities;
using GestorDocumentosExceptions;
using mvc4.Business;
using mvc4.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace GestorDocumentos.Controllers
{
    public class DocumentController : Controller
    {
        private string directorio = WebConfigurationManager.AppSettings["MVC-DATA"];
        private string directorio_ma = WebConfigurationManager.AppSettings["MVC-DATA-MA"];
        private string directorio_imagenes = WebConfigurationManager.AppSettings["MVC-IMAGENES"];
        private string directorio_notas = WebConfigurationManager.AppSettings["MVC-NOTAS"];
        private static string URL_SOLR = WebConfigurationManager.AppSettings["webSolr"] + @"/solr/test-1/update?commitWithin=1000&overwrite=true&wt=json";

        // GET: Document
        public ActionResult Index()
        {
            var loggin = User.Identity.IsAuthenticated;
            if (!loggin)
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpGet]
        public ActionResult ImagenesDoe(string imagen)
        {
            //20180102AEAB1C1AA5356B551760805CE8686A7C
            string a = imagen.Substring(0, 4);
            string m = imagen.Substring(4, 2);
            string d = imagen.Substring(6, 2);
            string i = imagen.Substring(8, imagen.Length - 8);
            string extension = (i.Substring(i.LastIndexOf("_"), i.Length - i.LastIndexOf("_"))).Replace("_", "");

            var dir = directorio_imagenes + a + "\\" + m + "\\" + d + "\\" + i.Replace("_", ".");
            //var path = Path.Combine(dir, imagen + ".gif");
            return base.File(dir, "image/" + extension);
        }

        public ActionResult Nuevo(string id)
        {
            if (id != null && id != "")
            {
                System.Web.HttpContext.Current.Session["id-doc-referenciaNueva"] = id;
            }
            NuevoDocumentViewModel modelo = new NuevoDocumentViewModel();

            log_documentoEntity log_ = new log_documentoEntity();
            log_.idUser = User.Identity.Name;
            log_.idDocumento = id;
            log_.descripcion = "Se crea nuevo documento";

            LogBO.setLogCreateDoc(log_);

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

                    log_documentoEntity log_ = new log_documentoEntity();
                    log_.idUser = User.Identity.Name;
                    log_.idDocumento = nuevo.IdDocumento.ToString();
                    log_.descripcion = "Se crea nuevo documento";

                    LogBO.setLogCreateDoc(log_);

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

            try
            {
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

                log_documentoEntity log_ = new log_documentoEntity();
                log_.idUser = User.Identity.Name;
                log_.idDocumento = id.ToString();
                log_.descripcion = "Se edita documento";

                return View(detalleDocumento);
            }
            catch (BusinessException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
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

                log_documentoEntity log_ = new log_documentoEntity();
                log_.idUser = User.Identity.Name;
                log_.idDocumento = model.Document.IdDocumento.ToString();
                log_.descripcion = "Se edita documento";

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

                log_documentoEntity log_ = new log_documentoEntity();
                log_.idUser = User.Identity.Name;
                log_.idDocumento = id.ToString();
                log_.descripcion = "Se elimino el documento";

                LogBO.setLogCreateDoc(log_);

                return Redirect("~/Document");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        public ActionResult Ma_NuevaReferencia(string id)
        {
            Documento doc = new Documento();
            doc = Indexador.Solr.getDocumentoById(id, false);
            ViewBag.TextoOriginal = doc.Texto;
            ViewBag.Coleccion = doc.Coleccion;
            Documento referencia = new Documento();
            referencia.IdDocumento = id;

            //log_documentoEntity log_ = new log_documentoEntity();
            //log_.idUser = User.Identity.Name;
            //log_.idDocumento = id;
            //log_.descripcion = "Se crea nueva referencia del documento Medio Ambiental";

            //LogBO.setLogCreateDoc(log_);

            return View(referencia);
        }

        [HttpPost]
        public ActionResult Ma_NuevaReferencia(Documento doc)
        {
            ViewBag.Error = null;
            string rutaBorrador = "";
            try
            {
                Documento docOriginal = Indexador.Solr.getDocumentoById(doc.IdDocumento, true);
                if(docOriginal.Version.IndexOf("_borrador") < 0)
                    rutaBorrador = docOriginal.Version.Replace(".xml", "_borrador.xml");
                else
                    rutaBorrador = docOriginal.Version;

                if (doc.Norma == "0" || doc.Titulo == null || doc.Texto == null || doc.Coleccion == null)
                {
                    ViewBag.Error = "Estimado usuario, todos los campos son obligatorios, por favor complete el formulario para continuar.";
                    ViewBag.TextoOriginal = docOriginal.Texto;
                    ViewBag.Coleccion = docOriginal.Coleccion;
                    return View(doc);
                }
                else
                {
                    //doc.Fecha = DateTime.Now;
                    //doc.Coleccion = docOriginal.Coleccion;
                    doc.Origen = docOriginal.Origen;
                    #region links documento nuevo
                    doc.Links = new List<Link>();
                    Link l = new Link();
                    l.Tipo = docOriginal.Norma;
                    l.Url = docOriginal.IdDocumento;
                    string textoReferenciaDestino = "";
                    if (docOriginal.Numero != null && docOriginal.Numero != "")
                    {
                        textoReferenciaDestino = "Número " + docOriginal.Numero;
                    }
                    else
                    {
                        if (docOriginal.Articulo != null && docOriginal.Articulo != "")
                            textoReferenciaDestino += "Artículo N° " + docOriginal.Articulo;
                        if (docOriginal.Inciso != null && docOriginal.Inciso != "")
                            textoReferenciaDestino += ", Inciso " + docOriginal.Inciso;
                        if (docOriginal.Tribunal != null && docOriginal.Tribunal != "")
                            textoReferenciaDestino += docOriginal.Tribunal;
                        if (docOriginal.Partes != null && docOriginal.Partes != "")
                            textoReferenciaDestino += ".- " + docOriginal.Partes;
                    }
                    textoReferenciaDestino += ".- " + docOriginal.Titulo;
                    l.Texto = textoReferenciaDestino;
                    l.Colecciones = string.Join(", ", docOriginal.Coleccion);
                    doc.Links.Add(l);
                    #endregion

                    string versionFinal = FileBo.SerializeXML(doc);
                    doc.id = null;
                    doc.IdDocumento = UtilesBO.getMd5(versionFinal);
                    //1985-01-03T00:00:00
                    doc.Fecha = docOriginal.Fecha.Substring(0, 10);
                    //string f = docOriginal.Fecha.ToString("yyyy-MM-dd");
                    //IFormatProvider culture = new CultureInfo("en-US", true);
                    //doc.Fecha = DateTime.ParseExact(f, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                    string usuario = User.Identity.GetUserName();

                    #region links documento original

                    if (docOriginal.Links == null)
                        docOriginal.Links = new List<Link>();

                    List<Link> links = new List<Link>();
                    l = new Link();
                    l.Texto = doc.Titulo;
                    l.Tipo = doc.Norma;
                    l.Url = doc.IdDocumento;
                    l.Colecciones =  string.Join(", ", doc.Coleccion);
                    links.Add(l);

                    foreach (Link link in docOriginal.Links)
                    {
                        links.Add(link);
                    }
                    docOriginal.Links = links;
                    docOriginal.Estado = 99;
                    docOriginal.Usuario = usuario;
                    #endregion

                    versionFinal = FileBo.SerializeXML(doc);


                    string rutaFinal = docOriginal.Version.Substring(0, docOriginal.Version.LastIndexOf("\\"));
                    FileBo.setXmlStringToFile(rutaFinal + "\\" + doc.IdDocumento + ".xml", versionFinal);

                    versionFinal = FileBo.SerializeXML(docOriginal);
                    FileBo.setXmlStringToFile(rutaBorrador, versionFinal);

                    
                    Indexador.Solr.cambiaEstadoDocumento(docOriginal.id, 99, usuario);


                    Indexador.Solr.sendXmlDocumento(doc, true);
                }
                return Redirect("~/Document/Ma_EditarDocumento/" + docOriginal.IdDocumento);
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
        public ActionResult Ma_Nuevo(Documento ma)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string norma = ma.Norma.Replace(" ", "_") + "\\";
                    //sgd_documentoEntity nuevo = new sgd_documentoEntity();

                    //nuevo.Descripcion = "Base de datos medio ambiental";
                    //nuevo.EsBorrador = false;
                    //nuevo.Titulo = ma.Titulo;
                    //nuevo.Texto = ma.Texto;
                    //nuevo.FechaCreacion = DateTime.Now;
                    //nuevo.VersionFinal = true;
                    //nuevo.Version = 1;
                    //int idDoc = DocumentoBO.setDocumentoMA(nuevo);
                    //if (idDoc == 0)
                    //    throw new Exception("No se puede guerdar el documento");

                    //ma.IdDocumento = "N" + string.Format("{0:0000000000}", idDoc);
                    string xml = FileBo.SerializeXML(ma);
                    string idDoc = UtilesBO.getMd5(xml);
                    FileBo.setXmlStringToFile(directorio_ma + norma + idDoc + ".xml", xml);

                    Ma_SendSorl(ma, true);
                    log_documentoEntity log_ = new log_documentoEntity();
                    log_.idUser = User.Identity.Name;
                    log_.idDocumento = idDoc;
                    log_.descripcion = "Se crea nuevo documento Medio Ambiental";

                    LogBO.setLogCreateDoc(log_);

                    return Redirect("~/Document/Ma_NuevoSuccess/");
                }

                return View(ma);
            }
            catch (BusinessException ex)
            {
                ModelState.AddModelError("", ex.Message);
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

        [HttpPost]
        public string Nota(Nota nota)
        {
            Nota notaResult = new Nota();
            try
            {
                string r = directorio_notas; // directorio + nota.Coleccion + "\\Notas\\";
                if (nota.Result == 1)
                {
                    string txt = System.IO.File.ReadAllText(r + nota.TextoNota + ".html", Encoding.UTF8);
                    notaResult.TextoNota = txt;
                    notaResult.Result = 0;
                    notaResult.Coleccion = null;
                }
                else
                {

                    if (!Directory.Exists(r))
                        Directory.CreateDirectory(r);

                    string h = UtilesBO.getMd5(nota.TextoNota);

                    StreamWriter sw_ = new StreamWriter(r + "\\" + nota.Coleccion + h + ".html", false);
                    sw_.Write(nota.TextoNota);
                    sw_.Close();

                    log_documentoEntity log_ = new log_documentoEntity();
                    log_.idUser = User.Identity.Name;
                    log_.idDocumento = notaResult.Result.ToString();
                    log_.descripcion = "Se crea nueva nota";

                    LogBO.setLogCreateDoc(log_);

                    notaResult.TextoNota = nota.Coleccion + h;
                    notaResult.Coleccion = null;
                    notaResult.Result = 0;
                }
            }
            catch (Exception ex)
            {
                new TechnicalException("Error al mostrar las notas", ex);
                notaResult.Result = 1;
                notaResult.TextoNota = "Error al buscar nota, por favor reintentar";
            }

            return JsonConvert.SerializeObject(notaResult);
        }

        [HttpPost]
        public string Ma_Historial(string Id)
        {
            try
            {
                List<log_documentoEntity> historial = LogBO.getHistorialDocumento(Id);
                return JsonConvert.SerializeObject(historial);
            }
            catch (BusinessException bex)
            {
                return "{\"Error\":\"" + bex.Message + "\"}";
            }
        }

        public ActionResult Ma_VerVersion(string id)
        {
            Documento d = Indexador.Solr.getDocumentoById(id.Substring(0, id.IndexOf("_")), true);
            //string response = SolrBO.SolrQueryById(id.Substring(0, id.IndexOf("_")));

            //var expConverter = new ExpandoObjectConverter();
            //dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(response, expConverter);

            //string idDocumento = "";
            //string norma = "";
            //foreach (var doc in obj.response.docs)
            //{
            //    idDocumento = doc.IdDocumento;
            //    norma = (doc.Norma).Replace(" ", "_") + "\\";
            //}

            //Documento ma = new Documento();
            try
            {
                string[] archivo = null;
                if (id.IndexOf("_") > -1)
                {
                    archivo = id.Split('_');
                    ViewBag.Version = archivo[1].TrimStart('0');
                }
                else
                {
                    ViewBag.Version = "original";
                }

                string xml = System.IO.File.ReadAllText(d.Version.Replace(".xml", "_" + archivo[1] + ".xml"));
                d = (Documento)FileBo.DeserializeXML(d.GetType(), xml);

                log_documentoEntity log_ = new log_documentoEntity();
                log_.idUser = User.Identity.Name;
                log_.idDocumento = id.ToString();
                log_.descripcion = "Se crea nueva versión del documento";

                LogBO.setLogCreateDoc(log_);

                return View(d);
            }
            catch (BusinessException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(d);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(d);
            }
        }

        public ActionResult SetReferencia(string textoReferencia, string idDocumento, string tipo, string[] colecciones)
        {
            string idDocOriginal = "";
            try
            {
                idDocOriginal = (string)System.Web.HttpContext.Current.Session["id-doc-referencia"];

                string norma = "";

                Documento docOriginal = Indexador.Solr.getDocumentoById(idDocOriginal, true);
                Documento docDestino = Indexador.Solr.getDocumentoById(idDocumento, true);

                if (docOriginal != null)
                {
                    #region documento original

                    //string textoReferenciaDestino = "";
                    //if (docOriginal.Organismo != "")
                    //    textoReferenciaDestino = docOriginal.Organismo + "<br />";

                    // textoReferenciaDestino += docOriginal.Norma + " ";
                    //if (docOriginal.Numero != null && docOriginal.Numero != "")
                    //{
                    //    textoReferenciaDestino += "Número " + docOriginal.Numero;
                    //}
                    //else
                    //{
                    //    if (docOriginal.Articulo != null && docOriginal.Articulo != "")
                    //        textoReferenciaDestino += "Artículo N° " + docOriginal.Articulo;
                    //    if (docOriginal.Inciso != null && docOriginal.Inciso != "")
                    //        textoReferenciaDestino += ", Inciso " + docOriginal.Inciso;
                    //}
                    //if (docOriginal.Titulo != "")
                    //    textoReferenciaDestino += ".- " + docOriginal.Titulo;


                    if (docOriginal.Links == null)
                        docOriginal.Links = new List<Link>();

                    List<Link> links = new List<Link>();
                    Link l = new Link();
                    l.Texto = textoReferencia;
                    l.Tipo = tipo + ".- " + docDestino.Norma;
                    l.Url = idDocumento;
                    l.Colecciones = string.Join(", ", colecciones);
                    links.Add(l);

                    foreach (Link link in docOriginal.Links)
                    {
                        links.Add(link);
                    }

                    docOriginal.Links = links;
                    docOriginal.Estado = 99;

                    string versionFinal = FileBo.SerializeXML(docOriginal);
                    string usuario = User.Identity.GetUserName();


                    if (docOriginal.Version.IndexOf("_borrador") < 0)
                    {
                        FileBo.setXmlStringToFile(docOriginal.Version.Replace(".xml", "_borrador.xml"), versionFinal);
                        Indexador.Solr.cambiaEstadoDocumento(docOriginal.id, 99, usuario); //borrador
                    }
                    else
                        FileBo.setXmlStringToFile(docOriginal.Version, versionFinal);
                    #endregion
                    #region documento destino                   
                    //if (docDestino.Links == null)
                    //    docDestino.Links = new List<Link>();

                    //links = new List<Link>();
                    //l = new Link();
                    //l.Texto = textoReferenciaDestino;
                    //l.Tipo = docOriginal.Norma;
                    //l.Url = idDocOriginal;
                    //links.Add(l);

                    //foreach (Link link in docDestino.Links)
                    //{
                    //    links.Add(link);
                    //}
                    //docDestino.Links = links;
                    //norma = docDestino.Norma.Replace(" ", "_") + "\\";
                    //versionFinal = FileBo.SerializeXML(docDestino);
                    //FileBo.setXmlStringToFile(docDestino.Version, versionFinal);
                    #endregion
                }

                //setLog(docOriginal, "Agrega referencia documento existente: " + textoReferencia);
                //log_documentoEntity log_ = new log_documentoEntity();
                //log_.idUser = User.Identity.Name;
                //log_.idDocumento = idDocumento;
                //log_.descripcion = "Se crea nueva referencia del documento " + docOriginal.IdDocumento + " al " + docDestino.IdDocumento;

                //LogBO.setLogCreateDoc(log_);

                Historial historial = new Historial();
                historial.Tipo = 2; // agrega referencia doc existente
                historial.Estado = 0; // pendiente
                historial.IdOriginal = docOriginal.IdDocumento;
                historial.IdReferencia = docDestino.IdDocumento;
                Indexador.Solr.sendXmlHistoria(historial);
                System.Web.HttpContext.Current.Session["id-doc-referencia"] = null;

            }
            catch (BusinessException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }

            return Redirect("~/Document/Ma_EditarDocumento/" + idDocOriginal);
        }

        public ActionResult Ma_VerDocumento(string id)
        {
            Documento d = new Documento();

            Documento docR = new Documento();
            string rutaDoc = "";
            ViewBag.Referencia = false;
            try
            {
                try
                {
                    if (System.Web.HttpContext.Current.Session["id-doc-referencia"] != null)
                    {
                        string idR = (string)System.Web.HttpContext.Current.Session["id-doc-referencia"];
                        Documento d_ = Indexador.Solr.getDocumentoById(idR, false);
                        //rutaDoc = Indexador.Solr.getUrlDocumentById(d_.Version); //SolrBO.SolrGetUrlDocumentById(idR);
                        string xml = System.IO.File.ReadAllText(d_.Version);
                        docR = (Documento)FileBo.DeserializeXML(d.GetType(), xml);
                        ViewBag.Referencia = true;
                        ViewBag.DocumentoR = docR;
                    }
                }
                catch (Exception ex)
                { }

                d = Indexador.Solr.getDocumentoById(id, false); // SolrBO.SolrQueryById(id);
                string c = d.Coleccion[0];
                if (c.IndexOf("DO") < 0)
                    ViewBag.Coleccion = d.Coleccion[0];
                else
                    ViewBag.Coleccion = "DOE";
                return View(d);
            }
            catch (BusinessException ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return View(d);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", "No se encontro el xml del documento por favor verificar o comunicarse con el administrador.");
                new TechnicalException("Error al ver documento, id :" + id, ex);
                d = new Documento();
                d.Links = new List<Link>();
                return View(d);
            }
        }

        [HttpGet]
        public ActionResult Ma_NuevoDocumento()
        {
            ViewBag.ListaNormas = getNormas();
            Documento d = new Documento();
            d.Fecha = DateTime.Now.ToShortDateString();
            d.Links = new List<Link>();
            d.Versiones = new List<VersionesDocumento>();
            return View(d);
        }

        private string[] getNormas()
        {
            string ruta = WebConfigurationManager.AppSettings["MVC-DIRECTORIO"] + "Configuracion\\Normas.txt";
            string[] txt = (System.IO.File.ReadAllText(ruta)).Replace("\r", "").Replace("\n", "").Split(',');
            return txt;
        }

        [HttpGet]
        public ActionResult Ma_EditarDocumento(string id)
        {
            Documento d = new Documento();

            string idNota = null;
            if (id.IndexOf("-") > -1)
            {
                string[] ids = id.Split('-');
                id = ids[0];
                idNota = ids[1];
            }

            try
            {

                d = Indexador.Solr.getDocumentoById(id, true);

                #region notas
                if (idNota != null && idNota != "")
                {
                    string html = "";
                    string rNota = directorio + d.Coleccion + "\\Notas\\" + idNota + ".html";
                    if (System.IO.File.Exists(rNota))
                    {
                        ViewBag.IdNota = idNota;
                    }
                    else
                    {
                        ViewBag.IdNota = "";
                    }
                }
                string c = d.Origen;
                if (c.IndexOf("DO") < 0)
                    ViewBag.Coleccion = d.Origen;
                else
                    ViewBag.Coleccion = "DOE";
                #endregion

                bool administrador = false;
                string rol = Session["rol"].ToString();
                if (rol == Sys_RolEntity.ADMINISTRADOR)
                    administrador = true;

                if (d.Estado == 98 && !administrador)
                    ViewBag.Estado = "Pendiente";
                else
                    ViewBag.Estado = "Borrador";

                if (d.Estado == 0)
                    ViewBag.Estado = "";

                if (d.Usuario != null)
                    ViewBag.Usuario = d.Usuario;
                else
                    ViewBag.Usuario = "";

                ViewBag.listadoUsuarios = usuarioBO.getListaUsuarios();

                return View(d);
            }
            catch (BusinessException ex)
            {
                return Redirect("~/Document/Ma_Error/");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return View(d);
            }
        }

        public ActionResult Ma_Error()
        {
            return View();
        }

        [HttpPost]
        public string GetLista(string lista, string tipo, string coleccion)
        {
            try
            {
                coleccion = (coleccion.Replace("\n", "").Replace("\r", "")).Replace(", ", ",").Trim();
                lista = "," + (lista.Replace("\n", "").Replace("\r", "")).Replace(", ", ",").Trim() + ",";
                string ruta = WebConfigurationManager.AppSettings["MVC-DIRECTORIO"] + "Configuracion\\";
                if (tipo == "Seccion" || tipo == "Categoria")
                    ruta = ruta + "MA_";
                if (tipo == "Tema")
                    tipo = "temas";

                string[] txt = (System.IO.File.ReadAllText(ruta + tipo + ".txt")).Replace("\r", "").Replace("\n", "").Split(',');

                Listado listado = new Listado();
                listado.Detalles = new List<Detalle>();
                bool lee = false;
                for (int x = 0; x < txt.Length; x++)
                {
                    Detalle detalle = new Detalle();
                    detalle.Nombre = txt[x];

                    if (tipo == "temas")
                    {
                        if (detalle.Nombre.IndexOf("##MA") > -1)
                        {
                            if (coleccion.IndexOf("MA") > -1)
                            {
                                lee = true;
                                detalle.Nombre = ".:: MEDIO AMBIENTAL ::.";
                            }
                            else
                            {
                                lee = false;
                            }
                        }
                        else if (detalle.Nombre.IndexOf("##LA") > -1)
                        {
                            if (coleccion.IndexOf("LA") > -1)
                            {
                                lee = true;
                                detalle.Nombre = ".:: LEGISLACION ACTUALIZADA ::.";
                            }
                            else
                            {
                                lee = false;
                            }
                        }
                    }
                    else
                    {
                        lee = true;
                    }

                    if (lee)
                    {
                        if (lista.IndexOf("," + txt[x] + ",") > -1)
                            detalle.Seleccionado = true;
                        else
                            detalle.Seleccionado = false;
                        listado.Detalles.Add(detalle);
                    }
                }
                return JsonConvert.SerializeObject(listado);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult Ma_EliminaReferencias(string referencias)
        {
            string usuario = User.Identity.GetUserName();
            string[] r = null;
            try
            {
                r = referencias.Split('-');
                string idDocumento = r[1];
                string idReferencia = r[0];
                string textoReferencia = "";

                Documento d = Indexador.Solr.getDocumentoById(idDocumento, true);
                string xml = System.IO.File.ReadAllText(d.Version);
                Documento doc = (Documento)FileBo.DeserializeXML(d.GetType(), xml);
                List<Link> links = new List<Link>();
                foreach(Link l in doc.Links)
                {
                    if (l.Url != idReferencia)
                        links.Add(l);
                    else
                        textoReferencia = l.Texto;
                }
                doc.Links = links;

                doc.Usuario = usuario;
                doc.Estado = 99;
                string versionFinal = FileBo.SerializeXML(doc);
                if(d.Version.IndexOf("_borrador") > -1)
                    FileBo.setXmlStringToFile(d.Version, versionFinal);
                else
                    FileBo.setXmlStringToFile(d.Version.Replace(".xml", "_borrador.xml"), versionFinal);

                Historial historial = new Historial();
                historial.Tipo = 1; // elimina referencia
                historial.Estado = 0; // pendiente
                historial.IdOriginal = idDocumento;
                historial.IdReferencia = idReferencia;
                Indexador.Solr.sendXmlHistoria(historial);
                Indexador.Solr.cambiaEstadoDocumento(d.id, 99, usuario);
                //setLog(d, "Guarda borrador ELIMINA REFERENCIA.- " + textoReferencia);

            }
            catch (Exception)
            {

            }

            return Redirect("~/Document/Ma_EditarDocumento/" + r[1]);
        }

        [HttpGet]
        public ActionResult Ma_EliminaBorrador(string id)
        {
            Documento d = new Documento();
            try
            {
                d = Indexador.Solr.getDocumentoById(id, true);
                string ruta = d.Version;
                if (System.IO.File.Exists(ruta))
                {
                    System.IO.File.Delete(ruta);
                    Indexador.Solr.cambiaEstadoDocumento(d.id, 0, "");
                }
            }
            catch (BusinessException)
            {

            }
            return Redirect("~/Document/Ma_EditarDocumento/" + id);
        }

        [HttpPost]
        public ActionResult Ma_RechazarBorrador(DetalleReasignacion reasignacion)
        {
            Documento d = Indexador.Solr.getDocumentoById(reasignacion.documentoReasignacion, true);

            d.Estado = 99;
            d.Usuario = reasignacion.usuarioReasignacion;
            string xml = FileBo.SerializeXML(d);
            FileBo.setXmlStringToFile(d.Version, xml);

            Indexador.Solr.cambiaEstadoDocumento(d.id, 99, reasignacion.usuarioReasignacion);

            string texto = "(Reasignado a " + reasignacion.usuarioReasignacion + " ) " + reasignacion.textoReasignacion;

            setLog(d, texto);

            return Redirect("~/Document/Ma_DocumentoReasignado/");
        }

        public ActionResult Ma_DocumentoReasignado()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Ma_NuevoDocumento(Documento ma)
        {
            string error = "";
            try
            {
                string usuario = User.Identity.GetUserName();

                ma.ColeccionGlosa = (ma.ColeccionGlosa.Replace("\r", "").Replace("\n", "")).Trim();
                if (ma.CategoriaGlosa != null)
                    ma.CategoriaGlosa = (ma.CategoriaGlosa.Replace("\r", "").Replace("\n", "")).Trim();
                if (ma.SeccionGlosa != null)
                    ma.SeccionGlosa = (ma.SeccionGlosa.Replace("\r", "").Replace("\n", "")).Trim();
                if (ma.TemaGlosa != null)
                    ma.TemaGlosa = (ma.TemaGlosa.Replace("\r", "").Replace("\n", "")).Trim();

                if (ma.ColeccionGlosa != null && ma.ColeccionGlosa != "")
                    ma.Coleccion = (ma.ColeccionGlosa.Replace(", ", ",")).Split(',');
                if (ma.CategoriaGlosa != null && ma.CategoriaGlosa != "")
                    ma.Categoria = (ma.CategoriaGlosa.Replace(", ", ",")).Split(',');
                if (ma.SeccionGlosa != null && ma.SeccionGlosa != "")
                    ma.Seccion = (ma.SeccionGlosa.Replace(", ", ",")).Split(',');
                if (ma.TemaGlosa != null && ma.TemaGlosa != "")
                    ma.Tema = (ma.TemaGlosa.Replace(", ", ",")).Split(',');

                if (ma.ColeccionGlosa == null || ma.ColeccionGlosa == "")
                    error += "Debe indicar la colección a la que pertenece el documento.- ";
                if (ma.Norma == null || ma.Norma == "0")
                    error += "Debe indicar el tipo de norma del documento.- ";
                if (ma.Fecha == null || ma.Fecha == "")
                    error += "Debe indicar la fecha del documento.- ";

                if (error != "")
                    throw new Exception(error);



                if (ma.EsBorrador)
                {
                    string fecha = ma.Fecha.Replace("/", "-");
                    string[] f = fecha.Split('-');

                    ma.Fecha = f[2] + "-" + f[1] + "-" + f[0];
                    ma.Estado = 99; // borrador
                    ma.Usuario = usuario;
                    ma.EsBorrador = true;
                    ma.Origen = ma.Coleccion[0];


                    ma.IdDocumento = UtilesBO.getMd5(FileBo.SerializeXML(ma));
                    string xml = FileBo.SerializeXML(ma);                    

                    string rutaBorrador = WebConfigurationManager.AppSettings["MVC-DATA"];
                    if (ma.Origen != "BITE" && ma.Origen != "MA" && ma.Origen != "LA")
                        rutaBorrador += "DOE\\" + f[2] + "\\" + f[1] + "\\" + f[0] + "\\";
                    else
                        rutaBorrador += ma.Coleccion[0] + "\\";
                    if (!Directory.Exists(rutaBorrador))
                        Directory.CreateDirectory(rutaBorrador);
                    FileBo.setXmlStringToFile(rutaBorrador + ma.IdDocumento + "_borrador.xml", xml);
                    FileBo.setXmlStringToFile(rutaBorrador + ma.IdDocumento + ".xml", xml);
                    Indexador.Solr.sendXmlDocumento(ma, true);
                    //Indexador.Solr.cambiaEstadoDocumento(d.id, estado, usuario); //borrador
                }
                return Redirect("~/Document/Ma_EditarDocumento/" + ma.IdDocumento);

            }
            catch (Exception ex)
            {
                ViewBag.SelectNorma = ma.Norma;
                ViewBag.ListaNormas = getNormas();
                ma.EsBorrador = false;
                ModelState.AddModelError("Error", ex.Message);
                return View(ma);
            }
        }

        [HttpGet]
        public ActionResult Ma_EliminarVersion(string id)
        {
            string idDocumento = "";
            string idv = "";
            try
            {
                string[] d = (id.Replace("btnEliminarVersion_", "")).Split('_');
                idDocumento = d[0];
                Documento ma = Indexador.Solr.getDocumentoById(d[0], true);
                foreach(VersionesDocumento v in ma.Versiones)
                {
                    if (v.nombre == d[0] + "_" + d[1])
                    {
                        v.estado = 1;
                        idv = v.id;
                    }
                }
                string xml = FileBo.SerializeXML(ma);
                string rutaBorrador = ma.Version; //.Replace(".xml", "_" + d[1] + ".xml");
                FileBo.setXmlStringToFile(rutaBorrador, xml);
                setLog(ma, "Elimina versión número " + idv);
            }
            catch { };
            return Redirect("~/Document/Ma_EditarDocumento/" + idDocumento);
        }

        [HttpPost]
        public ActionResult Ma_EditarVersion(Documento doc)
        {
            string idDocumento = "";
            try
            {
                if (doc.Texto == "")
                    throw new Exception("Debe indicar el texto del documento");
                if (doc.TextoDescripcionVersion == "")
                    throw new Exception("Debe indicar la descripción de la versión");

                // lee doc original
                Documento ma = new Documento();
                string[] d = doc.IdDocumento.Split('_');
                ma = Indexador.Solr.getDocumentoById(d[0], true);

                // lee doc version
                Documento maV = new Documento();
                string xmlV = System.IO.File.ReadAllText(ma.Version.Replace(".xml", "_" + d[1] + ".xml"));
                maV = (Documento)FileBo.DeserializeXML(doc.GetType(), xmlV);

                // actualiza y guarda version
                maV.Texto = doc.Texto;
                string idv = "";
                idDocumento = d[0];
                foreach(VersionesDocumento v in maV.Versiones)
                {
                    if(v.nombre == doc.IdDocumento)
                    {
                        v.descripcion = doc.TextoDescripcionVersion;
                    }
                }
                // actualiza y guarda doc original
                foreach (VersionesDocumento v in ma.Versiones)
                {
                    if (v.nombre == doc.IdDocumento)
                    {
                        v.descripcion = doc.TextoDescripcionVersion;
                        idv = v.id;
                    }
                }
                string xml = FileBo.SerializeXML(ma);
                FileBo.setXmlStringToFile(ma.Version, xml);

                xmlV = FileBo.SerializeXML(maV);
                string rutaBorrador = ma.Version.Replace(".xml", "_" + d[1] + ".xml");
                FileBo.setXmlStringToFile(rutaBorrador, xmlV);
                setLog(ma, "Modifica versión número " + idv);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
                return View(doc);
            }
            return Redirect("~/Document/Ma_EditarDocumento/" + idDocumento);
        }

        [HttpGet]
        public ActionResult Ma_EditarVersion(string id)
        {
            string[] d = id.Split('_');
            Documento doc = Indexador.Solr.getDocumentoById(d[0], true);
            string xml = System.IO.File.ReadAllText(doc.Version.Replace(".xml", "_" + d[1] + ".xml"));
            Documento _doc = (Documento)FileBo.DeserializeXML(doc.GetType(), xml);

            int t = Convert.ToInt32(d[1].TrimStart('0'));
            VersionesDocumento version = _doc.Versiones[t - 1];
            ViewBag.Version = t;
            doc.Texto = _doc.Texto;
            doc.TextoDescripcionVersion = version.descripcion;
            doc.IdDocumento = id;
            ViewBag.IdOriginal = d[0];
            return View(doc);
        }

        [HttpPost]
        public ActionResult Ma_EditarDocumento(Documento ma)
        {
            string usuario = User.Identity.GetUserName();
            try
            {
                //if (ma.Coleccion == null)
                //    throw new BusinessException("Debe indicar la colección");
                //if(ma.Texto == "")
                //    throw new BusinessException("Debe indicar el texto del documento");


                bool administrador = false;
                string rol = Session["rol"].ToString();
                if (rol == Sys_RolEntity.ADMINISTRADOR)
                    administrador = true;
                else
                    ma.EsBorrador = true;

                ma.ColeccionGlosa = (ma.ColeccionGlosa.Replace("\r", "").Replace("\n", "")).Trim();
                if (ma.CategoriaGlosa != null)
                    ma.CategoriaGlosa = (ma.CategoriaGlosa.Replace("\r", "").Replace("\n", "")).Trim();
                if (ma.SeccionGlosa != null)
                    ma.SeccionGlosa = (ma.SeccionGlosa.Replace("\r", "").Replace("\n", "")).Trim();
                if (ma.TemaGlosa != null)
                    ma.TemaGlosa = (ma.TemaGlosa.Replace("\r", "").Replace("\n", "")).Trim();
                Documento d = new Documento();
                List<Historial> historial = new List<Historial>();

                d = Indexador.Solr.getDocumentoById(ma.id, true);
                historial = Indexador.Solr.getHistorial(ma.IdDocumento);

                string xmlBorrador = System.IO.File.ReadAllText(d.Version);
                Documento docBorrador = (Documento)FileBo.DeserializeXML(ma.GetType(), xmlBorrador);

                string rutaBorrador = "";
                if (d.Version.IndexOf("_borrador") < 0)
                    rutaBorrador = d.Version.Replace(".xml", "_borrador.xml");
                else
                    rutaBorrador = d.Version;

                if (ma.ColeccionGlosa != null)
                    ma.Coleccion = (ma.ColeccionGlosa.Replace(", ", ",")).Split(',');
                if (ma.CategoriaGlosa != null)
                    ma.Categoria = (ma.CategoriaGlosa.Replace(", ", ",")).Split(',');
                if (ma.SeccionGlosa != null)
                    ma.Seccion = (ma.SeccionGlosa.Replace(", ", ",")).Split(',');
                if (ma.TemaGlosa != null)
                    ma.Tema = (ma.TemaGlosa.Replace(", ", ",")).Split(',');

                ma.Origen = d.Origen;
                ma.Links = docBorrador.Links;
                ma.Fecha = d.Fecha;

                int estado = 0;
                if (ma.EsBorrador)
                {
                    if (d.Estado != 98)
                    {
                        ma.Versiones = null;
                        if (ma.Publicar)
                        {
                            estado = 98; // queda pendiente revision
                            setLog(ma, "(Pendiente publicación) " + ma.TextoCambio);
                        }
                        else
                        {
                            estado = 99; // estado borrador                                                        
                        }
                        ma.Estado = estado;
                        ma.Usuario = usuario;
                        string xml = FileBo.SerializeXML(ma);
                        FileBo.setXmlStringToFile(rutaBorrador, xml);
                        Indexador.Solr.cambiaEstadoDocumento(d.id, estado, usuario); //borrador
                    }
                }
                else
                {
                    if (System.IO.File.Exists(rutaBorrador))
                        System.IO.File.Delete(rutaBorrador);

                    string versionXmlOriginal = System.IO.File.ReadAllText(d.Version.Replace("_borrador.xml", ".xml"));
                    Documento VersionOriginalMa = (Documento)FileBo.DeserializeXML(ma.GetType(), versionXmlOriginal);

                    int totalVersiones = 0;
                    if (ma.VersionFinal)
                    {
                        VersionesDocumento v = new VersionesDocumento();
                        List<VersionesDocumento> versiones = new List<VersionesDocumento>();
                        if (VersionOriginalMa.Versiones == null)
                        {
                            totalVersiones = 1;
                            v.nombre = d.IdDocumento + "_" + string.Format("{0:0000000000}", totalVersiones);
                            v.id = "1";
                            v.descripcion = ma.TextoDescripcionVersion;
                        }
                        else
                        {
                            totalVersiones = VersionOriginalMa.Versiones.Count + 1;
                            v.nombre = d.IdDocumento + "_" + string.Format("{0:0000000000}", totalVersiones);
                            v.id = Convert.ToString(totalVersiones);
                            v.descripcion = ma.TextoDescripcionVersion;
                        }
                        VersionOriginalMa.Versiones.Add(v);


                        versionXmlOriginal = FileBo.SerializeXML(VersionOriginalMa);
                        FileBo.setXmlStringToFile(d.Version.Replace("_borrador.xml", "").Replace(".xml", "") + "_" + string.Format("{0:0000000000}", totalVersiones) + ".xml", versionXmlOriginal);
                    }
                    //ma.Versiones = VersionOriginalMa.Versiones;
                    //ma.Links = VersionOriginalMa.Links;

                    VersionOriginalMa.Origen = ma.Origen;
                    VersionOriginalMa.Coleccion = ma.Coleccion; // (ma.ColeccionGlosa.Replace(", ", ",")).Split(',');
                    VersionOriginalMa.Norma = ma.Norma;
                    VersionOriginalMa.Organismo = ma.Organismo;
                    VersionOriginalMa.Suborganismo = ma.Suborganismo;
                    VersionOriginalMa.Seccion = ma.Seccion;
                    VersionOriginalMa.Articulo = ma.Articulo;
                    VersionOriginalMa.Inciso = ma.Inciso;
                    VersionOriginalMa.Numero = ma.Numero;
                    VersionOriginalMa.Categoria = ma.Categoria;
                    VersionOriginalMa.Tema = ma.Tema;
                    VersionOriginalMa.Titulo = ma.Titulo;
                    VersionOriginalMa.Texto = ma.Texto;
                    VersionOriginalMa.id = d.id;
                    VersionOriginalMa.Estado = estado;
                    VersionOriginalMa.Usuario = usuario;
                    VersionOriginalMa.Links = docBorrador.Links;

                    string versionFinal = FileBo.SerializeXML(VersionOriginalMa);
                    FileBo.setXmlStringToFile(d.Version.Replace("_borrador.xml", ".xml"), versionFinal);

                    Indexador.Solr.sendXmlDocumento(VersionOriginalMa, false);

                    #region actualiza links segun historial
                    if (historial != null && historial.Count > 0)
                    {
                        foreach (Historial h in historial)
                        {
                            Documento docHistorial = Indexador.Solr.getDocumentoById(h.IdReferencia, true);
                            string rutaHistorial = "";
                            if (System.IO.File.Exists(docHistorial.Version))
                                rutaHistorial = docHistorial.Version;
                            else
                                rutaHistorial = docHistorial.Version.Replace("_borrador.xml", ".xml");

                            string xmlReferencia = System.IO.File.ReadAllText(rutaHistorial);
                            Documento docBorradorHistorial = (Documento)FileBo.DeserializeXML(docHistorial.GetType(), xmlReferencia);
                            if (h.Tipo == 1) // elimina referencia doc destino
                            {
                                List<Link> links = new List<Link>();
                                foreach (Link l in docBorradorHistorial.Links)
                                {
                                    if (l.Url != h.IdOriginal)
                                        links.Add(l);
                                }                              
                                docBorradorHistorial.Links = links;
                                if (docBorradorHistorial.Norma == "COMENTARIO" || docBorradorHistorial.Norma == "EJEMPLO"
                                    || (docBorradorHistorial.Norma).Replace("_", " ") == "JURISPRUDENCIA JUDICIAL")
                                {
                                    if (docBorradorHistorial.Links.Count == 0)
                                        EliminaNorma(docBorradorHistorial.IdDocumento);
                                }
                                string versionFinalReferencia = FileBo.SerializeXML(docBorradorHistorial);
                                FileBo.setXmlStringToFile(docHistorial.Version.Replace("_borrador.xml", ".xml"), versionFinalReferencia);
                            }
                            if (h.Tipo == 2) // agrega ref doc existente
                            {
                                string txtLink = "";
                                if (VersionOriginalMa.Organismo != null && VersionOriginalMa.Organismo != "")
                                    txtLink += VersionOriginalMa.Organismo + ".- ";
                                if (VersionOriginalMa.Norma != null && VersionOriginalMa.Norma != "")
                                    txtLink += VersionOriginalMa.Norma;
                                if (VersionOriginalMa.Numero != null && VersionOriginalMa.Numero != "")
                                    txtLink += " Número " + VersionOriginalMa.Numero;
                                if (VersionOriginalMa.Articulo != null && VersionOriginalMa.Articulo != "")
                                    txtLink += " Artículo " + VersionOriginalMa.Articulo;
                                if (VersionOriginalMa.Inciso != null && VersionOriginalMa.Inciso != "")
                                    txtLink += ", Inciso " + VersionOriginalMa.Inciso;
                                if (VersionOriginalMa.Titulo != null && VersionOriginalMa.Titulo != "")
                                    txtLink += ".- " + VersionOriginalMa.Titulo;
                                if (docBorradorHistorial.Links == null)
                                    docBorradorHistorial.Links = new List<Link>();

                                Link l = new Link();
                                l.Texto = txtLink;
                                l.Tipo = VersionOriginalMa.Norma;
                                l.Url = VersionOriginalMa.IdDocumento;
                                docBorradorHistorial.Links.Add(l);

                                string versionFinalReferencia = FileBo.SerializeXML(docBorradorHistorial);
                                FileBo.setXmlStringToFile(docHistorial.Version.Replace("_borrador.xml", ".xml"), versionFinalReferencia);

                            }
                            Indexador.Solr.cambiaEstadoHistorial(h.id, 1); // finaliza cambio
                        }

                    }
                    #endregion
                    Indexador.Solr.cambiaEstadoDocumento(d.id, 0, ""); // saca estado borrador

                    if (d.Estado == 98)
                        ma.TextoCambio = "(Acepta publicación) " + ma.TextoCambio;
                    if(!ma.VersionFinal)
                        ma.TextoCambio = "(Guardado sin versión) " + ma.TextoCambio;
                    setLog(VersionOriginalMa, ma.TextoCambio);

                }
            }
            catch (BusinessException bx)
            {
                ModelState.AddModelError("", bx.Message);
                return View(ma);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(ma);
            }
            return Redirect("~/Document/Ma_EditarDocumento/" + ma.id);
        }

        private bool EliminaNorma(string id)
        {
            Indexador.Solr.EliminaDocumento(id);
            return true;
        }

        [HttpPost]
        public ActionResult ma_buscar(string q, int p)
        {
            try
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
            catch (BusinessException bx)
            {
                ModelState.AddModelError("", bx.Message);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
            
        }

        [HttpPost]
        public ActionResult Buscar(string q, int p)
        {
            try
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
            catch (BusinessException bx)
            {
                ModelState.AddModelError("", bx.Message);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        [HttpGet]
        public ActionResult Buscar()
        {
            try
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
            catch (BusinessException bx)
            {
                ModelState.AddModelError("", bx.Message);
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        private bool guardaArchivo(string ruta, sgd_documentoEntity doc)
        {
            string xml = FileBo.SerializeXML(doc);
            FileBo.setXmlStringToFile(ruta, xml);
            return true;
        }

        private bool Ma_SendSorl(Documento ma, bool nuevo)
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
            xml += "<field name=\"IdDocumento\">" + ma.IdDocumento + "</field>";
            xml += "<field name=\"Orden\">" + ma.Orden + "</field>";
            xml += "<field name=\"Coleccion\">" + ma.Coleccion + "</field>";
            xml += "<field name=\"Anio\">" + ma.Anio + "</field>";
            xml += "<field name=\"Apendice\">" + ma.Apendice + "</field>";
            xml += "<field name=\"AplicaNorma\">" + ma.AplicaNorma + "</field>";
            xml += "<field name=\"Articulo\">" + ma.Articulo + "</field>";
            xml += "<field name=\"AplicaArticulo\">" + ma.AplicaArticulo + "</field>";
            xml += "<field name=\"Categoria\">" + ma.Categoria + "</field>";
            xml += "<field name=\"Comentario\">" + ma.Comentario + "</field>";
            xml += "<field name=\"Cve\">" + ma.Cve + "</field>";
            xml += "<field name=\"Fecha\">" + ma.Fecha + "</field>";
            xml += "<field name=\"Iddo\">" + ma.Iddo + "</field>";
            xml += "<field name=\"IdRep\">" + ma.IdRep + "</field>";
            xml += "<field name=\"Inciso\">" + ma.Inciso + "</field>";
            xml += "<field name=\"Minred\">" + ma.Minred + "</field>";
            xml += "<field name=\"Nompop\">" + ma.Nompop + "</field>";
            xml += "<field name=\"Norma\">" + ma.Norma + "</field>";
            xml += "<field name=\"Numero\">" + ma.Numero + "</field>";
            xml += "<field name=\"Organismo\">" + ma.Organismo + "</field>";
            xml += "<field name=\"OrgansimoUno\">" + ma.OrganismoUno + "</field>";
            xml += "<field name=\"Regco\">" + ma.Regco + "</field>";
            xml += "<field name=\"Resuel\">" + ma.Resuel + "</field>";
            xml += "<field name=\"Rol\">" + ma.Rol + "</field>";
            xml += "<field name=\"Seccion\">" + ma.Seccion + "</field>";
            xml += "<field name=\"Suborganismo\">" + ma.Suborganismo + "</field>";
            xml += "<field name=\"Tema\">" + ma.Tema + "</field>";
            xml += "<field name=\"Temas\">" + ma.Temas + "</field>";
            xml += "<field name=\"Titulo\">" + ma.Titulo + "</field>";
            xml += "<field name=\"Texto\">" + ma.Texto + "</field>";
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

        private void setLog(Documento doc, string texto)
        {
            string d = "";
            try
            {
                if (doc.Organismo != null && doc.Organismo != "")
                    d += doc.Organismo + ".- ";
                if (doc.Norma != null && doc.Norma != "")
                    d += doc.Norma + ".- ";
                if (doc.Numero != null && doc.Numero != "")
                    d += "Número " + doc.Numero + ".- ";
                if (doc.Articulo != null && doc.Articulo != "")
                    d += "Artículo " + doc.Articulo + ".- ";
                if (doc.Inciso != null && doc.Inciso != "")
                    d += "Inciso " + doc.Inciso + ".- ";
                if (doc.Tribunal != null && doc.Tribunal != "")
                    d += "Tribunal: " + doc.Tribunal + ".- ";
                if (doc.Partes != null && doc.Partes != "")
                    d += "Partes: " + doc.Partes + ".";

                log_documentoEntity log_ = new log_documentoEntity();
                log_.idUser = User.Identity.Name;
                log_.idDocumento = doc.IdDocumento;
                log_.documento = d;
                log_.descripcion = texto;

                LogBO.setLogCreateDoc(log_);
            }
            catch { };
        }
    }
}