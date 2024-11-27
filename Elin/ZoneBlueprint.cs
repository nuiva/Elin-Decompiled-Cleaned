using System;
using System.Collections.Generic;

public class ZoneBlueprint : EClass
{
	public EloMap.TileInfo tileCenter
	{
		get
		{
			if (this.surrounding == null)
			{
				return null;
			}
			return this.surrounding[1, 1];
		}
	}

	public void Create()
	{
		this.OnCreate();
	}

	public virtual void OnCreate()
	{
	}

	public void GenerateMap(Zone zone)
	{
		this.zone = zone;
		this.idDebug = ZoneBlueprint.debugCount;
		ZoneBlueprint.debugCount++;
		if (this.map == null)
		{
			this.map = new Map();
		}
		zone.map = this.map;
		if (!this.zoneProfile)
		{
			this.zoneProfile = zone.GetProfile();
		}
		if (!this.genSetting.variation)
		{
			this.genSetting.variation = this.zoneProfile.variation.Instantiate<MapGenVariation>();
		}
		this.genSetting.seed = ((this.zoneProfile.seeds.height == -1) ? (Rand.rnd(10000) + 1) : this.zoneProfile.seeds.height);
		bool flag = false;
		if (zone.IDGenerator != null)
		{
			MapGenDungen.Instance.Generate(this);
		}
		else if (zone.IsRegion)
		{
			MapGenRegion.Instance.Generate(this);
		}
		else
		{
			flag = true;
			MapGen.Instance.Generate(this);
		}
		this.map.SetZone(zone);
		if (flag)
		{
			MapGen.Instance.Populate(this.map);
		}
	}

	public Map map;

	public Zone zone;

	public ZoneProfile zoneProfile;

	public List<Chara> charas = new List<Chara>();

	public List<Thing> things = new List<Thing>();

	public ZoneBlueprint.MapGenSetting genSetting;

	public int idDebug;

	public EloMap.TileInfo[,] surrounding;

	public static int debugCount;

	public bool customMap;

	public bool ignoreRoad;

	public struct MapGenSetting
	{
		public int seed;

		public MapGenVariation variation;
	}
}
