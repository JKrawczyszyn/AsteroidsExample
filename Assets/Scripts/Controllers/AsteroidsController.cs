using System;
using Model;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Controllers
{
    public class AsteroidsController : IDisposable
    {
        private readonly AsteroidsModel _model;

        private readonly int _parts;

        private readonly int[] _partsFrom, _partsTo;

        private int _partCounter;

        private int _xStart;
        private int _yStart;
        private int _xEnd;
        private int _yEnd;

        public int CellsWidth => _model.CellsWidth;
        public int CellsHeight => _model.CellsHeight;

        public AsteroidsController(AsteroidsModel model, int parts, int seed)
        {
            _model = model;

            _parts = parts;

            (_partsFrom, _partsTo) = ComputeParts(model.CellsCount, parts);

            _model.OnGetRandomPositionAndVelocity += GetRandomPositionAndVelocity;

            Random.InitState(seed);

            AddAsteroidInCenterOfEachCell();
        }

        private (int[] _partsFrom, int[] _partsTo) ComputeParts(int cellsCount, int parts)
        {
            var partsFrom = new int[parts];
            var partsTo = new int[parts];

            var partSize = cellsCount / parts;

            for (var i = 0; i < parts; i++)
            {
                partsFrom[i] = i * partSize;
                partsTo[i] = (i + 1) * partSize;
            }

            partsTo[parts - 1] = cellsCount;

            return (partsFrom, partsTo);
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
            var id = 0;

            for (var x = 0; x < _model.CellsWidth; x++)
            {
                for (var y = 0; y < _model.CellsHeight; y++)
                {
                    var position = new Vector2(x + 0.5f, y + 0.5f);
                    var velocity = GetRandomVelocity();

                    Create(position, velocity, id);

                    id++;
                }
            }
        }

        private Vector2 GetRandomPosition()
        {
            Vector2 position;

            do
            {
                position = new Vector2(Random.Range(0, _model.CellsWidth), Random.Range(0, _model.CellsHeight));
            } while (position.x.Between(_xStart, _xEnd - 1) && position.y.Between(_yStart, _yEnd - 1));

            return position;
        }

        private Vector2 GetRandomVelocity()
        {
            return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * Random.Range(0.1f, 0.2f);
        }

        private void Create(Vector2 position, Vector2 velocity, int id)
        {
            _model.AddAsteroid(position, velocity, id);
        }

        public void Update(float deltaTime, int xStart, int yStart, int xEnd, int yEnd)
        {
            _xStart = xStart;
            _xEnd = xEnd;
            _yStart = yStart;
            _yEnd = yEnd;

            _model.UpdateDeltaTime(deltaTime);
            _model.UpdateViewport(xStart, yStart, xEnd, yEnd);
            _model.UpdatePart(_partsFrom[_partCounter], _partsTo[_partCounter], xStart, yStart, xEnd, yEnd);

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

        public void Destroy(int id)
        {
            _model.Destroy(id);
        }
    }
}
