using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terminal
{
    namespace ProcesoCompra
    {
        internal interface iBoleto
        {
            public int NroBoleto { get; set; }
            public bool Rebaja { get; set; }
            public string Origen { get; set; }
            public string Destino { get; set; }
            public DateTime Fecha { get; set; }
            public int NumeroAsiento { get; set; }
        }
    }

}
