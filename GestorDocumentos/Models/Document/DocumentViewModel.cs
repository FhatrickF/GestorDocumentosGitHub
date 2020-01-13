using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace mvc4.Models
{
    public class DocumentoViewModel
    {
        public int IdDocumento { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public int? Version { get; set; }
        public bool? VersionFinal { get; set; }
        public bool EsBorrador { get; set; }
        public string Texto { get; set; }
    }

    public class NuevoDocumentViewModel
    {
        public int? IdDocumento { get; set; }
        [Required]
        public string Titulo { get; set; }
        [Required]
        public string Descripcion { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public int Version { get; set; }
        public bool VersionFinal { get; set; }
        public bool EsBorrador { get; set; }
        [AllowHtml]
        public string Texto { get; set; }
    }

    public class VersionesDocumento
    {
        public string id { get; set; }
        public string nombre { get; set; }
    }

    public class DetalleDocumento
    {
        public NuevoDocumentViewModel Document { get; set; }
        public List<VersionesDocumento> ListaVersiones { get; set; }
        public string TextoCambio { get; set; }
    }

    public class MedioAmbiental
    {
        public string id { get; set; }
        public string IdDocumento { get; set; }
        public string Seccion { get; set; }
        public DateTime Fecha { get; set; }
        public string Numero { get; set; }
        public string Norma { get; set; }
        [Required]
        public string Organismo { get; set; }
        public string SubOrganismo { get; set; }
        public string Categoria { get; set; }
        public string Tema { get; set; }
        [Required]
        public string Titulo { get; set; }
        [AllowHtml]
        [Required]
        public string Texto { get; set; }
        public bool EsBorrador { get; set; }
        public List<VersionesDocumento> Versiones { get; set; }
    }

    public class Listado
    {
        public List<Detalle> Detalles { get; set; }
    }

    public class Detalle
    {
        public string Nombre { get; set; }
        public bool Seleccionado { get; set; }
    }

    public class Documento
    {
        private string _Coleccion = "";
        private string _Categoria = "";
        private string _Seccion = "";
        private string _Tema = "";

        public string id { get; set; }
        public string IdDocumento { get; set; }
        public int Orden { get; set; }
        public string Origen { get; set; }
        public string[] Coleccion { get; set; }
        [XmlIgnoreAttribute]
        public String ColeccionGlosa
        {
            get
            {
                if (Coleccion != null)
                {
                    _Coleccion = "";
                    for (int x = 0; x < Coleccion.Length; x++)
                    {
                        _Coleccion = _Coleccion + Coleccion[x] + ", ";
                    }
                    _Coleccion = _Coleccion.Trim(',').Trim();
                    _Coleccion = _Coleccion.Substring(0, _Coleccion.Length - 1);
                }
                return _Coleccion;
            }
            set
            {
                _Coleccion = value;
            }
        }
        public int Anio { get; set; }
        public string AnioC { get; set; }
        public string Apendice { get; set; }
        public string Articulo { get; set; }
        public string AplicaArticulo { get; set; }
        public string[] Categoria { get; set; }
        [XmlIgnoreAttribute]
        public string CategoriaGlosa
        {
            get
            {
                if (Categoria != null && Categoria[0] != "")
                {
                    _Categoria = "";
                    for (int x = 0; x < Categoria.Length; x++)
                    {
                        _Categoria = _Categoria + Categoria[x] + ", ";
                    }
                    _Categoria = _Categoria.Trim(',').Trim();
                    _Categoria = _Categoria.Substring(0, _Categoria.Length - 1);
                }
                return _Categoria;
            }
            set
            {
                _Categoria = value;
            }
        }
        public string Comentario { get; set; }
        public string Cve { get; set; }
        public string Fecha { get; set; }
        public string Iddo { get; set; }
        public string IdRep { get; set; }
        public string Inciso { get; set; }
        public string Minred { get; set; }
        public string Nompop { get; set; }
        [Required]
        public string Norma { get; set; }
        public int IdTipoNorma { get; set; }
        public string AplicaNorma { get; set; }
        public string Numero { get; set; }
        public string AplicaNumero { get; set; }
        public string Organismo { get; set; }
        public int IdOrganismo { get; set; }
        public string OrganismoUno { get; set; }
        public string Regco { get; set; }
        public string Resuel { get; set; }
        public string Rol { get; set; }
        public string[] Seccion { get; set; }
        [XmlIgnoreAttribute]
        public string SeccionGlosa
        {
            get
            {
                if (Seccion != null && Seccion[0] != "")
                {
                    _Seccion = "";
                    for (int x = 0; x < Seccion.Length; x++)
                    {
                        _Seccion = _Seccion + Seccion[x] + ", ";
                    }
                    _Seccion = _Seccion.Trim(',').Trim();
                    _Seccion = _Seccion.Substring(0, _Seccion.Length - 1);
                }
                return _Seccion;
            }
            set
            {
                _Seccion = value;
            }
        }
        public string[] Suborganismo { get; set; }
        public int IdSubOrganismo { get; set; }
        public string[] Tema { get; set; }
        [XmlIgnoreAttribute]
        public string TemaGlosa
        {
            get
            {
                if (Tema != null && Tema[0] != "")
                {
                    _Tema = "";
                    for (int x = 0; x < Tema.Length; x++)
                    {
                        _Tema = _Tema + Tema[x] + ", ";
                    }
                    _Tema = _Tema.Trim(',').Trim();
                    _Tema = _Tema.Substring(0, _Tema.Length - 1);
                }
                return _Tema;
            }
            set
            {
                _Tema = value;
            }
        }
        public string Temas { get; set; }
        [Required]
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public Nullable<System.DateTime> FechaCreacion { get; set; }
        public string Version { get; set; }
        public bool VersionFinal { get; set; }
        public bool EsBorrador { get; set; }
        public bool Publicar { get; set; }
        public int Estado { get; set; }
        public string Usuario { get; set; }
        public string TextoCambio { get; set; }
        [AllowHtml]
        [Required]
        public string Texto { get; set; }
        public List<VersionesDocumento> Versiones { get; set; }
        public List<Link> Links { get; set; }
    }

    public class Link
    {
        public string Tipo { get; set; }
        public string Texto { get; set; }
        public string Url { get; set; }
    }

    public class FormularioBusqueda
    {
        public string Borrador { get; set; }
        public string Pendiente { get; set; }
        public string Ct { get; set; }
        public string Lr { get; set; }
        public string Lt { get; set; }
        public string Lzf { get; set; }
        public string Liva { get; set; }
        public string Circular { get; set; }
        public string Decreto { get; set; }
        public string Dfl { get; set; }
        public string Dl { get; set; }
        public string Ds { get; set; }
        public string Ley { get; set; }
        public string Resolucion { get; set; }
        public string Fecha { get; set; }
        public string Numero { get; set; }
        public string Articulo { get; set; }
        public string Inciso { get; set; }
        public string Texto { get; set; }
        public string Coleccion { get; set; }
        public int Pagina { get; set; }
    }

    public class Nota
    {
        [AllowHtml]
        public string TextoNota { get; set; }
        public string Coleccion { get; set; }
        public int Result { get; set; }
    }

    public class Historial
    {
        public string id { get; set; }
        public int Tipo { get; set; }
        public string IdOriginal { get; set; }
        public string IdReferencia { get; set; }
        public int Estado { get; set; }
    }
}