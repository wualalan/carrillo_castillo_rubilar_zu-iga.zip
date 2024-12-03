using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Pasajeros;

namespace Terminal
{

    namespace Buses
    {
        internal class Asiento
        {

            private int numero;
            private bool estado;
            private Pasajero pasajero;

            public int Numero { get => numero; set => numero = value; }
            public bool Estado { get => estado; set => estado = value; }
            public Pasajero Pasajero { get => pasajero; set => pasajero = value; }

            public Asiento(int numero, bool estado)
            {
                this.numero = numero;
                this.estado = estado;
            }

           
        }
    }
}
