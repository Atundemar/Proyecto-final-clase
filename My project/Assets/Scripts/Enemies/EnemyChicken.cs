using UnityEngine;
using System.Collections;

public class EnemyChicken : MonoBehaviour
{
    [Header("References")]
    // Guardaremos todos los waypoints
    [SerializeField] private Transform[] waypoints;

    [Header("Configuration")]
    // Velocidad de movimiento
    [SerializeField] private float moveSpeed;
    // Velocidad de rotación
    [SerializeField] private float rotateSpeed;
    // Si true, hará la rotación; si no, simplemente hará el flip (cuando sea solo movimiento horizontal)
    [SerializeField] private bool canRotate;
    // Distancia para detectr cuando tendrá que cambiar la dirección
    [SerializeField] private float flipThreshold;
    // Siguiente waypoint al que tiene que ir
    private int currentWaypointIndex = 1;
    // Si true, se estará moviendo
    private bool isMoving = true;
    // Controlamos la corrutina de rotación
    private Coroutine rotationCoroutine;

    private void Start() {
        if (waypoints == null || waypoints.Length == 0){
            // Nos aseguramos que tengamos waypoints definidos
            Debug.Log($"No se han asignado los waypoints de: {transform.parent.name}.");
            //Desactivamos el componenre para eitar errores por consola
            enabled = false;
        }
    }
    
    private void Update() {
        HandleMovement();
    }

/// <summary>
/// Maneja el movimiento entre waypoints y la rotación si es necesaria
/// </summary>
    private void HandleMovement(){
        MoveTowardsWaypoint();
        
        // Si estamos cerca del waypoint...
        if (Vector2.Distance(transform.position, waypoints[currentWaypointIndex].position)<= flipThreshold){

        // Lo detenemos
        isMoving = false;
        // Posicionamos exactamente el enemigo en el waypoint
        transform.position = waypoints[currentWaypointIndex].position;

            // Si el enemigo puede rotar...
            if (canRotate){
                // Si tenemos la corrutina de rotación empezada, la detenemos
                if (rotationCoroutine != null) StopCoroutine(rotationCoroutine);
                // Empezamos la corrutina
                rotationCoroutine = StartCoroutine(StartTowardsWaypointRotation(waypoints[currentWaypointIndex].localEulerAngles));
            } else {
                // Al no rotar, igualamos directamente su dirección
                transform.rotation = waypoints[currentWaypointIndex].rotation;
                // Decemos que se puede mover de nuevo
                isMoving = true;
                // Actualizamos el waypoint
                UpdateNextWaypoint();
            }
        }
    }

/// <summary>
/// Realiza un movimiento suave hacia el waypoint objetivo
/// </summary>
    private void MoveTowardsWaypoint(){
        // Si está en movimiento...
        if (isMoving){
        // Se mueve al siguiente waypoibnt con una velocidad constante
        transform.position = Vector2.MoveTowards(transform.position, waypoints[currentWaypointIndex].position, moveSpeed * Time.deltaTime);
        
        }
    }

    /// <summary>
    /// Realiza una rotación suave hacie le objetivo especificado
    /// </summary>
    /// <param name="targetRotation"></param>
    /// <returns></returns>
    private IEnumerator StartTowardsWaypointRotation(Vector3 targetRotation){
        // Convertimos en Quaternion el vector de rotación
        Quaternion targetQuaternion = Quaternion.Euler(targetRotation);
       // Continuamos la rotación hasta alcanzar el objetivo
        while (Quaternion.Angle(transform.rotation, targetQuaternion) > 0.1f){
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetQuaternion, rotateSpeed * Time.deltaTime);
            yield return null;
        }
        // Aseguramos que la rotación final es precisa
        transform.rotation = targetQuaternion;
        //
        isMoving = true;
        //
        UpdateNextWaypoint();
    }

    /// <summary>
    /// Actualiza el índice del siguiente waypoint para crear el patrullaje cíclico.
    /// </summary>
    private void UpdateNextWaypoint(){
        currentWaypointIndex = (currentWaypointIndex +1) % waypoints.Length;
    }
}
