using System;
using System.Collections.Generic;
using UnityEngine;

public class Fov : EClass
{
	public struct LevelPoint
	{
		public int X;

		public int Y;
	}

	public static float nonGradientMod;

	public static List<Cell> newPoints = new List<Cell>();

	public static Map map;

	public static LevelPoint origin = default(LevelPoint);

	public static int range;

	public static float _power;

	public byte r;

	public byte g;

	public byte b;

	public bool isPC;

	public bool limitGradient;

	public Dictionary<int, byte> lastPoints = new Dictionary<int, byte>();

	private static int x;

	private static int y;

	public void SetVisible(int x, int y)
	{
		Cell cell = map.cells[x, y];
		if (cell.light > 100 && !isPC)
		{
			return;
		}
		int key = x + y * map.Size;
		if (isPC && !cell.outOfBounds)
		{
			cell.pcSync = true;
		}
		if (_power < 0.1f)
		{
			lastPoints[key] = 0;
			return;
		}
		float num = GetVisDistance(x, y, origin.X, origin.Y);
		float power = _power;
		if (limitGradient)
		{
			power *= nonGradientMod;
			if (num >= (float)range)
			{
				power *= EClass.scene.profile.global.edgeLight;
			}
		}
		else
		{
			if (num <= 1f)
			{
				num = 1f;
			}
			power *= (float)(range + 1) - num;
		}
		if (power < 1f)
		{
			power = 1f;
		}
		else if (power > 20f)
		{
			power = 20f;
		}
		if (cell.outOfBounds)
		{
			if (isPC && (power > 0f || GetVisDistance(origin.X, origin.Y, x, y) < 4f))
			{
				cell.isSeen = true;
			}
			power = 0f;
		}
		lastPoints[key] = (byte)power;
	}

	public void ClearVisible()
	{
		foreach (KeyValuePair<int, byte> lastPoint in lastPoints)
		{
			Cell cell = map.GetCell(lastPoint.Key);
			byte value = lastPoint.Value;
			cell.light -= value;
			cell.lightR -= (ushort)(value * r / 2);
			cell.lightG -= (ushort)(value * g / 2);
			cell.lightB -= (ushort)(value * b / 2);
			if (isPC)
			{
				cell.pcSync = false;
			}
		}
		lastPoints.Clear();
	}

	public List<Point> ListPoints()
	{
		List<Point> list = new List<Point>();
		foreach (KeyValuePair<int, byte> lastPoint in lastPoints)
		{
			list.Add(new Point(lastPoint.Key));
		}
		return list;
	}

	private float GetVisDistance(int x1, int y1, int x2, int y2)
	{
		return Mathf.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
	}

	public static float DistanceFloat(int x1, int y1, int x2, int y2)
	{
		return Mathf.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
	}

	public static int Distance(int x1, int y1, int x2, int y2)
	{
		return (int)Mathf.Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
	}

	public void Perform(int _x, int _z, int _range, float power = 1f)
	{
		range = _range;
		ClearVisible();
		if (range == 0)
		{
			return;
		}
		_power = power;
		origin.X = _x;
		origin.Y = _z;
		SetVisible(origin.X, origin.Y);
		int num = origin.X - range;
		if (num < 0)
		{
			num = 0;
		}
		int num2 = origin.Y - range;
		if (num2 < 0)
		{
			num2 = 0;
		}
		int num3 = origin.X + range + 1;
		if (num3 >= EClass._map.Size)
		{
			num3 = EClass._map.Size;
		}
		int num4 = origin.Y + range + 1;
		if (num4 >= EClass._map.Size)
		{
			num4 = EClass._map.Size;
		}
		for (int i = num; i < num3; i++)
		{
			TraceLine(i, num2);
			TraceLine(i, num4 - 1);
		}
		for (int j = num2 + 1; j < num4 - 1; j++)
		{
			TraceLine(num, j);
			TraceLine(num3 - 1, j);
		}
		if (range >= 6)
		{
			int max = EClass._map.Size - 1;
			int x = (origin.X + 4).ClampMax(max);
			int y = (origin.Y + 1).ClampMax(max);
			int x2 = (origin.X - 4).ClampMin(0);
			int y2 = (origin.Y - 1).ClampMin(0);
			int x3 = (origin.X + 1).ClampMax(max);
			int y3 = (origin.Y + 4).ClampMax(max);
			int x4 = (origin.X - 1).ClampMin(0);
			int y4 = (origin.Y - 4).ClampMin(0);
			TraceLine(x, y);
			TraceLine(x, y2);
			TraceLine(x2, y);
			TraceLine(x2, y2);
			TraceLine(x3, y3);
			TraceLine(x3, y4);
			TraceLine(x4, y3);
			TraceLine(x4, y4);
		}
		foreach (KeyValuePair<int, byte> lastPoint in lastPoints)
		{
			Cell cell = map.GetCell(lastPoint.Key);
			byte value = lastPoint.Value;
			cell.light += value;
			cell.lightR += (ushort)(value * r / 2);
			cell.lightG += (ushort)(value * g / 2);
			cell.lightB += (ushort)(value * b / 2);
			if (isPC && (value > 0 || GetVisDistance(origin.X, origin.Y, cell.x, cell.z) < 2f) && !cell.isSeen && (!cell.HasWall || EClass._zone.UseFog || cell.room == EClass.pc.Cell.room || cell.hasDoor))
			{
				EClass._map.SetSeen(cell.x, cell.z);
				newPoints.Add(cell);
			}
		}
		if (isPC && newPoints.Count > 0)
		{
			WidgetMinimap.UpdateMap(newPoints);
		}
	}

	private void TraceLine(int x2, int y2)
	{
		int value = x2 - origin.X;
		int value2 = y2 - origin.Y;
		int num = Math.Abs(value);
		int num2 = Math.Abs(value2);
		int num3 = Math.Sign(value);
		int num4 = Math.Sign(value2) << 16;
		int num5 = (origin.Y << 16) + origin.X;
		if (num < num2)
		{
			int num6 = num;
			num = num2;
			num2 = num6;
			int num7 = num3;
			num3 = num4;
			num4 = num7;
		}
		int num8 = num2 * 2;
		int num9 = -num;
		int num10 = num * 2;
		while (--num >= 0)
		{
			num5 += num3;
			num9 += num8;
			if (num9 > 0)
			{
				num9 -= num10;
				num5 += num4;
			}
			x = num5 & 0xFFFF;
			y = num5 >> 16;
			int num11 = (int)GetVisDistance(origin.X, origin.Y, x, y);
			if (range < 0 || num11 <= range)
			{
				SetVisible(x, y);
				if (map.cells[x, y].blockSight)
				{
					break;
				}
				continue;
			}
			break;
		}
	}
}
