using System;

public class POIMap
{
	public class Cell
	{
		public int x;

		public int z;

		public bool occupied;

		public Point GetCenter()
		{
			return new Point(x * cellSize + cellSize / 2, z * cellSize + cellSize / 2).Clamp();
		}
	}

	public static int cellSize;

	public static int mapSize;

	public Cell[,] cells;

	public int length => mapSize / cellSize + 1;

	public void Init(int _mapSize, int _cellSize)
	{
		cellSize = _cellSize;
		mapSize = _mapSize;
		cells = new Cell[length, length];
		Reset();
	}

	public void Reset()
	{
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length; j++)
			{
				cells[i, j] = new Cell
				{
					x = i,
					z = j
				};
			}
		}
	}

	public Point GetCenterOfEmptyCell(int tries = 100)
	{
		new Point();
		for (int i = 0; i < tries; i++)
		{
			int num = EClass.rnd(length - 2) + 1;
			int num2 = EClass.rnd(length - 2) + 1;
			if (!cells[num, num2].occupied)
			{
				return cells[num, num2].GetCenter();
			}
		}
		return Point.Invalid;
	}

	public Cell GetCenterCell(int radius = 1)
	{
		return cells[length / 2 - radius + EClass.rnd(radius * 2), length / 2 - radius + EClass.rnd(radius * 2)];
	}

	public Cell GetEmptyCell()
	{
		new Point();
		for (int i = 0; i < 100; i++)
		{
			int num = EClass.rnd(length - 2) + 1;
			int num2 = EClass.rnd(length - 2) + 1;
			if (cells[num, num2] == null)
			{
				return cells[num, num2];
			}
		}
		return null;
	}

	public void ForeachCenterOfEmptyCell(Action<Point> action)
	{
		for (int i = 1; i < length - 2; i++)
		{
			for (int j = 1; j < length - 2; j++)
			{
				if (!cells[i, j].occupied)
				{
					action(cells[i, j].GetCenter());
				}
			}
		}
	}

	public void OccyupyPOI(Point p, int radius = 0)
	{
		OccyupyPOI(p.x, p.z, radius);
	}

	public void OccyupyPOI(int _x, int _z, int radius)
	{
		int num = _x / cellSize;
		int num2 = _z / cellSize;
		cells[num, num2].occupied = true;
		if (radius <= 0)
		{
			return;
		}
		for (int i = num - radius; i < num + radius + 1; i++)
		{
			if (i < 0 || i >= length)
			{
				continue;
			}
			for (int j = num2 - radius; j < num2 + radius + 1; j++)
			{
				if (j >= 0 && j < length)
				{
					cells[i, j].occupied = true;
				}
			}
		}
	}
}
