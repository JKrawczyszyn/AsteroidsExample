using System;
using Model;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Controllers
{
    public class AsteroidsController
    {
        public event Action Destroyed;

        private readonly AsteroidsModel _model;

        private readonly int _parts;

        private readonly int[] _partsFrom, _partsTo;

        private int _partCounter;
        public int DestroyedCount { get; private set; }

        private readonly RandomProvider _randomProvider;

        public AsteroidsController(AsteroidsModel model, int parts, int seed, RandomProvider randomProvider)
        {
            _model = model;
            _parts = parts;
            _randomProvider = randomProvider;

            (_partsFrom, _partsTo) = new CellPartitionCalculator().Get(model.CellsCount, parts);

            DestroyedCount = 0;

            Random.InitState(seed);

            AddAsteroidInCenterOfEachCell();
        }

        private void AddAsteroidInCenterOfEachCell()
        {
            var id = 0;

            for (var x = 0; x < _model.CellsWidth; x++)
            {
                for (var y = 0; y < _model.CellsHeight; y++)
                {
                    var position = new Vector2(x + 0.5f, y + 0.5f);
                    var velocity = _randomProvider.GetRandomVelocity();

                    Create(position, velocity, id);

                    id++;
                }
            }
        }

        private void Create(Vector2 position, Vector2 velocity, int id)
        {
            _model.AddAsteroid(position, velocity, id);
        }

        public void UpdateViewport(int xStart, int yStart, int xEnd, int yEnd)
        {
            _randomProvider.UpdateViewport(xStart, yStart, xEnd, yEnd);
        }

        public void Update(float deltaTime)
        {
            _model.UpdateDeltaTime(deltaTime);

            // Update of cells is done in parts.
            _model.UpdatePart(_partsFrom[_partCounter], _partsTo[_partCounter], _randomProvider.GetRandomPosition,
                _randomProvider.GetRandomVelocity);

            _partCounter++;
            _partCounter %= _parts;
        }

        public int[] GetAsteroidIdsInCell(Vector2Int cellPosition)
        {
            return _model.GetAsteroidIdsInCell(cellPosition);
        }

        public Vector2 GetAsteroidLocalPosition(int id)
        {
            return _model.GetAsteroidLocalPosition(id);
        }

        public Vector2Int GetAsteroidCellPosition(int id)
        {
            return _model.GetAsteroidCellPosition(id);
        }

        public float GetDeltaTime(int id)
        {
            return _model.GetDeltaTime(id);
        }

        public Vector2 GetAsteroidVelocity(int id)
        {
            return _model.GetAsteroidVelocity(id);
        }

        public void Destroy(int id)
        {
            _model.Destroy(id);

            DestroyedCount++;

            Destroyed?.Invoke();
        }
    }
}
