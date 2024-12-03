using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terminal
{
    namespace ProcesoCompra
    {
        internal interface iCompra
        {
            public DateTime Fecha { get; set; }
            public string Ubicacion { get; set; }
            public Servicio Servicio { get; set; }
        }
    }


}
