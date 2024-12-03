using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Buses;

namespace Terminal
{
    namespace Personal
    {
        internal class Chofer : Persona
        {
            private DateTime horario;
            private Bus bus;
            public DateTime Horario { get => horario; set => horario = value; }
            public Bus Bus { get => bus; set => bus = value; }
        }
    }
    
}
