    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace Terminal.Clases
    {
        internal class Factura
        {
            private int numeroFactura;
            private DateTime fecha;
            private string nombreCliente;
            private string rutCliente;
            private decimal montoTotal;
            private List<ItemFactura> items;

            public int NumeroFactura1 { get => numeroFactura; set => numeroFactura = value; }
            public DateTime Fecha1 { get => fecha; set => fecha = value; }
            public string NombreCliente1 { get => nombreCliente; set => nombreCliente = value; }
            public string RutCliente1 { get => rutCliente; set => rutCliente = value; }
            public decimal MontoTotal1 { get => montoTotal; set => montoTotal = value; }
            public List<ItemFactura> Items1 { get => items; set => items = value; }


            public Factura(int numeroFactura, string nombreCliente, string rutCliente)
            {
                numeroFactura = numeroFactura;
                nombreCliente = nombreCliente;
                rutCliente = rutCliente;
                fecha = DateTime.Now; // Fecha actual
                items = new List<ItemFactura>();
                montoTotal = 0;
            }

            public void AgregarItem(string descripcion, decimal precio, int cantidad)
            {
                var item = new ItemFactura
                {
                    Descripcion = descripcion,
                    Precio = precio,
                    Cantidad = cantidad,
                    Total = precio * cantidad
                };

                items.Add(item);
                montoTotal += item.Total; // Actualiza el monto total
            }
            public void MostrarFactura()
            {
                Console.WriteLine($"Factura N°: {numeroFactura}");
                Console.WriteLine($"Fecha: {fecha}");
                Console.WriteLine($"Cliente: {nombreCliente} (RUT: {rutCliente})");
                Console.WriteLine("Ítems:");

                foreach (var item in items)
                {
                    Console.WriteLine($"- {item.Descripcion}: {item.Cantidad} x {item.Precio:C} = {item.Total:C}");
                }

                Console.WriteLine($"Monto Total: {montoTotal:C}");
            }

            public class ItemFactura
            {
                private string descripcion;
                private decimal precio;
                private int cantidad;
                private decimal total;

                public string Descripcion { get => descripcion; set => descripcion = value; }
                public decimal Precio { get => precio; set => precio = value; }
                public int Cantidad { get => cantidad; set => cantidad = value; }
                public decimal Total { get => total; set => total = value; }
            }
        }

    }
