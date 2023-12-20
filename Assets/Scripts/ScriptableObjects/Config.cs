using UnityEngine;

[CreateAssetMenu(fileName = "Config")]
public class Config : ScriptableObject
{
    [field: SerializeField] public Vector2Int GridSize { get; private set; }
}
