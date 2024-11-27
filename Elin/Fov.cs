using System;
using System.Collections.Generic;
using UnityEngine;

public class Fov : EClass
{
	public void SetVisible(int x, int y)
	{
		Cell cell = Fov.map.cells[x, y];
		if (cell.light > 100 && !this.isPC)
		{
			return;
		}
		int key = x + y * Fov.map.Size;
		if (this.isPC && !cell.outOfBounds)
		{
			cell.pcSync = true;
		}
		if (Fov._power < 0.1f)
		{
			this.lastPoints[key] = 0;
			return;
		}
		float num = this.GetVisDistance(x, y, Fov.origin.X, Fov.origin.Y);
		float num2 = Fov._power;
		if (this.limitGradient)
		{
			num2 *= Fov.nonGradientMod;
			if (num >= (float)Fov.range)
			{
				num2 *= EClass.scene.profile.global.edgeLight;
			}
		}
		else
		{
			if (num <= 1f)
			{
				num = 1f;
			}
			num2 *= (float)(Fov.range + 1) - num;
		}
		if (num2 < 1f)
		{
			num2 = 1f;
		}
		else if (num2 > 20f)
		{
			num2 = 20f;
		}
		if (cell.outOfBounds)
		{
			if (this.isPC && (num2 > 0f || this.GetVisDistance(Fov.origin.X, Fov.origin.Y, x, y) < 4f))
			{
				cell.isSeen = true;
			}
			num2 = 0f;
		}
		this.lastPoints[key] = (byte)num2;
	}

	public void ClearVisible()
	{
		foreach (KeyValuePair<int, byte> keyValuePair in this.lastPoints)
		{
			Cell cell = Fov.map.GetCell(keyValuePair.Key);
			byte value = keyValuePair.Value;
			Cell cell2 = cell;
			cell2.light -= value;
			Cell cell3 = cell;
			cell3.lightR -= (ushort)(value * this.r / 2);
			Cell cell4 = cell;
			cell4.lightG -= (ushort)(value * this.g / 2);
			Cell cell5 = cell;
			cell5.lightB -= (ushort)(value * this.b / 2);
			if (this.isPC)
			{
				cell.pcSync = false;
			}
		}
		this.lastPoints.Clear();
	}

	public List<Point> ListPoints()
	{
		List<Point> list = new List<Point>();
		foreach (KeyValuePair<int, byte> keyValuePair in this.lastPoints)
		{
			list.Add(new Point(keyValuePair.Key));
		}
		return list;
	}

	private float GetVisDistance(int x1, int y1, int x2, int y2)
	{
		return Mathf.Sqrt((float)((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)));
	}

	public static float DistanceFloat(int x1, int y1, int x2, int y2)
	{
		return Mathf.Sqrt((float)((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)));
	}

	public static int Distance(int x1, int y1, int x2, int y2)
	{
		return (int)Mathf.Sqrt((float)((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2)));
	}

	public void Perform(int _x, int _z, int _range, float power = 1f)
	{
		Fov.range = _range;
		this.ClearVisible();
		if (Fov.range == 0)
		{
			return;
		}
		Fov._power = power;
		Fov.origin.X = _x;
		Fov.origin.Y = _z;
		this.SetVisible(Fov.origin.X, Fov.origin.Y);
		int num = Fov.origin.X - Fov.range;
		if (num < 0)
		{
			num = 0;
		}
		int num2 = Fov.origin.Y - Fov.range;
		if (num2 < 0)
		{
			num2 = 0;
		}
		int num3 = Fov.origin.X + Fov.range + 1;
		if (num3 >= EClass._map.Size)
		{
			num3 = EClass._map.Size;
		}
		int num4 = Fov.origin.Y + Fov.range + 1;
		if (num4 >= EClass._map.Size)
		{
			num4 = EClass._map.Size;
		}
		for (int i = num; i < num3; i++)
		{
			this.TraceLine(i, num2);
			this.TraceLine(i, num4 - 1);
		}
		for (int j = num2 + 1; j < num4 - 1; j++)
		{
			this.TraceLine(num, j);
			this.TraceLine(num3 - 1, j);
		}
		if (Fov.range >= 6)
		{
			int max = EClass._map.Size - 1;
			int x = (Fov.origin.X + 4).ClampMax(max);
			int y = (Fov.origin.Y + 1).ClampMax(max);
			int x2 = (Fov.origin.X - 4).ClampMin(0);
			int y2 = (Fov.origin.Y - 1).ClampMin(0);
			int x3 = (Fov.origin.X + 1).ClampMax(max);
			int y3 = (Fov.origin.Y + 4).ClampMax(max);
			int x4 = (Fov.origin.X - 1).ClampMin(0);
			int y4 = (Fov.origin.Y - 4).ClampMin(0);
			this.TraceLine(x, y);
			this.TraceLine(x, y2);
			this.TraceLine(x2, y);
			this.TraceLine(x2, y2);
			this.TraceLine(x3, y3);
			this.TraceLine(x3, y4);
			this.TraceLine(x4, y3);
			this.TraceLine(x4, y4);
		}
		foreach (KeyValuePair<int, byte> keyValuePair in this.lastPoints)
		{
			Cell cell = Fov.map.GetCell(keyValuePair.Key);
			byte value = keyValuePair.Value;
			Cell cell2 = cell;
			cell2.light += value;
			Cell cell3 = cell;
			cell3.lightR += (ushort)(value * this.r / 2);
			Cell cell4 = cell;
			cell4.lightG += (ushort)(value * this.g / 2);
			Cell cell5 = cell;
			cell5.lightB += (ushort)(value * this.b / 2);
			if (this.isPC && (value > 0 || this.GetVisDistance(Fov.origin.X, Fov.origin.Y, (int)cell.x, (int)cell.z) < 2f) && !cell.isSeen && (!cell.HasWall || EClass._zone.UseFog || cell.room == EClass.pc.Cell.room || cell.hasDoor))
			{
				EClass._map.SetSeen((int)cell.x, (int)cell.z, true, true);
				Fov.newPoints.Add(cell);
			}
		}
		if (this.isPC && Fov.newPoints.Count > 0)
		{
			WidgetMinimap.UpdateMap(Fov.newPoints);
		}
	}

	private void TraceLine(int x2, int y2)
	{
		int value = x2 - Fov.origin.X;
		int value2 = y2 - Fov.origin.Y;
		int num = Math.Abs(value);
		int num2 = Math.Abs(value2);
		int num3 = Math.Sign(value);
		int num4 = Math.Sign(value2) << 16;
		int num5 = (Fov.origin.Y << 16) + Fov.origin.X;
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
			Fov.x = (num5 & 65535);
			Fov.y = num5 >> 16;
			int num11 = (int)this.GetVisDistance(Fov.origin.X, Fov.origin.Y, Fov.x, Fov.y);
			if (Fov.range >= 0 && num11 > Fov.range)
			{
				break;
			}
			this.SetVisible(Fov.x, Fov.y);
			if (Fov.map.cells[Fov.x, Fov.y].blockSight)
			{
				break;
			}
		}
	}

	public static float nonGradientMod;

	public static List<Cell> newPoints = new List<Cell>();

	public static Map map;

	public static Fov.LevelPoint origin = default(Fov.LevelPoint);

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

	public struct LevelPoint
	{
		public int X;

		public int Y;
	}
}
