using UnityEngine;

[DefaultExecutionOrder(-1)]
public class GameContext : MonoBehaviour
{
    public static AsteroidsController AsteroidsController;
    public static ShipModel ShipModel;

    private void Awake()
    {
        Application.targetFrameRate = 300;

        var asteroidsModel = new AsteroidsModel(512, 512);
        AsteroidsController = new AsteroidsController(asteroidsModel);

        ShipModel = new ShipModel(Vector2.zero, Vector2.zero, Vector2.up, 512, 512);
    }
}
