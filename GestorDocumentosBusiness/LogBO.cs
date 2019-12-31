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

        public static List<log_documentoEntity> getListLog()
        {
            try
            {
                return LogDAL.getListLog();
            }
            catch (BusinessException bx)
            {
                throw bx;
            }
        }
    }
}
