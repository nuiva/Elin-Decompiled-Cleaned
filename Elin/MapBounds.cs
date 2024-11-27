using System;
using Newtonsoft.Json;
using UnityEngine;

public class MapBounds : EClass
{
	public int CenterX
	{
		get
		{
			return (this.x + this.maxX) / 2;
		}
	}

	public int CenterZ
	{
		get
		{
			return (this.z + this.maxZ) / 2;
		}
	}

	public int Width
	{
		get
		{
			return this.maxX - this.x + 1;
		}
	}

	public int Height
	{
		get
		{
			return this.maxZ - this.z + 1;
		}
	}

	public void SetBounds(int _x, int _z, int _maxX, int _maxZ)
	{
		this.x = _x;
		this.z = _z;
		this.maxX = _maxX;
		this.maxZ = _maxZ;
	}

	public bool Contains(int dx, int dz)
	{
		return dx >= this.x && dz >= this.z && dx <= this.maxX && dz <= this.maxZ;
	}

	public bool Contains(Point p)
	{
		return this.Contains(p.x, p.z);
	}

	public Point GetCenterPos()
	{
		return new Point(this.CenterX, this.CenterZ);
	}

	public Point GetRandomTopPos()
	{
		return new Point(this.x + EClass.rnd(this.maxX - this.x), this.maxZ);
	}

	public Point GetRandomRightPos()
	{
		return new Point(this.maxX, this.z + EClass.rnd(this.maxZ - this.z));
	}

	public Point GetRandomBottomPos()
	{
		return new Point(this.x + EClass.rnd(this.maxX - this.x), this.z);
	}

	public Point GetRandomLeftPos()
	{
		return new Point(this.x, this.z + EClass.rnd(this.maxZ - this.z));
	}

	public Point GetTopPos(float rate = -1f)
	{
		return this.GetSpawnPos(this.x, this.maxZ, this.maxX, this.maxZ);
	}

	public Point GetRightPos(float rate = -1f)
	{
		return this.GetSpawnPos(this.maxX, this.z, this.maxX, this.maxZ);
	}

	public Point GetBottomPos(float rate = -1f)
	{
		return this.GetSpawnPos(this.x, this.z, this.maxX, this.z);
	}

	public Point GetLeftPos(float rate = -1f)
	{
		return this.GetSpawnPos(this.x, this.z, this.x, this.maxZ);
	}

	public Point GetRandomPoint()
	{
		return new Point(this.x + EClass.rnd(this.Width), this.z + EClass.rnd(this.Height));
	}

	public Point GetSpawnPos(int x, int z, int maxX, int maxZ)
	{
		Point point = new Point();
		for (int i = z; i < maxZ + 1; i++)
		{
			for (int j = x; j < maxX + 1; j++)
			{
				point.Set(j, i);
				foreach (Thing thing in point.Things)
				{
					if (thing.trait is TraitRoadSign)
					{
						if (thing.dir == 0)
						{
							point.z--;
						}
						else if (thing.dir == 1)
						{
							point.x++;
						}
						else if (thing.dir == 2)
						{
							point.z++;
						}
						else
						{
							point.x--;
						}
						return point;
					}
				}
			}
		}
		return new Point(x + (maxX - x) / 2, z + (maxZ - z) / 2);
	}

	public bool CanExpand(int a)
	{
		return this.x - a >= 1 || this.z - a >= 1 || this.maxX + a < EClass._map.Size - 1 || this.maxZ + a < EClass._map.Size - 1;
	}

	public void Expand(int a)
	{
		this.x -= a;
		this.z -= a;
		this.maxX += a;
		this.maxZ += a;
		if (this.x < 1)
		{
			this.x = 1;
		}
		if (this.z < 1)
		{
			this.z = 1;
		}
		if (this.maxX >= EClass._map.Size - 1)
		{
			this.maxX = EClass._map.Size - 2;
		}
		if (this.maxZ >= EClass._map.Size - 1)
		{
			this.maxZ = EClass._map.Size - 2;
		}
	}

	public Point GetSurface(int x, int z, bool walkable = true)
	{
		Point point = new Point(x, z);
		point.Clamp(false);
		if (!walkable || !point.cell.blocked)
		{
			return point;
		}
		return Point.Invalid;
	}

	public Point GetRandomSurface(int x, int z, int radius, bool walkable = true, bool allowWater = false)
	{
		for (int i = 0; i < radius * radius * 2; i++)
		{
			Point surface = this.GetSurface(x + EClass.rnd(radius) - EClass.rnd(radius), z + EClass.rnd(radius) - EClass.rnd(radius), walkable);
			if (surface.IsValid && (allowWater || !surface.IsWater))
			{
				return surface;
			}
		}
		return EClass._map.GetCenterPos();
	}

	public Point GetRandomSurface(bool centered = false, bool walkable = true, bool allowWater = false)
	{
		for (int i = 0; i < 10000; i++)
		{
			Point surface = this.GetSurface(centered ? (this.CenterX + EClass.rnd(this.Width / 4) - EClass.rnd(this.Width / 4)) : (this.x + EClass.rnd(this.Width)), centered ? (this.CenterZ + EClass.rnd(this.Height / 4) - EClass.rnd(this.Height / 4)) : (this.z + EClass.rnd(this.Height)), walkable);
			if (surface.IsValid && (allowWater || !surface.IsWater))
			{
				return surface;
			}
		}
		return this.GetSurface(this.CenterX, this.CenterZ, false);
	}

	public Point GetRandomSpawnPos()
	{
		for (int i = 0; i < 10000; i++)
		{
			Point randomPoint = EClass._map.GetRandomPoint();
			if (!randomPoint.cell.blocked && randomPoint.cell.room == null && randomPoint.cell.light <= 0 && randomPoint.IsValid)
			{
				return randomPoint;
			}
		}
		return this.GetCenterPos().GetNearestPoint(false, true, true, false);
	}

	public Point GetRandomEdge(int r = 3)
	{
		for (int i = 0; i < 10000; i++)
		{
			int num;
			int num2;
			if (EClass.rnd(2) == 0)
			{
				num = ((EClass.rnd(2) == 0) ? (this.x + EClass.rnd(r)) : (this.maxX - EClass.rnd(r)));
				num2 = this.z + EClass.rnd(this.Height);
			}
			else
			{
				num2 = ((EClass.rnd(2) == 0) ? (this.z + EClass.rnd(r)) : (this.maxZ - EClass.rnd(r)));
				num = this.x + EClass.rnd(this.Width);
			}
			Point surface = this.GetSurface(num, num2, false);
			if (surface.IsValid)
			{
				return surface;
			}
		}
		return this.GetSurface(this.Size / 2, this.Size / 2, false);
	}

	public Point GetRandomSpace(int width, int height, int tries = 100)
	{
		Point point = new Point();
		Point point2 = new Point();
		for (int i = 0; i < tries; i++)
		{
			bool flag = true;
			point2.Set(this.x + EClass.rnd(this.maxX - this.x), this.z + EClass.rnd(this.maxZ - this.z));
			for (int j = 0; j < height; j++)
			{
				for (int k = 0; k < width; k++)
				{
					point.Set(point2.x + k, point2.z + j);
					if (point.x > this.maxX || point.z > this.maxZ || point.IsBlocked || point.cell.HasZoneStairs(true))
					{
						flag = false;
						break;
					}
				}
				if (!flag)
				{
					break;
				}
			}
			if (flag)
			{
				return point2;
			}
		}
		Debug.Log("valid space not found:" + width.ToString() + "/" + height.ToString());
		return null;
	}

	public void ForeachCell(Action<Cell> action)
	{
		Cell[,] cells = EClass._map.cells;
		for (int i = this.x; i <= this.maxX; i++)
		{
			for (int j = this.z; j <= this.maxZ; j++)
			{
				action(cells[i, j]);
			}
		}
	}

	public void ForeachPoint(Action<Point> action)
	{
		Point point = new Point();
		for (int i = this.x; i <= this.maxX; i++)
		{
			for (int j = this.z; j <= this.maxZ; j++)
			{
				action(point.Set(i, j));
			}
		}
	}

	public void ForeachXYZ(Action<int, int> action)
	{
		for (int i = this.x; i <= this.maxX; i++)
		{
			for (int j = this.z; j <= this.maxZ; j++)
			{
				action(i, j);
			}
		}
	}

	[JsonProperty]
	public int x;

	[JsonProperty]
	public int z;

	[JsonProperty]
	public int maxX;

	[JsonProperty]
	public int maxZ;

	[JsonProperty]
	public int Size;
}
