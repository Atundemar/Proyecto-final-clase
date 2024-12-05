using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    [Header("References")]
    // referencia al rigidbod
    [SerializeField] private Rigidbody2D rb2d = default;
    // referencia al collider
    [SerializeField] private Collider2D collider2d = default;
    // referencia al animator
    [SerializeField] private Animator animator = default;
    [SerializeField] private CameraTracker cameraTracker = default;
    // Referencia al trail renderer para el dash
    [SerializeField] private TrailRenderer trailRenderer = default;

    [Header("Movement settings")]
    // velocidad que llevará el jugador
    [SerializeField] private float speed = 1f;
    // Velocidad de caída máxima
    [SerializeField] private float maxFallSpeed = 4;

    [Header("Jump settings")]
    // FUerza de salto
    [SerializeField] private float jumpForce = 0.8f;
    // Multiplicador de la gravedad
    [SerializeField] private float gravityMultiplier = 1f;
    // Variable para controlar la cantidad de saltos que se pueden realizar
    [SerializeField] private int jumpInAirAllowed = 2;

    [Header("Shoot settings")]
    // Tiempo que habrá entre disparos
    [SerializeField] private float shootCadency;
    // Punto desde el que saldrá el disparo
    [SerializeField] private Transform shootPoint;

    [Header("Dash Settings")]
    // Fuerza del dash
    [SerializeField] private float dashForce;
    // Duracion del dash
    [SerializeField] private float dashTime;
    // Tiempo de recarga entre dashes
    [SerializeField] private float dashCooldown;

    [Header("Ground Check")]
    // Sitio ubcado en los pies del personaje que marcará la posición donde se hará  la comprobación de que está en el suelo
    [SerializeField] private Transform groundCheck = default;
    // Layer que tendrá el suelo
    [SerializeField] private LayerMask groundLayer = default;
    // Layer de enemigos
    [SerializeField] private LayerMask enemyLayer = default;
    // Tamaño que tendrá la caja de colisones que detectará si está o no en el suelo
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.16f, 0.03f);
    // Límite de altura en Y para que el jugador muera
    [SerializeField] private float yDeadLimit = -1.5f;

    // Si true, estará en el suelo
    private bool isGrounded;
    // Variable que contendrá el valor del input horizontal
    private float horizontalInput;
    // True si está mirando a la derecha, si mira a la izquierda será false
    private bool isFacingRight = true;
    // Saltos consecutivos realizados
    private int jumpInAirCounter = 0;
    // Boolana que controlará cuando esté muerto
    private bool isDead = false;
    // Boolana que controlará si se puede mover
    private bool canMove = true;
    // Variable que me guardará la animación actual
    private string currentAnimation;
    // Si está o no disparando
    private bool isShooting;
    // Si puede o no disparar
    private bool canShoot;
    // Indica si el jugador esta realizando un dash
    private bool isDashing;
    //Controla si el jugador puede o no hacer un dash
    private bool canDash = true;
    // Temporizador para el dash
    private float dashTimer = 0f;
    // Coeeutina que controlará el dash
    private Coroutine dashCoroutine;
    // Corrutina que controlará el disparo
    private Coroutine shootCoroutine;

    // Variable pública que será solo de lectura y me dirá el valor de la variable privada isDead
    public bool IsDead => isDead;
    // Array que contednrá todos los efectos de sonido
    private AudioData[] sfx;

    private void OnDrawGizmos()
    {
        // Si el groundCheck(Transform) no está asignado cortamosel dibujado de gizmos
        if (groundCheck == null) return;
        // Coloreamos el gizmo en rojo
        Gizmos.color = Color.red;
        // 
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }

    private void Awake()
    {
        CheckReferences();
        // Rellenamos el array de sfx con los audios que tengamos en el path de los ScriptableObjects
        // El path es siempre a partir de Resources
        sfx = Resources.LoadAll<AudioData>("ScriptableObjects/PlayerSounds");
    }

    private void Start()
    {
        Initialize();
    }
    public void Initialize()
    {
        // Le asignamos la gravedad al jugador
        rb2d.gravityScale = gravityMultiplier;
        // Reseteamos la velocidad del jugaor
        rb2d.linearVelocity = Vector3.zero;
        // Activamos el collider
        collider2d.enabled = true;
        // Nos aseguramos de que se puede mover
        canMove = true;
        // Decimos que puede disparar
        canShoot = true;
        // Decimos que al iniciarse no esta muerto
        isDead = false;
        // Hacemos que no pueda emitir
        trailRenderer.emitting = false;
        // Si se está ejecutando la corrutia del cooldown del dash, la detenemos
        if(dashCoroutine != null) StopCoroutine(dashCoroutine);
        // Hacemos qeu pueda volver a dashear
        canDash = true;
        ChangeAnimation(Constants.PLAYER_IDLE_ANIMATION);

    }


    private void Update()
    {
        // Si estamos dasheando...
        if (isDashing)
        {
            // Aumentamos el temporizador
            dashTimer += Time.deltaTime;
            // Si el temporizador es mayor o igual que la duración del dash
            if (dashTimer >= dashTime)
            {
                // Paramos el dash
                EndDash();
            }
            // Salimos del método
            return;
        }

        // Si el jugador esá muerto o dasheando, dejamos de ejecutar el update
        if (isDead) return;

        EnemyCheck();
        GroundCheck();

        if (isShooting) TryShoot();


        // Si está en el suelo y el input horizontal es diferente de 0, pasamos a la animación de correr
        if (isGrounded && horizontalInput != 0)
        {
            ChangeAnimation(isShooting ? Constants.PLAYER_ATTACK_ANIMATION : Constants.PLAYER_RUN_ANIMATION);
        }
        // Si está en el suelo y el nput horizontal es 0, pasamos a idle
        else if (isGrounded && horizontalInput == 0) ChangeAnimation(Constants.PLAYER_IDLE_ANIMATION);
        // Si no está en el suelo y la velocidad en Y es menor que 0, pasamos a la animación de caída
        else if (!isGrounded && rb2d.linearVelocityY < 0) ChangeAnimation(Constants.PLAYER_JUMP_ANIMATION);

        CheckYLimit();

    }

    private void FixedUpdate()
    {
        Move();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // Si el objeto con el que colisiono es un enemigo
        if (other.gameObject.CompareTag("Enemy"))
        {
            // Iniciamos la secuencia de la muerte del jugador
            Dead();
        }
    }

    /// <summary>
    /// Evento que se dispará desde el sitema de inputs al ejecutar la acción correspondiente, en este caso, el salto.
    /// </summary>
    /// <param name="context"></param>
    public void OnJump(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.IsPaused) return;
        if (context.started)
        {
            Jump();
        }
        // Si levantamos el input de salto...
        if (context.canceled)
        {
            // Si la velocidad sigue siendo positiva, es decir, estamos subiendo...
            // Reducimos la velocidad a la mitad para aparentar un salto regulable
            if (rb2d.linearVelocityY > 0) rb2d.linearVelocityY /= 2f;
        }
    }

    /// <summary>
    /// Evento que captura la entrada horizontal
    /// </summary>
    /// <param name="context"></param>
    public void OnMove(InputAction.CallbackContext context)
    {
        // Capturamos la entrada horizontal
        horizontalInput = context.ReadValue<float>();
    }


    public void OnShoot(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.IsPaused) return;
        // AL presionar el input
        if (context.started) isShooting = true;

        // Al levantar el input
        if (context.canceled) isShooting = false;

    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if(GameManager.Instance.IsPaused || isDead) return;
        // Al presionar el input realizamos el dash
        if (context.started) StartDash();
    }

    /// <summary>
    /// Método que comprobará si tenemos todas las referencias asignadas
    /// </summary>
    private void CheckReferences()
    {
        if (rb2d == null) rb2d = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        if (cameraTracker == null) cameraTracker = Camera.main.GetComponent<CameraTracker>();
        if (trailRenderer == null) trailRenderer = GetComponent<TrailRenderer>();
    }

    /// <summary>
    /// Método que aplica la fuerza de salto
    /// </summary>
    private void Jump()
    {
        // Si está murto, no puede saltar
        if (isDead) return;

        // Si el jugador está en el suelo o puede realizar un saalto en el aire
        if (isGrounded || jumpInAirCounter < jumpInAirAllowed)
        {
            if (jumpInAirCounter == 0) ChangeAnimation(Constants.PLAYER_JUMP_ANIMATION);
            else ChangeAnimation(Constants.PLAYER_DOUBLE_JUMP);
            // Resetamos la velocidad en el eje Y
            rb2d.linearVelocityY = 0f;
            // Aplicamos la fuerza de salto
            rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            // Si al saltar está en el suelo, hacmos la animación de salto simple
            if (isGrounded) ChangeAnimation(Constants.PLAYER_JUMP_ANIMATION);
            // Si no lo está significará que estamos en el aire y por ende, ejecutamos el doble salto
            else ChangeAnimation(Constants.PLAYER_DOUBLE_JUMP);

            if (!isGrounded) jumpInAirCounter++;
        }
    }

    /// <summary>
    ///   Actualiza el estado de grounded
    /// </summary>
    private void GroundCheck()
    {
        // Comprobamos el contacto con el suelo usando un overlap box indicando su posición, tamaño. ángulo de rotación en Z y layer mask
        Collider2D groundedCollider = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);
        // Si el groundCollider no es nulo, el estado de grounded será true, de lo contraio será false
        isGrounded = groundedCollider != null;
        // Si el jugador está grounded, no aplicamos gravedad, de lo contrario si
        rb2d.gravityScale = isGrounded ? 0f : gravityMultiplier;
        // Si tocamos el suelo, reseteamos el contador de saltos a 0
        if (isGrounded) jumpInAirCounter = 0;
    }

    /// <summary>
    /// Detectará cuando el jugador cae encima de un enemigo
    /// </summary>
    private void EnemyCheck()
    {
        // Comprobamos el contacto con el suelo usando un overlap box indicando su posición, tamaño, ángulo de rotación en Z y layer mask
        Collider2D enemyCollider = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, enemyLayer);
        // Si el enemy Collider no es nulo y tiene el script de eney...
        if (enemyCollider != null && enemyCollider.TryGetComponent(out Enemy enemy))
        {
            // Reseteamos el número de saltos
            jumpInAirCounter = 0;
            // Saltamos
            Jump();
            // Ejecutamos al muerte del enemigo
            enemy.Dead();
        }
    }

    /// <summary>
    /// Método que maneja el movimiento del jugador
    /// </summary>
    private void Move()
    {
        if(GameManager.Instance.IsPaused || !canMove) return;
        // Si no puede moverse, salimos del método
        if (!canMove) return;

        // Aplicamos la velocidad horizontal en base al input
        rb2d.linearVelocityX = horizontalInput * speed;

        // Si el input es positivo; es decir, estamos yendo a la derecha Y no estamos mirando a la derecha
        if (horizontalInput > 0 && !isFacingRight)
        {
            // Hacemos el flip
            Flip();
            // Si el nput es negativo y seguimos mirando a la derecha
        }
        else if (horizontalInput < 0 && isFacingRight)
        {
            // Hacemos el flip
            Flip();
        }
    }

    /// <summary>
    /// Método que controlará cuando el character será flipeado
    /// </summary>
    private void Flip()
    {
        // Modificamos el valor de isFacingRight
        isFacingRight = !isFacingRight;

        // Cogemos la rotación deljugador directamente
        Vector3 rotation = transform.rotation.eulerAngles;
        // Hacemos que si está girando a la derecha o no tenga una rotacion u otra
        rotation.y = isFacingRight ? 0 : 180;
        // Aplicamos de nuevo la rotación
        transform.rotation = Quaternion.Euler(rotation);
        // Actualizamos el offset para la cámara
        // cameraTracker.UpdateXOffset(isFacingRight ? 1 : -1);
    }

    /// <summary>
    /// Ejecuta las acciones de muerte del jugador
    /// </summary>
    private void Dead()
    {

        // Si ya está muerto, salimos del método
        if (isDead) return;

        // Marcamos que se ha muerto
        isDead = true;
        // Dejamos de aplicar el movimiento
        canMove = false;
        // Resetamos la velocidad a 0
        rb2d.linearVelocity = Vector3.zero;
        // Quitamnos el collider
        collider2d.enabled = false;
        // Quitamos la gravedad
        rb2d.gravityScale = 0f;
        // Animación de muerte
        ChangeAnimation(Constants.PLAYER_DEATH_ANIMATION);
        // Notificamos que le jugador se ha muerto cuando pase el tiempo oportuno
        Invoke("EndGame", 1f);
    }

    private void EndGame()
    {
        GameManager.Instance.EndGame(false);
    }

    /// <summary>
    /// Método que cambiará la animación del jugador
    /// </summary>
    /// <param name="nexAnimation"></param>
    private void ChangeAnimation(string nextAnimation)
    {
        // Si la animación que vamos a poner es la actual, no hacemos nada
        if (currentAnimation == nextAnimation) return;
        // Ejecutamos la animación que corresponda
        animator.Play(nextAnimation);
        // Actualizamos la animación actual
        currentAnimation = nextAnimation;
    }

    /// <summary>
    /// Método encarrgado de disparar cuando pueda
    /// </summary>
    private void TryShoot()
    {
        // Si no puede disparar, salimos del método
        if (!canShoot || horizontalInput == 0f) return;
        // Generamos una bala con el pool manager
        PoolManager.Instance.Pull("player.projectiles", shootPoint.position, shootPoint.rotation);
        // Si tene,mos una corrutina marchando, la detenemos
        if (shootCoroutine != null) StopCoroutine(shootCoroutine);
        // Iniciamos una corrutina nueva
        shootCoroutine = StartCoroutine(StartShooDelay());
    }

    /// <summary>
    /// Corrutina que manejará la espera para que se pueda volver a dispara
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartShooDelay()
    {
        // Deja de poder disparar
        canShoot = false;
        // Esperamos a lo que diga shootcadency
        yield return new WaitForSeconds(shootCadency);
        // Puede volver a disparar
        canShoot = true;
    }

    /// <summary>
    /// Cuando el jugador pase por la altura mínima permitida, se morirá
    /// </summary>
    private void CheckYLimit()
    {
        if (transform.position.y <= yDeadLimit) Dead();
    }

    /// <summary>
    ///  Método que iniciará el dash
    /// </summary>
    private void StartDash()
    {
        // Si no puede dashear, salimos del método
        if (!canDash) return;
        // Activamos el dash
        isDashing = true;
        // Desactiamos la capacdad de moverse de manera normal
        canMove = false;
        // Activampos el trail renderer
        trailRenderer.emitting = true;
        // Aplicamos la fuerza del dash en el eje x en la dirección correcta
        rb2d.linearVelocityX = isFacingRight ? dashForce : -dashForce;
        // Iniciamos el timer de dash
        dashTimer = 0f;
        // Desactivamos la capacidad para hacer un dash
        canDash = false;
        // Si la corrutina del dash está ejecutandose, la paramos
        if(dashCoroutine != null) StopCoroutine(dashCoroutine);
        // Iniciamos la corrutina de cooldown
        StartCoroutine(StartDashCooldown());
    }

    /// <summary>
    /// método para finalizar el dash
    /// </summary>
    private void EndDash()
    {
        // Terminamos el dash
        isDashing = false;
        // Restablecemos el jugador
        rb2d.linearVelocityX = 0f;
        // Desactivamos el trailRendeerer
        trailRenderer.emitting = false;
        // Permitimos el movimieto normal
        if (!isDead) canMove = true;
        // Si haceis animación de dash - aqui teneis que hacer el cambio de animación a la que sale
    }

    /// <summary>
    ///  Corrutina para manejar el cooldown entre dashes
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartDashCooldown()
    {
        // Esperamos el tiempo que nos diga el cooldown
        yield return new WaitForSeconds(dashCooldown);
        // Activamos de nuevo la capacidad de dashear
        canDash = true;
    }

    /// <summary>
    /// Devuelve un audio clip en base al nombre de los audios
    /// </summary>
    /// <param name="audioName"></param>
    /// <returns></returns>
    private AudioClip GetAudioClipByName(string audioName)
    {
        // Cogemos el audio data correspondiente en base al nombre, devolviendo 
        AudioData audioData = sfx.Where(a => a.AudioName == audioName).SingleOrDefault();
        // Devuelve uno de los audios que tenga asignado de forma aleatoria
        return audioData.Clips[Random.Range(0, audioData.Clips.Length)];
    }

    /// <summary>
    /// Límita la velocidad de caída
    /// </summary>
    private void LimitFallSpeed()
    {
        // Si el jugador está cayendo y no está en el suelo
        if(rb2d.linearVelocityY < 0 && !isGrounded)
        {
            // Limitamos la veocidad de caída máxima
            float clampedFallSpeed = Mathf.Clamp(rb2d.linearVelocityY, -maxFallSpeed, float.MaxValue);
            // Aplicamos la velocidad máxima
            rb2d.linearVelocityY = clampedFallSpeed;
        }
    }
}
