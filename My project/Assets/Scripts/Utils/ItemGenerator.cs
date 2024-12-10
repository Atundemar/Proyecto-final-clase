using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemGenerator : MonoBehaviour
{
    // Objetos que puedo generar
    [SerializeField] private GameObject[] itemPrefabs;
    // Rádio de dispersión de los objetos generados (solo aparecerán en X y Z)
    [SerializeField] private float spawnRadius = 5f;
    // Offset para evitar que los objetos aparezcan en el centro (dentro del generador)
    [SerializeField] private float minOffset = 1f;
    // Cantidad de objetos a generar
    [SerializeField] private int spawnCount = 5;

    private void OnDrawGizmos()
    {

        Gizmos.color = new Color(255f, 0f, 0f);
        //Dibujamos el radio total
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
        Gizmos.color = new Color(255f, 0f, 255f);
        // Dibujamos el radio pequeño  de offset
        Gizmos.DrawWireSphere(transform.position, minOffset);
    }

    public void GenerateItems()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 randomPosition = GetRandomPositionOutsideOffset() + transform.position;
            Instantiate(itemPrefabs[Random.Range(0, itemPrefabs.Length)], randomPosition, Quaternion.identity);
        }
    }

    private Vector3 GetRandomPositionOutsideOffset()
    {
        Vector3 direction = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
        float distance = Random.Range(minOffset, spawnRadius);
        return direction * distance;

        //Calculamos un rango mayor al minOffset
        // float x = Random.Range(-effectiveRadius, effectiveRadius);
        // float z = Random.Range(-effectiveRadius, effectiveRadius);

        // Calculamos una posible posición para la moneda
        // Mientras la posición siga dentro del minOffset
        // while (new Vector2(x, z).magnitude < minOffset) 
        // {
        // Vamos volviendo a generar posibles posiciones
        //     x = Random.Range(-effectiveRadius, effectiveRadius);
        //     z = Random.Range(-effectiveRadius, effectiveRadius);

        // }
        // Devolvemos la posición final fuera del área de exclusión
        // return new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);
        // return new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);
    }
}
