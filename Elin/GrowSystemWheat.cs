using System;

public class GrowSystemWheat : GrowSystemCrop
{
	protected override bool UseGenericFirstStageTile
	{
		get
		{
			return false;
		}
	}

	public override int HarvestStage
	{
		get
		{
			return 3;
		}
	}

	public override int AutoMineStage
	{
		get
		{
			return 3;
		}
	}

	public override string GetSoundProgress()
	{
		return this.source.DefaultMaterial.GetSoundImpact(null);
	}

	public override int GetStageTile()
	{
		if (GrowSystem.currentStage.idx == this.StageLength - 1 && GrowSystem.cell.isHarvested)
		{
			return this.harvestTile + 1;
		}
		return base.GetStageTile();
	}

	public override void OnMineObj(Chara c = null)
	{
		if (base.IsWithered() || this.IsHarvestStage(base.stage.idx))
		{
			base.TryPick(GrowSystem.cell, ThingGen.Create("grass", "straw"), c, false);
			return;
		}
		base.OnMineObj(c);
	}
}
