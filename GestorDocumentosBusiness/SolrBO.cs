using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using GestorDocumentosEntities;
using mvc4.Business;

namespace mvc4.Business
{
    public class SolrBO
    {
        private static string URL_SOLR = WebConfigurationManager.AppSettings["webSolr"] + "/solr/test-2/update?commitWithin=1000&overwrite=true&wt=json";

        public static string SolrSelect(string texto, int pagina)
        {
            String urlAddress = "";
            string urlSolr = WebConfigurationManager.AppSettings["webSolr"];
            if (texto == "")
                urlAddress = urlSolr + "/solr/test-1/select?hl.fl=ma_texto&hl=on&q=*%3A*&rows=5&start=0";
            else
                urlAddress = urlSolr + "/solr/test-1/select?fl=id%2Cma_iddocumento%2Cma_categoria%2Cma_norma%2Cma_numero%2C%20ma_organismo&hl.fl=ma_texto&hl.simple.post=%3C%2Flabel%3E&hl.simple.pre=%3Clabel%20style%3D%22background-color%3A%20yellow%22%3E&hl=on&q=ma_texto%3A\"" + texto + "\"&rows=5&start=" + pagina + "&wt=json";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
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

        public static bool SolrAdd(string xmlData)
        {
            xmlData = xmlData.Replace("sgd_documento", "SolrXml");
            SolrXml solrXml = new SolrXml();
            solrXml = (SolrXml)FileBo.DeserializeXML(solrXml.GetType(), xmlData);

            string tagPattern = @"<[!--\W*?]*?[/]*?\w+.*?>";
            MatchCollection matches = Regex.Matches(solrXml.Texto, tagPattern);
            foreach (Match match in matches)
            {
                solrXml.Texto = solrXml.Texto.Replace(match.Value, string.Empty).Replace("\n", "");
            }

            string texto = HttpUtility.HtmlDecode(solrXml.Texto);
            string xml = "<add><doc>";
            xml += "<field name=\"id\">" + solrXml.IdDocumento + "</field>";
            xml += "<field name=\"IdDocumento\">" + solrXml.IdDocumento + "</field>";
            xml += "<field name=\"Titulo\">" + solrXml.Titulo + "</field>";
            xml += "<field name=\"Descripcion\">" + solrXml.Descripcion + "</field>";
            xml += "<field name=\"FechaCreacion\">" + solrXml.FechaCreacion + "</field>";
            xml += "<field name=\"Version\">" + solrXml.Version + "</field>";
            xml += "<field name=\"Texto\">" + texto + "</field>";
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

        public static string SolrQueryById(string id)
        {
            string urlSolr = WebConfigurationManager.AppSettings["webSolr"];
            string url = urlSolr + "/ solr/test-1/select?q=id%3A" + id;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
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
