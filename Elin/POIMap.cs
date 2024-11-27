using System;

public class POIMap
{
	public int length
	{
		get
		{
			return POIMap.mapSize / POIMap.cellSize + 1;
		}
	}

	public void Init(int _mapSize, int _cellSize)
	{
		POIMap.cellSize = _cellSize;
		POIMap.mapSize = _mapSize;
		this.cells = new POIMap.Cell[this.length, this.length];
		this.Reset();
	}

	public void Reset()
	{
		for (int i = 0; i < this.length; i++)
		{
			for (int j = 0; j < this.length; j++)
			{
				this.cells[i, j] = new POIMap.Cell
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
			int num = EClass.rnd(this.length - 2) + 1;
			int num2 = EClass.rnd(this.length - 2) + 1;
			if (!this.cells[num, num2].occupied)
			{
				return this.cells[num, num2].GetCenter();
			}
		}
		return Point.Invalid;
	}

	public POIMap.Cell GetCenterCell(int radius = 1)
	{
		return this.cells[this.length / 2 - radius + EClass.rnd(radius * 2), this.length / 2 - radius + EClass.rnd(radius * 2)];
	}

	public POIMap.Cell GetEmptyCell()
	{
		new Point();
		for (int i = 0; i < 100; i++)
		{
			int num = EClass.rnd(this.length - 2) + 1;
			int num2 = EClass.rnd(this.length - 2) + 1;
			if (this.cells[num, num2] == null)
			{
				return this.cells[num, num2];
			}
		}
		return null;
	}

	public void ForeachCenterOfEmptyCell(Action<Point> action)
	{
		for (int i = 1; i < this.length - 2; i++)
		{
			for (int j = 1; j < this.length - 2; j++)
			{
				if (!this.cells[i, j].occupied)
				{
					action(this.cells[i, j].GetCenter());
				}
			}
		}
	}

	public void OccyupyPOI(Point p, int radius = 0)
	{
		this.OccyupyPOI(p.x, p.z, radius);
	}

	public void OccyupyPOI(int _x, int _z, int radius)
	{
		int num = _x / POIMap.cellSize;
		int num2 = _z / POIMap.cellSize;
		this.cells[num, num2].occupied = true;
		if (radius > 0)
		{
			for (int i = num - radius; i < num + radius + 1; i++)
			{
				if (i >= 0 && i < this.length)
				{
					for (int j = num2 - radius; j < num2 + radius + 1; j++)
					{
						if (j >= 0 && j < this.length)
						{
							this.cells[i, j].occupied = true;
						}
					}
				}
			}
		}
	}

	public static int cellSize;

	public static int mapSize;

	public POIMap.Cell[,] cells;

	public class Cell
	{
		public Point GetCenter()
		{
			return new Point(this.x * POIMap.cellSize + POIMap.cellSize / 2, this.z * POIMap.cellSize + POIMap.cellSize / 2).Clamp(false);
		}

		public int x;

		public int z;

		public bool occupied;
	}
}
