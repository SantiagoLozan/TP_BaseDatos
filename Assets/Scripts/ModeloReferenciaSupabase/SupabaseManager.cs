using UnityEngine;
using Supabase;
using Supabase.Interfaces;
using System.Threading;
using Postgrest.Models;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;

public class SupabaseManager : MonoBehaviour

{

    [Header("Campos de Interfaz")]
    [SerializeField] TMP_InputField _userIDInput;
    [SerializeField] TMP_InputField _userPassInput;
    [SerializeField] TextMeshProUGUI _stateText;

    string supabaseUrl = "https://sluwyusxjyskvgprkyov.supabase.co"; //COMPLETAR
    string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InNsdXd5dXN4anlza3ZncHJreW92Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3MTg4Mjk1MzgsImV4cCI6MjAzNDQwNTUzOH0.19GZ2W5UfdXK0IKAp5twfepNI06wZ_yyToTd_tmAQCk"; //COMPLETAR


    Supabase.Client clientSupabase;

    private usuarios _usuarios = new usuarios();


    public async void UserLogin()
    {
        // Initialize the Supabase client
        clientSupabase = new Supabase.Client(supabaseUrl, supabaseKey);

        // prueba
        var test_response = await clientSupabase
            .From<usuarios>()
            .Select("*")
            .Get();
        Debug.Log(test_response.Content);



        // filtro seg�n datos de login
        var login_password = await clientSupabase
          .From<usuarios>()
          .Select("password")
          .Where(usuarios => usuarios.username == _userIDInput.text)
          .Get();


        if (login_password.Model.password.Equals(_userPassInput.text))
        {
            print("LOGIN SUCCESSFUL");
            _stateText.text = "LOGIN SUCCESSFUL";
            _stateText.color = Color.green;
        }
        else
        {
            print("WRONG PASSWORD");
            _stateText.text = "WRONG PASSWORD";
            _stateText.color = Color.red;
        }
    }

    public async void InsertarNuevoUsuario()
    {

        // Initialize the Supabase client
        clientSupabase = new Supabase.Client(supabaseUrl, supabaseKey);

        // Consultar el �ltimo id utilizado (ID = index)
        var ultimoId = await clientSupabase
            .From<usuarios>()
            .Select("id")
            .Order(usuarios => usuarios.id, Postgrest.Constants.Ordering.Descending) // Ordenar en orden descendente para obtener el �ltimo id
            .Get();

        int nuevoId = 1; // Valor predeterminado si la tabla est� vac�a

        if (ultimoId != null)
        {
            nuevoId = ultimoId.Model.id + 1; // Incrementar el �ltimo id
        }

        // Crear el nuevo usuario con el nuevo id
        var nuevoUsuario = new usuarios
        {

            id = nuevoId,
            username = _userIDInput.text,
            age = Random.Range(0, 100), //luego creo el campo que falta en la UI
            password = _userPassInput.text,
        };


        // Insertar el nuevo usuario
        var resultado = await clientSupabase
            .From<usuarios>()
            .Insert(new[] { nuevoUsuario });


        //verifico el estado de la inserci�n 
        if (resultado.ResponseMessage.IsSuccessStatusCode)
        {
            _stateText.text = "Usuario Correctamente Ingresado";
            _stateText.color = Color.green;
        }
        else
        {
            _stateText.text = "Error en el registro de usuario";
            _stateText.text = resultado.ResponseMessage.ToString();
            _stateText.color = Color.green;
        }

    }
}

