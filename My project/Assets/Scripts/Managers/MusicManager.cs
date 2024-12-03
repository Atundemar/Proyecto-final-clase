using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    // Referencia al audio source
    [SerializeField] private AudioSource audioSource;
    // Clip del menu
    [SerializeField] private AudioClip menuClip;
    // Clips para el juego
    [SerializeField] private AudioClip[] gameClips;
    // Tiempo que tardará en hacer la transición
    [SerializeField, Range(1, 5)] private float fadeTime = 2f;
    // Tiempo que tardará en hacer la transición del pitch
    [SerializeField, Range(0, 4)] private float pitchTime = 1f;
    // Pitch al que se actualizará
    [SerializeField] private float pitchSlow = 0.6f;

    private Coroutine fadeCoroutine;
    private Coroutine pitchCoroutine;
    private AudioClip lastAudioClip;
    private bool isInGame = false;

    // Lectura pública desde cualquier sitio del Instance pero una scritura privada;
    // es decir, solo en este script
    public static MusicManager Instance { get; private set; }

    private void Awake()
    {
        // Si la instancia es nula...
        if (!Instance)
        {
            // actualizamos la instance con este objeto
            Instance = this;
            // Impedimos que se destruya al cargar escenas
            DontDestroyOnLoad(gameObject);
            // En otro caso, destruimos el objeto para evitar duplicaciones
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Si estamos en el juego y el clip ha terminado, cambiamos al siguiente
        if(isInGame && !audioSource.isPlaying && audioSource.clip != null)
        {
            PlayGame();
        }
    }

    /// <summary>
    /// Empieza a ejecutar la música de juego
    /// </summary>
    public void EnterGameMode()
    {
        isInGame = true;
        PlayGame();
    }

    /// <summary>
    /// Ejecuta la música del menú
    /// </summary>
    public void EnterMenuMode()
    {
        isInGame = false;
        PlayMainMenu();
    }

    /// <summary>
    /// Método encargado de ejecutar un audio clip
    /// </summary>
    /// <param name="clip"></param>
    private void PlayAudioClip(AudioClip clip)
    {
        // Si el clip que vamos a pober es el mismo que está sonando, salimos del método
        if (audioSource.clip == clip) return;
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(StartFadeAndChangeClip(clip));
    }

    /// <summary>
    /// Cambia el clip con un efecto de fade
    /// </summary>
    /// <param name="clip"></param>
    /// <returns></returns>
    private IEnumerator StartFadeAndChangeClip(AudioClip clip)
    {
        float counter = fadeTime / 2;
        while (counter > 0)
        {
            audioSource.volume = counter / (fadeTime / 2);
            counter -= Time.deltaTime;
            yield return null;
        }
        audioSource.clip = clip;
        audioSource.Play();
        while (counter < (fadeTime / 2))
        {
            audioSource.volume = counter / (fadeTime / 2);
            counter += Time.deltaTime;
            yield return null;
        }
    }

    private void PlayMainMenu()
    {
        PlayAudioClip(menuClip);
    }

    private void PlayGame()
    {
        // Si estamos en juego y tenemos clips...
        if (isInGame && gameClips.Length > 0)
        {
            // Filtramos los clips para evitar que se repita el mismo dos veces
            AudioClip[] avaliableClips = gameClips.Where(clip => clip != lastAudioClip).ToArray();
            // Si tenemos clips aleatorios en el array
            if (avaliableClips.Length > 0)
            {
                // Cogemos un clip aleatorio en el array
                AudioClip randomGameClip = avaliableClips[Random.Range(0, avaliableClips.Length)];
                // Actualizamos el útimo clip reproducido
                lastAudioClip = randomGameClip;
                // Reproducimos el clip
                PlayAudioClip(randomGameClip);
            }
            else
            {
                Debug.LogWarning("No hay clips disponibles para reproducir");
            }
        }
    }
}