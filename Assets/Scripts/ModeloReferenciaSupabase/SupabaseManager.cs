using UnityEngine;
using Supabase;
using Supabase.Interfaces;
using System.Threading;
using Postgrest.Models;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class SupabaseManager : MonoBehaviour

{

    [Header("Campos de Interfaz")]
    [SerializeField] TMP_InputField _userIDInput;
    [SerializeField] TMP_InputField _userPassInput;
    [SerializeField] TextMeshProUGUI _stateText;

    string supabaseUrl = "https://mtaklkgetzskroigdppr.supabase.co"; //COMPLETAR
    string supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Im10YWtsa2dldHpza3JvaWdkcHByIiwicm9sZSI6ImFub24iLCJpYXQiOjE3MzMxMTYwNDIsImV4cCI6MjA0ODY5MjA0Mn0.x_tOH1ZWPj8BJcxXj4wHOTKAo7JbNzDxZWLpQoWd_4c"; //COMPLETAR


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

        // filtro según datos de login
        var login_password = await clientSupabase
            .From<usuarios>()
            .Select("password")
            .Where(usuarios => usuarios.username == _userIDInput.text)
            .Get();

        if (login_password != null && login_password.Model.password.Equals(_userPassInput.text))
        {
            print("LOGIN SUCCESSFUL");
            _stateText.text = "LOGIN SUCCESSFUL";
            _stateText.color = Color.green;
            CambiarEscena("TriviaSelectScene");
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

        // Consultar el ultimo id utilizado (ID = index)
        var ultimoIdResponse = await clientSupabase
            .From<usuarios>()
            .Select("id")
            .Order(usuarios => usuarios.id, Postgrest.Constants.Ordering.Descending) // Ordenar en orden descendente para obtener el ultimo id
            .Limit(1) // Limitar a un solo resultado
            .Get();

        int nuevoId = 1; // Valor predeterminado si la tabla esta vacia

        if (ultimoIdResponse.Models.Count > 0)
        {
            var ultimoId = ultimoIdResponse.Models[0];
            nuevoId = ultimoId.id + 1; // Incrementar el ultimo id
        }

        // Crear el nuevo usuario con el nuevo id
        var nuevoUsuario = new usuarios
        {
            id = nuevoId,
            username = _userIDInput.text,
            password = _userPassInput.text,
        };

        // Insertar el nuevo usuario
        var resultado = await clientSupabase
            .From<usuarios>()
            .Insert(new[] { nuevoUsuario });

        // Verificar el estado de la inserción 
        if (resultado.ResponseMessage.IsSuccessStatusCode)
        {
            _stateText.text = "Usuario Correctamente Ingresado";
            _stateText.color = Color.green;
        }
        else
        {
            _stateText.text = "Error en el registro de usuario";
            _stateText.text += resultado.ResponseMessage.ToString();
            _stateText.color = Color.red;
        }
    }
    public void CambiarEscena(string name)
    {
        SceneManager.LoadScene(name);
    }
}