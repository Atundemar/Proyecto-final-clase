using UnityEngine;

public class EnemyJumper : Enemy
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb2d;

    [Header("Configuration")]
    // Fuerza de salto
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float timeBetweenJumps = 1.4f;
    [SerializeField] private float groundCheckHeight = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    
    private bool isGrounded;
    private float jumpTimer;

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckHeight));
    }

    private void Update() {
        GroundCheck();

        // Si el tiempo que lleva la ejecución es mayor a mi temporizador de salto...
        if(Time.time >= jumpTimer){
            // Actualizaremos el temporizador de salto sumándole el tiempo de ejecución actual + o
            jumpTimer = Time.time + timeBetweenJumps;
            // 
            Jump();

        }
        UpdateAnimator();
    }

    /// <summary>
    /// Método que actualiza el estado de grounded
    /// </summary>
    private void GroundCheck(){
        // Realizamos un Linecast con el layer mask del suelo para identificar si el enemugi está tocando o no el suelo
        isGrounded = Physics2D.Linecast(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckHeight), groundLayer);

    }

   /// <summary>
   /// Método que actualizará el estado del animator
   /// </summary>
    private void UpdateAnimator(){
    animator.SetBool("isGrounded", isGrounded); 
    }

    /// <summary>
    /// Ejecuta el salto aplicando una fuerza de impulso
    /// </summary>
    private void Jump(){
        // Si no está en el suelo, salimos del método
        if (!isGrounded) return;
        // Aplicamos la fuerza vertical
        rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
}
