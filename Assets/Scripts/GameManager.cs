using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<question> responseList = new List<question>();
    public List<int> indicePreguntaUsada = new List<int>();

    public int randomQuestionIndex;
    public List<string> _answers = new List<string>();

    public string _correctAnswer;
    public int currentTriviaIndex { get; set; }
    private Dictionary<string, int> categoryToTriviaId = new Dictionary<string, int>()
    {
        { "Historia", 1 },
        { "Ciencia", 2 },
        { "Arte", 3 },
        { "Geografia", 4 }
    };

    void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CategoryAndQuestionQuery(bool isCalled)
    {
        isCalled = UIManagment.Instance.queryCalled;

        if (!isCalled)
        {
            // Limpiar datos anteriores de la UI
            ClearPreviousAnswers();
            UIManagment.Instance._questionText.text = string.Empty;

            Debug.Log("Iniciando trivia con " + responseList.Count + " preguntas.");

            if (responseList.Count == 0)
            {
                Debug.LogError("responseList está vacío. Verifica la carga de preguntas.");
                return;
            }

            // Obtener el nombre de la categoría seleccionada
            string selectedCategory = PlayerPrefs.GetString("SelectedTrivia");

            // Mapear la categoría a trivia_id
            if (!categoryToTriviaId.TryGetValue(selectedCategory, out int selectedTriviaId))
            {
                Debug.LogError("Categoría no válida: " + selectedCategory);
                return;
            }

            // Filtrar preguntas por trivia_id
            List<question> filteredQuestions = responseList.FindAll(q => q.trivia_id == selectedTriviaId);

            if (filteredQuestions.Count == 0)
            {
                Debug.LogError("No se encontraron preguntas para el trivia_id seleccionado: " + selectedTriviaId);
                return;
            }

            // Si se han mostrado todas las preguntas, reinicia la lista de índices usados
            if (indicePreguntaUsada.Count >= filteredQuestions.Count)
            {
                indicePreguntaUsada.Clear();
            }

            // Obtener un índice aleatorio que no se haya usado
            do
            {
                randomQuestionIndex = Random.Range(0, filteredQuestions.Count);
            } while (indicePreguntaUsada.Contains(randomQuestionIndex));

            // Registrar el índice como usado
            indicePreguntaUsada.Add(randomQuestionIndex);

            // Obtener la pregunta seleccionada
            var selectedQuestion = filteredQuestions[randomQuestionIndex];
            _correctAnswer = selectedQuestion.CorrectOption;

            // Cargar respuestas
            _answers.Add(selectedQuestion.Answer1);
            _answers.Add(selectedQuestion.Answer2);
            _answers.Add(selectedQuestion.Answer3);

            // Mezclar las respuestas
            _answers.Shuffle();

            // Actualizar la UI con la pregunta
            UIManagment.Instance._questionText.text = selectedQuestion.QuestionText;
            Debug.Log("Pregunta mostrada: " + selectedQuestion.QuestionText);

            // Mostrar las respuestas en los botones
            for (int i = 0; i < UIManagment.Instance._buttons.Length; i++)
            {
                if (i < _answers.Count) // Verificar que haya suficientes respuestas
                {
                    UIManagment.Instance._buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = _answers[i];

                    // Capturar el índice para el botón
                    int index = i;
                    UIManagment.Instance._buttons[i].onClick.AddListener(() => UIManagment.Instance.OnButtonClick(index));
                }
                else
                {
                    // Limpiar texto y listeners de botones sobrantes
                    UIManagment.Instance._buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = string.Empty;
                    UIManagment.Instance._buttons[i].onClick.RemoveAllListeners();
                }
            }

            Debug.Log("Respuestas mostradas: " + string.Join(", ", _answers));
            UIManagment.Instance.queryCalled = true;
        }
    }

    private void ClearPreviousAnswers()
    {
        _answers.Clear();
        foreach (Button button in UIManagment.Instance._buttons)
        {
            button.onClick.RemoveAllListeners();
        }
    }
}
