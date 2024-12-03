using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal;

namespace Terminal
{

    namespace Pasajeros
    {
        public enum TipoPasajero
        {
            Normal,
            Estudiante,
            AdultoMayor

        }
        internal class Pasajero : Persona
        {

            private string email = string.Empty;
            private TipoPasajero tipoPasajero;

            public string Email { get => email; set => email = value; }
            public TipoPasajero TipoPasajero { get => tipoPasajero; set => tipoPasajero = value; }

            public Pasajero() { }
            public Pasajero(string nombre, TipoPasajero tipo)
            {
                // Asumiendo que la clase Persona tiene una propiedad Nombre
                this.Nombre = nombre; // Asegúrate de que la clase Persona tenga esta propiedad
                this.tipoPasajero = tipo;
            }
        }
    }
}
