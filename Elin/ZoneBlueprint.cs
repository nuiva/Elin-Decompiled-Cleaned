using System.Collections.Generic;

public class ZoneBlueprint : EClass
{
	public struct MapGenSetting
	{
		public int seed;

		public MapGenVariation variation;
	}

	public Map map;

	public Zone zone;

	public ZoneProfile zoneProfile;

	public List<Chara> charas = new List<Chara>();

	public List<Thing> things = new List<Thing>();

	public MapGenSetting genSetting;

	public int idDebug;

	public EloMap.TileInfo[,] surrounding;

	public static int debugCount;

	public bool customMap;

	public bool ignoreRoad;

	public EloMap.TileInfo tileCenter
	{
		get
		{
			if (surrounding == null)
			{
				return null;
			}
			return surrounding[1, 1];
		}
	}

	public void Create()
	{
		OnCreate();
	}

	public virtual void OnCreate()
	{
	}

	public void GenerateMap(Zone zone)
	{
		this.zone = zone;
		idDebug = debugCount;
		debugCount++;
		if (map == null)
		{
			map = new Map();
		}
		zone.map = map;
		if (!zoneProfile)
		{
			zoneProfile = zone.GetProfile();
		}
		if (!genSetting.variation)
		{
			genSetting.variation = zoneProfile.variation.Instantiate();
		}
		genSetting.seed = ((zoneProfile.seeds.height == -1) ? (Rand.rnd(10000) + 1) : zoneProfile.seeds.height);
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
		map.SetZone(zone);
		if (flag)
		{
			MapGen.Instance.Populate(map);
		}
	}
}
