using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManagment : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _categoryText;
    [SerializeField] public TextMeshProUGUI _questionText;

    public Button[] _buttons = new Button[3];
    [SerializeField] Button _backButton;

    public bool queryCalled;

    public static UIManagment Instance { get; private set; }

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

    private void Update()
    {
        _categoryText.text = PlayerPrefs.GetString("SelectedTrivia");

        // Verificación de null antes de acceder a questionText
        if (_questionText != null && GameManager.Instance != null)
        {
            GameManager.Instance.CategoryAndQuestionQuery(queryCalled);
        }
        else
        {
            Debug.LogWarning("questionText o GameManager no están correctamente asignados.");
        }
    }

    public void OnButtonClick(int buttonIndex)
    {
        string selectedAnswer = _buttons[buttonIndex].GetComponentInChildren<TextMeshProUGUI>().text;

        if (selectedAnswer == GameManager.Instance._correctAnswer)
        {
            Debug.Log("Respuesta correcta!");
        }
        else
        {
            Debug.Log("Respuesta incorrecta. Inténtalo de nuevo.");
        }
    }

    public void PreviousScene()
    {
        Destroy(GameManager.Instance.gameObject);
        Destroy(UIManagment.Instance.gameObject);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
