using UnityEngine;

namespace Controllers
{
    public class RandomProvider
    {
        private readonly int _cellsWidth;
        private readonly int _cellsHeight;

        private int _xStart;
        private int _yStart;
        private int _xEnd;
        private int _yEnd;

        public RandomProvider(int cellsWidth, int cellsHeight)
        {
            _cellsWidth = cellsWidth;
            _cellsHeight = cellsHeight;
        }

        public void UpdateViewport(int xStart, int yStart, int xEnd, int yEnd)
        {
            _xStart = xStart;
            _yStart = yStart;
            _xEnd = xEnd;
            _yEnd = yEnd;
        }

        public Vector2 GetRandomPosition()
        {
            Vector2 position;

            do
            {
                position = new Vector2(Random.Range(0, _cellsWidth), Random.Range(0, _cellsHeight));
            } while (position.x.Between(_xStart, _xEnd - 1) && position.y.Between(_yStart, _yEnd - 1));

            return position;
        }

        public Vector2 GetRandomVelocity()
        {
            return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * Random.Range(0.1f, 0.2f);
        }
    }
}
