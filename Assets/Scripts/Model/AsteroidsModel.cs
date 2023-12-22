using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Model
{
    public class AsteroidsModel
    {
        public event Func<(Vector2 position, Vector2 velocity)> OnGetRandomPositionAndVelocity;

        public readonly int CellsWidth;
        public readonly int CellsHeight;

        private readonly float _timeToSpawnSeconds;
        private readonly float _sqrCollisionDistance;

        private readonly int _cellsCount;
        public int CellsCount => _cellsCount;

        private readonly int[] _cellPositionsX, _cellPositionsY;
        private readonly float[] _localPositionsX, _localPositionsY;
        private readonly float[] _velocitiesX, _velocitiesY;
        private readonly float[] _timesToSpawn;
        private readonly float[] _deltaTimes;
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
            _deltaTimes = new float[_cellsCount];
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

        private void AddRandomAsteroid(int id)
        {
            // Assert.IsNotNull(OnGetRandomPositionAndVelocity, "OnGetRandomPositionAndVelocity is null.");

            (Vector2 position, Vector2 velocity) = OnGetRandomPositionAndVelocity();

            AddAsteroid(position, velocity, id);
        }

        public void AddAsteroid(Vector2 position, Vector2 velocity, int id)
        {
            var cellPosition = new Vector2Int((int)position.x, (int)position.y);
            var localPosition = position - cellPosition;

            Assert.IsTrue(localPosition.x is >= 0 and < 1f);
            Assert.IsTrue(localPosition.y is >= 0 and < 1f);

            CellModel cellModel = GetCell(cellPosition);
            cellModel.AddAsteroid(id);

            _cellPositionsX[id] = cellPosition.x.Wrap(CellsWidth);
            _cellPositionsY[id] = cellPosition.y.Wrap(CellsHeight);
            _localPositionsX[id] = localPosition.x;
            _localPositionsY[id] = localPosition.y;
            _velocitiesX[id] = velocity.x;
            _velocitiesY[id] = velocity.y;
            _deltaTimes[id] = 0f;
            _isSpawned[id] = true;
        }

        public int[] GetAsteroidIdsInCell(Vector2Int cellPosition)
        {
            return GetAsteroidIdsInCell(cellPosition.x, cellPosition.y);
        }

        public int[] GetAsteroidIdsInCell(int x, int y)
        {
            return GetCell(x, y).Asteroids;
        }

        private CellModel GetCell(Vector2Int cellPosition)
        {
            return GetCell(cellPosition.x, cellPosition.y);
        }

        private CellModel GetCell(int x, int y)
        {
            return _cells[x.Wrap(CellsWidth), y.Wrap(CellsHeight)];
        }

        public void UpdateDeltaTime(float deltaTime)
        {
            for (var id = 0; id < _cellsCount; id++)
            {
                _deltaTimes[id] += deltaTime;
            }
        }

        public void UpdateViewport(int xStart, int yStart, int xEnd, int yEnd)
        {
            for (int x = xStart; x < xEnd; x++)
            {
                for (int y = yStart; y < yEnd; y++)
                {
                    int[] asteroidIds = GetAsteroidIdsInCell(x, y);

                    foreach (int id in asteroidIds)
                    {
                        if (id == -1)
                        {
                            break;
                        }

                        float deltaTime = _deltaTimes[id];

                        if (_timesToSpawn[id] > 0f)
                        {
                            _timesToSpawn[id] -= deltaTime;

                            _deltaTimes[id] = 0f;

                            continue;
                        }

                        if (!_isSpawned[id])
                        {
                            AddRandomAsteroid(id);
                        }

                        _deltaTimes[id] = 0f;

                        UpdatePositions(id, deltaTime);

                        CheckCollisions(id);
                    }
                }
            }
        }

        public void UpdatePart(int from, int to, int xStart, int yStart, int xEnd, int yEnd)
        {
            for (int id = from; id < to; id++)
            {
                int cellX = _cellPositionsX[id];
                int cellY = _cellPositionsY[id];

                if (cellX >= xStart && cellX < xEnd && cellY >= yStart && cellY < yEnd)
                {
                    continue;
                }

                float deltaTime = _deltaTimes[id];

                if (_timesToSpawn[id] > 0f)
                {
                    _timesToSpawn[id] -= deltaTime;

                    _deltaTimes[id] = 0f;

                    continue;
                }

                if (!_isSpawned[id])
                {
                    AddRandomAsteroid(id);
                }

                _deltaTimes[id] = 0f;

                UpdatePositions(id, deltaTime);

                CheckCollisions(id);
            }
        }

        private void UpdatePositions(int id, float deltaTime)
        {
            int cellX = _cellPositionsX[id];
            int cellY = _cellPositionsY[id];
            (int oldX, int oldY) = (cellX, cellY);

            float localPositionX = _localPositionsX[id];
            float localPositionY = _localPositionsY[id];

            localPositionX += _velocitiesX[id] * deltaTime;
            localPositionY += _velocitiesY[id] * deltaTime;

            if (localPositionX < 0)
            {
                localPositionX += 1f;
                cellX = (cellX - 1).Wrap(CellsWidth);
            }
            else if (localPositionX >= 1f)
            {
                localPositionX -= 1f;
                cellX = (cellX + 1).Wrap(CellsWidth);
            }

            if (localPositionY < 0)
            {
                localPositionY += 1f;
                cellY = (cellY - 1).Wrap(CellsHeight);
            }
            else if (localPositionY >= 1f)
            {
                localPositionY -= 1f;
                cellY = (cellY + 1).Wrap(CellsHeight);
            }

            _localPositionsX[id] = localPositionX;
            _localPositionsY[id] = localPositionY;

            if (oldX != cellX || oldY != cellY)
            {
                _cells[oldX, oldY].RemoveAsteroid(id);
                _cells[cellX, cellY].AddAsteroid(id);

                _cellPositionsX[id] = cellX;
                _cellPositionsY[id] = cellY;
            }
        }

        private void CheckCollisions(int id)
        {
            int cellX = _cellPositionsX[id];
            int cellY = _cellPositionsY[id];

            CellModel cellModel = _cells[cellX, cellY];

            foreach (int asteroidId in cellModel.Asteroids)
            {
                if (asteroidId == -1)
                {
                    break;
                }

                if (asteroidId == id || _timesToSpawn[asteroidId] > 0f)
                {
                    continue;
                }

                float sqrMagnitude = SqrMagnitude(
                    _localPositionsX[id], _localPositionsY[id],
                    _localPositionsX[asteroidId], _localPositionsY[asteroidId]);

                if (sqrMagnitude <= _sqrCollisionDistance)
                {
                    _isSpawned[asteroidId] = false;
                    _timesToSpawn[asteroidId] = _timeToSpawnSeconds;

                    cellModel.RemoveAsteroid(asteroidId);

                    _isSpawned[id] = false;
                    _timesToSpawn[id] = _timeToSpawnSeconds;

                    cellModel.RemoveAsteroid(id);

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

        public Vector2 GetAsteroidLocalPosition(int id)
        {
            return new Vector2(_localPositionsX[id], _localPositionsY[id]);
        }

        public Vector2Int GetAsteroidCellPosition(int id)
        {
            return new Vector2Int(_cellPositionsX[id], _cellPositionsY[id]);
        }

        public void Destroy(int id)
        {
            _isSpawned[id] = false;
            _timesToSpawn[id] = _timeToSpawnSeconds;

            int cellX = _cellPositionsX[id];
            int cellY = _cellPositionsY[id];

            _cells[cellX, cellY].RemoveAsteroid(id);
        }
    }
}
