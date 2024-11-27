using System;
using System.Collections.Generic;

public class Room : BaseArea
{
	public bool HasRoof
	{
		get
		{
			return this.lot.idRoofStyle != 0;
		}
	}

	public override string ToString()
	{
		return this.uid.ToString() ?? "";
	}

	public void OnLoad()
	{
		foreach (Point point in this.points)
		{
			point.cell.room = this;
		}
		EClass._map.rooms.mapIDs[this.uid] = this;
		this.SetDirty();
	}

	public void AddPoint(Point p)
	{
		this.points.Add(p);
		p.cell.room = this;
		this.lot = Lot.Void;
		this.SetDirty();
	}

	public void OnRemove()
	{
		this.Clean();
		for (int i = this.points.Count - 1; i >= 0; i--)
		{
			this.points[i].cell.room = null;
		}
		this.points.Clear();
		EClass._map.rooms.dirtyLots = true;
	}

	public void Clean()
	{
		this.plate = null;
	}

	public void Refresh()
	{
		this.Clean();
		this.roofCount.Clear();
		this.x = (this.mx = this.points[0].x);
		this.z = (this.mz = this.points[0].z);
		this.mh = 0;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		this.pointMinX = this.points[0];
		this.pointMaxX = this.points[0];
		foreach (Point point in this.points)
		{
			Cell cell = point.cell;
			if (point.x < this.x)
			{
				this.x = point.x;
			}
			else if (point.x > this.mx)
			{
				this.mx = point.x;
			}
			if (point.z < this.z)
			{
				this.z = point.z;
			}
			else if (point.z > this.mz)
			{
				this.mz = point.z;
			}
			if (point.x + point.z < this.pointMinX.x + this.pointMinX.z)
			{
				this.pointMinX = point;
			}
			if (point.x + point.z > this.pointMaxX.x + this.pointMaxX.z)
			{
				this.pointMaxX = point;
			}
			bool hasFullBlockOrWallOrFence = cell.HasFullBlockOrWallOrFence;
			if ((int)cell.TopHeight > this.mh && hasFullBlockOrWallOrFence)
			{
				this.mh = (int)cell.TopHeight;
			}
			if (hasFullBlockOrWallOrFence)
			{
				TileType tileType = point.sourceBlock.tileType;
				if (tileType.IsWall)
				{
					num3++;
				}
				else if (tileType.IsFence)
				{
					num2++;
				}
				else
				{
					num++;
				}
				int num5 = point.sourceBlock.id * 1000 + point.matBlock.id;
				if (!this.roofCount.ContainsKey(num5))
				{
					this.roofCount[num5] = 0;
				}
				Dictionary<int, int> dictionary = this.roofCount;
				int key = num5;
				int num6 = dictionary[key];
				dictionary[key] = num6 + 1;
			}
			if (this.plate == null)
			{
				this.plate = point.GetInstalled<TraitRoomPlate>();
			}
			num4 += point.x + point.z * 256;
			cell.fullWall = false;
			cell.lotWall = false;
		}
		int num7 = (this.mx - this.x + this.pointMinX.z - this.z) / 2;
		int num8 = this.pointMaxX.x - this.pointMinX.x + this.pointMaxX.z - this.pointMinX.z + 1;
		int num9 = (this.mx - this.x + this.mz - this.z) / 2 + 2;
		int num10 = this.pointMinX.x + num7 - num9;
		int num11 = this.pointMinX.z - num7 + num9;
		int num12 = 0;
		for (int i = 0; i < num8; i++)
		{
			int num13 = num10;
			int num14 = num11;
			bool flag = false;
			for (int j = 0; j < num9 + 1; j++)
			{
				if (num14 >= num12 - 1 && num13 >= 0 && num14 >= 0 && num13 < EClass._map.Size && num14 < EClass._map.Size)
				{
					Cell cell2 = EClass._map.cells[num13, num14];
					if (flag && (cell2.room != this || (!cell2.HasFullBlock && !cell2.HasWall)))
					{
						break;
					}
					if (cell2.room == this)
					{
						cell2.fullWall = true;
						if (!flag && !cell2.HasFullBlock && !cell2.HasWall)
						{
							break;
						}
						flag = true;
						num12 = num14;
					}
					num13++;
					num14--;
				}
			}
			if (i % 2 == 0)
			{
				num10++;
			}
			else
			{
				num11++;
			}
		}
		this.fullblock = (num > num3);
		this.roof = ((this.mx - this.x + (this.mz - this.z)) / 2 > num2);
		this.dirty = false;
		EClass._map.rooms.dirtyLots = true;
		this.type = new AreaTypeRoom
		{
			owner = this
		};
		base.SetRandomName(num4);
		if (this.plate != null)
		{
			this.data.name = this.plate.areaData.name;
			if (this.plate.areaData.type != null)
			{
				this.type = this.plate.areaData.type;
			}
			this.data.group = this.plate.areaData.group;
			this.data.atrium = this.plate.areaData.atrium;
		}
	}

	public void SetDirty()
	{
		this.dirty = true;
		EClass._map.rooms.dirtyRooms = true;
	}

	public int x;

	public int z;

	public int mx;

	public int mz;

	public int mh;

	public bool dirty;

	public bool roof;

	public bool fullblock;

	public Lot lot;

	public Point pointMinX;

	public Point pointMaxX;

	public Dictionary<int, int> roofCount = new Dictionary<int, int>();
}
