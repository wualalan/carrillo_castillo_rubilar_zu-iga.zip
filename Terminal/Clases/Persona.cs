using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terminal
{
    internal class Persona
    {
        private string rut = string.Empty;
        private string nombre = string.Empty;
        private string telefono;

        public string RUT { get => rut; set => rut = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public string Telefono { get => telefono; set => telefono = value; }
    }
}
