using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GameContext : MonoBehaviour
{
    [SerializeField] private Config _config;

    public static AsteroidsController AsteroidsController;
    public static ShipModel ShipModel;

    private void Awake()
    {
        Application.targetFrameRate = -1;

        var asteroidsModel = new AsteroidsModel(_config.GridSize.x, _config.GridSize.y);
        AsteroidsController = new AsteroidsController(asteroidsModel);

        ShipModel = new ShipModel(Vector2.zero, Vector2.zero, Vector2.up, _config.GridSize.x, _config.GridSize.y);
    }
}
