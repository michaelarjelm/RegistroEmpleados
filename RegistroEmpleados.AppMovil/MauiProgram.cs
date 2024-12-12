﻿using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.Extensions.Logging;
using RegistroEmpleados.Modelos.Modelos;

namespace RegistroEmpleados.AppMovil
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            ActualizarCargo();
            ActualizarEmpleado();
            return builder.Build();


        }

        public static async Task ActualizarCargo()
        {
            FirebaseClient client = new FirebaseClient("https://registroempleados-d7b5e-default-rtdb.firebaseio.com/");

            var cargos = await client.Child("Cargos").OnceAsync<Cargo>();

            if (cargos.Count == 0)
            {
                await client.Child("Cargos").PostAsync(new Cargo { Nombre = "Administrador" });
                await client.Child("Cargos").PostAsync(new Cargo { Nombre = "Supervisor" });
                await client.Child("Cargos").PostAsync(new Cargo { Nombre = "Usuario" });
            }
            else
            {
                foreach (var cargo in cargos)
                {
                    if (cargo.Object.Estado == null)
                    {
                        var cargoActualizado = cargo.Object;
                        cargoActualizado.Estado = true;

                        await client.Child("Cargos").Child(cargo.Key).PutAsync(cargoActualizado);
                    }
                }
            }
        }

        public static async Task ActualizarEmpleado()
        {
            FirebaseClient client = new FirebaseClient("https://registroempleados-d7b5e-default-rtdb.firebaseio.com/");

            var empleados = await client.Child("Empleados").OnceAsync<Empleado>();

            foreach (var empleado in empleados)
            {
                if (empleado.Object.Estado == null)
                {
                    var empleadoActualizado = empleado.Object;
                    empleadoActualizado.Estado = true;

                    await client.Child("Empleados").Child(empleado.Key).PutAsync(empleadoActualizado);
                }
            }

        }
    }
}
