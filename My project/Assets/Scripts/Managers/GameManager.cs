using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // ALmacenamos todos los nivelesa
    [SerializeField] private LevelManager[] levels;
    // Referencia al jugador
    [SerializeField] private PlayerController playerController;
    [SerializeField] private UIManager uIManager;
    // Referencia al nivel actual
    private LevelManager currentLevel;
    // Indice del nivel actual
    private int currentLevelIndex;
    // Property que mostrara de manera publuca la variable privada currntLevelIndex
    public int CurrentLevelIndex => currentLevelIndex;

    [Header("Camera")]
    // Referencia al camera tracker de la cámara
    [SerializeField] private CameraTracker cameraTracker;
    // Referenciia al autoMove de la cámara
    [SerializeField] private cameraAutoMove cameraAutoMove;

    private static GameManager instance;
    public static GameManager Instance => instance;

    private bool isPaused;
    public bool IsPaused => isPaused;


    private void OnEnable()
    {
        LevelManager.OnLevelCompleted += OnLevelCompleted;
    }

    private void Awake()
    {
        // SI no tenemos la instancia, la igualamos a la actual
        if (!instance) instance = this;
        // En otro caso la eliminamos
        else Destroy(gameObject);
    }

    private void Start()
    {
        //TODO: Cambiar al data manager
        currentLevelIndex = -1;
        OnLevelCompleted();
        cameraTracker.enabled = true;
        cameraAutoMove.enabled = false;
        // Ejecutamos al música del juego
        MusicManager.Instance.EnterGameMode();
    }

    private void OnDisable()
    {
        LevelManager.OnLevelCompleted -= OnLevelCompleted;
    }

    /// <summary>
    /// Se ejecutar� cuando el jugador presione el input de reset
    /// </summary>
    /// <param name="context"></param>
    public void OnReset(InputAction.CallbackContext context)
    {
        // Al presionar la tecla...
        if (context.started)
        {
            // Volvemos a cargar la escena que est� activa para que se resetee
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    /// <summary>
    /// Se ejecutará al presionar el inmput de pause
    /// </summary>
    /// <param name="context"></param>
    public void OnPause(InputAction.CallbackContext context)
    {
        // Actualizamos la pausa
        if (context.started) TogglePause();

    }

    /// <summary>
    /// lógica para pasar al siguiente nivel
    /// </summary>
    private void OnLevelCompleted()
    {
        // Destruimos el nivel actual
        if (currentLevel != null) Destroy(currentLevel.gameObject);
        // Actualizamos el índice
        currentLevelIndex++;
        // Si el índice está
        if (currentLevelIndex < levels.Length)
        {
            currentLevel = Instantiate(levels[currentLevelIndex], transform);
            playerController.transform.position = currentLevel.PlayerInitialPosition.position;
            playerController.Initialize();
        }
        else
        {
            // Llamamos al final del juego con 
            EndGame(true);
        }
        // En otro caso, hemos completado el uego porque no quedan más niveles
    }

    /// <summary>
    /// Lógica para reintentar un nivel
    /// </summary>
    public void RestartLevel()
    {
        // Si existe un nivel actual, lo destruimos 
        if (currentLevel != null) Destroy(currentLevel.gameObject);
        // Generamos el nivel
        currentLevel = Instantiate(levels[currentLevelIndex], transform);
        // Movemos el jugador a su inicio
        playerController.transform.position = currentLevel.PlayerInitialPosition.position;
        // Lo inicializamos de nuevo
        playerController.Initialize();
    }

    /// <summary>
    /// Método que gestionará el final del juego
    /// </summary>
    public void EndGame(bool win)
    {
        // Si es victoria...
        if (win)
        {
            uIManager.ShowWin();
            cameraTracker.enabled = false;
            cameraAutoMove.enabled = true;
            // 
            Destroy(playerController.gameObject);
        }
        // En otro caso es la derrota
        else
        {
            uIManager.ShowGameOver();
        }
    }

    /// <summary>
    /// Metodo que gestiona la pausa
    /// </summary>
    public void TogglePause()
    {
        // Cambiamos el estado de la pausa
        isPaused = !isPaused;
        // Hacemos que se pare o no el juego en base a si está o no en pausa
        Time.timeScale = isPaused ? 0f : 1f;
        // Si está en pause, mostramos el canvas de pausa
        if (isPaused) uIManager.ShowPause();
        // Si no, lo escondemos
        else uIManager.HidePause();
    }
}
