using System.Collections.Generic;
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
        for (var x = 0; x < _model.CellsWidth; x++)
        {
            for (var y = 0; y < _model.CellsHeight; y++)
            {
                _model.AddAsteroid(new Vector2Int(x, y), new Vector2(0.5f, 0.5f), Vector2.zero);
            }
        }
    }

    public void Update(float deltaTime)
    {
        _model.Update(deltaTime);
    }

    public IEnumerable<int> GetAsteroidIdsInCell(Vector2Int position)
    {
        return _model.GetAsteroidIdsInCell(position);
    }

    public Vector2 AsteroidLocalPosition(int index)
    {
        return _model.AsteroidLocalPosition(index);
    }
}
