using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terminal
{
    namespace Buses
    {
        internal class Horario
        {
            private DateTime horaSalida;
            private DateTime horaLLegada;

            public DateTime HoraSalida { get => horaSalida; set => horaSalida = value; }
            public DateTime HoraLLegada { get => horaLLegada; set => horaLLegada = value; }
        }
    }
}
