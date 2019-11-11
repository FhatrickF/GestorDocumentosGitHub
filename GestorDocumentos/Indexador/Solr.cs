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
using System.Web;
using System.Web.Configuration;

namespace GestorDocumentos.Indexador
{
    public static class Solr
    {
        private static string directorio_ma = WebConfigurationManager.AppSettings["MVC-DATA-MA"];

        public static Documento getDocumentoById(string id)
        {
            string urlSolr = WebConfigurationManager.AppSettings["webSolr"];
            string query = urlSolr + "/solr/test-1/select?q=id%3A" + id + "%20OR%20IdDocumento%3A" + id;
            Documento doc = new Documento();
            string response = getResponseSolr(query);
            
            if(response != "")
            {
                var expConverter = new ExpandoObjectConverter();
                dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(response, expConverter);
                string idDocumento = "";
                string Norma = "";
                string xml = "";
                foreach (var doc_ in obj.response.docs)
                {
                    idDocumento = doc_.IdDocumento;
                    Norma = (doc_.Norma).Replace(" ", "_") + "\\";
                }
                string ruta = directorio_ma + Norma + idDocumento + ".xml";
                if (System.IO.File.Exists(ruta))
                {
                    ruta = directorio_ma + Norma + idDocumento + ".xml";
                    xml = System.IO.File.ReadAllText(ruta);
                    doc = (Documento)FileBo.DeserializeXML(doc.GetType(), xml);
                }
                else
                {
                    doc = null;
                }
            }
            else
            {
                doc = null;
            }
                
            return doc;
        }

        private static string getResponseSolr(string query)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(query);
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
    }
}