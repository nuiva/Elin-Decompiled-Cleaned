using System;
using UnityEngine;

public class GrowSystem : EClass
{
	public bool IsLastStage()
	{
		return this.stage.idx == this.StageLength - 1;
	}

	public virtual RenderData RenderHarvest
	{
		get
		{
			return this.source.renderData;
		}
	}

	public virtual int Step
	{
		get
		{
			return 5;
		}
	}

	public virtual int StageLength
	{
		get
		{
			return 5;
		}
	}

	public virtual int DefaultStage
	{
		get
		{
			return Mathf.Clamp(this.StageLength - 2, 0, this.StageLength - 1);
		}
	}

	public virtual int HarvestStage
	{
		get
		{
			return -1;
		}
	}

	public virtual int AutoMineStage
	{
		get
		{
			return this.HarvestStage;
		}
	}

	protected virtual int HaltStage
	{
		get
		{
			return this.HarvestStage;
		}
	}

	protected virtual bool IsHarvestStage(int idx)
	{
		return idx == this.HarvestStage;
	}

	protected virtual bool DrawHarvestOnTop
	{
		get
		{
			return false;
		}
	}

	protected virtual bool UseGenericFirstStageTile
	{
		get
		{
			return true;
		}
	}

	protected virtual bool WitherOnLastStage
	{
		get
		{
			return true;
		}
	}

	protected virtual bool CanRegrow
	{
		get
		{
			return true;
		}
	}

	public virtual int ShadowStage
	{
		get
		{
			return 1;
		}
	}

	public virtual bool IsTree
	{
		get
		{
			return false;
		}
	}

	public virtual bool IsCrimeToHarvest
	{
		get
		{
			return false;
		}
	}

	public virtual float MtpProgress
	{
		get
		{
			return 1f;
		}
	}

	public virtual bool NeedSunlight
	{
		get
		{
			return true;
		}
	}

	public virtual bool BlockSight(Cell cell)
	{
		return false;
	}

	public virtual bool BlockPass(Cell cell)
	{
		return false;
	}

	public virtual string GetSoundProgress()
	{
		return "Material/grass";
	}

	public virtual AnimeID AnimeProgress
	{
		get
		{
			return AnimeID.HitObj;
		}
	}

	public GrowSystem.Stage stage
	{
		get
		{
			return this.stages[(int)(GrowSystem.cell.objVal / 30)];
		}
	}

	public virtual GrowSystem.Stage CreateStage()
	{
		return new GrowSystem.Stage();
	}

	public void Init(SourceObj.Row _row)
	{
		this.source = _row;
		this.stages = new GrowSystem.Stage[this.StageLength];
		string[] growth = _row._growth;
		string[] array = growth[1].Split('/', StringSplitOptions.None);
		this.baseTiles = new int[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			this.baseTiles[i] = array[i].ToInt();
		}
		if (growth.Length > 2)
		{
			this.harvestTile = this.RenderHarvest.ConvertTile(growth[2].ToInt());
			this.idHarvestThing = growth[3];
		}
		for (int j = 0; j < this.StageLength; j++)
		{
			GrowSystem.Stage stage = this.CreateStage();
			this.stages[j] = stage;
			stage.idx = j;
			if (j == 0 && this.UseGenericFirstStageTile)
			{
				this.SetGenericFirstStageTile(stage);
			}
			else
			{
				this.SetStageTile(stage);
			}
			if (this.harvestTile != 0 && this.IsHarvestStage(j))
			{
				stage.harvest = true;
			}
		}
	}

	public virtual void SetGenericFirstStageTile(GrowSystem.Stage s)
	{
		s.renderData = EClass.core.refs.renderers.objS_flat;
		s.SetTile(0, new int[]
		{
			100,
			101,
			102
		});
	}

	public virtual void SetStageTile(GrowSystem.Stage s)
	{
		s.renderData = this.source.renderData;
		s.SetTile(s.idx + (this.UseGenericFirstStageTile ? -1 : 0), this.baseTiles);
	}

	public virtual int GetStageTile()
	{
		return GrowSystem.currentStage.tiles[GrowSystem.cell.objDir % GrowSystem.currentStage.tiles.Length];
	}

	public virtual int GetShadow(int index)
	{
		return -1;
	}

	public virtual void OnRenderTileMap(RenderParam p)
	{
		int num = (int)(GrowSystem.cell.objVal / 30);
		GrowSystem.currentStage = this.stages[num];
		SourceObj.Row row = this.source;
		float y = p.y;
		float z = p.z;
		if (num != 0 || !this.UseGenericFirstStageTile)
		{
			p.y += row.pref.y;
			p.z += row.pref.z;
		}
		if (GrowSystem.currentStage.harvest)
		{
			if (GrowSystem.cell.isHarvested || this.harvestTile == 0)
			{
				p.tile = (float)((this.afterHarvestTile != 0) ? this.afterHarvestTile : this.GetStageTile());
				GrowSystem.currentStage.renderData.Draw(p);
			}
			else
			{
				if (this.DrawHarvestOnTop)
				{
					p.tile = (float)this.GetStageTile();
					GrowSystem.currentStage.renderData.Draw(p);
					p.liquidLv = 0;
				}
				p.tile = (float)this.harvestTile;
				this.RenderHarvest.Draw(p);
			}
		}
		else
		{
			p.tile = (float)this.GetStageTile();
			GrowSystem.currentStage.renderData.Draw(p);
		}
		p.y = y;
		p.z = z;
		if (num < this.ShadowStage)
		{
			return;
		}
		int shadow = this.GetShadow(num);
		if (shadow == -1)
		{
			shadow = row.pref.shadow;
		}
		if (shadow > 1 && !GrowSystem.cell.ignoreObjShadow)
		{
			EClass.screen.tileMap.passShadow.AddShadow(p.x + row.renderData.offsetShadow.x, p.y + row.renderData.offsetShadow.y, p.z + row.renderData.offsetShadow.z, ShadowData.Instance.items[shadow], row.pref, 0, p.snow);
		}
	}

	public void SetRandomStage()
	{
		int num = Rand.rnd(this.stages.Length);
		if (num == 0 && EClass.rnd(5) != 0 && this.stages.Length >= 2)
		{
			num++;
		}
		else if (num == this.stages.Length - 1 && EClass.rnd(5) != 0 && this.stages.Length >= 2)
		{
			num--;
		}
		GrowSystem.cell.objVal = (byte)(num * 30);
	}

	public void SetDefaultStage()
	{
		GrowSystem.cell.objVal = (byte)(this.DefaultStage * 30);
	}

	public bool CanGrow(VirtualDate date)
	{
		if (this.source.id == 0)
		{
			return false;
		}
		if (GrowSystem.cell.HasBlock && !GrowSystem.cell.sourceBlock.tileType.IsFence)
		{
			return false;
		}
		if (this.NeedSunlight)
		{
			if (date.sunMap == null)
			{
				date.BuildSunMap();
			}
			if ((GrowSystem.cell.HasRoof || date.IsWinter || EClass._map.IsIndoor) && !date.sunMap.Contains(GrowSystem.cell.index))
			{
				return false;
			}
		}
		return true;
	}

	public void TryGrow(VirtualDate date)
	{
		if (!this.CanGrow(date))
		{
			return;
		}
		this.Grow(1);
	}

	public void EqualizePlants(Point pos)
	{
		PlantData p1 = EClass._map.TryGetPlant(pos);
		if (p1 == null || p1.seed == null)
		{
			return;
		}
		pos.ForeachNeighbor(delegate(Point pos2)
		{
			PlantData plantData = EClass._map.TryGetPlant(pos2);
			Thing thing = (plantData != null) ? plantData.seed : null;
			if (thing == null)
			{
				Thing installed = pos2.Installed;
				if (installed != null && installed.id == p1.seed.id)
				{
					thing = installed;
				}
			}
			if (thing == null)
			{
				return;
			}
			if (p1.seed.refVal != thing.refVal)
			{
				return;
			}
			if (thing.encLV >= p1.seed.encLV)
			{
				p1.seed = thing;
			}
		}, false);
	}

	public void Grow(int mtp = 1)
	{
		bool flag = GrowSystem.cell.isWatered || (GrowSystem.cell.IsTopWater && this.source.tag.Contains("flood"));
		PlantData plantData = EClass._map.TryGetPlant(GrowSystem.cell);
		if (plantData != null && flag)
		{
			plantData.water++;
		}
		int num = this.Step * mtp * (flag ? 2 : 1);
		int num2 = (int)(GrowSystem.cell.objVal / 30);
		int num3 = ((int)GrowSystem.cell.objVal + num) / 30;
		if (num2 != num3)
		{
			if (EClass.player.isAutoFarming && num2 == this.HarvestStage && this.CanHarvest())
			{
				this.PopHarvest(null, null, -1);
			}
			if (EClass.player.isAutoFarming && num2 >= this.AutoMineStage)
			{
				Point point = GrowSystem.cell.GetPoint();
				this.EqualizePlants(point);
				Thing thing = this.TryPopSeed(null);
				this.PopMineObj(null);
				if (thing != null)
				{
					point.SetObj(0, 1, 0);
					EClass._zone.AddCard(thing, point).Install();
				}
			}
			else if (num2 == this.StageLength - 1)
			{
				if (EClass.player.isAutoFarming)
				{
					Point point2 = GrowSystem.cell.GetPoint();
					this.EqualizePlants(point2);
					Thing thing2 = this.TryPopSeed(null);
					if (thing2 != null)
					{
						point2.SetObj(0, 1, 0);
						EClass._zone.AddCard(thing2, point2).Install();
					}
				}
				else
				{
					this.OnExceedLastStage();
				}
			}
			else
			{
				this.OnReachNextStage();
			}
		}
		else
		{
			Cell cell = GrowSystem.cell;
			cell.objVal += (byte)num;
		}
		GrowSystem.cell.Refresh();
		EClass._map.RefreshFOV((int)GrowSystem.cell.x, (int)GrowSystem.cell.z, 6, false);
	}

	public virtual void OnReachNextStage()
	{
		this.SetStage((int)(GrowSystem.cell.objVal / 30 + 1), false);
	}

	public virtual void OnExceedLastStage()
	{
		GrowSystem.cell.objVal = (byte)(this.stages.Length * 30 - 1);
		if (!this.CanRegrow)
		{
			return;
		}
		if (Rand.rnd(2) == 0)
		{
			Point point = GrowSystem.cell.GetPoint();
			if (point.IsFarmField)
			{
				return;
			}
			if (Rand.rnd(2) == 0)
			{
				Point randomNeighbor = point.GetRandomNeighbor();
				if (randomNeighbor.cell.CanGrowWeed)
				{
					randomNeighbor.SetObj((int)GrowSystem.cell.obj, 1, 0);
					point.SetObj(0, 1, 0);
					return;
				}
			}
			GrowSystem.cell.objVal = 0;
			GrowSystem.cell.isHarvested = false;
			GrowSystem.cell.isWatered = GrowSystem.cell.HasLiquid;
		}
	}

	public void SetStage(int idx, bool renewHarvest = false)
	{
		GrowSystem.cell.objVal = (byte)(idx * 30);
		GrowSystem.cell.gatherCount = 0;
		if (GrowSystem.cell.HasLiquid)
		{
			GrowSystem.cell.isWatered = true;
		}
		if (renewHarvest)
		{
			GrowSystem.cell.isHarvested = true;
		}
	}

	public bool HaltGrowth()
	{
		return this.stage.idx == this.HarvestStage;
	}

	public virtual bool IsMature
	{
		get
		{
			return false;
		}
	}

	public void Perish()
	{
		if (EClass.player.isAutoFarming)
		{
			if (this.CanHarvest())
			{
				this.PopHarvest(null, null, -1);
			}
			Thing thing = this.TryPopSeed(null);
			if (thing != null)
			{
				EClass._zone.AddCard(thing, GrowSystem.cell.GetPoint()).Install();
			}
			EClass._map.SetObj((int)GrowSystem.cell.x, (int)GrowSystem.cell.z, 0, 1, 0);
			return;
		}
		if (this.WitherOnLastStage && this.stage.idx == this.StageLength - 2)
		{
			this.SetStage(this.StageLength - 1, false);
			return;
		}
		if (EClass.rnd(3) == 0)
		{
			Thing thing2 = this.TryPopSeed(null);
			if (thing2 != null)
			{
				EClass._zone.AddCard(thing2, GrowSystem.cell.GetPoint()).Install();
			}
		}
		EClass._map.SetObj((int)GrowSystem.cell.x, (int)GrowSystem.cell.z, 0, 1, 0);
	}

	public bool IsWithered()
	{
		return this.WitherOnLastStage && this.stage.idx == this.StageLength - 1;
	}

	public bool CanHarvest()
	{
		return this.stage.harvest && !GrowSystem.cell.isHarvested;
	}

	public virtual bool CanReapSeed()
	{
		return this.CanHarvest();
	}

	public virtual void OnHit(Chara c)
	{
	}

	public virtual void OnHitFail(Chara c)
	{
	}

	public void OnProgressComplete(Chara c)
	{
		EClass._map.MineObj(GrowSystem.cell.GetPoint(), null, c);
	}

	public void PopMineObj(Chara c = null)
	{
		this.OnMineObj(c);
		this.TryPopSeed(c);
	}

	public virtual void OnMineObj(Chara c = null)
	{
		if (!this.IsWithered())
		{
			int num = GrowSystem.cell.sourceObj.components.Length - 1;
			Thing t = ThingGen.Create(GrowSystem.cell.sourceObj.components[num].Split('/', StringSplitOptions.None)[0], GrowSystem.cell.matObj_fixed.alias);
			this.TryPick(GrowSystem.cell, t, c, false);
		}
	}

	public void TryPick(Cell cell, Thing t, Chara c, bool applySeed = false)
	{
		if (applySeed)
		{
			this.ApplySeed(t);
		}
		if (EClass.player.isAutoFarming)
		{
			if (EClass.rnd(4) == 0)
			{
				return;
			}
			if (t.Num > 1)
			{
				t.SetNum(t.Num / 2);
			}
			else if (EClass.rnd(2) == 0)
			{
				return;
			}
			if (!EClass._zone.TryAddThingInSharedContainer(t, null, true, false, null, true))
			{
				EClass.game.cards.container_shipping.AddCard(t);
				return;
			}
		}
		else
		{
			EClass._map.TrySmoothPick(cell, t, c);
		}
	}

	public void TryPick(Cell cell, string idThing, int idMat = -1, int num = 1, bool applySeed = false)
	{
		if (num <= 0)
		{
			return;
		}
		this.TryPick(cell, ThingGen.Create(idThing, idMat, -1).SetNum(num), EClass.pc, applySeed);
	}

	public Thing TryPopSeed(Chara c)
	{
		if (this.source.HasTag(CTAG.seed))
		{
			int num = GrowSystem.cell.IsFarmField ? 400 : 1000;
			if (EClass._zone.IsPCFaction)
			{
				int soilCost = EClass._zone.GetSoilCost();
				int maxSoil = EClass.Branch.MaxSoil;
				if (soilCost > maxSoil)
				{
					num += (soilCost - maxSoil) * 10;
				}
			}
			if (this.IsWithered())
			{
				num /= 5;
			}
			if (EClass.player.isAutoFarming || EClass.rnd(num) < EClass.rnd(this.source.chance))
			{
				Thing thing = TraitSeed.MakeSeed(this.source, EClass._map.TryGetPlant(GrowSystem.cell));
				if (EClass.player.isAutoFarming)
				{
					return thing;
				}
				this.TryPick(GrowSystem.cell, thing, c, false);
				return thing;
			}
		}
		return null;
	}

	public void Harvest(Chara c)
	{
		GrowSystem.cell.isHarvested = true;
		GrowSystem.cell.gatherCount = 0;
		this.PopHarvest(c, null, -1);
	}

	public void PopHarvest(Chara c, string idThing, int num = -1)
	{
		this.PopHarvest(c, ThingGen.Create(idThing, -1, -1), num);
	}

	public void PopHarvest(Chara c, Thing t = null, int num = -1)
	{
		if (t == null)
		{
			if (this.idHarvestThing.StartsWith('#'))
			{
				t = ThingGen.CreateFromCategory(this.idHarvestThing.Replace("#", ""), -1);
			}
			else
			{
				t = ThingGen.Create(this.idHarvestThing.IsEmpty("apple"), -1, -1);
			}
		}
		this.ApplySeed(t);
		PlantData plantData = EClass._map.TryGetPlant(GrowSystem.cell);
		if (plantData != null && plantData.size > 0)
		{
			t.c_weight = t.SelfWeight * (80 + plantData.size * plantData.size * 100) / 100;
			t.SetBool(115, true);
			t.isWeightChanged = true;
		}
		t.SetBlessedState(BlessedState.Normal);
		if (this.source._growth.Length > 4)
		{
			int num2 = EClass.rnd(this.source._growth[4].ToInt()) + 1;
			if (EClass._zone.IsPCFaction)
			{
				int soilCost = EClass._zone.GetSoilCost();
				int maxSoil = EClass.Branch.MaxSoil;
				if (soilCost > maxSoil && EClass.player.stats.days >= 5)
				{
					num2 -= EClass.rnd(2 + (soilCost - maxSoil) / 20);
				}
			}
			if (num2 <= 0)
			{
				if (!EClass.player.isAutoFarming)
				{
					c.Say("cropSpoiled", c, GrowSystem.cell.GetObjName(), null);
				}
				return;
			}
			t.SetNum(num2);
		}
		else
		{
			Debug.Log("harvest count not set:" + this.source.id.ToString() + "/" + this.source.alias);
		}
		if (num != -1)
		{
			t.SetNum(num);
		}
		if (c == null || EClass.player.isAutoFarming)
		{
			this.TryPick(GrowSystem.cell, t, c, false);
			return;
		}
		c.Pick(t, true, true);
	}

	public void ApplySeed(Thing t)
	{
		PlantData plantData = EClass._map.TryGetPlant(GrowSystem.cell);
		Thing thing = (plantData != null) ? plantData.seed : null;
		if (EClass._zone.IsUserZone)
		{
			thing = null;
		}
		if (thing != null)
		{
			int encLv = thing.encLV / 10 + ((thing.encLV > 0) ? 1 : 0);
			foreach (Element element in thing.elements.dict.Values)
			{
				if ((!element.IsFoodTrait || t.IsFood) && (element.IsFoodTrait || element.id == 2))
				{
					t.elements.ModBase(element.id, element.Value / 10 * 10);
				}
			}
			t.SetEncLv(encLv);
			t.c_refText = thing.c_refText;
			t.isCrafted = true;
		}
	}

	public virtual int GetHp()
	{
		return this.source.hp;
	}

	public static SourceObj.Row[] sourceSnowTree = new SourceObj.Row[]
	{
		EClass.sources.objs.map[54],
		EClass.sources.objs.map[55]
	};

	public const int DivStage = 30;

	public static Cell cell;

	public static GrowSystem.Stage currentStage;

	public SourceObj.Row source;

	public GrowSystem.Stage[] stages;

	public int[] baseTiles;

	public int harvestTile;

	public int afterHarvestTile;

	public string idHarvestThing;

	public class Stage
	{
		public void SetTile(int _tile)
		{
			this.tiles = new int[this.Convert(_tile)];
		}

		public void SetTile(int tileIdx, params int[] _tiles)
		{
			this.tiles = new int[_tiles.Length];
			for (int i = 0; i < this.tiles.Length; i++)
			{
				this.tiles[i] = this.Convert(_tiles[i] + tileIdx);
			}
		}

		public int Convert(int tile)
		{
			return this.renderData.ConvertTile(tile);
		}

		public int[] tiles;

		public int idx;

		public int tileEx;

		public RenderData renderData;

		public bool harvest;
	}
}
