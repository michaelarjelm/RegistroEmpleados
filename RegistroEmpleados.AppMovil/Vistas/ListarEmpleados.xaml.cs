using Firebase.Database;
using RegistroEmpleados.Modelos.Modelos;
using System.Collections.ObjectModel;

namespace RegistroEmpleados.AppMovil.Vistas;

public partial class ListarEmpleados : ContentPage
{
    FirebaseClient client = new FirebaseClient("https://registroempleados-d7b5e-default-rtdb.firebaseio.com/");
    public ObservableCollection<Empleado> Lista { get; set; } = new ObservableCollection<Empleado>();
    public ListarEmpleados()
    {
        InitializeComponent();
        BindingContext = this;
        CargarLista();
    }

    private async void CargarLista()
    {
        var empleados = await client
           .Child("Empleados")
           .OnceAsync<Empleado>();
        Lista.Clear(); // Limpia la lista antes de agregar nuevos datos

        foreach (var empleado in empleados)
        {
            Lista.Add(new Empleado
            {
                Id = empleado.Key, 
                PrimerNombre = empleado.Object.PrimerNombre,
                SegundoNombre = empleado.Object.SegundoNombre,
                PrimerApellido = empleado.Object.PrimerApellido,
                SegundoApellido = empleado.Object.SegundoApellido,
                CorreoElectronico = empleado.Object.CorreoElectronico,
                FechaInicio = empleado.Object.FechaInicio,
                Cargo = empleado.Object.Cargo
            });
        }
    }

    private void filtroSearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        string filtro=filtroEntry.Text.ToLower();
        if (filtro.Length > 0) 
        {
            listaCollection.ItemsSource = Lista.Where(x => x.NombreCompleto.ToLower().Contains(filtro));
        }
        else
        {
            listaCollection.ItemsSource = Lista;
        }
    }

    private async void NuevoEmpleadoBoton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CrearEmpleado());
    }

    private async void editarButton_Clicked(object sender, EventArgs e)
    {
        var boton = sender as ImageButton; // Obtén el botón que disparó el evento
        var empleado = boton?.CommandParameter as Empleado; // Obtén el empleado asociado

        if (empleado != null && !string.IsNullOrEmpty(empleado.Id))
        {
            // Navega a la vista de edición pasando el ID del empleado
            await Navigation.PushAsync(new EditarEmpleado(empleado.Id));
        }
        else
        {
            // Muestra un mensaje si no se pudo obtener el empleado
            await DisplayAlert("Error", "No se pudo obtener la información del empleado.", "OK");
        }
    }
}