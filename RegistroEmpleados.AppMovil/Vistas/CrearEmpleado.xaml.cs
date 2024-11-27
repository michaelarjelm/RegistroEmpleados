using Firebase.Database;
using Firebase.Database.Query;
using RegistroEmpleados.Modelos.Modelos;

namespace RegistroEmpleados.AppMovil.Vistas;

public partial class CrearEmpleado : ContentPage
{
    FirebaseClient client = new FirebaseClient("https://registroempleados-d7b5e-default-rtdb.firebaseio.com/");

    public List<Cargo> Cargos {  get; set; }

    public CrearEmpleado()
	{
		InitializeComponent();
        ListarCargos();
        BindingContext = this;
	}

    private void ListarCargos()
    {
        var cargos = client.Child("Cargos").OnceAsync<Cargo>();
        Cargos=cargos.Result.Select(x=>x.Object).ToList();
    }

    private void guardarButton_Clicked(object sender, EventArgs e)
    {

    }
}