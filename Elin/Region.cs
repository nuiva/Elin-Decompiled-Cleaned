using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class Region : Zone
{
	public override bool WillAutoSave
	{
		get
		{
			return false;
		}
	}

	public override ActionMode DefaultActionMode
	{
		get
		{
			return ActionMode.Region;
		}
	}

	public override bool IsRegion
	{
		get
		{
			return true;
		}
	}

	public override int DangerLv
	{
		get
		{
			return 1;
		}
	}

	public override bool BlockBorderExit
	{
		get
		{
			return true;
		}
	}

	public override Point RegionPos
	{
		get
		{
			return this._regionPos.Set(EClass.pc.pos);
		}
	}

	public override void OnActivate()
	{
		this.children.ForeachReverse(delegate(Spatial _z)
		{
			Zone zone = _z as Zone;
			if (zone.CanDestroy())
			{
				zone.Destroy();
			}
		});
		this.children.ForeachReverse(delegate(Spatial _z)
		{
			if (_z.destryoed)
			{
				this.children.Remove(_z);
			}
		});
		this.children.ForEach(delegate(Spatial a)
		{
			Zone zone = a as Zone;
			if (zone.IsInstance || (!zone.IsPCFaction && zone is Zone_Field) || zone is Zone_SisterHouse)
			{
				return;
			}
			this.elomap.SetZone(zone.x, zone.y, zone, false);
		});
		this.CheckRandomSites();
	}

	public void CheckRandomSites()
	{
		if (EClass.world.date.IsExpired(this.dateCheckSites))
		{
			this.dateCheckSites = EClass.world.date.GetRaw(0) + 1440;
			this.UpdateRandomSites();
		}
		if (base.FindZone("foxtown_nefu") == null)
		{
			SpatialGen.Create("foxtown_nefu", this, true, -99999, -99999, 0);
		}
		if (base.FindZone("little_garden") == null)
		{
			SpatialGen.Create("little_garden", this, true, -99999, -99999, 0);
		}
		this.elomap.objmap.UpdateMeshImmediate();
	}

	public void RenewRandomSites()
	{
		this.dateCheckSites = 0;
		Msg.Say("renewNefia");
		Debug.Log(this.ListRandomSites().Count);
		foreach (Zone zone in this.ListRandomSites())
		{
			zone.dateExpire = 1;
		}
	}

	public void UpdateRandomSites()
	{
		List<Zone> list = this.ListRandomSites();
		int num = 50 - list.Count;
		if (num <= 0)
		{
			return;
		}
		for (int i = 0; i < num; i++)
		{
			this.CreateRandomSite(this.GetRandomPoint(), null, false, 0);
		}
	}

	public void InitElomap()
	{
		EClass.scene.elomapActor.Initialize(this.elomap);
	}

	public Zone CreateRandomSite(Zone center, int radius = 8, string idSource = null, bool updateMesh = true, int lv = 0)
	{
		this.InitElomap();
		return this.CreateRandomSite(this.GetRandomPoint(center.IsRegion ? (EClass.pc.pos.x + EClass.scene.elomap.minX) : center.x, center.IsRegion ? (EClass.pc.pos.z + EClass.scene.elomap.minY) : center.y, radius, false), idSource, updateMesh, lv);
	}

	private Zone CreateRandomSite(Point pos, string idSource, bool updateMesh, int lv = 0)
	{
		if (pos == null)
		{
			return null;
		}
		if (idSource.IsEmpty())
		{
			idSource = this.GetRandomSiteSource().id;
		}
		Zone zone = SpatialGen.Create(idSource, this, true, pos.x, pos.z, 0) as Zone;
		if (lv <= 0)
		{
			if (EClass.player.CountKeyItem("license_adv") == 0 && !EClass.debug.enable)
			{
				lv = ((EClass.rnd(3) == 0) ? EClass.pc.LV : EClass.pc.FameLv) * (75 + EClass.rnd(50)) / 100 + EClass.rnd(EClass.rnd(10) + 1) - 3;
				if (lv >= 50)
				{
					lv = EClass.rndHalf(50);
				}
			}
			else
			{
				lv = EClass.pc.FameLv * 100 / (100 + EClass.rnd(50)) + EClass.rnd(EClass.rnd(10) + 1) - 3;
				if (EClass.rnd(10) == 0)
				{
					lv = lv * 3 / 2;
				}
				if (EClass.rnd(10) == 0)
				{
					lv /= 2;
				}
			}
		}
		zone._dangerLv = Mathf.Max(1, lv);
		zone.isRandomSite = true;
		zone.dateExpire = EClass.world.date.GetRaw(0) + 10080;
		if (this.elomap.IsSnow(zone.x, zone.y))
		{
			Zone zone2 = zone;
			int icon = zone2.icon;
			zone2.icon = icon + 1;
		}
		this.elomap.SetZone(zone.x, zone.y, zone, false);
		if (updateMesh)
		{
			this.elomap.objmap.UpdateMeshImmediate();
		}
		return zone;
	}

	public SourceZone.Row GetRandomSiteSource()
	{
		return (from a in EClass.sources.zones.rows
		where a.tag.Contains("random") && (EClass.debug.enable || !a.tag.Contains("debug"))
		select a).ToList<SourceZone.Row>().RandomItemWeighted((SourceZone.Row a) => (float)a.chance);
	}

	public Point GetRandomPoint()
	{
		Point point = new Point();
		for (int i = 0; i < 1000; i++)
		{
			point = this.map.bounds.GetRandomPoint();
			point.x += this.elomap.minX;
			point.z += this.elomap.minY;
			if (this.elomap.CanBuildSite(point.x, point.z, 1, ElomapSiteType.Nefia))
			{
				return point;
			}
		}
		return null;
	}

	public Point GetRandomPoint(int orgX, int orgY, int radius = 8, bool increaseRadius = false)
	{
		Point point = new Point();
		for (int i = 0; i < 1000; i++)
		{
			point.x = orgX + Rand.Range(-radius / 2, radius / 2);
			point.z = orgY + Rand.Range(-radius / 2, radius / 2);
			if (i % 100 == 0 && increaseRadius)
			{
				radius++;
			}
			if (this.elomap.CanBuildSite(point.x, point.z, 0, ElomapSiteType.Nefia))
			{
				return point;
			}
		}
		return null;
	}

	public Point GetRandomPoint(int orgX, int orgY, int minRadius, int maxRadius)
	{
		Point point = new Point();
		Point p = new Point(orgX, orgY);
		for (int i = 0; i < 1000; i++)
		{
			point.x = orgX + Rand.Range(-maxRadius, maxRadius);
			point.z = orgY + Rand.Range(-maxRadius, maxRadius);
			if (point.Distance(p) >= minRadius && !point.IsBlocked && this.elomap.CanBuildSite(point.x + this.elomap.minX, point.z + this.elomap.minY, 0, ElomapSiteType.Mob))
			{
				return point;
			}
		}
		return null;
	}

	public bool CanCreateZone(Point pos)
	{
		EClass.scene.elomapActor.Initialize(EClass.world.region.elomap);
		EloMap.TileInfo tileInfo = EClass.scene.elomapActor.elomap.GetTileInfo(pos.x, pos.z);
		return !tileInfo.idZoneProfile.IsEmpty() && !tileInfo.blocked;
	}

	public Zone CreateZone(Point pos)
	{
		return SpatialGen.Create("field", this, true, pos.x, pos.z, 0) as Zone;
	}

	public List<Zone> ListTowns()
	{
		List<Zone> list = new List<Zone>();
		foreach (Spatial spatial in EClass.game.spatials.map.Values)
		{
			if (spatial.CanSpawnAdv)
			{
				list.Add(spatial as Zone);
			}
		}
		return list;
	}

	public Zone GetRandomTown()
	{
		List<Zone> source = this.ListTowns();
		Zone zone = null;
		for (int i = 0; i < 5; i++)
		{
			zone = source.RandomItem<Zone>();
			Zone_SubTown zone_SubTown = zone as Zone_SubTown;
		}
		return zone;
	}

	public List<Zone> ListRandomSites()
	{
		return (from Zone a in this.children
		where a.isRandomSite
		select a).ToList<Zone>();
	}

	public List<Zone> ListZonesInRadius(Zone center, int radius = 10)
	{
		List<Zone> list = new List<Zone>();
		foreach (Zone zone in EClass.game.spatials.Zones)
		{
			if (zone.Dist(center) <= radius && zone != center && !(zone.source.parent != base.source.id))
			{
				list.Add(zone);
			}
		}
		return list;
	}

	public List<Zone> ListTravelZones(int radius = 100)
	{
		bool isRegion = EClass.pc.currentZone.IsRegion;
		List<Zone> list = new List<Zone>();
		if (!isRegion)
		{
			new Point(EClass.pc.currentZone.x - EClass.scene.elomap.minX, EClass.pc.currentZone.y - EClass.scene.elomap.minY);
		}
		else
		{
			Point pos = EClass.pc.pos;
		}
		foreach (Zone zone in EClass.game.spatials.Zones)
		{
			if (zone.CanFastTravel && !zone.IsInstance && (zone.isKnown || EClass.debug.returnAnywhere) && zone.parent == this)
			{
				list.Add(zone);
				zone.tempDist = zone.Dist(EClass.pc.pos);
			}
		}
		return list;
	}

	public override void OnAdvanceHour()
	{
		if (EClass.world.date.hour == 0)
		{
			foreach (Chara chara in this.ListMobs())
			{
				if (chara.Dist(EClass.pc) > 20 && EClass.rnd(2) == 0)
				{
					chara.Destroy();
				}
			}
		}
		if (this.ListMobs().Count < 6 && EClass.rnd(3) == 0)
		{
			Point randomPoint = this.GetRandomPoint(EClass.pc.pos.x, EClass.pc.pos.z, 8, 14);
			if (randomPoint != null)
			{
				RegionPoint regionPoint = new RegionPoint(EClass.pc.pos);
				BiomeProfile biome = regionPoint.biome;
				SpawnList list;
				if (biome.spawn.chara.Count > 0)
				{
					list = SpawnList.Get(biome.spawn.GetRandomCharaId(), null, null);
				}
				else
				{
					list = SpawnList.Get(biome.name, "chara", new CharaFilter
					{
						ShouldPass = ((SourceChara.Row s) => s.biome == biome.name || s.biome.IsEmpty())
					});
				}
				Chara chara2 = CharaGen.CreateFromFilter(list, regionPoint.dangerLv, -1);
				if (chara2 != null)
				{
					base.AddCard(chara2, randomPoint);
				}
			}
		}
	}

	public List<Chara> ListMobs()
	{
		List<Chara> list = new List<Chara>();
		foreach (Chara chara in EClass._map.charas)
		{
			if (!chara.IsPCFactionOrMinion && !chara.IsGlobal)
			{
				list.Add(chara);
			}
		}
		return list;
	}

	public EloMap elomap = new EloMap();

	[JsonProperty]
	public int dateCheckSites;
}
