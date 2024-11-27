using System;
using System.Collections.Generic;
using UnityEngine;

public class Lot : EClass
{
	public void SetBaseRoom(Room r)
	{
		this.board = null;
		this.fullblock = false;
		Lot.minX = r.x;
		Lot.maxX = r.mx;
		Lot.minZ = r.z;
		Lot.maxZ = r.mz;
		this.mh = r.mh;
		this.pointMinX = r.pointMinX;
		this.pointMaxX = r.pointMaxX;
		this.AddConnectedRooms(r);
		this.x = Lot.minX;
		this.mx = Lot.maxX;
		this.z = Lot.minZ;
		this.mz = Lot.maxZ;
		bool flag = false;
		Lot.roofCount.Clear();
		foreach (Room room in EClass._map.rooms.listRoom)
		{
			if (room.lot == this)
			{
				if (room.roof)
				{
					flag = true;
				}
				if (room.fullblock)
				{
					this.fullblock = true;
				}
				foreach (KeyValuePair<int, int> keyValuePair in room.roofCount)
				{
					if (!Lot.roofCount.ContainsKey(keyValuePair.Key))
					{
						Lot.roofCount[keyValuePair.Key] = 0;
					}
					Dictionary<int, int> dictionary = Lot.roofCount;
					int key = keyValuePair.Key;
					dictionary[key] += keyValuePair.Value;
				}
			}
		}
		int num = 0;
		int num2 = 0;
		foreach (KeyValuePair<int, int> keyValuePair2 in Lot.roofCount)
		{
			if (keyValuePair2.Value > num)
			{
				num = keyValuePair2.Value;
				num2 = keyValuePair2.Key;
			}
		}
		if (this.board != null)
		{
			if (this.board.data == null)
			{
				TraitHouseBoard.Data data = this.board.data = new TraitHouseBoard.Data();
				CoreRef.DefaultRoof defaultRoof = EClass.core.refs.defaultRoofs[EClass.sources.blocks.map[num2 / 1000].roof];
				data.height = (this.fullblock ? 1 : 2);
				data.heightFix = (this.fullblock ? 40 : 0);
				data.reverse = (EClass.rnd(2) == 0);
				data.idRoofStyle = ((!EClass._map.IsIndoor && flag) ? 2 : 0);
				data.idBlock = defaultRoof.idBlock;
				data.idRamp = defaultRoof.idRamp;
				SourceMaterial.Row row = EClass.sources.materials.map[num2 % 1000];
				data.colRoof = IntColor.ToInt(row.matColor);
			}
			this.RefreshData(this.board.data);
		}
		else
		{
			CoreRef.DefaultRoof defaultRoof2 = EClass.core.refs.defaultRoofs[EClass.sources.blocks.map[num2 / 1000].roof];
			this.height = (this.fullblock ? 1 : 2);
			this.heightFix = (this.fullblock ? 40 : 0);
			this.idRoofStyle = ((!EClass._map.IsIndoor && flag) ? 2 : 0);
			this.idBlock = defaultRoof2.idBlock;
			this.idRamp = defaultRoof2.idRamp;
			this.realHeight = (float)this.height * EClass.setting.render.roomHeightMod + 0.01f * (float)this.heightFix;
			Color color = EClass.sources.materials.alias["oak"].matColor.SetAlpha(1f);
			this.colRoof = (this.colBlock = (this.colDeco = BaseTileMap.GetColorInt(ref color, 100)));
			SourceMaterial.Row row2 = EClass.sources.materials.map[num2 % 1000];
			this.colRoof = BaseTileMap.GetColorInt(ref row2.matColor, 100);
		}
		int num3 = (this.mx - this.x + this.pointMinX.z - this.z) / 2;
		int num4 = this.pointMaxX.x - this.pointMinX.x + this.pointMaxX.z - this.pointMinX.z + 1;
		int num5 = (this.mx - this.x + this.mz - this.z) / 2 + 2;
		int num6 = this.pointMinX.x + num3 - num5;
		int num7 = this.pointMinX.z - num3 + num5;
		int num8 = 0;
		for (int i = 0; i < num4; i++)
		{
			int num9 = num6;
			int num10 = num7;
			bool flag2 = false;
			for (int j = 0; j < num5 + 1; j++)
			{
				if (num10 >= num8 - 1 && num9 >= 0 && num10 >= 0 && num9 < EClass._map.Size && num10 < EClass._map.Size)
				{
					Cell cell = EClass._map.cells[num9, num10];
					if (flag2)
					{
						Room room2 = cell.room;
						if (((room2 != null) ? room2.lot : null) != this || (!cell.HasFullBlock && !cell.HasWall))
						{
							break;
						}
					}
					Room room3 = cell.room;
					if (((room3 != null) ? room3.lot : null) == this)
					{
						cell.lotWall = true;
						if (!flag2 && !cell.HasFullBlock && !cell.HasWall)
						{
							break;
						}
						flag2 = true;
						num8 = num10;
					}
					num9++;
					num10--;
				}
			}
			if (i % 2 == 0)
			{
				num6++;
			}
			else
			{
				num7++;
			}
		}
	}

	public void RefreshData(TraitHouseBoard.Data data)
	{
		if (data.colRoof == 0)
		{
			data.colRoof = IntColor.ToInt(EClass.sources.materials.alias["oak"].matColor.SetAlpha(1f));
		}
		if (data.colBlock == 0)
		{
			data.colBlock = IntColor.ToInt(EClass.sources.materials.alias["oak"].matColor.SetAlpha(1f));
		}
		if (data.colDeco == 0)
		{
			data.colDeco = IntColor.ToInt(EClass.sources.materials.alias["oak"].matColor.SetAlpha(1f));
		}
		if (data.colDeco2 == 0)
		{
			data.colDeco2 = IntColor.ToInt(EClass.sources.materials.alias["oak"].matColor.SetAlpha(1f));
		}
		this.height = data.height;
		this.reverse = data.reverse;
		this.altRoof = data.altRoof;
		this.snow = data.snow;
		this.idRoofStyle = data.idRoofStyle;
		this.idRoofTile = data.idRoofTile;
		this.idBlock = data.idBlock;
		this.idRamp = data.idRamp;
		this.idBGM = data.idBGM;
		this.idDeco = data.idDeco;
		this.idDeco2 = data.idDeco2;
		this.heightFix = data.heightFix;
		this.decoFix = data.decoFix;
		this.decoFix2 = data.decoFix2;
		this.realHeight = (float)this.height * EClass.setting.render.roomHeightMod + 0.01f * (float)this.heightFix;
		Color color = IntColor.FromInt(data.colRoof);
		Color color2 = IntColor.FromInt(data.colBlock);
		Color color3 = IntColor.FromInt(data.colDeco);
		Color color4 = IntColor.FromInt(data.colDeco2);
		this.colRoof = BaseTileMap.GetColorInt(ref color, 100);
		this.colBlock = BaseTileMap.GetColorInt(ref color2, 100);
		this.colDeco = BaseTileMap.GetColorInt(ref color3, 100);
		this.colDeco2 = BaseTileMap.GetColorInt(ref color4, 100);
	}

	public void AddConnectedRooms(Room r2)
	{
		r2.lot = this;
		if (r2.x < Lot.minX)
		{
			Lot.minX = r2.x;
		}
		if (r2.mx > Lot.maxX)
		{
			Lot.maxX = r2.mx;
		}
		if (r2.z < Lot.minZ)
		{
			Lot.minZ = r2.z;
		}
		if (r2.mz > Lot.maxZ)
		{
			Lot.maxZ = r2.mz;
		}
		if (r2.mh > this.mh)
		{
			this.mh = r2.mh;
		}
		if (r2.pointMinX.x + r2.pointMinX.z < this.pointMinX.x + this.pointMinX.z)
		{
			this.pointMinX = r2.pointMinX;
		}
		if (r2.pointMaxX.x + r2.pointMaxX.z > this.pointMaxX.x + this.pointMaxX.z)
		{
			this.pointMaxX = r2.pointMaxX;
		}
		if (this.board == null)
		{
			foreach (Point point in r2.points)
			{
				TraitHouseBoard installed = point.GetInstalled<TraitHouseBoard>();
				if (installed != null)
				{
					this.board = installed;
					break;
				}
			}
		}
		foreach (Room room in EClass._map.rooms.listRoom)
		{
			if (room.lot == null)
			{
				bool flag = false;
				foreach (Point p in room.points)
				{
					using (List<Point>.Enumerator enumerator3 = r2.points.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							if (enumerator3.Current.Distance(p) == 1 && r2.data.group == room.data.group)
							{
								this.AddConnectedRooms(room);
								flag = true;
								break;
							}
						}
					}
					if (flag)
					{
						break;
					}
				}
			}
		}
	}

	public static Lot Void = new Lot();

	public static Dictionary<int, int> roofCount = new Dictionary<int, int>();

	public int id;

	public int height = 2;

	public int idRoofStyle;

	public int idBGM;

	public int heightFix;

	public int idRoofTile;

	public int idBlock;

	public int idRamp;

	public int idDeco;

	public int idDeco2;

	public int colRoof;

	public int colBlock;

	public int colDeco;

	public int colDeco2;

	public int decoFix;

	public int decoFix2;

	public bool reverse;

	public bool snow;

	public bool altRoof;

	public bool fullblock;

	public int x;

	public int z;

	public int mx;

	public int mz;

	public int mh;

	public bool sync;

	public bool dirty;

	public float realHeight;

	public float light;

	public Point pointMinX;

	public Point pointMaxX;

	private static int minX;

	private static int minZ;

	private static int maxX;

	private static int maxZ;

	public TraitHouseBoard board;
}
