using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Buses;

namespace Terminal
{
    namespace ProcesoCompra
    {
        internal interface iServicio
        {
            public int Id { get; set; }
            public DateTime Fecha { get; set; }
            public string Origen { get; set; }
            public string Destino { get; set; }
            public Bus Bus { get; set; }
        }
    }
}
