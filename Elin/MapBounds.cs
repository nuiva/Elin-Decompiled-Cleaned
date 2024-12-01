using System;
using Newtonsoft.Json;
using UnityEngine;

public class MapBounds : EClass
{
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

	public int CenterX => (x + maxX) / 2;

	public int CenterZ => (z + maxZ) / 2;

	public int Width => maxX - x + 1;

	public int Height => maxZ - z + 1;

	public void SetBounds(int _x, int _z, int _maxX, int _maxZ)
	{
		x = _x;
		z = _z;
		maxX = _maxX;
		maxZ = _maxZ;
	}

	public bool Contains(int dx, int dz)
	{
		if (dx >= x && dz >= z && dx <= maxX)
		{
			return dz <= maxZ;
		}
		return false;
	}

	public bool Contains(Point p)
	{
		return Contains(p.x, p.z);
	}

	public Point GetCenterPos()
	{
		return new Point(CenterX, CenterZ);
	}

	public Point GetRandomTopPos()
	{
		return new Point(x + EClass.rnd(maxX - x), maxZ);
	}

	public Point GetRandomRightPos()
	{
		return new Point(maxX, z + EClass.rnd(maxZ - z));
	}

	public Point GetRandomBottomPos()
	{
		return new Point(x + EClass.rnd(maxX - x), z);
	}

	public Point GetRandomLeftPos()
	{
		return new Point(x, z + EClass.rnd(maxZ - z));
	}

	public Point GetTopPos(float rate = -1f)
	{
		return GetSpawnPos(x, maxZ, maxX, maxZ);
	}

	public Point GetRightPos(float rate = -1f)
	{
		return GetSpawnPos(maxX, z, maxX, maxZ);
	}

	public Point GetBottomPos(float rate = -1f)
	{
		return GetSpawnPos(x, z, maxX, z);
	}

	public Point GetLeftPos(float rate = -1f)
	{
		return GetSpawnPos(x, z, x, maxZ);
	}

	public Point GetRandomPoint()
	{
		return new Point(x + EClass.rnd(Width), z + EClass.rnd(Height));
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
		if (x - a < 1 && z - a < 1 && maxX + a >= EClass._map.Size - 1)
		{
			return maxZ + a < EClass._map.Size - 1;
		}
		return true;
	}

	public void Expand(int a)
	{
		x -= a;
		z -= a;
		maxX += a;
		maxZ += a;
		if (x < 1)
		{
			x = 1;
		}
		if (z < 1)
		{
			z = 1;
		}
		if (maxX >= EClass._map.Size - 1)
		{
			maxX = EClass._map.Size - 2;
		}
		if (maxZ >= EClass._map.Size - 1)
		{
			maxZ = EClass._map.Size - 2;
		}
	}

	public Point GetSurface(int x, int z, bool walkable = true)
	{
		Point point = new Point(x, z);
		point.Clamp();
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
			Point surface = GetSurface(x + EClass.rnd(radius) - EClass.rnd(radius), z + EClass.rnd(radius) - EClass.rnd(radius), walkable);
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
			Point surface = GetSurface(centered ? (CenterX + EClass.rnd(Width / 4) - EClass.rnd(Width / 4)) : (x + EClass.rnd(Width)), centered ? (CenterZ + EClass.rnd(Height / 4) - EClass.rnd(Height / 4)) : (z + EClass.rnd(Height)), walkable);
			if (surface.IsValid && (allowWater || !surface.IsWater))
			{
				return surface;
			}
		}
		return GetSurface(CenterX, CenterZ, walkable: false);
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
		return GetCenterPos().GetNearestPoint();
	}

	public Point GetRandomEdge(int r = 3)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < 10000; i++)
		{
			if (EClass.rnd(2) == 0)
			{
				num = ((EClass.rnd(2) == 0) ? (x + EClass.rnd(r)) : (maxX - EClass.rnd(r)));
				num2 = z + EClass.rnd(Height);
			}
			else
			{
				num2 = ((EClass.rnd(2) == 0) ? (z + EClass.rnd(r)) : (maxZ - EClass.rnd(r)));
				num = x + EClass.rnd(Width);
			}
			Point surface = GetSurface(num, num2, walkable: false);
			if (surface.IsValid)
			{
				return surface;
			}
		}
		return GetSurface(Size / 2, Size / 2, walkable: false);
	}

	public Point GetRandomSpace(int width, int height, int tries = 100)
	{
		Point point = new Point();
		Point point2 = new Point();
		for (int i = 0; i < tries; i++)
		{
			bool flag = true;
			point2.Set(x + EClass.rnd(maxX - x), z + EClass.rnd(maxZ - z));
			for (int j = 0; j < height; j++)
			{
				for (int k = 0; k < width; k++)
				{
					point.Set(point2.x + k, point2.z + j);
					if (point.x > maxX || point.z > maxZ || point.IsBlocked || point.cell.HasZoneStairs())
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
		Debug.Log("valid space not found:" + width + "/" + height);
		return null;
	}

	public void ForeachCell(Action<Cell> action)
	{
		Cell[,] cells = EClass._map.cells;
		for (int i = x; i <= maxX; i++)
		{
			for (int j = z; j <= maxZ; j++)
			{
				action(cells[i, j]);
			}
		}
	}

	public void ForeachPoint(Action<Point> action)
	{
		Point point = new Point();
		for (int i = x; i <= maxX; i++)
		{
			for (int j = z; j <= maxZ; j++)
			{
				action(point.Set(i, j));
			}
		}
	}

	public void ForeachXYZ(Action<int, int> action)
	{
		for (int i = x; i <= maxX; i++)
		{
			for (int j = z; j <= maxZ; j++)
			{
				action(i, j);
			}
		}
	}
}
