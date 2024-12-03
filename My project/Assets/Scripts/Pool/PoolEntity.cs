using System;
using UnityEngine;
[System.Serializable]   

public class PoolEntity : MonoBehaviour
{
    // Almacena la pool a la aque pertenece esta entidad
    [HideInInspector] public string poolID;
    // Para desactivar los elementos visuales del objeto
    public Renderer[] renderers;
    // Para saber si el iobjeto est� o no activo
    public bool active;
    // Action al que se suscribir� la pool, para capturar las entidaddes que vuelven a la pool
    public static Action<PoolEntity> OnReturnToPool;

    /// <summary>
    /// Acciones a realizar al extraer el objeto de la pool
    /// </summary>
    public virtual void Initialize()
    {
        active = true;
        EnableRenderers(true);
    }

    /// <summary>
    /// Activa o desactiva los elementos visuales del objeto
    /// </summary>
    /// <param name="enable"></param>
    private void EnableRenderers(bool enable){
        foreach (Renderer rend in renderers){
            rend.enabled = enable;
        }
    }

    /// <summary>
    /// Acciones para desactivar los renderers del entity
    /// </summary>
    public virtual void Deactivate(){
        active = false;
        EnableRenderers(false);
    }

    /// <summary>
    /// Desactiva el objeto e informa a la pool que quiere volver pasando su propia referencia  
    /// </summary>
    public void ReturnPool(){
        Deactivate();
        OnReturnToPool?.Invoke(this);
    }

    /// Localiza y almacena los renderers del entity
    [ContextMenu("FindRenderers")]

    public void FindRnderers(){
        renderers = GetComponentsInChildren<Renderer>();
    }
}

