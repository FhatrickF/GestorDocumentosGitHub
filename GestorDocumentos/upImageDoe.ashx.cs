using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using mvc4.Business;

namespace GestorDocumentos
{
    /// <summary>
    /// Descripción breve de upImageDoe
    /// </summary>
    public class upImageDoe : IHttpHandler
    {
        private string directorio_imagenes = WebConfigurationManager.AppSettings["MVC-IMAGENES"];

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            try
            {
                //string dirFullPath = HttpContext.Current.Server.MapPath("~/Content/img/");
                //string[] files;
                //int numFiles;
                //files = System.IO.Directory.GetFiles(dirFullPath);
                //numFiles = files.Length;
                //numFiles = numFiles + 1;
                string str_image = "";

                string[] fecha = (context.Request["Fecha"]).Split('-');
                string ruta = directorio_imagenes + fecha[0] + "\\" + fecha[1] + "\\" + fecha[2] + "\\";

                if (!Directory.Exists(ruta))
                    Directory.CreateDirectory(ruta);

                foreach (string s in context.Request.Files)
                {
                    HttpPostedFile file = context.Request.Files[s];
                    string fileName = file.FileName;
                    string fileExtension = file.ContentType;

                    if (!string.IsNullOrEmpty(fileName))
                    {
                        fileExtension = Path.GetExtension(fileName);
                        str_image = GestorDocumentosBusiness.UtilesBO.getMd5(DateTime.Now.ToString() + fileName);
                        string imagen = str_image + fileExtension;
                        str_image = fecha[0] + fecha[1] + fecha[2] + str_image + fileExtension.Replace(".", "_");
                        
                        //str_image = "MyPHOTO_" + numFiles.ToString() + fileExtension;
                        string pathToSave_100 = ruta + imagen;
                        file.SaveAs(pathToSave_100);
                    }
                }
                //  database record update logic here  ()

                context.Response.Write(str_image);
            }
            catch (Exception ac)
            {

            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}