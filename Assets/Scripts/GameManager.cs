using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public List<question> responseList; // Lista donde guardo la respuesta de la query hecha en la pantalla de selección de categoría

    private List<int> indicePreguntaUsada = new List<int>();

    public int currentTriviaIndex = 0;
    public int randomQuestionIndex = 0;

    public List<string> _answers = new List<string>();

    public bool queryCalled;

    public int _points;

    private int _maxAttempts = 10;

    public int _numQuestionAnswered = 0;

    string _correctAnswer;

    public static GameManager Instance { get; private set; }

    void Awake()
    {
        // Configura la instancia
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Para mantener el objeto entre escenas
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        StartTrivia();
        queryCalled = false;
    }

    void StartTrivia()
    {
        // Cargar la trivia desde la base de datos
        // triviaManager.LoadTrivia(currentTriviaIndex);
        // print(responseList.Count);
    }

    public void ResetTrivia()
    {
        indicePreguntaUsada.Clear();
        currentTriviaIndex = 0;
        randomQuestionIndex = 0;
        _answers.Clear();
        queryCalled = false;
        _points = 0;
        _numQuestionAnswered = 0;
        _correctAnswer = string.Empty;
    }

    public void ReturnToCategorySelection()
    {
        ResetTrivia();
        SceneManager.LoadScene("CategorySelectionScene");
    }

    public void CategoryAndQuestionQuery(bool isCalled)
    {
        isCalled = UIManagment.Instance.queryCalled;

        if (!isCalled)
        {
            // Limpiar las respuestas y los listeners antes de asignar nuevas respuestas
            ClearPreviousAnswers();

            Debug.Log("Iniciando trivia con " + responseList.Count + " preguntas.");

            if (responseList.Count > 0)
            {
                Debug.Log("Primera pregunta: " + responseList[0].QuestionText);
            }
            else
            {
                Debug.LogError("responseList está vacío. Verifica la inicialización de responseList.");
            }

            // Si se han mostrado todas las preguntas, reiniciar el registro de índices
            if (indicePreguntaUsada.Count >= responseList.Count)
            {
                indicePreguntaUsada.Clear();
            }

            // Obtener un índice aleatorio que no se haya utilizado
            do
            {
                randomQuestionIndex = Random.Range(0, responseList.Count);
            } while (indicePreguntaUsada.Contains(randomQuestionIndex));

            // Agregar el índice a la lista de utilizados
            indicePreguntaUsada.Add(randomQuestionIndex);

            // Obtener la pregunta y las respuestas
            var selectedQuestion = GameManager.Instance.responseList[randomQuestionIndex];
            _correctAnswer = selectedQuestion.CorrectOption;

            _answers.Add(selectedQuestion.Answer1);
            _answers.Add(selectedQuestion.Answer2);
            _answers.Add(selectedQuestion.Answer3);

            // Mezclar las respuestas
            _answers.Shuffle();

            // Mensajes de depuración para verificar las respuestas
            Debug.Log("Pregunta: " + selectedQuestion.QuestionText);
            Debug.Log("Respuestas mezcladas: " + string.Join(", ", _answers));

            // Asignar las respuestas a los botones, asegurando que no se accede a índices fuera de rango
            for (int i = 0; i < UIManagment.Instance._buttons.Length; i++)
            {
                if (i < _answers.Count) // Asegurar que hay suficientes respuestas
                {
                    UIManagment.Instance._buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = _answers[i];

                    int index = i; // Captura el valor actual de i en una variable local -- SINO NO FUNCA!
                    UIManagment.Instance._buttons[i].onClick.AddListener(() => UIManagment.Instance.OnButtonClick(index));
                }
                else
                {
                    // Limpiar texto y listeners de botones sobrantes
                    UIManagment.Instance._buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = string.Empty;
                    UIManagment.Instance._buttons[i].onClick.RemoveAllListeners();
                }
            }

            UIManagment.Instance.queryCalled = true;
        }
    }

    private int GetUniqueRandomQuestionIndex()
    {
        int index;
        do
        {
            index = Random.Range(0, GameManager.Instance.responseList.Count);
        } while (indicePreguntaUsada.Contains(index));

        return index;
    }

    private void ClearPreviousAnswers()
    {
        // Limpiar la lista de respuestas
        _answers.Clear();

        // Limpiar el texto y los listeners de los botones
        foreach (var button in UIManagment.Instance._buttons)
        {
            button.GetComponentInChildren<TextMeshProUGUI>().text = string.Empty;
            button.onClick.RemoveAllListeners();
        }
    }

    private void Update()
    {
    }
}

