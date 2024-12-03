using UnityEngine;

public class cameraAutoMove : MonoBehaviour
{
    [SerializeField] private float speed = 0.5f;

    private void Update(){
        transform.position += Vector3.right * speed * Time.deltaTime;
    }
}
