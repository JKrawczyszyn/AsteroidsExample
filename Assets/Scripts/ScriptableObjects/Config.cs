using UnityEngine;

[CreateAssetMenu(fileName = "Config")]
public class Config : ScriptableObject
{
    [field: SerializeField]
    public Vector2Int GridSize { get; private set; }

    [field: SerializeField]
    public float TimeToSpawnSeconds { get; private set; }

    [field: SerializeField]
    public float CollisionDistance { get; private set; }

    [field: SerializeField, Tooltip(
                "Asteroids outside of camera are updated less frequently."
                + "This is the value asteroid count is divided by"
                + " to get the number of asteroids outside of camera that are updated in one frame.")]
    public int Parts { get; private set; }

    [field: SerializeField]
    public int Seed { get; private set; }
}
