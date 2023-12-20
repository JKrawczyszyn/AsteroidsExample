using UnityEngine;

[CreateAssetMenu(fileName = "Config")]
public class Config : ScriptableObject
{
    [field: SerializeField] public Vector2Int GridSize { get; private set; }
    [field: SerializeField] public float TimeToSpawnSeconds { get; private set; }
    [field: SerializeField] public float CollisionDistance { get; private set; }
    [field: SerializeField] public int Seed { get; private set; }
}
