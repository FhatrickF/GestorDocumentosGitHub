using GestorDocumentosDataAccess;
using GestorDocumentosEntities;
using GestorDocumentosExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorDocumentosBusiness
{
    public class LogBO
    {
        public static bool setLogCreateDoc(log_documentoEntity log_)
        {
            try
            {
                LogDAL.setLogCreateDoc(log_);
                return true;
            }
            catch (BusinessException bx)
            {
                return false;
            }
        }

        public static List<log_documentoEntity> getHistorialDocumento(string Id)
        {
            List<log_documentoEntity> historial = new List<log_documentoEntity>();
            try
            {
                historial = LogDAL.getHistorialDocumento(Id);
            }
            catch (TechnicalException tex)
            {
                throw new BusinessException("Ocurrió un error al recuperar historial del documento.");
            }
            return historial;
        }

        public static List<log_documentoEntity> getListLog(string fecha)
        {
            try
            {
                return LogDAL.getListLog(fecha);
            }
            catch (BusinessException bx)
            {
                throw bx;
            }
        }
    }
}
