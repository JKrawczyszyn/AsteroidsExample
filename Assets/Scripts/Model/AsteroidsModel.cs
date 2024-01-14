using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Model
{
    public class AsteroidsModel
    {
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

        public AsteroidsModel(Config config)
        {
            int width = config.GridSize.x;
            int height = config.GridSize.y;
            float timeToSpawnSeconds = config.TimeToSpawnSeconds;
            float collisionDistance = config.CollisionDistance;

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

        private void AddRandomAsteroid(int id, Func<Vector2> getRandomPosition, Func<Vector2> getRandomVelocity)
        {
            // Assert.IsNotNull(OnGetRandomPositionAndVelocity, "OnGetRandomPositionAndVelocity is null.");
            Vector2 position = getRandomPosition();
            Vector2 velocity = getRandomVelocity();

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
                _deltaTimes[id] += deltaTime;
        }

        public void UpdatePart(int from, int to, Func<Vector2> getRandomPosition, Func<Vector2> getRandomVelocity)
        {
            for (int id = from; id < to; id++)
                ProcessAsteroid(id, getRandomPosition, getRandomVelocity);
        }

        private void ProcessAsteroid(int id, Func<Vector2> getRandomPosition, Func<Vector2> getRandomVelocity)
        {
            float deltaTime = _deltaTimes[id];

            if (_timesToSpawn[id] > 0f)
            {
                _timesToSpawn[id] -= deltaTime;

                _deltaTimes[id] = 0f;

                return;
            }

            if (!_isSpawned[id])
                AddRandomAsteroid(id, getRandomPosition, getRandomVelocity);

            _deltaTimes[id] = 0f;

            UpdatePositions(id, deltaTime);

            CheckCollisions(id);
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

            var collided = false;

            // Same cell
            CellModel cellModel = _cells[cellX, cellY];

            foreach (int id2 in cellModel.Asteroids)
            {
                if (id == id2)
                    continue;

                if (id2 == -1)
                    break;

                collided = ProcessCollision(id, id2, cellModel, cellModel);
                if (collided)
                    break;
            }

            if (collided)
                return;

            // Neighbour cell +1 Y
            CellModel cellModel2 = _cells[cellX, (cellY + 1).Wrap(CellsHeight)];

            foreach (int id2 in cellModel2.Asteroids)
            {
                if (id2 == -1)
                    break;

                collided = ProcessCollision(id, id2, cellModel, cellModel2);
                if (collided)
                    break;
            }

            if (collided)
                return;

            // Neighbour cell +1 X
            cellModel2 = _cells[(cellX + 1).Wrap(CellsWidth), cellY];

            foreach (int id2 in cellModel2.Asteroids)
            {
                if (id2 == -1)
                    break;

                collided = ProcessCollision(id, id2, cellModel, cellModel2);
                if (collided)
                    break;
            }

            // We could check also diagonal neighbours, but chance of collision is very low.
        }

        private bool ProcessCollision(int id, int id2, CellModel cellModel, CellModel cellModel2)
        {
            if (_timesToSpawn[id2] > 0f)
                return false;

            float sqrMagnitude = SqrMagnitude(
                _cellPositionsX[id] + _localPositionsX[id], _cellPositionsY[id] + _localPositionsY[id],
                _cellPositionsX[id2] + _localPositionsX[id2], _cellPositionsY[id2] + _localPositionsY[id2]
            );

            if (sqrMagnitude > _sqrCollisionDistance)
                return false;

            _isSpawned[id] = false;
            _timesToSpawn[id] = _timeToSpawnSeconds;

            cellModel.RemoveAsteroid(id);

            _isSpawned[id2] = false;
            _timesToSpawn[id2] = _timeToSpawnSeconds;

            cellModel2.RemoveAsteroid(id2);

            return true;
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

        public Vector2 GetAsteroidVelocity(int id)
        {
            return new Vector2(_velocitiesX[id], _velocitiesY[id]);
        }

        public float GetDeltaTime(int id)
        {
            return _deltaTimes[id];
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
