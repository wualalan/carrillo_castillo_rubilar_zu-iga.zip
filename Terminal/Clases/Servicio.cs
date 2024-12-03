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
       
        internal class Servicio : iServicio
        {
            private int id;
            private DateTime fecha;
            private string origen;
            private string destino;
            private Bus bus;

            public int Id { get => id; set => id = value; }
            public DateTime Fecha { get => fecha; set => fecha = value; }
            public string Origen { get => origen; set => origen = value; }
            public string Destino { get => destino; set => destino = value; }
            public Bus Bus { get => bus; set => bus = value; }

            public Servicio(int id, string origen, string destino, DateTime fecha)
            {
                this.id = id;
                this.origen = origen;
                this.destino = destino;
                this.fecha = fecha;
            }
            public Servicio() { }
        }
    }
}
