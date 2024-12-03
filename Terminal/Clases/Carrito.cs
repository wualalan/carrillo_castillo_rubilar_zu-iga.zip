using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Pasajeros;
using Terminal.ProcesoCompra;

namespace Terminal.Clases
{
    internal class Carrito : Pasajero
    {
        private List<Boleto> boletos = new List<Boleto>();
        private int precioTotal;

        // Propiedades públicas encapsuladas
        public List<Boleto> Boletos
        {
            get => boletos;
            private set => boletos = value;
        }

        public int CantidadBoletos => Boletos.Count;

        public int PrecioTotal
        {
            get => precioTotal;
            private set => precioTotal = value;
        }

        public Carrito(string nombre, TipoPasajero tipo) : base(nombre, tipo)
        {
            // Inicializa la lista de boletos y el precio total
            Boletos = new List<Boleto>();
            PrecioTotal = 0;
        }

        // Métodos
        public void AgregarBoleto(Boleto boleto, int precioBase)
        {
            Boletos.Add(boleto);
            ActualizarPrecioTotal(precioBase);
        }

        public void QuitarBoleto(int numeroAsiento, int precioBase)
        {
            var boleto = Boletos.FirstOrDefault(b => b.NumeroAsiento == numeroAsiento);
            if (boleto != null)
            {
                Boletos.Remove(boleto);
                ActualizarPrecioTotal(precioBase);
            }
        }

        private void ActualizarPrecioTotal(int precioBase)
        {
            PrecioTotal = Boletos.Count * precioBase;
        }

        public void AplicarDescuentos()
        {
            foreach (var boleto in Boletos)
            {
                // Verificar si Pasajero es del tipo correcto
                if (boleto.Pasajero is Pasajero pasajero)
                {
                    switch (pasajero.TipoPasajero)
                    {
                        case TipoPasajero.Estudiante:
                            PrecioTotal = (int)(PrecioTotal * 0.8); // 20% de descuento
                            boleto.Rebaja = true;
                            break;
                        case TipoPasajero.AdultoMayor:
                            PrecioTotal = (int)(PrecioTotal * 0.85); // 15% de descuento
                            boleto.Rebaja = true;
                            break;
                        default:
                            boleto.Rebaja = false;
                            break;
                    }
                }
            }
        }
    }
}