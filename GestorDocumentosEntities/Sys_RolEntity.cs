using System;
using System.Collections.Generic;
using System.Text;

namespace GestorDocumentosEntities
{
    public partial class Sys_RolEntity
    {
        public const string ADMINISTRADOR = "ec8e5619-329b-4314-a8e4-97f8ee3360a7";
        public const string EDITOR = "f3005930-bc3d-4544-b7d5-94377924d283";

        public static string getRol(string id)
        {
            string tipo = string.Empty;
            switch (id)
            {
                case Sys_RolEntity.ADMINISTRADOR: tipo = "Administrador"; break;
                case Sys_RolEntity.EDITOR: tipo = "Editor"; break;
            }
            return tipo;
        }
    }
}
