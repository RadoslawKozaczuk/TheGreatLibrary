using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using static System.Console;

namespace DesignPatterns.Structural
{
	/*
		Motivation:
		Electrical devices have different power (interface) requirements.
		We cannot modify our gadgets to support every possible interface.
		Thus, we use a special device (an adapter) to give us the interface we require from the interface we have.

		Definition:
		A construct which adapts an existing interface X to conform to the required interface Y.
	*/
	class Adapter
	{
		// adapter is all about converting the interface you are given to the interface you want
		class Point
		{
			public readonly int X;
			public readonly int Y;

			public Point(int x, int y)
			{
				X = x;
				Y = y;
			}

			public override string ToString() => $"{nameof(X)}: {X}, {nameof(Y)}: {Y}";
		}

		// we have a line composed out of two Points
		class Line
		{
			public readonly Point Start;
			public readonly Point End;

			public Line(Point start, Point end)
			{
				Start = start;
				End = end;
			}
		}

		// graphic object made out of collection of lines
		class VectorObject : Collection<Line>
		{

		}

		class VectorRectangle : VectorObject
		{
			public VectorRectangle(int x, int y, int width, int height)
			{
				Add(new Line(new Point(x, y), new Point(x + width, y)));
				Add(new Line(new Point(x + width, y), new Point(x + width, y + height)));
				Add(new Line(new Point(x, y), new Point(x, y + height)));
				Add(new Line(new Point(x, y + height), new Point(x + width, y + height)));
			}
		}

		class LineToPointAdapter : Collection<Point>
		{
			static int _count;

			public LineToPointAdapter(Line line)
			{
				Write((_count > 0 ? Environment.NewLine : "")
					+ $"{++_count}: Generating points for line [{line.Start.X},{line.Start.Y}]-[{line.End.X},{line.End.Y}] (no caching) => ");

				int left = Math.Min(line.Start.X, line.End.X);
				int right = Math.Max(line.Start.X, line.End.X);
				int top = Math.Min(line.Start.Y, line.End.Y);
				int bottom = Math.Max(line.Start.Y, line.End.Y);
				int dx = right - left;
				int dy = line.End.Y - line.Start.Y;

				if (dx == 0)
				{
					for (int y = top; y <= bottom; ++y)
						Add(new Point(left, y));
				}
				else if (dy == 0)
				{
					for (int x = left; x <= right; ++x)
						Add(new Point(x, top));
				}
			}
		}

		static readonly List<VectorObject> _vectorObjects = new List<VectorObject>
		{
			new VectorRectangle(1, 1, 10, 10),
			new VectorRectangle(3, 3, 6, 6)
		};

		// the interface we have
		static void DrawPoint(Point p)
		{
			Write(".");
		}

		public static void Demo()
		{
			foreach (var vo in _vectorObjects)
				foreach (var line in vo)
					foreach (var point in new LineToPointAdapter(line))
						DrawPoint(point);
		}
	}
}
