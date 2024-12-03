using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SplashManager : MonoBehaviour
{
    // Nombre de la escena que cargará cuando termine este splash
    [SerializeField] private string sceneNameAfterSplash = "Game";
    // Duración del splash
    [SerializeField, Range (0, 5)] private float splashDuration = 3f;
    // Referencia a la imagen que hará el fade
    [SerializeField] private Image fadeImage;
    // Gradiente para configurar el proceso del fade
    [SerializeField] private Gradient fadeGradient;

    private float timer = 0f;

    private void Update() {
        // Incrementamos el tiempo en cada frame
        timer += Time.deltaTime;
        // Evaluamos el color que debe tener la imagen del fade según el gradiente y el tiempo transcurrido
        fadeImage.color = fadeGradient.Evaluate(timer / splashDuration);
        // 
        if(timer >= splashDuration){
            SceneManager.LoadScene(sceneNameAfterSplash);
        }
    }
}
