using System;
using System.Collections.Generic;
using UnityEngine;

public class GenBounds : EClass
{
	private bool IsSub(BiomeProfile.Tile g, int x, int y)
	{
		BiomeProfile.BaseTile.SubType subType = g.subType;
		switch (subType)
		{
		case BiomeProfile.BaseTile.SubType.Rnd5:
			return EClass.rnd(5) == 0;
		case BiomeProfile.BaseTile.SubType.Rnd10:
			return EClass.rnd(10) == 0;
		case BiomeProfile.BaseTile.SubType.Rnd20:
			return EClass.rnd(20) == 0;
		default:
			return subType == BiomeProfile.BaseTile.SubType.Pattern && (x + y % 2) % 2 == 0;
		}
	}

	public void SetFloor(BiomeProfile.Tile t, int x, int z)
	{
		bool flag = this.IsSub(t, x, z);
		this.SetFloor(x, z, flag ? t.matSub : t.mat, flag ? t.idSub : t.id, EClass.rnd(EClass.rnd(8) + 1));
	}

	public void SetBlock(BiomeProfile.Tile t, int x, int z)
	{
		bool flag = this.IsSub(t, x, z);
		this.SetBlock(x, z, flag ? t.matSub : t.mat, flag ? t.idSub : t.id, 0);
	}

	public void SetFloor(int x, int z, int idMat, int idFloor, int direction = 0)
	{
		Cell cell = this.map.cells[x, z];
		cell._floorMat = (byte)idMat;
		cell._floor = (byte)idFloor;
		cell.floorDir = direction;
	}

	public void SetBlock(int x, int z, int idMat, int idBlock, int direction = 0)
	{
		Cell cell = this.map.cells[x, z];
		cell._blockMat = (byte)idMat;
		cell._block = (byte)idBlock;
		cell.blockDir = direction;
		cell.effect = null;
	}

	public bool IsEmpty()
	{
		for (int i = this.y - this.marginPartial; i < this.y + this.height + this.marginPartial; i++)
		{
			if (i < 0 || i >= EClass._map.Size)
			{
				return false;
			}
			for (int j = this.x - this.marginPartial; j < this.x + this.width + this.marginPartial; j++)
			{
				if (j < 0 || j >= EClass._map.Size)
				{
					return false;
				}
				Cell cell = this.map.cells[j, i];
				if (this.FuncCheckEmpty != null && !this.FuncCheckEmpty(cell))
				{
					return false;
				}
				if (cell.blocked || cell.HasBlock || cell.Installed != null)
				{
					return false;
				}
			}
		}
		return true;
	}

	public List<Point> ListEmptyPoint()
	{
		List<Point> list = new List<Point>();
		for (int i = this.y; i < this.y + this.height; i++)
		{
			if (i >= 0 && i < EClass._map.Size)
			{
				for (int j = this.x; j < this.x + this.width; j++)
				{
					if (j >= 0 && j < EClass._map.Size)
					{
						Cell cell = this.map.cells[j, i];
						if (!cell.blocked && !cell.HasBlock && (cell.Installed == null || !cell.Installed.trait.IsBlockPath))
						{
							Point point = new Point(j, i);
							if (!point.HasChara)
							{
								list.Add(point);
							}
						}
					}
				}
			}
		}
		return list;
	}

	public static GenBounds Create(Zone z)
	{
		MapBounds bounds = z.map.bounds;
		return new GenBounds
		{
			zone = z,
			map = z.map,
			x = bounds.x,
			y = bounds.z,
			width = bounds.Width,
			height = bounds.Height
		};
	}

	public GenBounds GetBounds(int w, int h, bool ignoreBlock)
	{
		return this.GetBounds(this.map, this.zone, this.x, this.y, this.width, this.height, w, h, ignoreBlock);
	}

	public GenBounds GetBounds(Map map, Zone zone, int x, int y, int width, int height, int dw, int dh, bool ignoreBlock)
	{
		if (dw >= width || dh >= height)
		{
			return null;
		}
		GenBounds genBounds = new GenBounds
		{
			x = x,
			y = y,
			width = dw,
			height = dh,
			map = map,
			zone = zone,
			marginPartial = this.marginPartial,
			FuncCheckEmpty = this.FuncCheckEmpty
		};
		for (int i = 0; i < 200; i++)
		{
			if (ignoreBlock)
			{
				genBounds.x = EClass.rnd(width - dw) + x - dw / 2;
				genBounds.y = EClass.rnd(height - dh) + y - dh / 2;
				if (genBounds.x > 0 && genBounds.y > 0)
				{
					return genBounds;
				}
			}
			else
			{
				genBounds.x = EClass.rnd(width - dw) + x;
				genBounds.y = EClass.rnd(height - dh) + y;
				if (genBounds.IsEmpty())
				{
					return genBounds;
				}
			}
		}
		return null;
	}

	public PartialMap TryAddMapPiece(MapPiece.Type type = MapPiece.Type.Any, float ruin = -1f, string tags = null, Action<PartialMap, GenBounds> onCreate = null)
	{
		if (ruin == -1f)
		{
			ruin = this.zone.RuinChance;
		}
		PartialMap partialMap = MapPiece.Instance.GetMap(type, tags.IsEmpty(this.zone.biome.tags), ruin);
		if (partialMap == null)
		{
			Debug.Log("TryAddMap Piece: no map");
			return null;
		}
		bool flag = partialMap.dir == 1 || partialMap.dir == 3;
		GenBounds bounds = this.GetBounds(flag ? partialMap.h : partialMap.w, flag ? partialMap.w : partialMap.h, partialMap.ignoreBlock);
		if (bounds == null)
		{
			return null;
		}
		partialMap.Apply(new Point(bounds.x, bounds.y), PartialMap.ApplyMode.Apply);
		if (onCreate != null)
		{
			onCreate(partialMap, bounds);
		}
		Debug.Log(partialMap.path);
		return partialMap;
	}

	public Map map;

	public Zone zone;

	public int Size;

	public int x;

	public int y;

	public int width;

	public int height;

	public int marginPartial;

	public Func<Cell, bool> FuncCheckEmpty;
}
