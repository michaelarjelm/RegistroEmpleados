
using Firebase.Database;
using Firebase.Database.Query;
using RegistroEmpleados.Modelos.Modelos;
using System.Collections.ObjectModel;

namespace RegistroEmpleados.AppMovil.Vistas;

public partial class EditarEmpleado : ContentPage
{
    FirebaseClient client = new FirebaseClient("https://registroempleados-d7b5e-default-rtdb.firebaseio.com/");
    public List<Cargo> Cargos { get; set; }
    public ObservableCollection<string> ListaCargos { get; set; } = new ObservableCollection<string>();
    public EditarEmpleado(string idEmpleado)
    {
        InitializeComponent();
        BindingContext = this;
        CargarListaCargos();

        CargarEmpleado(idEmpleado);
    }

    private async void CargarListaCargos()
    {
        try
        {
            var cargos = await client
                .Child("Cargos")
                .OnceAsync<Cargo>();

            ListaCargos.Clear();

            foreach (var cargo in cargos)
            {
                ListaCargos.Add(cargo.Object.Nombre);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Error: "+ex.Message, "OK");
        }
    }

    private async void CargarEmpleado(string idEmpleado)
    {
        try
        {
            var empleado = await client
                .Child("Empleados")
                .Child(idEmpleado)
                .OnceSingleAsync<Empleado>();

            if (empleado != null)
            {
                PrimerNombreEntry.Text = empleado.PrimerNombre;
                SegundoNombreEntry.Text = empleado.SegundoNombre;
                PrimerApellidoEntry.Text = empleado.PrimerApellido;
                SegundoApellidoEntry.Text = empleado.SegundoApellido;
                CorreoEntry.Text = empleado.CorreoElectronico;
                SueldoEntry.Text = empleado.Sueldo.ToString();
                CargoPicker.SelectedItem = empleado.Cargo?.Nombre;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Error: "+ex.Message, "OK");
        }
    }
}