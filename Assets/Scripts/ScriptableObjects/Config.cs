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
                + "Asteroid count is divided by this value "
                + " to get the number of asteroids outside of camera that are updated in one frame.")]
    public int Parts { get; private set; }

    [field: SerializeField]
    public int Seed { get; private set; }

    [field: SerializeField]
    public float Acceleration { get; private set; }

    [field: SerializeField]
    public float MaxVelocity { get; private set; }

    [field: SerializeField]
    public float Rotation { get; private set; }

    [field: SerializeField]
    public float ShootDelay { get; private set; }

    [field: SerializeField]
    public float BulletSpeed { get; private set; }

    [field: SerializeField]
    public float BulletDestroyTime { get; private set; }
}
