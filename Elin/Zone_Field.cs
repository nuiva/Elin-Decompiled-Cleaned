using System;

public class Zone_Field : Zone
{
	public override bool WillAutoSave
	{
		get
		{
			return false;
		}
	}

	public bool IsBridge
	{
		get
		{
			return base.lv == 0 && base.Tile.IsBridge;
		}
	}

	public override bool HasLaw
	{
		get
		{
			return this.IsBridge;
		}
	}

	public override float ChanceSpawnNeutral
	{
		get
		{
			if (!this.IsBridge)
			{
				return base.ChanceSpawnNeutral;
			}
			return 0.5f;
		}
	}

	public override bool UseFog
	{
		get
		{
			return base.lv <= 0;
		}
	}

	public override ZoneFeatureType FeatureType
	{
		get
		{
			return ZoneFeatureType.RandomField;
		}
	}

	public override int DangerLvFix
	{
		get
		{
			return base.Tile.source.dangerLv;
		}
	}

	public override string IdBiome
	{
		get
		{
			return EClass._map.config.idBiome.IsEmpty(base.Tile.source.idBiome.IsEmpty("Plain"));
		}
	}

	public override float PrespawnRate
	{
		get
		{
			return 1.2f;
		}
	}

	public override bool isClaimable
	{
		get
		{
			return EClass.pc.homeBranch != null;
		}
	}

	public override bool CanFastTravel
	{
		get
		{
			return base.IsPCFaction;
		}
	}

	public override string idExport
	{
		get
		{
			if (!this.IsBridge)
			{
				return base.idExport;
			}
			return "bridge";
		}
	}

	public override float BigDaddyChance
	{
		get
		{
			return 0.02f;
		}
	}

	public override float EvolvedChance
	{
		get
		{
			return 0.05f;
		}
	}

	public override float OreChance
	{
		get
		{
			return 5f;
		}
	}

	public override float ShrineChance
	{
		get
		{
			if (!EClass.debug.test)
			{
				return 0.04f;
			}
			return 1f;
		}
	}

	public override string IDBaseLandFeat
	{
		get
		{
			if (!base.isBeach)
			{
				return base.IDBaseLandFeat;
			}
			return "bfBeach";
		}
	}

	public override void OnGenerateMap()
	{
		base.OnGenerateMap();
		if (base.lv == 0)
		{
			base.idPrefix = EClass.sources.zoneAffixes.rows.RandomItem<SourceZoneAffix.Row>().id;
			base.dateExpire = EClass.world.date.GetRaw(0) + 10080;
		}
		bool draw = EClass.pc.HasCondition<ConDrawBacker>();
		base.ApplyBackerPet(draw);
		this.map.AddBackerTree(draw);
		if (this.IsBridge)
		{
			EClass._map.ForeachCell(delegate(Cell c)
			{
				c.isSeen = true;
			});
		}
	}
}
