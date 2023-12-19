using UnityEngine;
using UnityEngine.Assertions;

public class AsteroidsModel
{
    public readonly int CellsWidth;
    public readonly int CellsHeight;

    private readonly int[] _cellPositionsX, _cellPositionsY;
    private readonly float[] _localPositionsX, _localPositionsY;
    private readonly float[] _velocitiesX, _velocitiesY;
    private readonly float[] _timeToSpawn;

    private readonly CellModel[,] _cells;

    private int _currentId;

    public AsteroidsModel(int width, int height)
    {
        CellsWidth = width;
        CellsHeight = height;

        _cellPositionsX = new int[width * height];
        _cellPositionsY = new int[width * height];
        _localPositionsX = new float[width * height];
        _localPositionsY = new float[width * height];
        _velocitiesX = new float[width * height];
        _velocitiesY = new float[width * height];
        _timeToSpawn = new float[width * height];

        _cells = new CellModel[width, height];

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                _cells[x, y] = new CellModel();
            }
        }

        _currentId = 0;
    }

    public void AddAsteroid(Vector2Int cellPosition, Vector2 localPosition, Vector2 velocity)
    {
        Assert.IsTrue(localPosition.x is >= 0 and < 1f);
        Assert.IsTrue(localPosition.y is >= 0 and < 1f);

        int index = GetNextId();

        CellModel cellModel = GetCell(cellPosition);
        cellModel.AddAsteroid(index);

        _cellPositionsX[index] = cellPosition.x.Wrap(CellsWidth);
        _cellPositionsY[index] = cellPosition.y.Wrap(CellsHeight);
        _localPositionsX[index] = localPosition.x;
        _localPositionsY[index] = localPosition.y;
        _velocitiesX[index] = velocity.x;
        _velocitiesY[index] = velocity.y;
    }

    public int[] GetAsteroidIdsInCell(Vector2Int position)
    {
        return GetCell(position).Asteroids;
    }

    private CellModel GetCell(Vector2Int position)
    {
        return GetCell(position.x, position.y);
    }

    private CellModel GetCell(int x, int y)
    {
        return _cells[x.Wrap(CellsWidth), y.Wrap(CellsHeight)];
    }

    private int GetNextId()
    {
        return _currentId++;
    }

    public void Update(float deltaTime)
    {
        for (var i = 0; i < _currentId; i++)
        {
            if (_timeToSpawn[i] > 0f)
            {
                _timeToSpawn[i] -= deltaTime;

                continue;
            }

            UpdatePositions(i, deltaTime);

            CheckCollisions(i);
        }
    }

    private void UpdatePositions(int i, float deltaTime)
    {
        int x = _cellPositionsX[i];
        int y = _cellPositionsY[i];
        (int oldX, int oldY) = (x, y);

        float localPositionX = _localPositionsX[i];
        float localPositionY = _localPositionsY[i];

        localPositionX += _velocitiesX[i] * deltaTime;
        localPositionY += _velocitiesY[i] * deltaTime;

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

        _localPositionsX[i] = localPositionX;
        _localPositionsY[i] = localPositionY;

        if (oldX != x || oldY != y)
        {
            _cellPositionsX[i] = x;
            _cellPositionsY[i] = y;

            _cells[oldX, oldY].RemoveAsteroid(i);
            _cells[x, y].AddAsteroid(i);
        }
    }

    private void CheckCollisions(int i)
    {
        int x = _cellPositionsX[i];
        int y = _cellPositionsY[i];

        foreach (int asteroidId in _cells[x, y].Asteroids)
        {
            if (asteroidId == -1)
            {
                break;
            }

            if (asteroidId == i || _timeToSpawn[asteroidId] > 0f)
            {
                continue;
            }

            float sqrMagnitude = SqrMagnitude(
                _localPositionsX[i], _localPositionsY[i],
                _localPositionsX[asteroidId], _localPositionsY[asteroidId]);

            if (sqrMagnitude < 0.2f)
            {
                _timeToSpawn[i] = 1f;
                _timeToSpawn[asteroidId] = 1f;
            }
        }
    }

    private float SqrMagnitude(float x1, float y1, float x2, float y2)
    {
        float dx = x1 - x2;
        float dy = y1 - y2;

        return (dx * dx) + (dy * dy);
    }

    public Vector2 AsteroidLocalPosition(int index)
    {
        return new Vector2(_localPositionsX[index], _localPositionsY[index]);
    }
}
