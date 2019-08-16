using System;
using static CSharpSeven.Grid;

namespace CSharpSeven
{
    static class ExtensionMethods
    {
        public static bool Any(this GridCellClass[,] cells, int x, int y, int sizeX, int sizeY, Func<GridCellClass, bool> func)
        {
            for (int i = x; i < x + sizeX; i++)
                for (int j = y; j < y + sizeY; j++)
                    if (func(cells[i, j]))
                        return true;

            return false;
        }

        public static bool Any(this GridCellStruct[,] cells, int x, int y, int sizeX, int sizeY, FunctionRefStruct<GridCellStruct> func)
        {
            for (int i = x; i < x + sizeX; i++)
                for (int j = y; j < y + sizeY; j++)
                    if (func(ref cells[i, j]))
                        return true;

            return false;
        }

        public static void All(this GridCellClass[,] cells, int x, int y, int sizeX, int sizeY, Action<GridCellClass> action)
        {
            for (int i = x; i < x + sizeX; i++)
                for (int j = y; j < y + sizeY; j++)
                    action(cells[i, j]);
        }

        public static void All(this GridCellStruct[,] cells, int x, int y, int sizeX, int sizeY, ActionRefStruct<GridCellStruct> action)
        {
            for (int i = x; i < x + sizeX; i++)
                for (int j = y; j < y + sizeY; j++)
                    action(ref cells[i, j]);
        }
    }

    // data model
    public class GridCellClass
    {
        public bool IsOccupied { get => Building != null; }

        public object Building;
        public int X, Y;
    }

    // data model
    public struct GridCellStruct
    {
        public bool IsOccupied { get => Building != null; }

        public object Building;
        public int X, Y;
    }

    public class Grid
    {
        // to circumnavigate the regular anonymous method declaration limitation
        public delegate void ActionRefStruct<T1>(ref GridCellStruct cell);
        public delegate bool FunctionRefStruct<T1>(ref GridCellStruct cell);

        const int GRID_SIZE_X = 2, GRID_SIZE_Y = 2;

        readonly GridCellClass[,] _cellsC = new GridCellClass[GRID_SIZE_X, GRID_SIZE_Y];
        readonly GridCellStruct[,] _cellsS = new GridCellStruct[GRID_SIZE_X, GRID_SIZE_Y];

        public Grid()
        {
            for (int i = 0; i < GRID_SIZE_X; i++)
                for (int j = 0; j < GRID_SIZE_Y; j++)
                {
                    _cellsC[i, j] = new GridCellClass() { X = i, Y = j };
                    _cellsS[i, j] = new GridCellStruct() { X = i, Y = j };
                }
        }

        // for classes everything work out of the box as they are reference types
        public bool IsAreaFreeClass(int x, int y, int sizeX, int sizeY)
            => !_cellsC.Any(x, y, sizeX, sizeY, (cell) => cell.IsOccupied);

        // for structs we need to create our own delegate type and explicitly determine the type of the anonymous function's parameter
        public bool IsAreaFreeStruct(int x, int y, int sizeX, int sizeY)
            => !_cellsS.Any(x, y, sizeX, sizeY, (ref GridCellStruct cell) => cell.IsOccupied);

        public void MarkAreaAsOccupiedClass(int x, int y, int sizeX, int sizeY, object building)
            => _cellsC.All(x, y, sizeX, sizeY, (cell) => cell.Building = building);

        public void MarkAreaAsFreeClass(int x, int y, int sizeX, int sizeY)
            => _cellsC.All(x, y, sizeX, sizeY, (cell) => cell.Building = null);

        public void MarkAreaAsOccupiedStruct(int x, int y, int sizeX, int sizeY, object building)
            => _cellsS.All(x, y, sizeX, sizeY, (ref GridCellStruct cell) => cell.Building = building);

        public void MarkAreaAsFreeStruct(int x, int y, int sizeX, int sizeY)
            => _cellsS.All(x, y, sizeX, sizeY, (ref GridCellStruct cell) => cell.Building = null);
    }

    public static class RefStructActionAndFunction
    {
        public static void TestMethod()
        {
            Grid g = new Grid();
            object troll = new object();

            g.MarkAreaAsOccupiedClass(0, 0, 1, 1, troll);
            bool test = g.IsAreaFreeClass(0, 0, 1, 1);
            Console.WriteLine("Area correctly occupied (class) - " + !test);
            g.MarkAreaAsFreeClass(0, 0, 1, 1);
            test = g.IsAreaFreeClass(0, 0, 1, 1);
            Console.WriteLine("Area correctly freed (class) - " + test);

            g.MarkAreaAsOccupiedStruct(0, 0, 1, 1, troll);
            test = g.IsAreaFreeStruct(0, 0, 1, 1);
            Console.WriteLine("Area correctly occupied (class) - " + !test);
            g.MarkAreaAsFreeStruct(0, 0, 1, 1);
            test = g.IsAreaFreeStruct(0, 0, 1, 1);
            Console.WriteLine("Area correctly freed (class) - " + test);
        }
    }
}
