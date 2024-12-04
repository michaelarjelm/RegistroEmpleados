using Firebase.Database;
using Firebase.Database.Query;
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

        var empleadosActivos = empleados
            .Where(e => e.Object.Estado == true)
            .ToList();
        Lista.Clear(); 

        foreach (var empleado in empleadosActivos)
        {
            Lista.Add(new Empleado
            {
                Id = empleado.Key, 
                PrimerNombre = empleado.Object.PrimerNombre,
                SegundoNombre = empleado.Object.SegundoNombre,
                PrimerApellido = empleado.Object.PrimerApellido,
                SegundoApellido = empleado.Object.SegundoApellido,
                CorreoElectronico = empleado.Object.CorreoElectronico,
                Sueldo = empleado.Object.Sueldo,
                FechaInicio = empleado.Object.FechaInicio,
                Estado = empleado.Object.Estado,
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
        var boton = sender as ImageButton; 
        var empleado = boton?.CommandParameter as Empleado; 

        if (empleado != null && !string.IsNullOrEmpty(empleado.Id))
        {
            await Navigation.PushAsync(new EditarEmpleado(empleado.Id));
        }
        else
        {
            await DisplayAlert("Error", "No se pudo obtener la informaci�n del empleado.", "OK");
        }
    }

    private async void deshabilitarButton_Clicked(object sender, EventArgs e)
    {
        var boton = sender as ImageButton;
        var empleado = boton?.CommandParameter as Empleado;

        if (empleado == null)
        {
            await DisplayAlert("Error", "No se pudo obtener la informaci�n del empleado.", "OK");
            return;
        }

        bool confirmacion = await DisplayAlert(
        "Confirmaci�n",
        $"�Est� seguro de deshabilitar al empleado {empleado.NombreCompleto}?",
        "S�", "No");

        if (confirmacion)
        {
            try
            {
                empleado.Estado = false;
               
                await client
                    .Child("Empleados")
                    .Child(empleado.Id) 
                    .PutAsync(empleado);

                await DisplayAlert("�xito", $"El empleado {empleado.NombreCompleto} ha sido deshabilitado.", "OK");
                CargarLista();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Ocurri� un error al deshabilitar al empleado: {ex.Message}", "OK");
            }
        }
    }

    private async void listaCollection_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var empleado = e.CurrentSelection.FirstOrDefault() as Empleado;

        if (empleado != null)
        {
            await Navigation.PushAsync(new DetalleEmpleado(empleado));
        }

    ((CollectionView)sender).SelectedItem = null;
    }
}