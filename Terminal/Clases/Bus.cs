    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Terminal.Personal;
    using Terminal.ProcesoCompra;

    namespace Terminal
    {
       namespace Buses
       {
            internal class Bus
            {
                private static int _contadorIds = 1;
                private int id;
                private List<Asiento> asientos = new List<Asiento>();
                private Chofer chofer;
                private Auxiliar auxiliar;
                public int Id { get => id; set => id = value; }
                public List<Asiento> Asientos { get => asientos; set => asientos = value; }
                public Chofer Chofer { get => chofer; set => chofer = value; }
                public Auxiliar Auxiliar { get => auxiliar; set => auxiliar = value; }

                public Bus()
                {
                    // Asignar un ID único al bus
                    id = _contadorIds++;

                    // Inicializar los 40 asientos
                    for (int i = 0; i < 40; i++)
                    {
                        // Crear un nuevo asiento e agregarlo a la lista
                        Asientos.Add(new Asiento(i+1,false));
                    }
                }
            }
        }


        }

