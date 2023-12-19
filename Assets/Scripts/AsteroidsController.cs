using UnityEngine;

public class AsteroidsController
{
    private readonly AsteroidsModel _model;

    public AsteroidsController(AsteroidsModel model)
    {
        _model = model;

        AddAsteroidInCenterOfEachCell();
    }

    private void AddAsteroidInCenterOfEachCell()
    {
        Random.InitState(111);

        for (var x = 0; x < _model.CellsWidth; x++)
        {
            for (var y = 0; y < _model.CellsHeight; y++)
            {
                var localPosition = new Vector2(0.5f, 0.5f);
                var velocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized
                    * Random.Range(0.1f, 0.2f);
                _model.AddAsteroid(new Vector2Int(x, y), localPosition, velocity);
            }
        }
    }

    public void Update(float deltaTime)
    {
        _model.Update(deltaTime);
    }

    public int[] GetAsteroidIdsInCell(Vector2Int position)
    {
        return _model.GetAsteroidIdsInCell(position);
    }

    public Vector2 AsteroidLocalPosition(int index)
    {
        return _model.AsteroidLocalPosition(index);
    }
}
