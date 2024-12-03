using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.ObjectModel;

namespace Terminal
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

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
            if (radioButton != null && radioButton.Name == "botonVuelta" && radioButton.IsChecked == false)
            {
                // Hacer invisible el StackPanel
                elegirVuelta.IsVisible = false;
            }
        }

        private void Button_Confirmar(object sender, RoutedEventArgs e)
        {
            pag1.IsVisible = false;
            pag3.IsVisible = true;


            //Guardar o recuperar datos

        }



    }
}