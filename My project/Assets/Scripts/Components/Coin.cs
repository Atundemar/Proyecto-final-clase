using System;
using System.Xml.Serialization;
using UnityEngine;

public class Coin : MonoBehaviour, IPickable
{
    // Referencia al sistema de part�culas de la moneda
    [SerializeField] private ParticleSystem pickUpParticles;
    // Referencia al audio source que lanza� el sonido cuando la moneda sea obtenida
    [SerializeField] private AudioSource audioSource;

    public static Action OnCoinCollected;

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

    }
    public void PickUp()
    {
        // Lanzamos el evento de que una moneda ha sido recogida
        // El '?' es para que se lance siempre que tenga suscriptores
        OnCoinCollected?.Invoke();
        // Reproducimos el sonido
        audioSource.Play();
        // Generamos las part�culas en el sitio que est� la moneda y con la rotaci�n base que tuviera el sistema de part�culas
        Instantiate(pickUpParticles, transform.position, Quaternion.identity);
        // Desstruimos el gameObject; es decir, el objeto que tuviera el script, en este caso la moneda
        Destroy(gameObject, 0.2f);
    }

}
