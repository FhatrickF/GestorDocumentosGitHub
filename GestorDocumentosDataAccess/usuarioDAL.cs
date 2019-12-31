using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestorDocumentosDataAccess.Modelo;
using GestorDocumentosEntities;
using GestorDocumentosExceptions;

namespace GestorDocumentosDataAccess
{
    public class usuarioDAL
    {
        public static usuarioEntity getUserbyName(string name)
        {
            try
            {
                usuarioEntity us = new usuarioEntity();
                using (infoEntities db = new infoEntities())
                {
                    AspNetUsers aspUs = db.AspNetUsers.Where(x => x.Email == name).FirstOrDefault();
                    if(aspUs != null)
                    {
                        us.Id = aspUs.Id;
                        us.Email = aspUs.Email;
                        us.UserName = aspUs.UserName;
                        us.Rol = aspUs.Rol;
                        return us;
                    }
                    else
                    {
                        throw new Exception("No es posible encontrar el rol del usuario");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception ("No es posible encontrar el rol del usuario");
            }
        }

        public static void setRol(string id, string rol)
        {
            try
            {
                using (infoEntities db = new infoEntities())
                {
                    AspNetUsers aspUs = db.AspNetUsers.Where(x => x.Id == id).FirstOrDefault();
                    if (aspUs == null)
                        throw new BusinessException("El usuario no existe");
                    else
                    {
                        aspUs.Rol = rol.Trim();
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                new TechnicalException("No es posible agregar el rol al usuario", ex);
                throw new BusinessException("No es posible agregar el rol al usuario");
            }
        }

        public static usuarioEntity getUserbyId(string id)
        {
            try
            {
                usuarioEntity us = new usuarioEntity();
                using (infoEntities db = new infoEntities())
                {
                    AspNetUsers aspUs = db.AspNetUsers.Where(x => x.Id == id).FirstOrDefault();
                    if (aspUs != null)
                    {
                        us.Email = aspUs.Email;
                        us.UserName = aspUs.UserName;
                        us.Rol = aspUs.Rol;
                        return us;
                    }
                    else
                    {
                        throw new BusinessException("No es posible encontrar el usuario");
                    }
                }
            }
            catch (Exception ex)
            {
                new TechnicalException("No es posible encontrar el usuario", ex);
                throw new BusinessException("No es posible encontrar el usuario");
            }
        }

        public static void deleteById(string id)
        {
            try
            {
                using (infoEntities db = new infoEntities())
                {
                    AspNetUsers aspUs = db.AspNetUsers.Where(x => x.Id == id.ToString()).FirstOrDefault();
                    if (aspUs == null)
                        throw new BusinessException("No es posible encontrar el id del usuario");
                    else
                    {
                        db.AspNetUsers.Remove(aspUs);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                new TechnicalException("No es posible eliminar el usuario", ex);
                throw new BusinessException("No es posible eliminar el usuario");
            }
        }

        public static List<usuarioTabEntity> getListUser()
        {
            try
            {
                List<usuarioTabEntity> listUser = new List<usuarioTabEntity>();
                using (infoEntities db = new infoEntities())
                {
                    List<AspNetUsers> aspUs = db.AspNetUsers.ToList();
                    foreach (var list in aspUs)
                    {
                        usuarioTabEntity us = new usuarioTabEntity();
                        us.Email = list.Email;
                        us.Id = list.Id;
                        us.Rol = list.Rol;
                        us.UserName = list.UserName;

                        listUser.Add(us);
                    }
                }
                return listUser;
            }
            catch (Exception ex)
            {
                new TechnicalException("No es posible encontrar la lista de usuario usuarios", ex);
                throw new BusinessException("No es posible encontrar la lista de usuarios");
            }
        }
    }
}
