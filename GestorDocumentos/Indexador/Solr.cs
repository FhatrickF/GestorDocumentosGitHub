using GestorDocumentosExceptions;
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

namespace GestorDocumentos.Indexador
{
    public static class Solr
    {
        private static string directorio_ma = WebConfigurationManager.AppSettings["MVC-DATA"];
        private static string SOLR_URL = WebConfigurationManager.AppSettings["SOLR-URL"];
        private static string SOLR_CORE = WebConfigurationManager.AppSettings["SOLR-CORE"];

        //public static bool setEstadoDocumento(string id, int estado)
        //{
        //    string xml = "";
        //    xml += "<add>";
        //    xml += "<doc>";
        //    xml += "<field name=\"id\">" + id + "</field>";
        //    xml += "<field name=\"Estado\" update=\"set\">" + estado + "</field>";
        //    xml += "</doc>";
        //    xml += "</add>";

        //}

        public static string getResponse(string query)
        {
            return getResponseSolr(query);
        }

        public static List<Historial> getHistorial(string idDocumento)
        {
            List<Historial> historial = new List<Historial>();
            try
            {
                //string query = "select?q=IdOriginal%3A" + idDocumento;
                string query = "select?q=IdOriginal%3A" + idDocumento + "%20AND%20Estado%3A0";
                string response = getHistorialQuery(query);
                if (response == "")
                    throw new Exception();

                var expConverter = new ExpandoObjectConverter();
                dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(response, expConverter);
                foreach (var d in obj.response.docs)
                {
                    Historial h = new Historial();
                    h.id = d.id;
                    h.Tipo = Convert.ToInt32(d.Tipo);
                    h.IdOriginal = d.IdOriginal;
                    h.IdReferencia = d.IdReferencia;
                    historial.Add(h);
                }
            }
            catch (Exception ex)
            {
                historial = null;
            }

            return historial;
        }

        public static Documento getDocumentoById(string id, bool versiones)
        {
            try
            {
                string query = "select?q=id%3A" + id + "%20OR%20IdDocumento%3A" + id;
                Documento doc = new Documento();
                string response = getResponseSolr(query);

                if (response != "")
                {
                    var expConverter = new ExpandoObjectConverter();
                    dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(response, expConverter);
                    string idDocumento = "";
                    string id_ = "";
                    string Coleccion = "";
                    string Origen = "";
                    string Fecha = "";
                    string xml = "";
                    foreach (var doc_ in obj.response.docs)
                    {
                        foreach (var v in doc_.Coleccion)
                        {
                            Coleccion = v;
                        }
                        try
                        {
                            Origen = doc_.Origen;
                        }
                        catch { }
                        id_ = doc_.id;
                        idDocumento = doc_.IdDocumento;
                        Fecha = Convert.ToString(doc_.Fecha);
                        Fecha = Fecha.Replace("/", "-");
                        string[] f = Fecha.Split('-');
                        string o = f[0];
                        if (o.Length == 1)
                        {
                            o = "0" + o;
                            f[0] = o;
                        }
                        o = f[1];
                        if (o.Length == 1)
                        {
                            o = "0" + o;
                            f[1] = o;
                        }

                        Fecha = f[0] + "-" + f[1] + "-" + f[2];
                        //Norma = (doc_.Norma).Replace(" ", "_") + "\\";
                    }
                    if (Origen == "")
                        Origen = Coleccion;
                    else
                        Coleccion = Origen;

                    if (Coleccion != "BITE" && Coleccion != "MA" && Coleccion != "LA")
                    {
                        Fecha = Fecha.Substring(0, 10);
                        string[] f = Fecha.Split('-');
                        Fecha = f[2] + "\\" + f[1] + "\\" + f[0];
                        Coleccion = "DOE\\" + Fecha;
                    }
                    string ruta = directorio_ma + Coleccion + "\\" + idDocumento + ".xml";

                    if (File.Exists(ruta.Replace(".xml", "_borrador.xml")))
                    {
                        xml = System.IO.File.ReadAllText(ruta.Replace(".xml", "_borrador.xml"));
                        doc = (Documento)FileBo.DeserializeXML(doc.GetType(), xml);
                        doc.EsBorrador = true;
                        doc.Version = ruta.Replace(".xml", "_borrador.xml");
                    }
                    else if (System.IO.File.Exists(ruta))
                    {
                        //ruta = directorio_ma + Norma + idDocumento + ".xml";
                        xml = System.IO.File.ReadAllText(ruta);
                        doc = (Documento)FileBo.DeserializeXML(doc.GetType(), xml);
                        doc.EsBorrador = false;
                        doc.Version = ruta;
                    }
                    else
                    {
                        doc = null;
                    }
                    if (doc != null)
                    {
                        if (doc.Origen == null)
                            doc.Origen = Origen;
                        doc.id = id_;
                        if (doc.Seccion == null || doc.Seccion.Length == 0)
                        {
                            doc.Seccion = new string[1];
                            doc.Seccion[0] = "";
                        }
                        if (doc.Suborganismo == null || doc.Suborganismo.Length == 0)
                        {
                            doc.Suborganismo = new string[1];
                            doc.Suborganismo[0] = "";
                        }
                        if (doc.Categoria == null || doc.Categoria.Length == 0)
                        {
                            doc.Categoria = new string[1];
                            doc.Categoria[0] = "";
                        }
                        if (doc.Tema == null || doc.Tema.Length == 0)
                        {
                            doc.Tema = new string[1];
                            doc.Tema[0] = "";
                        }
                        #region links
                        List<Link> links = new List<Link>();
                        if (doc.Links == null || doc.Links.Count == 0)
                        {
                            if (doc.Links == null)
                                doc.Links = new List<Link>();
                            Link link = new Link();
                            link.Tipo = "El documento no contiene links";
                            links.Add(link);
                        }
                        #endregion
                    }
                    if (doc != null && versiones)
                    {
                        if (doc.Versiones == null || doc.Versiones.Count == 0)
                        {
                            if (doc.Versiones == null)
                                doc.Versiones = new List<VersionesDocumento>();
                            VersionesDocumento version = new VersionesDocumento();
                            version.nombre = doc.IdDocumento;
                            version.id = "original";
                            doc.Versiones.Add(version);
                        }
                    }
                }
                else
                {
                    doc = null;
                }

                return doc;
            }
            catch (BusinessException bx)
            {
                throw bx;
            }
            catch (Exception ex)
            {
                new TechnicalException("Error al buscar docuemnto por id en el sorl Method getDocumentoById", ex);
                throw new BusinessException("No es posible buscar el documento por id, volver a intentar más tarde.");
            }
        }

        public static bool cambiaEstadoDocumento(string id, int estado, string usuario)
        {
            string xml = "";
            xml += "<add>";
            xml += "<doc>";
            xml += "<field name=\"id\">" + id + "</field>";
            xml += "<field name=\"Estado\" update=\"set\">" + Convert.ToString(estado) + "</field>";
            xml += "<field name=\"Usuario\" update=\"set\">" + usuario + "</field>";
            xml += "</doc>";
            xml += "</add>";
            sendDocument(xml);
            return true;
        }

        public static bool cambiaEstadoHistorial(string id, int estado)
        {
            string xml = "";
            xml += "<add>";
            xml += "<doc>";
            xml += "<field name=\"id\">" + id + "</field>";
            xml += "<field name=\"Estado\" update=\"set\">" + Convert.ToString(estado) + "</field>";
            xml += "</doc>";
            xml += "</add>";
            sendHistorial(xml);
            return true;
        }

        public static bool sendXmlHistoria(Historial historial)
        {
            string xml = "";
            xml = "<add><doc>";
            xml += "<field name=\"Tipo\">" + historial.Tipo + "</field>";
            xml += "<field name=\"IdOriginal\">" + historial.IdOriginal + "</field>";
            xml += "<field name=\"IdReferencia\">" + historial.IdReferencia + "</field>";
            xml += "<field name=\"Estado\">" + historial.Estado + "</field>";
            xml += "</doc></add>";
            sendHistorial(xml);
            return true;
        }

        public static bool sendXmlDocumento(Documento documento, bool nuevo)
        {
            documento.Texto = Regex.Replace(documento.Texto, "[<].*?>", " ");
            documento.Texto = Regex.Replace(documento.Texto, @"\s+", " ");

            documento.Texto = DecodeHtmlText(documento.Texto);

            string xml = "";

            xml = "<add><doc>";
            if (!nuevo)
            {
                xml += "<field name=\"id\">" + documento.id + "</field>";
            }
            xml += "<field name=\"IdDocumento\">" + documento.IdDocumento + "</field>";
            xml += "<field name=\"Orden\">" + documento.Orden + "</field>";
            if(documento.Coleccion != null && documento.Coleccion.Length > 0)
            {
                for (int i = 0; i < documento.Coleccion.Length; i++)
                {
                    xml += "<field name=\"Coleccion\">" + documento.Coleccion[i] + "</field>";
                }
            }
            xml += "<field name=\"Origen\">" + documento.Origen + "</field>";
            xml += "<field name=\"Anio\">" + documento.Anio + "</field>";
            xml += "<field name=\"Apendice\">" + documento.Apendice + "</field>";
            xml += "<field name=\"AplicaNorma\">" + documento.AplicaNorma + "</field>";
            xml += "<field name=\"Articulo\">" + documento.Articulo + "</field>";
            xml += "<field name=\"AplicaArticulo\">" + documento.AplicaArticulo + "</field>";
            if(documento.Categoria != null && documento.Categoria.Length > 0)
            {
                for (int i = 0; i < documento.Categoria.Length; i++)
                {
                    xml += "<field name=\"Categoria\">" + documento.Categoria[i] + "</field>";
                }
            }           
            xml += "<field name=\"Comentario\">" + documento.Comentario + "</field>";
            xml += "<field name=\"Cve\">" + documento.Cve + "</field>";
            xml += "<field name=\"Fecha\">" + documento.Fecha + "</field>";
            xml += "<field name=\"Iddo\">" + documento.Iddo + "</field>";
            xml += "<field name=\"IdRep\">" + documento.IdRep + "</field>";
            xml += "<field name=\"Inciso\">" + documento.Inciso + "</field>";
            xml += "<field name=\"Minred\">" + documento.Minred + "</field>";
            xml += "<field name=\"Nompop\">" + documento.Nompop + "</field>";
            xml += "<field name=\"Norma\">" + documento.Norma + "</field>";
            xml += "<field name=\"Numero\">" + documento.Numero + "</field>";
            xml += "<field name=\"Organismo\">" + documento.Organismo + "</field>";
            xml += "<field name=\"OrgansimoUno\">" + documento.OrganismoUno + "</field>";
            xml += "<field name=\"Regco\">" + documento.Regco + "</field>";
            xml += "<field name=\"Resuel\">" + documento.Resuel + "</field>";
            xml += "<field name=\"Rol\">" + documento.Rol + "</field>";
            if (documento.Seccion != null && documento.Seccion.Length > 0)
            {
                for (int i = 0; i < documento.Seccion.Length; i++)
                {
                    xml += "<field name=\"Seccion\">" + documento.Seccion[i] + "</field>";
                }
            }            
            xml += "<field name=\"Suborganismo\">" + documento.Suborganismo + "</field>";
            if (documento.Tema != null && documento.Tema.Length > 0)
            {
                for (int i = 0; i < documento.Tema.Length; i++)
                {
                    xml += "<field name=\"Tema\">" + documento.Tema[i] + "</field>";
                }
            }
            xml += "<field name=\"Temas\">" + documento.Temas + "</field>";
            xml += "<field name=\"Titulo\">" + documento.Titulo + "</field>";
            xml += "<field name=\"Texto\">" + documento.Texto + "</field>";
            xml += "</doc></add>";

            if (sendDocument(xml))
                return true;
            else
                return false;            
        }

        public static string getUrlDocumentById(string id)
        {
            string urlSolr = WebConfigurationManager.AppSettings["webSolr"];
            string url = SOLR_URL + SOLR_CORE + "select?q=id%3A" + id + "%20OR%20IdDocumento%3A" + id;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response;
            response = (HttpWebResponse)request.GetResponse();

            string responseStr = "";
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = response.GetResponseStream();
                responseStr = new StreamReader(responseStream).ReadToEnd();

                var expConverter = new ExpandoObjectConverter();
                dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(responseStr, expConverter);

                string idDocumento = "";
                string Norma = "";
                foreach (var doc in obj.response.docs)
                {
                    idDocumento = doc.IdDocumento;
                    Norma = (doc.Norma).Replace(" ", "_") + "\\";
                }
                if (System.IO.File.Exists(directorio_ma + Norma + idDocumento + ".xml"))
                    responseStr = directorio_ma + Norma + idDocumento + ".xml";
                else
                    responseStr = "";
            }
            return responseStr;
        }

        private static string getHistorialQuery(string query)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(SOLR_URL + "Historial-Referencias/" + query);
                HttpWebResponse response;
                response = (HttpWebResponse)request.GetResponse();

                string responseStr = "";
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream responseStream = response.GetResponseStream();
                    responseStr = new StreamReader(responseStream).ReadToEnd();
                }
                return responseStr;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        private static string getResponseSolr(string query)
        {
            try
            {
                query = query.Replace(",", "%2C").Replace(" ", "%20").Replace(":", "%3A").Replace("'", "%22");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(SOLR_URL + SOLR_CORE + query);
                HttpWebResponse response;
                response = (HttpWebResponse)request.GetResponse();

                string responseStr = "";
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream responseStream = response.GetResponseStream();
                    responseStr = new StreamReader(responseStream).ReadToEnd();
                }
                return responseStr;
            }
            catch (Exception ex)
            {
                new TechnicalException("Error al buscar docuemnto en el sorl Method getResponseSolr", ex);
                throw new BusinessException("Error, por favor comunicarse con el administrador.");
            }
        }

        private static bool sendDocument(string xml)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(SOLR_URL + SOLR_CORE + "update?commitWithin=1000&overwrite=true&wt=json");
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
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }

        private static bool sendHistorial(string xml)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(SOLR_URL + "Historial-Referencias/update?commitWithin=1000&overwrite=true&wt=json");
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
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return false;
        }

        private static string DecodeHtmlText(string texto)
        {
            StringWriter myWriter = new StringWriter();
            HttpUtility.HtmlDecode(texto, myWriter);
            return myWriter.ToString();
        }
    }
}