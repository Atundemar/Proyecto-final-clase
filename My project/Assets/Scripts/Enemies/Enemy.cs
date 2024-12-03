using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    /// <summary>
    /// Método que se ejecutará cuando se muere el enemigo
    /// </summary>
    public virtual void Dead(){
        animator.SetBool("isDead", true);
        Destroy(gameObject, 0.2f);
    }
}
