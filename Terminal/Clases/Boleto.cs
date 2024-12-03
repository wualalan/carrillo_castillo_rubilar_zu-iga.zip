    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Terminal.Pasajeros;

    namespace Terminal
    {
        namespace ProcesoCompra
        {
            internal class Boleto : iBoleto
            {

                private int nroBoleto;
                private string origen;
                private string destino;
                private DateTime fecha;
                private bool rebaja;
                private int numeroAsiento;
                private Pasajero pasajero;
                private TipoPago metodoPago;
                private int precio;
                

                public int NroBoleto { get => nroBoleto; set => nroBoleto = value; }
                public bool Rebaja { get => rebaja; set => rebaja = value; }
                public string Origen { get => origen; set => origen = value; }
                public string Destino { get => destino; set => destino = value; }
                public DateTime Fecha { get => fecha; set => fecha = value; }
                public int NumeroAsiento { get => numeroAsiento; set => numeroAsiento = value; }
                
                
                public TipoPago MetodoPago { get => metodoPago; set => metodoPago = value; }
                public int Precio { get => precio; set => precio = value; }
                internal Pasajero Pasajero { get => pasajero; set => pasajero = value; }

                public void verificarRebaja(TipoPasajero tipo)
                {
                        if(tipo != TipoPasajero.Normal)
                        {
                            rebaja = true;
                        }
                }
            }



        }
    }
