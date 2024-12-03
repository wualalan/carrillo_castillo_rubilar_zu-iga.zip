using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text.Json.Nodes;
using Terminal.Buses;
using Terminal.ProcesoCompra;
using Tmds.DBus.Protocol;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.IO;
using Avalonia.Data.Converters;
using System.Globalization;
using Terminal.Pasajeros;
using Avalonia.Controls.Documents;
using Avalonia.Media.Imaging;
using Terminal.Personal;
using Avalonia;
using Terminal.Clases;
namespace Terminal
{
    public partial class MainWindow : Window
    {

        List<Servicio> serviciosBus = new List<Servicio>();
        Servicio servicioSeleccionado;
        List<Servicio>serviciosSeleccionados=new List<Servicio>();
        List<Asiento> asientos; // Lista de asientos
        List<int>asientosSeleccionados = new List<int>() ; // Asientos seleccionados
        Pasajero pasajero = new Pasajero();
        Boleto boleto = new Boleto();
        int precioViaje = 7000;
        List<Boleto>boletos = new List<Boleto>();
        private Carrito carrito;


        public MainWindow()
        {
            InitializeComponent();
            
            datepicker.DisplayDateStart = DateTime.Today;
            ingresarFecha.DisplayDateStart = DateTime.Today;
            carrito = new Carrito("Nombre del Pasajero", TipoPasajero.Normal);


            var btnIngresarServicio = this.FindControl<Button>("ingresarServicio");
            btnIngresarServicio.Click += agregarServicio;

            

            //Hay que agregar una carpeta con el json al proyecto
            string path = Path.Combine(AppContext.BaseDirectory, "json", "Terminal.json"); ;
            string json = File.ReadAllText(path);
            serviciosBus = JsonSerializer.Deserialize<List<Servicio>>(json);


            string rutaJson  = Path.Combine(AppContext.BaseDirectory, "json", "compras.json");;

           // Leer el JSON existente
            if (File.Exists(rutaJson))
            {
                string contenidoJson = File.ReadAllText(rutaJson);
                boletos = JsonSerializer.Deserialize<List<Boleto>>(contenidoJson) ?? new List<Boleto>();
            }
            else
            {
                boletos = new List<Boleto>();
            }


            

        }

        private void MostrarResumen()
        {
            // Calcular el precio total basado en el tipo de pasajero
            int totalPasaje = asientosSeleccionados.Count * precioViaje;
            int descuento = 0;

            // Aplicar descuentos según el tipo de pasajero
            if (pasajero.TipoPasajero == TipoPasajero.Estudiante)
            {
                descuento = 2000; // Descuento para estudiantes
            }
            else if (pasajero.TipoPasajero == TipoPasajero.AdultoMayor)
            {
                descuento = 3000; // Descuento para adultos mayores
            }

            int totalConDescuento = totalPasaje - (descuento * asientosSeleccionados.Count);

            string resumen = $"Total de asientos seleccionados: {asientosSeleccionados.Count}\n" +
                             $"Total a pagar: {totalConDescuento} CLP\n" +
                             $"Detalles de los boletos:\n";

            foreach (var numeroAsiento in asientosSeleccionados)
            {
                int precioConDescuento = precioViaje - descuento;
                resumen += $"- Asiento: {numeroAsiento}, Precio: {precioViaje - descuento}\n"; // Mostrar precio con descuento
            }

            MessageText.Text = resumen;
        }

        private void LimpiarSeleccionAsientos()
        {
            asientosSeleccionados.Clear(); // Limpia la lista de asientos seleccionados

            // Actualiza la interfaz de usuario para reflejar que no hay asientos seleccionados
            foreach (var child in AsientosGrid.Children)
            {
                if (child is Button botonAsiento)
                {
                    botonAsiento.Background = Brushes.Green; // Cambia el color de fondo a verde (disponible)
                    botonAsiento.IsEnabled = true; // Asegúra de que los botones estén habilitados
                }
            }

            MessageText.Text = "Selección de asientos limpiada."; // Mensaje opcional de confirmación
        }

        private void GuardarBoletos()
        {
            try
            {
                // Serializa la lista de boletos a un string JSON
                string nuevoJson = JsonSerializer.Serialize(boletos, new JsonSerializerOptions { WriteIndented = true });

                // Define la ruta donde se guardará el archivo JSON
                string rutaJson = Path.Combine(AppContext.BaseDirectory, "json", "compras.json");

                // Escribe el JSON en el archivo
                File.WriteAllText(rutaJson, nuevoJson);
            }
            catch (Exception ex)
            {
                MessageText.Text = $"Error al guardar los boletos: {ex.Message}";
            }
        }
        private void ProcesarCompra()
        {
            if (asientosSeleccionados.Count == 0)
            {
                MessageText.Text = "Seleccione al menos un asiento.";
                return;
            }

            TipoPago metodoPagoSeleccionado;
            if (comboBoxMetodoPago.SelectedItem is ComboBoxItem selectedItem)
            {
                string metodoPago = selectedItem.Content.ToString();
                try
                {
                    metodoPagoSeleccionado = (TipoPago)Enum.Parse(typeof(TipoPago), selectedItem.Content.ToString());
                }
                catch (ArgumentException)
                {
                    MessageText.Text = $"Método de pago '{metodoPago}' no encontrado.";
                    return;
                }
            }
            else
            {
                MessageText.Text = "Seleccione un método de pago.";
                return;
            }

            foreach (var numeroAsiento in asientosSeleccionados)
            {
                // Crear un nuevo boleto
                Boleto nuevoBoleto = new Boleto
                {
                    NroBoleto = GenerarNumeroBoleto(), // Método para generar un número de boleto único
                    Origen = "OrigenEjemplo", // Asigna un valor adecuado aquí
                    Destino = "DestinoEjemplo", // Asigna un valor adecuado aquí
                    Fecha = DateTime.Now, // O la fecha que desees asignar
                    NumeroAsiento = numeroAsiento,
                    Pasajero = pasajero // Asegura de que el objeto pasajero esté correctamente inicializado
                };

                // Calcular el precio con descuento
                int precioFinal = precioViaje;

                if (pasajero.TipoPasajero == TipoPasajero.Estudiante)
                {
                    precioFinal -= 2000; // Descuento para estudiantes
                }
                else if (pasajero.TipoPasajero == TipoPasajero.AdultoMayor)
                {
                    precioFinal -= 3000; // Descuento para adultos mayores
                }

                // Asigna el precio final al boleto
                nuevoBoleto.Precio = precioFinal;

                boletos.Add(nuevoBoleto);
            }

            GuardarBoletos(); // Guarda los boletos en el archivo JSON
            MessageText.Text = "Compra procesada con éxito."; // Mensaje de éxito

            LimpiarSeleccionAsientos(); // Limpia la selección de asientos después de procesar la compra
        }

        private int GenerarNumeroBoleto()
        {
            return boletos.Count > 0 ? boletos.Max(b => b.NroBoleto) + 1 : 1;
        }
      

        private void Button_QuitarBoleto_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            pag6.IsVisible = false;
            pag7.IsVisible = true;
            if (int.TryParse(TextBoxNumeroAsientoQuitar.Text, out int numeroAsiento))
            {
                // Lógica para quitar el boleto
                var boleto = boletos.FirstOrDefault(b => b.NumeroAsiento == numeroAsiento);
                if (boleto != null)
                {
                    if (boleto.Pasajero != null)
                    {
                        // Quitar el boleto de la lista
                        boletos.Remove(boleto);
                        MessageText.Text = $"Boleto {numeroAsiento} eliminado.";
                        // Actualizar el estado del asiento en los serviciosBus
                        var servicio = serviciosBus.SelectMany(s => s.Bus.Asientos)
                        .FirstOrDefault(a => a.Numero == numeroAsiento);
                        if (servicio != null)
                        {
                            servicio.Pasajero = null; // Desasigna el pasajero del asiento
                            servicio.Estado = false; // Marca el asiento como disponible
                        }

                        // Guardar los cambios en el JSON
                        GuardarBoletos();

                        // Actualizar la interfaz de usuario
                        InicializarAsientos(asientos); // Vuelve a cargar los asientos


                    }
                }

            }
        }


        private void Button_Modificar_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            // Obtener el RUT del TextBox
            string rutBuscado = TextBoxRutConsulta.Text;

            
   

            
            // Filtrar el servicio, asiento y pasajero
            var resultado = serviciosBus
                .SelectMany(servicio => servicio.Bus.Asientos
                    .Where(asiento => asiento.Pasajero != null && asiento.Pasajero.RUT == rutBuscado)
                    .Select(asiento => new
                    {
                        ServicioId = servicio.Id,
                        Origen = servicio.Origen,
                        Destino = servicio.Destino,
                        Fecha = servicio.Fecha,
                        NumeroAsiento = asiento.Numero,
                        Pasajero = asiento.Pasajero
                    })
                ).FirstOrDefault();

            if (resultado == null)
            {
                MessageText.Text = "RUT ingresado no encontrado.";
                return;
            }
            

            // Obtener el número de asiento a quitar desde el TextBox
            if (int.TryParse(TextBoxNumeroAsientoQuitar.Text, out int numeroAsiento))
            {
                // Verificar si el asiento pertenece al pasajero
                var asientoAEliminar = serviciosBus
                    .SelectMany(servicio => servicio.Bus.Asientos)
                    .FirstOrDefault(asiento => asiento.Numero == numeroAsiento && asiento.Pasajero?.RUT == rutBuscado);

                if (asientoAEliminar != null)
                {
                    // Lógica para quitar el boleto
                    var boleto = boletos.FirstOrDefault(b => b.NumeroAsiento == numeroAsiento);
                    if (boleto != null)
                    {
                        // Eliminar el boleto de la lista
                        boletos.Remove(boleto);

                        // Desasignar el pasajero del asiento
                        asientoAEliminar.Pasajero = null;
                        asientoAEliminar.Estado = false; // Marca el asiento como disponible

                        // Guardar los cambios en los boletos
                        GuardarBoletos();

                        // Guardar los cambios en serviciosBus
                        string nuevoJson1 = JsonSerializer.Serialize(serviciosBus, new JsonSerializerOptions { WriteIndented = true });
                        File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "json", "Terminal.json"), nuevoJson1);

                        MessageTexto.Text = "Boleto quitado con éxito.";
                        TextBoxNumeroAsientoQuitar.Text = string.Empty;
                    }
                    else
                    {
                        MessageTexto.Text = "No se encontró el boleto.";
                    }
                }
                else
                {
                    MessageTexto.Text = "El asiento no pertenece al pasajero o no está ocupado.";
                }
            }
            else
            {
                MessageTexto.Text = "Ingrese un número de asiento válido.";
            }
            
        }

        private void ActualizarPrecioTotal()
        {
            txtValorPasaje.Text = $"Valor Pasaje: {carrito.PrecioTotal}";
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            // Obtener el RadioButton que activó el evento
            var radioButton = sender as RadioButton;

            // Verificar si el RadioButton es el de "Ida y vuelta"
            if (radioButton != null && radioButton.Name == "botonVuelta" && radioButton.IsChecked == true)
            {
                // Hacer visible el StackPanel
                elegirVuelta.IsVisible = true;
            }
        }

        private void RadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            // Obtener el RadioButton que activó el evento
            var radioButton = sender as RadioButton;

            // Verificar si el RadioButton es el de "Ida y vuelta"
            if (radioButton != null && radioButton.Name == "botonIda" && radioButton.IsChecked == true)
            {
                // Hacer invisible el StackPanel
                elegirVuelta.IsVisible = false;
            }
        }

        private void Button_Itinerarios(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                if(!(botonIda.IsChecked ?? false) && !(botonVuelta.IsChecked ?? false))
                {
                    MessageTextBlock.Text = "SELECCIONE EL TIPO DE VIAJE";
                }
                else if (comboBoxOrigen.SelectedItem == null)
                {
                    MessageTextBlock.Text = "SELECCIONE UN ORIGEN";
                }
                else if (comboBoxDestino.SelectedItem == null)
                {
                    MessageTextBlock.Text = "SELECCIONE UN DESTINO";
                }
                else if(!datepicker.SelectedDate.HasValue)
                {
                    MessageTextBlock.Text = "SELECCIONE UNA FECHA";
                }
                else
                {
                    MessageTextBlock.Text = string.Empty;


                    // Cambiar a la página de itinerarios
                    pag1.IsVisible = false;
                    pag2.IsVisible = true;

                    // Guardar o recuperar datos
                    var origenSeleccionado = (comboBoxOrigen.SelectedItem as ComboBoxItem)?.Content as string;
                    var destinoSeleccionado = (comboBoxDestino.SelectedItem as ComboBoxItem)?.Content as string;

                    // Verificar que se haya seleccionado una opción válida
                    if (origenSeleccionado == "Seleccione una opción" || destinoSeleccionado == "Seleccione una opción")
                    {
                        // Mostrar un mensaje de error (opcional)
                        Console.WriteLine("Debe seleccionar una opción válida en origen y destino.");
                        return; // Salir si la selección no es válida
                    }

                    // Obtener la fecha seleccionada desde el DatePicker
                    DateTime? fechaSeleccionada = datepicker.SelectedDate;

                    // Verificar si se seleccionó una fecha
                    if (!fechaSeleccionada.HasValue)
                    {
                        // Mostrar un mensaje de error (opcional)
                        Console.WriteLine("Debe seleccionar una fecha válida.");
                        return; // Salir si no hay una fecha seleccionada
                    }

                    DateTime fecha = fechaSeleccionada.Value;

                    // Filtrar los servicios
                    var serviciosFiltrados = FiltrarServicios(serviciosBus, origenSeleccionado, destinoSeleccionado, fecha);

                    // Mostrar los servicios filtrados
                    MostrarServiciosFiltrados(serviciosFiltrados);


                }


            }
            catch (Exception ex)
            {
                MessageTextBlock.Text = $"Error: {ex.Message}";
            }


           
        }

        private void Button_IngresarPasajeros(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            pag3.IsVisible = false;
            pag4.IsVisible = true;

        }

        private void Button_Volver21(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            pag2.IsVisible = false;
            pag1.IsVisible = true;
        }

        private void Button_Volver32(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            pag3.IsVisible = false;
            pag2.IsVisible = true;
        }

        private void Button_Volver43(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            pag4.IsVisible = false;
            pag3.IsVisible = true;
        }
        private void Button_Volver65(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            pag6.IsVisible = false;
            pag5.IsVisible = true;
        }

        private void Button_Volver76(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            pag7.IsVisible = false;
            pag6.IsVisible = true;
            MessageTexto.Text = string.Empty;
        }
        private List<Servicio> FiltrarServicios(List<Servicio> servicios, string origen, string destino, DateTime fechaSeleccionada)
        {
            return servicios
                .Where(s => s.Origen == origen
                            && s.Destino == destino
                            && s.Fecha.Date == fechaSeleccionada.Date)  // Comparar solo la fecha
                .ToList();
        }

        private void MostrarServiciosFiltrados(List<Servicio> serviciosFiltrados)
        {
            // Limpiar el grid antes de agregar nuevos elementos
            var grid = this.FindControl<Grid>("busServiceGrid");
            grid.Children.Clear();
            columnas();
            // Iterar a través de la lista de servicios filtrados y agregar filas al Grid
            foreach (var servicio in serviciosFiltrados)
            {
                // Convertir los valores del servicio en strings para agregar al grid
                int id = servicio.Id;
                string fechaYHora = servicio.Fecha.ToString("dd-MM-yyyy HH:mm"); // Fecha y hora en formato "dd-MM-yyyy HH:mm"
                string origen = servicio.Origen;
                string destino = servicio.Destino;

                // Llamar a la función para agregar una nueva fila al Grid
                AddBusServiceRow(id,fechaYHora, origen, destino);
            }

        }
        private void columnas()
        {
            
            var grid = this.FindControl<Grid>("busServiceGrid");
            var Id = new TextBlock { Text = "ID", HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center, Foreground = new SolidColorBrush(Colors.Black) };
            var textoFecha = new TextBlock { Text = "Fecha", HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center, Foreground = new SolidColorBrush(Colors.Black)  };
            var textoOrigen = new TextBlock { Text = "Origen", HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center, Foreground = new SolidColorBrush(Colors.Black) };
            var textoDestino = new TextBlock { Text = "Destino", HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center , Foreground = new SolidColorBrush(Colors.Black)};

            Grid.SetColumn(Id, 0);
            Grid.SetRow(Id, 0);
            Grid.SetColumn(textoFecha, 1);  
            Grid.SetRow(textoFecha, 0); 
            Grid.SetColumn(textoOrigen, 2);
            Grid.SetRow(textoOrigen, 0);
            Grid.SetColumn(textoDestino, 3);
            Grid.SetRow(textoDestino, 0);
            
            grid.Children.Add(Id);
            grid.Children.Add(textoFecha);
            grid.Children.Add(textoOrigen);
            grid.Children.Add(textoDestino);

        }

        private void AddBusServiceRow(int ID, string fechaYHora, string origen, string destino)
        {
            var grid = this.FindControl<Grid>("busServiceGrid");

            int rowCount = grid.RowDefinitions.Count;

            // Añadir una nueva fila al Grid
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });

            // Crear los TextBlocks para cada columna
            var Id = new TextBlock { Text = $"{ID}", HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center, Margin = new Avalonia.Thickness(10, 25), FontSize=16 };
            var fechaYHoraTextBlock = new TextBlock { Text = fechaYHora, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center, Margin = new Avalonia.Thickness(10, 25), FontSize = 16 };
            var origenTextBlock = new TextBlock { Text = origen, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center, Margin = new Avalonia.Thickness(10, 25), FontSize = 16 };
            var destinoTextBlock = new TextBlock { Text = destino, HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center, Margin = new Avalonia.Thickness(10, 25), FontSize = 16 };

            // Crear un botón para seleccionar el servicio
            var selectButton = new Button
            {
                Content = "Ver Asientos",
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Margin = new Avalonia.Thickness(10),
                Background = new SolidColorBrush(Colors.SteelBlue),
                Name = "botonAsientos",
                FontSize = 16,
            };

            selectButton.Click += (s, e) =>
            {
                pag2.IsVisible = false;
                pag3.IsVisible = true;
                servicioSeleccionado = serviciosBus.FirstOrDefault(s => s.Id == ID);

                asientos = servicioSeleccionado.Bus.Asientos;
                InicializarAsientos(asientos);


            };



            // Colocar los elementos en el Grid
            Grid.SetRow(Id, rowCount);
            Grid.SetColumn(Id, 0);
            Grid.SetRow(fechaYHoraTextBlock, rowCount);
            Grid.SetColumn(fechaYHoraTextBlock, 1);
            Grid.SetRow(origenTextBlock, rowCount);
            Grid.SetColumn(origenTextBlock, 2);
            Grid.SetRow(destinoTextBlock, rowCount);
            Grid.SetColumn(destinoTextBlock, 3);
            Grid.SetRow(selectButton, rowCount);
            Grid.SetColumn(selectButton, 4); // Columna para el botón

            // Añadir los elementos al Grid
            grid.Children.Add(selectButton);
            grid.Children.Add(Id);
            grid.Children.Add(fechaYHoraTextBlock);
            grid.Children.Add(origenTextBlock);
            grid.Children.Add(destinoTextBlock);
            
        }

        private TipoPasajero ObtenerTipoPasajeroSeleccionado()
        {
            if (normalRadioButton.IsChecked == true)
                return TipoPasajero.Normal;
            else if (estudianteRadioButton.IsChecked == true)
                return TipoPasajero.Estudiante;
            else if (adultoMayorRadioButton.IsChecked == true)
                return TipoPasajero.AdultoMayor;

            // Valor por defecto si no se selecciona nada
            return TipoPasajero.Normal;
        }
        private void Button_VerResumenPagar(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            try
            {
                TipoPago metodoPagoSeleccionado;
                if (comboBoxMetodoPago.SelectedItem is ComboBoxItem selectedItem)
                {
                    metodoPagoSeleccionado = (TipoPago)Enum.Parse(typeof(TipoPago), selectedItem.Content.ToString());
                }
                else
                {
                    MessageText.Text = "Seleccione un método de pago.";
                    return;
                }

                pasajero = new Pasajero(TextBoxNombre.Text, ObtenerTipoPasajeroSeleccionado());
                DateTime fechaActual= DateTime.Now;
                pasajero.Nombre = TextBoxNombre.Text;
                pasajero.RUT = TextBoxRutCompra.Text;
                pasajero.Telefono = TextBoxTelefono.Text;
                pasajero.Email = TextBoxEmail.Text;

                if (normalRadioButton.IsChecked == true)
                {
                    pasajero.TipoPasajero = TipoPasajero.Normal;
                }
                else if (estudianteRadioButton.IsChecked == true)
                {
                    pasajero.TipoPasajero = TipoPasajero.Estudiante;
                }
                else if (adultoMayorRadioButton.IsChecked == true) {

                    pasajero.TipoPasajero = TipoPasajero.AdultoMayor;
                }
                if (string.IsNullOrEmpty(pasajero.Nombre))
                {
                    MessageText.Text = "INGRESE EL NOMBRE";
                }
                else if (string.IsNullOrEmpty(pasajero.RUT) || pasajero.RUT.Length < 10 || !pasajero.RUT.Contains("-"))
                {
                    MessageText.Text = "INGRESE EL RUT";
                }
                else if(!(normalRadioButton.IsChecked ?? false) && !(estudianteRadioButton.IsChecked ?? false) && !(adultoMayorRadioButton.IsChecked ?? false))
                {
                    MessageText.Text = "INGRESE EL TIPO DE PASAJERO";
                }
                else if(string.IsNullOrEmpty(pasajero.Telefono) || pasajero.Telefono.Length < 9)
                {
                    MessageText.Text = "INGRESE EL TELEFONO";
                }
                else if(string.IsNullOrEmpty(pasajero.Email) || !pasajero.Email.Contains("@") || !pasajero.Email.Contains("."))
                {
                    MessageText.Text = "INGRESE UN CORREO ELECTRÓNICO VÁLIDO";
                }
                else
                {
                    TipoPasajero tipoPasajero;

                    if (normalRadioButton.IsChecked == true)
                    {
                        tipoPasajero = TipoPasajero.Normal;
                    }
                    else if (estudianteRadioButton.IsChecked == true)
                    {
                        tipoPasajero = TipoPasajero.Estudiante;
                    }
                    else // (adultoMayorRadioButton.IsChecked == true)
                    {
                        tipoPasajero = TipoPasajero.AdultoMayor;
                    }

                    pasajero = new Pasajero(TextBoxNombre.Text, tipoPasajero)
                    {
                        RUT = TextBoxRutCompra.Text,
                        Telefono = TextBoxTelefono.Text,
                        Email = TextBoxEmail.Text
                    };

                    MessageText.Text = string.Empty;
                    boleto.NumeroAsiento = asientosSeleccionados[0];
                    boleto.Destino = servicioSeleccionado.Destino;
                    boleto.Origen = servicioSeleccionado.Origen;
                    boleto.Rebaja = false;
                    boleto.Fecha = servicioSeleccionado.Fecha;
                    boleto.NroBoleto = boletos.Any() ? boletos.Max(c => c.NroBoleto) + 1 : 1;

                    boletos.Add(boleto);

                    int precioConDescuento = precioViaje; // Precio base

                    if (pasajero.TipoPasajero == TipoPasajero.Estudiante)
                    {
                        precioConDescuento -= 2000; // Descuento para estudiantes
                    }
                    else if (pasajero.TipoPasajero == TipoPasajero.AdultoMayor)
                    {
                        precioConDescuento -= 3000; // Descuento para adultos mayores
                    }

                   


                    carrito.AgregarBoleto(boleto, precioViaje);


                    string nuevoJson = JsonSerializer.Serialize(boletos, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "json", "compras.json"), nuevoJson);

                    carrito.AplicarDescuentos();

                    

                    foreach (var item in serviciosBus)
                    {
                        if (item == servicioSeleccionado)
                        {
                            foreach (var item1 in item.Bus.Asientos)
                            {

                                if (item1.Numero == asientosSeleccionados[0])
                                {
                                    item1.Estado = true;
                                    item1.Pasajero = pasajero;

                                }
                            }
                        }
                    }


                    string nuevoJson1 = JsonSerializer.Serialize(serviciosBus, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "json", "Terminal.json"), nuevoJson1);


                    TextBoxNombre.Text = "";
                    TextBoxRutCompra.Text = "";
                    TextBoxTelefono.Text = "";
                    TextBoxEmail.Text = "";

                    pag4.IsVisible = false;
                    pag5.IsVisible = true;

                    txtFechaBoleto.Text = $"Fecha: {boleto.Fecha}";
                    txtOrigenBoleto.Text = $"Origen: {boleto.Origen}";
                    txtDestinoBoleto.Text = $"Destino: {boleto.Destino}";
                    txtNumeroAsiento.Text = $"Numero Asiento: {boleto.NumeroAsiento}";
                    txtValorPasaje.Text = $"Valor Pasaje: {precioConDescuento}";

                    txtNombreFactura.Text = "Nombre Emisor";
                    txtDireccionFactura.Text = "Direccion Emisor";
                    txtNombrePasajeroFactura.Text = $"Nombre Pasajero: {pasajero.Nombre}";
                    txtRutFactura.Text = $"Rut Pasajero: {pasajero.RUT}";
                    txtFechaFactura.Text = $"Fecha: {DateTime.Now}";
                    txtOrigenFactura.Text = $"Origen: {boleto.Origen}";
                    txtDestinoFactura.Text = $"Destino: {boleto.Destino}";
                    txtNumeroAsientoFactura.Text = $"Numero Asiento: {boleto.NumeroAsiento}";
                    txtValorPasajeFactura.Text = $"Valor Pasaje: {precioConDescuento}";

                }

            }

            catch (Exception ex)
            {
                MessageText.Text = $"Error: {ex.Message}";
            }

        }

        private Button? asientoSeleccionado = null; // Almacena el botón del asiento seleccionado

        

        private void Asiento_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button botonAsiento)
            {
                int numeroAsiento = int.Parse(botonAsiento.Content.ToString());
                Asiento asiento = asientos.Find(a => a.Numero == numeroAsiento);

                if (asientosSeleccionados.Contains(numeroAsiento))
                {
                    // Deseleccionar asiento
                    asientosSeleccionados.Remove(numeroAsiento);
                    botonAsiento.Background = Brushes.Green;
                }
                else
                {
                    // Seleccionar asiento
                    asientosSeleccionados.Add(numeroAsiento);
                    botonAsiento.Background = Brushes.LightSkyBlue;
                }

                
            }
        }

        private void InicializarAsientos(List<Asiento> listAsientos)
        {

            AsientosGrid.Children.Clear();

            // Crear botones dinámicamente en el Grid
            foreach (var asiento in listAsientos)
            {
                Button botonAsiento = new Button
                {
                    Content = asiento.Numero.ToString(),
                    Width = 38,
                    Height = 38,
                    Margin = new Avalonia.Thickness(1),
                    Background = asiento.Estado ? Brushes.Red : Brushes.Green,
                    Foreground = Brushes.White,
                    IsEnabled = !asiento.Estado, // Deshabilitar si el asiento está ocupado
                    FontSize = 16
                };

                botonAsiento.Click += Asiento_Click;

                // Determinar posición en el Grid
                int row = (asiento.Numero - 1) / 4; // Fila
                int col = (asiento.Numero - 1) % 4; // Columna
                if (col >= 2) col += 1; // Ajustar espacio entre las dos columnas centrales

                // Agregar botón al Grid
                Grid.SetRow(botonAsiento, row);
                Grid.SetColumn(botonAsiento, col);
                AsientosGrid.Children.Add(botonAsiento);
            }
        }
        //evento boton consulta
        private void Button_Click_1(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            stackBlocks.Children.Clear();
            string rutBuscado = TextBoxRutConsulta.Text;
            // Filtrar el servicio, asiento y pasajero

            var resultado = serviciosBus
                .SelectMany(servicio => servicio.Bus.Asientos
                    .Where(asiento => asiento.Pasajero != null && asiento.Pasajero.RUT == rutBuscado)
                    .Select(asiento => new
                    {
                        ServicioId = servicio.Id,
                        Origen = servicio.Origen,
                        Destino = servicio.Destino,
                        Fecha = servicio.Fecha,
                        NumeroAsiento = asiento.Numero,
                        Pasajero = asiento.Pasajero
                    })
                ).FirstOrDefault();
            if (resultado == null)
            {
                var error = new TextBlock { Text = "RUT INGRESADO NO ENCONTRADO", Foreground = new SolidColorBrush(Colors.Red), TextAlignment = TextAlignment.Center, HorizontalAlignment = HorizontalAlignment};
                
                stackBlocks.Children.Add(error);
            }
            else 
            {
                  var stackblock = this.FindControl<StackPanel>("stackBlocks");

                var origen = new TextBlock { Text= $"Origen:{" "+resultado.Origen} ", Foreground = new SolidColorBrush(Colors.Black), FontSize=16, TextAlignment = TextAlignment.Left, HorizontalAlignment = HorizontalAlignment, Margin = new Thickness(8) };
                var destino = new TextBlock { Text = $"Destino:{" " + resultado.Destino} ", Foreground = new SolidColorBrush(Colors.Black), FontSize = 16, TextAlignment = TextAlignment.Left, HorizontalAlignment = HorizontalAlignment, Margin = new Thickness(8) };
                var fecha= new TextBlock { Text = $"Fecha :{" " + resultado.Fecha} ", Foreground = new SolidColorBrush(Colors.Black), FontSize = 16, TextAlignment = TextAlignment.Left, HorizontalAlignment = HorizontalAlignment, Margin = new Thickness(8) };
                var idServicio= new TextBlock { Text = $"ID del servicio:{" " + resultado.ServicioId} ", Foreground = new SolidColorBrush(Colors.Black), FontSize = 16, TextAlignment = TextAlignment.Left, HorizontalAlignment = HorizontalAlignment, Margin = new Thickness(8) };
                var numeroAsiento= new TextBlock { Text = $"Numero Asiento:{" " + resultado.NumeroAsiento} ", Foreground = new SolidColorBrush(Colors.Black), FontSize = 16, TextAlignment = TextAlignment.Left, HorizontalAlignment = HorizontalAlignment, Margin = new Thickness(8) };
                var nombre= new TextBlock { Text = $"Nombre Pasajero:{" " + resultado.Pasajero.Nombre} ", Foreground = new SolidColorBrush(Colors.Black), FontSize = 16, TextAlignment = TextAlignment.Left, HorizontalAlignment = HorizontalAlignment, Margin = new Thickness(8) };
                var rut= new TextBlock { Text = $"Rut:{" " + resultado.Pasajero.RUT} ", Foreground = new SolidColorBrush(Colors.Black), FontSize = 16, TextAlignment = TextAlignment.Left, HorizontalAlignment = HorizontalAlignment, Margin = new Thickness(8) };


                stackblock.Children.Add(origen);
                stackblock.Children.Add(destino);
                stackblock.Children.Add(fecha);
                stackblock.Children.Add(idServicio);
                stackblock.Children.Add(numeroAsiento);
                stackblock.Children.Add(nombre);
                stackblock.Children.Add(rut);
            }
            

        }

        private void agregarServicio(object? sender, Avalonia.Interactivity.RoutedEventArgs e )
        {
            
            try
            {
                if (ingresarOrigen.SelectedItem == null)
                {
                    MessageTextBlockError.Text = "SELECCIONE UN ORIGEN";
                }
                else if(ingresarDestino.SelectedItem == null)
                {
                    MessageTextBlockError.Text = "SELECCIONE UN DESTINO";
                }
                else if (string.IsNullOrEmpty(nombreChofer.Text))
                {
                    MessageTextBlockError.Text = "INGRESE NOMBRE DEL CHOFER";
                }
                else if (string.IsNullOrEmpty(nombreAuxiliar.Text))
                {
                    MessageTextBlockError.Text = "INGRESE NOMBRE DEL AUXILIAR";
                }
                else if(string.IsNullOrEmpty(rutChofer.Text) || rutChofer.Text.Length < 10 || !rutChofer.Text.Contains("-"))
                {
                    MessageTextBlockError.Text = "INGRESE RUT DEL CHOFER";
                }
                else if(string.IsNullOrEmpty(rutAuxiliar.Text) || rutAuxiliar.Text.Length < 10 || !rutAuxiliar.Text.Contains("-"))
                {
                    MessageTextBlockError.Text = "INGRESE RUT DEL AUXILIAR";
                }
                else if(!ingresarFecha.SelectedDate.HasValue )
                {
                    MessageTextBlockError.Text = "SELECCIONE UNA FECHA";
                }
                else if (string.IsNullOrEmpty(ingresarHora.Text))
                {
                    MessageTextBlockError.Text = "INGRESE HORA";
                }
                else if (string.IsNullOrEmpty(ingresarMinutos.Text))
                {
                    MessageTextBlockError.Text = "INGRESE MINUTOS";
                }

                else
                {
                    MessageTextBlockError.Text = string.Empty;
                    // Crear objetos y guardar datos recuperados

                    // Crear y asignar datos del chofer
                    Chofer chofer1 = new Chofer
                    {
                        Nombre = nombreChofer.Text,
                        RUT = rutChofer.Text
                    };

                    // Crear y asignar datos del auxiliar
                    Auxiliar auxiliar1 = new Auxiliar
                    {
                        Nombre = nombreAuxiliar.Text,
                        RUT = rutAuxiliar.Text
                    };

                    // Crear y asignar datos del bus
                    Bus bus1 = new Bus
                    {
                        Auxiliar = auxiliar1,
                        Chofer = chofer1
                    };

                    // Crear y asignar datos del servicio
                    Servicio servicio1 = new Servicio
                    {
                        Id = serviciosBus.Any() ? serviciosBus.Max(c => c.Id) + 1 : 1,
                        Bus = bus1,
                        Destino = (ingresarDestino.SelectedItem as ComboBoxItem)?.Content as string,
                        Origen = (ingresarOrigen.SelectedItem as ComboBoxItem)?.Content as string,
                        Fecha = ingresarFecha.SelectedDate.HasValue
                                ? ingresarFecha.SelectedDate.Value
                                : throw new InvalidOperationException("INGRESE UNA FECHA")
                    };

                    // Crear el TimeSpan con la hora y los minutos ingresados
                    TimeSpan tiempo = new TimeSpan(int.Parse(ingresarHora.Text), int.Parse(ingresarMinutos.Text), 0);

                    // Agregar el TimeSpan a la fecha y actualizar la propiedad Fecha
                    servicio1.Fecha = servicio1.Fecha.Add(tiempo);

                    // Opcional: Agregar servicio a la lista o procesarlo
                    serviciosBus.Add(servicio1);

                    string nuevoJson1 = JsonSerializer.Serialize(serviciosBus, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(Path.Combine(AppContext.BaseDirectory, "json", "Terminal.json"), nuevoJson1);




                    ingresarOrigen.SelectedItem = null;
                    ingresarDestino.SelectedItem = null;
                    nombreChofer.Text = string.Empty;
                    rutChofer.Text = string.Empty;
                    nombreAuxiliar.Text = string.Empty;
                    rutAuxiliar.Text = string.Empty;
                    ingresarHora.Text = string.Empty;
                    ingresarMinutos.Text = string.Empty;


                    // Reiniciar el selector de fecha
                    ingresarFecha.SelectedDate = null;
                }
       

            }
            catch (Exception ex) 
            {
                MessageTextBlockError.Text = $"Error: {ex.Message}";
            }

        }

        private void Button_Click_2(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            pag5.IsVisible = false;
            pag1.IsVisible = true;
        }
    }


}
