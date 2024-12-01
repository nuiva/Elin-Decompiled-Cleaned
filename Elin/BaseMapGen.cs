using System;
using NoiseSystem;

public class BaseMapGen : GenBounds
{
	public static string err;

	[NonSerialized]
	public NoiseLayer layerHeight;

	[NonSerialized]
	public NoiseLayer layerRiver;

	[NonSerialized]
	public NoiseLayer layerStratum;

	[NonSerialized]
	public NoiseLayer layerBiome;

	[NonSerialized]
	public BiomeProfile[] biomeProfiles;

	[NonSerialized]
	public BiomeProfile biomeShore;

	[NonSerialized]
	public BiomeProfile biomeSand;

	[NonSerialized]
	public BiomeProfile biomeWater;

	[NonSerialized]
	public Crawler[] crawlers;

	[NonSerialized]
	protected bool skipWater;

	[NonSerialized]
	public ZoneBlueprint bp;

	[NonSerialized]
	public ZoneProfile zp;

	[NonSerialized]
	protected float[,] heights1;

	[NonSerialized]
	protected float[,] heights2;

	[NonSerialized]
	protected float[,] heights3;

	[NonSerialized]
	protected float[,] waters;

	[NonSerialized]
	protected float[,] heights3d;

	[NonSerialized]
	public int blockHeight;

	[NonSerialized]
	public int seed;

	[NonSerialized]
	public int lastSize;

	[NonSerialized]
	public int OX;

	[NonSerialized]
	public int OZ;

	[NonSerialized]
	protected float waterCount;

	[NonSerialized]
	public BiomeProfile[,] biomes;

	[NonSerialized]
	public bool[,] subBiomes;

	[NonSerialized]
	public MapGenVariation variation;

	[NonSerialized]
	public MapHeight hSetting;

	public bool extraBiome => biomeProfiles.Length > 3;

	public void SetSize(int size, int _poiSize)
	{
		Size = size;
		if (map.poiMap == null || Size != POIMap.mapSize)
		{
			map.poiMap = new POIMap();
			map.poiMap.Init(Size, _poiSize);
		}
	}

	public void Generate(ZoneBlueprint _bp)
	{
		BiomeProfile.Init();
		bp = _bp;
		zone = bp.zone;
		zp = bp.zoneProfile;
		map = bp.map;
		OX = zp.offsetX;
		OZ = zp.offsetZ;
		blockHeight = zp.blockHeight;
		hSetting = zp.height;
		MapGenVariation mapGenVariation = (variation = bp.genSetting.variation);
		layerHeight = mapGenVariation.layerHeight;
		layerRiver = mapGenVariation.layerRiver;
		layerStratum = mapGenVariation.layerStratum;
		layerBiome = mapGenVariation.layerBiome;
		biomeProfiles = mapGenVariation.biomeProfiles;
		biomeShore = mapGenVariation.biomeShore;
		biomeWater = mapGenVariation.biomeWater;
		biomeSand = EClass.core.refs.biomes.Sand;
		GenerateTerrain();
		if (zp.indoor)
		{
			map.config.bg = MapBG.None;
			map.config.indoor = true;
		}
		if (map.config.idSceneProfile.IsEmpty())
		{
			map.config.idSceneProfile = zp.idSceneProfile.IsEmpty(zp.indoor ? "indoor" : null);
		}
		map.config.bg = zp.mapBG;
	}

	protected virtual void GenerateTerrain()
	{
		OnGenerateTerrain();
	}

	protected virtual bool OnGenerateTerrain()
	{
		return false;
	}
}
