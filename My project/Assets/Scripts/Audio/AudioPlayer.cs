using UnityEngine;

public class AudioPlayer : PoolEntity
{
    // Referencia al audioSource
    [SerializeField] private AudioSource audioSource;
    // Tiepo en eel que tendr� que volver a la pool
    private float lifeTime;

    private void Awake()
    {
        if (!audioSource) audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // Si lleva m�s tiempo activo que su tiempo de vida, vuelve a la pool
        if (Time.time > lifeTime) ReturnPool();
    }
    /// <summary>
    /// Configura todo lo necesario para que ejecute el sonido
    /// </summary>
    /// <param name="clip"></param>
    public void Configure(AudioClip clip)
    {
        // Asignamos el clip
        audioSource.clip = clip;
        //
        audioSource.Play();
        // 
        lifeTime = audioSource.clip.length + Time.time;
    }
}
