
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
    private Empleado empleadoActual= new Empleado(); 
    private string empleadoId;

    public EditarEmpleado(string idEmpleado)
    {
        InitializeComponent();
        BindingContext = this;
        empleadoId = idEmpleado;
        CargarListaCargos();        
        CargarEmpleado(empleadoId);
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

    private async void ActualizarButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(PrimerNombreEntry.Text) ||
                string.IsNullOrWhiteSpace(PrimerApellidoEntry.Text) ||
                string.IsNullOrWhiteSpace(CorreoEntry.Text) ||
                string.IsNullOrWhiteSpace(SueldoEntry.Text) ||
                CargoPicker.SelectedItem == null)
            {
                await DisplayAlert("Error", "Todos los campos son obligatorios.", "OK");
                return;
            }

            if (!CorreoEntry.Text.Contains("@"))
            {
                await DisplayAlert("Error", "El correo electrónico no es válido.", "OK");
                return;
            }

            if (!int.TryParse(SueldoEntry.Text, out int sueldo))
            {
                await DisplayAlert("Error", "El sueldo debe ser un número válido.", "OK");
                return;
            }

            if (sueldo <= 0)
            {
                await DisplayAlert("Error", "El sueldo debe ser mayor a 0.", "OK");
                return;
            }
            empleadoActual.Id = empleadoId;
            empleadoActual.PrimerNombre = PrimerNombreEntry.Text.Trim();
            empleadoActual.SegundoNombre = SegundoNombreEntry.Text?.Trim();
            empleadoActual.PrimerApellido = PrimerApellidoEntry.Text.Trim();
            empleadoActual.SegundoApellido = SegundoApellidoEntry.Text?.Trim();
            empleadoActual.CorreoElectronico = CorreoEntry.Text.Trim();
            empleadoActual.Sueldo = sueldo;
            empleadoActual.Cargo = new Cargo { Nombre = CargoPicker.SelectedItem.ToString() };

            await client
                .Child("Empleados")
                .Child(empleadoActual.Id) 
                .PutAsync(empleadoActual);

            await DisplayAlert("Éxito", "El empleado se ha actualizado correctamente.", "OK");
            await Navigation.PopAsync(); 
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un error al guardar los cambios: {ex.Message}", "OK");
        }
    }
}