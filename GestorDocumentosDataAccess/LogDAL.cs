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

        public static List<log_documentoEntity> getListLog()
        {
            try
            {

                List<log_documentoEntity> logs = new List<log_documentoEntity>();
                using (infoEntities db = new infoEntities())
                {
                    foreach (var log_ in db.Log_Documento.ToList())
                    {
                        log_documentoEntity _Documento = new log_documentoEntity();
                        _Documento.id = log_.id;
                        _Documento.idDocumento = log_.idDocumento;
                        _Documento.idUser = log_.idUser;
                        _Documento.documento = log_.logDocumento;
                        _Documento.descripcion = log_.logDescripcion;
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
