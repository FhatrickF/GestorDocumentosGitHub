using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorDocumentosExceptions
{
    [Serializable]
    public class BusinessException : ApplicationException
    {
        public BusinessException(string mensaje, Exception original) : base(mensaje, original) { }

        public BusinessException(string mensaje) : base(mensaje) { }
    }
}
