using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Terminal.Clases.Factura;

namespace Terminal
{
    namespace ProcesoCompra
    {
        internal interface iFactura
        {
            int NumeroFactura { get; }
            DateTime Fecha { get; }
            string NombreCliente { get; }
            string RutCliente { get; }
            decimal MontoTotal { get; }
            IReadOnlyList<ItemFactura> Items { get; }

            void AgregarItem(string descripcion, decimal precio, int cantidad);
            void MostrarFactura();
        }

    }
    
}
