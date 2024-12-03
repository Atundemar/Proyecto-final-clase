using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Referencia al canvas principal
    [SerializeField] private CanvasGroup mainMenuCanvas;
    // Referencia al canvas del tutorial
    [SerializeField] private CanvasGroup tutorialCanvas;
    // Referencia al canvas de ajustes
    [SerializeField] private CanvasGroup settingsCanvas;
    // Referencia al canvas de créditos
    [SerializeField] private CanvasGroup creditsCanvas;
    // Referencia al audioMixer
    [SerializeField] private AudioMixer audioMixer;

    private void Start()
    {
        // Activamos el método
        ReturnToMenuButton();
        Time.timeScale = 1f;
        // Vamos a la música del menú
        MusicManager.Instance.EnterMenuMode();
    }

    /// <summary>
    /// Método que gestionará el botón de comenzar el juego
    /// </summary>
    public void StartButton()
    {
        SceneManager.LoadScene("Game");
    }

    /// <summary>
    /// Método que gestionará el botón de tutorial
    /// </summary>
    public void TutorialButton()
    {
        // Desactivamos el menú principal
        mainMenuCanvas.Toggle(false);
        // Activamos el canvas de tutorial
        tutorialCanvas.Toggle(true);

    }

    /// <summary>
    /// Método que gestionará el botón de ajustes
    /// </summary>
    public void SettingsButton()
    {
        // Desactivamos el menú principal
        mainMenuCanvas.Toggle(false);
        // Activamos el canvas de ajustes
        settingsCanvas.Toggle(true);

    }

    public void CreditsButton()
    {
        //
        mainMenuCanvas.Toggle(false);
        //
        creditsCanvas.Toggle(true);
    }
  

    /// <summary>
    /// Método que gestionará el botón de salir
    /// </summary>
    public void ExitButton()
    {
        // Si es una aplicación, salimos de la misma
#if UNITY_STANDALONE
    Application.Quit();
#endif
        // Si es el editor de unity, cortamos el play
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#endif

    }

    /// <summary>
    /// Método que gestionará el btónde vuelta a menú
    /// </summary>
    public void ReturnToMenuButton()
    {
        mainMenuCanvas.Toggle(true);
        tutorialCanvas.Toggle(false);
        settingsCanvas.Toggle(false);
        creditsCanvas.Toggle(false);
    }

    /// <summary>
    /// Ponemos o quitamos la pantalla completa del juego
    /// </summary>
    /// <param name="isFullScreen"></param>
    public void SetFullScreen(bool isFullScreen)
    {
        // Actualizamos la pantalla completa a lo que diga el toggle
        Screen.fullScreen = isFullScreen;
        // Forzamos la resolución máxima de la pantalla cuando sea pantalla completa
        if (isFullScreen) Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height,true);
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }
}
