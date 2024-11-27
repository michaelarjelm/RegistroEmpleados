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

    private void CargarLista()
    {
        client.Child("Empleados").AsObservable<Empleado>().Subscribe((empleado) =>
        {
            if (empleado.Object != null)
            {
                Lista.Add(empleado.Object);
            }
        });
    }

    private void filtroSearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {

    }

    private async void NuevoEmpleadoBoton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CrearEmpleado());
    }
}