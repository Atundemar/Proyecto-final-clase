using UnityEngine;

public class CameraTracker : MonoBehaviour
{
    // Transform del objeto a seguir, eas un valor por referencia por lo que siempre tendremos elvalor actualizdo
    [SerializeField] private Transform target;
    // Booleana que indica si la cámara hará el seguimiento en el eje X
    [SerializeField] private bool followX;
    // Booleana que indica si la cámara hará el sdeguimento en el eje Y
    [SerializeField] private bool followY;
    // Desviación de la posición en X 
    [SerializeField, Range(-5, 5)] private float offsetX = 1;
    // Desviación de la posición en Y
    [SerializeField, Range(-5, 5)] private float offsetY = 1;
    [SerializeField] private float lerpSpeed = 0.2f;
    // Variable que tendrá la posición a la que tiene que ir la cámara
    private Vector3 nextPosition;
    private float nextOffset;
    private float xOffsetToApply;

    private void Update() {
        // Cogemos la posición actual de la cámara
        nextPosition = transform.position;
        // Si la cámara tiene que seguir en el ejeX, ajustamos la posición en X
            // en base a la posición del target más el offset en x 
        if (followX){
            // En la variable xOffseTtoApply guardaremos el valor que vaya cogiendo del Lerp
            xOffsetToApply = Mathf.Lerp(xOffsetToApply, nextOffset, lerpSpeed * Time.deltaTime);
            nextPosition.x = target.position.x + xOffsetToApply;
        }
            // Si la cámara tiene que seguir en el ejeY, ajustamos la posición en X
            // en base a la posición del target más el offset en y
        if (followY){
            nextPosition.y = target.position.y + offsetY;
        }
        // Asignamos la posición
        transform.position = nextPosition;
    }

    /// <summary>
    /// Método que actualizará el offset en la X para dejar más sitio de visión hacia el lugar que estamos yendo
    /// </summary>
    /// <param name="newOffset"></param>
     public void UpdateXOffset(float newOffsetSign){
        // Le damos el valor pertinente al nextOffset
        // Es decir, el offset que esté consigurado en la X con el signo cambiado
        offsetX = offsetX * Mathf.Sign(newOffsetSign);
    }
}
