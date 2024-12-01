using NoiseSystem;
using UnityEngine;

public class MapGenVariation : ScriptableObject
{
	public NoiseLayer layerHeight;

	public NoiseLayer layerRiver;

	public NoiseLayer layerStratum;

	public NoiseLayer layerBiome;

	public BiomeProfile[] biomeProfiles;

	public BiomeProfile biomeShore;

	public BiomeProfile biomeWater;

	public float maxWaterRatio;

	public bool embark = true;

	public string GetText()
	{
		return base.name;
	}
}
