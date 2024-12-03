using UnityEngine;
using System;

public class LevelManager : MonoBehaviour
{
    // Posición en la que iniciará el jugador
    [field: SerializeField] public Transform PlayerInitialPosition{ get; private set;}
    public static Action OnLevelCompleted;

}
