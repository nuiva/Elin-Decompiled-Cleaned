using System;
using NoiseSystem;

public class BaseMapGen : GenBounds
{
	public bool extraBiome
	{
		get
		{
			return this.biomeProfiles.Length > 3;
		}
	}

	public void SetSize(int size, int _poiSize)
	{
		this.Size = size;
		if (this.map.poiMap == null || this.Size != POIMap.mapSize)
		{
			this.map.poiMap = new POIMap();
			this.map.poiMap.Init(this.Size, _poiSize);
		}
	}

	public void Generate(ZoneBlueprint _bp)
	{
		BiomeProfile.Init();
		this.bp = _bp;
		this.zone = this.bp.zone;
		this.zp = this.bp.zoneProfile;
		this.map = this.bp.map;
		this.OX = this.zp.offsetX;
		this.OZ = this.zp.offsetZ;
		this.blockHeight = this.zp.blockHeight;
		this.hSetting = this.zp.height;
		MapGenVariation mapGenVariation = this.variation = this.bp.genSetting.variation;
		this.layerHeight = mapGenVariation.layerHeight;
		this.layerRiver = mapGenVariation.layerRiver;
		this.layerStratum = mapGenVariation.layerStratum;
		this.layerBiome = mapGenVariation.layerBiome;
		this.biomeProfiles = mapGenVariation.biomeProfiles;
		this.biomeShore = mapGenVariation.biomeShore;
		this.biomeWater = mapGenVariation.biomeWater;
		this.biomeSand = EClass.core.refs.biomes.Sand;
		this.GenerateTerrain();
		if (this.zp.indoor)
		{
			this.map.config.bg = MapBG.None;
			this.map.config.indoor = true;
		}
		if (this.map.config.idSceneProfile.IsEmpty())
		{
			this.map.config.idSceneProfile = this.zp.idSceneProfile.IsEmpty(this.zp.indoor ? "indoor" : null);
		}
		this.map.config.bg = this.zp.mapBG;
	}

	protected virtual void GenerateTerrain()
	{
		this.OnGenerateTerrain();
	}

	protected virtual bool OnGenerateTerrain()
	{
		return false;
	}

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
}
