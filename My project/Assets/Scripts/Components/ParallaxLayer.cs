using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    // Velocidad del fondo, con el valor máximo igual a la velocidad de la cámara
    [SerializeField] private float speedFactor = 0.06f;
    // Posición para controllar el offset de la textura
    private Vector2 pos = Vector2.zero;
    // Posición anterior de la cámara
    private Vector2 camLastPosition;
    // Referencia a la cámara
    [SerializeField] private Camera cam;
    // Referencia al renderer
    [SerializeField] private Renderer rend;
    [SerializeField] private bool autoMove;
    [SerializeField] private bool autoMoveSpeed;

    private void Awake() {
        if (cam == null) cam =  Camera.main;
        if (rend == null) rend = GetComponent<Renderer>();
    }

    private void Start() {
        // inicializamos la posición de la cámara
        camLastPosition = cam.transform.position;
        // Calculamos el tamaño que tendrá el fondo en base a la pantalla
        Vector2 backgroundHalfSize = new Vector2(
                                    (cam.orthographicSize * Screen.width) / Screen.height,
                                    cam.orthographicSize);
                                    // Ajustamos la escala del fondo para que se ajuste al tamaño de la pantalla
        transform.localScale = new Vector3(backgroundHalfSize.x * 2f,
                                            backgroundHalfSize.y * 2f,
                                            transform.localScale.z
        );
    // Ajustmaos el tilling para que sea proporcionado de forma corrercta en la escala
    // Lo dejamos a la mitad para reducir el número de repetciones ya que ofrece un resultadop 
    // más estético
    rend.material.SetTextureScale("_MainTex", backgroundHalfSize);
    }

    private void Update() {

        // Calculamos el desplazamieno de a ´camara respecto al frame anterior
        Vector2 camVariation = new Vector2(cam.transform.position.x - camLastPosition.x,
                                            cam.transform.position.y - camLastPosition.y);

        /// Modificamos el offset que se aplicará a la textura aplicando la atenuación 
        /// de movimiento indicado por parámetro
        pos.x += camVariation.x * speedFactor;
        pos.y += camVariation.y * speedFactor;
        // Aplicamos el offset a la textura
        rend.material.mainTextureOffset = pos;
        // Actualizamos la posición de la cámara
        camLastPosition = cam.transform.position;
    }
}
