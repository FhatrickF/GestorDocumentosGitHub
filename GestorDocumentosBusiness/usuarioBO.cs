using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestorDocumentosDataAccess;
using GestorDocumentosEntities;

namespace GestorDocumentosBusiness
{
    public class usuarioBO
    {
        public static usuarioEntity getUserbyName(string name)
        {
            try
            {
                return usuarioDAL.getUserbyName(name);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static List<usuarioTabEntity> getListUser()
        {
            try
            {
                List<usuarioTabEntity> listUser = usuarioDAL.getListUser();
                return listUser;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
