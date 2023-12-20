using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AsteroidsController : IDisposable
{
    private readonly AsteroidsModel _model;

    public AsteroidsController(AsteroidsModel model, int seed)
    {
        _model = model;

        _model.OnGetRandomPositionAndVelocity += GetRandomPositionAndVelocity;

        Random.InitState(seed);

        AddAsteroidInCenterOfEachCell();
    }

    public void Dispose()
    {
        _model.OnGetRandomPositionAndVelocity -= GetRandomPositionAndVelocity;
    }

    private (Vector2 position, Vector2 velocity) GetRandomPositionAndVelocity()
    {
        return (GetRandomPosition(), GetRandomVelocity());
    }

    private void AddAsteroidInCenterOfEachCell()
    {
        var index = 0;

        for (var x = 0; x < _model.CellsWidth; x++)
        {
            for (var y = 0; y < _model.CellsHeight; y++)
            {
                var position = new Vector2(x + 0.5f, y + 0.5f);
                var velocity = GetRandomVelocity();

                Create(position, velocity, index);

                index++;
            }
        }
    }

    private Vector2 GetRandomPosition()
    {
        return new Vector2(Random.Range(0f, _model.CellsWidth), Random.Range(0f, _model.CellsHeight));
    }

    private Vector2 GetRandomVelocity()
    {
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * Random.Range(0.1f, 0.2f);
    }

    private void Create(Vector2 position, Vector2 velocity, int index)
    {
        _model.AddAsteroid(position, velocity, index);
    }

    public void Update(float deltaTime)
    {
        _model.Update(deltaTime);
    }

    public void FixedUpdate()
    {
        _model.FixedUpdate();
    }

    public int[] GetAsteroidIdsInCell(Vector2Int cellPosition)
    {
        return _model.GetAsteroidIdsInCell(cellPosition);
    }

    public Vector2 GetAsteroidLocalPosition(int index)
    {
        return _model.GetAsteroidLocalPosition(index);
    }
}
