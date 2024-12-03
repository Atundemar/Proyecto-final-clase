using UnityEngine;

public class PlayerProjectile : PoolEntity
{
    [Header("Components")]
    // Referencia al collider
    [SerializeField] private Collider2D col;
    // referencia al rigidbody
    [SerializeField] private Rigidbody2D rb2d;
    
    [Header("Settigs")]
    // Velocidad del proyectil
    [SerializeField] private float speed = 10;
    // Tiempo de vida del proyectil
    [SerializeField] private float lifeTime = 3;
    // Layer que tendrá todo lo que es disparable
    [SerializeField] private LayerMask shootableLayer;

    private float lifeTimeStamp;

    private void Update() {
        if(lifeTimeStamp < Time.time && active) ReturnPool();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // Suma binaria
        // para que sea 1 en el resultado debe ser ambos numeros 1 (ej: 1 y 1 = 1, 0 y 0 = 0, 1 y 0 = 0)
        // 11111000
        // 10010010
        // --------
        // 10010000
        // Si el resultado final fuera 0 completamente -> significa que el layer no está contenido en la máscara

        if ((shootableLayer & (1 << other.gameObject.layer)) != 0){
            
            // Cogemos el enemigo para matarlo
            if(other.TryGetComponent(out Enemy enemy)){
                enemy.Dead();
            }
        }
        // Volvemos a la pool
        ReturnPool();
    }


    public override void Initialize()
    {
        base.Initialize();
        col.enabled = true;
        rb2d.linearVelocity = transform.right * speed;
        lifeTimeStamp = Time.time + lifeTime;
        
    }

    public override void Deactivate()
    {
        base.Deactivate();
        col.enabled = false;
    }
}
