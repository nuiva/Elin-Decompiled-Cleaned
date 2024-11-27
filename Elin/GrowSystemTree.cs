using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GrowSystemTree : GrowSystem
{
	public override RenderData RenderHarvest
	{
		get
		{
			return EClass.core.refs.renderers.objL_harvest;
		}
	}

	public override int Step
	{
		get
		{
			return 3;
		}
	}

	public override int StageLength
	{
		get
		{
			return 6;
		}
	}

	public override bool BlockPass(Cell cell)
	{
		return cell.objVal / 30 > 1;
	}

	public override bool BlockSight(Cell cell)
	{
		return cell.objVal / 30 > 2;
	}

	public override float MtpProgress
	{
		get
		{
			if (this.Bud || this.Baby)
			{
				return 0.5f;
			}
			if (this.Grown)
			{
				return 2f;
			}
			if (!this.Mature)
			{
				return 1f;
			}
			return 3f;
		}
	}

	public override int HarvestStage
	{
		get
		{
			return 4;
		}
	}

	protected override bool DrawHarvestOnTop
	{
		get
		{
			return true;
		}
	}

	public override bool IsMature
	{
		get
		{
			return this.Grown || this.Mature;
		}
	}

	public override bool IsTree
	{
		get
		{
			return true;
		}
	}

	public override int ShadowStage
	{
		get
		{
			return 0;
		}
	}

	public bool Bud
	{
		get
		{
			return base.stage.idx == 0;
		}
	}

	public bool Baby
	{
		get
		{
			return base.stage.idx == 1;
		}
	}

	public bool Young
	{
		get
		{
			return base.stage.idx == 2;
		}
	}

	public bool Grown
	{
		get
		{
			return base.stage.idx == 3;
		}
	}

	public bool Mature
	{
		get
		{
			return base.stage.idx == 4;
		}
	}

	public bool Withered
	{
		get
		{
			return base.stage.idx == 5;
		}
	}

	public bool IsPalulu
	{
		get
		{
			return this.source.id == 17;
		}
	}

	public bool IsBamboo
	{
		get
		{
			return this.source.id == 103;
		}
	}

	protected override bool UseGenericFirstStageTile
	{
		get
		{
			return false;
		}
	}

	public override int GetShadow(int index)
	{
		if (this.source.id == 103)
		{
			return this.source.pref.shadow;
		}
		return GrowSystemTree.shadows[index];
	}

	public override void SetGenericFirstStageTile(GrowSystem.Stage s)
	{
		s.renderData = EClass.core.refs.renderers.objS_flat;
		s.SetTile(0, new int[]
		{
			103
		});
	}

	public override void SetStageTile(GrowSystem.Stage s)
	{
		s.renderData = this.source.renderData;
		s.SetTile(s.idx + ((s.idx != 0) ? -1 : 0), this.baseTiles);
	}

	public override void OnHitFail(Chara c)
	{
		if (c == null)
		{
			return;
		}
		if (this.Young || this.Grown || this.Mature || this.Withered)
		{
			Chara chara = CharaGen.Create("putty", -1);
			EClass._zone.AddCard(chara, GrowSystem.cell.GetPoint().GetNearestPoint(false, false, true, false));
			chara.renderer.PlayAnime(AnimeID.Fall, true);
			c.Say("mobFromTree", chara, null, null);
		}
	}

	public override int GetHp()
	{
		if (base.stage.idx > this.HarvestStage)
		{
			return (int)((float)base.GetHp() * 1.5f);
		}
		return (int)((float)base.GetHp() * 0.5f);
	}

	public int GetPlantBonus(PlantData plant)
	{
		if (plant == null || plant.seed == null)
		{
			return 0;
		}
		int num = plant.seed.encLV / 10;
		if (EClass.rnd(10) > plant.seed.encLV % 10)
		{
			num++;
		}
		return Mathf.Min(EClass.rnd(num + 1), 4);
	}

	public override void OnMineObj(Chara c = null)
	{
		GrowSystemTree.<>c__DisplayClass45_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		Point point = GrowSystem.cell.GetPoint();
		int id = GrowSystem.cell.matObj_fixed.id;
		CS$<>8__locals1.plant = EClass._map.TryGetPlant(GrowSystem.cell);
		if (this.Bud || this.Baby)
		{
			if (EClass.rnd(2) == 0 || this.IsPalulu)
			{
				base.TryPick(GrowSystem.cell, this.IsPalulu ? "leaf_palulu" : "leaf", -1, 1, false);
			}
			if (this.IsBamboo)
			{
				base.PopHarvest(c, "bamboo_shoot", -1);
			}
			base.TryPick(GrowSystem.cell, "branch", id, 1 + this.<OnMineObj>g__PlantBonus|45_0(ref CS$<>8__locals1), false);
			return;
		}
		if (this.Young)
		{
			if (this.IsBamboo)
			{
				base.PopHarvest(c, "bamboo_shoot", -1);
			}
			base.TryPick(GrowSystem.cell, "branch", id, 2 + this.<OnMineObj>g__PlantBonus|45_0(ref CS$<>8__locals1), false);
			if (this.IsPalulu)
			{
				base.TryPick(GrowSystem.cell, "leaf_palulu", -1, 1, false);
			}
			base.TryPick(GrowSystem.cell, "log", id, EClass.rnd(2) + this.<OnMineObj>g__PlantBonus|45_0(ref CS$<>8__locals1) / 2, true);
			return;
		}
		if (this.Grown || this.Mature)
		{
			base.TryPick(GrowSystem.cell, this.IsPalulu ? "leaf_palulu" : "bark", -1, 1 + this.<OnMineObj>g__PlantBonus|45_0(ref CS$<>8__locals1) / 2, false);
			base.TryPick(GrowSystem.cell, "log", id, 1 + EClass.rnd(3) + this.<OnMineObj>g__PlantBonus|45_0(ref CS$<>8__locals1), true);
			base.TryPick(GrowSystem.cell, "resin", -1, 1 + this.<OnMineObj>g__PlantBonus|45_0(ref CS$<>8__locals1) / 2, false);
			if (!EClass.player.isAutoFarming)
			{
				if (EClass.rnd(500) == 0)
				{
					base.TryPick(GrowSystem.cell, "throw_putit", -1, 1, false);
				}
				if (point.IsFarmField)
				{
					base.TryPick(GrowSystem.cell, TraitSeed.MakeSeed(point.sourceObj, CS$<>8__locals1.plant), c, false);
					return;
				}
			}
		}
		else
		{
			base.TryPick(GrowSystem.cell, "bark", -1, EClass.rnd(3) + this.<OnMineObj>g__PlantBonus|45_0(ref CS$<>8__locals1), false);
			base.TryPick(GrowSystem.cell, "resin", -1, 1 + this.<OnMineObj>g__PlantBonus|45_0(ref CS$<>8__locals1) / 2, false);
			if (!EClass.player.isAutoFarming)
			{
				base.TryPick(GrowSystem.cell, TraitSeed.MakeSeed(GrowSystem.cell.sourceObj, CS$<>8__locals1.plant).SetNum(1 + EClass.rnd(3)), c, false);
				if (EClass.rnd(100) == 0)
				{
					base.TryPick(GrowSystem.cell, "throw_putit", -1, 1, false);
				}
			}
		}
	}

	[CompilerGenerated]
	private int <OnMineObj>g__PlantBonus|45_0(ref GrowSystemTree.<>c__DisplayClass45_0 A_1)
	{
		return this.GetPlantBonus(A_1.plant);
	}

	public static int[] shadows = new int[]
	{
		31,
		31,
		32,
		33,
		34,
		1,
		1,
		1
	};
}
