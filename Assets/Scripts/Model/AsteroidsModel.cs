using System;
using UnityEngine;
using UnityEngine.Assertions;

public class AsteroidsModel
{
    public event Func<(Vector2 position, Vector2 velocity)> OnGetRandomPositionAndVelocity;

    public readonly int CellsWidth;
    public readonly int CellsHeight;

    private readonly float _timeToSpawnSeconds;
    private readonly float _sqrCollisionDistance;

    private readonly int _cellsCount;
    // private readonly int _batchSize = 100;
    // private int _currentBatch = 0;

    private readonly int[] _cellPositionsX, _cellPositionsY;
    private readonly float[] _localPositionsX, _localPositionsY;
    private readonly float[] _velocitiesX, _velocitiesY;
    private readonly float[] _timesToSpawn;
    private readonly bool[] _isSpawned;

    private readonly CellModel[,] _cells;

    public AsteroidsModel(int width, int height, float timeToSpawnSeconds, float collisionDistance)
    {
        CellsWidth = width;
        CellsHeight = height;

        _timeToSpawnSeconds = timeToSpawnSeconds;
        _sqrCollisionDistance = collisionDistance * collisionDistance;

        _cellsCount = width * height;

        _cellPositionsX = new int[_cellsCount];
        _cellPositionsY = new int[_cellsCount];
        _localPositionsX = new float[_cellsCount];
        _localPositionsY = new float[_cellsCount];
        _velocitiesX = new float[_cellsCount];
        _velocitiesY = new float[_cellsCount];
        _timesToSpawn = new float[_cellsCount];
        _isSpawned = new bool[_cellsCount];

        _cells = new CellModel[width, height];

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                _cells[x, y] = new CellModel();
            }
        }
    }

    private void AddRandomAsteroid(int index)
    {
        // Assert.IsNotNull(OnGetRandomPositionAndVelocity, "OnGetRandomPositionAndVelocity is null.");

        (Vector2 position, Vector2 velocity) = OnGetRandomPositionAndVelocity();

        AddAsteroid(position, velocity, index);
    }

    public void AddAsteroid(Vector2 position, Vector2 velocity, int index)
    {
        Vector2Int cellPosition = new Vector2Int((int)position.x, (int)position.y);
        Vector2 localPosition = position - cellPosition;

        Assert.IsTrue(localPosition.x is >= 0 and < 1f);
        Assert.IsTrue(localPosition.y is >= 0 and < 1f);

        CellModel cellModel = GetCell(cellPosition);
        cellModel.AddAsteroid(index);

        _cellPositionsX[index] = cellPosition.x.Wrap(CellsWidth);
        _cellPositionsY[index] = cellPosition.y.Wrap(CellsHeight);
        _localPositionsX[index] = localPosition.x;
        _localPositionsY[index] = localPosition.y;
        _velocitiesX[index] = velocity.x;
        _velocitiesY[index] = velocity.y;
        _isSpawned[index] = true;
    }

    public int[] GetAsteroidIdsInCell(Vector2Int cellPosition)
    {
        return GetCell(cellPosition).Asteroids;
    }

    private CellModel GetCell(Vector2Int cellPosition)
    {
        return _cells[cellPosition.x.Wrap(CellsWidth), cellPosition.y.Wrap(CellsHeight)];
    }

    public void Update(float deltaTime)
    {
        for (var index = 0; index < _cellsCount; index++)
        {
            if (_timesToSpawn[index] > 0f)
            {
                _timesToSpawn[index] -= deltaTime;

                continue;
            }

            if (!_isSpawned[index])
            {
                AddRandomAsteroid(index);
            }

            UpdatePositions(index, deltaTime);

            CheckCollisions(index);
        }
    }

    public void FixedUpdate()
    {
        // int end = _batchSize * (_currentBatch + 1);
        // if (end > _cellsCount)
        // {
        // end = _cellsCount;
        // }

        // for (int index = _batchSize * _currentBatch; index < end; index++)
        for (int index = 0; index < _cellsCount; index++)
        {
            if (!_isSpawned[index])
            {
                continue;
            }

            CheckCollisions(index);
        }

        // _currentBatch++;
        //
        // Debug.Log($"_currentBatch {_currentBatch}");
        //
        // if (_batchSize * _currentBatch >= _cellsCount)
        // {
        //     _currentBatch = 0;
        // }
    }

    private void UpdatePositions(int index, float deltaTime)
    {
        int x = _cellPositionsX[index];
        int y = _cellPositionsY[index];
        (int oldX, int oldY) = (x, y);

        float localPositionX = _localPositionsX[index];
        float localPositionY = _localPositionsY[index];

        localPositionX += _velocitiesX[index] * deltaTime;
        localPositionY += _velocitiesY[index] * deltaTime;

        if (localPositionX < 0)
        {
            localPositionX += 1f;
            x = (x - 1).Wrap(CellsWidth);
        }
        else if (localPositionX >= 1f)
        {
            localPositionX -= 1f;
            x = (x + 1).Wrap(CellsWidth);
        }

        if (localPositionY < 0)
        {
            localPositionY += 1f;
            y = (y - 1).Wrap(CellsHeight);
        }
        else if (localPositionY >= 1f)
        {
            localPositionY -= 1f;
            y = (y + 1).Wrap(CellsHeight);
        }

        _localPositionsX[index] = localPositionX;
        _localPositionsY[index] = localPositionY;

        if (oldX != x || oldY != y)
        {
            _cellPositionsX[index] = x;
            _cellPositionsY[index] = y;

            _cells[oldX, oldY].RemoveAsteroid(index);
            _cells[x, y].AddAsteroid(index);
        }
    }

    private void CheckCollisions(int index)
    {
        int x = _cellPositionsX[index];
        int y = _cellPositionsY[index];

        CellModel cellModel = _cells[x, y];

        foreach (int asteroidId in cellModel.Asteroids)
        {
            if (asteroidId == -1)
            {
                break;
            }

            if (asteroidId == index || _timesToSpawn[asteroidId] > 0f)
            {
                continue;
            }

            float sqrMagnitude = SqrMagnitude(
                _localPositionsX[index], _localPositionsY[index],
                _localPositionsX[asteroidId], _localPositionsY[asteroidId]);

            if (sqrMagnitude <= _sqrCollisionDistance)
            {
                _isSpawned[asteroidId] = false;
                _timesToSpawn[asteroidId] = _timeToSpawnSeconds;

                cellModel.RemoveAsteroid(asteroidId);

                _isSpawned[index] = false;
                _timesToSpawn[index] = _timeToSpawnSeconds;

                cellModel.RemoveAsteroid(index);

                break;
            }
        }
    }

    private float SqrMagnitude(float x1, float y1, float x2, float y2)
    {
        float dx = x1 - x2;
        float dy = y1 - y2;

        return (dx * dx) + (dy * dy);
    }

    public Vector2 GetAsteroidLocalPosition(int index)
    {
        return new Vector2(_localPositionsX[index], _localPositionsY[index]);
    }
}
