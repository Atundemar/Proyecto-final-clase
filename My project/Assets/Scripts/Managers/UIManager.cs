using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private readonly string gameOverLevelTextFormat = "Current Level: {0}";
    [Header("Game over")]
    [SerializeField] private CanvasGroup gameOverCanvas;
    [SerializeField] private TMP_Text gameOverLevelText;

    [Header("Win")]
    [SerializeField] private CanvasGroup winCanvas;

    [Header("Pause")]
    [SerializeField] private CanvasGroup pauseCanvas;

    private void Start()
    {
        gameOverCanvas.Toggle(false);
        winCanvas.Toggle(false);
        pauseCanvas.Toggle(false);
    }

    /// <summary>
    /// Método encargado de enseñar lo necesario en pantalla de victoria
    /// </summary>
    public void ShowWin()
    {
        // Activamos el canvas de la victoria
        winCanvas.Toggle(true);
    }

    /// <summary>
    /// Método encargado de enseñar todo lo necesario en la pantalla
    /// </summary>
    public void ShowGameOver()
    {
        // Mostramos el canvas
        gameOverCanvas.Toggle(true);
        // Actualizamos el texto del nivel en el que te has quedado en base al indice actual que tenga el GameManager
        gameOverLevelText.text = string.Format(gameOverLevelTextFormat, GameManager.Instance.CurrentLevelIndex + 1);
    }

    public void ReturnToMenuButton()
    {
        
        SceneManager.LoadScene("MainMenu");
    }


    public void RestartLevel()
    {
        // Llamamos al método para reiniciar el nivel
        GameManager.Instance.RestartLevel();
        // Desactivamos el canvas de derrota
        gameOverCanvas.Toggle(false);
    }

    /// <summary>
    /// Méwtodo para volver al juego tras la pausa
    /// </summary>
    public void ResumeButton()
    {
        GameManager.Instance.TogglePause();
    }

    /// <summary>
    /// Método para mostrar todo lo necesario en cuanto a la pausa en pantalla
    /// </summary>
    public void ShowPause()
    {
        pauseCanvas.Toggle(true);
    }

    /// <summary>
    /// Método para esconder todo lo necesario en cuento a la pausa en pantalla
    /// </summary>
    public void HidePause()
    {
        pauseCanvas.Toggle(false );
    }
}
