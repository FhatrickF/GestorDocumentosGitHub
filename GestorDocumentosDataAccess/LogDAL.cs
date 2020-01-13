using GestorDocumentosDataAccess.Modelo;
using GestorDocumentosEntities;
using GestorDocumentosExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorDocumentosDataAccess
{
    public class LogDAL
    {
        public static void setLogCreateDoc(log_documentoEntity log_)
        {
            try
            {
                using (infoEntities db = new infoEntities())
                {
                    Log_Documento _Documento = new Log_Documento();
                    _Documento.idDocumento = log_.idDocumento;
                    _Documento.idUser = log_.idUser;
                    _Documento.logDescripcion = log_.descripcion;
                    _Documento.logDocumento = log_.documento;
                    _Documento.logFecha = DateTime.Now;
                    db.Log_Documento.Add(_Documento);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                new TechnicalException("Error.- No se pudo guardar la acción en el log. ", ex);
                throw new BusinessException("error");
            }
        }

        public static List<log_documentoEntity> getHistorialDocumento(string Id)
        {
            List<log_documentoEntity> historial = new List<log_documentoEntity>();
            try
            {
                using (infoEntities db = new infoEntities())
                {
                    List<Log_Documento> log = db.Log_Documento.Where(x => x.idDocumento == Id).OrderBy(x => x.logFecha).ToList();
                    foreach (var log_ in log)
                    {
                        log_documentoEntity _Documento = new log_documentoEntity();
                        _Documento.id = log_.id;
                        _Documento.idDocumento = log_.idDocumento;
                        _Documento.idUser = log_.idUser;
                        _Documento.documento = log_.logDocumento;
                        _Documento.descripcion = log_.logDescripcion;                        
                        _Documento.hora = log_.logFecha.ToString();
                        historial.Add(_Documento);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new TechnicalException("No se pudo recuperar historial del documento. Id = " + Id, ex);
            }
            return historial;
        }

        public static List<log_documentoEntity> getListLog(string fecha)
        {
            try
            {
                DateTime inicio = new DateTime();
                DateTime final = new DateTime();

                if (fecha == "")
                    fecha = DateTime.Now.ToString("yyyy-MM-dd");

                inicio = Convert.ToDateTime(fecha + " 00:00:00");
                final = Convert.ToDateTime(fecha + " 23:59:59");

                List<log_documentoEntity> logs = new List<log_documentoEntity>();
                using (infoEntities db = new infoEntities())
                {
                    List<Log_Documento> log = db.Log_Documento.Where(x => x.logFecha >= inicio && x.logFecha <= final).OrderBy(x => x.logFecha).ToList();
                    foreach (var log_ in log)
                    {
                        log_documentoEntity _Documento = new log_documentoEntity();
                        _Documento.id = log_.id;
                        _Documento.idDocumento = log_.idDocumento;
                        _Documento.idUser = log_.idUser;
                        _Documento.documento = log_.logDocumento;
                        _Documento.descripcion = log_.logDescripcion;
                        string[] hora = (log_.logFecha.ToString()).Split(' ');
                        _Documento.hora = hora[1];
                        logs.Add(_Documento);
                    }
                }
                return logs;
            }
            catch (Exception ex)
            {
                new TechnicalException("Error.- No se pudo obtener la lista de acciones en el log. ", ex);
                throw new BusinessException("Error.- No se pudo obtener la lista de acciones en el log");
            }
        }
    }
}
