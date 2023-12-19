using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class AsteroidsModel
{
    private readonly CellModel[,] _cells;

    private readonly int[] _cellPositionsX, _cellPositionsY;
    private readonly float[] _localPositionsX, _localPositionsY;
    private readonly float[] _velocitiesX, _velocitiesY;

    private int _currentId;

    public int CellsWidth { get; }
    public int CellsHeight { get; }

    public AsteroidsModel(int width, int height)
    {
        CellsWidth = width;
        CellsHeight = height;

        _cells = new CellModel[width, height];

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                _cells[x, y] = new CellModel();
            }
        }

        _cellPositionsX = new int[width * height];
        _cellPositionsY = new int[width * height];
        _localPositionsX = new float[width * height];
        _localPositionsY = new float[width * height];
        _velocitiesX = new float[width * height];
        _velocitiesY = new float[width * height];

        _currentId = 0;
    }

    public void AddAsteroid(Vector2Int cellPosition, Vector2 localPosition, Vector2 velocity)
    {
        Assert.IsTrue(localPosition.x is >= 0 and < 1f);
        Assert.IsTrue(localPosition.y is >= 0 and < 1f);

        int index = GetNextId();

        CellModel cellModel = GetCell(cellPosition);
        cellModel.AddAsteroid(index);

        _cellPositionsX[index] = cellPosition.x;
        _cellPositionsY[index] = cellPosition.y;
        _localPositionsX[index] = localPosition.x;
        _localPositionsY[index] = localPosition.y;
        _velocitiesX[index] = velocity.x;
        _velocitiesY[index] = velocity.y;
    }

    public IEnumerable<int> GetAsteroidIdsInCell(Vector2Int position)
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
            _localPositionsX[i] += _velocitiesX[i] * deltaTime;
            _localPositionsY[i] += _velocitiesY[i] * deltaTime;

            int x = _cellPositionsX[i];
            int y = _cellPositionsY[i];
            (int oldX, int oldY) = (x, y);

            float localPositionX = _localPositionsX[i];
            float localPositionY = _localPositionsY[i];

            if (localPositionX < 0)
            {
                localPositionX += 1f;
                x--;
            }
            else if (localPositionX >= 1f)
            {
                localPositionX -= 1f;
                x++;
            }

            if (localPositionY < 0)
            {
                localPositionY += 1f;
                y--;
            }
            else if (localPositionY >= 1f)
            {
                localPositionY -= 1f;
                y++;
            }

            if (oldX == x && oldY == y)
                continue;

            _localPositionsX[i] = localPositionX;
            _localPositionsY[i] = localPositionY;
            _cellPositionsX[i] = x;
            _cellPositionsY[i] = y;

            GetCell(oldX, oldY).RemoveAsteroid(i);
            GetCell(x, y).AddAsteroid(i);
        }
    }

    public Vector2 AsteroidLocalPosition(int index)
    {
        return new Vector2(_localPositionsX[index], _localPositionsY[index]);
    }
}
