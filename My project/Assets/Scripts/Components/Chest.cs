using UnityEngine;

public class Chest : MonoBehaviour
{

    [SerializeField] private GameObject prompt;
    [SerializeField] private ItemGenerator generator;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;
 
    private bool playerInside;
    // Si true, ya estará abierto
    private bool isOpened;

    private void Start()
    {
        prompt.SetActive(false);
    }

    private void Update()
    {
        // Si el jugador est� dentro de su radio de acci�n...
        if (playerInside)
        {
            // Hacemos que se muestre un canvas encima con la tecla que tiene que presionar

            // En el caso de que no est� abierto y el jugador presione la tecla Q...
            if (!isOpened && Input.GetKeyDown(KeyCode.Q))
            {
                // Abrimos el cofre
                isOpened = true;
                Debug.Log("Abrimos el cofre");
                // Generamos los items
                generator.GenerateItems();
                // Quitamos el indicador de la tecla
                prompt.SetActive(false);
                // Ejecutamos la animaci�n
                animator.SetTrigger(Constants.ANIM_CHEST_OPEN);
                // Reproducimos el sonido de apertura del cofre
                audioSource.Play();
            }
        }
    }

   private void OnTriggerEnter(Collider other)
    {
        // Si el objeto que entra en su trigger es el jugador...
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            // Si el cofre no est� ya abierto
            if (!isOpened) { 
                // Activamos la indicaci�n
                prompt.SetActive(true);
            }
        }


    }
    private void OnTriggerExit(Collider other)
    {
        // Si el objeto que sale de su trigger es el jugador...
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            // Si el prompt est� activo...
            if (prompt.activeSelf) { 
                // Lo desactivamos
                prompt.SetActive(true);
            
            }
        }
    }
}
