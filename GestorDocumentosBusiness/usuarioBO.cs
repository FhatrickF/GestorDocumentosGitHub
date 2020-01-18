using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestorDocumentosDataAccess;
using GestorDocumentosEntities;
using GestorDocumentosExceptions;

namespace GestorDocumentosBusiness
{
    public class usuarioBO
    {
        public static List<usuarioEntity> getListaUsuarios()
        {
            List<usuarioEntity> usuarios = new List<usuarioEntity>();
            try
            {
                usuarios = usuarioDAL.getListaUsuarios();
            }
            catch (TechnicalException ex)
            {
                usuarios = null;
            }
            return usuarios;
        }

        public static usuarioEntity getUserbyName(string name)
        {
            try
            {
                return usuarioDAL.getUserbyName(name);
            }
            catch (BusinessException ex)
            {
                throw new BusinessException(ex.Message);
            }
        }

        public static List<usuarioTabEntity> getListUser()
        {
            try
            {
                List<usuarioTabEntity> listUser = usuarioDAL.getListUser();
                return listUser;
            }
            catch (BusinessException ex)
            {
                throw new BusinessException(ex.Message);
            }
        }

        public static void setRol(string id, string rol)
        {
            try
            {
                usuarioDAL.setRol(id, rol);
            }
            catch (BusinessException ex)
            {
                throw new BusinessException(ex.Message);
            }
        }

        public static usuarioEntity getUserbyId(string id)
        {
            try
            {
                return usuarioDAL.getUserbyId(id);
            }
            catch (BusinessException ex)
            {
                throw new BusinessException(ex.Message);
            }
        }

        public static void deleteById(string id)
        {
            try
            {
                usuarioDAL.deleteById(id);
            }
            catch (BusinessException ex)
            {
                throw new BusinessException(ex.Message);
            }
        }
    }
}
