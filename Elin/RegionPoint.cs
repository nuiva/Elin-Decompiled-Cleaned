using System;
using UnityEngine;

public class RegionPoint : Point
{
	public RegionPoint(Point p)
	{
		this.SetRegionPoint(p.x, p.z);
	}

	public void SetRegionPoint(int _x, int _z)
	{
		base.Set(_x + EClass.scene.elomap.minX, _z + EClass.scene.elomap.minY);
		EClass._zone.Region.GetZoneAt(this.x, this.z);
		int roadDist = EClass.scene.elomap.GetRoadDist(this.x, this.z);
		int num = (EClass.pc.homeBranch == null) ? 0 : EClass.pc.pos.Distance(EClass.game.StartZone.mapX, EClass.game.StartZone.mapY);
		this.tile = EClass.scene.elomapActor.elomap.GetTileInfo(this.x, this.z);
		string key = this.tile.source.idBiome.IsEmpty("Plain");
		this.biome = EClass.core.refs.biomes.dict[key];
		int num2 = Mathf.Clamp(roadDist - 4, 0, 200) + Mathf.Clamp(num / 4, 0, 10);
		if (roadDist > 20)
		{
			num2 += roadDist - 20;
		}
		if (num2 >= 100)
		{
			num2 = 100;
		}
		Mathf.Min(num2 / 10, 4);
		EClass.rnd(3 + Mathf.Min(num2 / 10, 4));
		Debug.Log(string.Concat(new string[]
		{
			this.x.ToString(),
			"/",
			this.z.ToString(),
			" road dist:",
			roadDist.ToString(),
			" homeDist:",
			num.ToString(),
			" dangerLv:",
			num2.ToString()
		}));
		this.dangerLv = num2 + 5;
	}

	public int dangerLv;

	public EloMap.TileInfo tile;

	public BiomeProfile biome;
}
