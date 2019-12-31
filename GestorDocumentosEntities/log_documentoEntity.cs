using System;
using System.Collections.Generic;
using System.Text;

namespace GestorDocumentosEntities
{
    public class log_documentoEntity
    {
        public int id { get; set; }
        public string idUser { get; set; }
        public string idDocumento { get; set; }
        public string documento { get; set; }
        public string descripcion { get; set; }
    }
}
