using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terminal
{
    namespace ProcesoCompra
    {
        public enum TipoPago
        {
            Efectivo,
            Débito,   
            Crédito  
        }
        internal class Compra : iCompra
        {
       
            private DateTime fecha;
            private string ubicacion = string.Empty;
            private Servicio servicio ;
            private TipoPago metodoPago;

            public DateTime Fecha { get => fecha; set => fecha = value; }
            public string Ubicacion { get => ubicacion; set => ubicacion = value; }
            public Servicio Servicio { get => servicio; set => servicio = value; }
            public TipoPago MetodoPago { get => metodoPago; set => metodoPago = value; }


        }
    }
}
