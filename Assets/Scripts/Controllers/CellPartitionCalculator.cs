namespace Controllers
{
    public class CellPartitionCalculator
    {
        public (int[] _partsFrom, int[] _partsTo) Get(int cellsCount, int parts)
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
    }
}
