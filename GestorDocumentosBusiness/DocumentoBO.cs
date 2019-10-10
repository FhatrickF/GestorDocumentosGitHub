using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestorDocumentosDataAccess;
using GestorDocumentosEntities;

namespace GestorDocumentosBusiness
{
    public class DocumentoBO
    {
        public static bool setDocumento(sgd_documentoEntity nuevo)
        {
            try
            {

                return DocumentoDAL.setDocumento(nuevo);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static sgd_documentoEntity getDocumentoById(int id)
        {
            try
            {
                return DocumentoDAL.getDocumentoById(id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al Buscar id del documento");
            }
        }

        public static void setSaveDocumento(sgd_documentoEntity documento)
        {
            try
            {
                DocumentoDAL.setSaveDocumento(documento);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static void deleteById(int id)
        {
            try
            {
                DocumentoDAL.deleteById(id);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static int setDocumentoMA(sgd_documentoEntity nuevo)
        {
            try
            {
                return DocumentoDAL.setDocumentoMA(nuevo);
            }
            catch (Exception)
            {

                throw new Exception("No se puede crear el archivo");
            }
        }
    }
}
