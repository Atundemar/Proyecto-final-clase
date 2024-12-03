using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable Objects/Audio Data")]

public class AudioData : ScriptableObject
{
    [field: SerializeField] public string AudioName { get; private set;}
    [field: SerializeField] public AudioClip[] Clips { get; private set;}

}
