using UnityEngine;
using System.Linq;


public class PoolManager : MonoBehaviour
{
    public Pool[] pools;

    public static PoolManager instance;
    public static PoolManager Instance => instance;

    private void Awake()
    {
        if (!instance) instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable() {
        PoolEntity.OnReturnToPool += Push;
    }

    private void Start()
    {
        InitializePools();
    }

    private void OnDisable() {
        PoolEntity.OnReturnToPool += Push;
    }

    /// <summary>
    /// inicia todas las pools
    /// </summary>
    private void InitializePools()
    {
        // Recorremos todas las pools
        foreach (Pool pool in pools)
        {
            // Repetiremos esta operaci�n tantas veces como prewarm se haya especificado
            for (int i = 0; i < pool.prewarm; i++)
            {
                // Instanciamos una nueva entidad
                PoolEntity temp = CreatePoolEntity(pool.id);
                // La dejamos desactivada
                temp.Deactivate();
                // La ponemos en la cola
                pool.pool.Enqueue(temp);
            }
        }
    }

    /// <summary>
    /// Crea un nuevo PoolENtity del pool indicado
    /// </summary>
    /// <param name="poolID"></param>
    /// <returns></returns>
        private PoolEntity CreatePoolEntity(string poolID)
    {
        // Variale para almacenar el nuevo entity generao
        PoolEntity entity = null;
        // Buscmamos la pool con el ID indiacado como par�metro
        Pool pool = pools.Where(s => s.id == poolID).FirstOrDefault();
        // Si encontramos la pool
        if (pool != null)
        {
            // Instanciamos el entit con el prefab de la pool
            entity = Instantiate(pool.prefab, transform);
            // Asignamos al nuevo entity el ID de la pool
            entity.poolID = pool.id;
        }
        // Devolvemos el nuevo entity generado
        return entity;
    }

    /// <summary>
    /// Vuelve a meter un entity en su pool correspondiente
    /// Esté método será el que ultilizaremos parasuscribirnos al Action de los PoolEntity
    /// </summary>
    /// <param name="entity"></param>
    public void Push(PoolEntity entity){
        // Intentamos recuperar el pool que cumple con la condición de tener elmismo ID que el entity recibido como parámetro
        Pool pool = pools.Where(s => s.id == entity.poolID).FirstOrDefault();
        // Si la pool no es nula, lo agregamos a la cola
        pool.pool.Enqueue(entity); // Es lo mismo que esto: // if (pool != null) pool.pool.Enqueue(entity);
    }

    /// <summary>
    /// Extrae un entity de la pool indicada y lo posiciona, rota y activa con los parámetros indicados
    /// </summary>
    /// <returns></returns>
    public PoolEntity Pull(string poolID, Vector3 position, Quaternion rotation){
        // Variable para contener el entity resultante
        PoolEntity entity = null;
        // Buscamos el pool que tenga el ID indicado
        Pool pool = pools.Where(s => s.id == poolID).SingleOrDefault();
        // Si existe la pool...
        if(pool != null){
            // Si no ha sido posible realizar un Dequeue (si no quedan elementos en la pool)
            if(!pool.pool.TryDequeue(out entity)){
                // Creamos un nuevo entity en su lugar y lo entregamos
                entity = CreatePoolEntity(pool.id);
            }
        }
        // Si tenemos un entity...
        if(entity != null){
            // Lo posicionamos, rotamos e inicializamos
            entity.transform.position = position;
            entity.transform.rotation = rotation;
            entity.Initialize();
        }
        // Devolvemos el entity
        return entity;
    }
}
