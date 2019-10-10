using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestorDocumentosDataAccess.Modelo;
using GestorDocumentosEntities;

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
                throw new Exception("No es posible encontrar el rol del usuario");
            }
        }
    }
}
