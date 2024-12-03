using System.Collections;
using UnityEngine;

public class SpriteFlash : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Coroutine colorFlashCoroutine;
    
    private void Awake() {
        if (!spriteRenderer) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }    

    [ContextMenu("Test Color Flash")]
    public void TestColorFlash() => StartColorFlash(Color.black, 2.5f);

    /// <summary>
    /// Asigna el color flash e inicia la corrutina de recuperación durante el tiempo indicado
    /// </summary>
    /// <param name="color"></param>
    /// <param name="time"></param>
    private void StartColorFlash(Color color, float time){
        // Primero fijamos el color del flash
            spriteRenderer.color = color;
            // Si ya había un color recover previsto, lo cortamos
            if(colorFlashCoroutine != null) StopCoroutine(colorFlashCoroutine);
            // Iniciamos la corrutina que recupera gradualmente el color segun el tiempo indicado
            colorFlashCoroutine = StartCoroutine(StartColorRecover(time));

    }

    /// <summary>
    /// Recupera eñ color original en el tiempo indicado
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private IEnumerator StartColorRecover(float time){
        // Creamoos una variable para contar el tiempo de recuperación
        float timeElapsed = 0f;
        // Almacenamos el color antes de comenzar el cambio
        Color startColor = spriteRenderer.color;
        
        // Mientras el tiempo no llegue a su fin repetiremos el bucle sin parar
        while(timeElapsed < time){
            // Actualizaremos el color del sprite
            spriteRenderer.color = Color.Lerp(startColor, Color.white, timeElapsed / time);
            // Aumentamos el tiempo transcurrido
            timeElapsed += Time.deltaTime;
            // Esperamos hasta que termine el 
            yield return new WaitForEndOfFrame();
        }

    }
}
