using Unity.VisualScripting;
using UnityEngine;

public class FinalLevelTrigger : MonoBehaviour
{
   private void OnTriggerEnter2D(Collider2D other) {
    // Si el jugador entra en su trigger...
    if(other.CompareTag("Player")){
        // Ejecutamos el evento final de nivel
        LevelManager.OnLevelCompleted?.Invoke();
    }
   }
}
