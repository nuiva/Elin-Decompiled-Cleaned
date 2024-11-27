using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Algorithms;
using Newtonsoft.Json;
using UnityEngine;

public class Spatial : EClass
{
	public int GetInt(int id, int? defaultInt = null)
	{
		int result;
		if (this.mapInt.TryGetValue(id, out result))
		{
			return result;
		}
		return defaultInt.GetValueOrDefault();
	}

	public void AddInt(int id, int value)
	{
		this.SetInt(id, this.GetInt(id, null) + value);
	}

	public void SetInt(int id, int value = 0)
	{
		if (value == 0)
		{
			if (this.mapInt.ContainsKey(id))
			{
				this.mapInt.Remove(id);
			}
			return;
		}
		this.mapInt[id] = value;
	}

	public int _bits
	{
		get
		{
			return this._ints[0];
		}
		set
		{
			this._ints[0] = value;
		}
	}

	public int uid
	{
		get
		{
			return this._ints[1];
		}
		set
		{
			this._ints[1] = value;
		}
	}

	public int icon
	{
		get
		{
			return this._ints[2];
		}
		set
		{
			this._ints[2] = value;
		}
	}

	public int x
	{
		get
		{
			return this._ints[3];
		}
		set
		{
			this._ints[3] = value;
		}
	}

	public int y
	{
		get
		{
			return this._ints[4];
		}
		set
		{
			this._ints[4] = value;
		}
	}

	public int lastActive
	{
		get
		{
			return this._ints[5];
		}
		set
		{
			this._ints[5] = value;
		}
	}

	public int idPrefix
	{
		get
		{
			return this._ints[6];
		}
		set
		{
			this._ints[6] = value;
		}
	}

	public int lv
	{
		get
		{
			return this._ints[7];
		}
		set
		{
			this._ints[7] = value;
		}
	}

	public int visitCount
	{
		get
		{
			return this._ints[8];
		}
		set
		{
			this._ints[8] = value;
		}
	}

	public int dateExpire
	{
		get
		{
			return this._ints[9];
		}
		set
		{
			this._ints[9] = value;
		}
	}

	public int dateRevive
	{
		get
		{
			return this._ints[10];
		}
		set
		{
			this._ints[10] = value;
		}
	}

	public int _dangerLv
	{
		get
		{
			return this._ints[11];
		}
		set
		{
			this._ints[11] = value;
		}
	}

	public int dateRegenerate
	{
		get
		{
			return this._ints[12];
		}
		set
		{
			this._ints[12] = value;
		}
	}

	public int influence
	{
		get
		{
			return this._ints[13];
		}
		set
		{
			this._ints[13] = value;
		}
	}

	public int investment
	{
		get
		{
			return this._ints[14];
		}
		set
		{
			this._ints[14] = value;
		}
	}

	public int development
	{
		get
		{
			return this._ints[15];
		}
		set
		{
			this._ints[15] = value;
		}
	}

	public int electricity
	{
		get
		{
			return this._ints[16];
		}
		set
		{
			this._ints[16] = value;
		}
	}

	public int dateHat
	{
		get
		{
			return this._ints[17];
		}
		set
		{
			this._ints[17] = value;
		}
	}

	public int uidBoss
	{
		get
		{
			return this._ints[18];
		}
		set
		{
			this._ints[18] = value;
		}
	}

	public int dateQuest
	{
		get
		{
			return this._ints[19];
		}
		set
		{
			this._ints[19] = value;
		}
	}

	public int version
	{
		get
		{
			return this._ints[20];
		}
		set
		{
			this._ints[20] = value;
		}
	}

	public bool isGenerated
	{
		get
		{
			return this.bits[0];
		}
		set
		{
			this.bits[0] = value;
		}
	}

	public bool isShore
	{
		get
		{
			return this.bits[3];
		}
		set
		{
			this.bits[3] = value;
		}
	}

	public bool isRandomSite
	{
		get
		{
			return this.bits[6];
		}
		set
		{
			this.bits[6] = value;
		}
	}

	public bool isKnown
	{
		get
		{
			return this.bits[7];
		}
		set
		{
			this.bits[7] = value;
		}
	}

	public bool isMapSaved
	{
		get
		{
			return this.bits[8];
		}
		set
		{
			this.bits[8] = value;
		}
	}

	public bool isExternalZone
	{
		get
		{
			return this.bits[9];
		}
		set
		{
			this.bits[9] = value;
		}
	}

	public bool isConquered
	{
		get
		{
			return this.bits[10];
		}
		set
		{
			this.bits[10] = value;
		}
	}

	public bool isBeach
	{
		get
		{
			return this.bits[11];
		}
		set
		{
			this.bits[11] = value;
		}
	}

	public bool isPeace
	{
		get
		{
			return this.bits[12];
		}
		set
		{
			this.bits[12] = value;
		}
	}

	public bool isDeathLocation
	{
		get
		{
			return this.bits[13];
		}
		set
		{
			this.bits[13] = value;
		}
	}

	public Faction mainFaction
	{
		get
		{
			return EClass.game.factions.dictAll.TryGetValue(this.idMainFaction, null) ?? EClass.Wilds;
		}
		set
		{
			this.idMainFaction = ((value == null) ? EClass.Wilds.uid : value.uid);
		}
	}

	public SourceZone.Row source
	{
		get
		{
			SourceZone.Row result;
			if ((result = this._source) == null)
			{
				result = (this._source = EClass.sources.zones.map[this.id]);
			}
			return result;
		}
	}

	public bool IsPlayerFaction
	{
		get
		{
			return this.mainFaction == EClass.pc.faction;
		}
	}

	public bool IsClosed
	{
		get
		{
			return this.source.tag.Contains("closed");
		}
	}

	public int mapX
	{
		get
		{
			if (!EClass._zone.IsRegion)
			{
				return this.x;
			}
			return this.x - EClass.scene.elomap.minX;
		}
	}

	public int mapY
	{
		get
		{
			if (!EClass._zone.IsRegion)
			{
				return this.y;
			}
			return this.y - EClass.scene.elomap.minY;
		}
	}

	public virtual int DangerLv
	{
		get
		{
			return this._dangerLv;
		}
	}

	public virtual int DangerLvFix
	{
		get
		{
			return 0;
		}
	}

	public virtual float VolumeSea
	{
		get
		{
			if (!this.isShore)
			{
				return 0f;
			}
			return 1f;
		}
	}

	public virtual bool ShowDangerLv
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanSpawnAdv
	{
		get
		{
			return false;
		}
	}

	public string pathSave
	{
		get
		{
			return GameIO.pathCurrentSave + this.uid.ToString() + "/";
		}
	}

	public virtual string Name
	{
		get
		{
			return ((this.idPrefix == 0) ? "" : (EClass.sources.zoneAffixes.map[this.idPrefix].GetName().ToTitleCase(false) + Lang.space)) + this.name.IsEmpty(this.source.GetText("name", false)) + this.NameSuffix;
		}
	}

	public string NameWithDangerLevel
	{
		get
		{
			return this.Name + (this.isRandomSite ? "dangerLv".lang(this.DangerLv.ToString() ?? "", null, null, null, null) : "");
		}
	}

	public virtual string NameSuffix
	{
		get
		{
			return "";
		}
	}

	public override string ToString()
	{
		return this.Name + "(" + this.uid.ToString() + ")";
	}

	public virtual bool IsRegion
	{
		get
		{
			return false;
		}
	}

	public virtual bool CanFastTravel
	{
		get
		{
			return !this.isRandomSite && !this.IsClosed;
		}
	}

	public EloMap.TileInfo Tile
	{
		get
		{
			if (this._tile == null)
			{
				EClass.scene.elomapActor.Initialize(EClass.world.region.elomap);
				this._tile = EClass.scene.elomapActor.elomap.GetTileInfo(this.x, this.y);
			}
			return this._tile;
		}
	}

	public virtual bool IsSnowZone
	{
		get
		{
			return this.Tile.IsSnow;
		}
	}

	public virtual bool IsSnowCovered
	{
		get
		{
			return !EClass._map.IsIndoor && (this.IsSnowZone || EClass.world.season.isWinter);
		}
	}

	public virtual Point RegionPos
	{
		get
		{
			return this._regionPos.Set(this.x - EClass.scene.elomap.minX, this.y - EClass.scene.elomap.minY);
		}
	}

	public virtual bool isClaimable
	{
		get
		{
			return false;
		}
	}

	public int Seed
	{
		get
		{
			return EClass.game.seed + Mathf.Abs(this.x) * 1000 + Mathf.Abs(this.y);
		}
	}

	[OnSerializing]
	private void OnSerializing(StreamingContext context)
	{
		this._bits = (int)this.bits.Bits;
	}

	[OnDeserialized]
	private void OnDeserialized(StreamingContext context)
	{
		this.bits.Bits = (uint)this._bits;
	}

	public void Create(string _id, int _x, int _y, int _icon)
	{
		this.x = _x;
		this.y = _y;
		this.icon = _icon;
		Rand.SetSeed(this.Seed);
		this.id = _id;
		this.development = this.source.dev;
		this.source.name == "*r";
		this._dangerLv = this.source.LV;
		if (EClass.debug.travelAnywhere)
		{
			this.isKnown = true;
		}
		this.OnCreate();
		Rand.SetSeed(-1);
	}

	public void Register()
	{
		if (this.uid == 0)
		{
			EClass.game.spatials.AssignUID(this);
		}
	}

	public virtual void OnCreate()
	{
	}

	public virtual void OnAfterCreate()
	{
	}

	public virtual void SetMainFaction(Faction f)
	{
		this.mainFaction = f;
	}

	public Zone GetFirstZone()
	{
		if (this is Zone)
		{
			return this as Zone;
		}
		foreach (Spatial spatial in this.children)
		{
			Zone firstZone = spatial.GetFirstZone();
			if (firstZone != null)
			{
				return firstZone;
			}
		}
		return null;
	}

	public void OnBeforeSave()
	{
		this._OnBeforeSave();
	}

	public virtual void _OnBeforeSave()
	{
	}

	public void _OnLoad()
	{
		foreach (Spatial spatial in this.children)
		{
			spatial._OnLoad();
		}
		this.OnLoad();
	}

	public virtual void OnLoad()
	{
	}

	public virtual void Destroy()
	{
	}

	public void DeleteMapRecursive()
	{
		foreach (Spatial spatial in this.children)
		{
			spatial.DeleteMapRecursive();
		}
		IO.DeleteDirectory(this.pathSave);
	}

	public virtual void AddChild(Spatial child)
	{
		child.parent = this;
		this.children.Add(child);
	}

	public void RemoveChild(Spatial child)
	{
		this.children.Remove(child);
		child.parent = null;
	}

	public Zone FindDeepestZone()
	{
		Spatial spatial = this;
		foreach (Spatial spatial2 in this.children)
		{
			if (!spatial2.isExternalZone && spatial2.lv < spatial.lv)
			{
				spatial = spatial2;
			}
		}
		return spatial as Zone;
	}

	public Zone FindZone(int lv)
	{
		foreach (Spatial spatial in this.children)
		{
			if (!spatial.isExternalZone && spatial.lv == lv)
			{
				return spatial as Zone;
			}
		}
		if (this.lv != lv)
		{
			return null;
		}
		return this as Zone;
	}

	public Zone FindZone(string _id)
	{
		foreach (Spatial spatial in this.children)
		{
			Zone zone = spatial._FindZone(_id);
			if (zone != null)
			{
				return zone;
			}
		}
		return null;
	}

	protected Zone _FindZone(string _id)
	{
		foreach (Spatial spatial in this.children)
		{
			Zone zone = spatial._FindZone(_id);
			if (zone != null)
			{
				return zone;
			}
		}
		if (this is Zone && this.id == _id)
		{
			return (Zone)this;
		}
		return null;
	}

	public virtual bool CanKill()
	{
		return false;
	}

	public int Dist(Point p)
	{
		PathProgress pathProgress = PathManager.Instance.RequestPathImmediate(new Point(this.x - EClass.scene.elomap.minX, this.y - EClass.scene.elomap.minY), p, EClass.pc, PathManager.MoveType.Default, -1, 0);
		if (pathProgress.HasPath)
		{
			int num = 0;
			foreach (PathFinderNode pathFinderNode in pathProgress.nodes)
			{
				EloMap.TileInfo tileInfo = EClass.scene.elomap.GetTileInfo(pathFinderNode.X + EClass.scene.elomap.minX, pathFinderNode.Z + EClass.scene.elomap.minY);
				bool flag = tileInfo != null && tileInfo.IsSnow;
				num += (flag ? 2 : 1);
			}
			return num;
		}
		return Fov.Distance(this.x, this.y, p.x + EClass.scene.elomap.minX, p.z + EClass.scene.elomap.minY);
	}

	public int Dist(Spatial s)
	{
		return Fov.Distance(s.x, s.y, this.x, this.y);
	}

	public void MakeGameObject(GameObject parentGo)
	{
		GameObject gameObject = new GameObject();
		gameObject.name = this.ToString();
		gameObject.transform.SetParent(parentGo.transform);
		gameObject.AddComponent<SpatialInspector>().spatial = this;
		foreach (Spatial spatial in this.children)
		{
			spatial.MakeGameObject(gameObject);
		}
	}

	[JsonProperty]
	public Spatial parent;

	[JsonProperty]
	public string id;

	[JsonProperty]
	public string idUser;

	[JsonProperty]
	public string name;

	[JsonProperty]
	public string idMainFaction;

	[JsonProperty]
	public string idProfile;

	[JsonProperty]
	public string idCurrentSubset;

	[JsonProperty]
	public string idHat;

	[JsonProperty]
	public int[] _ints = new int[30];

	[JsonProperty]
	public List<Spatial> children = new List<Spatial>();

	[JsonProperty]
	public List<Spatial> connections = new List<Spatial>();

	[JsonProperty(PropertyName = "Y")]
	public Dictionary<int, int> mapInt = new Dictionary<int, int>();

	public bool destryoed;

	public bool isImported;

	public BitArray32 bits;

	private SourceZone.Row _source;

	private EloMap.TileInfo _tile;

	protected Point _regionPos = new Point();
}
