using System;

namespace Model
{
    public class CellModel
    {
        public readonly int[] Asteroids = new int[20];

        private int _cursor;

        public CellModel()
        {
            for (var i = 0; i < Asteroids.Length; i++)
            {
                Asteroids[i] = -1;
            }
        }

        public void AddAsteroid(int index)
        {
            // Assert.IsTrue(!Asteroids.Contains(index), "Asteroid already exists in cell.");
            // Assert.IsTrue(_cursor < Asteroids.Length, "Cell is full.");

            Asteroids[_cursor++] = index;
        }

        public void RemoveAsteroid(int index)
        {
            // Assert.IsTrue(Asteroids.Contains(index), "Asteroid doesn't exist in cell.");
            // Assert.IsTrue(_cursor > 0, "Cell is empty.");

            int indexInCell = Array.IndexOf(Asteroids, index);

            _cursor--;

            Asteroids[indexInCell] = Asteroids[_cursor];
            Asteroids[_cursor] = -1;
        }
    }
}
