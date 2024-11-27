using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using CreativeSpore.SuperTilemapEditor;
using Newtonsoft.Json;
using UnityEngine;

public class EloMap : EClass
{
	public Region region
	{
		get
		{
			return EClass.world.region;
		}
	}

	public string idMap
	{
		get
		{
			return "map_ntyris";
		}
	}

	[OnSerializing]
	internal void OnSerializing(StreamingContext context)
	{
		if (this.cells != null)
		{
			this._ints = new int[this.w, this.h];
			for (int i = 0; i < this.h; i++)
			{
				for (int j = 0; j < this.w; j++)
				{
					this._ints[j, i] = this.cells[j, i].GetInt();
				}
			}
		}
	}

	[OnDeserialized]
	internal void OnDeserialized(StreamingContext context)
	{
		if (this._ints != null && this._ints.GetLength(0) > 0)
		{
			this.w = this._ints.GetLength(0);
			this.h = this._ints.GetLength(1);
			this.cells = new EloMap.Cell[this.w, this.h];
			for (int i = 0; i < this.h; i++)
			{
				for (int j = 0; j < this.w; j++)
				{
					this.cells[j, i] = new EloMap.Cell(this._ints[j, i]);
				}
			}
		}
	}

	public void Init(EloMapActor _actor)
	{
		if (this.initialized)
		{
			return;
		}
		this.actor = _actor;
		this.initialized = true;
		this.group = this.actor.transMap.GetComponentInChildren<TilemapGroup>();
		this.seaMap = this.group.Tilemaps[0];
		this.cloudmap = this.group.Tilemaps[7];
		this.extramap = this.group.Tilemaps[6];
		this.fogmap = this.group.Tilemaps[5];
		this.objmap = this.group.Tilemaps[4];
		this.objScatterMap = this.group.Tilemaps[3];
		this.w = this.fogmap.GridWidth;
		this.h = this.fogmap.GridHeight;
		this.minX = this.fogmap.MinGridX;
		this.minY = this.fogmap.MinGridY;
		if (this.cells == null)
		{
			this.cells = new EloMap.Cell[this.w, this.h];
			for (int i = 0; i < this.h; i++)
			{
				for (int j = 0; j < this.w; j++)
				{
					this.cells[j, i] = new EloMap.Cell();
				}
			}
		}
		foreach (Spatial spatial in this.region.children)
		{
			int x = spatial.x;
			int y = spatial.y;
			Zone zone = spatial as Zone;
			EloMap.Cell cell = this.GetCell(x, y);
			if (cell == null)
			{
				Debug.Log("cell is null:" + x.ToString() + "/" + y.ToString());
			}
			else
			{
				cell.zone = zone;
				if (!zone.IsInstance)
				{
					cell.obj = zone.icon;
				}
			}
		}
		for (int k = 0; k < this.h; k++)
		{
			for (int l = 0; l < this.w; l++)
			{
				EloMap.Cell cell2 = this.cells[l, k];
				int num = this.minX + l;
				int num2 = this.minY + k;
				if (cell2.obj != 0)
				{
					this.objmap.SetTile(num, num2, cell2.obj, 0, eTileFlags.None);
				}
				if (cell2.zone != null)
				{
					if (cell2.zone.UseLight)
					{
						this.AddLight(num, num2, "elolight");
					}
					if (cell2.zone.IsClosed)
					{
						this.extramap.SetTile(num, num2, 333, 0, eTileFlags.None);
					}
				}
			}
		}
		this.extramap.UpdateMeshImmediate();
		this.objmap.UpdateMeshImmediate();
	}

	public void SetZone(int gx, int gy, Zone z, bool updateMesh = false)
	{
		EloMap.Cell cell = this.GetCell(gx, gy);
		if (cell == null)
		{
			Debug.Log("cell is null:" + gx.ToString() + "/" + gy.ToString());
			return;
		}
		if (z != null && cell.obj == z.icon)
		{
			return;
		}
		cell.obj = ((z != null) ? z.icon : 0);
		if (z != null && z.source.tag.Contains("iconFlag"))
		{
			cell.obj = 306;
		}
		if (cell.zone != null && cell.zone.UseLight)
		{
			this.RemoveLight(gx, gy);
		}
		cell.zone = z;
		if (cell.obj == 0)
		{
			this.objmap.Erase(gx, gy);
		}
		else
		{
			this.objmap.SetTile(gx, gy, cell.obj, 0, eTileFlags.None);
		}
		if (z != null && z.UseLight)
		{
			this.AddLight(gx, gy, "elolight");
		}
		if (updateMesh)
		{
			this.objmap.UpdateMeshImmediate();
		}
	}

	public EloMap.Cell GetCell(Point pos)
	{
		return this.GetCell(pos.x + this.minX, pos.z + this.minY);
	}

	public EloMap.Cell GetCell(int gx, int gy)
	{
		if (gx < this.minX || gy < this.minY || gx >= this.minX + this.w || gy >= this.minY + this.h)
		{
			return null;
		}
		return this.cells[gx - this.minX, gy - this.minY];
	}

	public EloMap.TileInfo GetTileInfo(int gx, int gy)
	{
		EloMap.TileInfo t = new EloMap.TileInfo();
		bool skip = false;
		this.group.Tilemaps.ForeachReverse(delegate(STETilemap m)
		{
			if (m == this.fogmap | skip)
			{
				return;
			}
			int tileIdFromTileData = Tileset.GetTileIdFromTileData(m.GetTileData(gx, gy));
			TileData tileData = new TileData(m.GetTileData(gx, gy));
			int tileId = tileData.tileId;
			if (tileId >= 22 && tileId <= 25)
			{
				bool flipHorizontal = tileData.flipHorizontal;
				bool flipVertical = tileData.flipVertical;
				bool rot = tileData.rot90;
				int num = (flipHorizontal ? 1 : 0) + (flipVertical ? 1 : 0) * 2 + (rot ? 1 : 0) * 4;
				t.isRoad = true;
				t.roadLeft = (tileId == 23 || (tileId == 24 && (num == 4 || num == 0)) || (tileId == 25 && num != 6));
				t.roadRight = (tileId == 23 || (tileId == 24 && (num == 6 || num == 1)) || (tileId == 25 && num != 4));
				t.roadUp = (tileId == 22 || (tileId == 24 && (num == 4 || num == 6)) || (tileId == 25 && num != 0));
				t.roadDown = (tileId == 22 || (tileId == 24 && (num == 0 || num == 1)) || (tileId == 25 && num != 2));
			}
			SourceGlobalTile.Row row = EClass.sources.globalTiles.tileAlias.TryGetValue(tileIdFromTileData, null);
			if (row == null)
			{
				return;
			}
			t.source = row;
			t.sprite = TilemapUtils.GetOrCreateTileSprite(this.actor.tileset, row.tiles[0], 0f);
			string alias = row.alias;
			if (!(alias == "bridge"))
			{
				if (!(alias == "wall") && !(alias == "rock"))
				{
					if (!(alias == "sea"))
					{
						if (alias == "beach")
						{
							t.shore = true;
						}
					}
					else
					{
						t.sea = true;
					}
				}
				else
				{
					t.rock = true;
				}
			}
			if (!row.zoneProfile.IsEmpty())
			{
				skip = true;
				return;
			}
			if (row.attribs[0] == 0)
			{
				t.blocked = true;
			}
		});
		return t;
	}

	public List<SourceGlobalTile.Row> GetSources(int gx, int gy)
	{
		List<SourceGlobalTile.Row> list = new List<SourceGlobalTile.Row>();
		foreach (STETilemap stetilemap in this.group.Tilemaps)
		{
			if (!(stetilemap == this.fogmap))
			{
				int tileIdFromTileData = Tileset.GetTileIdFromTileData(stetilemap.GetTileData(gx, gy));
				if (EClass.sources.globalTiles.tileAlias.ContainsKey(tileIdFromTileData))
				{
					list.Add(EClass.sources.globalTiles.tileAlias.TryGetValue(tileIdFromTileData, null));
				}
			}
		}
		return list;
	}

	public bool CanBuildSite(int gx, int gy, int radius = 0, ElomapSiteType type = ElomapSiteType.Nefia)
	{
		if (radius != 0)
		{
			for (int i = gy - radius; i < gy + radius + 1; i++)
			{
				for (int j = gx - radius; j < gx + radius + 1; j++)
				{
					if (!this.CanBuildSite(j, i, 0, ElomapSiteType.Nefia))
					{
						return false;
					}
				}
			}
			return true;
		}
		EloMap.Cell cell = this.GetCell(gx, gy);
		if (cell == null || cell.zone != null || this.cloudmap.GetTileData(gx, gy) != 4294967295U)
		{
			return false;
		}
		SourceGlobalTile.Row row = this.GetSources(gx, gy).LastItem<SourceGlobalTile.Row>();
		if (type == ElomapSiteType.Mob)
		{
			if (row.id == 4 && EClass.rnd(5) == 0)
			{
				return false;
			}
			if (row.id == 7 && EClass.rnd(2) == 0)
			{
				return false;
			}
		}
		else
		{
			if (row == null || !row.tag.Contains("site"))
			{
				return false;
			}
			if (row.id == 7 && EClass.rnd(5) == 0)
			{
				return false;
			}
		}
		return true;
	}

	public bool IsSnow(int gx, int gy)
	{
		if (this.GetCell(gx, gy) == null)
		{
			return false;
		}
		SourceGlobalTile.Row row = this.GetSources(gx, gy).LastItem<SourceGlobalTile.Row>();
		return row != null && row.id == 7;
	}

	public Zone GetZone(Point p)
	{
		return this.GetZone(p.x + this.minX, p.z + this.minY);
	}

	public Zone GetZone(int gx, int gy)
	{
		Zone zone = null;
		foreach (Spatial spatial in this.region.children)
		{
			if (spatial.x == gx && spatial.y == gy)
			{
				Zone zone2 = spatial as Zone;
				if (((zone2 != null) ? zone2.instance : null) == null && (zone == null || zone is Zone_Field))
				{
					zone = (spatial as Zone);
				}
			}
		}
		return zone;
	}

	public int GetRoadDist(int gx, int gy)
	{
		if (!this.initialized)
		{
			EClass.scene.elomapActor.Initialize(EClass.world.region.elomap);
		}
		for (int i = 0; i < 100; i++)
		{
			for (int j = gy - i; j < gy + i + 1; j++)
			{
				for (int k = gx - i; k < gx + i + 1; k++)
				{
					if (j == gy - i || j == gy + i || k == gx - i || k == gx + i)
					{
						uint tileData = this.objScatterMap.GetTileData(k, j);
						if (((tileData != 4294967295U) ? ((tileData & 268369920U) >> 16) : 0U) == 3U)
						{
							return i;
						}
						tileData = this.seaMap.GetTileData(k, j);
						if (((tileData != 4294967295U) ? ((tileData & 268369920U) >> 16) : 0U) == 3U)
						{
							return i;
						}
					}
				}
			}
		}
		return 100;
	}

	public void AddLight(int gx, int gy, string id = "elolight")
	{
		SpriteRenderer spriteRenderer = Util.Instantiate<SpriteRenderer>(id, this.actor.transLight);
		EloMapLight item = new EloMapLight
		{
			sr = spriteRenderer,
			gx = gx,
			gy = gy
		};
		this.actor.lights.Add(item);
		spriteRenderer.transform.position = TilemapUtils.GetGridWorldPos(this.fogmap, gx, gy);
	}

	public void RemoveLight(int gx, int gy)
	{
		foreach (EloMapLight eloMapLight in this.actor.lights)
		{
			if (eloMapLight.gx == gx && eloMapLight.gy == gy)
			{
				UnityEngine.Object.DestroyImmediate(eloMapLight.sr.gameObject);
				this.actor.lights.Remove(eloMapLight);
				break;
			}
		}
	}

	[JsonProperty]
	public int[,] _ints;

	public EloMap.Cell[,] cells;

	public TilemapGroup group;

	public STETilemap fogmap;

	public STETilemap seaMap;

	public STETilemap objScatterMap;

	public STETilemap objmap;

	public STETilemap extramap;

	public STETilemap cloudmap;

	public int w;

	public int h;

	public int minX;

	public int minY;

	public bool initialized;

	public EloMapActor actor;

	public class Cell
	{
		public Cell()
		{
		}

		public Cell(int i)
		{
			this.obj = i / 10;
		}

		public int GetInt()
		{
			return this.obj * 10;
		}

		public Zone zone;

		public int obj;
	}

	public class TileInfo
	{
		public bool IsSnow
		{
			get
			{
				return this.idSurface == "snow_edge" || this.idSurface == "snow";
			}
		}

		public string idSurface
		{
			get
			{
				SourceGlobalTile.Row row = this.source;
				return ((row != null) ? row.alias : null) ?? "";
			}
		}

		public string idZoneProfile
		{
			get
			{
				return this.source.zoneProfile;
			}
		}

		public string name
		{
			get
			{
				return this.source.GetName();
			}
		}

		public bool CanEmbark
		{
			get
			{
				return this.idZoneProfile != null;
			}
		}

		public bool IsBridge
		{
			get
			{
				return this.idSurface == "bridge";
			}
		}

		public bool IsNeighborRoad
		{
			get
			{
				return this.roadLeft || this.roadRight || this.roadUp || this.roadDown;
			}
		}

		public Sprite sprite;

		public bool blocked;

		public bool isRoad;

		public bool roadLeft;

		public bool roadRight;

		public bool roadUp;

		public bool roadDown;

		public bool rock;

		public bool sea;

		public bool shore;

		public SourceGlobalTile.Row source;
	}
}
